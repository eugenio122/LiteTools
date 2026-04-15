using System.Windows.Forms;

namespace LiteTools.Interfaces
{
    /// <summary>
    /// Contrato base que toda DLL (LiteShot, LiteFlow, etc) deve implementar 
    /// para ser reconhecida e carregada pelo LiteTools Host.
    /// </summary>
    public interface ILitePlugin
    {
        string Name { get; }
        string Version { get; }

        // O LiteTools agora passa o idioma global selecionado para o plugin!
        void Initialize(IImagePublisher publisher, string currentLanguage);

        // O LiteTools chama isto para alojar as configurações do plugin na nave-mãe
        UserControl GetSettingsUI();

        void Shutdown();
    }
}