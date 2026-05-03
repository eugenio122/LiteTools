namespace LiteTools.Interfaces
{
    // ========================================================================
    // 1. INTERFACES PRINCIPAIS (O DNA DO ECOSSISTEMA)
    // ========================================================================

    /// <summary>
    /// Fornece acesso a dados globais e estado da sessão da Nave-mãe.
    /// Permite que um plugin pergunte dados gerados por outro em tempo real.
    /// </summary>
    public interface ILiteHostContext
    {
        void SetSessionMetadata(string key, object value);
        object GetSessionMetadata(string key);
        bool TryGetSessionMetadata<T>(string key, out T value);
    }
}