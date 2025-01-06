using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overflow : MonoBehaviour
{
    public AnimatePlane animatePlane;

    void Start()
    {
        if (animatePlane != null)
        {
            animatePlane.OnOverflow += HandleOverflow;
        }
    }

    void OnDestroy()
    {
        if (animatePlane != null)
        {
            animatePlane.OnOverflow -= HandleOverflow;
        }
    }

    private void HandleOverflow(bool isOverflowing)
    {
        if (isOverflowing)
        {
            Debug.Log("Overflow occurred!");
        }
        else
        {
            Debug.Log("No overflow.");
        }
    }
}