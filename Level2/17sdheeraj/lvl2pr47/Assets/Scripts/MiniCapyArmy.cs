using UnityEngine;

public class MiniCapyArmy : MonoBehaviour
{
    [HideInInspector] public bool hasArmy = false;
    public float cooldownTimer = 0f;
    private GameObject[] squad = new GameObject[3];
    private float armyTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.A) && cooldownTimer <= 0f && !hasArmy)
        {
            CallArmy();
        }

        if (hasArmy)
        {
            armyTimer -= Time.deltaTime;
            for (int i = 0; i < 3; i++)
            {
                if (squad[i] != null)
                {
                    Vector3 target = transform.position - Vector3.right * (0.8f * (i + 1));
                    squad[i].transform.position = Vector3.Lerp(squad[i].transform.position, target, 5f * Time.deltaTime);
                }
            }

            if (armyTimer <= 0f)
            {
                hasArmy = false;
                for (int i = 0; i < 3; i++) if (squad[i] != null) Destroy(squad[i]);
            }
        }
    }

    public void CallArmy()
    {
        if (cooldownTimer > 0f) return;
        hasArmy = true;
        armyTimer = 10f;
        for (int i = 0; i < 3; i++)
        {
            squad[i] = new GameObject("MiniCapy_" + i);
            squad[i].transform.position = transform.position - Vector3.right * (0.8f * (i + 1));
            squad[i].transform.localScale = Vector3.one * 0.4f;
            var sr = squad[i].AddComponent<SpriteRenderer>();
            var pSr = GetComponent<SpriteRenderer>();
            if (pSr != null) sr.sprite = pSr.sprite;
            sr.sortingOrder = 3;
        }
        cooldownTimer = 12f;
    }
}