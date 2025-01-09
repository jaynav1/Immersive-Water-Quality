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
        overflowParticles = overflowParticles.GetComponent<ParticleSystem>();
        overflowParticles.Stop();

        // Start the animation
        StartCoroutine(Animate());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator Animate()
    {
        // Use multiple subroutines to animate the farm

        // 1. fill the irrigation water plane
        //yield return StartCoroutine(irrigationWaterPlane.GetComponent<AnimatePlane>().movePlane(1));
        yield return irrigationWaterPlaneScript.movePlane(1000);

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
        yield return StartCoroutine(paddockScript.AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddockScript.AnimateSaturation());


        // 4. fill the reuse water plane while draining the paddock
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




}
