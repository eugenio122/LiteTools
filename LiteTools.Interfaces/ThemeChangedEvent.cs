namespace LiteTools.Interfaces
{
    // ========================================================================
    // 2. EVENTOS DE SISTEMA E AMBIENTE
    // ========================================================================

    /// <summary>
    /// Evento disparado pelo Host quando o utilizador altera o tema global.
    /// </summary>
    public class ThemeChangedEvent
    {
        public bool IsDarkMode { get; }
        public ThemeChangedEvent(bool isDarkMode) { IsDarkMode = isDarkMode; }
    }
}