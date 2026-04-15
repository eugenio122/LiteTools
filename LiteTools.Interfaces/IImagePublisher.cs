using System.Drawing;

namespace LiteTools.Interfaces
{
    /// <summary>
    /// Contrato de comunicação de saída. 
    /// O LiteTools (Host) implementa isto e passa para os plugins.
    /// </summary>
    public interface IImagePublisher
    {
        void Publish(Bitmap image);
    }
}