using UnityEngine;

/// <summary>
/// Projetil disparado pelos inimigos atiradores.
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 9f;
    [SerializeField] private float lifeTime = 6f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        Vector2 dir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.left;
        rb.linearVelocity = dir * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health == null)
                health = other.GetComponentInParent<PlayerHealth>();

            if (health != null)
                health.Kill("Atingido por um projetil");

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Enemy"))
            return;

        Destroy(gameObject);
    }
}
