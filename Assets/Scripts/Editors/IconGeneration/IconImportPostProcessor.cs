#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editors.IconGeneration
{
    public static class IconImportPostProcessor
    {
        public static void Apply(string assetPath, int maxSize)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
                return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.isReadable = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = Mathf.Max(32, maxSize);

            importer.SaveAndReimport();
        }
    }
}
#endif
