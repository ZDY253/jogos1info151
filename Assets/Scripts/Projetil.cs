using UnityEngine;

/// <summary>
/// Projetil do jogador. Destroi inimigos e desaparece ao encostar no cenario.
/// </summary>
public class Projetil : MonoBehaviour
{
    [SerializeField] private float velocidade = 10f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool olhandoDireita = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void DefinirDirecao(bool direita)
    {
        olhandoDireita = direita;

        if (sprite != null)
        {
            sprite.flipX = !direita;
        }
    }

    private void Start()
    {
        rb.linearVelocity = new Vector2(
            (olhandoDireita ? 1f : -1f) * velocidade,
            0f
        );

        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
