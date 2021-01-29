using System;
using System.Drawing;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Serilog;
using UOStudio.TextureAtlasGenerator.Abstractions;
using Xunit;

namespace UOStudio.TextureAtlasGenerator.UnitTests
{
    public class ExtractAssets : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHashCalculator _hashCalculator;
        private readonly IUltimaArtProvider _ultimaArtProvider;

        private readonly Bitmap _landArt;
        private readonly Bitmap _itemArt;

        public ExtractAssets()
        {
            _logger = Substitute.For<ILogger>();
            _hashCalculator = Substitute.For<IHashCalculator>();
            _configuration = Substitute.For<IConfiguration>();
            _ultimaArtProvider = Substitute.For<IUltimaArtProvider>();

            _landArt = new Bitmap(1, 1);
            _itemArt = new Bitmap(1, 1);
        }

        public void Dispose()
        {
            _landArt.Dispose();
            _itemArt.Dispose();
        }

        [Fact]
        public void No_Valid_UltimaArt_Will_Extract_No_TextureAssets()
        {
            _ultimaArtProvider.GetRawLand(Arg.Any<int>()).Returns(_ => null);
            _ultimaArtProvider.GetRawStatic(Arg.Any<int>()).Returns(_ => null);

            var assetExtractor = new AssetExtractor(
                _logger,
                _hashCalculator,
                _configuration,
                _ultimaArtProvider);

            var extractedAssets = assetExtractor.ExtractAssets();
            extractedAssets.Should().BeEmpty();
        }
    }
}
