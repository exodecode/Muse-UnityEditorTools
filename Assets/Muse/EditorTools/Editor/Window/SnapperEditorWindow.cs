using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace Muse
{
    using static Math;

    public class SnapperEditorWindow : EditorWindow
    {
        public enum GridType
        {
            Cartesian,
            Polar
        }

        const string UNDO_STR_SNAP = "snap objects";
        public float gridSize;
        public GridType gridType = GridType.Cartesian;
        public int angularDivisions = 24;

        [MenuItem("Tools/Muse/SnapperWindow %&S")]
        static void ShowWindow() => GetWindow<SnapperEditorWindow>("Snapper");

        SerializedObject serializedObject;
        SerializedProperty propertyGridSize;
        SerializedProperty propertyGridType;
        SerializedProperty propertyAngularDivisions;

        void OnEnable()
        {
            serializedObject = new SerializedObject(this);

            propertyGridSize = serializedObject.FindProperty("gridSize");
            propertyGridType = serializedObject.FindProperty("gridType");
            propertyAngularDivisions = serializedObject.FindProperty("angularDivisions");

            gridSize = EditorPrefs.GetFloat("SNAPPER_TOOL_gridSize", 1f);
            gridType = (GridType)EditorPrefs.GetInt("SNAPPER_TOOL_gridType", 0);
            angularDivisions = EditorPrefs.GetInt("SNAPPER_TOOL_angularDivisions", 24);

            Selection.selectionChanged += Repaint;
            SceneView.duringSceneGui += DuringSceneGUI;
        }

        void OnDisable()
        {
            EditorPrefs.SetFloat("SNAPPER_TOOL_gridSize", gridSize);
            EditorPrefs.SetInt("SNAPPER_TOOL_gridType", (int)gridType);
            EditorPrefs.SetInt("SNAPPER_TOOL_angularDivisions", angularDivisions);

            Selection.selectionChanged -= Repaint;
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        void DuringSceneGUI(SceneView sceneView)
        {
            const float gridDrawExtent = 16;

            if (Event.current.type == EventType.Repaint)
            {
                Handles.zTest = CompareFunction.LessEqual;

                if (gridType == GridType.Cartesian)
                    DrawGridCartesian(gridDrawExtent, gridSize);
                else
                    DrawGridPolar(gridDrawExtent, gridSize, angularDivisions);
            }
        }

        static void DrawGridPolar(float _gridDrawExtent, float _gridSize, int angularDivisions)
        {
            var ringCount = Mathf.RoundToInt(_gridDrawExtent / _gridSize);

            float radiusOuter = (ringCount - 1) * _gridSize;

            //radial grid
            for (int i = 1; i < ringCount; i++)
                Handles.DrawWireDisc(Vector3.zero, Vector3.up, _gridSize * i);

            //angular grid
            for (int i = 0; i < angularDivisions; i++)
            {
                float t = i / (float)angularDivisions;
                // go from t to an angle
                // want angle in radians
                // convert 0-1 to 0-2PI(also called tau)
                float angRad = t * TAU;// turns to radians
                float x = Mathf.Cos(angRad);
                float z = Mathf.Sin(angRad);

                Vector3 dir = new Vector3(x, 0, z);
                Handles.DrawAAPolyLine(Vector3.zero, dir * radiusOuter);
            }
        }

        static void DrawGridCartesian(float _gridDrawExtent, float _gridSize)
        {
            int lineCount = Mathf.RoundToInt((_gridDrawExtent * 2) / _gridSize);
            int halfLineCount = lineCount / 2;

            if (lineCount % 2 == 0)
                lineCount++;

            for (int i = 0; i < lineCount; i++)
            {
                int intOffset = i - halfLineCount;

                float xCoord = intOffset * _gridSize;
                float zCoord0 = halfLineCount * _gridSize;
                float zCoord1 = -halfLineCount * _gridSize;

                Vector3 p0 = new Vector3(xCoord, 0f, zCoord0);
                Vector3 p1 = new Vector3(xCoord, 0f, zCoord1);
                Handles.DrawAAPolyLine(p0, p1);

                p0 = new Vector3(zCoord0, 0f, xCoord);
                p1 = new Vector3(zCoord1, 0f, xCoord);
                Handles.DrawAAPolyLine(p0, p1);
            }
        }

        void OnGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(propertyGridType);
            EditorGUILayout.PropertyField(propertyGridSize);
            propertyGridSize.floatValue = Mathf.Max(0, propertyGridSize.floatValue);

            if (gridType == GridType.Polar)
            {
                EditorGUILayout.PropertyField(propertyAngularDivisions);
                propertyAngularDivisions.intValue = Mathf.Max(4, propertyAngularDivisions.intValue);
            }

            serializedObject.ApplyModifiedProperties();

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Snap"))
                    SnapSelection();
            }
        }

        void SnapSelection()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                var go = Selection.gameObjects[i];
                Undo.RecordObject(go.transform, UNDO_STR_SNAP);
                go.transform.position = GetSnappedPosition(go.transform.position, gridSize, angularDivisions, gridType);
            }
        }

        static Vector3 GetSnappedPosition(Vector3 _posOriginal, float _gridSize, int _angularDivisions, GridType _gridType)
        {
            if (_gridType == GridType.Cartesian)
                return _posOriginal.Round(_gridSize);
            else if (_gridType == GridType.Polar)
            {
                var vec = new Vector2(_posOriginal.x, _posOriginal.z);
                var distFromCenter = vec.magnitude;
                var distSnapped = distFromCenter.Round(_gridSize);
                //atan2 takes a direction and gives an angle
                var angleInRadians = Mathf.Atan2(vec.y, vec.x); // 0 to TAU
                var angleInTurns = angleInRadians / TAU; // 0 to 1
                var angleTurnsSnapped = angleInTurns.Round(1f / _angularDivisions);
                var angleRadiansSnapped = angleTurnsSnapped * TAU;
                var dirSnapped = new Vector2(Mathf.Cos(angleRadiansSnapped), Mathf.Sin(angleRadiansSnapped));
                var vecSnapped = dirSnapped * distSnapped;

                return new Vector3(vecSnapped.x, _posOriginal.y, vecSnapped.y);
            }

            return default;
        }
    }
}