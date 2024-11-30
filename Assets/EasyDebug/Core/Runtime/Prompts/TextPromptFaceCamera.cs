using UnityEngine;

namespace EasyDebug.Prompts
{
    public class TextPromptFaceCamera : MonoBehaviour
    {
        private Camera _mainCamera; // camera object has to be tagged with "MainCamera"

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            transform.LookAwayFrom(_mainCamera.transform);
        }
    }
    public static class TransformExtensions
    {
        /// <summary>
        /// Makes the source Transform look directly away from the target Transform.
        /// </summary>
        /// <param name="source">The Transform to rotate.</param>
        /// <param name="target">The Transform to look away from.</param>
        public static void LookAwayFrom(this Transform source, Transform target)
        {
            if (source == null || target == null) return;

            // Calculate the direction away from the target
            Vector3 directionAway = source.position - target.position;

            // Ensure the direction has magnitude to avoid invalid rotations
            if (directionAway.sqrMagnitude > 0.0001f)
            {
                source.rotation = Quaternion.LookRotation(directionAway.normalized);
            }
        }
    }
}
