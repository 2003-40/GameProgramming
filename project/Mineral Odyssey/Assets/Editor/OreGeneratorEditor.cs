#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OreGenerator))]
public class OreGeneratorEditor : Editor
{
    private void OnSceneGUI()
    {
        OreGenerator generator = (OreGenerator)target;

        // 获取当前代码中的边界值
        // 注意：如果按方案二改了变量名，这里对应的获取方式也要变
        int minX = serializedObject.FindProperty("minX").intValue;
        int maxX = serializedObject.FindProperty("maxX").intValue;
        int minY = serializedObject.FindProperty("minY").intValue;
        int maxY = serializedObject.FindProperty("maxY").intValue;

        // 绘制一个绿色的方框来可视化当前的生成范围
        Handles.color = Color.green;
        Vector3 topLeft = new Vector3(minX, maxY + 1, 0);
        Vector3 topRight = new Vector3(maxX + 1, maxY + 1, 0);
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX + 1, minY, 0);

        Handles.DrawLine(topLeft, topRight);
        Handles.DrawLine(topRight, bottomRight);
        Handles.DrawLine(bottomRight, bottomLeft);
        Handles.DrawLine(bottomLeft, topLeft);
        
        // 可以在这里结合 Handles.Slider 制作拖拽手柄，直接在视口里拉伸范围
    }
}
#endif