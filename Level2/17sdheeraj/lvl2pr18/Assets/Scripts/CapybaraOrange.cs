using UnityEngine;

public class CapybaraOrange : MonoBehaviour
{
    public Sprite orangeSprite;
    public float balanceDuration = 8f;

    [HideInInspector]
    public bool isBalancing = false;
    [HideInInspector]
    public float balanceTimer = 0f;

    private GameObject orangeObj;
    private SpriteRenderer orangeSr;
    private PlayerMovement pm;
    private SpriteRenderer capySr;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        capySr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            if (isBalancing) DropOrange();
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (isBalancing)
            {
                DropOrange();
            }
            else
            {
                StartBalancing();
            }
        }

        if (isBalancing)
        {
            // If player sprints, orange wobbles off!
            var t = typeof(PlayerMovement);
            var sprintField = t.GetField("isSprinting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool sprinting = sprintField != null && (bool)sprintField.GetValue(pm);

            if (sprinting)
            {
                DropOrange();
                return;
            }

            balanceTimer += Time.deltaTime;

            if (orangeObj != null)
            {
                float wobble = Mathf.Sin(Time.time * 6f) * 0.04f;
                float dir = (capySr != null && capySr.flipX) ? -0.1f : 0.1f;
                orangeObj.transform.position = transform.position + new Vector3(dir + wobble, 0.85f, 0f);
            }

            if (balanceTimer >= balanceDuration)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.AddScore(5);
                }
                balanceTimer = 0f;
            }
        }
    }

    public void StartBalancing()
    {
        isBalancing = true;
        balanceTimer = 0f;

        if (orangeObj == null)
        {
            orangeObj = new GameObject("OrangeHat");
            orangeObj.transform.SetParent(transform);
            orangeSr = orangeObj.AddComponent<SpriteRenderer>();
            orangeSr.sprite = orangeSprite;
            orangeSr.sortingOrder = 10;
        }
        else
        {
            orangeObj.SetActive(true);
        }
    }

    public void DropOrange()
    {
        isBalancing = false;
        balanceTimer = 0f;
        if (orangeObj != null)
        {
            orangeObj.SetActive(false);
        }
    }
}
