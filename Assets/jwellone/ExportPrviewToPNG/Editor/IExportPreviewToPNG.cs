using System.Collections;
using UObject = UnityEngine.Object;

#nullable enable

namespace jwelloneEditor
{
    interface IExportPreviewToPNG
    {
        IEnumerator ExportIfNeeded(string assetPath, UObject obj);
    }
}
