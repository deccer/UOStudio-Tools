using System.Drawing;
using System.IO;

namespace TexturePacker
{
    internal struct BitmapInfo
    {
        public BitmapInfo(string fileName, Image image)
        {
            TileId = int.Parse(Path.GetFileNameWithoutExtension(fileName));
            Width = image.Width;
            Height = image.Height;
            Circumference = 2 * (Width + Height);
            Orientation = Height > Width ? Orientation.Portrait :
                Height == Width ? Orientation.Square : Orientation.Landscape;
        }

        public int TileId;
        public int Circumference;
        public Orientation Orientation;
        public int Width;
        public int Height;
    }
}