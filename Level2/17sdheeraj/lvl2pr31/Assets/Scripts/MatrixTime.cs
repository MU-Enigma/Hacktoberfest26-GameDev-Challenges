using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixTime : MonoBehaviour
{
    public float slowScale = 0.25f;
    public float duration = 4.0f;
    public float cooldown = 5.5f;

    [HideInInspector]
    public bool isMatrixActive = false;

    private float activeTimer = 0f;
    private float cooldownTimer = 0f;
    private float trailTimer = 0f;
    private SpriteRenderer sr;
    private PlayerMovement pm;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        pm = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (pm == null) pm = GetComponent<PlayerMovement>();
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
        return CheckMode("VerityEdit", "isPlayingEdit") ||
               CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing") ||
               CheckMode("CapybaraBurrow", "isBurrowing");
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isMatrixActive)
            {
                StopMatrix();
            }
            else if (cooldownTimer <= 0f && !IsOtherModeActive())
            {
                StartMatrix();
            }
        }

        if (isMatrixActive)
        {
            activeTimer -= Time.unscaledDeltaTime;
            if (activeTimer <= 0f)
            {
                StopMatrix();
                return;
            }

            // Spawn green/cyan ghost trails
            trailTimer += Time.unscaledDeltaTime;
            if (trailTimer >= 0.08f)
            {
                trailTimer = 0f;
                SpawnGhostTrail();
            }
        }
    }

    public void StartMatrix()
    {
        isMatrixActive = true;
        activeTimer = duration;
        cooldownTimer = cooldown + duration;
        Time.timeScale = slowScale;
    }

    public void StopMatrix()
    {
        isMatrixActive = false;
        Time.timeScale = 1f;
    }

    private void SpawnGhostTrail()
    {
        if (sr == null || sr.sprite == null) return;

        GameObject ghost = new GameObject("MatrixGhost");
        ghost.transform.position = transform.position;
        ghost.transform.localScale = transform.localScale;

        var gSr = ghost.AddComponent<SpriteRenderer>();
        gSr.sprite = sr.sprite;
        gSr.flipX = sr.flipX;
        gSr.sortingOrder = sr.sortingOrder - 1;
        gSr.color = new Color(0.2f, 1f, 0.4f, 0.65f);

        StartCoroutine(FadeGhost(ghost, gSr));
    }

    private IEnumerator FadeGhost(GameObject obj, SpriteRenderer gSr)
    {
        float timer = 0f;
        float fadeTime = 0.45f;
        Color initial = gSr.color;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(initial.a, 0f, timer / fadeTime);
            gSr.color = new Color(initial.r, initial.g, initial.b, alpha);
            yield return null;
        }

        Destroy(obj);
    }

    void OnDisable()
    {
        if (isMatrixActive) Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        if (isMatrixActive) Time.timeScale = 1f;
    }
}

