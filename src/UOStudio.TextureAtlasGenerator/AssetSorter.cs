using System.Collections.Generic;
using System.Linq;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class AssetSorter : IAssetSorter
    {
        public IReadOnlyCollection<TextureAsset> SortAssets(
            IReadOnlyCollection<TextureAsset> textureAssets)
        {
            return textureAssets
                .OrderByDescending(textureAsset => textureAsset.Bitmap.Height)
                .ThenByDescending(textureAsset => textureAsset.Bitmap.Width * textureAsset.Bitmap.Height)
                .ToArray();
        }
    }
}