using UnityEngine;
using UnityEditor;

namespace DirtyBoy {
  public static class MarkDirty {
    const string Name = "Mark Dirty";
    const string ContextPath = "CONTEXT/Object/" + Name;
    const string GameObjectPath = "GameObject/" + Name;
    const string AssetsPath = "Assets/" + Name;

    [MenuItem(ContextPath)]
    [MenuItem(GameObjectPath)]
    [MenuItem(AssetsPath)]
    public static void Command() {
      foreach (var obj in Selection.objects) {
        EditorUtility.SetDirty(obj);
      }
      Debug.Log($"Marked {Selection.objects.Length} objects dirty");
    }

    [MenuItem(ContextPath, true)]
    [MenuItem(GameObjectPath, true)]
    [MenuItem(AssetsPath, true)]
    public static bool Validate() =>
      Selection.objects.Length > 0;
  }
}