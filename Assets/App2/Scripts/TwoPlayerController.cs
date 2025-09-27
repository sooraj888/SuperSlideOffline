using System.Collections.Generic;
using UnityEngine;

public class TwoPlayerController : MonoBehaviour
{
    [Header("Assign your prefab here")]
    public GameObject prefab;

    public string strikerTag = "Striker";
    public float boardHeight = 0f; // Y position of the board surface

    [SerializeField]
    private float speed = 10f; // Speed of movement

    [Header("Settings")]
    public float maxForce = 15f;
    private class FingerData
    {
        public GameObject HighLiter;
        public Rigidbody rb;
        public Vector3 offset;

        public bool isPositionLocked = false;
        public float lockXPos = 0f;
    }

    private Dictionary<int, FingerData> activeTouches = new Dictionary<int, FingerData>();

    [Header("Board Boundary")]
    public BoxCollider boardCollider; // Assign your board's BoxCollider in Inspector


    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out RaycastHit hit))
                    {
                        Rigidbody rb = hit.collider.attachedRigidbody;

                        Vector3 screenPos = new Vector3(touch.position.x, touch.position.y, Camera.main.WorldToScreenPoint(hit.collider.attachedRigidbody.position).z);
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                        Vector3 targetPos = new Vector3(worldPos.x, rb.position.y, worldPos.z);

                        Vector3 clickOffset = rb.position - hit.point;

                        // Add offset if needed
                        targetPos += new Vector3(clickOffset.x, 0f, clickOffset.z);


                        //  Clamp using BoxCollider bounds
                        Bounds bounds = boardCollider.bounds;

                        if (targetPos.z > 0)
                        {
                            Debug.Log("zpos" + targetPos.z + "bonds min" + bounds.min.z + "bound max" + bounds.max.z);
                            return;
                        }

                        if (rb != null && hit.collider.CompareTag(strikerTag))
                        {
                            activeTouches[touch.fingerId] = new FingerData
                            {
                                HighLiter = Instantiate(prefab, hit.collider.transform),
                                rb = rb,
                                offset = clickOffset,
                                lockXPos = rb.position.x
                            };
                        }
                    }
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (activeTouches.TryGetValue(touch.fingerId, out FingerData fd))
                    {
                        Vector3 screenPos = new Vector3(touch.position.x, touch.position.y, Camera.main.WorldToScreenPoint(fd.rb.position).z);
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                        Vector3 targetPos = new Vector3(worldPos.x, fd.rb.position.y, worldPos.z);

                        // Add offset if needed
                        targetPos += new Vector3(fd.offset.x, 0f, fd.offset.z);


                        //  Clamp using BoxCollider bounds
                        Bounds bounds = boardCollider.bounds;

                        //Debug.Log("zpos" + targetPos.z + "bonds min" + bounds.min.z + "bound max" + bounds.max.z);


                        
                        targetPos.z = Mathf.Clamp(targetPos.z, bounds.min.z, bounds.max.z);

                        

                        if (targetPos.z < -7)
                        {
                            targetPos.x = fd.lockXPos;
                            fd.isPositionLocked = true;
                        }
                        else
                        {
                            //Debug.Log("pause the movement and Add force to shoot " + targetPos.z);
                            targetPos.x = Mathf.Clamp(targetPos.x, bounds.min.x, bounds.max.x);
                            fd.lockXPos = targetPos.x;
                            fd.isPositionLocked = false;

                        }

                        fd.rb.MovePosition(Vector3.Lerp(fd.rb.position, targetPos, speed * Time.deltaTime));
                        //fd.rb.MovePosition(targetPos);





                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (activeTouches.TryGetValue(touch.fingerId, out FingerData endFd))
                    {
                        if(endFd.isPositionLocked)
                        {
                            Debug.Log("Add force to shoot");
                            Vector3 dragEnd = GetWorldPoint(touch.position, endFd.rb.position);
                            Vector3 strikeDir = endFd.rb.position - dragEnd; // pull back direction
                            strikeDir.y = 0f;
                            float distance = strikeDir.magnitude;
                            float power = Mathf.Clamp(distance * maxForce, 0f, maxForce);
                            Vector3 force = strikeDir.normalized * power;
                            endFd.rb.AddForce(force, ForceMode.Impulse); // Adjust force as needed
                        }
                        Destroy(endFd.HighLiter);
                        activeTouches.Remove(touch.fingerId);
                    }
                    break;
            }
        }
    }

    // Convert screen point to board world position
    Vector3 GetWorldPoint(Vector3 touch,Vector3 pos)
    {
        Vector3 screenPos = new Vector3(touch.x, touch.y, Camera.main.WorldToScreenPoint(pos).z);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
}
