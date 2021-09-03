using UnityEngine;
using UnityEditor;

namespace Muse
{
    using static ShortcutKeys;

    public class ModelCheckerEditorWindow : EditorWindow
    {
        /*
            see polycounts:
                vertices
                triangles

            check for missing materials

            have profiles for a model based on the platform and it's intended use
                gameplay prop
                background prop
                character
                building
                LOD number or if it doesn't have LODs

            check the bounds of the mesh and get a ratio of volume to poly count to get it's poly density
                triangle density
                vertex density

            show inconsistencies in poly density based on a value

            preview of the mesh
                poly heat map
        */

        [MenuItem("Tools/Muse/Model Checker" + SHORTCUT_WINDOW_MODELCHECK)]
        static void ShowWindow() => GetWindow<ModelCheckerEditorWindow>("Model Checker");

        void OnGUI()
        {

        }
    }
}