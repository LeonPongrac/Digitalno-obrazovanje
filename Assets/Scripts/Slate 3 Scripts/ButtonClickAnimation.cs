using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickAnimation : MonoBehaviour
{
    public float moveDistance = 0.1f; // Udaljenost na osi X za klik
    public float animationTime = 0.1f; // Vrijeme trajanja animacije (u oba smjera)

    private Vector3 originalPosition; // Pohrana originalne pozicije
    private bool isAnimating = false; // Sprjecava visestruke animacije istovremeno

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void OnMouseDown()
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimateButtonClick());
        }
    }
        
    private System.Collections.IEnumerator AnimateButtonClick()
    {
        isAnimating = true;

        // Ciljana pozicija kada se objekt pomakne
        Vector3 targetPosition = originalPosition + new Vector3(moveDistance, 0, 0);

        // Pomicanje prema cilju
        float elapsedTime = 0;
        while (elapsedTime < animationTime)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;

        // Vracanje na originalnu poziciju
        elapsedTime = 0;
        while (elapsedTime < animationTime)
        {
            transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;

        isAnimating = false;
    }
}

