using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkLight : MonoBehaviour
{
    public Light lampLight; // Reference to the Light component
    public Renderer lampRenderer;
    public Color redColor = Color.red;
    public Color greenColor = Color.green;

    private Coroutine currentCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        if (lampLight == null)
        {
            lampLight = GetComponent<Light>();
        }

        if (lampRenderer == null)
        {
            lampRenderer = GetComponent<Renderer>();
        }

        // Ensure the lamp is initially off
        lampLight.enabled = false;
        lampRenderer.material.color = Color.black;
    }

    public void LightLamp(Color color)
    {
        // Stop any ongoing coroutine to ensure the lamp behaves predictably
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(LightLampCoroutine(color));
    }

    private IEnumerator LightLampCoroutine(Color color)
    {
        // Set the lamp color and enable it
        lampLight.color = color;
        lampLight.enabled = true;

        lampRenderer.material.color = color;


        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Turn off the lamp
        lampLight.enabled = false;
        lampRenderer.material.color = Color.black;

        currentCoroutine = null; // Clear the current coroutine reference
    }
}
