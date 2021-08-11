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
    }
}