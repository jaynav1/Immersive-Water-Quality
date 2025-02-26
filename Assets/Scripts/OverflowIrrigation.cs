using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverflowIrrigation : MonoBehaviour
{

    public float animationSpeed = 1.0f;
    public Color overflowColor;

    private Material material;
    private bool isOverflowing = false;
    private float reuseOffset;
    private bool useOffset = true;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().sharedMaterial;
        material.SetColor("_Color", overflowColor);
        material.SetFloat("_Panner", 0f);
        material.SetFloat("_Drain", 0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Coroutine to animate the overflow
    public IEnumerator StartOverflow()
    {
        if (!isOverflowing)
        {
            Debug.Log("Start Overflow in coroutine");
            isOverflowing = true;
            material.SetFloat("_Drain", 0f);
            yield return AnimatePanner(0f, 2f);
        }
    }


    // Coroutine to stop the overflow
    public IEnumerator StopOverflow()
    {
        if (isOverflowing)
        {
            Debug.Log("Stop Overflow in coroutine");
            material.SetFloat("_Drain", 1f);
            isOverflowing = false;
            yield return AnimatePanner(2f, 0f);
        }
    }

    // Public method to set the reuse offset of the paddock
    public void SetOffset(bool newUseOffset)
    {
        useOffset = newUseOffset;
        material.SetFloat("_ReuseOffset", useOffset ? reuseOffset : 0.0f);
    }

    // Coroutine to animate the panner value
    private IEnumerator AnimatePanner(float initialPanner, float targetPanner)
    {
        float elapsedTime = 0f;
        float duration = Mathf.Abs(targetPanner - initialPanner) / animationSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            material.SetFloat("_Panner", Mathf.Lerp(initialPanner, targetPanner, t));
            yield return null;
        }
    }
}
