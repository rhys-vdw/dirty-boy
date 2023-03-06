using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DirtyBoy {
  class DirtyBoySettingsProvider {
    const string ReserializeWatcherMessage =
      "When enabled, any modification of MonoBehaviour or ScriptableObject " +
      "source will cause all prefabs or assets of that type to be " +
      "reserialized to disk.\n\n" +
      "This will update any added or renamed properties.";

    const string Namespace = nameof(DirtyBoy);
    const string ReserializeWatcherEnabledKey = Namespace + "EnableReserializeWatcher";

    public static bool ReserializeWatcherEnabled {
      get => EditorPrefs.GetBool(ReserializeWatcherEnabledKey);
      private set => EditorPrefs.SetBool(ReserializeWatcherEnabledKey, value);
    }

    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider() =>
      new SettingsProvider("Preferences/Dirty Boy", SettingsScope.User) {
        guiHandler = searchContext => {
          EditorGUI.BeginChangeCheck();

          EditorGUILayout.HelpBox(ReserializeWatcherMessage, MessageType.Info);
          EditorGUIUtility.labelWidth = 300; 
          ReserializeWatcherEnabled = EditorGUILayout.Toggle(
            new GUIContent("Reserialize assets on script change"),
            ReserializeWatcherEnabled
          );
        },

        // Support smart search filtering and label highlighting.
        keywords = new HashSet<string> {
          "reserialize", "prefab", "scriptable", "scriptableObjet", "asset", "dirty"
        }
      };
  }
}