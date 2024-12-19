using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlane : MonoBehaviour
{
    // Constants for the heights of the floor and channel bed
    private const float floorHeight = -9.07f;
    private const float channelBedHeight = -15.75f;

    // Public variable to set the fill percentage in the inspector
    [Range(0.0f, 1.0f)]
    public float fillPercentage = 0.0f;

    // Speed parameter for the animation
    public float animationSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial position of the plane based on the fill percentage
        Vector3 newLocation = transform.localPosition;
        newLocation.y = Mathf.Lerp(channelBedHeight, floorHeight, fillPercentage);
        transform.localPosition = newLocation;
    }

    // Update is called once per frame
    void Update()
    {
        // No update logic needed for now
    }

    // Coroutine to animate the plane to a new fill percentage
    private IEnumerator movePlane(float newPercentage)
    {
        // Get the current position of the plane
        Vector3 newLocation = transform.localPosition;
        float currentHeight = newLocation.y;

        // Calculate the target height using SmoothStep for a smoother transition
        float targetHeight = Mathf.SmoothStep(channelBedHeight, floorHeight, newPercentage);

        // Move the plane towards the target height
        while (currentHeight != targetHeight)
        {
            // Move the current height towards the target height based on the animation speed
            currentHeight = Mathf.MoveTowards(currentHeight, targetHeight, animationSpeed * Time.deltaTime);
            newLocation.y = currentHeight;
            transform.localPosition = newLocation;
            yield return null; // Wait for the next frame
        }
    }

    // Public method to set the fill percentage and start the animation
    public void setFillPercentage(float newPercentage)
    {
        fillPercentage = newPercentage; // Update the fill percentage
        StartCoroutine(movePlane(newPercentage)); // Start the animation coroutine
    }
}
