using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TexturePacker
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BitmapInfo[] bitmapInfos;
            const string bitmapInfoFileName = @"C:\Temp\bitmapinfos.json";
            const string inputDirectory = @"C:\Temp\TexturePacker-Input";
            const string outputDirectory = @"C:\Temp\TexturePacker-Output";

            //NormalizeUoFiddlerOutput(inputDirectory); // run only once
            if (!File.Exists(bitmapInfoFileName))
            {
                var fileNames = Directory.GetFiles(inputDirectory, "*.png", SearchOption.TopDirectoryOnly);
                var fileCount = fileNames.Length;
                bitmapInfos = new BitmapInfo[fileCount];
                for (var i = 0; i < fileCount; i++)
                {
                    var fileName = fileNames[i];
                    using var bitmap = Bitmap.FromFile(fileName);

                    bitmapInfos[i] = new BitmapInfo(fileName, bitmap);
                }

                var bitmapInfosJson = JsonConvert.SerializeObject(bitmapInfos, Formatting.Indented);
                File.WriteAllText(bitmapInfoFileName, bitmapInfosJson);
            }
            else
            {
                var bitmapInfosJson = File.ReadAllText(bitmapInfoFileName);
                bitmapInfos = JsonConvert.DeserializeObject<BitmapInfo[]>(bitmapInfosJson);
            }

            CreateAtlas(bitmapInfos, Orientation.Portrait, inputDirectory, outputDirectory);
            CreateAtlas(bitmapInfos, Orientation.Landscape, inputDirectory, outputDirectory);
            CreateAtlas(bitmapInfos, Orientation.Square, inputDirectory, outputDirectory);
        }

        private static void NormalizeUoFiddlerOutput(string directory)
        {
            var fileNames = Directory.GetFiles(directory, "*.png", SearchOption.TopDirectoryOnly);
            foreach (var fileName in fileNames)
            {
                var splitFileName = Path.GetFileNameWithoutExtension(fileName)
                    .Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var staticId = int.Parse(splitFileName[1].Substring(2),
                    NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber);

                var filePath = Path.Combine(directory, $"{staticId}.png");
                File.Move(fileName, filePath);
            }
        }

        private static void CreateAtlas(
            BitmapInfo[] bitmapInfos,
            Orientation orientation,
            string inputDirectory,
            string outputDirectory)
        {
            var sw = Stopwatch.StartNew();
            var sortedInfos = bitmapInfos.Where(bi => bi.Orientation == orientation)
                .OrderByDescending(bi => bi.Circumference).ToArray();
            var atlasPages = new List<Bitmap>(16);
            var currentSpriteY = 0;
            var currentSpriteX = 0;
            var atlasPageIndex = 0;
            Graphics currentGraphics = null;
            Stack<int> firstColumn = new Stack<int>();
            var surfaceArea = 0.0f;
            for (var i = 0; i < sortedInfos.Length; i++)
            {
                if (currentSpriteX == 0 && currentSpriteY == 0)
                {
                    if (atlasPages.Count > 0)
                    {
                        currentGraphics?.Dispose();
                        atlasPages[atlasPageIndex].Save(Path.Combine(outputDirectory, $"{orientation}{atlasPageIndex}.png"));
                        atlasPageIndex++;
                    }

                    Bitmap atlasPage;
                    if (orientation == Orientation.Square)
                    {
                        atlasPage = new Bitmap(2048, 2048);
                    }
                    else if (orientation == Orientation.Landscape)
                    {
                        atlasPage = new Bitmap(4096, 2048);
                    }
                    else
                    {
                        atlasPage = new Bitmap(4096, 2048);
                    }

                    atlasPages.Add(atlasPage);
                    currentGraphics = Graphics.FromImage(atlasPage);
                }

                var spriteInfo = sortedInfos[i];
                if (currentSpriteX == 0)
                {
                    firstColumn.Push(i);
                }

                using var sprite = Bitmap.FromFile(Path.Combine(inputDirectory, $"{spriteInfo.TileId}.png"));

                currentGraphics.DrawImage(sprite, currentSpriteX, currentSpriteY);

                surfaceArea += sprite.Width * sprite.Height;

                currentSpriteX += spriteInfo.Width;
                if (currentSpriteX + spriteInfo.Width >= atlasPages[atlasPageIndex].Width)
                {
                    currentSpriteX = 0;
                    currentSpriteY += sortedInfos[firstColumn.Pop()].Height;
                }

                if (currentSpriteY + spriteInfo.Height >= atlasPages[atlasPageIndex].Height)
                {
                    currentSpriteX = 0;
                    currentSpriteY = 0;
                }
            }

            atlasPages[atlasPageIndex].Save(Path.Combine(outputDirectory, $"{orientation}{atlasPageIndex}.png"));
            sw.Stop();

            Debug.WriteLine($"Processing {sortedInfos.Length} {orientation} Sprites Done - {atlasPageIndex + 1} pages - took {sw.Elapsed.TotalSeconds}s");

            foreach (var page in atlasPages)
            {
                page.Dispose();
            }

            atlasPages.Clear();
        }
    }
}