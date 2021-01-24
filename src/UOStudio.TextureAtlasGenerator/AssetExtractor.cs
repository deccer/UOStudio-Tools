using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.TextureAtlasGenerator.Ultima;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class AssetExtractor : IAssetExtractor
    {
        private readonly ILogger _logger;
        private readonly IHashCalculator _hashCalculator;

        private readonly string _ultimaOnlinePath;

        public AssetExtractor(
            ILogger logger,
            IHashCalculator hashCalculator,
            IConfiguration configuration)
        {
            _logger = logger;
            _hashCalculator = hashCalculator;

            _ultimaOnlinePath = configuration["UltimaOnlinePath"];
        }

        public IReadOnlyCollection<TextureAsset> ExtractAssets()
        {
            Files.Initialize(_ultimaOnlinePath);

            var assets = new List<TextureAsset>(0x20000);
            var sw = Stopwatch.StartNew();

            assets.AddRange(ExtractArt(0x4000, TileType.Land));
            assets.AddRange(ExtractArt(Art.GetMaxItemID(), TileType.Item));

            sw.Stop();
            _logger.Information("Extracting Art from {@UltimaOnlinePath}. Took {@TotalSeconds}s.",
                _ultimaOnlinePath, sw.Elapsed.TotalSeconds);

            return assets;
        }

        private IReadOnlyCollection<TextureAsset> ExtractArt(int tileCount, TileType tileType)
        {
            _logger.Information($"Extracting {tileType}-Art from {{@UltimaOnlinePath}}", _ultimaOnlinePath);
            var assets = new List<TextureAsset>(16384);
            for (var i = 0; i < tileCount; i++)
            {
                var artHash = _hashCalculator.CalculateHash(Art.GetRawStatic(i));
                assets.Add(new TextureAsset(i, tileType, artHash, Art.GetStatic(i, false)));
            }

            return assets;
        }
    }
}