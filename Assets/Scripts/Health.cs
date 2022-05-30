using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int _baseHealth = 100;
    [SerializeField] private Slider _currentHealthSlider;
    [SerializeField] private TMP_Text _currentHealthText;
    private float _currentHealth;
    private int _startHealth;
    public UnityEvent Dead = new();
    public UnityEvent<float> Damage = new();

    void Start()
    {
        _startHealth = Random.Range((int)(_baseHealth * 0.9f), (int)(_baseHealth * 1.1f));//Здоровье персонажа +-10%
        _currentHealth = _startHealth;
        HealthIndication();
    }
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _startHealth);
        HealthIndication();
        Damage.Invoke(damage);
        if (_currentHealth <= 0)
        {
            Dead.Invoke();
        }
    }

    private void HealthIndication()
    {
        _currentHealthText.text = _currentHealth.ToString();
        _currentHealthSlider.value = _currentHealth / _startHealth;
    }
}
