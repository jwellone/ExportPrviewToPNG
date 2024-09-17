using System;
using System.Linq;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UObject = UnityEngine.Object;

#nullable enable

namespace jwelloneEditor
{
    public class ExportAssetPreviewToPNG : IExportPreviewToPNG
    {
        readonly static Type[] _validTypes = new[] { typeof(Material), typeof(GameObject), typeof(AudioClip) };

        IEnumerator IExportPreviewToPNG.ExportIfNeeded(string assetPath, UObject obj)
        {
            var type = obj.GetType();
            if (!_validTypes.Any(x => x == type))
            {
                yield break;
            }

            var rootPath = Path.Combine(ExportPreviewToPNG.exportRootPath, GetType().Name);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var texture = AssetPreview.GetAssetPreview(obj);
            while (texture == null)
            {
                texture = AssetPreview.GetAssetPreview(obj);
                yield return null;
            }

            var path = Path.Combine(rootPath, $"{Path.GetFileNameWithoutExtension(assetPath)}.png");
            File.WriteAllBytes(path, texture.EncodeToPNG());
            GameObject.DestroyImmediate(texture);

            Debug.Log($"Export {Path.GetFullPath(path)}");
        }
    }
}