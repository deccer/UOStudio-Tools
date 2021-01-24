using System.Collections.Generic;
using System.Drawing;

namespace UOStudio.TextureAtlasGenerator
{
    internal interface IAtlasPageGenerator
    {
        IReadOnlyCollection<Bitmap> GeneratePages(IReadOnlyList<TextureAsset> textureAssets);
    }
}