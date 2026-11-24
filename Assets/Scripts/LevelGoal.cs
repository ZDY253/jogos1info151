using UnityEngine;

/// <summary>
/// Portal no fim da fase. Encostar nele conclui o nivel.
/// </summary>
public class LevelGoal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance != null)
            GameManager.Instance.CompleteLevel();
    }
}
