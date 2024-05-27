using UnityEngine;
using KnowledgeConquest.Client.Extensions;

namespace KnowledgeConquest.Client
{
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [Space]
        [SerializeField] private Vector2 _mousePanSpeed = Vector2.one;
        [SerializeField] private float _mouseZoomSpeed = 1f;
        [SerializeField] private Vector2 _touchPanSpeed = Vector2.one;
        [SerializeField] private float _touchZoomSpeed = 1f;

        [Space]
        [SerializeField] private Rect _cameraBounds;
        [SerializeField] private Vector2 _boundsOffset = Vector2.zero;
        [SerializeField] private ZoomEffect _zoomMin;
        [SerializeField] private ZoomEffect _zoomMax;

        [Space]
        [SerializeField] private float _panSmoothing = 5f;
        [SerializeField] private float _zoomSmoothing = 5f;

        [System.Serializable]
        private struct ZoomEffect
        {
            public float angle;
            public float fieldOfView;
            public float height;
        }

        public Camera Camera => _camera;
        public Vector2 TargetPosition => _targetPosition;
        public float TargetZoom => _targetZoom;
        public bool IsControlsEnabled { get; set; } = true;
        public Rect ViewBounds
        {
            get => _cameraBounds;
            set => _cameraBounds = value;
        }

        private Vector2 _targetPosition;
        private float _targetZoom;
        private float _currentZoom = 1f;
        private bool _isZooming = false;
        private float _lastZoomDistance;
        private Vector2 _lastPanScreenPosition;
        private int _panFingerId;

        private void Start()
        {
            _targetPosition = transform.position.ToXZPlane();
            _targetZoom = _currentZoom;
        }

        private void Update()
        {
            if (IsControlsEnabled)
            {
                if (Input.touchSupported)
                {
                    UpdateTouch();
                }
                else
                {
                    UpdateMouse();
                }
            }
            UpdatePosition();
            UpdateZoom();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(_cameraBounds.center, _cameraBounds.size.FromXZPlane());
        }

        public void MoveTo(Vector2 position)
        {
            var bounds = new Rect(_cameraBounds);
            bounds.center = _cameraBounds.center + _boundsOffset;
            position.x = Mathf.Clamp(position.x, bounds.xMin, bounds.xMax);
            position.y = Mathf.Clamp(position.y, bounds.yMin, bounds.yMax);
            _targetPosition = position;
        }

        public void Zoom(float zoom)
        {
            _targetZoom = Mathf.Clamp01(zoom);
        }

        private void UpdatePosition()
        {
            var pos = transform.position.ToXZPlane();
            pos = Vector2.Lerp(pos, _targetPosition, Time.deltaTime * _panSmoothing);
            transform.position = pos.FromXZPlane(transform.position.y);
        }

        private void UpdateZoom()
        {
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, Time.deltaTime * _zoomSmoothing);

            var angles = transform.rotation.eulerAngles;
            angles.x = Mathf.Lerp(_zoomMin.angle, _zoomMax.angle, _currentZoom);
            transform.rotation = Quaternion.Euler(angles);

            var pos = _camera.transform.localPosition;
            pos.z = -Mathf.Lerp(_zoomMin.height, _zoomMax.height, _currentZoom);
            _camera.transform.localPosition = pos;

            _camera.fieldOfView = Mathf.Lerp(_zoomMin.fieldOfView, _zoomMax.fieldOfView, _currentZoom);
        }

        private void UpdateMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                BeginPan(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                Pan(Input.mousePosition, _mousePanSpeed);
            }
            var scroll = -Input.GetAxis("Mouse ScrollWheel");
            Zoom(_targetZoom + scroll * _mouseZoomSpeed);
        }

        private void UpdateTouch()
        {
            var touchCount = Input.touchCount;
            if (touchCount == 1)
            {
                TouchPan();
            }

            if (touchCount == 2)
            {
                TouchZoom();
            }
            else
            {
                _isZooming = false;
            }
        }

        private void TouchPan()
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                BeginPan(touch.position);
                _panFingerId = touch.fingerId;
            }
            else if (touch.fingerId == _panFingerId && touch.phase == TouchPhase.Moved)
            {
                Pan(touch.position, _touchPanSpeed);
            }
        }

        private void TouchZoom()
        {
            var touchA = Input.GetTouch(0).position;
            var touchB = Input.GetTouch(1).position;
            var newDistance = Vector2.Distance(touchA, touchB);
            if (!_isZooming)
            {
                _isZooming = true;
            }
            else
            {
                var offset = newDistance - _lastZoomDistance;
                Zoom(_targetZoom - offset * _touchZoomSpeed);
            }
            _lastZoomDistance = newDistance; 
        }

        private void BeginPan(Vector2 screenPosition)
        {
            _lastPanScreenPosition = screenPosition;
        }

        private void Pan(Vector2 newScreenPosition, Vector2 speed)
        {
            var offset = ScreenVectorToWorld2d(_lastPanScreenPosition - newScreenPosition);
            var move = offset * speed;
            var pos = _targetPosition + move;
            MoveTo(pos);
            _lastPanScreenPosition = newScreenPosition;
        }

        private Vector2 ScreenVectorToWorld2d(Vector2 screenVector)
        {
            return screenVector / new Vector2(Screen.width, Screen.height);
        }
    }
}
