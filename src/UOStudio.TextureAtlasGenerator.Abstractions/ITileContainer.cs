using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface ITileContainer
    {
        void AddLandTile(LandTile tile);

        void AddItemTile(ItemTile tile);

        void Save(string fileName, int atlasPageCount);
    }
}