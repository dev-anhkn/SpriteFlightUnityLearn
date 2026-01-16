using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerCtrl : MonoBehaviour
{
    // ===== Camera =====
    private Camera _camera;

    // ===== Score =====
    private float _elapsedTime;
    private float _score;
    private float _highScore;
    public float scoreMultiplier = 10f;

    // ===== UI =====
    public UIDocument uiDocument;
    private Label _scoreText;
    private Label _highScoreText;
    private Button _restartButton;

    // ===== Movement =====
    public float thrustForce = 2f;
    public float maxSpeed = 10f;
    public Rigidbody2D rb;

    // ===== Booster flame =====
    public GameObject boosterFlame;
    public float pulseRange = 0.1f;
    public float pulseSpeed = 10f;
    private Vector3 _flameMaxScale;

    // ===== Effects & misc =====
    public GameObject explosionEffect;
    public GameObject borderParent;

    private const string HighScoreKey = "HIGH_SCORE";

    // ========================= START =========================
    private void Start()
    {
        InitializeCamera();
        InitializeRigidbody();
        InitializeUI();
        InitializeBoosterFlame();
    }

    private void Update()
    {
        UpdateScore();
        HandlePlayerMovement();
    }

    private void InitializeCamera()
    {
        _camera = Camera.main;
    }

    private void InitializeRigidbody()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Khởi tạo UI, score, button
    private void InitializeUI()
    {
        _scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        _highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        _restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");

        _highScore = PlayerPrefs.GetFloat(HighScoreKey, 0);

        _highScoreText.style.display = DisplayStyle.None;
        _restartButton.style.display = DisplayStyle.None;

        _restartButton.clicked += ReloadScene;
    }

    // Lưu scale gốc của booster flame (scale MAX)
    private void InitializeBoosterFlame()
    {
        _flameMaxScale = boosterFlame.transform.localScale;
    }

    private void HandlePlayerMovement()
    {
        if (IsThrustPressed())
        {
            RotateTowardMouse();
            ApplyThrustForce();
            LimitMaxSpeed();
            UpdateBoosterFlame();
        }
        else
        {
            DisableBoosterFlame();
        }
    }

    // Kiểm tra có đang giữ chuột trái không
    private static bool IsThrustPressed()
    {
        return Mouse.current.leftButton.isPressed;
    }

    // Xoay tàu về hướng chuột
    private void RotateTowardMouse()
    {
        var mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.value);
        Vector2 direction = (mousePos - transform.position).normalized;
        transform.up = direction;
    }

    // Áp lực đẩy cho tàu
    private void ApplyThrustForce()
    {
        rb.AddForce(transform.up * thrustForce);
    }

    // Giới hạn vận tốc tối đa
    private void LimitMaxSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // ========================= BOOSTER FLAME =========================

    // Bật flame và tạo hiệu ứng phập phùng
    private void UpdateBoosterFlame()
    {
        boosterFlame.SetActive(true);

        var pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        var scaleX = Mathf.Lerp(_flameMaxScale.x - pulseRange, _flameMaxScale.x, pulse);
        var scaleY = Mathf.Lerp(_flameMaxScale.y - pulseRange, _flameMaxScale.y, pulse);

        boosterFlame.transform.localScale = new Vector3(scaleX, scaleY, _flameMaxScale.z);
    }

    // Tắt booster flame
    private void DisableBoosterFlame()
    {
        boosterFlame.SetActive(false);
    }

    // Cập nhật điểm theo thời gian sống
    private void UpdateScore()
    {
        _elapsedTime += Time.deltaTime;
        _score = Mathf.FloorToInt(_elapsedTime * scoreMultiplier);
        _scoreText.text = "Score: " + _score;
    }

    private void OnCollisionEnter2D()
    {
        StopGame();
        UpdateHighScore();
        UpdateGameOverUI();
        PlayExplosionEffect();
        CleanupAfterDeath();
    }

    // Ngừng game & physics
    private void StopGame()
    {
        enabled = false;
        rb.simulated = false;
    }

    // Tính và lưu high score
    private void UpdateHighScore()
    {
        if (!(_score > _highScore)) return;
        _highScore = _score;
        PlayerPrefs.SetFloat(HighScoreKey, _highScore);
        PlayerPrefs.Save();
    }

    // Hiển thị UI game over
    private void UpdateGameOverUI()
    {
        _highScoreText.text = "High Score: " + _highScore;
        _highScoreText.style.display = DisplayStyle.Flex;
        _restartButton.style.display = DisplayStyle.Flex;
    }

    // Tạo hiệu ứng nổ
    private void PlayExplosionEffect()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
    }

    // Dọn dẹp sau khi chết
    private void CleanupAfterDeath()
    {
        Destroy(gameObject, 0.05f);
        borderParent.SetActive(false);
    }

    // Reload scene hiện tại
    private static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
