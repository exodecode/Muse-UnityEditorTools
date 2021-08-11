using UnityEditor;

namespace Muse
{
    using static GameObjectTools;
    using static EditorUtils;

    public static class GameObjectToolsEditor
    {
        [MenuItem("Tools/Muse/GameObject/Assign To Parent With Same Name")]
        static void MakeAndAssignToParentWithSameNameItem() => MakeAndAssignToParentWithSameName(GetSelectedTransforms());

        [MenuItem("Tools/Muse/GameObject/Sort/Texture")]
        static void SortGameObjectsBasedOnHavingTextureItem() => SortGameObjectsBasedOnHavingTexture(GetSelectedTransforms());

        [MenuItem("Tools/Muse/GameObject/Sort/Alpabetically")]
        static void SortGameObjectsAlpabeticallyItem() => SortGameObjectsAlpabetically(GetSelectedTransforms());

        // [MenuItem("Tools/Muse/GameObject/Collider/Add Mesh Colliders Based On Child Meshes")]
        // static void AddMeshCollidersBasedOnChildMeshesItem() => AddMeshCollidersBasedOnChildMeshes(GetSelectedTransforms());

        // [MenuItem("Tools/Muse/GameObject/Collider/Remove All Colliders")]
        // static void RemoveAllCollidersItem() => RemoveAllColliders(GetSelectedTransforms());

        // [MenuItem("Tools/Muse/GameObject/Collider/Try Add and Adjust Box Collider Based Children")]
        // static void AddBoxColliderBasedOnChildMeshesItem() => AddBoxColliderBasedOnChildMeshes(GetSelectedTransforms());

        // [MenuItem("Tools/Muse/Organization/Create Folders For Selected assets")]
        // static void FoldersForSelectedItem() => FoldersForSelected(Selection.gameObjects);
    }
}