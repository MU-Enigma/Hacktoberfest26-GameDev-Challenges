using UnityEngine;

public class CelebrationManager : MonoBehaviour
{
    public static CelebrationManager instance;

    [HideInInspector]
    public bool isCelebrating = false;
    private float celebrationTimer = 0f;
    private bool hasTriggered = false;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.score >= 100 && !hasTriggered)
        {
            TriggerCelebration();
        }

        if (isCelebrating)
        {
            celebrationTimer += Time.deltaTime;
        }
    }

    public void TriggerCelebration()
    {
        hasTriggered = true;
        isCelebrating = true;
        celebrationTimer = 0f;
    }

    public void ResetCelebration()
    {
        isCelebrating = false;
        hasTriggered = false;
        celebrationTimer = 0f;
    }

    void OnGUI()
    {
        if (isCelebrating)
        {
            float width = 400f;
            float height = 65f;
            float x = (Screen.width - width) / 2f;
            float y = 80f;

            GUIStyle bannerStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            bannerStyle.normal.textColor = Color.yellow;

            GUI.Box(new Rect(x, y, width, height), "100 POINTS REACHED!\nYOU WIN!", bannerStyle);
        }
    }
}
