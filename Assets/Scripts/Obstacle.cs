using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.0f;

    public float minSpeed = 50f;
    public float maxSpeed = 150f;

    public float maxSpinSpeed = 10f;
    public float maxVelocity = 20f;


    public Rigidbody2D rb;
    public GameObject bounceEffectPrefab;

    private void Start()
    {
        ApplyRandomSize();
        ApplyRandomSpeedAndForce();
        ApplyRandomTorque();
    }

    private void ApplyRandomSize()
    {
        // 1. Random kích thước obstacle
        var randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);
    }

    private void ApplyRandomSpeedAndForce()
    {
        // 2. Tính tốc độ ban đầu (size lớn → chậm hơn)
        var randomSpeed = Random.Range(minSpeed, maxSpeed) / transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();
        // 4. Random hướng di chuyển
        var randomDirection = Random.insideUnitCircle;
        // 5. Đẩy obstacle theo hướng ngẫu nhiên
        rb.AddForce(randomDirection * randomSpeed);
    }

    private void ApplyRandomTorque()
    {
        // 6. Random lực xoay
        var randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    private void FixedUpdate()
    {
        ApplyVelocityClamp();
    }

    private void ApplyVelocityClamp()
    {
        // Giới hạn độ lớn vận tốc của Rigidbody, đảm bảo tàu không vượt quá maxVelocity
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxVelocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DestroyBounceEffect(collision);
    }

    private void DestroyBounceEffect(Collision2D collision)
    {
        var contactPoint = collision.GetContact(0).point;
        var bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);
        // Destroy the effect after 1 second
        Destroy(bounceEffect, 1f);
    }
}
