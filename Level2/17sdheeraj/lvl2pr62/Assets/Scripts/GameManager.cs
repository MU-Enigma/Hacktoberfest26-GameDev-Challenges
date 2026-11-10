using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;
    public int highScore = 0;
    public bool isGameOver = false;
    public GameObject duckPrefab;

    [HideInInspector]
    public bool hasUsedRevive = false;
    [HideInInspector]
    public bool isReviving = false;
    [HideInInspector]
    public string currentSpell = "";
    [HideInInspector]
    public string typedInput = "";

    private readonly string[] revivalSpells = new string[]
    {
        "Wake up, you majestic potato chunk!",
        "Back to reality, you beautiful mud pancake!",
        "Stop napping, you absolute citrus goblin!",
        "Return to the realm, you glorious pond log!",
        "Pull yourself together, you supreme swamp nugget!",
        "Time to float again, you legendary water pig!",
        "Unbake yourself, you pristine thermal loaf!",
        "Rebooting systems, you maximum density grass puppy!"
    };

    private GameObject player;
    private GameObject enemy;
    private GameObject currentDuck;
    private Texture2D bgTexture;

    void Awake()
    {
        instance = this;

        Camera cam = Camera.main;
        if (cam == null) cam = FindAnyObjectByType<Camera>();
        if (cam != null)
        {
            cam.enabled = true;
            cam.targetDisplay = 0;
            cam.stereoTargetEye = StereoTargetEyeMask.None;
        }

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
            if (!isReviving && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R)))
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
        if (!hasUsedRevive)
        {
            isReviving = true;
            typedInput = "";
            currentSpell = revivalSpells[Random.Range(0, revivalSpells.Length)];
        }
        else
        {
            isReviving = false;
        }
    }

    public void RevivePlayer()
    {
        isGameOver = false;
        isReviving = false;
        hasUsedRevive = true;

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

        if (player != null)
        {
            player.GetComponent<GoldenPower>()?.ActivateGolden(4f);
        }
    }

    public void RestartGame()
    {
        score = 0;
        isGameOver = false;
        hasUsedRevive = false;
        isReviving = false;
        typedInput = "";
        currentSpell = "";

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

        CelebrationManager.instance?.ResetCelebration();
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

        // Top-Right Abilities & Controls List
        if (!isGameOver)
        {
            DrawAbilitiesList();
        }

        // One-time Typing Spell Revive Prompt
        if (isGameOver && isReviving)
        {
            float width = 540f;
            float height = 290f;
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
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = new Color(0.4f, 1f, 0.7f);
            GUI.Label(new Rect(x, y + 12, width, 30), "REVIVAL SPELL (1 CHANCE)", titleStyle);

            GUIStyle descStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            descStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(x, y + 44, width, 22), "Type this spell to revive your Capybara:", descStyle);

            GUIStyle spellBoxStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            spellBoxStyle.normal.textColor = Color.yellow;
            GUI.Box(new Rect(x + 20, y + 70, width - 40, 42), "\"" + currentSpell + "\"", spellBoxStyle);

            GUIStyle inputStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            GUI.SetNextControlName("ReviveInputField");
            typedInput = GUI.TextField(new Rect(x + 20, y + 122, width - 40, 36), typedInput, inputStyle);
            GUI.FocusControl("ReviveInputField");

            bool isMatch = string.Equals(typedInput.Trim(), currentSpell.Trim(), System.StringComparison.OrdinalIgnoreCase);

            GUIStyle reviveBtnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            if (isMatch)
            {
                reviveBtnStyle.normal.textColor = Color.green;
            }

            string btnLabel = isMatch ? "CAST SPELL" : "Type spell above to revive...";
            GUI.enabled = isMatch;
            if (GUI.Button(new Rect(x + 60, y + 172, width - 120, 44), btnLabel, reviveBtnStyle) ||
                (isMatch && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
            {
                RevivePlayer();
            }
            GUI.enabled = true;

            GUIStyle skipStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13
            };
            if (GUI.Button(new Rect(x + 180, y + 230, 180, 34), "Give Up / Restart", skipStyle))
            {
                RestartGame();
            }
        }
        else if (isGameOver && !isReviving)
        {
            // Standard Game Over Panel
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

    void DrawAbilitiesList()
    {
        if (player == null)
        {
            var pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj;
            else
            {
                var capy = GameObject.Find("Capybara");
                if (capy != null) player = capy;
            }
        }

        float panelWidth = 190f;
        float startX = Screen.width - panelWidth - 15f;
        float currentY = 15f;

        // Header box
        GUIStyle headerStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        headerStyle.normal.textColor = new Color(1f, 0.9f, 0.4f);
        GUI.Box(new Rect(startX, currentY, panelWidth, 20), "CONTROLS & ABILITIES", headerStyle);
        currentY += 22f;

        GUIStyle itemStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };

        // Shop items: Duck & Baby
        if (currentDuck == null)
        {
            itemStyle.normal.textColor = (score >= 6) ? Color.yellow : Color.gray;
            if (GUI.Button(new Rect(startX, currentY, panelWidth, 21), "[B] Adopt Duck (6)", itemStyle))
            {
                BuyDuck();
            }
            currentY += 22f;
        }

        var stackComp = player != null ? player.GetComponent<BabyCapybaraStack>() : null;
        if (stackComp == null || stackComp.babyCount < stackComp.maxBabies)
        {
            itemStyle.normal.textColor = (score >= 8) ? new Color(0.4f, 1f, 0.4f) : Color.gray;
            if (GUI.Button(new Rect(startX, currentY, panelWidth, 21), "[K] Baby Capy (8)", itemStyle))
            {
                BuyBaby();
            }
            currentY += 22f;
        }

        // Mechanics list: dynamically check player components
        if (player != null)
        {
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "CapybaraCar", "[V] Ok I Pull Up Car", "isInCar", "EnterCar", "ExitCar");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "CapybaraBurrow", "[R] Burrow Teleport", "isBurrowing", "StartBurrow");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "IndianCapybara", "[I] Indian Mode", "isIndianMode", "TryToggle");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "DiscoMode", "[Y] Disco Party", "isDisco", "ToggleDisco");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "VerityEdit", "[U] Capy Edit", "isPlayingEdit", "StartEdit");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "CardboardBox", "[H] Stealth Box", "isInBox", "EnterBox", "ExitBox");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "BananaPeelTrap", "[P] Banana Peel", null, "DropPeel");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "LaserEyes", "[L] Laser Eyes", "isFiring", "FireLasers");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "MatrixTime", "[T] Matrix Time", "isMatrixActive", "StartMatrix", "StopMatrix");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "GroundPound", "[X] Ground Pound", "isPounding", "DoGroundPound");
            DrawAbilityItem(ref currentY, startX, panelWidth, itemStyle, "GlitchTeleport", "[G] Glitch Teleport", "isGlitching", "DoGlitch");
        }
    }

    private void DrawAbilityItem(ref float y, float startX, float panelWidth, GUIStyle itemStyle,
        string typeName, string label, string stateField, string toggleMethod, string exitMethod = null)
    {
        Component comp = player != null ? player.GetComponent(typeName) : null;
        if (comp == null) return;

        bool isActive = false;
        float cd = 0f;
        var t = comp.GetType();

        if (!string.IsNullOrEmpty(stateField))
        {
            var field = t.GetField(stateField);
            if (field != null && field.GetValue(comp) is bool b) isActive = b;
        }

        var cdField = t.GetField("cooldownTimer");
        if (cdField != null && cdField.GetValue(comp) is float f) cd = f;

        string displayLabel = label;
        if (isActive)
        {
            itemStyle.normal.textColor = Color.yellow;
            displayLabel += " (ACTIVE)";
        }
        else if (cd > 0f)
        {
            itemStyle.normal.textColor = Color.gray;
            displayLabel += " (" + cd.ToString("F1") + "s)";
        }
        else
        {
            itemStyle.normal.textColor = Color.white;
        }

        if (GUI.Button(new Rect(startX, y, panelWidth, 21), displayLabel, itemStyle))
        {
            if (isActive && !string.IsNullOrEmpty(exitMethod))
            {
                var m = t.GetMethod(exitMethod);
                if (m != null) m.Invoke(comp, null);
            }
            else
            {
                var m = t.GetMethod(toggleMethod);
                if (m != null) m.Invoke(comp, null);
            }
        }
        y += 22f;
    }
}
