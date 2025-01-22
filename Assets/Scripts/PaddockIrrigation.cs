using System.Collections;
using UnityEngine;

public class PaddockIrrigation : MonoBehaviour
{
    // Enum to represent the state of the paddock
    private enum PaddockState
    {
        Empty,
        Full,
        Saturated,
    }

    private PaddockState currentState = PaddockState.Empty;
    private Material paddockMaterial;

    public float animationSpeed = 0.1f;
    public Color basePaddockColor;
    public Color saturatedPaddockColor;

    void Start()
    {
        // Initialize the paddock material and set initial properties
        paddockMaterial = GetComponent<Renderer>().material;
        paddockMaterial.SetFloat("_Panner", -3.0f);
        paddockMaterial.SetColor("_Color", basePaddockColor);
    }

    // Coroutine to animate the filling of the paddock
    public IEnumerator AnimateFill()
    {
        yield return AnimatePanner(-3.0f, 0.0f);
        currentState = PaddockState.Full;
    }

    // Coroutine to animate the draining of the paddock
    public IEnumerator AnimateDrain()
    {
        yield return AnimatePanner(0.0f, 3.0f);
        currentState = PaddockState.Empty;
    }

    // Coroutine to animate the saturation of the paddock
    public IEnumerator AnimateSaturation()
    {
        if (currentState != PaddockState.Full) yield break;
        yield return AnimateColor(basePaddockColor, saturatedPaddockColor);
        currentState = PaddockState.Saturated;
    }

    // Private method to animate the panner property of the material
    private IEnumerator AnimatePanner(float initialPanner, float targetPanner)
    {
        float elapsedTime = 0f;
        float duration = Mathf.Abs(targetPanner - initialPanner) / animationSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            paddockMaterial.SetFloat("_Panner", Mathf.Lerp(initialPanner, targetPanner, t));
            yield return null;
        }
    }

    // Private method to animate the color property of the material
    private IEnumerator AnimateColor(Color initialColor, Color targetColor)
    {
        float elapsedTime = 0f;
        float duration = 1f / animationSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            paddockMaterial.SetColor("_Color", Color.Lerp(initialColor, targetColor, t));
            yield return null;
        }
    }
}
