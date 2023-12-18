using UnityEngine;
using UnityEditor;

namespace DirtyBoy {
  class Watcher : AssetPostprocessor {
    static void OnPostprocessAllAssets(
      string[] importedAssets,
      string[] deletedAssets,
      string[] movedAssets,
      string[] movedFromAssetPaths
    ) {
      if (Application.isBatchMode) {
        return;
      }
      foreach (string path in importedAssets) {
        var script = (MonoScript) AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
        if (script == null) continue;
        var scriptClass = script.GetClass();
        if (scriptClass == null) continue;
        if (typeof(ScriptableObject).IsAssignableFrom(scriptClass)) {
          var guids = AssetDatabase.FindAssets($"t:{scriptClass}");
          foreach (var guid in guids) {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var o = AssetDatabase.LoadAssetAtPath(assetPath, scriptClass);
            if (o == null) {
              Debug.LogWarning($"Expected to find {scriptClass} at '{assetPath}'");
            } else {
              EditorUtility.SetDirty(o);
            }
          }
        }
      }
    }
  }
}