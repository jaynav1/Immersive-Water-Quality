using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FarmSimulator : MonoBehaviour
{
    // Public variables for the game objects
    public GameObject irrigationWaterPlane;
    public GameObject reuseWaterPlane;
    public GameObject effluentWaterPlane;
    public GameObject paddock;
    public GameObject overflow;

    // Animation variables
    private AnimatePlane irrigationWaterPlaneScript;
    private AnimatePlane reuseWaterPlaneScript;
    private AnimatePlane effluentWaterPlaneScript;
    private PaddockIrrigation paddockScript;
    private ParticleSystem overflowParticles;

    // Start is called before the first frame update
    void Start()
    {
        // Get the scripts from the game objects
        irrigationWaterPlaneScript = irrigationWaterPlane.GetComponent<AnimatePlane>();
        reuseWaterPlaneScript = reuseWaterPlane.GetComponent<AnimatePlane>();
        effluentWaterPlaneScript = effluentWaterPlane.GetComponent<AnimatePlane>();
        paddockScript = paddock.GetComponent<PaddockIrrigation>();
        overflowParticles = overflow.GetComponent<ParticleSystem>();
        overflowParticles.Stop();

        // Start the animation for the desired scenario
        StartCoroutine(AnimateNormalWeather());
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Scenario 1: Normal weather conditions with optimum storage condition
    private IEnumerator AnimateNormalWeather()
    {
        // 1. fill the irrigation water plane
        yield return irrigationWaterPlaneScript.movePlane(1500);

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
        yield return StartCoroutine(paddockScript.AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddockScript.AnimateSaturation());

        // 4. fill the reuse water plane while draining the paddock and no overflow due to sufficient storage
        StartCoroutine(paddockScript.AnimateDrain());
        yield return StartCoroutine(reuseWaterPlaneScript.movePlane(1000));

        // 5. fill the effluent water plane and no overflow due to sufficient storage
        yield return StartCoroutine(effluentWaterPlaneScript.movePlane(500));

        // 6. Pump water back to irrigation (effluent + pump rate of reuse)
        StartCoroutine(effluentWaterPlaneScript.movePlane(0));
        StartCoroutine(reuseWaterPlaneScript.movePlane(1000 - 500));
        yield return StartCoroutine(irrigationWaterPlaneScript.movePlane(500 + 100));
    }

    // Scenario 2: Heavy rainfall with overflow
    private IEnumerator AnimateHeavyRainfall()
    {
        // 1. fill the irrigation water plane
        yield return irrigationWaterPlaneScript.movePlane(1800);

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
        yield return StartCoroutine(paddockScript.AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddockScript.AnimateSaturation());

        // 4. fill the reuse water plane while draining the paddock and overflow due to heavy rainfall
        StartCoroutine(paddockScript.AnimateDrain());
        yield return StartCoroutine(reuseWaterPlaneScript.movePlane(1000));
        overflowParticles.Play();

        // 5. fill the effluent water plane 
        yield return StartCoroutine(effluentWaterPlaneScript.movePlane(500));

        // 6. Pump water back to irrigation (effluent + pump rate of reuse)
        StartCoroutine(effluentWaterPlaneScript.movePlane(0));
        StartCoroutine(reuseWaterPlaneScript.movePlane(1000 - 500));
        overflowParticles.Stop();
        yield return StartCoroutine(irrigationWaterPlaneScript.movePlane(500 + 100));
    }

    // Scenario 3: Drought conditions with limited water
    private IEnumerator AnimateDroughtConditions()
    {
        // 1. fill the irrigation water plane with limited water
        yield return irrigationWaterPlaneScript.movePlane(500);

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
        yield return StartCoroutine(paddockScript.AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddockScript.AnimateSaturation());

        // 4. fill the reuse water plane while draining the paddock with limited water
        StartCoroutine(paddockScript.AnimateDrain());
        yield return StartCoroutine(reuseWaterPlaneScript.movePlane(500));

        // 5. fill the effluent water plane with limited water
        yield return StartCoroutine(effluentWaterPlaneScript.movePlane(250));

        // 6. Pump water back to irrigation (effluent + pump rate of reuse)
        StartCoroutine(effluentWaterPlaneScript.movePlane(0));
        StartCoroutine(reuseWaterPlaneScript.movePlane(500 - 250));
        yield return StartCoroutine(irrigationWaterPlaneScript.movePlane(250 + 50));
    }
}
