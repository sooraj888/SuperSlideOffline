using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;   // Required for TextMeshPro

public class TriggerCounter : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI countText;

    // Keep track of Striker objects inside
    public HashSet<GameObject> strikerObjects = new HashSet<GameObject>();

    [SerializeField]
    private TriggerCounter otherTriggerCounter;

    [SerializeField] private TextMeshProUGUI resultText;

    public Transform StrikerHolderTransform;

    private int totalStrikers = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Striker"))
        {
            if (!otherTriggerCounter.strikerObjects.Contains(other.gameObject))
            {
                strikerObjects.Add(other.gameObject);
                UpdateCountUI();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Striker"))
        {
            strikerObjects.Remove(other.gameObject);
            otherTriggerCounter.strikerObjects.Add(other.gameObject);
            otherTriggerCounter.UpdateCountUI();
            UpdateCountUI();
        }
    }


    // Update the UI text
    public void UpdateCountUI()
    {
        if (countText != null)
        {
            countText.text = strikerObjects.Count.ToString();
            Vector2 percentage = getPercentage(strikerObjects.Count,otherTriggerCounter.strikerObjects.Count);
            //Debug.Log("Percentage: "+ gameObject.name + percentage);

            // Apply color & text
            countText.color = GetColorOnPercentage((int)percentage.x);
            otherTriggerCounter.countText.color = GetColorOnPercentage((int)percentage.y);

            checkWin(strikerObjects.Count, otherTriggerCounter.strikerObjects.Count);
        }
    }

    Color GetColorOnPercentage(int percentage)
    {
        Color orange = new Color(1f, 0.65f, 0f); // RGB for orange (R,G,B from 0 to 1)

        return percentage switch
        {
            >= 80 => Color.red,
            >= 60 => orange,
            >= 40 => Color.yellow,
            _ => Color.green
        };
    }

    private Vector2 getPercentage(int a, int b)
    {
        float total = a + b;

        if (total == 0)
        {
            return Vector2.zero;
        }

        float percentA = (a / total) * 100f;
        float percentB = (b / total) * 100f;
        return new Vector2(percentA,percentB);
    }


    void checkWin(int p1, int p2)
    {
        if ((p1 == 0 || p2 == 0) && (p1+p2 == totalStrikers))
        {            

            Debug.Log("Game Over"+p1+"p2"+p2);
            if (p1 == 0)
            {
                resultText.text = "You Won!";
                resultText.color = Color.green;
                otherTriggerCounter.resultText.text = "You Lost!";
                otherTriggerCounter.resultText.color = Color.red;

            }
            else if (p2 == 0)
            {
                resultText.text = "You Lost!";
                resultText.color = Color.red;
                otherTriggerCounter.resultText.text = "You Won!";
                otherTriggerCounter.resultText.color = Color.green;

            }
            otherTriggerCounter.resultText.gameObject.SetActive(true);
            resultText.gameObject.SetActive(true);
        }
    }
}
