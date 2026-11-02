
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        DuckController duck =
            other.GetComponent<DuckController>();

        if (duck != null)
            duck.ResetDuck();
    }
}
