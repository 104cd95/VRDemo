using UnityEngine;
using UnityEngine.EventSystems;

public class Canvas3D : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private Transform worldPosition;
    [SerializeField] private float sphereRadius;
    
    private Camera mainCamera;
    private Vector2 dragOffset;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Start dragging only when the left mouse button is pressed
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        // Save the mouse cursor offset relative to the canvas center in screen coordinates for smooth dragging
        Vector3 canvasCenter = mainCamera.WorldToScreenPoint(transform.position);
        dragOffset = eventData.position - new Vector2(canvasCenter.x, canvasCenter.y);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // Drag only when the left mouse button is pressed
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        // If the ray from the near plane of the camera intersects the virtual sphere,
        // move the canvas to the far intersection point and rotate it towards the center of the sphere, maintaining the horizon
        Ray ray = mainCamera.ScreenPointToRay(eventData.position - dragOffset);
        if (RaySphereIntersection(ray, worldPosition.position, sphereRadius, out Vector3 nearPoint, out Vector3 farPoint))
        {
            transform.position = farPoint;
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position, Vector3.up);
        }
    }
    
    private bool RaySphereIntersection(Ray ray, Vector3 sphereCenter, float sphereRadius, out Vector3 nearPoint, out Vector3 farPoint) 
    {
        // Vector from the ray origin to the sphere center
        Vector3 rayCenter = sphereCenter - ray.origin;
        // Projection of the rayCenter onto the ray direction
        float rayCenterProj = Vector3.Dot(rayCenter, ray.direction);
        // Distance between the sphere center and the projection end point using Pythagorean theorem
        float centerDist = Mathf.Sqrt(Vector3.Dot(rayCenter, rayCenter) - rayCenterProj * rayCenterProj);
        // Distance between the (near or far) intersection and the projection end point using Pythagorean theorem
        float intersectDist = Mathf.Sqrt(sphereRadius * sphereRadius - centerDist * centerDist);
        // Distance between the ray origin and the near intersection
        float nearIntersectDist = rayCenterProj - intersectDist;
        // Distance between the ray origin and the far intersection
        float farIntersectDist = rayCenterProj + intersectDist;
        // Intersections
        nearPoint = nearIntersectDist >= 0 ? ray.origin + ray.direction * nearIntersectDist : default;
        farPoint = farIntersectDist >= 0 ? ray.origin + ray.direction * farIntersectDist : default;
        return nearIntersectDist >= 0 || farIntersectDist >= 0;
    }
}
