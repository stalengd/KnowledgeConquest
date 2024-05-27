using UnityEngine;

namespace KnowledgeConquest.Client.Utils
{
    public sealed class Billboard : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            transform.forward = _camera.transform.forward;
        }
    }
}
