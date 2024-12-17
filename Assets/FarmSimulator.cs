using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FarmSimulator : MonoBehaviour
{
    private const float floorHeight = -9.07f;
    private const float channelBedHeight = -15.75f;

    public GameObject irrigationWaterPlane;
    public GameObject reuseWaterPlane;
    public GameObject effluentWaterPlane;

    [Range(0.0f, 1.0f)]
    public float irrigationFillPercentage = 0.0f;

    [Range(0.0f, 1.0f)]
    public float reuseFillPercentage = 0.0f;

    [Range(0.0f, 1.0f)]
    public float effluentFillPercentage = 0.0f;

    public float animationSpeed = 0.1f; // Speed parameter for the animation

    // Start is called before the first frame update
    void Start()
    {
        IrrigationSupply(1);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnValidate()
    {
        UpdateWaterPlanes();
    }

    private void UpdateWaterPlanes()
    {
        Vector3 irrigationPosition = irrigationWaterPlane.transform.localPosition;
        irrigationPosition.y = Mathf.Lerp(channelBedHeight, floorHeight, irrigationFillPercentage);
        irrigationWaterPlane.transform.localPosition = irrigationPosition;

        Vector3 reusePosition = reuseWaterPlane.transform.localPosition;
        reusePosition.y = Mathf.Lerp(channelBedHeight, floorHeight, reuseFillPercentage);
        reuseWaterPlane.transform.localPosition = reusePosition;

        Vector3 effluentPosition = effluentWaterPlane.transform.localPosition;
        effluentPosition.y = Mathf.Lerp(channelBedHeight, floorHeight, effluentFillPercentage);
        effluentWaterPlane.transform.localPosition = effluentPosition;
    }

    // Simulation step functions

    /// <summary>
    /// Fill the irrigation channel with water
    /// </summary>
    /// <param name="percentage"></param>
    private void IrrigationSupply(float percentage)
    {
        StartCoroutine(AnimateFillPercentage(percentage));
    }



    private IEnumerator AnimateFillPercentage(float targetPercentage)
    {
        float startPercentage = irrigationFillPercentage;
        float elapsedTime = 0f;
        float duration = 1f / animationSpeed; // Calculate the duration based on the animation speed

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            irrigationFillPercentage = Mathf.Lerp(startPercentage, targetPercentage, elapsedTime / duration);
            UpdateWaterPlanes();
            yield return null;
        }

        irrigationFillPercentage = targetPercentage;
        UpdateWaterPlanes();
    }
}
