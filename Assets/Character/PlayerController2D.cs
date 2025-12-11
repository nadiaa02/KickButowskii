using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 40f;      
    [SerializeField] private float jumpForce = 20f;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isAlive = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Debug.Log(isGrounded);

        if (!isAlive) return;

        if (groundCheckPoint != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheckPoint.position,
                groundCheckRadius,
                groundLayer
            );
        }
        


        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }


    private void FixedUpdate()
    {
        if (!isAlive) return;

        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Antenna"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (!isAlive) return;
        isAlive = false;

        rb.velocity = Vector2.zero;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
