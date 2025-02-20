using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private const float ZOOM_LERP_SPEED = 0.1f;
    private float dragSpeed = 0.1f;
    private float zoomSpeed = 10;
    private float minZoom = 6f;
    private float maxZoom = 14f;
    private Vector3 movementBoundsMin;
    private Vector3 movementBoundsMax;
    private float movementBoundsSize = 20;

    private Vector3 lastMousePosition;
    private Camera cam;
    private Transform t;
    private float targetZoom;

    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    private bool IsOverUi()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    private void Awake()
    {
        cam = Camera.main;
        t = transform;
    }
    private void Start()
    {
        movementBoundsMin = t.position - Vector3.one * movementBoundsSize;
        movementBoundsMax = t.position + Vector3.one * movementBoundsSize;
        targetZoom = cam.orthographicSize;
    }
    private void LateUpdate()
    {
        HandleCameraMovement();
        HandleCameraZoom();
    }

    private void HandleCameraMovement()
    {
        if (IsOverUi()) return;
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            Vector3 move = new Vector3(-delta.x * dragSpeed, -delta.y * dragSpeed, 0);
            t.transform.position += Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * move;
            t.transform.position = new Vector3(
                Mathf.Clamp(t.transform.position.x, movementBoundsMin.x, movementBoundsMax.x),
                Mathf.Clamp(t.transform.position.y, movementBoundsMin.y, movementBoundsMax.y),
                Mathf.Clamp(t.transform.position.z, movementBoundsMin.z, movementBoundsMax.z)
            );
        }
    }

    private void HandleCameraZoom()
    {
        
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            targetZoom -= scrollInput * zoomSpeed;
        }
        if (Input.touchCount == 2)
        {
            float touchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            if (touchDistance > 0)
            {
                float zoomDelta = touchDistance * zoomSpeed * Time.deltaTime;
                targetZoom -= zoomDelta;
            }
        }
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, ZOOM_LERP_SPEED);
    }
}
