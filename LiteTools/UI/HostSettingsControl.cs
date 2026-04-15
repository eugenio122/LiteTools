using LiteTools.Core;
using LiteTools.Models;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LiteTools.UI
{
    public class HostSettingsControl : UserControl
    {
        private CheckBox chkStartWithWindows;
        private CheckBox chkShowNotifications;
        private ComboBox cmbLanguage;
        private CheckedListBox clbPlugins;
        private Button btnImport;
        private Button btnSave;

        private HostSettings _settings;

        // Evento que avisa o MainForm para recarregar tudo após salvar
        public event Action RequestReload;

        public HostSettingsControl()
        {
            _settings = HostSettings.Load();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Size = new Size(600, 450);
            this.BackColor = SystemColors.ControlLightLight;

            // Uso do LanguageManager para todos os textos
            Label lblTitle = new Label { Text = LanguageManager.GetString("HostSettingsTitle"), Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            this.Controls.Add(lblTitle);

            chkStartWithWindows = new CheckBox { Text = LanguageManager.GetString("StartWithWindows"), AutoSize = true, Location = new Point(25, 60), Width = 350 };
            this.Controls.Add(chkStartWithWindows);

            chkShowNotifications = new CheckBox { Text = LanguageManager.GetString("ShowNotifications"), AutoSize = true, Location = new Point(25, 90), Width = 350 };
            this.Controls.Add(chkShowNotifications);

            // Idioma
            Label lblLanguage = new Label { Text = LanguageManager.GetString("LanguageLabel"), AutoSize = true, Location = new Point(25, 130) };
            this.Controls.Add(lblLanguage);

            cmbLanguage = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(25, 150), Width = 150 };
            cmbLanguage.Items.AddRange(new object[] { "pt-BR", "en-US", "es-ES", "fr-FR", "de-DE", "it-IT" });
            this.Controls.Add(cmbLanguage);

            // Gestão de Plugins (Ativar / Desativar)
            Label lblPlugins = new Label { Text = LanguageManager.GetString("PluginsManagement"), AutoSize = true, Location = new Point(25, 190) };
            this.Controls.Add(lblPlugins);

            clbPlugins = new CheckedListBox { Location = new Point(25, 210), Width = 350, Height = 100, CheckOnClick = true };
            this.Controls.Add(clbPlugins);

            // Botões
            btnImport = new Button { Text = LanguageManager.GetString("ImportPlugin"), Location = new Point(25, 320), Width = 150, Height = 30 };
            btnImport.Click += BtnImport_Click;
            this.Controls.Add(btnImport);

            btnSave = new Button { Text = LanguageManager.GetString("SaveAndApply"), Location = new Point(225, 320), Width = 150, Height = 30, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void LoadData()
        {
            chkStartWithWindows.Checked = _settings.StartWithWindows;
            chkShowNotifications.Checked = _settings.ShowNotifications;

            if (cmbLanguage.Items.Contains(_settings.Language))
                cmbLanguage.SelectedItem = _settings.Language;
            else
                cmbLanguage.SelectedIndex = 0;

            clbPlugins.Items.Clear();
            string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            if (Directory.Exists(pluginsDir))
            {
                foreach (var file in Directory.GetFiles(pluginsDir, "*.dll"))
                {
                    string fileName = Path.GetFileName(file);
                    bool isEnabled = !_settings.DisabledPlugins.Contains(fileName);
                    clbPlugins.Items.Add(fileName, isEnabled);
                }
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "LiteTools Plugin (*.dll)|*.dll", Title = LanguageManager.GetString("ImportPlugin") })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
                        string destFile = Path.Combine(pluginsDir, Path.GetFileName(ofd.FileName));

                        File.Copy(ofd.FileName, destFile, true);
                        LoadData();
                        MessageBox.Show(LanguageManager.GetString("ImportSuccess"), "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _settings.StartWithWindows = chkStartWithWindows.Checked;
            _settings.ShowNotifications = chkShowNotifications.Checked;
            _settings.Language = cmbLanguage.SelectedItem.ToString();

            // Reconstrói a lista de plugins desativados
            _settings.DisabledPlugins.Clear();
            for (int i = 0; i < clbPlugins.Items.Count; i++)
            {
                if (!clbPlugins.GetItemChecked(i))
                {
                    _settings.DisabledPlugins.Add(clbPlugins.Items[i].ToString());
                }
            }

            _settings.Save();
            ApplyWindowsStartup(_settings.StartWithWindows);

            // Avisa a Nave-Mãe que tem de reiniciar e carregar as novas DLLs/Idiomas
            RequestReload?.Invoke();

            MessageBox.Show(LanguageManager.GetString("SettingsSaved"), "LiteTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

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
            catch { } // Silenciado para evitar popups se não tiver permissão de Admin
        }
    }
}