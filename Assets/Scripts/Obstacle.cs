using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.0f;
    
    public float minSpeed = 50f;
    public float maxSpeed = 150f;
    
    public float maxSpinSpeed = 10f;
    
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);
        var randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        rb =  GetComponent<Rigidbody2D>();
        var randomDirection = Random.insideUnitCircle;
        
        rb.AddForce(randomDirection * randomSpeed);
        
        var randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
