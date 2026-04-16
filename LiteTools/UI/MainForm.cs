using LiteTools.Core;
using LiteTools.Interfaces;
using LiteTools.Models;
using LiteTools.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LiteTools.UI
{
    public partial class MainForm : Form, IImagePublisher
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private EventBus _eventBus;
        private List<ILitePlugin> _activePlugins;
        private string _pluginsFolder;
        private PluginLoader _pluginLoader;
        private DateTime _lastNotificationTime = DateTime.MinValue;

        // Controlos visuais que vieram do antigo SettingsForm
        private ListBox lstMenu;
        private Panel pnlContent;

        public MainForm()
        {
            InitializeComponent();
            var forceHandle = this.Handle;

            // CARREGA O IDIOMA GLOBAL ANTES DE DESENHAR QUALQUER COISA!
            LanguageManager.CurrentLanguage = HostSettings.Load().Language;

            _pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            if (!Directory.Exists(_pluginsFolder)) Directory.CreateDirectory(_pluginsFolder);

            _eventBus = new EventBus();
            _activePlugins = new List<ILitePlugin>();
            _pluginLoader = new PluginLoader();

            // Inicializa a Interface Visual Principal
            InitializeUI();
            ConfigureSystemTray();

            // Carrega os plugins e preenche o menu lateral
            LoadPlugins();
        }

        private void InitializeUI()
        {
            this.Text = LanguageManager.GetString("AppTitle");
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(800, 500);

            this.FormClosing += MainForm_FormClosing;

            lstMenu = new ListBox { Dock = DockStyle.Left, Width = 200, Font = new Font("Segoe UI", 10), IntegralHeight = false };
            lstMenu.SelectedIndexChanged += LstMenu_SelectedIndexChanged;

            pnlContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), BackColor = SystemColors.ControlLightLight };

            this.Controls.Add(pnlContent);
            this.Controls.Add(lstMenu);
        }

        // ====================================================================
        // CICLO DE VIDA E BANDEJA DO SISTEMA
        // ====================================================================
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
        }

        private void ConfigureSystemTray()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add(LanguageManager.GetString("TrayOpenHost"), null, OnOpenSettings);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(LanguageManager.GetString("TrayOpenPlugins"), null, OnOpenPluginsFolder);
            trayMenu.Items.Add(LanguageManager.GetString("TrayReloadPlugins"), null, OnReloadPlugins);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(LanguageManager.GetString("TrayExit"), null, OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "LiteTools - QA Host";

            // Aqui você pode usar o seu novo ícone laranja quando estiver pronto!
            trayIcon.Icon = this.Icon ?? SystemIcons.Application;

            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += OnOpenSettings;
        }

        // ====================================================================
        // PUBLISHER (SÍNCRONO DE ALTA VELOCIDADE)
        // ====================================================================
        public void Publish(Bitmap image)
        {
            try
            {
                using (Bitmap clonedForPlugins = new Bitmap(image))
                {
                    foreach (var plugin in _activePlugins)
                    {
                        if (plugin is IImageSubscriber subscriber)
                            subscriber.ReceiveImage(clonedForPlugins);
                    }
                }

                if ((DateTime.Now - _lastNotificationTime).TotalSeconds > 1.5)
                {
                    _lastNotificationTime = DateTime.Now;
                    if (HostSettings.Load().ShowNotifications && this.IsHandleCreated)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            trayIcon.ShowBalloonTip(1000, "LiteTools", LanguageManager.GetString("CaptureProcessed"), ToolTipIcon.Info);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.IsHandleCreated) this.BeginInvoke(new Action(() => MessageBox.Show($"Falha ao processar:\n\n{ex.Message}", "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        // ====================================================================
        // GESTÃO DE PLUGINS E MENUS
        // ====================================================================
        private void LoadPlugins()
        {
            var settings = HostSettings.Load();

            // Passa o idioma e a lista de desativados para o Loader
            _activePlugins = _pluginLoader.LoadPlugins(_pluginsFolder, this, settings.Language, settings.DisabledPlugins);

            LoadMenu();

            if (_activePlugins.Count > 0 && settings.ShowNotifications)
            {
                trayIcon.ShowBalloonTip(3000, "LiteTools", $"{_activePlugins.Count} {LanguageManager.GetString("PluginsActivated")}", ToolTipIcon.Info);
            }
        }

        private void LoadMenu()
        {
            lstMenu.Items.Clear();
            lstMenu.Items.Add(LanguageManager.GetString("MenuGeneral"));

            foreach (var plugin in _activePlugins)
            {
                lstMenu.Items.Add(plugin.Name);
            }

            if (lstMenu.Items.Count > 0) lstMenu.SelectedIndex = 0;
        }

        private void LstMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMenu.SelectedIndex < 0) return;

            pnlContent.Controls.Clear();

            if (lstMenu.SelectedIndex == 0)
            {
                var hostSettingsUI = new HostSettingsControl();

                hostSettingsUI.RequestReload += () =>
                {
                    LanguageManager.CurrentLanguage = HostSettings.Load().Language;
                    this.Text = LanguageManager.GetString("AppTitle");

                    // Recria o menu da bandeja com os novos textos traduzidos
                    ConfigureSystemTray();

                    foreach (var plugin in _activePlugins) try { plugin.Shutdown(); } catch { }
                    _activePlugins.Clear();
                    LoadPlugins();
                };

                hostSettingsUI.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(hostSettingsUI);
                return;
            }

            var selectedPlugin = _activePlugins[lstMenu.SelectedIndex - 1];
            var pluginUI = selectedPlugin.GetSettingsUI();

            if (pluginUI != null)
            {
                pluginUI.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(pluginUI);
            }
            else
            {
                pnlContent.Controls.Add(new Label { Text = LanguageManager.GetString("NoPluginSettings"), AutoSize = true });
            }
        }


        // ====================================================================
        // AÇÕES DO TRAY MENU
        // ====================================================================
        private void OnOpenSettings(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}