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
            const string namePrefix = "Test";

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

            var tileInfos = new List<Tile>();
            var atlasInfos = new List<AtlasInfo>();

            CreateAtlas(bitmapInfos, inputDirectory, outputDirectory, namePrefix, ref tileInfos, ref atlasInfos);

            var atlas = new Atlas
            {
                TileInfos = tileInfos.ToArray(),
                AtlasInfos = atlasInfos.ToArray()
            };

            var atlasJson = JsonConvert.SerializeObject(atlas, Formatting.Indented);
            File.WriteAllText(Path.Combine(outputDirectory, $"{namePrefix}-Atlas.json"), atlasJson);
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

        private static void CreateAtlas(BitmapInfo[] bitmapInfos,

            string inputDirectory,
            string outputDirectory,
            string namePrefix,
            ref List<Tile> tileInfos,
            ref List<AtlasInfo> atlasInfos)
        {
            var sw = Stopwatch.StartNew();
            var sortedInfos = bitmapInfos
                .OrderByDescending(bi => bi.Orientation)
                .ThenByDescending(bi => bi.Circumference)
                .ToArray();

            var atlasPages = new List<Bitmap>(16);
            var currentSpriteY = 0;
            var currentSpriteX = 0;
            var atlasPageIndex = 0;
            string fileName;
            Graphics currentGraphics = null;
            var firstColumn = new Stack<int>();

            for (var i = 0; i < sortedInfos.Length; i++)
            {
                if (currentSpriteX == 0 && currentSpriteY == 0)
                {
                    if (atlasPages.Count > 0)
                    {
                        currentGraphics?.Dispose();
                        fileName = Path.Combine(outputDirectory, $"{namePrefix}-{atlasPageIndex}.png");
                        atlasPages[atlasPageIndex].Save(fileName);

                        Console.WriteLine($"Creating Atlas Page {atlasPages.Count}");
                        atlasInfos.Add(new AtlasInfo
                        {
                            FileName = fileName,
                            PageNumber = atlasPageIndex
                        });

                        atlasPageIndex++;
                    }

                    var atlasPage = new Bitmap(2048, 2048);

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

                var spriteAtlasX = currentSpriteX / (float)atlasPages[atlasPageIndex].Width;
                var spriteAtlasY = currentSpriteY / (float)atlasPages[atlasPageIndex].Height;
                var spriteAtlasW = sprite.Width / (float)atlasPages[atlasPageIndex].Width;
                var spriteAtlasH = sprite.Height / (float)atlasPages[atlasPageIndex].Height;

                var tile = new Tile
                {
                    PageIndex = atlasPageIndex,
                    Id = spriteInfo.TileId,

                    U0 = spriteAtlasX,
                    V0 = spriteAtlasY,

                    U1 = spriteAtlasX + spriteAtlasW,
                    V1 = spriteAtlasY,

                    U2 = spriteAtlasX,
                    V2 = spriteAtlasY + spriteAtlasH,

                    U3 = spriteAtlasX + spriteAtlasW,
                    V3 = spriteAtlasY + spriteAtlasH
                };
                tileInfos.Add(tile);

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

            fileName = Path.Combine(outputDirectory, $"{namePrefix}-{atlasPageIndex}.png");
            atlasPages[atlasPageIndex].Save(fileName);
            atlasInfos.Add(new AtlasInfo
            {
                FileName = fileName,
                PageNumber = atlasPageIndex
            });
            sw.Stop();

            Debug.WriteLine($"Processing {sortedInfos.Length} Sprites Done - {atlasPageIndex + 1} pages - took {sw.Elapsed.TotalSeconds}s");

            foreach (var page in atlasPages)
            {
                page.Dispose();
            }

            atlasPages.Clear();
        }
    }
}