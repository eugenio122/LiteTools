using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LiteTools.Interfaces;

namespace LiteTools.Core
{
    public class PluginLoader
    {
        public List<ILitePlugin> LoadPlugins(string folderPath, IImagePublisher publisher, string currentLanguage, List<string> disabledPlugins)
        {
            var loadedPlugins = new List<ILitePlugin>();

            if (!Directory.Exists(folderPath))
            {
                return loadedPlugins;
            }

            string[] dllFiles = Directory.GetFiles(folderPath, "*.dll");

            foreach (string file in dllFiles)
            {
                // Verifica se a DLL atual está na lista de desativadas. Se estiver, pula para a próxima!
                if (disabledPlugins != null && disabledPlugins.Contains(Path.GetFileName(file)))
                {
                    continue;
                }

                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);

                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(ILitePlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                        .ToList();

                    if (pluginTypes.Count == 0 && Path.GetFileName(file).Contains("LiteShot"))
                    {
                        MessageBox.Show($"O LiteTools encontrou o ficheiro {Path.GetFileName(file)}, mas não reconheceu a interface ILitePlugin.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    foreach (var type in pluginTypes)
                    {
                        ILitePlugin plugin = (ILitePlugin)Activator.CreateInstance(type);

                        // Injeta o Idioma no momento da inicialização
                        plugin.Initialize(publisher, currentLanguage);

                        loadedPlugins.Add(plugin);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro fatal ao carregar o plugin {Path.GetFileName(file)}:\n\n{ex.Message}", "Erro de Carregamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return loadedPlugins;
        }
    }
}