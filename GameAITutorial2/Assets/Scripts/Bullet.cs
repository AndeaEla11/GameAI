using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    bool hasHit = false;

    void Start()
    {
        Destroy(gameObject, 3f); 
    }

    void OnCollisionEnter(Collision other)
    {
        if (hasHit) return;
        hasHit = true;

        //player
        var player = other.collider.GetComponentInParent<Player>();
        if (player != null)
        {
            player.TakeDamege(damage);
            Destroy(gameObject);
            return;
        }

        //npcA and npcB
        var hp = other.collider.GetComponentInParent<npc>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);


    }
}
