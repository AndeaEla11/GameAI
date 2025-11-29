using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1;

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.AddScore(value);
            Destroy(gameObject);
        }
    }
}
