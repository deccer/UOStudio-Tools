using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Serilog;
using UOStudio.TextureAtlasGenerator.Abstractions;
using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class TileContainer : ITileContainer
    {
        private readonly ILogger _logger;
        private readonly IList<LandTile> _landTiles;
        private readonly IList<ItemTile> _itemTiles;

        public TileContainer(ILogger logger)
        {
            _logger = logger.ForContext<TileContainer>();
            _landTiles = new List<LandTile>(0x8000);
            _itemTiles = new List<ItemTile>(0x8000);
        }

        public void AddLandTile(LandTile tile)
        {
            _landTiles.Add(tile);
        }

        public void AddItemTile(ItemTile tile)
        {
            _itemTiles.Add(tile);
        }

        public void Save(string fileName, int atlasPageCount)
        {
            SetW(atlasPageCount);
            var json = JsonConvert.SerializeObject(_landTiles, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        private void SetW(int atlasPageCount)
        {
            foreach (var tile in _landTiles)
            {
                tile.Uvws.SetW(atlasPageCount);
            }
        }
    }
}