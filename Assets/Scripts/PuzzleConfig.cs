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
using static UnityEngine.ParticleSystem;
using System.ComponentModel;

public class PuzzleConfig : MonoBehaviour
{
    // List to store the wires
    private List<GameObject> allWires = new List<GameObject>();
    private List<GameObject> wires = new List<GameObject>();

    private TextMeshProUGUI serialNumber;

    private BlinkLight blinkLight;

    private StartGameConfig startGameConfig;

    public Material WireRed;
    public Material WireBlack;
    public Material WireBlue;
    public Material WireWhite;
    public Material WireYellow;

    int numberOfWires = 3;
    int logic;

    public delegate void PuzzleGameEnd();
    public event PuzzleGameEnd OnGameEnd;

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
            if (obj.name.StartsWith("Lamp"))
            {
                blinkLight = obj.gameObject.GetComponent<BlinkLight>();
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

        startGameConfig = GameObject.Find("GameLogic").GetComponent<StartGameConfig>();
    }

    public void StartGame() { 

        // Place wires
        numberOfWires = Random.Range(3, 7);//3 or 4, (3,7) -> 3,4,5,6
        PlaceWires(numberOfWires);
    }

    private void PlaceWires(int numberOfWires)
    {
        for (int i = 0; i < numberOfWires; i++)
        {
            allWires[i].SetActive(true);
            setWireId(i, allWires[i]);
            wires.Add(allWires[i]);
            setCanBePressd(true, allWires[i]);
        }
        for (int i = numberOfWires; i < allWires.Count; i++)
        {
            allWires[i].SetActive(false);
        }

        // Add materials to wires WireBlack, WireBlue, WireWhite, WireYellow, WireRed, WireRed, WireRed, WireBlue, WireBlue, WireBlue

        Material[] materials = new Material[] { WireBlack, WireYellow, WireWhite, WireYellow, WireRed, WireYellow };
        List<Material> assignMaterials = new List<Material>();
        foreach (GameObject wire in wires)
        {
            int randomMaterial = Random.Range(0, materials.Length);
            AssignMaterialToWire(wire, materials[randomMaterial]);
            assignMaterials.Add(materials[randomMaterial]);
        }

        foreach (var mat in assignMaterials)
        {
            //Debug.Log(mat.name);
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
                        if (wire.GetComponent<Renderer>().material.ToString().Contains("blue"))
                        {
                            WireHandler wireHandler = wire.GetComponent<WireHandler>();
                            if (wireHandler != null)
                            {
                                w = wire;
                                id = wireHandler.id;
                            }
                            else
                            {
                                Debug.Log("wireHandler != null");
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

                // Case 1: If there is more than one red wire and the last digit of the serial number is odd, cut the last red wire. works

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
                        if (wire.GetComponent<Renderer>().material.ToString().Contains("red"))
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

                // Case 2: Otherwise, if the last wire is yellow and there are no red wires, cut the first wire. works
                if(wires[3].GetComponent<Renderer>().material.ToString().Contains("yellow") && !assignMaterials.Contains(WireRed))
                {
                    setWireSolution(true, wires[0]);
                    return;
                }

                // Case 3: Otherwise, if there is exactly one blue wire, cut the first wire. works

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

                // Case 5: Otherwise, cut the second wire. works

                setWireSolution(true, wires[1]);

                break;

            case 5:

                //If the last wire is black and the last digit of the serial number is odd, cut the fourth wire.

                if (wires[4].GetComponent<Renderer>().material.ToString().Contains("black") && IsLastDigitOdd(serialNumber))
                {
                    setWireSolution(true, wires[3]);
                    return;
                }

                //Otherwise, if there is exactly one red wire and there is more than one yellow wire, cut the first wire.

                if (assignMaterials.Count(i => EqualityComparer<Material>.Default.Equals(i, WireRed)) == 1 && assignMaterials.Count(i => EqualityComparer<Material>.Default.Equals(i, WireYellow)) > 1)
                {
                    setWireSolution(true, wires[0]);
                    return;
                }

                //Otherwise, if there are no black wires, cut the second wire.

                if (!assignMaterials.Contains(WireBlack))
                {
                    setWireSolution(true, wires[1]);
                    return;
                }

                //Otherwise, cut the first wire

                setWireSolution(true, wires[0]);

                break;

            case 6:

                //If there are no yellow wires and the last digit of the serial number is odd, cut the third wire.

                if (!assignMaterials.Contains(WireYellow) && IsLastDigitOdd(serialNumber))
                {
                    setWireSolution(true, wires[2]);
                    return;
                }

                //Otherwise, if there is exactly one yellow wire and there is more than one white wire, cut the fourth wire.

                if (assignMaterials.Count(i => EqualityComparer<Material>.Default.Equals(i, WireYellow)) == 1 && assignMaterials.Count(i => EqualityComparer<Material>.Default.Equals(i, WireWhite)) > 1)
                {
                    setWireSolution(true, wires[3]);
                    return;
                }

                //Otherwise, if there are no red wires, cut the last wire.

                if (!assignMaterials.Contains(WireRed))
                {
                    setWireSolution(true, wires[5]);
                    return;
                }

                //Otherwise, cut the fourth wire.

                setWireSolution(true, wires[3]);

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

    void setCanBePressd(bool canBePressd ,GameObject wire)
    {
        WireHandler wireHandler = wire.GetComponent<WireHandler>();
        if (wireHandler != null)
        {
            wireHandler.canBePressd = canBePressd;
            if (canBePressd == false)
            {
                wire.tag = "Untagged";
            }
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
            blinkLight.LightLamp(Color.green);
            foreach (var item in wires)
            {
                setCanBePressd(false, item);
            }
            OnGameEnd?.Invoke();
            Debug.Log("Win");
        }
        else
        {
            blinkLight.LightLamp(Color.red);
            startGameConfig.AddStrike();
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
            Debug.Log("Out " + number);
            // Get the last digit of the number
            int lastDigit = number % 10;
            Debug.Log("lastDigit " + lastDigit);
            // Check if the last digit is odd
            return lastDigit % 2 != 0;
        }

        // Return false if the text is not a valid number
        return false;
    }

}