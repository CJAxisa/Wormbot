using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform followTarget;
    public Vector2 safeZoneRatios;
    public Vector4 stageBounds;
    public float followVelocity = 0.01f;

    public bool debugOverlay;

    private Rect _viewDimensions;
    private Rect _safeZone;
    private Vector3 _heading;
    private Camera _camera;
    private float t;

    void Start() {
        _camera = GetComponent<Camera>();
        _viewDimensions.height = _camera.orthographicSize * 2;
        _viewDimensions.width = _viewDimensions.height * Screen.width / Screen.height;
        _viewDimensions.x = transform.position.x - _viewDimensions.width / 2f;
        _viewDimensions.y = transform.position.y - _viewDimensions.height / 2f;

        _safeZone.position = _viewDimensions.center - _viewDimensions.size * safeZoneRatios;
        _safeZone.size = _viewDimensions.size * safeZoneRatios;
    }
    
    void Update() {
        _viewDimensions.center = transform.position;
        _safeZone.center = transform.position;

        if (!_safeZone.Contains(followTarget.transform.position)) {
            if (t < 0) t = 0;
            t += followVelocity;

            _heading = transform.position - followTarget.position;

            if (_heading.x > _safeZone.size.x / 2.0f) _heading.x = _safeZone.size.x / 2.0f;
            else if(_heading.x < _safeZone.size.x / -2.0f) _heading.x = _safeZone.size.x / -2.0f;
            if (_heading.y > _safeZone.size.y / 2.0f) _heading.y = _safeZone.size.y / 2.0f;
            else if (_heading.y < _safeZone.size.y / -2.0f) _heading.y = _safeZone.size.y / -2.0f;

            transform.position += (followTarget.position - (transform.position - _heading)) * followVelocity;

        }
        else
        {
            if (t > 0) t -= followVelocity * 2.0f;
            else t = 0;
        }
        
        if (debugOverlay) {
            Debug.DrawLine(_viewDimensions.min, _viewDimensions.max, Color.blue);
            Debug.DrawLine(_safeZone.min, _safeZone.max, Color.red);
            Debug.DrawLine(_viewDimensions.center - new Vector2(_heading.x, _heading.y), followTarget.position, Color.red);
        }
    }
}
