using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    private Camera _camera;
    private float elapsedTime = 0f;
    
    private float score = 0f;
    public float scoreMultiplier = 10f;
    
    
    public float thrustForce = 1f;
    public float maxSpeed = 5f;
    public Rigidbody2D rb;

    public GameObject boosterFlame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        elapsedTime += Time.deltaTime;
        Debug.Log("Elapsed time: " + elapsedTime);
        
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        Debug.Log("Score: " + score);
        
        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate mouse direction
            var mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlame.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlame.SetActive(false);
        }
    }

    void OnCollisionEnter2D()
    {
        Destroy(gameObject);
    }
}
