using UnityEngine;
using UnityEditor;

namespace DirtyBoy {
  public static class MarkSelectionDirty {
    [MenuItem("CONTEXT/Object/Mark dirty")]
    [MenuItem("GameObject/Mark dirty")]
    [MenuItem("Assets/Mark dirty")]
    public static void Command() {
      foreach (var obj in Selection.objects) {
        Debug.Log(obj.GetType());
        EditorUtility.SetDirty(obj);
      }
      Debug.Log($"Marked {Selection.objects.Length} objects dirty");
    }

    [MenuItem("CONTEXT/Object/Mark dirty", true)]
    [MenuItem("GameObject/Mark dirty", true)]
    [MenuItem("Assets/Mark dirty", true)]
    public static bool CommandValidator() =>
      Selection.objects.Length > 0;
   }
}
