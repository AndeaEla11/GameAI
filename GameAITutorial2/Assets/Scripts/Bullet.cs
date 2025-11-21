using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    void Start()
    {
        Destroy(gameObject, 3f); 
    }

    void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
