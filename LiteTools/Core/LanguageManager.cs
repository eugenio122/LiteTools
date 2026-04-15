using System.Collections.Generic;

namespace LiteTools.Core
{
    /// <summary>
    /// Gerencia o sistema de internacionalização (i18n) nativo do Host (LiteTools).
    /// </summary>
    public static class LanguageManager
    {
        // O idioma atual (Será alimentado pelo HostSettings ao iniciar)
        public static string CurrentLanguage = "pt-BR";

        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            ["pt-BR"] = new()
            {
                ["HostSettingsTitle"] = "Configurações da Nave-Mãe",
                ["StartWithWindows"] = "Iniciar o LiteTools automaticamente com o Windows",
                ["ShowNotifications"] = "Exibir balões de notificação do sistema (Tray)",
                ["LanguageLabel"] = "Idioma Global:",
                ["PluginsManagement"] = "Gestão de Plugins Instalados:",
                ["ImportPlugin"] = "Importar Plugin (.dll)",
                ["SaveAndApply"] = "Salvar e Aplicar",
                ["AppTitle"] = "LiteTools - QA Host Platform",
                ["AppRunningBackground"] = "A Nave-Mãe continua a executar em segundo plano.",
                ["TrayOpenHost"] = "Abrir Nave-Mãe",
                ["TrayOpenPlugins"] = "Abrir pasta de Plugins",
                ["TrayReloadPlugins"] = "Recarregar Plugins",
                ["TrayExit"] = "Encerrar LiteTools",
                ["MenuGeneral"] = "Geral (LiteTools)",
                ["NoPluginSettings"] = "Este plugin não possui configurações.",
                ["ImportSuccess"] = "Plugin importado! Clique em 'Salvar e Aplicar' para carregar.",
                ["ImportInUseError"] = "Não é possível substituir um plugin em uso. Desative-o, salve, e tente novamente.",
                ["ImportError"] = "Erro ao importar: ",
                ["SettingsSaved"] = "Configurações do Host salvas e aplicadas com sucesso!",
                ["CaptureProcessed"] = "Captura processada com sucesso!",
                ["PluginsActivated"] = "plugin(s) ativado(s)."
            },
            ["en-US"] = new()
            {
                ["HostSettingsTitle"] = "Host Settings",
                ["StartWithWindows"] = "Start LiteTools automatically with Windows",
                ["ShowNotifications"] = "Show system tray balloon notifications",
                ["LanguageLabel"] = "Global Language:",
                ["PluginsManagement"] = "Installed Plugins Management:",
                ["ImportPlugin"] = "Import Plugin (.dll)",
                ["SaveAndApply"] = "Save and Apply",
                ["AppTitle"] = "LiteTools - QA Host Platform",
                ["AppRunningBackground"] = "The Host is still running in the background.",
                ["TrayOpenHost"] = "Open Host Platform",
                ["TrayOpenPlugins"] = "Open Plugins folder",
                ["TrayReloadPlugins"] = "Reload Plugins",
                ["TrayExit"] = "Exit LiteTools",
                ["MenuGeneral"] = "General (LiteTools)",
                ["NoPluginSettings"] = "This plugin does not have a settings interface.",
                ["ImportSuccess"] = "Plugin imported! Click 'Save and Apply' to load it.",
                ["ImportInUseError"] = "Cannot replace a plugin currently in use. Disable it, save, and try again.",
                ["ImportError"] = "Import error: ",
                ["SettingsSaved"] = "Host settings saved and applied successfully!",
                ["CaptureProcessed"] = "Capture processed successfully!",
                ["PluginsActivated"] = "plugin(s) activated."
            },
            ["es-ES"] = new()
            {
                ["HostSettingsTitle"] = "Configuraciones del Host",
                ["StartWithWindows"] = "Iniciar LiteTools automáticamente con Windows",
                ["ShowNotifications"] = "Mostrar notificaciones del sistema (Bandeja)",
                ["LanguageLabel"] = "Idioma Global:",
                ["PluginsManagement"] = "Gestión de Plugins Instalados:",
                ["ImportPlugin"] = "Importar Plugin (.dll)",
                ["SaveAndApply"] = "Guardar y Aplicar",
                ["AppTitle"] = "LiteTools - Plataforma Host QA",
                ["AppRunningBackground"] = "El Host sigue ejecutándose en segundo plano.",
                ["TrayOpenHost"] = "Abrir Plataforma Host",
                ["TrayOpenPlugins"] = "Abrir carpeta de Plugins",
                ["TrayReloadPlugins"] = "Recargar Plugins",
                ["TrayExit"] = "Cerrar LiteTools",
                ["MenuGeneral"] = "General (LiteTools)",
                ["NoPluginSettings"] = "Este plugin no tiene interfaz de configuración.",
                ["ImportSuccess"] = "¡Plugin importado! Haz clic en 'Guardar y Aplicar' para cargarlo.",
                ["ImportInUseError"] = "No se puede reemplazar un plugin en uso. Desactívalo, guarda e inténtalo de nuevo.",
                ["ImportError"] = "Error al importar: ",
                ["SettingsSaved"] = "¡Configuraciones del Host guardadas y aplicadas con éxito!",
                ["CaptureProcessed"] = "¡Captura procesada con éxito!",
                ["PluginsActivated"] = "plugin(s) activado(s)."
            },
            ["fr-FR"] = new()
            {
                ["HostSettingsTitle"] = "Paramètres de l'Hôte",
                ["StartWithWindows"] = "Démarrer LiteTools automatiquement avec Windows",
                ["ShowNotifications"] = "Afficher les notifications dans la zone de notification",
                ["LanguageLabel"] = "Langue Globale :",
                ["PluginsManagement"] = "Gestion des Plugins Installés :",
                ["ImportPlugin"] = "Importer un Plugin (.dll)",
                ["SaveAndApply"] = "Enregistrer et Appliquer",
                ["AppTitle"] = "LiteTools - Plateforme Hôte QA",
                ["AppRunningBackground"] = "L'Hôte continue de s'exécuter en arrière-plan.",
                ["TrayOpenHost"] = "Ouvrir la Plateforme Hôte",
                ["TrayOpenPlugins"] = "Ouvrir le dossier des Plugins",
                ["TrayReloadPlugins"] = "Recharger les Plugins",
                ["TrayExit"] = "Quitter LiteTools",
                ["MenuGeneral"] = "Général (LiteTools)",
                ["NoPluginSettings"] = "Ce plugin n'a pas d'interface de configuration.",
                ["ImportSuccess"] = "Plugin importé ! Cliquez sur 'Enregistrer et Appliquer' pour le charger.",
                ["ImportInUseError"] = "Impossible de remplacer un plugin en cours d'utilisation. Désactivez-le, enregistrez et réessayez.",
                ["ImportError"] = "Erreur d'importation : ",
                ["SettingsSaved"] = "Paramètres de l'hôte enregistrés et appliqués avec succès !",
                ["CaptureProcessed"] = "Capture traitée avec succès !",
                ["PluginsActivated"] = "plugin(s) activé(s)."
            },
            ["de-DE"] = new()
            {
                ["HostSettingsTitle"] = "Host-Einstellungen",
                ["StartWithWindows"] = "LiteTools automatisch mit Windows starten",
                ["ShowNotifications"] = "Benachrichtigungen im System-Tray anzeigen",
                ["LanguageLabel"] = "Globale Sprache:",
                ["PluginsManagement"] = "Verwaltung installierter Plugins:",
                ["ImportPlugin"] = "Plugin importieren (.dll)",
                ["SaveAndApply"] = "Speichern und Anwenden",
                ["AppTitle"] = "LiteTools - QA Host-Plattform",
                ["AppRunningBackground"] = "Der Host wird weiterhin im Hintergrund ausgeführt.",
                ["TrayOpenHost"] = "Host-Plattform öffnen",
                ["TrayOpenPlugins"] = "Plugin-Ordner öffnen",
                ["TrayReloadPlugins"] = "Plugins neu laden",
                ["TrayExit"] = "LiteTools beenden",
                ["MenuGeneral"] = "Allgemein (LiteTools)",
                ["NoPluginSettings"] = "Dieses Plugin hat keine Einstellungsoberfläche.",
                ["ImportSuccess"] = "Plugin importiert! Klicken Sie auf 'Speichern und Anwenden', um es zu laden.",
                ["ImportInUseError"] = "Ein verwendetes Plugin kann nicht ersetzt werden. Deaktivieren Sie es, speichern Sie und versuchen Sie es erneut.",
                ["ImportError"] = "Importfehler: ",
                ["SettingsSaved"] = "Host-Einstellungen erfolgreich gespeichert und angewendet!",
                ["CaptureProcessed"] = "Aufnahme erfolgreich verarbeitet!",
                ["PluginsActivated"] = "Plugin(s) aktiviert."
            },
            ["it-IT"] = new()
            {
                ["HostSettingsTitle"] = "Impostazioni Host",
                ["StartWithWindows"] = "Avvia LiteTools automaticamente con Windows",
                ["ShowNotifications"] = "Mostra notifiche nella barra delle applicazioni",
                ["LanguageLabel"] = "Lingua Globale:",
                ["PluginsManagement"] = "Gestione Plugin Installati:",
                ["ImportPlugin"] = "Importa Plugin (.dll)",
                ["SaveAndApply"] = "Salva e Applica",
                ["AppTitle"] = "LiteTools - Piattaforma Host QA",
                ["AppRunningBackground"] = "L'Host è ancora in esecuzione in background.",
                ["TrayOpenHost"] = "Apri Piattaforma Host",
                ["TrayOpenPlugins"] = "Apri cartella Plugin",
                ["TrayReloadPlugins"] = "Ricarica Plugin",
                ["TrayExit"] = "Esci da LiteTools",
                ["MenuGeneral"] = "Generale (LiteTools)",
                ["NoPluginSettings"] = "Questo plugin non ha un'interfaccia di configurazione.",
                ["ImportSuccess"] = "Plugin importato! Fai clic su 'Salva e Applica' per caricarlo.",
                ["ImportInUseError"] = "Impossibile sostituire un plugin in uso. Disabilitalo, salva e riprova.",
                ["ImportError"] = "Errore di importazione: ",
                ["SettingsSaved"] = "Impostazioni dell'host salvate e applicate con successo!",
                ["CaptureProcessed"] = "Cattura elaborata con successo!",
                ["PluginsActivated"] = "plugin attivato/i."
            }
        };

        /// <summary>
        /// Procura a tradução baseada na chave e no idioma atual.
        /// </summary>
        public static string GetString(string key)
        {
            if (Translations.ContainsKey(CurrentLanguage) && Translations[CurrentLanguage].ContainsKey(key))
                return Translations[CurrentLanguage][key];

            // Retorna a própria chave se não encontrar a tradução, para fácil identificação em tempo de debug
            return key;
        }
    }
}