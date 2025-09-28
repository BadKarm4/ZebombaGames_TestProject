using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pendulum : MonoBehaviour
{
    [SerializeField] private float amplitude = 30f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform spawnPoint;

    private float angle;
    private Circle currentCircle;
    private bool isSpawning;

    private void Start()
    {
        GameplayManager.Instance.inputActions.Gameplay.DropCircle.performed += DropCircle;
    }

    private void OnDisable()
    {
        if (spawnPoint.childCount >= 1)
        {
            Destroy(spawnPoint.GetChild(0).gameObject);
        }

        currentCircle = null;
        isSpawning = false;
    }

    private void Update()
    {
        angle = amplitude * Mathf.Sin(Time.time * speed);
        transform.localRotation = Quaternion.Euler(0, 0, angle);

        if (!currentCircle && !isSpawning)
        {
            StartCoroutine(SpawnNewCircleWithDelay(1.5f));
        }
    }

    private IEnumerator SpawnNewCircleWithDelay(float delay)
    {
        isSpawning = true;

        yield return new WaitForSeconds(delay);

        currentCircle = GameplayManager.Instance.SpawnCircleRandom(spawnPoint.position);
        currentCircle.transform.parent = spawnPoint;
        currentCircle.transform.localPosition = Vector3.zero;

        isSpawning = false;
    }

    public void DropCircle(InputAction.CallbackContext context)
    {
        if (!currentCircle) return;

        if (context.performed && !GameplayManager.Instance.isTutorial)
        {
            currentCircle.SetKinematic(RigidbodyType2D.Dynamic);
            currentCircle.transform.parent = null;

            GameplayManager.Instance.TryDropCircle(currentCircle);

            currentCircle = null;
        }
    }
}
