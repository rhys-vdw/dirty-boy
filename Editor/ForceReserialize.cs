using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DirtyBoy {
  public static class ForceReserialize {
    const string MenuPath = "Assets/Force Reserialize";
    [MenuItem(MenuPath)]
    public static void Command() {
      var paths = new List<string>();
      foreach (var obj in Selection.objects) {
        if (EditorUtility.IsPersistent(obj)) {
          paths.Add(AssetDatabase.GetAssetPath(obj));
        }
      }
      if (paths.Count > 0) {
        AssetDatabase.ForceReserializeAssets(paths);
        Debug.Log($"Reserialized {paths.Count} assets");
      }
    }

    [MenuItem(MenuPath, true)]
    public static bool Validate() {
      foreach (var obj in Selection.objects) {
        if (EditorUtility.IsPersistent(obj)) {
          return true;
        }
      }
      return false;
    }
  }
}