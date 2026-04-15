using System;
using System.IO;
using System.Text.Json;

namespace LiteTools.Models
{
    public class HostSettings
    {
        public bool StartWithWindows { get; set; } = false;
        public bool ShowNotifications { get; set; } = true;

        // O idioma global da plataforma e dos plugins
        public string Language { get; set; } = "pt-BR";

        // Lista de nomes de DLLs que o utilizador escolheu desativar
        public List<string> DisabledPlugins { get; set; } = new List<string>();

        // Ficheiro de configuração exclusivo do Host
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "litetools.json");

        public static HostSettings Load()
        {
            if (File.Exists(ConfigPath))
            {
                try { return JsonSerializer.Deserialize<HostSettings>(File.ReadAllText(ConfigPath)) ?? new HostSettings(); }
                catch { return new HostSettings(); }
            }
            return new HostSettings();
        }

        public void Save()
        {
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}