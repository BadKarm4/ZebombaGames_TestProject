using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Zoom settings")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    private Camera cam;
    private Vector2 prevTouch0Pos, prevTouch1Pos;
    private bool wasPinching = false;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Touchscreen.current == null) return;

        var activeTouches = Touchscreen.current.touches.Where(t => t.isInProgress).ToArray();

        if (activeTouches.Length == 2)
        {
            Vector2 touch0Pos = activeTouches[0].position.ReadValue();
            Vector2 touch1Pos = activeTouches[1].position.ReadValue();

            if (!wasPinching)
            {
                prevTouch0Pos = touch0Pos;
                prevTouch1Pos = touch1Pos;
                wasPinching = true;
                return;
            }

            float prevMagnitude = (prevTouch0Pos - prevTouch1Pos).magnitude;
            float currentMagnitude = (touch0Pos - touch1Pos).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            float zoomChange = difference * zoomSpeed * 0.01f;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomChange, minZoom, maxZoom);

            prevTouch0Pos = touch0Pos;
            prevTouch1Pos = touch1Pos;
        }
        else
        {
            wasPinching = false;
        }
    }
}
