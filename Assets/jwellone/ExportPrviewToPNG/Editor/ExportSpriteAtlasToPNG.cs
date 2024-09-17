using System.IO;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;
using UObject = UnityEngine.Object;

#nullable enable

namespace jwelloneEditor
{
    class ExportSpriteAtlasToPNG : IExportPreviewToPNG
    {
        const BindingFlags _flags = BindingFlags.Static | BindingFlags.NonPublic;
        static readonly MethodInfo _methodInfo = typeof(SpriteAtlasExtensions).GetMethod("GetPreviewTextures", _flags);

        IEnumerator IExportPreviewToPNG.ExportIfNeeded(string assetPath, UObject obj)
        {
            var spriteAtlas = obj as SpriteAtlas;
            if (spriteAtlas == null)
            {
                yield break;
            }

            var textures = _methodInfo.Invoke(null, new object[] { spriteAtlas }) as Texture2D[];
            if (textures == null)
            {
                yield break;
            }

            var rootPath = Path.Combine(ExportPreviewToPNG.exportRootPath, GetType().Name);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            for (var i = 0; i < textures.Length; ++i)
            {
                var texture = ExportPreviewToPNG.Copy(textures[i]);
                var path = Path.Combine(rootPath, $"{Path.GetFileNameWithoutExtension(assetPath)}{i}.png");
                File.WriteAllBytes(path, texture.EncodeToPNG());
                GameObject.DestroyImmediate(texture);
                Debug.Log($"Export {Path.GetFullPath(path)}");
            }
        }
    }
}