using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using System;


/// <summary>
/// Spine动画控制器
/// </summary>
public class SpineCtr
{


    /// <summary>
    /// 切换spine动画
    /// </summary>
    /// <param name="skeletonAnimatio">Skeleton animatio.</param>
    /// <param name="indexs">Indexs.</param>
    public static void SwitchAnimationState(SkeletonAnimation skeletonAnimatio, params int[] indexs)
    {
       
        if (skeletonAnimatio == null || indexs.Length == 0)
            return;

        for (int index = 0; index < indexs.Length; index++)
        {
            SpineConfig.GetSpineAnimationInfo(indexs[index], out string name, out int order);
            //Debug.Log("当前动作：" + name);
            TrackEntry entry=skeletonAnimatio.AnimationState.AddAnimation(0, name, true, 1f);
            entry.MixDuration = 0.5f;

        }
    }


    public static void SwitchAnimationState(SkeletonAnimation skeletonAnimatio, Spine.AnimationState.TrackEntryDelegate callback, params int[] indexs)
    {
       
        if (skeletonAnimatio == null || indexs.Length == 0)
            return;

        for (int index = 0; index < indexs.Length; index++)
        {
            SpineConfig.GetSpineAnimationInfo(indexs[index], out string name, out int order); 
            bool isRepeat = indexs[index] != SpineConfig.AC_TYPE_POSE;
            TrackEntry trackEntry = skeletonAnimatio.AnimationState.AddAnimation(0, name, isRepeat, 1f);
            trackEntry.MixDuration = 0.5f; 
            if (!isRepeat)
                trackEntry.Complete += callback;
        }
    }


    /// <summary>
    /// 清除spine动画
    /// </summary>
    /// <param name="skeletonAnimatio">Skeleton animatio.</param>
    /// <param name="indexs">Indexs.</param>
    public static void RemoveAnimationTrack(SkeletonAnimation skeletonAnimatio, params int[] indexs)
    {

        if (skeletonAnimatio == null || indexs.Length == 0)
            return;

        for (int index = 0; index < indexs.Length; index++)
        {
            SpineConfig.GetSpineAnimationInfo(indexs[index], out string name, out int order);
            skeletonAnimatio.AnimationState.SetEmptyAnimation(order, 0);
        }
    }

    /// <summary>
    /// 重置会空闲动作
    /// </summary>
    /// <param name="skeletonAnimation">Skeleton animation.</param>
    public static void ResetAniamtionToIdle(SkeletonAnimation skeletonAnimation)
    {
        for (int index = 2; index < 8; index++)
        {
            SpineConfig.GetSpineAnimationInfo(index, out string name, out int order); 
            skeletonAnimation.AnimationState.SetEmptyAnimation(order, 0);
        }

        SwitchAnimationState(skeletonAnimation, new int[] { SpineConfig.AC_TYPE_IDLE });
    }


    /// <summary>
    /// Sets the idle animation.用于初始化角色使用
    /// </summary>
    /// <param name="skeletonAnimation">Skeleton animation.</param>
    public static void SetIdleAnimation(SkeletonAnimation skeletonAnimation)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
    }



}
