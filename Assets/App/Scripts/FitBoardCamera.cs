using UnityEngine;

[ExecuteAlways]
public class FitBoardCamera : MonoBehaviour
{
    [SerializeField] private Transform board; // Reference to your board
    [SerializeField] private float padding = 0.1f; // Extra space around edges
    [SerializeField] private float perspectiveFOV = 60f; // Default FOV for perspective

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        FitToBoard();
    }

    void OnEnable()
    {
        ScreenResizeEvent.OnScreenResize += FitToBoard;
    }

    void OnDisable()
    {
        ScreenResizeEvent.OnScreenResize -= FitToBoard;
    }

    public void FitToBoard()
    {
        if (board == null || cam == null) return;

        Renderer rend = board.GetComponent<Renderer>();
        if (rend == null) return;

        Bounds bounds = rend.bounds;

        float boardWidth = bounds.size.x + padding;
        float boardHeight = bounds.size.z + padding;

        float aspect = cam.aspect;

        if (cam.orthographic)
        {
            // ORTHOGRAPHIC MODE
            float sizeByHeight = boardHeight / 2f;
            float sizeByWidth = (boardWidth / 2f) / aspect;
            cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);

            Vector3 camPos = bounds.center;
            camPos.y = 10; // top-down view
            camPos.z = transform.position.z;
            transform.position = camPos;
        }
        else
        {
            // PERSPECTIVE MODE
            cam.fieldOfView = perspectiveFOV;

            float halfFOV = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;

            // Distance needed to fit height
            float distanceHeight = (boardHeight * 0.5f) / Mathf.Tan(halfFOV);
            // Distance needed to fit width
            float distanceWidth = (boardWidth * 0.5f) / (Mathf.Tan(halfFOV) * aspect);

            float requiredDistance = Mathf.Max(distanceHeight, distanceWidth);

            Vector3 camPos = bounds.center;
            camPos.y = requiredDistance; // put camera above board
            camPos.z = transform.position.z;
            transform.position = camPos;

            cam.transform.LookAt(bounds.center);
        }
    }
}
