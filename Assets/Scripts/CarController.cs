using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public bool moveRight = true;

    [Header("Boundaries")]
    public float destroyDistance = 12f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Move car horizontally
        float direction = moveRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        // Destroy if too far from start
        if (Vector3.Distance(transform.position, startPosition) > destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        // Visualize movement direction in editor
        Gizmos.color = moveRight ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}