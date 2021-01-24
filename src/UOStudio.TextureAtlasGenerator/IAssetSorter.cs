using System.Collections.Generic;

namespace UOStudio.TextureAtlasGenerator
{
    internal interface IAssetSorter
    {
        IReadOnlyCollection<TextureAsset> SortAssets(IReadOnlyCollection<TextureAsset> textureAssets);
    }
}