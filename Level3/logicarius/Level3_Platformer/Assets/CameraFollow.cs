using UnityEngine;

// Put this on the Main Camera. Follows the chicken smoothly, looks ahead in the direction you run,
// and stays inside the arena (ArenaBuilder sets the bounds).
public class CameraFollow : MonoBehaviour {
    public Transform target;                         // leave empty: it finds the chicken by itself
    public Vector2 offset = new Vector2(3f, 2f);     // look this far ahead (x) and above (y)
    public float smoothTime = 0.18f;                 // lower = snappier, higher = floatier

    Rect bounds;
    bool useBounds;
    Vector3 velocity;
    float lookDir = 1f;
    Camera cam;

    public void SetBounds(Rect r) { bounds = r; useBounds = true; }

    void Awake() { cam = GetComponent<Camera>(); }

    void Start() {
        if (target == null) {
            PlayerMovement p = FindFirstObjectByType<PlayerMovement>();
            if (p != null) target = p.transform;
        }
        if (target != null) transform.position = Goal();
    }

    Vector3 Goal() {
        Vector3 g = new Vector3(target.position.x + offset.x * lookDir, target.position.y + offset.y, transform.position.z);
        if (useBounds && cam != null && cam.orthographic) {
            float hh = cam.orthographicSize, hw = hh * cam.aspect;
            g.x = (bounds.width <= 2f * hw) ? bounds.center.x : Mathf.Clamp(g.x, bounds.xMin + hw, bounds.xMax - hw);
            g.y = (bounds.height <= 2f * hh) ? bounds.center.y : Mathf.Clamp(g.y, bounds.yMin + hh, bounds.yMax - hh);
        }
        return g;
    }

    void LateUpdate() {
        if (target == null || Time.deltaTime <= 0f) return;
        float x = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(x) > 0.1f) lookDir = Mathf.Sign(x);
        transform.position = Vector3.SmoothDamp(transform.position, Goal(), ref velocity, smoothTime);
    }
}