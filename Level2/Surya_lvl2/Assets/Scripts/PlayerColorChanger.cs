
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerColorChanger : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;

    private readonly Color[] colors =
    {
        Color.red,
        Color.yellow,
        Color.blue,
        Color.white
    };

    private int currentColorIndex = 0;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            Debug.Log("Z pressed! Changing player colour.");
            ChangeColor();
        }
    }

    void ChangeColor()
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = colors[currentColorIndex];
        }

        Debug.Log("Player colour changed to: " + colors[currentColorIndex]);

        currentColorIndex = (currentColorIndex + 1) % colors.Length;
    }
}
