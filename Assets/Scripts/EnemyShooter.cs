using UnityEngine;

/// <summary>
/// Inimigo parado que atira no jogador quando ele se aproxima.
/// </summary>
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float range = 16f;
    [SerializeField] private float cooldown = 1.7f;
    [SerializeField] private float aimHeight = 1.2f;

    private Transform player;
    private float nextShot;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        nextShot = Time.time + cooldown;
    }

    private void Update()
    {
        if (player == null || projectilePrefab == null)
            return;

        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsLevelComplete))
            return;

        if (Time.time < nextShot)
            return;

        Vector2 origin = firePoint != null ? firePoint.position : transform.position;
        Vector2 target = (Vector2)player.position + Vector2.up * aimHeight;
        Vector2 direction = target - origin;

        if (direction.magnitude > range)
            return;

        nextShot = Time.time + cooldown;

        GameObject projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        EnemyProjectile script = projectile.GetComponent<EnemyProjectile>();

        if (script != null)
            script.Launch(direction);
    }
}
