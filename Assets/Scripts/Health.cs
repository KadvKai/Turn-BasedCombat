using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private Slider _currentHealthSlider;
    [SerializeField] TMP_Text _currentHealthText;
    private float _currentHealth;
    private int _startHealth;
    public UnityEvent Dead = new();
    public UnityEvent Damage = new();
    
    void Start()
    {
        _startHealth = Random.Range(80, 120);
        _currentHealth = _startHealth;
        HealthIndication();
    }
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        Damage.Invoke();
        if (_currentHealth<=0)
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
