using UnityEngine;

/// <summary>
/// Inimigo que anda entre dois pontos.
/// O dano de contato fica no componente Spike do prefab.
/// </summary>
public class EnemyController : MonoBehaviour
{
    public Transform position1;
    public Transform position2;

    public float velocity = 2f;
    public float minimumDistance = 0.5f;

    private bool goingRight = true;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (position1 == null || position2 == null)
            return;

        float targetX = goingRight ? position2.position.x : position1.position.x;
        float distance = targetX - transform.position.x;

        if (Mathf.Abs(distance) <= Mathf.Max(0.1f, minimumDistance))
        {
            goingRight = !goingRight;
            targetX = goingRight ? position2.position.x : position1.position.x;
            distance = targetX - transform.position.x;
        }

        float direction = Mathf.Sign(distance);

        if (rb != null)
            rb.linearVelocity = new Vector2(direction * Mathf.Abs(velocity), rb.linearVelocity.y);

        if (sprite != null)
            sprite.flipX = direction < 0f;
    }
}
