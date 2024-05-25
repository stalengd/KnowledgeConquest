using UnityEngine;

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
        [SerializeField] private float _zoomMin = 1f;
        [SerializeField] private float _zoomMax = 5f;

        [Space]
        [SerializeField] private float _panSmoothing = 5f;
        [SerializeField] private float _zoomSmoothing = 5f;

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
        private bool _isZooming = false;
        private float _lastZoomDistance;
        private Vector2 _lastPanScreenPosition;
        private int _panFingerId;

        private void Start()
        {
            _targetPosition = transform.position;
            _targetZoom = _camera.orthographicSize;
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
            Gizmos.DrawWireCube(_cameraBounds.center, _cameraBounds.size);
        }

        public void MoveTo(Vector2 position)
        {
            var zoom = _targetZoom;
            var worldViewSize = GetWorldViewSize(zoom);
            var bounds = new Rect(_cameraBounds);
            bounds.size -= worldViewSize;
            bounds.center = _cameraBounds.center;
            position.x = Mathf.Clamp(position.x, bounds.xMin, bounds.xMax);
            position.y = Mathf.Clamp(position.y, bounds.yMin, bounds.yMax);
            _targetPosition = position;
        }

        public void Zoom(float zoom)
        {
            _targetZoom = Mathf.Clamp(zoom, _zoomMin, _zoomMax);
            MoveTo(_targetPosition);
        }

        public Vector2 GetWorldViewSize(float zoom)
        {
            return new Vector2(zoom * 2f * (Screen.width / (float)Screen.height), zoom * 2f);
        }

        private void UpdatePosition()
        {
            var pos = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _panSmoothing);
            pos.z = transform.position.z;
            transform.position = pos;
        }

        private void UpdateZoom()
        {
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, Time.deltaTime * _zoomSmoothing);
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
            var normalized = screenVector / new Vector2(Screen.width, Screen.height);
            return normalized * GetWorldViewSize(_targetZoom);
        }
    }
}
