using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface IUvwCalculatorStrategy
    {
        Uvws CalculcateUvws(TextureAsset textureAsset, int atlasPageSize, int currentX, int currentY, int page);
    }
}
