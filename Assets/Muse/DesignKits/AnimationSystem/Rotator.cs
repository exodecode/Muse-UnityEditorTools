using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Muse
{
    public class Rotator : Animated
    {
        public enum RotationAxis { Pitch, Yaw, Roll }

        public float speed;
        public RotationAxis rotationAxis;
        public GameObject target;

        Transform targetTransform;
        Coroutine currentCoro;

        float stopTime;

        public override void Reset()
        {
            StopAnimation();
        }

        public override void PlayAnimation()
        {
            if (currentCoro != null)
                StopCoroutine(currentCoro);

            currentCoro = StartCoroutine(PlayCoro());
        }

        public override void StopAnimation()
        {
            if (currentCoro != null)
                StopCoroutine(currentCoro);

            stopTime = Time.time;
        }

        void Awake()
        {
            if (!target)
                target = gameObject;

            targetTransform = target.transform;
        }

        IEnumerator PlayCoro()
        {
            while (targetTransform)
            {
                var spin = (Time.time - stopTime) * speed;

                targetTransform.localRotation =
                    Quaternion.Euler(
                        targetTransform.localRotation.x + (rotationAxis == RotationAxis.Pitch ? spin : 0),
                        targetTransform.localRotation.y + (rotationAxis == RotationAxis.Yaw ? spin : 0),
                        targetTransform.localRotation.z + (rotationAxis == RotationAxis.Roll ? spin : 0));

                yield return null;
            }

            stopTime = Time.time;
        }
    }
}