using UnityEngine;

public class SonicBurp : MonoBehaviour
{
    [HideInInspector] public bool isBurping = false;
    public float cooldownTimer = 0f;
    private LineRenderer wave;
    private float burpTimer = 0f;

    void Awake()
    {
        wave = gameObject.AddComponent<LineRenderer>();
        wave.positionCount = 10;
        wave.startWidth = 0.2f;
        wave.endWidth = 0.8f;
        wave.material = new Material(Shader.Find("Sprites/Default"));
        wave.startColor = new Color(0.3f, 1f, 0.3f, 0.9f);
        wave.endColor = new Color(0.8f, 1f, 0.2f, 0.1f);
        wave.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.B) && cooldownTimer <= 0f && !isBurping)
        {
            DoBurp();
        }

        if (isBurping)
        {
            burpTimer -= Time.deltaTime;
            for (int i = 0; i < 10; i++)
            {
                float t = i / 9f;
                wave.SetPosition(i, transform.position + new Vector3(t * 4f, Mathf.Sin(t * 10f + Time.time * 20f) * 0.5f, 0));
            }

            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 4.5f)
            {
                croc.transform.position += Vector3.right * 5f * Time.deltaTime;
            }

            if (burpTimer <= 0f)
            {
                isBurping = false;
                wave.enabled = false;
            }
        }
    }

    public void DoBurp()
    {
        if (cooldownTimer > 0f) return;
        isBurping = true;
        burpTimer = 0.6f;
        wave.enabled = true;
        cooldownTimer = 5f;
    }
}