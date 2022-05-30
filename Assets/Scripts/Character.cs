using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private int _baseDamage = 30;
    [SerializeField] private AnimationReferenceAsset _damageAnimation, _idleAnimation, _attackAnimation;
    [SerializeField] private EventDataReferenceAsset _hittEvent;
    [SerializeField] private Slider _currentHealthSlider;
    [SerializeField] private Color _visibleColor;
    [SerializeField] private Color _hiddenColor;
    [SerializeField] private float _hidingTime = 1;
    private int _damage;
    private SkeletonAnimation _skeletonAnimation;
    private readonly string _colorProperty = "_Color";
    private MaterialPropertyBlock _block;
    private MeshRenderer _meshRenderer;
    private bool CharacterSelection;
    public bool CharacterActive { get; private set; }
    public bool CharacterDead { get; private set; }
    public event UnityAction<int> CharacterHitt;

    private void Awake()
    {
        _damage = Random.Range((int)(_baseDamage * 0.9f), (int)(_baseDamage * 1.1f));//Урон персонажа +-10%
        _block = new();
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        StartCoroutine(TSAnimation());
        _skeletonAnimation.AnimationState.Event += AnimationState_Event;
    }


    private void OnDestroy()
    {
        _skeletonAnimation.AnimationState.Event -= AnimationState_Event;
    }
    private IEnumerator TSAnimation()
    {
        _skeletonAnimation.timeScale = Random.Range(0.8f, 1.2f);
        yield return new WaitForSeconds(5f);
        _skeletonAnimation.timeScale = 1;
    }

    public void Targeting(bool targeting)
    {
        if (targeting)
        {
            _currentHealthSlider.gameObject.SetActive(true);
        }
        else
        {
            if (!CharacterSelection) _currentHealthSlider.gameObject.SetActive(false);
        }
    }

    public void Selection(bool selection)
    {
        _currentHealthSlider.gameObject.SetActive(selection);
        CharacterSelection = selection;
    }
    public void PreparingBattle(bool preparing)
    {
        if (preparing) _meshRenderer.sortingLayerName = "Battle";
        else _meshRenderer.sortingLayerName = "Default";
    }
    public void Attack()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0, _attackAnimation, false);
        _skeletonAnimation.AnimationState.AddAnimation(0, _idleAnimation, true, 0);
        CharacterActive = true;
        _skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
    }

    private void AnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data == _hittEvent.EventData) CharacterHitt?.Invoke(_damage);
    }

    public void Dead()
    {
        StartCoroutine(MaterialColorChange(_visibleColor, _hiddenColor));
        CharacterDead = true;
        Destroy(gameObject, _hidingTime);
    }
    public void Damage()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0, _damageAnimation, false);
        _skeletonAnimation.AnimationState.AddAnimation(0, _idleAnimation, true, 0);
        CharacterActive = true;
        _skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
    }

    public void Hide(bool hide)
    {
        if (hide) StartCoroutine(MaterialColorChange(_visibleColor, _hiddenColor));
        else StartCoroutine(MaterialColorChange(_hiddenColor, _visibleColor));
    }
    private IEnumerator MaterialColorChange(Color startColor, Color finishColor)
    {
        var time = _hidingTime;
        Color color;
        while (time > 0)
        {
            color = Color.Lerp(finishColor, startColor, time / _hidingTime);
            _block.SetColor(_colorProperty, color);
            _meshRenderer.SetPropertyBlock(_block);
            yield return null;
            time -= Time.deltaTime;
        }
    }
    private void AnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        CharacterActive = false;
        _skeletonAnimation.AnimationState.Complete -= AnimationState_Complete;

    }
}
