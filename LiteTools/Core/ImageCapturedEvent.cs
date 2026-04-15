using System;
using System.Drawing;

namespace LiteTools.Core
{
    /// <summary>
    /// Evento interno do Host. 
    /// O Host cria isso quando recebe um Bitmap do IImagePublisher 
    /// e distribui pelo EventBus para outros plugins interessados.
    /// </summary>
    public class ImageCapturedEvent
    {
        public Bitmap CapturedImage { get; }
        public DateTime Timestamp { get; }

        public ImageCapturedEvent(Bitmap image)
        {
            CapturedImage = image;
            Timestamp = DateTime.Now;
        }
    }
}