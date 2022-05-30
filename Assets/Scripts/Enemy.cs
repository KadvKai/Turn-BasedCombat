using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Battlefield _battlefield;
    [SerializeField] float _timeSelection = 1;
    public event UnityAction<Character> SelectedCharacter;

    private void OnEnable()
    {
        Invoke(nameof(SelectionCharacter), _timeSelection);
    }

    private void SelectionCharacter()
    {
        var character = _battlefield.PlayerCharacters[Random.Range(0, _battlefield.PlayerCharacters.Count)];
        SelectedCharacter?.Invoke(character);
    }
}
