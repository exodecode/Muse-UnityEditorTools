using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Muse
{
    using static SceneUtils;
    using static ShortcutKeys;

    public class SceneProfileLoaderWindow : EditorWindow
    {
        string[] options;
        int popupIndex;
        SceneProfile[] sceneProfiles;
        Vector2 scrollPos;

        [MenuItem("Tools/Muse/Scene Profile LoaderWindow" + SHORTCUT_WINDOW_SCENEPROFILE)]
        static void ShowWindow() => GetWindow<SceneProfileLoaderWindow>("Scene Profile Loader");

        void OnEnable()
        {
            sceneProfiles = GetAllInstancesF<SceneProfile>();
            options = sceneProfiles.Select(profile => profile.name).ToArray();
        }

        void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                var popupStyle = new GUIStyle("DropDownButton");
                popupStyle.fontSize = 14;
                popupStyle.alignment = TextAnchor.MiddleLeft;

                var buttonStyle = new GUIStyle("Button");
                buttonStyle.fontSize = 14;

                var scrollStyle = new GUIStyle("HelpBox");
                scrollStyle.fontSize = 14;
                scrollStyle.alignment = TextAnchor.MiddleLeft;
                scrollStyle.wordWrap = false;

                var sceneProfile = sceneProfiles[popupIndex];
                var activeScene = sceneProfile.activeScene;
                var additiveScenes = sceneProfile.additiveScenes;

                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, scrollStyle))
                {
                    popupIndex = EditorGUILayout.Popup(popupIndex, options, popupStyle);
                    scrollPos = scrollView.scrollPosition;

                    var s = new GUIStyle("Box");
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Load Scenes", buttonStyle))
                        LoadScenes(activeScene, additiveScenes);

                    if (GUILayout.Button("Unload Additive Scenes", buttonStyle))
                        UnloadAdditiveScenes(false, additiveScenes);

                    if (GUILayout.Button("Remove Additive Scenes", buttonStyle))
                        UnloadAdditiveScenes(true, additiveScenes);

                    EditorGUILayout.Space();

                    if (GUILayout.Button("Collapse All Scenes", buttonStyle))
                        SetExpandedForAllScenes(false, additiveScenes);
                }
            }
        }

        static T[] GetAllInstancesF<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            var assets = paths.Select(path => AssetDatabase.LoadAssetAtPath<T>(path));

            return assets.ToArray();
        }
    }
}