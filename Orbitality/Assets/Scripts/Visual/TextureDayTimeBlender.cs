using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureDayTimeBlender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _target;
    public float Speed = 1;
    [SerializeField] private Sprite[] _sprite;
    private float _timeDone;
    private float _lastOffset;
    private int _idx=-1;

    void Awake() => SetNextSlide();

    void Update()
    {
        _timeDone += Time.deltaTime * Mathf.Max(float.MinValue, Speed);
        var newOffset = _timeDone % 1;
        if (newOffset < _lastOffset)
        {
            SetNextSlide();
        }
        _lastOffset = newOffset;
        _target.material.SetFloat("_BlendFactor", _lastOffset);
    }

    private void SetNextSlide()
    {
        _idx++;
        _target.sprite = _sprite[_idx % _sprite.Length];
        //_target.material.SetTexture("_MainTex", _sprite[_idx % _sprite.Length].texture);
        _target.material.SetTexture("_MainTex2", _sprite[(_idx + 1) % _sprite.Length].texture);
    }
}
