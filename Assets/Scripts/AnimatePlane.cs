using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlane : MonoBehaviour
{
    // Constants for the heights of the floor and channel bed
    private const float floorHeight = -9.07f;
    private const float channelBedHeight = -15.75f;

    // Current volume of the plane
    private float currentVolume = 0.0f;

    // Public variable to set the fill volume in the inspector
    public float fillVolume = 0.0f;

    // Speed parameter for the animation
    public float animationSpeed = 0.1f;

    // Overflow particle system if applicable
    public ParticleSystem overflowParticles;
    public GameObject overflowPlane;

    // Overflow irrigation script
    private OverflowIrrigation overflowIrrigation;

    // Start is called before the first frame update
    void Start()
    {
        // Exception if the volume is not positive
        if (fillVolume <= 0)
        {
            throw new ArgumentException("Fill volume must be positive");
        }
        // Set the initial position of the plane based on the fill volume
        Vector3 newLocation = transform.localPosition;
        newLocation.y = Mathf.Lerp(channelBedHeight, floorHeight, 0);
        transform.localPosition = newLocation;

        // Get the OverflowIrrigation script from the overflow plane
        if (overflowPlane != null)
        {
            overflowIrrigation = overflowPlane.GetComponent<OverflowIrrigation>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // No update logic needed for now
    }

    // Coroutine to animate the plane to a new fill volume
    public IEnumerator movePlane(float newVolume)
    {
        // turn overflow off
        if (overflowParticles != null)
        {
            overflowParticles.Stop();
        }

        // Get the current position of the plane
        Vector3 newLocation = transform.localPosition;
        float currentHeight = newLocation.y;

        // Calculate the target height using SmoothStep for a smoother transition
        float targetHeight = Mathf.SmoothStep(channelBedHeight, floorHeight, getPercentage(newVolume));

        // Move the plane towards the target height
        while (Mathf.Abs(currentHeight - targetHeight) > 0.01f) // Use a small threshold for comparison
        {
            // Move the current height towards the target height based on the animation speed
            currentHeight = Mathf.MoveTowards(currentHeight, targetHeight, animationSpeed * Time.deltaTime);
            newLocation.y = currentHeight;
            transform.localPosition = newLocation;
            yield return null; // Wait for the next frame
        }

        // Ensure the final position is set
        newLocation.y = targetHeight;
        transform.localPosition = newLocation;
        currentVolume = newVolume;

        // If fillVolume is maximized, overflow
        if (newVolume == fillVolume)
        {
            if (overflowParticles != null)
            {
                overflowParticles.Play();
            }

            if (overflowIrrigation != null)
            {
                StartCoroutine(overflowIrrigation.StartOverflow());
            }
        }
    }

    // Helper method to calculate the percentage of the fill volume
    private float getPercentage(float newVolume)
    {
        if (fillVolume == 0)
        {
            return 0;
        }
        return Mathf.Clamp(newVolume / fillVolume, 0f, 1f);
    }

    // New method to change volume by some amount
    public IEnumerator ChangeVolumeByAmount(float amount)
    {
        currentVolume = Mathf.Clamp(currentVolume + amount, 0, fillVolume);
        yield return movePlane(currentVolume);
    }
}
