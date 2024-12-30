using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SerialNumberGenerator : MonoBehaviour
{
    // Reference to the TextMeshPro component
    public TextMeshProUGUI numberText;

    // Start is called before the first frame update
    void Start()
    {
        // Generate a random 3-digit number
        int randomNumber = GenerateRandomNumber();

        // Display the number in the TextMeshPro component
        DisplayNumber(randomNumber);
    }

    // Function to generate a random 3-digit number
    int GenerateRandomNumber()
    {
        return Random.Range(100, 1000); // Random.Range is inclusive of 100 and exclusive of 1000
    }

    // Function to display the number in the TextMeshPro text field
    void DisplayNumber(int number)
    {
        if (numberText != null)
        {
            numberText.text = number.ToString(); // Set the text to the generated number
        }
        else
        {
            Debug.LogError("Number TextMeshPro component is not assigned!");
        }
    }
}
