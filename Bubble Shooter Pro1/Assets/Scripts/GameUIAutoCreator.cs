using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1000)]
public class GameUIAutoCreator : MonoBehaviour
{
    [Header("Canvas Settings")]
    public string canvasName = "Canvas";

    [Header("UI Names")]
    public string scoreTextName = "ScoreText";
    public string loseLineName = "LoseLineUI";
    public string resultPanelName = "ResultPanel";
    public string resultTitleName = "ResultTitleText";
    public string finalScoreName = "FinalScoreText";
    public string replayButtonName = "ReplayButton";
    public string exitButtonName = "ExitButton";

    private GameManager gameManager;
    private Canvas canvas;
    private Font defaultFont;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        CreateEventSystemIfNeeded();
        CreateCanvasIfNeeded();
        CreateScoreText();
        CreateLoseLine();
        CreateResultPanel();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            Text scoreText = FindText(scoreTextName);
            GameObject resultPanel = GameObject.Find(resultPanelName);
            Text resultTitleText = FindText(resultTitleName);
            Text finalScoreText = FindText(finalScoreName);
            Image loseLine = FindImage(loseLineName);

            if (scoreText != null)
                gameManager.scoreText = scoreText;

            if (resultPanel != null)
                gameManager.resultPanel = resultPanel;

            if (resultTitleText != null)
                gameManager.resultTitleText = resultTitleText;

            if (finalScoreText != null)
                gameManager.finalScoreText = finalScoreText;

            if (loseLine != null)
                gameManager.loseLineUI = loseLine.GetComponent<RectTransform>();
        }
    }

    private void CreateEventSystemIfNeeded()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem != null)
            return;

        GameObject obj = new GameObject("EventSystem");
        obj.AddComponent<EventSystem>();
        obj.AddComponent<StandaloneInputModule>();
    }

    private void CreateCanvasIfNeeded()
    {
        GameObject canvasObj = GameObject.Find(canvasName);

        if (canvasObj == null)
            canvasObj = new GameObject(canvasName);

        canvas = canvasObj.GetComponent<Canvas>();

        if (canvas == null)
            canvas = canvasObj.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();

        if (scaler == null)
            scaler = canvasObj.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        if (canvasObj.GetComponent<GraphicRaycaster>() == null)
            canvasObj.AddComponent<GraphicRaycaster>();
    }

    private void CreateScoreText()
    {
        Text scoreText = FindText(scoreTextName);

        if (scoreText == null)
        {
            GameObject obj = new GameObject(scoreTextName);
            obj.transform.SetParent(canvas.transform, false);
            scoreText = obj.AddComponent<Text>();
        }

        scoreText.font = defaultFont;
        scoreText.text = "Score: 0";
        scoreText.fontSize = 46;
        scoreText.color = Color.black;
        scoreText.alignment = TextAnchor.MiddleLeft;
        scoreText.raycastTarget = false;

        RectTransform rect = scoreText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(25f, -20f);
        rect.sizeDelta = new Vector2(460f, 90f);

        if (gameManager != null)
            gameManager.scoreText = scoreText;
    }

    private void CreateLoseLine()
    {
        Image line = FindImage(loseLineName);

        if (line == null)
        {
            GameObject obj = new GameObject(loseLineName);
            obj.transform.SetParent(canvas.transform, false);
            line = obj.AddComponent<Image>();
        }

        line.color = new Color(0.95f, 0f, 0.05f, 1f);
        line.raycastTarget = false;

        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        // Vạch thua thấp hơn nữa.
        rect.anchoredPosition = new Vector2(0f, -520f);

        // Vạch ngang gần hết màn hình 9:16.
        rect.sizeDelta = new Vector2(1080f, 18f);

        if (gameManager != null)
            gameManager.loseLineUI = rect;
    }

    private void CreateResultPanel()
    {
        GameObject panelObj = GameObject.Find(resultPanelName);

        if (panelObj == null)
        {
            panelObj = new GameObject(resultPanelName);
            panelObj.transform.SetParent(canvas.transform, false);
        }

        Image panelImage = panelObj.GetComponent<Image>();

        if (panelImage == null)
            panelImage = panelObj.AddComponent<Image>();

        panelImage.color = new Color(0f, 0f, 0f, 0.82f);
        panelImage.raycastTarget = true;

        CanvasGroup group = panelObj.GetComponent<CanvasGroup>();

        if (group == null)
            group = panelObj.AddComponent<CanvasGroup>();

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.localScale = Vector3.one;

        panelObj.transform.SetAsLastSibling();

        Text titleText = CreateTextUnderPanel(
            panelObj.transform,
            resultTitleName,
            "GAME OVER",
            88,
            Color.white,
            new Vector2(0f, 360f),
            new Vector2(900f, 150f)
        );

        Text finalScoreText = CreateTextUnderPanel(
            panelObj.transform,
            finalScoreName,
            "Score: 0",
            54,
            Color.white,
            new Vector2(0f, 230f),
            new Vector2(800f, 100f)
        );

        Button replayButton = CreateButtonUnderPanel(
            panelObj.transform,
            replayButtonName,
            "Chơi lại",
            new Vector2(0f, 80f),
            new Vector2(520f, 120f),
            44
        );

        Button exitButton = CreateButtonUnderPanel(
            panelObj.transform,
            exitButtonName,
            "Thoát",
            new Vector2(0f, -80f),
            new Vector2(520f, 120f),
            44
        );

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(() =>
        {
            GameManager manager = FindObjectOfType<GameManager>();

            if (manager != null)
                manager.RestartGame();
        });

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() =>
        {
            GameManager manager = FindObjectOfType<GameManager>();

            if (manager != null)
                manager.ExitGame();
        });

        if (gameManager != null)
        {
            gameManager.resultPanel = panelObj;
            gameManager.resultTitleText = titleText;
            gameManager.finalScoreText = finalScoreText;
        }

        panelObj.SetActive(false);
    }

    private Text CreateTextUnderPanel(
        Transform parent,
        string name,
        string content,
        int fontSize,
        Color color,
        Vector2 anchoredPosition,
        Vector2 size
    )
    {
        Text text = FindText(name);

        if (text == null)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            text = obj.AddComponent<Text>();
        }

        text.font = defaultFont;
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;

        return text;
    }

    private Button CreateButtonUnderPanel(
        Transform parent,
        string name,
        string label,
        Vector2 anchoredPosition,
        Vector2 size,
        int fontSize
    )
    {
        Button button = FindButton(name);
        GameObject obj;

        if (button == null)
        {
            obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            Image image = obj.AddComponent<Image>();
            image.color = Color.white;
            image.raycastTarget = true;

            button = obj.AddComponent<Button>();
        }
        else
        {
            obj = button.gameObject;
        }

        Image buttonImage = obj.GetComponent<Image>();

        if (buttonImage != null)
        {
            buttonImage.color = new Color(1f, 1f, 1f, 0.95f);
            buttonImage.raycastTarget = true;
        }

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;

        Text text = button.GetComponentInChildren<Text>();

        if (text == null)
        {
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(obj.transform, false);
            text = textObj.AddComponent<Text>();
        }

        text.font = defaultFont;
        text.text = label;
        text.fontSize = fontSize;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        textRect.localScale = Vector3.one;

        return button;
    }

    private Text FindText(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);

        if (obj == null)
            return null;

        return obj.GetComponent<Text>();
    }

    private Image FindImage(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);

        if (obj == null)
            return null;

        return obj.GetComponent<Image>();
    }

    private Button FindButton(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);

        if (obj == null)
            return null;

        return obj.GetComponent<Button>();
    }
}