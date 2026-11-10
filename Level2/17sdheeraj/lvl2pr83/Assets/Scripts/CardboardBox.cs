using UnityEngine;

public class CardboardBox : MonoBehaviour
{
    public Sprite boxSprite;
    public Sprite questionSprite;

    [HideInInspector]
    public bool isInBox = false;

    private GameObject boxObj;
    private SpriteRenderer boxSr;
    private SpriteRenderer capySr;
    private PlayerMovement pm;
    private float normalSpeed = 5f;

    private GameObject crocodile;
    private GameObject questionMarkObj;
    private Vector3 crocWanderDir;
    private float crocWanderTimer = 0f;

    void Awake()
    {
        LoadAssets();
    }

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        capySr = GetComponent<SpriteRenderer>();
        if (pm != null)
        {
            normalSpeed = pm.moveSpeed;
        }

        LoadAssets();
    }

    private void LoadAssets()
    {
        if (boxSprite == null)
        {
            boxSprite = Resources.Load<Sprite>("cardboard_box");
        }
        if (questionSprite == null)
        {
            questionSprite = Resources.Load<Sprite>("alert_question");
        }
    }

    private bool CheckMode(string typeName, string fieldName)
    {
        Component comp = GetComponent(typeName);
        if (comp != null && comp != this)
        {
            var field = comp.GetType().GetField(fieldName);
            if (field != null)
            {
                object val = field.GetValue(comp);
                if (val is bool b && b) return true;
            }
        }
        return false;
    }

    public bool IsOtherModeActive()
    {
        return CheckMode("CapybaraCar", "isInCar") ||
               CheckMode("IndianCapybara", "isIndianMode") ||
               CheckMode("DiscoMode", "isDisco") ||
               CheckMode("VerityEdit", "isPlayingEdit") ||
               CheckMode("CapybaraBurrow", "isBurrowing") ||
               CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing") ||
               CheckMode("MatrixTime", "isMatrixActive") ||
               CheckMode("GroundPound", "isPounding");
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            if (isInBox) ExitBox();
            return;
        }

        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.B))
        {
            if (isInBox)
            {
                ExitBox();
            }
            else if (!IsOtherModeActive())
            {
                EnterBox();
            }
        }

        if (isInBox)
        {
            if (pm != null)
            {
                pm.moveSpeed = normalSpeed * 0.7f;
            }

            // Keep box attached and sync flip
            if (boxObj != null)
            {
                boxObj.transform.position = transform.position + new Vector3(0f, 0.15f, 0f);
                if (boxSr != null && capySr != null)
                {
                    boxSr.flipX = capySr.flipX;
                }
            }

            // Distract crocodile
            UpdateCrocodileDistraction();
        }
    }

    public void EnterBox()
    {
        isInBox = true;
        LoadAssets();

        if (pm != null)
        {
            pm.moveSpeed = normalSpeed * 0.7f;
        }

        if (capySr != null)
        {
            capySr.enabled = false;
        }

        if (boxObj == null)
        {
            boxObj = new GameObject("BoxVisual");
            boxObj.transform.SetParent(transform);
            boxObj.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            boxObj.transform.localScale = new Vector3(2.2f, 2.2f, 1f);
            boxSr = boxObj.AddComponent<SpriteRenderer>();
            boxSr.sprite = boxSprite;
            boxSr.sortingOrder = 5;
        }
        else
        {
            boxObj.SetActive(true);
        }

        // Show question mark over crocodile
        crocodile = GameObject.Find("Crocodile");
        if (crocodile != null && questionSprite != null)
        {
            if (questionMarkObj == null)
            {
                questionMarkObj = new GameObject("AlertQuestion");
                questionMarkObj.transform.SetParent(crocodile.transform);
                questionMarkObj.transform.localPosition = new Vector3(0f, 1.3f, 0f);
                questionMarkObj.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                var qSr = questionMarkObj.AddComponent<SpriteRenderer>();
                qSr.sprite = questionSprite;
                qSr.sortingOrder = 10;
            }
            else
            {
                questionMarkObj.SetActive(true);
            }

            var ai = crocodile.GetComponent<EnemyAI>();
            if (ai != null) ai.enabled = false;
            crocWanderDir = Random.insideUnitCircle.normalized;
        }
    }

    public void ExitBox()
    {
        isInBox = false;

        if (pm != null)
        {
            pm.moveSpeed = normalSpeed;
        }

        if (capySr != null)
        {
            capySr.enabled = true;
        }

        if (boxObj != null)
        {
            boxObj.SetActive(false);
        }

        if (questionMarkObj != null)
        {
            questionMarkObj.SetActive(false);
        }

        if (crocodile != null)
        {
            var ai = crocodile.GetComponent<EnemyAI>();
            if (ai != null) ai.enabled = true;
        }
    }

    private void UpdateCrocodileDistraction()
    {
        if (crocodile == null) crocodile = GameObject.Find("Crocodile");
        if (crocodile == null) return;

        crocWanderTimer += Time.deltaTime;
        if (crocWanderTimer >= 1.5f)
        {
            crocWanderTimer = 0f;
            crocWanderDir = Random.insideUnitCircle.normalized;
        }

        // Move crocodile slowly in random wander direction away from player
        crocodile.transform.position += crocWanderDir * (1.5f * Time.deltaTime);

        // Keep question mark floating with small bob
        if (questionMarkObj != null)
        {
            float bob = Mathf.Sin(Time.time * 5f) * 0.1f;
            questionMarkObj.transform.localPosition = new Vector3(0f, 1.3f + bob, 0f);
        }
    }
}

