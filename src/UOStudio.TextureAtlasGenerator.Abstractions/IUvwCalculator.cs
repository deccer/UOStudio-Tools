using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface IUvwCalculator
    {
        Uvws CalculcateUvws(TextureAsset textureAsset, int atlasPageSize, int currentX, int currentY, int page);

        TileType TileType { get; }
    }
}
