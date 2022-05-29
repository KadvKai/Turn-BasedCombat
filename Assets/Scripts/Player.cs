using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Battlefield))]
public class Player : MonoBehaviour
{
    private PlayerInput _playrInput;
    private Camera _camera;
    private Character _currentCharacter;
    private Battlefield _battlefield;
    public event UnityAction<Character> SelectedCharacter;
    private void Awake()
    {
        _battlefield = GetComponent<Battlefield>();
        _camera = Camera.main;
        _playrInput = new();
        _playrInput.BaseInput.Targeting.performed += Selected_performed;
        _playrInput.BaseInput.Targeting.Enable();
        _playrInput.BaseInput.Selection.canceled +=cont=> Selection_canceled();

    }
    private void OnDestroy()
    {
        _playrInput.BaseInput.Targeting.performed -= Selected_performed;
        _playrInput.BaseInput.Selection.canceled -= cont => Selection_canceled();
        _playrInput.BaseInput.Targeting.Disable();
    }

    private void OnEnable()
    {
        _playrInput.BaseInput.Selection.Enable();
    }
    private void OnDisable()
    {
        _playrInput.BaseInput.Selection.Disable();
    }
    private void Selection_canceled()
    {
        if (_currentCharacter!=null&& _battlefield.EnemyCharacters.Contains(_currentCharacter))
        {
            SelectedCharacter?.Invoke(_currentCharacter);
        }
    }

    private void Selected_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        var ray = _camera.ScreenPointToRay(context.ReadValue<Vector2>());
        var collider = Physics2D.GetRayIntersection(ray, Mathf.Infinity).collider;
        if (collider != null)
        {
            var character = collider.GetComponent<Character>();
            if (character != _currentCharacter)
            {
                if (_currentCharacter != null) _currentCharacter.Targeting(false);
                if (character != null) character.Targeting(true);
                _currentCharacter = character;
            }
        }
        else
        {
            if (_currentCharacter != null) _currentCharacter.Targeting(false);
            _currentCharacter = null;
        }
    }
}
