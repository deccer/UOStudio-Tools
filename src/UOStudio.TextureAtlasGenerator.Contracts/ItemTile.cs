namespace UOStudio.TextureAtlasGenerator.Contracts
{
    public class ItemTile : Tile
    {
        public ItemTile(int id, Uvws uvws, int height)
            : base(id, uvws)
        {
            Height = height;
        }

        public int Height;
    }
}