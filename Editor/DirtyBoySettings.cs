using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// Create a new type of Settings Asset.
namespace DirtyBoy {
  internal class DirtyBoySettings : ScriptableObject {
    const string Path = "Assets/" + nameof(DirtyBoySettings) + ".asset";

    [SerializeField] internal bool _enableReserializeWatcher = false;

    internal static DirtyBoySettings GetOrCreateSettings() {
      var settings = AssetDatabase.LoadAssetAtPath<DirtyBoySettings>(Path);
      if (settings == null)
      {
        settings = ScriptableObject.CreateInstance<DirtyBoySettings>();
        settings._enableReserializeWatcher = false;
        AssetDatabase.CreateAsset(settings, Path);
        AssetDatabase.SaveAssets();
      }
      return settings;
    }

    internal static SerializedObject GetSerializedSettings() =>
      new SerializedObject(GetOrCreateSettings());
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
        // By default the last token of the path is used as display name if no label is provided.
        // label = "Dirty Boy",
        // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
        guiHandler = searchContext => {
            var settings = DirtyBoySettings.GetSerializedSettings();
            EditorGUILayout.HelpBox(ReserializeWatcherMessage, MessageType.Info);
            EditorGUIUtility.labelWidth = 300; 
            EditorGUILayout.PropertyField(
              settings.FindProperty(nameof(DirtyBoySettings._enableReserializeWatcher)),
              new GUIContent("Reserialize assets on script change")
            );
            settings.ApplyModifiedProperties();
        },

        // Populate the search keywords to enable smart search filtering and label highlighting:
        keywords = new HashSet<string> { "reserialize", "prefab", "scriptable", "scriptableObjet", "asset" }
      };
  }
}