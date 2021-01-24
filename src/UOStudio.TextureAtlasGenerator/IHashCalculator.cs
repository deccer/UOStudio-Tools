namespace UOStudio.TextureAtlasGenerator
{
    public interface IHashCalculator
    {
        string CalculateHash(byte[] bytes);
    }
}