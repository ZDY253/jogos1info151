using UnityEngine;

/// <summary>
/// Perigo generico de contato: espinhos, inimigos, lava...
/// O motivo da morte pode ser configurado no Inspector.
/// </summary>
public class Spike : MonoBehaviour
{
    [SerializeField] private string deathReason = "";

    private void Awake()
    {
        if (string.IsNullOrEmpty(deathReason))
        {
            deathReason = gameObject.CompareTag("Enemy")
                ? "Tocado por um inimigo"
                : "Espinhos";
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryKill(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryKill(other.gameObject);
    }

    private void TryKill(GameObject other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            health = other.GetComponentInParent<PlayerHealth>();

        if (health != null)
            health.Kill(deathReason);
    }

    public void SetDeathReason(string reason)
    {
        deathReason = reason;
    }
}
