using UnityEngine;
using UnityEditor;

namespace Muse
{
    using static EditorUtils;

    public class ColliderAdjusterEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Muse/Collider Adjuster")]
        static void ShowWindow() => GetWindow<ColliderAdjusterEditorWindow>("Collider Adjuster");

        void OnEnable() => Selection.selectionChanged += Repaint;
        void OnDisable() => Selection.selectionChanged -= Repaint;

        void OnGUI()
        {
            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                if (GUILayout.Button("Add Mesh Colliders Based On Child Meshes"))
                    AddMeshCollidersBasedOnChildMeshes(GetSelectedTransforms());
                if (GUILayout.Button("Remove All Colliders From Selections"))
                    RemoveAllColliders(GetSelectedTransforms());
                if (GUILayout.Button("Add Box Collider Based On Child Bounds"))
                    AddBoxColliderBasedOnChildMeshes(GetSelectedTransforms());
            }
        }

        static void AddMeshCollidersBasedOnChildMeshes(Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];
                var meshFilters = t.GetComponentsInChildren<MeshFilter>();

                for (int j = 0; j < meshFilters.Length; j++)
                {
                    var g = t.gameObject;
                    var mc = Undo.AddComponent<MeshCollider>(g);
                    mc.sharedMesh = meshFilters[j].sharedMesh;
                }
            }
        }

        static void RemoveAllColliders(Transform[] transforms)
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

        static void AddBoxColliderBasedOnChildMeshes(Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                var hasBounds = false;
                var t = transforms[i];
                var renderers = t.GetComponentsInChildren<MeshRenderer>();

                var bc = t.GetComponent<BoxCollider>();

                if (bc == null)
                {
                    var g = t.gameObject;
                    bc = Undo.AddComponent<BoxCollider>(g);
                }

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