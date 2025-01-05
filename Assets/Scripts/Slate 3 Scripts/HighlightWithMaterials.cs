using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightWithMaterials : MonoBehaviour
{
    [System.Serializable]
    public class HighlightableObject
    {
        public GameObject targetObject; // Objekt koji treba biti highlightan
        public Material highlightMaterial; // Materijal koji ce se koristiti za highlight
    }

    public HighlightableObject[] objectsToHighlight; // Lista objekata i njihovih highlight materijala
    public SimonSays simonSaysScript;

    private Material[] originalMaterials;
    private Renderer[] renderers;
    private GameObject currentlyHighlightedObject = null;

    void Start()
    {
        renderers = new Renderer[objectsToHighlight.Length];
        originalMaterials = new Material[objectsToHighlight.Length];

        for (int i = 0; i < objectsToHighlight.Length; i++)
        {
            if (objectsToHighlight[i].targetObject != null)
            {
                renderers[i] = objectsToHighlight[i].targetObject.GetComponent<Renderer>();
                if (renderers[i] != null)
                {
                    originalMaterials[i] = renderers[i].material;
                }
            }
        }
    }

    void Update()
    {
        // Provjeri je li SimonSays u stanju da dozvoli highlightanje
        // Ovo mora biti false da highlight radi
        if (simonSaysScript != null && !(simonSaysScript.GameRunning && simonSaysScript.SequenceFinished && !simonSaysScript.AnswerGiven))
        {
            ResetHighlight(); // Resetiraj bilo koji trenutni highlight
            return;
        }

        // Raycast iz pozicije miša
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool objectHighlighted = false;

        if (Physics.Raycast(ray, out hit))
        {
            // Provjera je li pogoden neki od specificiranih objekata
            for (int i = 0; i < objectsToHighlight.Length; i++)
            {
                if (hit.transform.gameObject == objectsToHighlight[i].targetObject)
                {
                    HighlightObject(i);
                    objectHighlighted = true;
                    break;
                }
            }
        }

        // Ako nijedan objekt nije pogoden, resetiraj highlight
        if (!objectHighlighted)
        {
            ResetHighlight();
        }
    }

    void HighlightObject(int index)
    {
        if (currentlyHighlightedObject != objectsToHighlight[index].targetObject)
        {
            ResetHighlight();

            // Postavljanje novog highlighta
            if (renderers[index] != null)
            {
                renderers[index].material = objectsToHighlight[index].highlightMaterial;
                currentlyHighlightedObject = objectsToHighlight[index].targetObject;
            }
        }
    }

    void ResetHighlight()
    {
        if (currentlyHighlightedObject != null)
        {
            // Vracanje originalnog materijala
            for (int i = 0; i < objectsToHighlight.Length; i++)
            {
                if (currentlyHighlightedObject == objectsToHighlight[i].targetObject)
                {
                    renderers[i].material = originalMaterials[i];
                    currentlyHighlightedObject = null;
                    break;
                }
            }
        }
    }
}
