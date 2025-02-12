using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverflowIrrigation : MonoBehaviour
{

    public float animationSpeed = 1.0f;
    public Color overflowColor;

    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.SetColor("_Color", overflowColor);
        material.SetFloat("_Panner", 0f);
        material.SetFloat("_Drain", 0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartOverflow()
    {
        material.SetFloat("_Drain", 0f);
        yield return AnimatePanner(0f, 2f);
    }

    public IEnumerator StopOverflow()
    {
        material.SetFloat("_Drain", 1f);
        yield return AnimatePanner(2f, 0f);
    }

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
