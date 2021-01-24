using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TextureAtlasExtract
{
    internal class Atlas
    {
        public AtlasInfo[] AtlasInfos { get; set; }

        public Tile[] TileInfos { get; set; }
    }

    internal class AtlasInfo
    {
        public string FileName { get; set; }

        public int PageNumber { get; set; }

        public Orientation Orientation { get; set; }
    }

    public enum Orientation
    {
        Square,
        Landscape,
        Portrait
    }

    internal class Tile
    {
        public int Id;
        public Orientation Orientation;
        public int AtlasPageIndex;
        public float U0;
        public float V0;
        public float U1;
        public float V1;
        public float U2;
        public float V2;
        public float U3;
        public float V3;
    }

    internal static class Program
    {
        public static void Main(string[] args)
        {
            const string bitmapInfoFileName = @"C:\Temp\bitmapinfos.json";
            const string inputDirectory = @"C:\Temp\TexturePacker-Input";
            const string outputDirectory = @"C:\Temp\TexturePacker-Output";
            const string namePrefix = "Test";

            var atlasJson = File.ReadAllText(Path.Combine(outputDirectory, $"{namePrefix}-Atlas.json"));
            var atlas = JsonConvert.DeserializeObject<Atlas>(atlasJson);

            var armoireTile = atlas.TileInfos.FirstOrDefault(ti => ti.Id == 0x0A4E);
            if (armoireTile == null)
            {
                return;
            }

            var armoireTexture = atlas.AtlasInfos.FirstOrDefault(a => a.Orientation == armoireTile.Orientation
                                                                      && a.PageNumber == armoireTile.AtlasPageIndex);
            if (armoireTexture == null)
            {
                return;
            }

            using var atlasTexture = Bitmap.FromFile(Path.Combine(outputDirectory, armoireTexture.FileName));

            using var armoireBitmap = new Bitmap(45, 85);
            using var armoireGraphics = Graphics.FromImage(armoireBitmap);

            var sourceRect = new Rectangle
            {
                X = (int) (armoireTile.U0 * atlasTexture.Width),
                Y = (int) (armoireTile.U0 * atlasTexture.Height),
                Width = armoireBitmap.Width,
                Height = armoireBitmap.Height
            };

            armoireGraphics.DrawImage(atlasTexture, 0, 0, sourceRect, GraphicsUnit.Pixel);
            armoireBitmap.Save(Path.Combine(outputDirectory, "Armoire.png"), ImageFormat.Png);
        }
    }
}