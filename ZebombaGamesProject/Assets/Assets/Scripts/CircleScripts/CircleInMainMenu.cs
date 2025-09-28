using UnityEngine;

public class CircleInMainMenu : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector2 dir = Random.insideUnitCircle.normalized;

        rb.linearVelocity = dir * 3f;
    }

    private void OnEnable()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;

        rb.linearVelocity = dir * 3f;
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity.normalized * 3f;

        velocity = velocity.normalized * 3f;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        float absAngle = Mathf.Abs(angle % 90f);

        if (absAngle < 15f || absAngle > (90f - 15f))
        {
            velocity = Quaternion.Euler(0, 0, Random.Range(-15f, 15f)) * velocity;
        }

        rb.linearVelocity = velocity;
    }
}
