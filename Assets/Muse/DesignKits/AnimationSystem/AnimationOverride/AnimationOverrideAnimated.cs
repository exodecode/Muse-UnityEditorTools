using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Muse
{
    public class AnimationOverrideAnimated : Animated
    {
        public string clipName = "base";
        public AnimationClip[] animationClips;

        public bool applyRootMotion = false;

        Animator animator;
        AnimatorOverrideController animatorOverrideController;
        int index;

        Dictionary<string, int> animationIndexDictionary = new Dictionary<string, int>();

        public override void Reset()
        {
            index = 0;
        }

        public override void PlayAnimation()
        {
            PlayNextClip();
        }

        public override void StopAnimation()
        {
            PlayNextClip();
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            animator.applyRootMotion = applyRootMotion;
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;

            animationIndexDictionary =
                Enumerable.Range(0, animationClips.Length)
                .Select(i => new KeyValuePair<string, int>(animationClips[i].name, i))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public void PlayNextClip()
        {
            if (index >= animationClips.Length)
                index = 0;

            PlayAnimClipAtIndex(index++);
        }

        void PlayAnimClipAtIndex(int index) => animatorOverrideController[clipName] = animationClips[index];

        public void PlayAnimClipByName(string s)
        {
            if (animationIndexDictionary.ContainsKey(s))
                animatorOverrideController[clipName] = animationClips[animationIndexDictionary[s]];
#if UNITY_EDITOR
            else
                Debug.Log("Could Not Find Key: " + s);
#endif
        }
    }
}