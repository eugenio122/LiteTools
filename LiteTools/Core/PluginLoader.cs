using LiteTools.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace LiteTools.Core
{
    /// <summary>
    /// Módulo responsável por varrer a pasta de plugins, carregar as bibliotecas (DLLs) dinamicamente 
    /// via Reflection e instanciar as ferramentas do ecossistema LiteQASuite (LiteFlow, LiteJson, etc.).
    /// </summary>
    public class PluginLoader
    {
        /// <summary>
        /// Carrega todos os plugins ativos encontrados na pasta especificada e injeta a infraestrutura do Host.
        /// </summary>
        /// <param name="folderPath">Caminho físico da pasta "plugins".</param>
        /// <param name="hostContext">O contexto de memória global (injetado nos plugins).</param>
        /// <param name="eventBus">O barramento de eventos para comunicação Pub/Sub (injetado nos plugins).</param>
        /// <param name="currentLanguage">O idioma atual da plataforma.</param>
        /// <param name="disabledPlugins">Lista de nomes de ficheiros (ex: "LiteShot.dll") que o utilizador escolheu ignorar.</param>
        /// <returns>Uma lista contendo as instâncias inicializadas e prontas para uso de todos os plugins válidos.</returns>
        public List<ILitePlugin> LoadPlugins(string folderPath, ILiteHostContext hostContext, IEventBus eventBus, string currentLanguage, List<string> disabledPlugins)
        {
            var loadedPlugins = new List<ILitePlugin>();

            if (!Directory.Exists(folderPath))
            {
                return loadedPlugins;
            }

            // Lê recursivamente todas as subpastas organizacionais (SearchOption.AllDirectories)
            string[] dllFiles = Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories);

            foreach (string file in dllFiles)
            {
                // Verifica se a DLL atual está na lista de desativadas pelo utilizador. Se estiver, salta.
                if (disabledPlugins != null && disabledPlugins.Contains(Path.GetFileName(file)))
                {
                    continue;
                }

                try
                {
                    // Carrega o binário na memória da aplicação
                    Assembly assembly = Assembly.LoadFrom(file);

                    // Busca todas as classes contidas na DLL que herdam do nosso contrato (ILitePlugin)
                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(ILitePlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                        .ToList();

                    // Aviso de segurança amigável caso a DLL pareça fazer parte do ecossistema (LiteShot), mas não implemente o contrato correto
                    if (pluginTypes.Count == 0 && Path.GetFileName(file).Contains("LiteShot"))
                    {
                        MessageBox.Show($"O LiteTools encontrou o ficheiro {Path.GetFileName(file)}, mas não reconheceu a interface ILitePlugin.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    // Instancia e inicializa cada plugin válido encontrado
                    foreach (var type in pluginTypes)
                    {
                        ILitePlugin plugin = (ILitePlugin)Activator.CreateInstance(type);

                        // INJEÇÃO DE DEPENDÊNCIA: O Host passa o Contexto e o Barramento para o Plugin viver
                        plugin.Initialize(hostContext, eventBus, currentLanguage);

                        loadedPlugins.Add(plugin);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Extrai o nome exato da DLL/dependência auxiliar que está faltando para o plugin principal funcionar!
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"O plugin '{Path.GetFileName(file)}' depende de arquivos que não foram encontrados.\n");

                    foreach (Exception loaderEx in ex.LoaderExceptions)
                    {
                        if (loaderEx != null) sb.AppendLine($"- {loaderEx.Message}");
                    }
                    MessageBox.Show(sb.ToString(), "Erro de Dependência no Plugin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Captura qualquer outro erro catastrófico para não derrubar a Nave-Mãe inteira
                    MessageBox.Show($"Erro fatal ao carregar {Path.GetFileName(file)}:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return loadedPlugins;
        }
    }
}