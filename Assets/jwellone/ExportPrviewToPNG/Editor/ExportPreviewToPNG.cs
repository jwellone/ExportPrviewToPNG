using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;

#nullable enable

namespace jwelloneEditor
{
    public static class ExportPreviewToPNG
    {
        public static string exportRootPath = $"{Application.dataPath}/../ExportPreview";

        [MenuItem("Assets/Export Preview to PNG")]
        static void MenuItemForExportAssetPreviewToPNG()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(Export());
        }

        static IEnumerator Export()
        {
            if (!Directory.Exists(exportRootPath))
            {
                Directory.CreateDirectory(exportRootPath);
            }

            var targetType = typeof(IExportPreviewToPNG);
            var types = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass && targetType.IsAssignableFrom(t)).
                ToList();

            var exporters = new List<IExportPreviewToPNG>();
            foreach (var type in types)
            {
                var instance = (IExportPreviewToPNG)Activator.CreateInstance(type);
                exporters.Add(instance);
            }

            var count = 0;
            var length = (float)Selection.objects.Length;
            foreach (var obj in Selection.objects)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                EditorUtility.DisplayProgressBar("Export", assetPath, ++count / length);

                if (string.IsNullOrEmpty(assetPath) || Directory.Exists(assetPath))
                {
                    continue;
                }

                foreach (var export in exporters)
                {
                    yield return export.ExportIfNeeded(assetPath, obj);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        public static Texture2D Copy(Texture2D source)
        {
            var rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default);

            Graphics.Blit(source, rt);
            var tmpRT = RenderTexture.active;

            RenderTexture.active = rt;
            var dest = new Texture2D(source.width, source.height);
            dest.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            //dest.Apply();

            RenderTexture.active = tmpRT;

            RenderTexture.ReleaseTemporary(rt);
            return dest;
        }
    }
}