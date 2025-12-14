using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using Script.UI; // Để gọi GameController

public class PlayerStats : MonoBehaviour
{
    [Header("UI References")]
    public Image healthFill;
    public Image staminaFill;
    public GameObject deathPanel;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 10f;
    [SerializeField] private float currentStamina;

    private StarterAssetsInputs _input;
    private ThirdPersonController _controller;

    void Start()
    {
        // Gọi hàm reset để đảm bảo lúc mới vào game là đầy máu
        ResetStats();

        _input = GetComponent<StarterAssetsInputs>();
        _controller = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        HandleStamina();
        UpdateUI();
    }

    void HandleStamina()
    {
        if (_input == null) return;

        if (_input.sprint && _input.move != Vector2.zero)
        {
            if (currentStamina > 0)
            {
                currentStamina -= staminaDrainRate * Time.deltaTime;
            }
            else
            {
                currentStamina = 0;
                _input.sprint = false;
            }
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"Bị đánh! Máu còn: {currentHealth}");

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        currentHealth = 0;
        Debug.Log("Player Dead!");

        if (deathPanel != null)
        {
            // Bật UI cha (nếu có) để đảm bảo nó hiện lên
            if (deathPanel.transform.parent != null)
                deathPanel.transform.parent.gameObject.SetActive(true);

            // Gọi Pause Game
            Script.UI.GameController.PauseGame(deathPanel);

            // Mở khóa chuột
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogError("QUÊN KÉO CÁI DEATH PANEL VÀO SCRIPT PLAYER STATS RỒI ÔNG ƠI!");
        }
    }

    void UpdateUI()
    {
        if (healthFill != null) healthFill.fillAmount = currentHealth / maxHealth;
        if (staminaFill != null) staminaFill.fillAmount = currentStamina / maxStamina;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateUI();
    }

    // --- HÀM MỚI: RESET MÁU VÀ THỂ LỰC ---
    public void ResetStats()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        UpdateUI();
        Debug.Log("Đã hồi đầy máu và thể lực!");
    }
}