using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    [HideInInspector] public bool isBlowing = false;
    public float cooldownTimer = 0f;
    private float windTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && cooldownTimer <= 0f && !isBlowing)
        {
            StartWind();
        }

        if (isBlowing)
        {
            windTimer -= Time.deltaTime;
            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 6f)
            {
                croc.transform.position += Vector3.right * 5f * Time.deltaTime;
            }

            if (windTimer <= 0f)
            {
                isBlowing = false;
            }
        }
    }

    public void StartWind()
    {
        if (cooldownTimer > 0f) return;
        isBlowing = true;
        windTimer = 3f;
        cooldownTimer = 6f;
    }
}