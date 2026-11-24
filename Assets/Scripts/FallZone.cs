using UnityEngine;

/// <summary>
/// Zona abaixo do mapa. Matar o jogador ao cair no vazio.
/// </summary>
public class FallZone : MonoBehaviour
{
    [SerializeField] private string deathReason = "Caiu no abismo";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            health = other.GetComponentInParent<PlayerHealth>();

        if (health != null)
            health.Kill(deathReason);
    }
}
