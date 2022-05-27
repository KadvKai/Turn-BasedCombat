using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private AnimationReferenceAsset _damage, _idle, _attack;
    [SerializeField] private Slider _currentHealthSlider;
   
    private SkeletonAnimation _skeletonAnimation;
    public Color freezeColor;
    public Color freezeBlackColor;
    public string colorProperty = "_Color";
    public string blackTintProperty = "_Black";
    private MaterialPropertyBlock _block;
    private MeshRenderer _meshRenderer;



    private void Start()
    {
        _block = new();
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        StartCoroutine(TSAnimation());
        _meshRenderer = GetComponent<MeshRenderer>();
        Dead();
    }
    private IEnumerator TSAnimation()
    {
        _skeletonAnimation.timeScale = Random.Range(0.8f,1.2f);
        yield return new WaitForSeconds(5f);
        _skeletonAnimation.timeScale = 1;
    }

    public void Selected()
    {
       
    }
    public void UnSelected()
    {

    }

    public void Dead()
    {
        Debug.Log("dead");
        _block.SetColor(colorProperty, freezeColor);
        _block.SetColor(blackTintProperty, freezeBlackColor);
        _meshRenderer.SetPropertyBlock(_block);
    }

    public void Damage()
    {

    }

}
