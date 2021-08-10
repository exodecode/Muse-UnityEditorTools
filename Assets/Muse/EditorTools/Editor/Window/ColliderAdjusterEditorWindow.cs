using UnityEngine;
using UnityEditor;

namespace Muse
{
    using static EditorUtils;

    public class ColliderAdjusterEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Muse/Collider Adjuster")]
        private static void ShowWindow() => GetWindow<ColliderAdjusterEditorWindow>("Collider Adjuster");

        void OnEnable() => Selection.selectionChanged += Repaint;
        void OnDisable() => Selection.selectionChanged -= Repaint;

        private void OnGUI()
        {
            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                if (GUILayout.Button("Add Mesh Colliders Based On Child Meshes"))
                    AddMeshCollidersBasedOnChildMeshes(GetSelectedTransforms());
                if (GUILayout.Button("Remove All Colliders From Selections"))
                    RemoveAllColliders(GetSelectedTransforms());
                if (GUILayout.Button("Add Box Collider Based On Child Bounds"))
                    AddMeshCollidersBasedOnChildMeshes(GetSelectedTransforms());
            }
        }

        public static void AddMeshCollidersBasedOnChildMeshes(Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];
                var meshFilters = t.GetComponentsInChildren<MeshFilter>();

                for (int j = 0; j < meshFilters.Length; j++)
                {
                    var mc = t.gameObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = meshFilters[j].sharedMesh;
                }
            }
        }

        public static void RemoveAllColliders(Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];
                var colliders = t.GetComponentsInChildren<Collider>();
                for (int j = 0; j < colliders.Length; j++)
                {
                    var collider = colliders[j];
                    Undo.DestroyObjectImmediate(collider);
                }
            }
        }

        public static void AddBoxColliderBasedOnChildMeshes(Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                var hasBounds = false;
                var t = transforms[i];
                var renderers = t.GetComponentsInChildren<MeshRenderer>();

                var bc = t.GetComponent<BoxCollider>();
                if (bc == null)
                    bc = t.gameObject.AddComponent<BoxCollider>();

                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

                for (int j = 0; j < renderers.Length; j++)
                {
                    var renderer = renderers[j];
                    if (renderer != null)
                    {
                        if (hasBounds)
                            bounds.Encapsulate(renderer.bounds);
                        else
                        {
                            bounds = renderer.bounds;
                            hasBounds = true;
                        }
                    }
                }

                bc.center = bounds.center - t.position;
                bc.size = bounds.size;
            }
        }
    }
}