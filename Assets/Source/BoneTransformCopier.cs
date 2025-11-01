using UnityEngine;
using System.Collections.Generic;

public class BoneTransformCopier : MonoBehaviour
{
    [Header("Source and Target Root Bones")]
    public Transform sourceRootBone;
    public Transform targetRootBone;

    private List<(Transform source, Transform target)> bonePairs;

    private void Start()
    {
        if (sourceRootBone == null || targetRootBone == null)
        {
            Debug.LogError("Source or Target root bone not assigned.", this);
            enabled = false;
            return;
        }

        var targetBoneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in targetRootBone.GetComponentsInChildren<Transform>(true))
            targetBoneMap[bone.name] = bone;

        bonePairs = new List<(Transform, Transform)>();
        foreach (Transform sourceBone in sourceRootBone.GetComponentsInChildren<Transform>(true))
        {
            if (targetBoneMap.TryGetValue(sourceBone.name, out Transform targetBone))
                bonePairs.Add((sourceBone, targetBone));
        }

        if (bonePairs.Count == 0)
            Debug.LogWarning("No matching bones found between source and target.", this);
    }

    private void LateUpdate()
    {
        foreach (var (source, target) in bonePairs)
        {
            target.position = source.position;
            target.rotation = source.rotation;
           
        }
    }
}