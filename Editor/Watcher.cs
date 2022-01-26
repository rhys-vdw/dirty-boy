using UnityEngine;
using UnityEditor;
using System.Text;
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
          if (objects.Length > 0) {
            foreach (var obj in objects) {
              var component = obj as Component;
              if (component != null && PrefabUtility.IsPartOfPrefabAsset(component)) {
                Debug.Log("persistent? " + EditorUtility.IsPersistent(component));
                var prefab = component.transform.root.gameObject;
                var prefabPath = AssetDatabase.GetAssetPath(prefab);
                EditorUtility.SetDirty(prefab);
                reserializePaths.Add(prefabPath);
              } else if (EditorUtility.IsPersistent(obj)) {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(assetPath)) {
                  reserializePaths.Add(assetPath);
                }
              }
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