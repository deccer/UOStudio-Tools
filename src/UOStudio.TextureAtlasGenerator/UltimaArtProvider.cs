using System.Drawing;
using UOStudio.TextureAtlasGenerator.Abstractions;
using UOStudio.TextureAtlasGenerator.Ultima;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class UltimaArtProvider : IUltimaArtProvider
    {
        public byte[] GetRawStatic(int artId)
            => Art.GetRawStatic(artId);

        public byte[] GetRawLand(int artId)
            => Art.GetRawLand(artId);

        public Bitmap GetStatic(int artId)
            => Art.GetStatic(artId, false);

        public Bitmap GetLand(int artId)
            => Art.GetLand(artId);

        public void InitializeFiles(string ultimaOnlinePath)
        {
            Files.Initialize(ultimaOnlinePath);
        }
    }
}
