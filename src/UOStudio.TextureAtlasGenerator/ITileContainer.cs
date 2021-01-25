namespace UOStudio.TextureAtlasGenerator
{
    public interface ITileContainer
    {
        void AddTile(Tile tile);

        void Save(string fileName, int atlasPageCount);
    }
}