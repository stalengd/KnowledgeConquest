using UnityEngine;
using UnityEngine.EventSystems;

namespace KnowledgeConquest.Client.UI
{
    public sealed class DualToggle : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject _active0Group;
        [SerializeField] private GameObject _active1Group;

        public bool Value 
        {
            get => _value;
            set
            {
                _value = value;
                RefreshVisuals();
                ValueChanged?.Invoke(value);
            }
        }

        public event System.Action<bool> ValueChanged;

        private bool _value;

        private void Start()
        {
            RefreshVisuals();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Value = !Value;
        }

        private void RefreshVisuals()
        {
            _active0Group.SetActive(!Value);
            _active1Group.SetActive(Value);
        }
    }
}
