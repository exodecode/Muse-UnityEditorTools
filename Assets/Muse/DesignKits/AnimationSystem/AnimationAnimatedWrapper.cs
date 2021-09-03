using System.Collections;
using UnityEngine;

namespace Muse
{
    [RequireComponent(typeof(Animation))]
    public class AnimationAnimatedWrapper : Animated
    {
        Animation anim;

        private void Awake() => anim = GetComponent<Animation>();
        private void Start() => anim.playAutomatically = false;

        public override void Reset()
        {
            StartCoroutine(Stop());
        }

        public override void PlayAnimation()
        {
            anim.Play();
        }

        public override void StopAnimation()
        {
            StartCoroutine(Stop());
        }

        IEnumerator Stop()
        {
            if (anim.isPlaying)
                anim.Stop();

            anim.Play();
            yield return null;
            anim.Stop();
        }

    }
}