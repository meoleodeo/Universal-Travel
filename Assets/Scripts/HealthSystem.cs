using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Settings")]
    public bool isPlayer = false; 
    [SerializeField] private int maxHealth = 3; 
    
    [Header("UI (Optional)")]
    public TextMeshProUGUI healthText; 

    [Header("Events")]
    public UnityEvent OnDeath;
    public UnityEvent<int> OnHealthChanged;
    
    // --- THÊM SỰ KIỆN NÀY ---
    public UnityEvent OnDamaged; 
    // ------------------------

    private int _localHealth;

    public int CurrentHealth
    {
        get { return isPlayer ? PlayerData.currentHealth : _localHealth; }
        private set 
        {
            if (isPlayer) PlayerData.currentHealth = value;
            else _localHealth = value;
        }
    }

    private void Start()
    {
        if (isPlayer)
        {
            OnHealthChanged?.Invoke(PlayerData.currentHealth);
            UpdateHealthUI();
        }
        else
        {
            _localHealth = maxHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;

        OnDamaged?.Invoke();

        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }
        
        if (isPlayer) UpdateHealthUI();
    }
    
    public void Heal(int amount)
    {
        if (CurrentHealth <= 0) return;
        CurrentHealth += amount;
        int limit = isPlayer ? PlayerData.maxHealth : maxHealth;
        if (CurrentHealth > limit) CurrentHealth = limit;
        OnHealthChanged?.Invoke(CurrentHealth);
        if (isPlayer) UpdateHealthUI();
    }

    private void Die()
    {
        if (isPlayer && AudioManager.instance != null) AudioManager.instance.StopMusic();
        OnDeath?.Invoke();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null && isPlayer) healthText.text = "Health: " + PlayerData.currentHealth.ToString();
    }
}