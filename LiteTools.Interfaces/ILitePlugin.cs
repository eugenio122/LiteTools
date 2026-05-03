using System.Windows.Forms;

namespace LiteTools.Interfaces
{
    // ========================================================================
    // 1. INTERFACES PRINCIPAIS (O DNA DO ECOSSISTEMA)
    // ========================================================================

    /// <summary>
    /// Contrato base que toda DLL (LiteShot, LiteFlow, LiteJson, LiteAutomation) deve implementar 
    /// para ser reconhecida e carregada pelo LiteTools Host.
    /// </summary>
    public interface ILitePlugin
    {
        string Name { get; }
        string Version { get; }

        // O LiteTools agora passa o Contexto Global, o Barramento de Eventos e o Idioma na inicialização
        void Initialize(ILiteHostContext hostContext, IEventBus eventBus, string currentLanguage);

        // O LiteTools chama isto para alojar as configurações do plugin na nave-mãe
        UserControl GetSettingsUI();

        void Shutdown();
    }
}