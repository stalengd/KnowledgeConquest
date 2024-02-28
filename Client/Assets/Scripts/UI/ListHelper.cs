using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KnowledgeConquest.Client.UI
{
    [System.Serializable]
    public struct ListHelper<T> where T : Component
    {
        [SerializeField] private RectTransform _holder;
        [SerializeField] private T _itemPrefab;
        [SerializeField] private bool _useHolderChildAsPrefab;


        public T this[int index]
        {
            get
            {
                if (_useHolderChildAsPrefab) index += 1;
                return _holder.GetChild(index).GetComponent<T>();
            }
        }

        public int ElementsCount
        {
            get { return _holder.childCount; }
        }

        private GameObject ItemPrefab 
        {
            get
            {
                if (_useHolderChildAsPrefab)
                {
                    var prefab = _itemPrefab.gameObject;
                    prefab.SetActive(false);
                    return prefab;
                }
                return _itemPrefab.gameObject;
            }
        }

        public IEnumerable<T> GetElements()
        {
            return _holder.OfType<Transform>().Select(t => t.GetComponent<T>());
        }

        public T CreateElement()
        {
            var go = Object.Instantiate(ItemPrefab, _holder);
            go.SetActive(true);
            return go.GetComponent<T>();
        }

        public void CreateElements<FromT>(IEnumerable<FromT> fromCollection, System.Action<FromT, T> action)
        {
            foreach (var item in fromCollection)
            {
                var controller = CreateElement();
                action(item, controller);
            }
        }

        public void SetElements<FromT>(IEnumerable<FromT> fromCollection, System.Action<FromT, T> action)
        {
            // TODO: More effective way
            Clear();
            CreateElements(fromCollection, action);
        }


        public void Clear()
        {
            foreach (Transform element in _holder)
            {
                if (_useHolderChildAsPrefab && element == _itemPrefab.transform) continue;
                Object.Destroy(element.gameObject);
            }
        }
    }

    [System.Serializable]
    public struct ListHelper<TComponent, TModel> where TComponent : Component
    {
        [SerializeField] private RectTransform _holder;
        [SerializeField] private TComponent _itemPrefab;
        [SerializeField] private bool _useHolderChildAsPrefab;

        public readonly TComponent this[int index] => _holder.GetChild(index).GetComponent<TComponent>();
        public readonly TComponent this[TModel model] => _map?.GetValueOrDefault(model);

        public readonly int ElementsCount => _map?.Count ?? 0;

        private GameObject ItemPrefab 
        {
            get
            {
                if (_useHolderChildAsPrefab)
                {
                    var prefab = _itemPrefab.gameObject;
                    prefab.SetActive(false);
                    return prefab;
                }
                return _itemPrefab.gameObject;
            }
        }

        private Dictionary<TModel, TComponent> _map;


        public readonly IEnumerable<TComponent> GetElements()
        {
            return _map != null ? _map.Values : Enumerable.Empty<TComponent>();
        }

        public TComponent CreateElement(TModel model)
        {
            _map ??= new Dictionary<TModel, TComponent>();
            var go = Object.Instantiate(ItemPrefab, _holder);
            go.SetActive(true);
            var component = go.GetComponent<TComponent>();
            _map.Add(model, component);
            return component;
        }

        public void CreateElements(IEnumerable<TModel> fromCollection, System.Action<TModel, TComponent> action)
        {
            foreach (var model in fromCollection)
            {
                var controller = CreateElement(model);
                action(model, controller);
            }
        }

        public void SetElements(IEnumerable<TModel> fromCollection, System.Action<TModel, TComponent> action)
        {
            // TODO: More effective way
            Clear();
            CreateElements(fromCollection, action);
        }


        public void Clear()
        {
            _map ??= new Dictionary<TModel, TComponent>();
            foreach (Transform element in _holder)
            {
                if (_useHolderChildAsPrefab && element == _itemPrefab.transform) continue;
                Object.Destroy(element.gameObject);
            }
            _map.Clear();
        }
    }

}