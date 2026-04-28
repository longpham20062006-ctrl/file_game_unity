using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isGameOver = false;
    public float score = 0f;
    public float obstacleSpeed = 6f;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!isGameOver)
        {
            score += Time.deltaTime * 10f;
        }

        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
    }

    private void OnGUI()
    {
        GUIStyle scoreStyle = new GUIStyle(GUI.skin.label);
        scoreStyle.fontSize = 24;
        scoreStyle.normal.textColor = Color.black;

        GUI.Label(new Rect(20, 20, 200, 40), "Score: " + Mathf.FloorToInt(score), scoreStyle);

        if (isGameOver)
        {
            GUIStyle gameOverStyle = new GUIStyle(GUI.skin.label);
            gameOverStyle.fontSize = 40;
            gameOverStyle.normal.textColor = Color.red;
            gameOverStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Label(
                new Rect(Screen.width / 2 - 200, Screen.height / 2 - 60, 400, 50),
                "GAME OVER",
                gameOverStyle
            );

            GUIStyle restartStyle = new GUIStyle(GUI.skin.label);
            restartStyle.fontSize = 22;
            restartStyle.normal.textColor = Color.black;
            restartStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Label(
                new Rect(Screen.width / 2 - 200, Screen.height / 2, 400, 40),
                "Nhấn R để chơi lại",
                restartStyle
            );
        }
    }
}