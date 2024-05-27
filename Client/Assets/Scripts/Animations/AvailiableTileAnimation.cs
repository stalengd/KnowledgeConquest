using DG.Tweening;
using UnityEngine;

namespace KnowledgeConquest.Client.Animations
{
    public sealed class AvailiableTileAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 _scaleFrom;
        [SerializeField] private Vector3 _scaleTo;
        [SerializeField] private float _frequency = 1f;

        private Sequence _sequence;

        private void Start()
        {
            transform.localScale = _scaleFrom;
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOScale(_scaleTo, 1 / _frequency / 2));
            _sequence.Append(transform.DOScale(_scaleFrom, 1 / _frequency / 2));
            _sequence.SetLoops(-1);
        }

        private void OnDestroy()
        {
            _sequence.Kill();
        }
    }
}
