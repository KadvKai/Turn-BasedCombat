using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattlefieldCanvas : MonoBehaviour
{
    [SerializeField] private float _showTime;
    [SerializeField] private TMP_Text _newRoundText;
    [SerializeField] private TMP_Text _playerText;
    [SerializeField] private TMP_Text _enemyText;

    public void NewRound()
    {
        StartCoroutine(ShowText(_newRoundText.gameObject));
    }
    public void PlayerTurn(bool playerTurn)
    {
        if (playerTurn) StartCoroutine(ShowText(_playerText.gameObject));
        else StartCoroutine(ShowText(_enemyText.gameObject));
    }

    private IEnumerator ShowText(GameObject textGameObject)
    {
        textGameObject.SetActive(true);
        yield return new WaitForSeconds(_showTime);
        textGameObject.SetActive(false);
    }
}
