using UnityEngine;

namespace Muse
{
    public abstract class Animated : MonoBehaviour
    {
        public abstract void PlayAnimation();
        public abstract void StopAnimation();
        public abstract void Reset();
    }
}