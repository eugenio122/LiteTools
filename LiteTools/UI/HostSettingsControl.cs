using LiteTools.Core;
using LiteTools.Models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LiteTools.UI
{
    /// <summary>
    /// Painel visual (UserControl) de configurações gerais da Nave-Mãe.
    /// É renderizado no painel central (pnlContent) do MainForm quando a opção "Geral (LiteTools)" 
    /// é selecionada no Dropdown de navegação.
    /// </summary>
    public class HostSettingsControl : UserControl
    {
        // Controles de Interface
        private CheckBox chkStartWithWindows;
        private CheckBox chkShowNotifications;
        private CheckBox chkShowHostAfterCapture;
        private CheckBox chkDarkMode;
        private ComboBox cmbLanguage;
        private CheckedListBox clbPlugins;
        private Button btnImport;
        private Button btnSave;

        // Referência para o modelo de dados de configurações
        private HostSettings _settings;

        /// <summary>
        /// Evento disparado quando o utilizador clica em "Salvar e Aplicar".
        /// O MainForm escuta este evento para saber quando deve reiniciar a interface e os plugins.
        /// </summary>
        public event Action RequestReload;

        public HostSettingsControl()
        {
            _settings = HostSettings.Load();
            InitializeUI();
            LoadData();
        }

        /// <summary>
        /// Constrói a interface de utilizador programaticamente.
        /// Organizada em blocos lógicos: Configurações Gerais, Idioma, Gestão de Plugins e Lançador de Browsers.
        /// </summary>
        private void InitializeUI()
        {
            this.BackColor = _settings.IsDarkMode ? Color.FromArgb(30, 30, 30) : SystemColors.ControlLightLight;
            Color textColor = _settings.IsDarkMode ? Color.White : Color.Black;
            Color comboBackColor = _settings.IsDarkMode ? Color.FromArgb(45, 45, 48) : SystemColors.Window;

            // 1. O CORAÇÃO RESPONSIVO: TableLayoutPanel cria uma "grelha" que se adapta a qualquer resolução.
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Preenche todo o painel, não importa o tamanho
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(30) // Margem interna global para respirar
            };

            // Define a proporção das colunas (60% para a lista de plugins, 40% para os navegadores)
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            // Define como as linhas vão se comportar (A linha 4 vai esticar para preencher o vazio)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 0: Título
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 1: Checkboxes
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 2: Idioma
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 3: Labels
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 4: Lista e Botões (ESTICA AQUI)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 5: Botões de Ação
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 6: Rodapé

            // ================= ROW 0: Título =================
            Label lblTitle = new Label { Text = LanguageManager.GetString("HostSettingsTitle"), Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, ForeColor = textColor, Margin = new Padding(0, 0, 0, 20) };
            mainLayout.Controls.Add(lblTitle, 0, 0);
            mainLayout.SetColumnSpan(lblTitle, 2);

            // ================= ROW 1: Checkboxes (FlowLayout agrupa itens um embaixo do outro) =================
            FlowLayoutPanel pnlChecks = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Dock = DockStyle.Fill, WrapContents = false };
            chkStartWithWindows = new CheckBox { Text = LanguageManager.GetString("StartWithWindows"), AutoSize = true, ForeColor = textColor, Margin = new Padding(0, 0, 0, 10), Cursor = Cursors.Hand };
            chkShowNotifications = new CheckBox { Text = LanguageManager.GetString("ShowNotifications"), AutoSize = true, ForeColor = textColor, Margin = new Padding(0, 0, 0, 10), Cursor = Cursors.Hand };
            chkShowHostAfterCapture = new CheckBox { Text = LanguageManager.GetString("ShowHostAfterCapture"), AutoSize = true, ForeColor = textColor, Margin = new Padding(0, 0, 0, 10), Cursor = Cursors.Hand };
            chkDarkMode = new CheckBox { Text = LanguageManager.GetString("DarkMode"), AutoSize = true, ForeColor = textColor, Margin = new Padding(0, 0, 0, 20), Cursor = Cursors.Hand };
            pnlChecks.Controls.AddRange(new Control[] { chkStartWithWindows, chkShowNotifications, chkShowHostAfterCapture, chkDarkMode });

            mainLayout.Controls.Add(pnlChecks, 0, 1);
            mainLayout.SetColumnSpan(pnlChecks, 2);

            // ================= ROW 2: Idioma =================
            FlowLayoutPanel pnlLang = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 30), WrapContents = false };
            Label lblLanguage = new Label { Text = LanguageManager.GetString("LanguageLabel"), AutoSize = true, ForeColor = textColor, Margin = new Padding(0, 0, 0, 5) };
            cmbLanguage = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200, BackColor = comboBackColor, ForeColor = textColor, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10) };
            cmbLanguage.Items.AddRange(new object[] { "pt-BR", "en-US", "es-ES", "fr-FR", "de-DE", "it-IT" });
            pnlLang.Controls.AddRange(new Control[] { lblLanguage, cmbLanguage });

            mainLayout.Controls.Add(pnlLang, 0, 2);
            mainLayout.SetColumnSpan(pnlLang, 2);

            // ================= ROW 3: Títulos das Listas =================
            Label lblPlugins = new Label { Text = LanguageManager.GetString("PluginsManagement"), AutoSize = true, ForeColor = textColor, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(0, 0, 0, 5) };
            Label lblBrowsers = new Label { Text = LanguageManager.GetString("TestBrowsersLabel"), AutoSize = true, ForeColor = textColor, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(0, 0, 0, 5) };
            mainLayout.Controls.Add(lblPlugins, 0, 3);
            mainLayout.Controls.Add(lblBrowsers, 1, 3);

            // ================= ROW 4: A Área que Estica (Lista Esquerda / Botões Direita) =================
            clbPlugins = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                BackColor = comboBackColor,
                ForeColor = textColor,
                Font = new Font("Segoe UI", 10),
                IntegralHeight = false, // Permite que a lista obedeça ao TableLayout em vez do tamanho da fonte
                Margin = new Padding(0, 0, 20, 20),
                BorderStyle = BorderStyle.FixedSingle
            };
            mainLayout.Controls.Add(clbPlugins, 0, 4);

            FlowLayoutPanel pnlBrowsers = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, Dock = DockStyle.Fill, WrapContents = false };
            Button btnChrome = new Button { Text = "Google Chrome", Width = 200, Height = 40, Margin = new Padding(0, 0, 0, 10), FlatStyle = FlatStyle.Flat, BackColor = comboBackColor, ForeColor = textColor, Cursor = Cursors.Hand };
            btnChrome.Click += (s, e) => LaunchBrowser(BrowserLauncher.BrowserType.Chrome);
            Button btnEdge = new Button { Text = "Microsoft Edge", Width = 200, Height = 40, Margin = new Padding(0, 0, 0, 10), FlatStyle = FlatStyle.Flat, BackColor = comboBackColor, ForeColor = textColor, Cursor = Cursors.Hand };
            btnEdge.Click += (s, e) => LaunchBrowser(BrowserLauncher.BrowserType.Edge);
            Button btnFirefox = new Button { Text = "Mozilla Firefox", Width = 200, Height = 40, Margin = new Padding(0, 0, 0, 10), FlatStyle = FlatStyle.Flat, BackColor = comboBackColor, ForeColor = textColor, Cursor = Cursors.Hand };
            btnFirefox.Click += (s, e) => LaunchBrowser(BrowserLauncher.BrowserType.Firefox);
            pnlBrowsers.Controls.AddRange(new Control[] { btnChrome, btnEdge, btnFirefox });
            mainLayout.Controls.Add(pnlBrowsers, 1, 4);

            // ================= ROW 5: Botões de Ação (Importar / Salvar) =================
            FlowLayoutPanel pnlActions = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 30) };
            btnImport = new Button { Text = LanguageManager.GetString("ImportPlugin"), Width = 180, Height = 40, Margin = new Padding(0, 0, 15, 0), FlatStyle = FlatStyle.Flat, BackColor = comboBackColor, ForeColor = textColor, Cursor = Cursors.Hand };
            btnImport.Click += BtnImport_Click;
            btnSave = new Button { Text = LanguageManager.GetString("SaveAndApply"), Width = 180, Height = 40, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            pnlActions.Controls.AddRange(new Control[] { btnImport, btnSave });

            mainLayout.Controls.Add(pnlActions, 0, 5);
            mainLayout.SetColumnSpan(pnlActions, 2);

            // ================= ROW 6: Rodapé =================
            FlowLayoutPanel pnlFooter = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Dock = DockStyle.Fill };
            Label lblVersion = new Label { Text = "LiteTools v1.1.0", AutoSize = true, ForeColor = Color.Gray };
            LinkLabel lnkGitHub = new LinkLabel { Text = LanguageManager.GetString("GitHubLink"), AutoSize = true, LinkColor = Color.FromArgb(0, 120, 215), Cursor = Cursors.Hand };
            lnkGitHub.LinkClicked += (s, e) => { try { Process.Start(new ProcessStartInfo { FileName = "https://github.com/eugenio122/LiteTools/", UseShellExecute = true }); } catch { } };
            pnlFooter.Controls.AddRange(new Control[] { lblVersion, lnkGitHub });

            mainLayout.Controls.Add(pnlFooter, 0, 6);
            mainLayout.SetColumnSpan(pnlFooter, 2);

            // Adiciona a grelha pronta ao controlo base
            this.Controls.Add(mainLayout);
        }

        /// <summary>
        /// Carrega os dados do modelo (litetools.json) e varre a pasta de plugins para 
        /// refletir o estado atual na interface (checkboxes, combobox, etc.).
        /// </summary>
        private void LoadData()
        {
            chkStartWithWindows.Checked = _settings.StartWithWindows;
            chkShowNotifications.Checked = _settings.ShowNotifications;
            chkShowHostAfterCapture.Checked = _settings.ShowHostAfterCapture;
            chkDarkMode.Checked = _settings.IsDarkMode;

            if (cmbLanguage.Items.Contains(_settings.Language))
                cmbLanguage.SelectedItem = _settings.Language;
            else
                cmbLanguage.SelectedIndex = 0;

            clbPlugins.Items.Clear();
            string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

            // Whitelist (Mostra apenas as DLLs Oficiais do Ecossistema)
            var allowedPlugins = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "LiteShot.dll", "LiteFlow.dll", "LiteJson.dll", "LiteAutomation.dll"
            };

            if (Directory.Exists(pluginsDir))
            {
                foreach (var file in Directory.GetFiles(pluginsDir, "*.dll", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileName(file);

                    // Condição: Deve estar na Whitelist e ainda não ter sido adicionado à interface
                    if (allowedPlugins.Contains(fileName) && !clbPlugins.Items.Contains(fileName))
                    {
                        bool isEnabled = !_settings.DisabledPlugins.Contains(fileName);
                        clbPlugins.Items.Add(fileName, isEnabled);
                    }
                }
            }
        }


        // ====================================================================
        // EVENTOS E REGRAS DE NEGÓCIO DA UI
        // ====================================================================

        /// <summary>
        /// Dispara a abertura do navegador persistente com a porta CDP ativa.
        /// </summary>
        private void LaunchBrowser(BrowserLauncher.BrowserType browser)
        {
            try
            {
                BrowserLauncher.Launch(browser);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao lançar navegador:\n{ex.Message}", "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Abre a janela de seleção de ficheiros para o QA importar uma nova versão de uma DLL de plugin.
        /// A DLL selecionada é copiada diretamente para a pasta "plugins".
        /// </summary>
        private void BtnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "LiteTools Plugin (*.dll)|*.dll", Title = LanguageManager.GetString("ImportPlugin") })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
                        string fileName = Path.GetFileName(ofd.FileName);

                        // DIRETRIZ: Whitelist para saber se criamos uma subpasta ou não
                        var allowedPlugins = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        {
                            "LiteShot.dll", "LiteFlow.dll", "LiteJson.dll", "LiteAutomation.dll"
                        };

                        // Por padrão, o destino é a raiz da pasta "plugins"
                        string finalDestDir = pluginsDir;

                        // Se for uma DLL oficial do ecossistema, criamos/usamos uma subpasta dedicada
                        if (allowedPlugins.Contains(fileName))
                        {
                            string targetFolderName = fileName.Replace(".dll", "", StringComparison.OrdinalIgnoreCase);
                            finalDestDir = Path.Combine(pluginsDir, targetFolderName);

                            if (!Directory.Exists(finalDestDir))
                            {
                                Directory.CreateDirectory(finalDestDir);
                            }
                        }

                        string destFile = Path.Combine(finalDestDir, fileName);

                        File.Copy(ofd.FileName, destFile, true);
                        LoadData();

                        // Verifica se foi importado para a raiz (Dependência) para dar o aviso
                        if (!allowedPlugins.Contains(fileName))
                        {
                            MessageBox.Show($"A biblioteca '{fileName}' foi copiada para a raiz da pasta de plugins.\n\nLembre-se de abrir a pasta manualmente e movê-la para dentro da subpasta do plugin que necessita dela!", "LiteTools - Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(LanguageManager.GetString("ImportSuccess"), "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show(LanguageManager.GetString("ImportInUseError"), "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(LanguageManager.GetString("ImportError") + ex.Message, "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Coleta o estado atual dos componentes da interface, salva no litetools.json,
        /// aplica regras do Windows (Regedit) e dispara o aviso de recarregamento para o MainForm.
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            _settings.StartWithWindows = chkStartWithWindows.Checked;
            _settings.ShowNotifications = chkShowNotifications.Checked;
            _settings.ShowHostAfterCapture = chkShowHostAfterCapture.Checked;
            _settings.IsDarkMode = chkDarkMode.Checked;
            _settings.Language = cmbLanguage.SelectedItem.ToString();

            // Reconstrói a lista de bloqueio com base nos itens que o utilizador desmarcou na CheckedListBox
            _settings.DisabledPlugins.Clear();
            for (int i = 0; i < clbPlugins.Items.Count; i++)
            {
                if (!clbPlugins.GetItemChecked(i))
                {
                    _settings.DisabledPlugins.Add(clbPlugins.Items[i].ToString());
                }
            }

            _settings.Save();

            // Aplica a regra de inicialização com o Windows no Regedit
            ApplyWindowsStartup(_settings.StartWithWindows);

            // Avisa o MainForm que as configurações mudaram e ele precisa fazer o reload dos plugins/idioma
            RequestReload?.Invoke();

            MessageBox.Show(LanguageManager.GetString("SettingsSaved"), "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Adiciona ou remove o executável da Nave-Mãe na chave de arranque automático do Regedit do Windows.
        /// Usa a HKCU (Current User) para não exigir privilégios de Administrador.
        /// </summary>
        private void ApplyWindowsStartup(bool enable)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (enable) key.SetValue("LiteToolsHost", Application.ExecutablePath);
                    else key.DeleteValue("LiteToolsHost", false);
                }
            }
            catch { /* Ignora falhas silenciosamente caso o Regedit esteja bloqueado por políticas de TI */ }
        }
    }
}