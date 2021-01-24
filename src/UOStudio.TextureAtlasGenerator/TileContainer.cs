using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Serilog;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class TileContainer : ITileContainer
    {
        private readonly ILogger _logger;
        private readonly IList<Tile> _tiles;

        public TileContainer(ILogger logger)
        {
            _logger = logger.ForContext<TileContainer>();
            _tiles = new List<Tile>(0x8000);
        }

        public void AddTile(Tile tile)
        {
            _tiles.Add(tile);
        }

        public void Save(string fileName)
        {
            var json = JsonConvert.SerializeObject(_tiles, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public void SetW(int atlasPageCount)
        {
            foreach (var tile in _tiles)
            {
                tile.Uvws.SetW(atlasPageCount);
            }
        }
    }
}