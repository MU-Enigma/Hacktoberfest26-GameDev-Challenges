
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInvisibility : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private PlayerColorChanger colorChanger;

    private bool isInvisible = false;

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
            // Only allow invisibility when the player is white.
            if (colorChanger != null && !colorChanger.IsDefaultColor)
            {
                Debug.Log("You must be white to become invisible!");
                return;
            }

            isInvisible = !isInvisible;

            foreach (SpriteRenderer sr in spriteRenderers)
            {
                sr.enabled = !isInvisible;
            }

            Debug.Log(isInvisible
                ? "Player is invisible!"
                : "Player is visible!");
        }
    }
}
