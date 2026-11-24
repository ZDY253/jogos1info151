using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// HUD, menu de fases e telas de Game Over / fase concluida.
/// Toda a interface e criada por codigo para nao depender de prefabs.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private enum EndMode { GameOver, Complete }

    private TMP_FontAsset font;
    private GameObject hudPanel;
    private GameObject menuPanel;
    private GameObject controlsHint;
    private GameObject overlay;
    private GameObject endPanel;

    private TextMeshProUGUI levelLabel;
    private TextMeshProUGUI coinsLabel;
    private TextMeshProUGUI totalLabel;
    private TextMeshProUGUI timerLabel;
    private TextMeshProUGUI endTitle;
    private TextMeshProUGUI endDescription;
    private TextMeshProUGUI primaryLabel;
    private TextMeshProUGUI secondaryLabel;
    private Button primaryButton;
    private Button secondaryButton;

    private EndMode endMode;

    public static void EnsureExists()
    {
        if (Instance != null)
            return;

        GameObject go = new GameObject("UIManager");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<UIManager>();
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

        font = TMP_Settings.defaultFontAsset;
        if (font == null)
            font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        BuildInterface();
    }

    private void BuildInterface()
    {
        GameObject canvasObject = new GameObject("Canvas Jogo", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        BuildHud(canvasObject.transform);
        BuildMenu(canvasObject.transform);
        BuildOverlay(canvasObject.transform);
        BuildControlsHint(canvasObject.transform);
    }

    private void BuildHud(Transform parent)
    {
        Image panel = MakeImage(parent, "HUD", new Color(0f, 0f, 0f, 0.45f));
        RectTransform rt = panel.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(24f, -24f);
        rt.sizeDelta = new Vector2(560f, 190f);

        hudPanel = panel.gameObject;

        levelLabel = MakeLabel(panel.transform, "Fase", 30, TextAlignmentOptions.Left, Color.white,
            new Vector2(18f, -14f), new Vector2(520f, 38f));
        levelLabel.fontStyle = FontStyles.Bold;

        coinsLabel = MakeLabel(panel.transform, "Moedas", 30, TextAlignmentOptions.Left, new Color(1f, 0.9f, 0.35f),
            new Vector2(18f, -56f), new Vector2(520f, 36f));

        totalLabel = MakeLabel(panel.transform, "Total", 24, TextAlignmentOptions.Left, new Color(0.85f, 0.9f, 1f),
            new Vector2(18f, -96f), new Vector2(520f, 34f));

        timerLabel = MakeLabel(panel.transform, "Tempo", 28, TextAlignmentOptions.Left, Color.white,
            new Vector2(18f, -136f), new Vector2(520f, 36f));
    }

    private void BuildMenu(Transform parent)
    {
        Image panel = MakeImage(parent, "Menu de Fases", new Color(0.04f, 0.05f, 0.1f, 0.88f));
        RectTransform rt = panel.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-24f, -24f);
        rt.sizeDelta = new Vector2(480f, 300f);

        menuPanel = panel.gameObject;

        TextMeshProUGUI title = MakeLabel(panel.transform, "Titulo", 30, TextAlignmentOptions.Left, Color.white,
            new Vector2(18f, -14f), new Vector2(440f, 38f));
        title.text = "Escolher fase";
        title.fontStyle = FontStyles.Bold;

        for (int i = 0; i < 5; i++)
        {
            int number = i + 1;

            Button button = MakeButton(panel.transform, "Fase " + number, number.ToString(), 28,
                new Color(0.18f, 0.35f, 0.62f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(18f + i * 88f, -62f), new Vector2(76f, 58f), out _);

            button.onClick.AddListener(() =>
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.LoadLevel(number);
            });
        }

        Button quit = MakeButton(panel.transform, "Sair", "Sair", 24,
            new Color(0.55f, 0.18f, 0.18f), new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(18f, -136f), new Vector2(180f, 52f), out _);

        quit.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.QuitGame();
        });

        TextMeshProUGUI info = MakeLabel(panel.transform, "Info", 20, TextAlignmentOptions.TopLeft,
            new Color(0.85f, 0.88f, 0.95f),
            new Vector2(18f, -202f), new Vector2(440f, 80f));
        info.text = "Setas ou A/D: mover\nEspaco: pular    Botao direito: atirar\nColete as moedas e chegue ao portal!";
    }

    private void BuildOverlay(Transform parent)
    {
        Image background = MakeImage(parent, "Overlay", new Color(0f, 0f, 0f, 0.72f));
        Stretch(background.rectTransform);

        overlay = background.gameObject;

        Image panel = MakeImage(background.transform, "Painel Fim", new Color(0.07f, 0.08f, 0.14f, 0.98f));
        RectTransform prt = panel.rectTransform;
        prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0.5f);
        prt.pivot = new Vector2(0.5f, 0.5f);
        prt.anchoredPosition = Vector2.zero;
        prt.sizeDelta = new Vector2(880f, 500f);
        endPanel = panel.gameObject;

        endTitle = MakeLabel(panel.transform, "Titulo Fim", 56, TextAlignmentOptions.Center, Color.white,
            new Vector2(0f, 160f), new Vector2(820f, 90f));
        endTitle.fontStyle = FontStyles.Bold;
        CenterAnchor(endTitle.rectTransform);

        endDescription = MakeLabel(panel.transform, "Descricao Fim", 30, TextAlignmentOptions.Center,
            new Color(0.9f, 0.93f, 1f), new Vector2(0f, 40f), new Vector2(800f, 180f));
        CenterAnchor(endDescription.rectTransform);

        primaryButton = MakeButton(panel.transform, "Botao Principal", "Tentar de novo", 28,
            new Color(0.16f, 0.55f, 0.28f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-160f, -160f), new Vector2(280f, 72f), out primaryLabel);

        secondaryButton = MakeButton(panel.transform, "Botao Secundario", "Menu", 28,
            new Color(0.25f, 0.3f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(160f, -160f), new Vector2(280f, 72f), out secondaryLabel);

        primaryButton.onClick.AddListener(OnPrimaryClicked);
        secondaryButton.onClick.AddListener(OnSecondaryClicked);

        overlay.SetActive(false);
    }

    private void BuildControlsHint(Transform parent)
    {
        TextMeshProUGUI hint = MakeLabel(parent, "Controles", 20, TextAlignmentOptions.Left,
            new Color(1f, 1f, 1f, 0.75f), new Vector2(24f, 20f), new Vector2(900f, 40f));

        RectTransform rt = hint.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot = new Vector2(0f, 0f);

        hint.text = "Setas/A-D: mover    Espaco: pular    Botao direito: atirar";
        hint.gameObject.name = "Dica de Controles";

        controlsHint = hint.gameObject;
    }

    public void SetupForScene(LevelConfig config)
    {
        EnsureEventSystem();

        bool gameplay = config != null;

        hudPanel.SetActive(gameplay);

        // A cena Menu ja tem a propria interface feita a mao.
        // Nesse caso nao mostramos o menu extra criado por codigo.
        bool sceneHasOwnCanvas = SceneHasExternalCanvas();

        menuPanel.SetActive(!gameplay && !sceneHasOwnCanvas);
        controlsHint.SetActive(gameplay);
        overlay.SetActive(false);

        if (gameplay)
            RefreshHud();
        else
            timerLabel.gameObject.SetActive(false);
    }

    private bool SceneHasExternalCanvas()
    {
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        foreach (Canvas canvas in canvases)
        {
            if (!canvas.transform.IsChildOf(transform))
                return true;
        }

        return false;
    }

    private void Update()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager == null || !gameManager.LevelActive)
            return;

        RefreshHud();
    }

    private void RefreshHud()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null)
            return;

        LevelConfig config = gameManager.Config;

        levelLabel.text = config != null ? config.levelTitle : "Fase";

        int total = config != null ? config.totalCoins : 0;
        coinsLabel.text = "Moedas: " + gameManager.LevelCoins + "/" + total;
        totalLabel.text = "Total: " + gameManager.TotalCoins + "    Mortes: " + gameManager.Deaths;

        if (config != null && config.timeLimit > 0f)
        {
            timerLabel.gameObject.SetActive(true);

            float time = Mathf.Max(0f, gameManager.TimeRemaining);
            timerLabel.text = "Tempo: " + Mathf.CeilToInt(time) + "s";
            timerLabel.color = time <= 10f ? new Color(1f, 0.35f, 0.35f) : Color.white;
        }
        else
        {
            timerLabel.gameObject.SetActive(false);
        }
    }

    public void ShowGameOver(string reason, int coins, int totalCoins, int deaths)
    {
        endMode = EndMode.GameOver;

        overlay.SetActive(true);
        endTitle.text = "Voce morreu!";
        endTitle.color = new Color(1f, 0.42f, 0.42f);
        endDescription.text = "Motivo: <b>" + reason + "</b>\n" +
                              "Moedas nesta fase: " + coins + "/" + totalCoins + "\n" +
                              "Mortes: " + deaths;

        primaryLabel.text = "Tentar de novo";
        secondaryLabel.text = "Menu";
        secondaryButton.gameObject.SetActive(true);

        RefreshHud();
    }

    public void ShowLevelComplete(int coins, int totalCoins, bool isLastLevel)
    {
        endMode = EndMode.Complete;

        overlay.SetActive(true);
        endTitle.text = isLastLevel ? "Voce venceu!" : "Fase concluida!";
        endTitle.color = new Color(0.45f, 1f, 0.55f);
        endDescription.text = "Moedas nesta fase: <b>" + coins + "/" + totalCoins + "</b>\n" +
                              "Moedas totais: " + GameManager.Instance.TotalCoins;

        primaryLabel.text = isLastLevel ? "Voltar ao menu" : "Proxima fase";
        secondaryLabel.text = "Menu";
        secondaryButton.gameObject.SetActive(!isLastLevel);

        RefreshHud();
    }

    private void OnPrimaryClicked()
    {
        if (GameManager.Instance == null)
            return;

        if (endMode == EndMode.GameOver)
            GameManager.Instance.RestartLevel();
        else
            GameManager.Instance.NextLevel();
    }

    private void OnSecondaryClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadMenu();
    }

    private void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        InputSystemUIInputModule module = eventSystem.GetComponent<InputSystemUIInputModule>();
        module.AssignDefaultActions();
    }

    private TextMeshProUGUI MakeLabel(Transform parent, string name, float size, TextAlignmentOptions alignment,
        Color color, Vector2 position, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = position;
        rt.sizeDelta = sizeDelta;

        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.text = "";

        return text;
    }

    private Button MakeButton(Transform parent, string name, string label, float fontSize, Color color,
        Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 sizeDelta, out TextMeshProUGUI labelText)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = position;
        rt.sizeDelta = sizeDelta;

        Image image = go.GetComponent<Image>();
        image.color = color;

        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;

        GameObject textObject = new GameObject("Texto", typeof(RectTransform));
        textObject.transform.SetParent(go.transform, false);
        Stretch(textObject.GetComponent<RectTransform>());

        labelText = textObject.AddComponent<TextMeshProUGUI>();
        labelText.font = font;
        labelText.fontSize = fontSize;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = Color.white;
        labelText.raycastTarget = false;
        labelText.text = label;

        return button;
    }

    private Image MakeImage(Transform parent, string name, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        Image image = go.GetComponent<Image>();
        image.color = color;

        return image;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void CenterAnchor(RectTransform rt)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
}
