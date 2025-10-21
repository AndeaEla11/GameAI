using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; 

    private Rigidbody rb; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical"); 

        Vector3 move = new Vector3 (moveX, 0, moveZ);

        rb.MovePosition(transform.position +  move * Time.fixedDeltaTime * moveSpeed);
        
    }
}
