using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace LiteTools.Core
{
    /// <summary>
    /// Módulo responsável por localizar e lançar navegadores nativos do sistema
    /// com a porta de depuração (9222) aberta para o protocolo CDP/WebDriver BiDi.
    /// Suporta arquitetura offline-first (não faz download, usa o instalado).
    /// </summary>
    public static class BrowserLauncher
    {
        public enum BrowserType
        {
            Chrome,
            Edge,
            Firefox
        }

        /// <summary>
        /// Lança o navegador especificado com um perfil persistente focado em automação/debug.
        /// </summary>
        public static void Launch(BrowserType browser)
        {
            string exePath = GetBrowserExecutablePath(browser);

            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            {
                throw new FileNotFoundException($"{browser} não foi encontrado neste sistema.");
            }

            // DIRETRIZ: Usa a LocalAppData para garantir persistência do perfil do utilizador.
            // Isso evita que o SDET perca configurações (ex: Preserve Log do DevTools) a cada reinicialização.
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string profileDir = Path.Combine(localAppData, "LiteSuiteBrowserProfile", browser.ToString());

            // Apenas cria o diretório se for a primeira vez. NÃO deletamos mais a pasta.
            if (!Directory.Exists(profileDir))
            {
                Directory.CreateDirectory(profileDir);
            }

            string args = string.Empty;

            if (browser == BrowserType.Chrome || browser == BrowserType.Edge)
            {
                // Flags padrão para Chromium:
                // --remote-debugging-port=9222: Abre a porta CDP para o LiteJson/LiteAutomation
                // --user-data-dir: Força o uso do perfil isolado e persistente que definimos
                // --no-first-run --no-default-browser-check: Ignora telas de boas-vindas da Microsoft/Google
                // --disable-sync: Evita sincronização de contas acidental
                args = $"--remote-debugging-port=9222 --user-data-dir=\"{profileDir}\" --no-first-run --no-default-browser-check --disable-sync";
            }
            else if (browser == BrowserType.Firefox)
            {
                // Firefox moderno suporta CDP via remote-debugging-port
                // O argumento de perfil no Firefox usa um único traço (-profile)
                args = $"--remote-debugging-port=9222 -profile \"{profileDir}\" -no-remote";
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                UseShellExecute = true
            };

            Process.Start(processInfo);
        }

        /// <summary>
        /// Busca o caminho do executável do navegador inspecionando o Registro do Windows (App Paths).
        /// Busca primeiro em LocalMachine (instalação global) e faz fallback para CurrentUser.
        /// </summary>
        private static string GetBrowserExecutablePath(BrowserType browser)
        {
            string registryPath = browser switch
            {
                BrowserType.Chrome => @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe",
                BrowserType.Edge => @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe",
                BrowserType.Firefox => @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe",
                _ => string.Empty
            };

            // Tenta procurar na máquina local (Instalação para todos os usuários)
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                if (key?.GetValue("") is string path && !string.IsNullOrEmpty(path)) return path;
            }

            // Tenta procurar no usuário atual (Instalação local/per-user)
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (key?.GetValue("") is string path && !string.IsNullOrEmpty(path)) return path;
            }

            return null;
        }
    }
}