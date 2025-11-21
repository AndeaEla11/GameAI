using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{

    public float speed;
    

    [HideInInspector]
    public int health = 100;
    public Text PlayerHPText;


    private CharacterController controller; 

    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");


        Vector3 direction = new Vector3(x, 0f, z);

        if (direction.magnitude > 1f)
            direction = direction.normalized;


        controller.Move(direction * speed * Time.deltaTime);


    }

    public void TakeDamege(int damage)
    {
        health -= damage;

        UpdateHPUI(); 

        if (health <= 0)
            PlayerDied();


    }

    void UpdateHPUI()
    {
        PlayerHPText.text = "HP: " + health; 
    }

    void PlayerDied()
    {
        gameObject.SetActive(false);
    }

}
