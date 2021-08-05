using UnityEditor;

namespace Muse
{
    using static PlacementTools;
    using static EditorUtils;

    public static class PlacementToolsEditor
    {
        [MenuItem("Tools/Muse/Placement/Set Random Y Rotation")]
        public static void SetRandomYRotationItem() => SetRandomYRotation(GetSelectedTransforms());

        [MenuItem("Tools/Muse/Placement/Place In A Line")]
        public static void PlaceObjectsInALineItem() => PlaceObjectsInALine(GetSelectedTransforms());
    }
}