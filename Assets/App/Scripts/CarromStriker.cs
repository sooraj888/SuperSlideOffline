using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarromStriker : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Settings")]
    public float maxForce = 15f; // maximum force on full drag
    public Transform boardPlane;  // reference to board for raycast plane

    private Vector3 dragStart;
    private bool isDragging = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.FreezeRotationX |
        //                 RigidbodyConstraints.FreezeRotationZ |
        //                 RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
        // Start drag
        if (Input.GetMouseButtonDown(0))
        {
            dragStart = GetWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        // Release drag
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 dragEnd = GetWorldPoint(Input.mousePosition);
            Vector3 strikeDir = dragStart - dragEnd; // pull back direction
            strikeDir.y = 0f;

            float distance = strikeDir.magnitude * 10;
            Vector3 force = strikeDir.normalized * Mathf.Clamp(distance, 0f, maxForce);

            rb.AddForce(force, ForceMode.Impulse);
            isDragging = false;
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
