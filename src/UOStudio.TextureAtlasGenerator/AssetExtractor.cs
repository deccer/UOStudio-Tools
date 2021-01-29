﻿using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.TextureAtlasGenerator.Abstractions;
using UOStudio.TextureAtlasGenerator.Contracts;
using UOStudio.TextureAtlasGenerator.Ultima;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class AssetExtractor : IAssetExtractor
    {
        private readonly ILogger _logger;
        private readonly IHashCalculator _hashCalculator;
        private readonly IUltimaArtProvider _ultimaArtProvider;

        private readonly string _ultimaOnlinePath;

        public AssetExtractor(
            ILogger logger,
            IHashCalculator hashCalculator,
            IConfiguration configuration,
            IUltimaArtProvider ultimaArtProvider)
        {
            _logger = logger;
            _hashCalculator = hashCalculator;
            _ultimaArtProvider = ultimaArtProvider;

            _ultimaOnlinePath = configuration["UltimaOnlinePath"];
        }

        public IReadOnlyCollection<TextureAsset> ExtractAssets()
        {
            _ultimaArtProvider.InitializeFiles(_ultimaOnlinePath);

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
                var artRaw = tileType == TileType.Item
                    ? _ultimaArtProvider.GetRawStatic(i)
                    : _ultimaArtProvider.GetRawLand(i);
                if (artRaw == null)
                {
                    //_logger.Debug("{@TileType} {@TileId} could not be extracted and will be skipped.", tileType, i);
                    continue;
                }

                var artHash = _hashCalculator.CalculateHash(artRaw);
                var art = tileType == TileType.Item
                    ? _ultimaArtProvider.GetStatic(i)
                    : _ultimaArtProvider.GetLand(i);
                assets.Add(new TextureAsset(i, tileType, artHash, art));
            }

            return assets;
        }
    }
}
