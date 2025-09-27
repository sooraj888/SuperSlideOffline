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

    [SerializeField] private StrikerOnPress strikerOnPress;
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
                        
                        if (hit.collider.CompareTag(strikerTag) && ((strikerOnPress.p1SelectedStriker !=null && strikerOnPress.p1SelectedStriker == hit.collider.gameObject) || (strikerOnPress.p2SelectedStriker != null && strikerOnPress.p2SelectedStriker == hit.collider.gameObject)))
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

                        //float distance = strikeDir.magnitude * 10;

                        float distance = strikeDir.magnitude;

                        // Map distance to force
                        float power = Mathf.Clamp(distance * maxForce, 0f, maxForce);


                        //Vector3 force = strikeDir.normalized * Mathf.Clamp(distance, 0f, maxForce);

                        Vector3 force = strikeDir.normalized * power;


                        if (strikerOnPress.p1SelectedStriker != null && strikerOnPress.p1SelectedStriker == fd.rb.gameObject  && strikerOnPress.IsP1StrikerSelected == true)
                        {
                            strikerOnPress.IsP1StrikerSelected = false;
                            activeTouches.Remove(touch.fingerId);
                            return;
                        }
                        if (strikerOnPress.p2SelectedStriker != null && strikerOnPress.p2SelectedStriker == fd.rb.gameObject && strikerOnPress.IsP2StrikerSelected == true)
                        {
                            strikerOnPress.IsP2StrikerSelected = false;
                            activeTouches.Remove(touch.fingerId);
                            return;
                        }

                        fd.rb.AddForce(force, ForceMode.Impulse);

                        strikerOnPress.ResetStrikers(resetGO: fd.rb.gameObject);


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
