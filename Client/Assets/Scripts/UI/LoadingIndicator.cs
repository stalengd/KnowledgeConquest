using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KnowledgeConquest.Client.UI
{
    public sealed class LoadingIndicator : MonoBehaviour
    {
        [SerializeField] private Image _loadingIconImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private float _animationFrequency = 1f;
        [SerializeField] private Color _backgroundColorA;
        [SerializeField] private Color _backgroundColorB;

        private readonly List<Tween> _tweens = new();

        public void Show()
        {
            gameObject.SetActive(true);

            _tweens.Clear();
            _tweens.Add(_loadingIconImage.transform.DORotate(new Vector3(0f, 0f, 360f), 1 / _animationFrequency, RotateMode.FastBeyond360).SetLoops(-1));

            _backgroundImage.color = _backgroundColorA;
            var bgSeq = DOTween.Sequence(this);
            bgSeq.Append(_backgroundImage.DOColor(_backgroundColorB, 1 / _animationFrequency / 2));
            bgSeq.Append(_backgroundImage.DOColor(_backgroundColorA, 1 / _animationFrequency / 2));
            bgSeq.SetLoops(-1);
            _tweens.Add(bgSeq);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            foreach (var tween in _tweens)
            {
                tween.Kill();
            }
        }
    }
}
