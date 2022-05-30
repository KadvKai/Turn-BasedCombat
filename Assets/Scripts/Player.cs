using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Battlefield _battlefield;
    [SerializeField] private Canvas _playerCanvas;
    private PlayerInput _playrInput;
    private Camera _camera;
    private Character _currentCharacter;
    public event UnityAction<Character> SelectedCharacter;
    private void Awake()
    {
        _camera = Camera.main;
        _playrInput = new();
        _playrInput.BaseInput.Targeting.performed += Selected_performed;
        _playrInput.BaseInput.Targeting.Enable();
        _playrInput.BaseInput.Selection.canceled += cont => Selection_canceled();
    }

    public void AttackButton()
    {
        _playerCanvas.gameObject.SetActive(false);
        _playrInput.BaseInput.Selection.Enable();
    }
    public void SkipButton()
    {
        _playerCanvas.gameObject.SetActive(false);
        SelectedCharacter?.Invoke(null);
    }

    private void Selection_canceled()
    {
        if (_currentCharacter != null && _battlefield.EnemyCharacters.Contains(_currentCharacter))
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
    private void OnDestroy()
    {
        _playrInput.BaseInput.Targeting.performed -= Selected_performed;
        _playrInput.BaseInput.Selection.canceled -= cont => Selection_canceled();
        _playrInput.BaseInput.Targeting.Disable();
    }

    private void OnEnable()
    {
        _playerCanvas.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        _playrInput.BaseInput.Selection.Disable();
    }
}
