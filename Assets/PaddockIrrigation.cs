using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddockIrrigation : MonoBehaviour
{
    // Constants for the paddock states
    private enum PaddockState
    {
        Empty,
        Full,
        Saturated,
    }

    // Variable to store the current state of the paddock
    private PaddockState currentState = PaddockState.Empty;
    // Variables for the initial scale and position of the plane
    private Vector3 initialScale;
    private Vector3 initialPosition;

    // Renderer for the paddock
    private Renderer paddockRenderer;

    // Public variable for animation speed
    public float animationSpeed = 0.1f; // Speed parameter for the animation

    // Public variables for the paddock colors
    public Color basePaddockColor;
    public Color saturatedPaddockColor;

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial scale and position of the plane
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;

        // Set the base color of the paddock
        paddockRenderer = GetComponent<Renderer>();
        if (paddockRenderer != null)
        {
            paddockRenderer.material.SetColor("_BaseColor", basePaddockColor);
        }

        // Hide the plane initially
        transform.localScale = new Vector3(0, initialScale.y, initialScale.z);
        // Set the new position of the plane
        transform.localPosition = new Vector3(-55, initialPosition.y, initialPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        // No update logic needed for now
    }

    // Coroutine to animate the filling of the paddock
    public IEnumerator AnimateFill()
    {
        if (currentState != PaddockState.Empty)
        {
            yield break; // Exit the coroutine if the paddock is not empty
        }
        float targetScaleX = initialScale.x; // Target scale for the X axis
        Vector3 targetPosition = initialPosition; // Target position

        float initialScaleX = transform.localScale.x; // Initial scale for the X axis
        float initialPositionX = transform.localPosition.x; // Initial position for the X axis

        // Calculate the duration based on the scale change
        float duration = Mathf.Abs(targetScaleX - initialScaleX) / animationSpeed;

        float elapsedTime = 0f;

        // Animate the scale and position over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Interpolate the scale and position
            float newScaleX = Mathf.Lerp(initialScaleX, targetScaleX, t);
            transform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

            float newPositionX = Mathf.Lerp(initialPositionX, targetPosition.x, t);
            transform.localPosition = new Vector3(newPositionX, initialPosition.y, initialPosition.z);

            yield return null; // Wait for the next frame
        }

        // Ensure the final values are set
        transform.localScale = new Vector3(targetScaleX, initialScale.y, initialScale.z);
        transform.localPosition = new Vector3(targetPosition.x, initialPosition.y, initialPosition.z);

        // Update the current state of the paddock
        currentState = PaddockState.Full;
    }

    // Coroutine to animate the draining of the paddock
    public IEnumerator AnimateDrain()
    {
        if (currentState == PaddockState.Empty)
        {
            yield break; // Exit the coroutine if the paddock is not full
        }
        float targetScaleX = 0; // Target scale for the X axis (drained state)
        Vector3 targetPosition = new Vector3(35, initialPosition.y, initialPosition.z); // Target position for the drained state

        float initialScaleX = transform.localScale.x; // Initial scale for the X axis
        float initialPositionX = transform.localPosition.x; // Initial position for the X axis

        // Calculate the duration based on the scale change
        float duration = Mathf.Abs(targetScaleX - initialScaleX) / animationSpeed;

        float elapsedTime = 0f;

        // Animate the scale and position over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Interpolate the scale and position
            float newScaleX = Mathf.Lerp(initialScaleX, targetScaleX, t);
            transform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

            float newPositionX = Mathf.Lerp(initialPositionX, targetPosition.x, t);
            transform.localPosition = new Vector3(newPositionX, initialPosition.y, initialPosition.z);

            yield return null; // Wait for the next frame
        }

        // Ensure the final values are set
        transform.localScale = new Vector3(targetScaleX, initialScale.y, initialScale.z);
        transform.localPosition = new Vector3(targetPosition.x, initialPosition.y, initialPosition.z);

        // Update the current state of the paddock
        currentState = PaddockState.Empty;
    }

    // Coroutine to animate the color change to saturation
    public IEnumerator AnimateSaturation()
    {
        if (currentState != PaddockState.Full)
        {
            yield break; // Exit the coroutine if the paddock is already saturated or empty
        }
        float duration = 1f / animationSpeed; // Duration for the color change
        float elapsedTime = 0f;

        // Animate the color change over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Interpolate the color
            Color newColor = Color.Lerp(basePaddockColor, saturatedPaddockColor, t);
            paddockRenderer.material.SetColor("_BaseColor", newColor);

            yield return null; // Wait for the next frame
        }

        // Ensure the final color is set
        paddockRenderer.material.SetColor("_BaseColor", saturatedPaddockColor);

        // Update the current state of the paddock
        currentState = PaddockState.Saturated;
    }
}
