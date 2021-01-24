namespace UOStudio.TextureAtlasGenerator
{
    public interface IUvwCalculator
    {
        Uvws CalculcateUvws(TextureAsset textureAsset, int atlasPageSize, int currentX, int currentY, int page);

        TileType TileType { get; }
    }
}