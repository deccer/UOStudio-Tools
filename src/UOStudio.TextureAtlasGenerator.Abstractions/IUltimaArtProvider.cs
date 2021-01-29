﻿using System.Drawing;

namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface IUltimaArtProvider
    {
        byte[] GetRawStatic(int artId);

        byte[] GetRawLand(int artId);

        Bitmap GetStatic(int artId);

        Bitmap GetLand(int artId);

        void InitializeFiles(string ultimaOnlinePath);
    }
}
