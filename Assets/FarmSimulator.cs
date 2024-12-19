using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FarmSimulator : MonoBehaviour
{

    public GameObject irrigationWaterPlane;
    public GameObject reuseWaterPlane;
    public GameObject effluentWaterPlane;


    // Start is called before the first frame update
    void Start()
    {
        irrigationWaterPlane.GetComponent<AnimatePlane>().setFillPercentage(0.5f);
        reuseWaterPlane.GetComponent<AnimatePlane>().setFillPercentage(0.5f);
        effluentWaterPlane.GetComponent<AnimatePlane>().setFillPercentage(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }


}
