using UnityEngine;

public class RespawnManager : MonoBehaviour
{

    public Transform respawnPoint;
    public GameObject player; 

    void Start()
    {
        respawnPoint = new GameObject("RespawnPoint").transform;
        respawnPoint.position = transform.position; 
    }

    
    public void RespawnPlayer()
    {
        player.transform.position = respawnPoint.position;
    }
}
