using System.Collections.Generic;

namespace UOStudio.TextureAtlasGenerator.Contracts
{
    public class Atlas
    {
        public int Width;

        public int Height;

        public int Depth;

        public List<ItemTile> Items;

        public List<LandTile> Lands;
    }
}