using UnityEngine;

public class PortalDoors : MonoBehaviour
{
    [HideInInspector] public bool hasPortals = false;
    public float cooldownTimer = 0f;
    private GameObject portalA;
    private GameObject portalB;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.P) && cooldownTimer <= 0f)
        {
            PlacePortal();
        }

        if (portalA != null && portalB != null)
        {
            if (Vector3.Distance(transform.position, portalA.transform.position) < 0.8f && cooldownTimer <= 0f)
            {
                transform.position = portalB.transform.position + Vector3.right * 1f;
                cooldownTimer = 2f;
            }
            else if (Vector3.Distance(transform.position, portalB.transform.position) < 0.8f && cooldownTimer <= 0f)
            {
                transform.position = portalA.transform.position + Vector3.right * 1f;
                cooldownTimer = 2f;
            }
        }
    }

    public void PlacePortal()
    {
        if (portalA == null)
        {
            portalA = CreatePortalObj("PortalA", Color.cyan);
            portalA.transform.position = transform.position;
        }
        else if (portalB == null)
        {
            portalB = CreatePortalObj("PortalB", new Color(1f, 0.5f, 0.1f));
            portalB.transform.position = transform.position;
            hasPortals = true;
            cooldownTimer = 8f;
        }
        else
        {
            Destroy(portalA);
            Destroy(portalB);
            portalA = null;
            portalB = null;
            hasPortals = false;
        }
    }

    GameObject CreatePortalObj(string pName, Color col)
    {
        var obj = new GameObject(pName);
        var sr = obj.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(res * 0.5f, res * 0.5f);
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c);
                if (d < res * 0.45f && d > res * 0.25f)
                    tex.SetPixel(x, y, col);
                else tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 3;
        return obj;
    }
}