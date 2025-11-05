using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class DeadPlayer : MonoBehaviour
{
    [Header("Source and Target Root Bones")]
    public Transform sourceRootBone;

    public Transform targetRootBone;

    private List<(Transform source, Transform target)> bonePairs;

    public void CopyBones(Transform sourceRoot)
    {
        sourceRootBone = sourceRoot;

        var targetBoneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in targetRootBone.GetComponentsInChildren<Transform>(true))
            targetBoneMap[bone.name] = bone;

        bonePairs = new List<(Transform, Transform)>();
        foreach (Transform sourceBone in sourceRootBone.GetComponentsInChildren<Transform>(true))
        {
            if (targetBoneMap.TryGetValue(sourceBone.name, out Transform targetBone))
                bonePairs.Add((sourceBone, targetBone));
        }

        foreach (var (source, target) in bonePairs)
        {
            target.position = source.position;
            target.rotation = source.rotation;
        }
    }
}