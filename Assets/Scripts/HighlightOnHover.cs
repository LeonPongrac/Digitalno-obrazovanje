using UnityEngine;

public class HighlightOnHover : MonoBehaviour
{
    public Color highlightColor = Color.yellow;   // Color to highlight the object
    public string highlightTag = "Highlightable"; // Tag of objects to be highlighted

    private Color originalColor;
    private Renderer lastRenderer;

    void Update()
    {
        // Perform raycast from camera to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits an object
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the specific tag
            if (hit.transform.CompareTag(highlightTag))
            {
                Renderer renderer = hit.transform.GetComponent<Renderer>();
                
                if (renderer != null)
                {
                    // If it's a new object, reset the last object's color
                    if (lastRenderer != renderer)
                    {
                        ResetLastRendererColor();
                        originalColor = renderer.material.color;
                        renderer.material.color = highlightColor;
                        lastRenderer = renderer;
                    }
                }
            }
            else
            {
                ResetLastRendererColor();
            }
        }
        else
        {
            ResetLastRendererColor();
        }
    }

    void ResetLastRendererColor()
    {
        // Reset the color of the last highlighted object
        if (lastRenderer != null)
        {
            lastRenderer.material.color = originalColor;
            lastRenderer = null;
        }
    }
}
