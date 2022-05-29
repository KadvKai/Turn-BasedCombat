using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Battlefield))]
public class Enemy : MonoBehaviour
{
    private Battlefield _battlefield;
    public event UnityAction<Character> SelectedCharacter;
    private void Awake()
    {
        _battlefield = GetComponent<Battlefield>();
    }
    private void OnEnable()
    {
        var character = _battlefield.PlayerCharacters[Random.Range(0, _battlefield.PlayerCharacters.Count)];
        SelectedCharacter?.Invoke(character);
    }
}
