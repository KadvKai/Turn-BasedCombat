using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndication : MonoBehaviour
{
    private TMP_Text _text;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    public void Damage(float damage)
    {
        gameObject.SetActive(true);
        _text.text = (-damage).ToString();
    }
    public void StopIndication()
    {
        gameObject.SetActive(false);
    }

}
