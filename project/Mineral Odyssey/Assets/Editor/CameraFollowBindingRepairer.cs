using UnityEditor;
using UnityEngine;

public static class CameraFollowBindingRepairer
{
    [MenuItem("Tools/Mineral Odyssey/Repair Open Scene Camera Follow")]
    public static void RepairOpenSceneCameraFollow()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[Camera Repair] No GameObject tagged Player found in the open scene.");
            return;
        }

        int repairedCount = 0;
        MonoBehaviour[] behaviours = Object.FindObjectsOfType<MonoBehaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            MonoBehaviour behaviour = behaviours[i];
            if (behaviour == null)
            {
                continue;
            }

            SerializedObject serializedObject = new SerializedObject(behaviour);
            SerializedProperty followProperty = serializedObject.FindProperty("m_Follow");
            if (followProperty == null || followProperty.propertyType != SerializedPropertyType.ObjectReference)
            {
                continue;
            }

            followProperty.objectReferenceValue = player.transform;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(behaviour);
            repairedCount++;
            Debug.Log($"[Camera Repair] Bound {behaviour.gameObject.name}.{behaviour.GetType().Name} to Player.");
        }

        if (repairedCount == 0)
        {
            Debug.LogWarning("[Camera Repair] No camera follow component with m_Follow was found in the open scene.");
            return;
        }

        Debug.Log($"[Camera Repair] Finished. Repaired camera follow components: {repairedCount}");
    }
}
