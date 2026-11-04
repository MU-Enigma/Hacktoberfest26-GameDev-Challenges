using UnityEngine;

public class CapybaraCar : MonoBehaviour
{
    public Sprite carSprite;
    public float driveSpeed = 8.5f;

    [HideInInspector]
    public bool isInCar = false;
    private GameObject carObj;
    private SpriteRenderer carSr;
    private PlayerMovement pm;
    private SpriteRenderer capySr;
    private float normalSpeed = 3.5f;
    private float honkTimer = 0f;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        capySr = GetComponent<SpriteRenderer>();
        if (pm != null)
        {
            normalSpeed = pm.moveSpeed;
        }
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            if (isInCar) ExitCar();
            return;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isInCar)
            {
                ExitCar();
            }
            else
            {
                EnterCar();
            }
        }

        if (isInCar)
        {
            if (pm != null)
            {
                pm.moveSpeed = driveSpeed;
            }

            if (carObj != null)
            {
                bool flip = capySr != null && capySr.flipX;
                carObj.transform.position = transform.position + new Vector3(flip ? 0.05f : -0.05f, -0.2f, 0f);
                if (carSr != null)
                {
                    carSr.flipX = flip;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Honk();
            }

            if (honkTimer > 0f)
            {
                honkTimer -= Time.deltaTime;
            }
        }
    }

    public void EnterCar()
    {
        isInCar = true;
        if (pm != null)
        {
            pm.moveSpeed = driveSpeed;
        }

        if (carObj == null)
        {
            carObj = new GameObject("CapyConvertible");
            carObj.transform.SetParent(transform);
            carObj.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
            carSr = carObj.AddComponent<SpriteRenderer>();
            carSr.sprite = carSprite;
            carSr.sortingOrder = (capySr != null ? capySr.sortingOrder : 0) + 1;
        }
        else
        {
            carObj.SetActive(true);
        }
    }

    public void ExitCar()
    {
        isInCar = false;
        if (pm != null)
        {
            pm.moveSpeed = normalSpeed;
        }

        if (carObj != null)
        {
            carObj.SetActive(false);
        }
    }

    public void Honk()
    {
        honkTimer = 0.8f;

        var croc = GameObject.Find("Crocodile");
        if (croc != null)
        {
            float dist = Vector2.Distance(transform.position, croc.transform.position);
            if (dist < 4.5f)
            {
                var ai = croc.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.Die();
                }
            }
        }
    }

    void OnGUI()
    {
        if (isInCar)
        {
            GUIStyle carStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            carStyle.normal.textColor = Color.red;
            GUI.Box(new Rect(20, 205, 185, 34), "[V] Car: ON | [Space] Honk", carStyle);

            if (honkTimer > 0f)
            {
                float bx = Screen.width / 2f - 75f;
                float by = Screen.height / 2f - 90f;
                GUIStyle honkStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 18,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                honkStyle.normal.textColor = Color.yellow;
                GUI.Box(new Rect(bx, by, 150, 40), "HONK HONK!", honkStyle);
            }
        }
    }
}
