using UnityEngine;

/// <summary>
/// Moeda coletavel. O contador fica no GameManager e aparece no HUD.
/// </summary>
public class coincontroller : MonoBehaviour
{
    [SerializeField] private int valor = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance != null)
            GameManager.Instance.CollectCoin(valor);

        PlatformController player = other.GetComponent<PlatformController>();
        if (player == null)
            player = other.GetComponentInParent<PlatformController>();

        if (player != null)
            player.ChangeTextCoin();

        Destroy(gameObject);
    }
}
