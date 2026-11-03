using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;
    public int highScore = 0;
    public bool isGameOver = false;

    private GameObject player;
    private GameObject enemy;
    private Texture2D bgTexture;

    void Awake()
    {
        instance = this;

        bgTexture = new Texture2D(1, 1);
        bgTexture.SetPixel(0, 0, new Color(0.1f, 0.12f, 0.15f, 0.85f));
        bgTexture.Apply();
    }

    void Start()
    {
        player = GameObject.Find("Capybara");
        enemy = GameObject.Find("Crocodile");
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }

    public void AddScore(int points)
    {
        score += points;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void RestartGame()
    {
        score = 0;
        isGameOver = false;

        if (player != null)
        {
            player.transform.position = Vector3.zero;
            player.GetComponent<CapybaraSleep>()?.WakeUp();
            player.GetComponent<CapybaraEat>()?.StopEating();
            player.GetComponent<CapybaraBath>()?.StopBathing();
        }

        if (enemy != null)
        {
            var ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.Respawn();
            }
            else
            {
                enemy.transform.position = new Vector3(-5.5f, -2.5f, 0f);
            }
        }

        var spawner = FindObjectOfType<FoodSpawner>();
        if (spawner != null)
        {
            spawner.ClearFood();
            spawner.SpawnFood();
        }
    }

    void OnGUI()
    {
        // Scoreboard Box
        GUIStyle scoreBoxStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        if (bgTexture != null)
        {
            scoreBoxStyle.normal.background = bgTexture;
        }
        scoreBoxStyle.normal.textColor = Color.white;

        GUI.Box(new Rect(20, 20, 240, 48), "Score: " + score + "  |  Best: " + highScore, scoreBoxStyle);

        // Game Over Panel
        if (isGameOver)
        {
            float width = 340f;
            float height = 230f;
            float x = (Screen.width - width) / 2f;
            float y = (Screen.height - height) / 2f;

            GUIStyle panelStyle = new GUIStyle(GUI.skin.box);
            if (bgTexture != null)
            {
                panelStyle.normal.background = bgTexture;
            }

            GUI.Box(new Rect(x, y, width, height), "", panelStyle);

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = new Color(1f, 0.35f, 0.35f);
            GUI.Label(new Rect(x, y + 15, width, 40), "GAME OVER", titleStyle);

            GUIStyle finalStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 19,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            finalStyle.normal.textColor = Color.yellow;
            GUI.Label(new Rect(x, y + 60, width, 30), "Final Score: " + score, finalStyle);

            GUIStyle bestStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            bestStyle.normal.textColor = new Color(1f, 0.85f, 0.4f);
            GUI.Label(new Rect(x, y + 95, width, 30), "High Score: " + highScore, bestStyle);

            GUI.skin.button.fontSize = 18;
            if (GUI.Button(new Rect(x + 75, y + 145, 190, 50), "Play Again"))
            {
                RestartGame();
            }
        }
    }
}
