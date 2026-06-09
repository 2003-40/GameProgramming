using UnityEditor;
using UnityEngine;

public static class PlayerAnimationBindingRepairer
{
    private const string OldToolPath = "WeaponAnchor/WeaponSprite";
    private const string NewToolPath = "ToolAnchor/ToolSprite";
    private const string SpritePropertyName = "m_Sprite";

    private static readonly string[] MiningClipPaths =
    {
        "Assets/Animations/MineDown.anim",
        "Assets/Animations/MineLeft.anim",
        "Assets/Animations/MineRight.anim",
        "Assets/Animations/MineUp.anim"
    };

    [MenuItem("Tools/Mineral Odyssey/Repair Mining Animation Bindings")]
    public static void RepairMiningAnimationBindings()
    {
        int repairedCount = 0;

        for (int i = 0; i < MiningClipPaths.Length; i++)
        {
            string clipPath = MiningClipPaths[i];
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (clip == null)
            {
                Debug.LogWarning($"[Animation Repair] Missing mining clip: {clipPath}");
                continue;
            }

            EditorCurveBinding oldBinding = EditorCurveBinding.PPtrCurve(OldToolPath, typeof(SpriteRenderer), SpritePropertyName);
            EditorCurveBinding newBinding = EditorCurveBinding.PPtrCurve(NewToolPath, typeof(SpriteRenderer), SpritePropertyName);

            ObjectReferenceKeyframe[] oldCurve = AnimationUtility.GetObjectReferenceCurve(clip, oldBinding);
            ObjectReferenceKeyframe[] newCurve = AnimationUtility.GetObjectReferenceCurve(clip, newBinding);

            if (oldCurve != null && oldCurve.Length > 0)
            {
                AnimationUtility.SetObjectReferenceCurve(clip, oldBinding, null);
                AnimationUtility.SetObjectReferenceCurve(clip, newBinding, oldCurve);
                EditorUtility.SetDirty(clip);
                repairedCount++;
                Debug.Log($"[Animation Repair] Rebound {clip.name}: {OldToolPath} -> {NewToolPath}");
                continue;
            }

            if (newCurve != null && newCurve.Length > 0)
            {
                Debug.Log($"[Animation Repair] Already bound correctly: {clip.name}");
                continue;
            }

            Debug.LogWarning($"[Animation Repair] No sprite curve found on {clip.name}. Recreate its ToolSprite sprite keys if this clip looks static.");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[Animation Repair] Finished. Repaired clips: {repairedCount}");
    }
}
