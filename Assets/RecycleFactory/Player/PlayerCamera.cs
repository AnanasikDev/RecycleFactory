using UnityEngine;

namespace RecycleFactory.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField, Min(0.1f)] private float rotationSpeed = 5f;
        [SerializeField, Min(0.01f)] private float rotationSmoothing = 0.1f;

        [Header("Zoom Settings")]
        [SerializeField, Min(0.1f)] private float zoomSpeed = 5f;
        [SerializeField, Range(5f, 50f)] private float minZoom = 10f;
        [SerializeField, Range(5f, 50f)] private float maxZoom = 30f;
        [SerializeField, Min(0.01f)] private float zoomSmoothing = 0.1f;

        [Header("Vertical Bounds")]
        [SerializeField] private float minVerticalAngle = -30f;
        [SerializeField] private float maxVerticalAngle = 60f;

        [Space(20)]
        [SerializeField] private Transform target;
        [SerializeField] internal Camera cameraHandler;

        private Vector3 targetRotation;
        private float targetDistance;
        private float currentDistance;

        public void Init()
        {
            targetDistance = Mathf.Clamp(Vector3.Distance(transform.position, target.position), minZoom, maxZoom);
            currentDistance = targetDistance;
            targetRotation = new Vector3(30, 0, 0);
        }

        private void Update()
        {
            HandleRotation();
            HandleZoom();
            UpdateCameraPosition();
        }

        private void HandleRotation()
        {
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
                float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed;

                targetRotation.y += mouseX;
                targetRotation.x = Mathf.Clamp(targetRotation.x + mouseY, minVerticalAngle, maxVerticalAngle);
            }
        }

        private void HandleZoom()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0f)
            {
                targetDistance = Mathf.Clamp(targetDistance - scrollInput * zoomSpeed, minZoom, maxZoom);
            }

            currentDistance = Mathf.Lerp(currentDistance, targetDistance, zoomSmoothing * Time.deltaTime);
        }

        private void UpdateCameraPosition()
        {
            // Convert spherical coordinates to Cartesian coordinates for smooth rotation
            Quaternion rotation = Quaternion.Euler(targetRotation.x, targetRotation.y, 0f);
            Vector3 offset = rotation * Vector3.forward * currentDistance;
            transform.position = target.position - offset;

            transform.LookAt(target);
        }
    }
}
