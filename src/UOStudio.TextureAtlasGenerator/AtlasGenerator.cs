using System;
using System.IO;
using System.Linq;
using Serilog;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class AtlasGenerator : IAtlasGenerator
    {
        private readonly ILogger _logger;
        private readonly IAssetExtractor _assetExtractor;
        private readonly IAssetSorter _assetSorter;
        private readonly IAtlasPageGenerator _atlasPageGenerator;
        private readonly ITileContainer _tileContainer;

        public AtlasGenerator(
            ILogger logger,
            IAssetExtractor assetExtractor,
            IAssetSorter assetSorter,
            IAtlasPageGenerator atlasPageGenerator,
            ITileContainer tileContainer)
        {
            _assetExtractor = assetExtractor;
            _assetSorter = assetSorter;
            _atlasPageGenerator = atlasPageGenerator;
            _tileContainer = tileContainer;
            _logger = logger.ForContext<AtlasGenerator>();
        }

        public void Run()
        {
            var textureAssets = _assetExtractor.ExtractAssets();
            _logger.Information("Extracted {@Count} Assets", textureAssets.Count);

            textureAssets = _assetSorter.SortAssets(textureAssets);

            var atlasPages = _atlasPageGenerator.GeneratePages(textureAssets.ToList());
            var atlasPageNumber = 0;
            var guid = Guid.NewGuid().ToString();
            foreach (var atlasPage in atlasPages)
            {
                var fileName = Path.Combine(Path.GetTempPath(), $"{guid}-{atlasPageNumber++:00}.png");
                atlasPage.Save(fileName);
            }

            _tileContainer.Save(Path.Combine(Path.GetTempPath(), "Atlas.xx.json"));
        }
    }
}