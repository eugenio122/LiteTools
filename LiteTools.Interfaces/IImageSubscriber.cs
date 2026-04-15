using System.Drawing;

namespace LiteTools.Interfaces
{
    /// <summary>
    /// Contrato de entrada. Se um plugin implementar isto, 
    /// o Host vai enviar-lhe todas as imagens capturadas.
    /// </summary>
    public interface IImageSubscriber
    {
        void ReceiveImage(Bitmap image);
    }
}