using UnityEngine;

public class GiantCapy : MonoBehaviour
{
    [HideInInspector] public bool isGiant = false;
    public float cooldownTimer = 0f;
    private float giantTimer = 0f;
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && cooldownTimer <= 0f && !isGiant)
        {
            ToggleGiant();
        }

        if (isGiant)
        {
            giantTimer -= Time.deltaTime;
            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 3f)
            {
                Vector3 flee = (croc.transform.position - transform.position).normalized;
                croc.transform.position += flee * 6f * Time.deltaTime;
            }

            if (giantTimer <= 0f)
            {
                isGiant = false;
                transform.localScale = originalScale;
            }
        }
    }

    public void ToggleGiant()
    {
        if (cooldownTimer > 0f) return;
        isGiant = true;
        giantTimer = 5f;
        transform.localScale = originalScale * 2.2f;
        cooldownTimer = 10f;
    }
}