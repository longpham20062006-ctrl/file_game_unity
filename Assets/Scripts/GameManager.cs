using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public BubbleGrid bubbleGrid;

    [Header("Lose Zone")]
    public string loseZoneObjectName = "LoseZone";
    public Transform loseZoneWorld;

    [Header("Lose Line UI")]
    public RectTransform loseLineUI;

    [Header("UI")]
    public Text scoreText;
    public GameObject resultPanel;
    public Text resultTitleText;
    public Text finalScoreText;

    [Header("Score Settings")]
    public int scorePerBubble = 10;
    public int scorePerDroppedBubble = 20;

    [Header("New Row Settings")]
    public int pointsPerNewRow = 100;
    public int nextNewRowScore = 100;

    [Header("Lose Settings")]
    public float bubbleRadiusWorld = 0.32f;
    public bool onlyCheckGridBubbles = true;

    [Header("Debug")]
    public bool showDebugLog = false;

    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        IsGameOver = false;
        Score = 0;
        nextNewRowScore = pointsPerNewRow;

        AutoFindReferences();
        UpdateScoreUI();

        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    private void Update()
    {
        if (IsGameOver)
            return;

        if (loseZoneWorld == null)
            AutoFindLoseZone();

        CheckLoseCondition();
    }

    private void AutoFindReferences()
    {
        if (bubbleGrid == null)
            bubbleGrid = FindObjectOfType<BubbleGrid>();

        if (scoreText == null)
        {
            GameObject obj = GameObject.Find("ScoreText");

            if (obj != null)
                scoreText = obj.GetComponent<Text>();
        }

        if (resultPanel == null)
        {
            GameObject obj = GameObject.Find("ResultPanel");

            if (obj != null)
                resultPanel = obj;
        }

        if (resultTitleText == null)
        {
            GameObject obj = GameObject.Find("ResultTitleText");

            if (obj != null)
                resultTitleText = obj.GetComponent<Text>();
        }

        if (finalScoreText == null)
        {
            GameObject obj = GameObject.Find("FinalScoreText");

            if (obj != null)
                finalScoreText = obj.GetComponent<Text>();
        }

        if (loseLineUI == null)
        {
            GameObject obj = GameObject.Find("LoseLineUI");

            if (obj != null)
                loseLineUI = obj.GetComponent<RectTransform>();
        }

        AutoFindLoseZone();
    }

    private void AutoFindLoseZone()
    {
        GameObject obj = GameObject.Find(loseZoneObjectName);

        if (obj != null)
            loseZoneWorld = obj.transform;
    }

    public void OnAfterBubbleAttached(int destroyedCount, int remainingCount)
    {
        if (IsGameOver)
            return;

        int droppedCount = 0;

        if (bubbleGrid != null)
            droppedCount = bubbleGrid.LastDroppedCount;

        if (destroyedCount >= 3)
            Score += destroyedCount * scorePerBubble;

        if (droppedCount > 0)
            Score += droppedCount * scorePerDroppedBubble;

        UpdateScoreUI();

        CheckScoreForNewRows();

        if (remainingCount <= 0)
        {
            ShowResult("VICTORY");
            return;
        }

        CheckLoseCondition();
    }

    private void CheckScoreForNewRows()
    {
        if (bubbleGrid == null)
            return;

        if (Score >= nextNewRowScore)
        {
            bubbleGrid.AddNewTopRow();
            nextNewRowScore += pointsPerNewRow;

            UpdateScoreUI();
            CheckLoseCondition();
        }
    }

    private void CheckLoseCondition()
    {
        if (loseZoneWorld == null)
            return;

        float loseY = loseZoneWorld.position.y;

        Bubble[] allBubbles = FindObjectsOfType<Bubble>();

        foreach (Bubble bubble in allBubbles)
        {
            if (bubble == null)
                continue;

            if (onlyCheckGridBubbles && !bubble.isOnGrid)
                continue;

            float bubbleBottomY = bubble.transform.position.y - bubbleRadiusWorld;

            if (showDebugLog)
            {
                Debug.Log(
                    "Bubble = " + bubble.name +
                    " | isOnGrid = " + bubble.isOnGrid +
                    " | BottomY = " + bubbleBottomY +
                    " | LoseY = " + loseY
                );
            }

            if (bubbleBottomY <= loseY)
            {
                ForceGameOver();
                return;
            }
        }
    }

    public void ForceGameOver()
    {
        if (IsGameOver)
            return;

        ShowResult("GAME OVER");
    }

    private void ShowResult(string title)
    {
        IsGameOver = true;

        if (resultPanel == null)
            AutoFindReferences();

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            resultPanel.transform.SetAsLastSibling();

            CanvasGroup group = resultPanel.GetComponent<CanvasGroup>();

            if (group == null)
                group = resultPanel.AddComponent<CanvasGroup>();

            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            Debug.LogError("ResultPanel vẫn đang None. Không thể hiện màn hình thua.");
        }

        if (resultTitleText != null)
        {
            resultTitleText.gameObject.SetActive(true);
            resultTitleText.text = title;
        }

        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(true);
            finalScoreText.text = "Score: " + Score;
        }

        StopAllMovingBubbles();

        Time.timeScale = 0f;
    }

    private void StopAllMovingBubbles()
    {
        Bubble[] allBubbles = FindObjectsOfType<Bubble>();

        foreach (Bubble bubble in allBubbles)
        {
            if (bubble == null)
                continue;

            Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();

            if (rb == null)
                continue;

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + Score;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}