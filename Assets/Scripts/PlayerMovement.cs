using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 10f;
    private Rigidbody2D rb;
    private Vector2 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) TryPickup();

        movement.Normalize();
        // movement = movement.normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.deltaTime);
        // Debug.Log(movement);
    }

    void TryPickup()
    {
        // float radius = 0.6f;
        // Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        // foreach (var hit in hits)
        // {
        //     Trash t = hit.GetComponent<Trash>();
        //     if (t != null && !t.IsCollected)
        //     {
        //         t.Collect();
        //         GameManager.Instance.AddScore(10);
        //         break;
        //     }
        // }


    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            GameManager.Instance.AddScore(10);
            Destroy(collision.gameObject);
        }
    }
}
