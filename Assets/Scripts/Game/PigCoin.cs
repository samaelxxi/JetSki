using UnityEngine;
using DG.Tweening;

public class PigCoin : MonoBehaviour
{
    [SerializeField] LayerMask _interactionLayerMask;

    public bool IsSpawned => _isSpawned;

    Vector3 _scale;
    bool _isSpawned = true;
    bool _isCollected = false;

    void Awake()
    {
        _scale = transform.localScale;
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _interactionLayerMask) == 0)
            return;

        Collect();
    }

    public void Spawn()
    {
        if (_isSpawned)
            return;

        _isSpawned = true;
        _isCollected = false;
        transform.localScale = _scale;
        gameObject.SetActive(true);
    }

    public void Collect()
    {
        if (!_isSpawned || _isCollected)
            return;

        _isCollected = true;

        // _isSpawned = false;
        // gameObject.SetActive(false);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOPunchScale(Vector3.one * 1.1f, 0.4f, vibrato: 4, elasticity: 0.4f)
            .SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            _isSpawned = false;
            gameObject.SetActive(false);
        });

        seq.Play();
    }
}
