using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o jogo inteiro: fases, moedas, mortes, tempo e transicoes.
/// Criado automaticamente antes da primeira cena carregar.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Cenas (na ordem do jogo)")]
    public string[] levelScenes = { "level 1", "level2", "level 3", "level 4", "level 5" };
    public string menuScene = "Menu";

    public int LevelNumber { get; private set; }
    public int LevelCoins { get; private set; }
    public int TotalCoins { get; private set; }
    public int Deaths { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool LevelActive { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsLevelComplete { get; private set; }
    public LevelConfig Config => levelConfig;

    private LevelConfig levelConfig;
    private string previousScene = "";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject go = new GameObject("GameManager");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<GameManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        levelConfig = Object.FindFirstObjectByType<LevelConfig>();
        LevelCoins = 0;
        IsGameOver = false;
        IsLevelComplete = false;

        if (levelConfig != null)
        {
            LevelActive = true;
            LevelNumber = levelConfig.levelNumber;
            TimeRemaining = levelConfig.timeLimit;

            // Sempre que o jogador sai do menu e comeca uma partida nova,
            // zera o progresso da partida anterior.
            if (previousScene == menuScene)
            {
                TotalCoins = 0;
                Deaths = 0;
            }
        }
        else
        {
            LevelActive = false;
            LevelNumber = 0;
            TimeRemaining = 0f;
        }

        previousScene = scene.name;

        UIManager.EnsureExists();
        UIManager.Instance.SetupForScene(levelConfig);
    }

    private void Update()
    {
        if (!LevelActive || IsGameOver || IsLevelComplete || levelConfig == null)
            return;

        if (levelConfig.timeLimit <= 0f)
            return;

        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            KillPlayer("Tempo esgotado");
        }
    }

    public void CollectCoin(int amount = 1)
    {
        LevelCoins += amount;
        TotalCoins += amount;
    }

    private void KillPlayer(string reason)
    {
        if (PlayerHealth.Current != null)
            PlayerHealth.Current.Kill(reason);
        else
            PlayerDied(reason);
    }

    /// <summary>Chamado pelo PlayerHealth quando o personagem morre.</summary>
    public void PlayerDied(string reason)
    {
        if (IsGameOver || IsLevelComplete)
            return;

        IsGameOver = true;
        Deaths++;
        Time.timeScale = 0f;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver(reason, LevelCoins, levelConfig != null ? levelConfig.totalCoins : 0, Deaths);
    }

    /// <summary>Chamado pelo portal no fim da fase.</summary>
    public void CompleteLevel()
    {
        if (IsGameOver || IsLevelComplete || levelConfig == null)
            return;

        IsLevelComplete = true;
        Time.timeScale = 0f;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowLevelComplete(LevelCoins, levelConfig.totalCoins, string.IsNullOrEmpty(levelConfig.nextScene));
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        TotalCoins = Mathf.Max(0, TotalCoins - LevelCoins);

        string current = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(current);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        if (levelConfig != null && !string.IsNullOrEmpty(levelConfig.nextScene))
            SceneManager.LoadScene(levelConfig.nextScene);
        else
            LoadMenu();
    }

    public void LoadLevel(int number)
    {
        if (number < 1 || number > levelScenes.Length)
            return;

        Time.timeScale = 1f;

        if (number == 1)
        {
            TotalCoins = 0;
            Deaths = 0;
        }

        SceneManager.LoadScene(levelScenes[number - 1]);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
