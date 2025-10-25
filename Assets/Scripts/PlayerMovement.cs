using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Boundaries")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isGameOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // No gravity for top-down
    }

    void Update()
    {
        if (isGameOver) return;

        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (isGameOver) return;

        // Move player
        Vector2 newPosition = rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime;

        // Clamp position to boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        rb.MovePosition(newPosition);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Car"))
        {
            GameOver();
        }
        else if (collision.CompareTag("Coin"))
        {
            GameManager.Instance.AddScore(10);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Trash"))
        {
            GameManager.Instance.AddScore(5);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Finish"))
        {
            GameManager.Instance.LevelComplete();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        GameManager.Instance.GameOver();
        // Optional: Add death animation or effect
        gameObject.SetActive(false);
    }

    public void ResetPlayer()
    {
        isGameOver = false;
        transform.position = new Vector3(0, -4, 0);
        gameObject.SetActive(true);
    }
}