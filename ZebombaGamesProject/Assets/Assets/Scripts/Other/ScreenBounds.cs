using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    private void Start()
    {
        Camera cam = Camera.main;

        Vector2 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        Vector2 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        Vector2 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));

        EdgeCollider2D edge = GetComponent<EdgeCollider2D>();

        Vector2[] points = new Vector2[]
        {
            bottomLeft,
            bottomRight,
            topRight,
            topLeft,
            bottomLeft
        };

        edge.points = points;
    }
}
