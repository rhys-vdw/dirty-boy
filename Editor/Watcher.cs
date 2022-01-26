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
            if (PrefabUtility.IsPartOfPrefabAsset(obj)) {
              var go = obj is Component
                ? (obj as Component).gameObject
                : (GameObject) obj;
              var prefab = go.transform.root.gameObject;
              // var prefabPath = AssetDatabase.GetAssetPath(prefab);
              // AssetDatabase.LoadAssetAtPath(prefabPath);
              EditorUtility.SetDirty(prefab);
              Debug.Log($"Set prefab {prefab} dirty!", prefab);
            } else {
              EditorUtility.SetDirty(obj);
              Debug.Log($"Set {obj} dirty!", obj);
            }
            // if (EditorUtility.IsPersistent(obj)) {
            //   var assetPath = AssetDatabase.GetAssetPath(obj);
            //   reserializePaths.Add(assetPath);
            // }
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