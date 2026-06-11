#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-only visualization for the ore generator bounds in the Unity Scene view.
/// </summary>
[CustomEditor(typeof(OreGenerator))]
public class OreGeneratorEditor : Editor
{
    private void OnSceneGUI()
    {
        // The cast is safe because this editor is only registered for OreGenerator components.
        OreGenerator generator = (OreGenerator)target;

        // Read the current generation bounds from the serialized fields.
        // Keep these property names in sync if the generator field names change.
        int minX = serializedObject.FindProperty("minX").intValue;
        int maxX = serializedObject.FindProperty("maxX").intValue;
        int minY = serializedObject.FindProperty("minY").intValue;
        int maxY = serializedObject.FindProperty("maxY").intValue;

        // Draw a green outline to visualize the current generation area.
        Handles.color = Color.green;
        Vector3 topLeft = new Vector3(minX, maxY + 1, 0);
        Vector3 topRight = new Vector3(maxX + 1, maxY + 1, 0);
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX + 1, minY, 0);

        Handles.DrawLine(topLeft, topRight);
        Handles.DrawLine(topRight, bottomRight);
        Handles.DrawLine(bottomRight, bottomLeft);
        Handles.DrawLine(bottomLeft, topLeft);
        
        // Handles.Slider can be added here later for direct viewport resizing.
    }
}
#endif
