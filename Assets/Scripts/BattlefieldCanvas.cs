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
    [SerializeField] private TMP_Text _playerWin;
    [SerializeField] private TMP_Text _playerLost;

    public void NewRound()
    {
        StartCoroutine(ShowText(_newRoundText.gameObject));
    }
    public void PlayerTurn(bool playerTurn)
    {
        if (playerTurn)
        {
            StartCoroutine(ShowText(_playerText.gameObject));
            _enemyText.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(ShowText(_enemyText.gameObject));
            _playerText.gameObject.SetActive(false);
        }
    }

    public void PlayerWin(bool playerWin)
    {
        if (playerWin) _playerWin.gameObject.SetActive(true);
        else _playerLost.gameObject.SetActive(true);
    }

    private IEnumerator ShowText(GameObject textGameObject)
    {
        textGameObject.SetActive(true);
        yield return new WaitForSeconds(_showTime);
        textGameObject.SetActive(false);
    }
}
