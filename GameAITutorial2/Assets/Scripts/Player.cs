using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
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
        {
            direction = direction.normalized;
        }

        controller.Move(direction * speed * Time.deltaTime);
    }

}
