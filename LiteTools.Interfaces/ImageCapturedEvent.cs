using System;
using System.Drawing;

namespace LiteTools.Interfaces
{
    // ========================================================================
    // 3. EVENTOS DE CAPTURA E IMAGEM
    // ========================================================================

    /// <summary>
    /// Evento disparado quando uma imagem é capturada e finalizada.
    /// Distribuído para os plugins interessados (ex: LiteFlow).
    /// </summary>
    public class ImageCapturedEvent
    {
        public Bitmap CapturedImage { get; }
        public string StepId { get; }
        public DateTime Timestamp { get; }

        public ImageCapturedEvent(Bitmap image, string stepId = null)
        {
            CapturedImage = image;
            StepId = stepId;
            Timestamp = DateTime.Now;
        }
    }
}