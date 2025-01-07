using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddockIrrigation : MonoBehaviour
{
    public bool filled = false;
    public bool saturated = false;

    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Renderer paddockRenderer;
    public float animationSpeed = 0.1f; // Speed parameter for the animation
    public Color basePaddockColor;
    public Color saturatedPaddockColor;

    // Start is called before the first frame update
    void Start()
    {
        filled = false;
        saturated = false;

        // Store the initial scale and position of the plane
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;

        paddockRenderer = GetComponent<Renderer>();
        if (paddockRenderer != null)
        {
            paddockRenderer.material.SetColor("_BaseColor", basePaddockColor);
        }

        // Hide the plane initially
        transform.localScale = new Vector3(0, initialScale.y, initialScale.z);
        // Hardcode the position because idk
        transform.localPosition = new Vector3(-55, initialPosition.y, initialPosition.z);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator AnimateFill()
    {
        float targetScaleX = initialScale.x;
        Vector3 targetPosition = initialPosition;

        float initialScaleX = transform.localScale.x;
        float initialPositionX = transform.localPosition.x;

        float duration = Mathf.Abs(targetScaleX - initialScaleX) / animationSpeed; // Calculate the duration based on the scale change

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            float newScaleX = Mathf.Lerp(initialScaleX, targetScaleX, t);
            transform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

            float newPositionX = Mathf.Lerp(initialPositionX, targetPosition.x, t);
            transform.localPosition = new Vector3(newPositionX, initialPosition.y, initialPosition.z);

            yield return null; // Wait for the next frame
        }

        // Ensure the final values are set
        transform.localScale = new Vector3(targetScaleX, initialScale.y, initialScale.z);
        transform.localPosition = new Vector3(targetPosition.x, initialPosition.y, initialPosition.z);
    }

    public IEnumerator AnimateDrain()
    {
        float targetScaleX = 0;
        Vector3 targetPosition = new Vector3(35, initialPosition.y, initialPosition.z);

        float initialScaleX = transform.localScale.x;
        float initialPositionX = transform.localPosition.x;

        float duration = Mathf.Abs(targetScaleX - initialScaleX) / animationSpeed; // Calculate the duration based on the scale change

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            float newScaleX = Mathf.Lerp(initialScaleX, targetScaleX, t);
            transform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

            float newPositionX = Mathf.Lerp(initialPositionX, targetPosition.x, t);
            transform.localPosition = new Vector3(newPositionX, initialPosition.y, initialPosition.z);

            yield return null; // Wait for the next frame
        }

        // Ensure the final values are set
        transform.localScale = new Vector3(targetScaleX, initialScale.y, initialScale.z);
        transform.localPosition = new Vector3(targetPosition.x, initialPosition.y, initialPosition.z);
    }

    public IEnumerator AnimateSaturation()
    {
        float duration = 1f / animationSpeed; // Duration for the color change
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            Color newColor = Color.Lerp(basePaddockColor, saturatedPaddockColor, t);
            paddockRenderer.material.SetColor("_BaseColor", newColor);

            yield return null; // Wait for the next frame
        }

        // Ensure the final color is set
        paddockRenderer.material.SetColor("_BaseColor", saturatedPaddockColor);
    }
}
