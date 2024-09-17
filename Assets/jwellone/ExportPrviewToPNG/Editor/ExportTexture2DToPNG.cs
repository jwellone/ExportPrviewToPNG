using System.IO;
using System.Collections;
using UnityEngine;
using UObject = UnityEngine.Object;

#nullable enable

namespace jwelloneEditor
{
    class ExportTexture2DToPNG : IExportPreviewToPNG
    {
        IEnumerator IExportPreviewToPNG.ExportIfNeeded(string assetPath, UObject obj)
        {
            var source = obj as Texture2D;
            if (source == null)
            {
                yield break;
            }

            var rootPath = Path.Combine(ExportPreviewToPNG.exportRootPath, GetType().Name);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var texture = ExportPreviewToPNG.Copy(source);
            var path = Path.Combine(rootPath, $"{Path.GetFileNameWithoutExtension(assetPath)}.png");
            File.WriteAllBytes(path, texture.EncodeToPNG());
            GameObject.DestroyImmediate(texture);

            Debug.Log($"Export {Path.GetFullPath(path)}");
        }
    }
}
