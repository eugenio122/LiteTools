using LiteTools.Core;
using LiteTools.Interfaces;
using LiteTools.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LiteTools.UI
{
    /// <summary>
    /// A Nave-Mãe (Orquestrador). 
    /// Atua como Host para os plugins e Mediator para a comunicação via EventBus.
    /// Não possui regras de negócio de QA, apenas gerencia o ciclo de vida e a infraestrutura.
    /// </summary>
    public partial class MainForm : Form
    {
        // ====================================================================
        // API DO WINDOWS PARA TECLA DE ATALHO GLOBAL (PrintScreen)
        // ====================================================================
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int CAPTURE_HOTKEY_ID = 1;
        // ====================================================================

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        // Pilares da Arquitetura Mediator
        private ILiteHostContext _hostContext;
        private IEventBus _eventBus;

        private List<ILitePlugin> _activePlugins;
        private string _pluginsFolder;
        private PluginLoader _pluginLoader;
        private DateTime _lastNotificationTime = DateTime.MinValue;

        // Controles Visuais Modernizados
        private Panel pnlTopBar;
        private FlowLayoutPanel pnlNavBar;
        private Panel pnlContent;

        // Mantém a referência do botão atualmente selecionado na Navbar
        private Button _activeNavButton;

        /// <summary>
        /// Construtor principal. Inicializa o ambiente, carrega as configurações, 
        /// prepara o barramento de eventos e aciona o carregamento das DLLs.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            var forceHandle = this.Handle;

            // Define o idioma base antes de inicializar qualquer componente visual
            LanguageManager.CurrentLanguage = HostSettings.Load().Language;

            _pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            if (!Directory.Exists(_pluginsFolder)) Directory.CreateDirectory(_pluginsFolder);

            // Organização da pasta de plugins. Cria as subpastas principais vazias por padrão.
            string[] defaultPluginFolders = { "LiteFlow", "LiteJson", "LiteShot", "LiteAutomation" };
            foreach (var folder in defaultPluginFolders)
            {
                string targetFolder = Path.Combine(_pluginsFolder, folder);
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);
            }

            // 1. Inicializa o Contexto Global e o Barramento de Eventos
            _hostContext = new LiteHostContext();
            _eventBus = new EventBus();

            // 2. O Host assina os eventos apenas para dar feedback visual (balões na bandeja)
            _eventBus.Subscribe<LiteTools.Interfaces.ImageCapturedEvent>(OnImageCaptured);
            _eventBus.Subscribe<LiteTools.Interfaces.ImageCaptureCanceledEvent>(OnImageCaptureCanceled);

            _activePlugins = new List<ILitePlugin>();
            _pluginLoader = new PluginLoader();

            // 3. Constrói a UI Moderna e a Bandeja do Sistema
            InitializeUI();
            ConfigureSystemTray();

            // 4. Regista a tecla PrintScreen globalmente no Windows
            // Isso permite que o QA tire print mesmo com o LiteTools minimizado
            RegisterHotKey(this.Handle, CAPTURE_HOTKEY_ID, 0, (int)Keys.PrintScreen);

            // 5. Inicia a varredura e carregamento dos plugins
            LoadPlugins();
        }

        /// <summary>
        /// Monta a interface principal do Host com a nova Navbar fluida.
        /// </summary>
        private void InitializeUI()
        {
            var settings = HostSettings.Load();
            this.Text = LanguageManager.GetString("AppTitle") ?? "LiteTools - QA Host Platform";
            this.MinimumSize = new Size(900, 600);

            // Diretriz UX: O executável nasce com a janela maximizada
            this.WindowState = FormWindowState.Maximized;

            this.FormClosing += MainForm_FormClosing;

            Color topBarColor = settings.IsDarkMode ? Color.FromArgb(45, 45, 48) : Color.FromArgb(240, 240, 240);
            Color contentColor = settings.IsDarkMode ? Color.FromArgb(30, 30, 30) : SystemColors.ControlLightLight;

            // Painel Superior (TopBar)
            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = topBarColor, Padding = new Padding(10) };

            // A Nova Navbar 
            pnlNavBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true
            };
            pnlTopBar.Controls.Add(pnlNavBar);

            // Painel central onde a tela do plugin selecionado será ancorada
            pnlContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0), BackColor = contentColor };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlTopBar);
        }

        // ====================================================================
        // TRANSAÇÃO DE CAPTURA (COMMIT / ROLLBACK)
        // ====================================================================

        /// <summary>
        /// Inicia o fluxo seguro de captura de tela via atalho global (PrintScreen),
        /// garantindo que o motor semântico extraia a DOM antes de a tela ser congelada e escurecida.
        /// </summary>
        private void TriggerCaptureFlow()
        {
            string stepId = Guid.NewGuid().ToString();

            // 1. INÍCIO DA TRANSAÇÃO: Avisa o LiteJson para extrair a tela imediatamente (Pre-Commit)
            var liteJson = _activePlugins.FirstOrDefault(p => p.Name.Contains("LiteJson"));
            if (liteJson != null)
            {
                // Invocamos via Reflection para não criar acoplamento duro com a DLL do LiteJson
                var method = liteJson.GetType().GetMethod("PreparePendingStep");
                method?.Invoke(liteJson, new object[] { stepId });
            }

            // 2. AÇÃO VISUAL: Avisa o LiteShot para congelar a tela
            var liteShot = _activePlugins.FirstOrDefault(p => p.Name.Contains("LiteShot"));
            if (liteShot != null)
            {
                var captureMethod = liteShot.GetType().GetMethod("InitiateCapture");
                captureMethod?.Invoke(liteShot, new object[] { stepId });
            }
        }

        /// <summary>
        /// Interceptador de Mensagens do Windows. Escuta o teclado globalmente para a tecla PrintScreen.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            // Código 0x0312 representa WM_HOTKEY
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == CAPTURE_HOTKEY_ID)
            {
                TriggerCaptureFlow();
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Intercepta a tentativa de fechamento da janela. Em vez de encerrar a aplicação,
        /// minimiza a Nave-Mãe para a Bandeja do Sistema (System Tray), mantendo os hooks ativos.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                var settings = HostSettings.Load();
                if (settings.ShowNotifications)
                    trayIcon.ShowBalloonTip(1500, "LiteTools", LanguageManager.GetString("AppRunningBackground"), ToolTipIcon.Info);
            }
            else
            {
                // Limpa o registro da tecla se a app for fechada à força pelo Windows
                UnregisterHotKey(this.Handle, CAPTURE_HOTKEY_ID);
            }
        }

        /// <summary>
        /// Configura o ícone na barra de tarefas (perto do relógio) e cria o menu de contexto.
        /// </summary>
        private void ConfigureSystemTray()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add(LanguageManager.GetString("TrayOpenHost") ?? "Abrir", null, OnOpenSettings);
            trayMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem browserMenu = new ToolStripMenuItem(LanguageManager.GetString("TrayBrowserDebug") ?? "Browsers");

            var chromeItem = new ToolStripMenuItem("Google Chrome");
            chromeItem.Click += (s, e) => LaunchBrowser(BrowserLauncher.BrowserType.Chrome);

            var edgeItem = new ToolStripMenuItem("Microsoft Edge");
            edgeItem.Click += (s, e) => LaunchBrowser(BrowserLauncher.BrowserType.Edge);

            var firefoxItem = new ToolStripMenuItem("Mozilla Firefox");
            firefoxItem.Click += (s, e) => LaunchBrowser(BrowserLauncher.BrowserType.Firefox);

            browserMenu.DropDownItems.Add(chromeItem);
            browserMenu.DropDownItems.Add(edgeItem);
            browserMenu.DropDownItems.Add(firefoxItem);

            trayMenu.Items.Add(browserMenu);
            trayMenu.Items.Add(new ToolStripSeparator());

            trayMenu.Items.Add(LanguageManager.GetString("TrayOpenPlugins") ?? "Pasta", null, OnOpenPluginsFolder);
            trayMenu.Items.Add(LanguageManager.GetString("TrayReloadPlugins") ?? "Recarregar", null, OnReloadPlugins);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(LanguageManager.GetString("TrayExit") ?? "Sair", null, OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "LiteTools - QA Host";
            trayIcon.Icon = this.Icon ?? SystemIcons.Application;
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += OnOpenSettings;
        }

        private void LaunchBrowser(BrowserLauncher.BrowserType browser)
        {
            try { BrowserLauncher.Launch(browser); }
            catch (FileNotFoundException ex) { MessageBox.Show(ex.Message, "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (Exception ex) { MessageBox.Show($"Erro ao lançar navegador:\n{ex.Message}", "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void OnImageCaptured(LiteTools.Interfaces.ImageCapturedEvent evt)
        {
            var settings = HostSettings.Load();

            // LÓGICA DO FOCO OPCIONAL
            if (settings.ShowHostAfterCapture && this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.Show();
                    this.WindowState = FormWindowState.Maximized;
                    this.BringToFront();
                    this.Activate(); // Força o foco do Windows
                }));
            }

            // LÓGICA DO BALÃO DE NOTIFICAÇÃO 
            if ((DateTime.Now - _lastNotificationTime).TotalSeconds > 1.5)
            {
                _lastNotificationTime = DateTime.Now;
                if (settings.ShowNotifications && this.IsHandleCreated)
                {
                    this.BeginInvoke(new Action(() => { trayIcon.ShowBalloonTip(1000, "LiteTools", LanguageManager.GetString("CaptureProcessed"), ToolTipIcon.Info); }));
                }
            }
        }

        private void OnImageCaptureCanceled(LiteTools.Interfaces.ImageCaptureCanceledEvent evt)
        {
            if ((DateTime.Now - _lastNotificationTime).TotalSeconds > 1.5)
            {
                _lastNotificationTime = DateTime.Now;
                if (HostSettings.Load().ShowNotifications && this.IsHandleCreated)
                {
                    this.BeginInvoke(new Action(() => { trayIcon.ShowBalloonTip(1000, "LiteTools", LanguageManager.GetString("CaptureCanceled"), ToolTipIcon.Warning); }));
                }
            }
        }

        /// <summary>
        /// Solicita ao PluginLoader que leia a pasta de plugins e injete as dependências.
        /// </summary>
        private void LoadPlugins()
        {
            var settings = HostSettings.Load();
            _activePlugins = _pluginLoader.LoadPlugins(_pluginsFolder, _hostContext, _eventBus, settings.Language, settings.DisabledPlugins);

            // Constrói a barra de navegação baseada nos plugins encontrados
            LoadNavbar();

            // Após carregar o ecossistema, o Host publica o estado global do tema
            _eventBus.Publish(new ThemeChangedEvent(settings.IsDarkMode));
        }

        // ====================================================================
        // CONSTRUÇÃO DINÂMICA DA NAVBAR
        // ====================================================================

        /// <summary>
        /// Renderiza os botões (abas) na barra superior substituindo o antigo ComboBox.
        /// </summary>
        private void LoadNavbar()
        {
            pnlNavBar.Controls.Clear();
            var settings = HostSettings.Load();
            bool isDark = settings.IsDarkMode;

            Color btnHoverColor = isDark ? Color.FromArgb(60, 60, 65) : Color.FromArgb(220, 220, 220);
            Color separatorColor = isDark ? Color.FromArgb(80, 80, 80) : Color.LightGray;
            Color textNormalColor = isDark ? Color.Gray : Color.DimGray;

            // Função local para padronizar a criação dos botões
            Button CreateNavButton(string text, object tag, bool isFirst = false)
            {
                Button btn = new Button
                {
                    Text = text,
                    Tag = tag, // Armazena a referência ("HOST_SETTINGS" ou a instância do Plugin)
                    Height = 40,
                    AutoSize = true,
                    MinimumSize = new Size(120, 40),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = textNormalColor,
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    Cursor = Cursors.Hand,
                    Margin = new Padding(isFirst ? 0 : 5, 0, 5, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = btnHoverColor;
                btn.FlatAppearance.MouseDownBackColor = btnHoverColor;
                btn.Click += NavButton_Click;
                return btn;
            }

            // 1. Botão "Geral (LiteTools)"
            var btnHost = CreateNavButton(LanguageManager.GetString("MenuGeneral") ?? "LiteTools", "HOST_SETTINGS", true);
            pnlNavBar.Controls.Add(btnHost);

            // 2. Separador Vertical
            if (_activePlugins.Count > 0)
            {
                Panel separator = new Panel { Width = 2, Height = 25, BackColor = separatorColor, Margin = new Padding(10, 8, 10, 0) };
                pnlNavBar.Controls.Add(separator);
            }

            // 3. Botões dos Plugins (Sanitizados e Ordenados)
            var preferredOrder = new List<string> { "LiteFlow", "LiteAutomation", "LiteJson", "LiteShot" };
            var sortedPlugins = _activePlugins.OrderBy(p =>
            {
                var match = preferredOrder.FirstOrDefault(order => p.Name.Contains(order));
                return match != null ? preferredOrder.IndexOf(match) : 999;
            }).ToList();

            Button liteFlowBtn = null;

            foreach (var plugin in sortedPlugins)
            {
                string displayName = plugin.Name;
                foreach (var standardName in preferredOrder)
                {
                    if (plugin.Name.Contains(standardName))
                    {
                        displayName = standardName;
                        break;
                    }
                }

                var btnPlugin = CreateNavButton(displayName, plugin);
                pnlNavBar.Controls.Add(btnPlugin);

                if (displayName == "LiteFlow") liteFlowBtn = btnPlugin;
            }

            // 4. Seleciona a aba LiteFlow nativamente ao abrir o programa chamando o Evento diretamente
            // Isso resolve o problema do PerformClick() que falha quando o botão não está visível no ecrã (ainda no construtor)
            if (liteFlowBtn != null)
                NavButton_Click(liteFlowBtn, EventArgs.Empty);
            else
                NavButton_Click(btnHost, EventArgs.Empty);
        }

        /// <summary>
        /// Disparado quando o utilizador clica em um botão da Navbar.
        /// Alterna o painel de configurações correspondente no painel central.
        /// </summary>
        private void NavButton_Click(object sender, EventArgs e)
        {
            Button clickedBtn = sender as Button;
            if (clickedBtn == null || clickedBtn == _activeNavButton) return;

            var settings = HostSettings.Load();
            bool isDark = settings.IsDarkMode;

            // Restaura o estilo visual (fonte normal) do botão anteriormente selecionado
            if (_activeNavButton != null)
            {
                _activeNavButton.Font = new Font("Segoe UI", 11, FontStyle.Regular);
                _activeNavButton.ForeColor = isDark ? Color.Gray : Color.DimGray;
            }

            // Aplica estilo "Ativo" (negrito) ao botão que o utilizador acabou de clicar
            _activeNavButton = clickedBtn;
            _activeNavButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            _activeNavButton.ForeColor = isDark ? Color.White : Color.Black;

            // Limpa o palco central
            pnlContent.Controls.Clear();

            // Renderiza Configurações da Nave-Mãe
            if (clickedBtn.Tag?.ToString() == "HOST_SETTINGS")
            {
                var hostSettingsUI = new HostSettingsControl();

                // Se o QA salvar configurações que afetam o Host (Idioma, Ativar/Desativar DLLs)
                hostSettingsUI.RequestReload += () =>
                {
                    LanguageManager.CurrentLanguage = HostSettings.Load().Language;
                    this.Text = LanguageManager.GetString("AppTitle");
                    ConfigureSystemTray();

                    foreach (var plugin in _activePlugins) try { plugin.Shutdown(); } catch { }
                    _activePlugins.Clear();
                    LoadPlugins();
                };

                hostSettingsUI.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(hostSettingsUI);
            }
            // Renderiza Configurações do Plugin Específico
            else if (clickedBtn.Tag is ILitePlugin plugin)
            {
                var pluginUI = plugin.GetSettingsUI();
                if (pluginUI != null)
                {
                    pluginUI.Dock = DockStyle.Fill;
                    pnlContent.Controls.Add(pluginUI);
                }
                else
                {
                    // Caso a ferramenta não tenha uma UI desenvolvida (Ex: LiteJson opera 100% invisível)
                    pnlContent.Controls.Add(new Label
                    {
                        Text = LanguageManager.GetString("NoPluginSettings") ?? "Sem configurações.",
                        AutoSize = true,
                        Location = new Point(20, 20),
                        ForeColor = isDark ? Color.White : Color.Black
                    });
                }
            }
        }

        // ====================================================================
        // EVENTOS DA BANDEJA DO SISTEMA (TRAY ICON)
        // ====================================================================
        private void OnOpenSettings(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
        }

        private void OnOpenPluginsFolder(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", _pluginsFolder);
        }

        private void OnReloadPlugins(object sender, EventArgs e)
        {
            LanguageManager.CurrentLanguage = HostSettings.Load().Language;
            foreach (var plugin in _activePlugins) try { plugin.Shutdown(); } catch { }
            _activePlugins.Clear();
            LoadPlugins();
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            foreach (var plugin in _activePlugins) try { plugin.Shutdown(); } catch { }
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e) { }
    }
}