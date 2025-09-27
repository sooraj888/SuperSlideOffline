
using UnityEngine;
using UnityEngine.UI;

public class StrikerOnPress : MonoBehaviour
{
    [SerializeField] private string targetTag = "Striker"; // Set your tag in Inspector


    [SerializeField] private TriggerCounter p1triggerCounter;
    [SerializeField] private TriggerCounter p2triggerCounter;

     public GameObject p1SelectedStriker = null;
     public GameObject p2SelectedStriker = null;

    private Camera cam;

    [SerializeField] private Slider p1slider;
    [SerializeField] private Slider p2slider;

    [SerializeField] private float minX = -3.55f;    // Minimum X position
    [SerializeField] private float maxX = 3.55f;

    public bool IsP1StrikerSelected = false;
    public bool IsP2StrikerSelected = false;

    void Start()
    {
        cam = Camera.main;

        if (p1slider != null)
        {
            // Add listener to update position when slider value changes
            p1slider.onValueChanged.AddListener(P1OnSliderValueChanged);
        }

        if (p2slider != null)
        {
            // Add listener to update position when slider value changes
            p2slider.onValueChanged.AddListener(P2OnSliderValueChanged);
        }
    }

    void Update()
    {
        // Handle mouse click (PC)
        if (Input.GetMouseButtonDown(0))
        {
            CheckObjectAtPosition(Input.mousePosition);
        }

        // Handle all touches (Mobile)
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    CheckObjectAtPosition(touch.position);
                }
            }
        }
    }

    void CheckObjectAtPosition(Vector3 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        // 3D objects
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                GameObject clickedStrikerGO = hit.collider.gameObject;
                //Debug.Log("Hit 3D Object with tag [" + targetTag + "]: " + hit.collider.gameObject.name);

                if (p1triggerCounter.strikerObjects.Contains(clickedStrikerGO))
                {
                    if (p1SelectedStriker == null)
                    {
                        p1SelectedStriker = clickedStrikerGO;
                        IsP1StrikerSelected = true;
                        p1SelectedStriker.transform.position = p1triggerCounter.StrikerHolderTransform.position;
                        p1slider.value = 0.5f;
                    }
                } else if (p2triggerCounter.strikerObjects.Contains(clickedStrikerGO))
                {
                    if (p2SelectedStriker == null)
                    {
                        p2SelectedStriker = clickedStrikerGO;
                        IsP2StrikerSelected = true;
                        p2SelectedStriker.transform.position = p2triggerCounter.StrikerHolderTransform.position;
                        p2slider.value = 0.5f;
                    }
                }
            }
        }
    }


    private void P1OnSliderValueChanged(float value)
    {
        if (p1SelectedStriker != null)
        {

            Vector3 pos = p1SelectedStriker.transform.position;
            pos.x = pos.x = Mathf.Lerp(minX, maxX, value); // Convert slider (0–1) to position
            p1SelectedStriker.transform.position = pos;
        }
        
    }

    private void P2OnSliderValueChanged(float value)
    {
        if (p2SelectedStriker != null)
        {

            Vector3 pos = p2SelectedStriker.transform.position;
            pos.x = pos.x = Mathf.Lerp(minX, maxX, value); // Convert slider (0–1) to position
            p2SelectedStriker.transform.position = pos;
        }

    }


    public void ResetStrikers(GameObject resetGO)
    {
        if (p1SelectedStriker != null && p1SelectedStriker==resetGO)
        {
            p1SelectedStriker = null;
            
        }
        if (p2SelectedStriker != null && p2SelectedStriker == resetGO)
        {
            p2SelectedStriker = null;
            
        }
    }
}
