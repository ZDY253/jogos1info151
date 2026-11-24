using UnityEngine;

/// <summary>
/// Configuracao de cada fase. O gerador de fases cria este componente em cada cena.
/// </summary>
public class LevelConfig : MonoBehaviour
{
    public int levelNumber = 1;
    public string levelTitle = "Fase 1";
    public int totalCoins = 0;

    [Tooltip("0 = sem limite de tempo")]
    public float timeLimit = 0f;

    [Tooltip("Nome da proxima cena. Vazio = ultima fase.")]
    public string nextScene = "";
}
