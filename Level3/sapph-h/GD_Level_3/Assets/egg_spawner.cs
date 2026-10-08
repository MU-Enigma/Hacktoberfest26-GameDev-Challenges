using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    public GameObject eggPrefab;
    public Transform spawnPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(eggPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
