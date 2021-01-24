namespace UOStudio.TextureAtlasGenerator
{
    public interface IUvwCalculatorStrategy
    {
        Uvws CalculcateUvws(TextureAsset textureAsset, int atlasPageSize, int currentX, int currentY, int page);
    }
}