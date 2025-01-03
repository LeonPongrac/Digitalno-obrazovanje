using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Security.Cryptography;

public class PuzzleConfig : MonoBehaviour
{
    // List to store the wires
    private List<GameObject> allWires = new List<GameObject>();

    private TextMeshProUGUI serialNumber;

    public Material WireRed;
    public Material WireBlack;
    public Material WireBlue;
    public Material WireWhite;
    public Material WireYellow;

    int numberOfWires = 3;
    int logic;

    // Myb need too also automaticly find materials
    void Start()
    {
        // Find Wires
        Transform[] objects = this.GetComponentsInChildren<Transform>();
        foreach (Transform obj in objects)
        {
            if (obj.name.StartsWith("Wire_"))
            {
                allWires.Add(obj.gameObject);
            }
        }

        // Find Serial number
        GameObject serialNumberObject = GameObject.Find("SerialNumber");

        if (serialNumberObject != null)
        {
            // Get the TextMeshProUGUI component on the found GameObject
            serialNumber = serialNumberObject.GetComponent<TextMeshProUGUI>();

            if (serialNumber != null)
            {
                Debug.Log("Found TextMeshProUGUI with name 'SerialNumber'. Text: " + serialNumber.text);
            }
            else
            {
                Debug.LogError("The GameObject 'SerialNumber' does not have a TextMeshProUGUI component!");
            }
        }
        else
        {
            Debug.LogError("No GameObject with the name 'SerialNumber' was found!");
        }

        // Place wires
        numberOfWires = Random.Range(3, 5);//3 or 4, (3,7) -> 3,4,5,6
        PlaceWires(numberOfWires);
    }

    private void PlaceWires(int numberOfWires)
    {
        List<GameObject> wires = new List<GameObject>();
        for (int i = 0; i < numberOfWires; i++)
        {
            allWires[i].SetActive(true);
            setWireId(i, allWires[i]);
            wires.Add(allWires[i]);
        }
        for (int i = numberOfWires; i < allWires.Count; i++)
        {
            allWires[i].SetActive(false);
        }

        // Add materials to wires

        Material[] materials = new Material[] { WireBlack, WireBlue, WireWhite, WireYellow, WireRed, WireRed, WireRed, WireBlue };
        List<Material> assignMaterials = new List<Material>();
        foreach (GameObject wire in wires)
        {
            int randomMaterial = Random.Range(0, materials.Length);
            AssignMaterialToWire(wire, materials[randomMaterial]);
            assignMaterials.Add(materials[randomMaterial]);
        }

        foreach (var mat in assignMaterials)
        {
            Debug.Log(mat.name);
        }

        switch (numberOfWires)
        {
            case 3:
                // Case 1: If there are no red wires, cut the second wire.

                if (!assignMaterials.Contains(WireRed))
                {
                    setWireSolution(true, wires[1]);
                    return;
                }

                // Case 2: Otherwise, if the last wire is white, cut the last wire.

                if (wires[2].GetComponent<Renderer>().material.Equals(WireWhite))
                {
                    setWireSolution(true, wires[2]);
                    return;
                }

                // Case 3: Otherwise, if there is more than one blue wire, cut the last blue wire.

                int blueCount = 0;
                foreach (var material in assignMaterials)
                {
                    if (material.Equals(WireBlue))
                    {
                        blueCount++;
                    }
                }
                if (blueCount > 1)
                {
                    GameObject w = new GameObject();
                    int id = -1;
                    foreach (var wire in wires)
                    {
                        if (wire.GetComponent<Renderer>().material.Equals(WireBlue))
                        {
                            if (wire.GetComponent<WireHandler>().id > id)
                            {
                                w = wire;
                                id = wire.GetComponent<WireHandler>().id;
                            }
                        }
                    }
                    setWireSolution(true, w);
                    return;
                }

                //Otherwise, cut the last wire.

                setWireSolution(true, wires[2]);

                break;

            case 4:

                // Case 1: If there is more than one red wire and the last digit of the serial number is odd, cut the last red wire.

                int redCount = 0;
                foreach (var material in assignMaterials)
                {
                    if (material.Equals(WireRed))
                    {
                        redCount++;
                    }
                }
                if (redCount > 1 && IsLastDigitOdd(serialNumber))
                {
                    GameObject w = new GameObject();
                    int id = -1;
                    foreach (var wire in wires)
                    {
                        if (wire.GetComponent<Renderer>().material.Equals(WireRed))
                        {
                            if (wire.GetComponent<WireHandler>().id > id)
                            {
                                w = wire;
                                id = wire.GetComponent<WireHandler>().id;
                            }
                        }
                    }
                    setWireSolution(true, w);
                    return;
                }

                // Case 2: Otherwise, if the last wire is yellow and there are no red wires, cut the first wire.

                if(wires[3].GetComponent<Renderer>().material.Equals(WireYellow) && !assignMaterials.Contains(WireRed))
                {
                    setWireSolution(true, wires[0]);
                    return;
                }

                // Case 3: Otherwise, if there is exactly one blue wire, cut the first wire.

                if (assignMaterials.Count(i => EqualityComparer<Material>.Default.Equals(i, WireBlue)) == 1)
                {
                    setWireSolution(true, wires[0]);
                    return;
                }

                // Case 4: Otherwise, if there is more than one yellow wire, cut the last wire.

                if (assignMaterials.Count(i => EqualityComparer<Material>.Default.Equals(i, WireYellow)) > 1)
                {
                    setWireSolution(true, wires[3]);
                    return;
                }

                // Case 5: Otherwise, cut the second wire.

                setWireSolution(true, wires[1]);

                break;

        }
    }

    void AssignMaterialToWire(GameObject wire, Material material)
    {
        Renderer wireRenderer = wire.GetComponent<Renderer>();
        if (wireRenderer != null)
        {
            wireRenderer.material = material;
        }
        else
        {
            Debug.LogError("No Renderer found on wire: " + wire.name);
        }
    }
    GameObject PopRandomWire(List<GameObject> wireList)
    {
        int randomIndex = Random.Range(0, wireList.Count);
        GameObject wireToPop = wireList[randomIndex];

        // Remove the wire from the list (this is equivalent to "Pop")
        wireList.RemoveAt(randomIndex);

        return wireToPop;
    }

    void setWireId(int id, GameObject wire)
    {
        WireHandler wireHandler = wire.GetComponent<WireHandler>();
        if (wireHandler != null)
        {
            wireHandler.id = id;
        }
    }

    void setWireSolution(bool wireWin, GameObject wire)
    {
        WireHandler wireHandler = wire.GetComponent<WireHandler>();
        if (wireHandler != null)
        {
            wireHandler.solution = wireWin;
        }
    }

    public void result(bool result)
    {
        if (result)
        {
            Debug.Log("Win");
        }
        else
        {
            Debug.Log("Loss");
        }
    }

    // Function to check if the last digit of the number in the TextMeshProUGUI is odd
    public static bool IsLastDigitOdd(TextMeshProUGUI textComponent)
    {
        if (textComponent == null || string.IsNullOrEmpty(textComponent.text))
        {
            // Return false if the TextMeshProUGUI is null or contains no text
            return false;
        }

        // Try to parse the text into an integer
        if (int.TryParse(textComponent.text, out int number))
        {
            // Get the last digit of the number
            int lastDigit = number % 10;

            // Check if the last digit is odd
            return lastDigit % 2 != 0;
        }

        // Return false if the text is not a valid number
        return false;
    }

}