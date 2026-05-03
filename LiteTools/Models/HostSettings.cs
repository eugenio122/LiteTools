using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LiteTools.Models
{
    /// <summary>
    /// Representa as configurações persistentes da Nave-Mãe (Host).
    /// Gerencia a serialização e desserialização dos dados no arquivo litetools.json.
    /// </summary>
    public class HostSettings
    {
        /// <summary>
        /// Define se o LiteTools deve ser iniciado automaticamente com o logon do Windows.
        /// </summary>
        public bool StartWithWindows { get; set; } = false;

        /// <summary>
        /// Define se as notificações visuais em formato de balão (System Tray) estão habilitadas.
        /// </summary>
        public bool ShowNotifications { get; set; } = true;

        /// <summary>
        /// Define se o Host deve saltar para a tela após um print
        /// </summary>
        public bool ShowHostAfterCapture { get; set; } = false;

        /// <summary>
        /// Define se o tema escuro (Dark Mode) está ativado. O Host e os plugins devem consumir essa configuração
        /// </summary>
        public bool IsDarkMode { get; set; } = false;

        /// <summary>
        /// O idioma global da plataforma. O Host e os plugins devem consumir essa configuração
        /// na inicialização para traduzir suas próprias interfaces. Padrão: "pt-BR".
        /// </summary>
        public string Language { get; set; } = "pt-BR";

        /// <summary>
        /// Lista de nomes de ficheiros de plugins (ex: "LiteShot.dll") que o utilizador 
        /// escolheu desativar pela interface. O PluginLoader irá ignorar estes ficheiros.
        /// </summary>
        public List<string> DisabledPlugins { get; set; } = new List<string>();

        // Caminho fixo do ficheiro de configuração exclusivo do Host na raiz do executável.
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "litetools.json");

        /// <summary>
        /// Carrega as configurações do disco. Se o ficheiro não existir ou o JSON estiver corrompido,
        /// retorna uma nova instância segura com os valores padrão.
        /// </summary>
        /// <returns>A instância populada de HostSettings.</returns>
        public static HostSettings Load()
        {
            if (File.Exists(ConfigPath))
            {
                try
                {
                    return JsonSerializer.Deserialize<HostSettings>(File.ReadAllText(ConfigPath)) ?? new HostSettings();
                }
                catch
                {
                    // Fallback de segurança: se o arquivo estiver corrompido, assume o padrão.
                    return new HostSettings();
                }
            }
            return new HostSettings();
        }

        /// <summary>
        /// Serializa as configurações atuais e salva no disco com formatação visualmente agradável (identada).
        /// </summary>
        public void Save()
        {
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}