using System.Collections;
using UnityEngine;

/// <summary>
/// Controla a vida do jogador. Qualquer perigo chama Kill(motivo).
/// O motivo aparece na tela de Game Over, provando as diferentes formas de morrer.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Current { get; private set; }

    public bool IsDead { get; private set; }

    private PlatformController controller;
    private Atirar shooter;
    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Current = this;

        controller = GetComponent<PlatformController>();
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shooter = GetComponentInChildren<Atirar>();
    }

    private void OnDestroy()
    {
        if (Current == this)
            Current = null;
    }

    public void Kill(string reason)
    {
        if (IsDead)
            return;

        IsDead = true;

        if (controller != null)
            controller.SetControlEnabled(false);

        if (shooter != null)
            shooter.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (bodyCollider != null)
            bodyCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0.25f, 0.25f, 1f);

        StartCoroutine(NotifyGameManager(reason));
    }

    private IEnumerator NotifyGameManager(string reason)
    {
        yield return new WaitForSecondsRealtime(0.35f);

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied(reason);
        else
            Debug.LogWarning("Jogador morreu: " + reason);
    }
}
