using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;
    public int highScore = 0;
    public bool isGameOver = false;
    public GameObject duckPrefab;

    private GameObject player;
    private GameObject enemy;
    private GameObject currentDuck;
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
        else
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                BuyDuck();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                BuyBaby();
            }
        }
    }

    public void BuyDuck()
    {
        if (currentDuck != null) return;
        if (score >= 6)
        {
            score -= 6;
            if (duckPrefab != null && player != null)
            {
                currentDuck = Instantiate(duckPrefab, player.transform.position + new Vector3(-1f, 0f, 0f), Quaternion.identity);
            }
        }
    }

    public void BuyBaby()
    {
        if (score >= 8 && player != null)
        {
            var stack = player.GetComponent<BabyCapybaraStack>();
            if (stack != null && stack.AddBaby())
            {
                score -= 8;
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

        if (currentDuck != null)
        {
            Destroy(currentDuck);
            currentDuck = null;
        }

        if (player != null)
        {
            player.GetComponent<BabyCapybaraStack>()?.ClearBabies();
        }

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

        // Shop Prompt for Duck
        if (!isGameOver && currentDuck == null)
        {
            GUIStyle shopStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            if (GUI.Button(new Rect(Screen.width - 210, 20, 190, 45), "[B] Adopt Duck (6pts)", shopStyle))
            {
                BuyDuck();
            }
        }

        // Shop Prompt for Baby Capybara
        var stackComp = player != null ? player.GetComponent<BabyCapybaraStack>() : null;
        if (!isGameOver && (stackComp == null || stackComp.babyCount < stackComp.maxBabies))
        {
            GUIStyle babyStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            if (GUI.Button(new Rect(Screen.width - 210, 75, 190, 45), "[K] Baby Capy (8pts)", babyStyle))
            {
                BuyBaby();
            }
        }

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
