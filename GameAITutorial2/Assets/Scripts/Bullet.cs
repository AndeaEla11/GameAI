using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10; 

    void Start()
    {
        Destroy(gameObject, 3f); 
    }

    void OnCollisionEnter(Collision other)
    {

        var hpA = other.collider.GetComponentInParent<npcA>();
        if (hpA != null )
        {
            hpA.TakeDamage( damage );
            Destroy(gameObject);
            return;
        }


        Destroy(gameObject);
    }
}
