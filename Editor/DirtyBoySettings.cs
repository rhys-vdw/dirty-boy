using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

// Based on:
// https://github.com/inkle/ink-unity-integration/blob/9fcec002bf7220fac6572b73b8d8b2838927605f/Packages/Ink/Editor/Core/Ink%20Settings/InkSettings.cs
namespace DirtyBoy {
  internal class DirtyBoySettings : ScriptableObject {
    [SerializeField] internal bool _enableReserializeWatcher = false;

    static string AbsolutePath => Path.GetFullPath(
      Path.Combine(Application.dataPath, "..", "ProjectSettings", nameof(DirtyBoySettings) + ".asset")
    );

    static DirtyBoySettings _instance = null;

    public static DirtyBoySettings Instance {
      get {
        if (_instance == null) {
          _instance = LoadOrCreate();
        }
        return _instance;
      }
    }

    static DirtyBoySettings LoadOrCreate() {
      var objects = InternalEditorUtility.LoadSerializedFileAndForget(AbsolutePath);
      var instance = null as DirtyBoySettings;
      if (objects.Length != 0) {
        // May become null if invalid.
        instance = objects[0] as DirtyBoySettings;
      }
      if (instance == null) {
        instance = ScriptableObject.CreateInstance<DirtyBoySettings>();
      }
      instance.Save();
      return instance;
    }

    public static SerializedObject GetSerializedSettings() =>
      new SerializedObject(Instance);

    public void Save() {
      InternalEditorUtility.SaveToSerializedFileAndForget(
        new [] { this },
        AbsolutePath,
        true
      );
    }
  }

  static class DirtyBoySettingsProvider
  {
    const string ReserializeWatcherMessage =
      "When enabled, any modification of MonoBehaviour or ScriptableObject " +
      "source will cause all prefabs or assets of that type to be " +
      "reserialized to disk.\n\n" +
      "This will update any added or renamed properties.";

    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider() =>
      new SettingsProvider("Project/Dirty Boy", SettingsScope.Project) {
        guiHandler = searchContext => {
          EditorGUI.BeginChangeCheck();

          var settings = DirtyBoySettings.GetSerializedSettings();
          EditorGUILayout.HelpBox(ReserializeWatcherMessage, MessageType.Info);
          EditorGUIUtility.labelWidth = 300; 
          EditorGUILayout.PropertyField(
            settings.FindProperty(nameof(DirtyBoySettings._enableReserializeWatcher)),
            new GUIContent("Reserialize assets on script change")
          );

          if (EditorGUI.EndChangeCheck()) {
            settings.ApplyModifiedProperties();
            ((DirtyBoySettings) settings.targetObject).Save();
          }
        },

        // Support smart search filtering and label highlighting.
        keywords = new HashSet<string> {
          "reserialize", "prefab", "scriptable", "scriptableObjet", "asset", "dirty"
        }
      };
  }
}