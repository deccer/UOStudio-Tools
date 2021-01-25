using System;
using System.Collections.Generic;
using System.Drawing;

namespace UOStudio.TextureAtlasGenerator
{
    public interface ITexture3dGenerator : IDisposable
    {
        byte[] Generate3dTexture(IEnumerable<Bitmap> atlasPages);
    }
}