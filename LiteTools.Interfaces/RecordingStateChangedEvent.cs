namespace LiteTools.Interfaces
{
    // ========================================================================
    // 2. EVENTOS DE SISTEMA E AMBIENTE
    // ========================================================================

    /// <summary>
    /// Evento disparado pelo LiteFlow quando o utilizador pausa/retoma a gravação (Botão Captando).
    /// O LiteJson usa isso como "freio" para evitar capturar ruído fora do teste.
    /// </summary>
    public class RecordingStateChangedEvent
    {
        public bool IsRecording { get; }
        public RecordingStateChangedEvent(bool isRecording) { IsRecording = isRecording; }
    }
}