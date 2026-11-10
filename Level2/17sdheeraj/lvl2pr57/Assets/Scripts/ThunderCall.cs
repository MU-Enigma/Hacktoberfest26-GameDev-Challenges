using UnityEngine;

public class ThunderCall : MonoBehaviour
{
    [HideInInspector] public bool isThundering = false;
    public float cooldownTimer = 0f;
    private LineRenderer bolt;
    private float boltTimer = 0f;

    void Awake()
    {
        EnsureBolt();
    }

    void EnsureBolt()
    {
        if (bolt != null) return;
        var obj = new GameObject("ThunderCallVisual");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        bolt = obj.AddComponent<LineRenderer>();
        bolt.positionCount = 4;
        bolt.startWidth = 0.2f;
        bolt.endWidth = 0.1f;
        bolt.material = new Material(Shader.Find("Sprites/Default"));
        bolt.startColor = Color.yellow;
        bolt.endColor = Color.white;
        bolt.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.T) && cooldownTimer <= 0f && !isThundering)
        {
            CallThunder();
        }

        if (isThundering)
        {
            boltTimer -= Time.deltaTime;
            if (boltTimer <= 0f)
            {
                isThundering = false;
                if (bolt != null) bolt.enabled = false;
            }
        }
    }

    public void CallThunder()
    {
        if (cooldownTimer > 0f) return;
        EnsureBolt();
        Vector3 strikePos = transform.position + Vector3.right * 3f;
        if (bolt != null)
        {
            bolt.SetPosition(0, strikePos + Vector3.up * 8f);
            bolt.SetPosition(1, strikePos + Vector3.up * 5f + Vector3.left * 0.5f);
            bolt.SetPosition(2, strikePos + Vector3.up * 2.5f + Vector3.right * 0.5f);
            bolt.SetPosition(3, strikePos);
            bolt.enabled = true;
        }
        isThundering = true;
        boltTimer = 0.25f;

        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(strikePos, croc.transform.position) < 3.5f)
        {
            croc.transform.position += Vector3.right * 4f;
        }

        cooldownTimer = 6f;
    }
}