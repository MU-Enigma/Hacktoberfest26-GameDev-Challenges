using UnityEngine;

public class FruitMagnet : MonoBehaviour
{
    public float pullSpeed = 12f;

    [HideInInspector]
    public bool isPulling = false;

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;

        isPulling = Input.GetKey(KeyCode.M);

        if (isPulling)
        {
            var foods = FindObjectsOfType<FoodItem>();
            foreach (var f in foods)
            {
                if (f != null)
                {
                    f.PullTowards(transform.position, pullSpeed);
                }
            }

            var golden = FindObjectsOfType<GoldenYuzu>();
            foreach (var g in golden)
            {
                if (g != null)
                {
                    g.transform.position = Vector2.MoveTowards(g.transform.position, transform.position, pullSpeed * Time.deltaTime);
                }
            }
        }
    }

    void OnGUI()
    {
        if (isPulling)
        {
            GUIStyle magStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            magStyle.normal.textColor = Color.cyan;
            GUI.Box(new Rect(20, 160, 160, 36), "MAGNET ACTIVE", magStyle);
        }
    }
}
