using System;

namespace LiteTools.Interfaces
{
    // ========================================================================
    // 3. EVENTOS DE CAPTURA E IMAGEM
    // ========================================================================

    /// <summary>
    /// Evento disparado pelo LiteShot quando o utilizador cancela a captura (ex: pressiona ESC).
    /// Permite que outros plugins descartem dados pendentes na memória.
    /// </summary>
    public class ImageCaptureCanceledEvent
    {
        public DateTime Timestamp { get; }
        public ImageCaptureCanceledEvent() { Timestamp = DateTime.Now; }
    }
}