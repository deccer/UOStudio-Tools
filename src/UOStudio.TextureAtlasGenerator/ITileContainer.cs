namespace UOStudio.TextureAtlasGenerator
{
    public interface ITileContainer
    {
        void AddTile(Tile tile);

        void Save(string fileName);

        void SetW(int atlasPageCount);
    }
}