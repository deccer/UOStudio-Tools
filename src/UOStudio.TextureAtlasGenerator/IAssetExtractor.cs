using System.Collections.Generic;

namespace UOStudio.TextureAtlasGenerator
{
    internal interface IAssetExtractor
    {
        IReadOnlyCollection<TextureAsset> ExtractAssets();
    }
}