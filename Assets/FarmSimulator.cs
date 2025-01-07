using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FarmSimulator : MonoBehaviour
{

    public GameObject irrigationWaterPlane;
    public GameObject reuseWaterPlane;
    public GameObject effluentWaterPlane;
    public GameObject paddock;
    public GameObject overflow;
    

    // Start is called before the first frame update
    void Start()
    {
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
        yield return StartCoroutine(irrigationWaterPlane.GetComponent<AnimatePlane>().movePlane(1));

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlane.GetComponent<AnimatePlane>().movePlane(0));
        yield return StartCoroutine(paddock.GetComponent<PaddockIrrigation>().AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddock.GetComponent<PaddockIrrigation>().AnimateSaturation());


        // 4. fill the reuse water plane while draining the paddock
        StartCoroutine(paddock.GetComponent<PaddockIrrigation>().AnimateDrain());
        yield return StartCoroutine(reuseWaterPlane.GetComponent<AnimatePlane>().movePlane(1));
        overflow.GetComponent<ParticleSystem>().Play();

        // 5. fill the effluent water plane
        yield return StartCoroutine(effluentWaterPlane.GetComponent<AnimatePlane>().movePlane(1));
        
    }




}
