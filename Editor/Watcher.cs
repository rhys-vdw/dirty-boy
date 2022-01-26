using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DirtyBoy {
  class Watcher : AssetPostprocessor {
    static void OnPostprocessAllAssets(
      string[] importedAssets,
      string[] deletedAssets,
      string[] movedAssets,
      string[] movedFromAssetPaths
    ) {
      var reserializePaths = new List<string>();
      foreach (string path in importedAssets) {
        var script = (MonoScript) AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
        if (script == null) continue;
        var scriptClass = script.GetClass();
        if (scriptClass == null) continue;
        if (typeof(UnityEngine.Object).IsAssignableFrom(scriptClass)) {
          var objects = Resources.FindObjectsOfTypeAll(scriptClass);
          foreach (var obj in objects) {
            if (EditorUtility.IsPersistent(obj)) {
              var assetPath = AssetDatabase.GetAssetPath(obj);
              reserializePaths.Add(assetPath);
            }
          }
        }
      }

      if (reserializePaths.Count > 0) {
        AssetDatabase.ForceReserializeAssets(reserializePaths);
        Debug.Log($"Reserialized:\n - {string.Join("\n - ", reserializePaths)}");
      }
    }
  }
}