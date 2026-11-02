
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerInvisibility : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private PlayerColorChanger colorChanger;

    private bool isInvisible = false;
    public float invisibilityDuration = 10f;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        colorChanger = GetComponent<PlayerColorChanger>();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (isInvisible)
                return;

            if (colorChanger != null && !colorChanger.IsDefaultColor)
            {
                Debug.Log("You must be white to become invisible!");
                return;
            }

            StartCoroutine(InvisibilityTimer());
        }
    }

    IEnumerator InvisibilityTimer()
    {
        isInvisible = true;
        SetVisibility(false);

        Debug.Log("Player is invisible for 10 seconds!");

        yield return new WaitForSeconds(invisibilityDuration);

        SetVisibility(true);
        isInvisible = false;

        Debug.Log("Invisibility expired! Player is visible again.");
    }

    void SetVisibility(bool visible)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.enabled = visible;
        }
    }
}
