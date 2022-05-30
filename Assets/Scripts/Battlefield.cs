using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Battlefield : MonoBehaviour
{
    [SerializeField] private Character[] _characterType;
    [SerializeField] private List<Transform> _playerPosition;
    [SerializeField] private List<Transform> _enemyPosition;
    [SerializeField] private Transform _playerBattlePosition;
    [SerializeField] private Transform _enemyBattlePosition;
    [SerializeField] private float _timeEnableBattle = 1;
    [SerializeField] private float _timeBattle = 1;
    [SerializeField] private SpriteRenderer _fog;
    [SerializeField] private Color _fogColor;
    [SerializeField] private float _timeFog = 1;
    [SerializeField] private Player _player;
    [SerializeField] private Enemy _enemy;
    private readonly List<Character> _charactersOnField = new();
    private readonly List<Character> _charactersRound = new();
    private Character _attackingCharacter;
    private Character _defendingCharacter;
    public List<Character> PlayerCharacters { get; private set; } = new();
    public List<Character> EnemyCharacters { get; private set; } = new();

    public UnityEvent NewRound = new();
    public UnityEvent<bool> PlayerTurn = new();
    public UnityEvent<bool> PlayerWin = new();
    private void Awake()
    {
        foreach (var position in _playerPosition)
        {
            var characterType = _characterType[Random.Range(0, _characterType.Length)];
            var character = Instantiate(characterType, position.position, Quaternion.identity);
            character.name = "Player" + position.position.x;
            _charactersOnField.Add(character);
            PlayerCharacters.Add(character);
        }
        foreach (var position in _enemyPosition)
        {
            var characterType = _characterType[Random.Range(0, _characterType.Length)];
            var character = Instantiate(characterType, position.position, Quaternion.identity);
            character.name = "Enemy" + position.position.x;
            character.GetComponent<SkeletonAnimation>().Skeleton.ScaleX = -1f;
            _charactersOnField.Add(character);
            EnemyCharacters.Add(character);
        }
    }
    private void Start()
    {
        StateSelection();
    }
    private void StateSelection()
    {
        if (_player == null || _enemy == null) return;
        if (_charactersRound.Count == 0)
        {
            _charactersRound.AddRange(_charactersOnField);
            NewRound.Invoke();
        }
        var ind = Random.Range(0, _charactersRound.Count);
        var character = _charactersRound[ind];
        _charactersRound.Remove(character);
        character.Selection(true);
        _attackingCharacter = character;
        if (PlayerCharacters.Contains(character))
        {
            PlayerTurn.Invoke(true);
            _player.SelectedCharacter += PlayerSelectedCharacter;
            _player.enabled = true;
        }
        else
        {
            PlayerTurn.Invoke(false);
            _enemy.SelectedCharacter += EnemySelectedCharacter;
            _enemy.enabled = true;
        }
    }
    private IEnumerator StateBattle()
    {
        yield return StartCoroutine(StartBattle());
        yield return StartCoroutine(Battle());
        yield return EndBattle();
        StateSelection();
    }

    private IEnumerator StartBattle()
    {
        _attackingCharacter.PreparingBattle(true);
        _defendingCharacter.PreparingBattle(true);
        yield return StartCoroutine(FogChange(_fogColor));
        _attackingCharacter.CharacterHitt += AttackingCharacter_CharacterHitt;
        Vector2 attackingCharacterPosition;
        Vector2 defendingCharacterPosition;
        if (PlayerCharacters.Contains(_attackingCharacter))
        {
            attackingCharacterPosition = _playerBattlePosition.position;
            defendingCharacterPosition = _enemyBattlePosition.position;
        }
        else
        {
            attackingCharacterPosition = _enemyBattlePosition.position;
            defendingCharacterPosition = _playerBattlePosition.position;
        }
        StartCoroutine(MovementCharacter(_defendingCharacter, defendingCharacterPosition));
        yield return StartCoroutine(MovementCharacter(_attackingCharacter, attackingCharacterPosition));
    }

    private IEnumerator Battle()
    {
        _attackingCharacter.Attack();
        yield return new WaitForSeconds(_timeBattle);
    }

    private IEnumerator EndBattle()
    {
        yield return new WaitWhile(() => _attackingCharacter.CharacterActive);
        yield return new WaitWhile(() => _defendingCharacter.CharacterActive);
        _attackingCharacter.CharacterHitt -= AttackingCharacter_CharacterHitt;
        _attackingCharacter.Selection(false);
        _defendingCharacter.Selection(false);

        Vector2 attackingCharacterPosition;
        Vector2 defendingCharacterPosition = Vector2.zero;
        if (PlayerCharacters.Contains(_attackingCharacter))
        {
            var attackingCharacterIndext = PlayerCharacters.IndexOf(_attackingCharacter);
            attackingCharacterPosition = _playerPosition[attackingCharacterIndext].position;
            if (!_defendingCharacter.CharacterDead)
            {
                var defendingCharacterIndext = EnemyCharacters.IndexOf(_defendingCharacter);
                defendingCharacterPosition = _enemyPosition[defendingCharacterIndext].position;
            }
        }
        else
        {
            var attackingCharacterIndext = EnemyCharacters.IndexOf(_attackingCharacter);
            attackingCharacterPosition = _enemyPosition[attackingCharacterIndext].position;
            if (!_defendingCharacter.CharacterDead)
            {
                var defendingCharacterIndext = PlayerCharacters.IndexOf(_defendingCharacter);
                defendingCharacterPosition = _playerPosition[defendingCharacterIndext].position;
            }
        }

        if (!_defendingCharacter.CharacterDead) StartCoroutine(MovementCharacter(_defendingCharacter, defendingCharacterPosition));
        else yield return StartCoroutine(CharacterDead(_defendingCharacter));
        yield return StartCoroutine(MovementCharacter(_attackingCharacter, attackingCharacterPosition));
        yield return StartCoroutine(FogChange(Color.clear));
        _attackingCharacter.PreparingBattle(false);
        if (!_defendingCharacter.CharacterDead) _defendingCharacter.PreparingBattle(false);
    }

    private IEnumerator MovementCharacter(Character character, Vector2 position)
    {
        Vector2 startposition = character.transform.position;
        var startTimeMovement = _timeEnableBattle;
        var timeMovement = startTimeMovement;
        while (timeMovement > 0)
        {
            timeMovement -= Time.deltaTime;
            character.transform.position = Vector2.Lerp(position, startposition, timeMovement / startTimeMovement);
            yield return null;
        }
        character.transform.position = position;
    }

    private IEnumerator FogChange(Color color)
    {
        var timeChange = _timeFog;
        var startColor = _fog.color;
        while (timeChange > 0)
        {
            timeChange -= Time.deltaTime;
            _fog.color = Color.Lerp(color, startColor, timeChange / _timeFog);
            yield return null;
        }
    }
    private void AttackingCharacter_CharacterHitt(int damage)
    {
        _defendingCharacter.GetComponent<Health>().TakeDamage(damage);
    }

    private void EnemySelectedCharacter(Character character)
    {
        _enemy.enabled = false;
        _enemy.SelectedCharacter -= EnemySelectedCharacter;
        character.Selection(true);
        _defendingCharacter = character;
        StartCoroutine(StateBattle()); ;
    }
    private void PlayerSelectedCharacter(Character character)
    {
        _player.enabled = false;
        _player.SelectedCharacter -= PlayerSelectedCharacter;
        if (character != null)
        {
            character.Selection(true);
            _defendingCharacter = character;
            StartCoroutine(StateBattle());
        }
        else
        {
            _attackingCharacter.Selection(false);
            StateSelection();
        }
    }

    private IEnumerator CharacterDead(Character character)
    {
        _charactersOnField.Remove(character);
        _charactersRound.Remove(character);
        if (PlayerCharacters.Contains(character))
        {
            PlayerCharacters.Remove(character);
            foreach (var playerCharacter in PlayerCharacters)
            {
                var playerCharacterIndext = PlayerCharacters.IndexOf(playerCharacter);
                var playerCharacterPosition = _playerPosition[playerCharacterIndext].position;
                StartCoroutine(MovementCharacter(playerCharacter, playerCharacterPosition));
            }
        }
        if (EnemyCharacters.Contains(character))
        {
            EnemyCharacters.Remove(character);
            foreach (var enemyCharacter in EnemyCharacters)
            {
                var enemyCharacterIndext = EnemyCharacters.IndexOf(enemyCharacter);
                var enemyCharacterPosition = _enemyPosition[enemyCharacterIndext].position;
                StartCoroutine(MovementCharacter(enemyCharacter, enemyCharacterPosition));
            }
        }
        if (PlayerCharacters.Count == 0)
        {
            PlayerWin.Invoke(false);
            Destroy(_player);
            Destroy(_enemy);
        }
        else if (EnemyCharacters.Count == 0)
        {
            PlayerWin.Invoke(true);
            Destroy(_player);
            Destroy(_enemy);
        }
        yield return character == null;
    }

}
