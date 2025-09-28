using System;
using UnityEngine;
using UnityEngine.UI;

public class Circle : MonoBehaviour
{
    [SerializeField] private ParticleSystem destroyParticleSystem;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public Vector2Int GridPos { get; set; }
    public bool HasLanded { get; private set; }
    public CircleColorType CircleColor { get; private set; }
    public int scoreByColor { get; private set; }

    private Rigidbody2D rb;

    private void Awake()
    {
        HasLanded = false;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize()
    {
        CircleColor = GetRandomColor();
        SetColor(CircleColor);
    }

    public void SetKinematic(RigidbodyType2D rigidbodyType2D)
    {
        rb.bodyType = rigidbodyType2D;
    }

    public bool IsSleeping()
    {
        return rb.linearVelocity.sqrMagnitude <= 0.0025f;
    }

    private void OnDestroy()
    {
        destroyParticleSystem.gameObject.transform.parent = null;
        destroyParticleSystem.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Circle") || collision.collider.CompareTag("Ground"))
        {
            HasLanded = true;
        }
    }

    public void SetColor(CircleColorType colorType)
    {
        CircleColor = colorType;
        var main = destroyParticleSystem.main;

        switch (colorType)
        {
            case CircleColorType.Red:
                spriteRenderer.color = Color.red;
                main.startColor = Color.red;
                scoreByColor = 15;
                break;
            case CircleColorType.Blue:
                spriteRenderer.color = Color.blue;
                main.startColor = Color.blue;
                scoreByColor = 10;
                break;
            case CircleColorType.Green:
                spriteRenderer.color = Color.green;
                main.startColor = Color.green;
                scoreByColor = 5;
                break;
        }
    }

    public void UpdateGridPos(int x, int y)
    {
        GridPos = new Vector2Int(x, y);
    }

    private CircleColorType GetRandomColor()
    {
        int count = Enum.GetValues(typeof(CircleColorType)).Length;
        int randomIndex = UnityEngine.Random.Range(0, count);
        return (CircleColorType)randomIndex;
    }
}
