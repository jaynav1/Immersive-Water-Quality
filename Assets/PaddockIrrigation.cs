using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddockIrrigation : MonoBehaviour
{
    public bool filled = false;
    public bool saturated = false;

    private Vector3 initialScale;
    private Vector3 initialPosition;
    public float animationSpeed = 0.1f; // Speed parameter for the animation

    // Start is called before the first frame update
    void Start()
    {
        filled = false;
        saturated = false;

        // Store the initial scale and position of the plane
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;

        // Hide the plane initially
        transform.localScale = new Vector3(0, initialScale.y, initialScale.z);
        transform.localPosition = new Vector3(initialPosition.x - (initialScale.x * 3), initialPosition.y, initialPosition.z);
        Irrigate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Irrigate()
    {
        Debug.Log("Irrigating the paddock...");
        filled = true;
        StartCoroutine(AnimateFill(true));
    }

    public void SaturateWater()
    {
        if (filled)
        {
            Debug.Log("Saturating water with nutrients");
            saturated = true;
        }
        else
        {
            Debug.Log("Cannot saturate water without filling the paddock first");
        }
    }

    public void Drain()
    {
        if (filled)
        {
            Debug.Log("Draining the paddock...");
            filled = false;
            saturated = false;
            StartCoroutine(AnimateFill(false));
        }
        else
        {
            Debug.Log("Cannot drain the paddock without filling it first");
        }
    }

    private IEnumerator AnimateFill(bool isFilling)
    {
        float targetScaleX = isFilling ? initialScale.x : 0;
        Vector3 targetPosition = isFilling ? initialPosition : new Vector3(initialPosition.x - initialScale.x / 2, initialPosition.y, initialPosition.z);

        while (transform.localScale.x != targetScaleX)
        {
            float newScaleX = Mathf.MoveTowards(transform.localScale.x, targetScaleX, animationSpeed * Time.deltaTime);
            transform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

            float newPositionX = Mathf.MoveTowards(transform.localPosition.x, targetPosition.x, animationSpeed * Time.deltaTime);
            transform.localPosition = new Vector3(newPositionX, initialPosition.y, initialPosition.z);

            yield return null; // Wait for the next frame
        }
    }
}
