using UnityEngine;
using System.Collections.Generic;

public class CarromStrikerMultiTouch : MonoBehaviour
{
    [Header("Settings")]
    public float maxForce = 15f;            // Maximum force on full drag
    public Transform boardPlane;            // Reference to board for raycast plane
    public string strikerTag = "Striker";   // Tag for selectable strikers

    // Each finger ID maps to striker + drag start position
    private class FingerData
    {
        public Rigidbody rb;
        public Vector3 dragStart;
    }

    private Dictionary<int, FingerData> activeTouches = new Dictionary<int, FingerData>();

    void Update()
    {
        Debug.Log(activeTouches.Count);
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                // Start drag
                case TouchPhase.Began:
                    Debug.Log("Hit: began");
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out RaycastHit hit))
                    {
                        
                        if (hit.collider.CompareTag(strikerTag))
                        {
                            Rigidbody rb = hit.collider.attachedRigidbody;
                            Vector3 dragStart = GetWorldPoint(touch.position);

                            if (rb != null)
                            {
                                activeTouches[touch.fingerId] = new FingerData
                                {
                                    rb = rb,
                                    dragStart = dragStart
                                };
                            }
                        }
                    }
                    break;

                // Release drag
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (activeTouches.ContainsKey(touch.fingerId))
                    {
                        FingerData fd = activeTouches[touch.fingerId];
                        Vector3 dragEnd = GetWorldPoint(touch.position);

                        Vector3 strikeDir = fd.dragStart - dragEnd; // pull back direction
                        strikeDir.y = 0f;

                        float distance = strikeDir.magnitude * 10;
                        Vector3 force = strikeDir.normalized * Mathf.Clamp(distance, 0f, maxForce);

                        fd.rb.AddForce(force, ForceMode.Impulse);

                        activeTouches.Remove(touch.fingerId); // remove after shooting
                    }
                    break;
            }
        }
    }

    // Convert screen point to board world position
    Vector3 GetWorldPoint(Vector3 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else if (boardPlane != null)
        {
            Plane plane = new Plane(Vector3.up, boardPlane.position);
            if (plane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
        }
        return Vector3.zero;
    }
}
