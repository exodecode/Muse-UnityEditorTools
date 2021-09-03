using System;
using System.Collections;
using UnityEngine;

namespace Muse
{
    public class AnimatedPlayer : MonoBehaviour
    {
        [Serializable]
        public class AnimationGroup
        {
            public string title = "asdf";
            public Animated[] animations;
            public float endDelay;
            public bool stop;
        }

        public float startDelay;
        public AnimationGroup[] animationGroups;

        Coroutine coroutine;

        void Start() => coroutine = StartCoroutine(Play());

        void ResetAll()
        {
            for (int i = 0; i < animationGroups.Length; i++)
                for (int j = 0; j < animationGroups[i].animations.Length; j++)
                    animationGroups[i].animations[j].Reset();
        }

#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetAll();

                if (coroutine != null)
                    StopCoroutine(coroutine);

                coroutine = StartCoroutine(Play());
            }
        }
#endif

        IEnumerator Play()
        {
            yield return new WaitForSeconds(startDelay);

            for (int i = 0; i < animationGroups.Length; i++)
            {
                var group = animationGroups[i];
                for (int j = 0; j < group.animations.Length; j++)
                {
                    var animation = group.animations[j];

                    if (group.stop)
                        animation.StopAnimation();
                    else
                        animation.PlayAnimation();
                }

                yield return new WaitForSeconds(group.endDelay);
            }
        }
    }
}