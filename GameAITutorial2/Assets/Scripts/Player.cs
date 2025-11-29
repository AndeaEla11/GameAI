using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{

    public float speed;
    

    [HideInInspector]
    public int health = 100;
    public Text PlayerHPText;
   
    private new Rigidbody rigidbody; 

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    public int score = 0;
    public Text scoreText;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        UpdateHPUI();
        if (scoreText != null)
            scoreText.text = "Score: 12/ " + score;
    }

    void FixedUpdate()
    {

        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");


        Vector3 direction = new Vector3(x, 0f, z);

        if (direction.magnitude > 1f)
            direction = direction.normalized;


        Vector3 currentPosition = rigidbody.position;
        Vector3 newPosition = rigidbody.position;

        newPosition = currentPosition + direction * speed * Time.fixedDeltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }

        rigidbody.MovePosition(newPosition);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

    }
    void Shoot()
    {


        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            bulletRb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null)
            scoreText.text = "Score: 12/" + score;
    }

    public void TakeDamege(int damage)
    {
        health = Mathf.Max(0, health - damage);
        UpdateHPUI();
        if (health == 0) 
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
