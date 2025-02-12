using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class FarmSimulator : MonoBehaviour
{

    // Public variables for the game objects
    public GameObject irrigationWaterPlane;
    public GameObject reuseWaterPlane;
    public GameObject effluentWaterPlane;
    public GameObject paddock;
    public GameObject overflow;
    public GameObject reusePipe;
    public GameObject shedPipe;
    public GameObject effluentPipe;

    // Pipe materials
    private Material reusePipeMaterial;
    private Material shedPipeMaterial;
    private Material effluentPipeMaterial;

    // Animation variables
    private AnimatePlane irrigationWaterPlaneScript;
    private AnimatePlane reuseWaterPlaneScript;
    private AnimatePlane effluentWaterPlaneScript;
    private PaddockIrrigation paddockScript;
    private ParticleSystem overflowParticles;

    private MaterialAnimator reusePipeAnimator;
    private MaterialAnimator shedPipeAnimator;
    private MaterialAnimator effluentPipeAnimator;

    private CowFactory cowFactory;
    
    private int currentScenario = 0;

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

        //Get materials from pipes and create instances
        reusePipeMaterial = reusePipe.GetComponent<Renderer>().material;
        shedPipeMaterial = shedPipe.GetComponent<Renderer>().material;
        effluentPipeMaterial = effluentPipe.GetComponent<Renderer>().material;

        // Initialize material animators
        reusePipeAnimator = new MaterialAnimator(reusePipeMaterial, "_Panner");
        shedPipeAnimator = new MaterialAnimator(shedPipeMaterial, "_Panner");
        effluentPipeAnimator = new MaterialAnimator(effluentPipeMaterial, "_Panner");
        
        //Cow factory
        cowFactory = GetComponent<CowFactory>();

        if (cowFactory != null)
        {
            cowFactory.SpawnCows();
        }
        
        // Start the animation for the desired scenario
        StartCoroutine(ChooseScenario(currentScenario));
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Scenario 1: Normal weather conditions with optimum storage condition
    private IEnumerator AnimateNormalWeather()
    {
        // Use multiple subroutines to animate the farm
        while (true)
        {
            
            // 1. fill the irrigation water plane while stopping reuse
            yield return irrigationWaterPlaneScript.ChangeVolumeByAmount(500);
            reusePipeAnimator.StopAnimation(this);
            effluentPipeAnimator.StopAnimation(this);

            // 2. fill the paddock while draining the irrigation water plane
            StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
            yield return StartCoroutine(paddockScript.AnimateFill());

            yield return cowFactory.MoveToPlane(paddock);

            // 3. saturate the paddock
            yield return StartCoroutine(paddockScript.AnimateSaturation());

            yield return cowFactory.ReturnCows();

            // 4. fill the reuse water plane while draining the paddock and no overflow due to sufficient storage
            StartCoroutine(paddockScript.AnimateDrain());
            yield return StartCoroutine(reuseWaterPlaneScript.ChangeVolumeByAmount(1700));

            // 5. fill the effluent water plane
            shedPipeAnimator.StartAnimation(this);
            yield return StartCoroutine(effluentWaterPlaneScript.ChangeVolumeByAmount(500));
            shedPipeAnimator.StopAnimation(this);

            // 6. Pump water back to irrigation (effluent + pump rate of reuse)

            StartCoroutine(effluentWaterPlaneScript.movePlane(0));
            StartCoroutine(reuseWaterPlaneScript.ChangeVolumeByAmount(-1500));
            reusePipeAnimator.StartAnimation(this);
            effluentPipeAnimator.StartAnimation(this);
            //overflowParticles.Stop();
            yield return StartCoroutine(irrigationWaterPlaneScript.ChangeVolumeByAmount(1000));
        }
        
    }

    // Scenario 2: Heavy rainfall with overflow
    private IEnumerator AnimateHeavyRainfall()
    {
        // 1. fill the irrigation water plane
        //reusePipeAnimator.StartAnimation(this);
        yield return irrigationWaterPlaneScript.movePlane(1900);
        //reusePipeAnimator.StopAnimation(this);

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
        shedPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(effluentWaterPlaneScript.movePlane(500));
        shedPipeAnimator.StopAnimation(this);

        // 6. Pump water back to irrigation (effluent + pump rate of reuse)
        StartCoroutine(effluentWaterPlaneScript.movePlane(0));
        StartCoroutine(reuseWaterPlaneScript.movePlane(1000 - 500));
        reusePipeAnimator.StartAnimation(this);
        effluentPipeAnimator.StartAnimation(this);
        overflowParticles.Stop();
        yield return StartCoroutine(irrigationWaterPlaneScript.movePlane(500 + 100));
        reusePipeAnimator.StopAnimation(this);
        effluentPipeAnimator.StopAnimation(this);
    }

    // Scenario 3: Drought conditions with limited water
    private IEnumerator AnimateDroughtConditions()
    {
        // 1. fill the irrigation water plane with limited water
        //reusePipeAnimator.StartAnimation(this);
        yield return irrigationWaterPlaneScript.movePlane(500);
        //reusePipeAnimator.StopAnimation(this);

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
        yield return StartCoroutine(paddockScript.AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddockScript.AnimateSaturation());

        // 4. fill the reuse water plane while draining the paddock with limited water
        StartCoroutine(paddockScript.AnimateDrain());
        yield return StartCoroutine(reuseWaterPlaneScript.movePlane(500));

        // 5. fill the effluent water plane with limited water
        shedPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(effluentWaterPlaneScript.movePlane(250));
        shedPipeAnimator.StopAnimation(this);

        // 6. Pump water back to irrigation (effluent + pump rate of reuse)
        StartCoroutine(effluentWaterPlaneScript.movePlane(0));
        StartCoroutine(reuseWaterPlaneScript.movePlane(500 - 250));
        reusePipeAnimator.StartAnimation(this);
        effluentPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(irrigationWaterPlaneScript.movePlane(250 + 50));
        reusePipeAnimator.StopAnimation(this);
        effluentPipeAnimator.StopAnimation(this);
    }

    // Scenarios for different storage volumes 
    // Least storage volume with normal weather condition
    private IEnumerator AnimateLeastStorageConditions()
    {
        // 1. fill the irrigation water plane as per the weather condition for which the storage volume is being visualised for 
        //reusePipeAnimator.StartAnimation(this);
        yield return irrigationWaterPlaneScript.movePlane(1700);
        //reusePipeAnimator.StopAnimation(this);

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
        yield return StartCoroutine(paddockScript.AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddockScript.AnimateSaturation());

        // 4. fill the reuse water plane while draining the paddock which leads to overflow due to less storage 
        StartCoroutine(paddockScript.AnimateDrain());
        yield return StartCoroutine(reuseWaterPlaneScript.movePlane(1200));
        overflowParticles.Play();

        // 5. fill the effluent water plane 
        shedPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(effluentWaterPlaneScript.movePlane(500));
        shedPipeAnimator.StopAnimation(this);

        // 6. Pump some water back to irrigation (effluent + pump rate of reuse) and also indicate some overflow
        StartCoroutine(effluentWaterPlaneScript.movePlane(0));
        StartCoroutine(reuseWaterPlaneScript.movePlane(1200 - 150));
        reusePipeAnimator.StartAnimation(this);
        effluentPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(irrigationWaterPlaneScript.movePlane(1700 + 150));
        reusePipeAnimator.StopAnimation(this);
        effluentPipeAnimator.StopAnimation(this);
    }

    // Average storage volume with normal weather condition
    private IEnumerator AnimateLeastStorageCondition()
    {
        // 1. fill the irrigation water plane as per the weather condition for which the storage volume is being visualised for
        //reusePipeAnimator.StartAnimation(this);
        yield return irrigationWaterPlaneScript.movePlane(1700);
        //reusePipeAnimator.StopAnimation(this);

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
        yield return StartCoroutine(paddockScript.AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddockScript.AnimateSaturation());

        // 4. fill the reuse water plane while draining the paddock  
        StartCoroutine(paddockScript.AnimateDrain());
        yield return StartCoroutine(reuseWaterPlaneScript.movePlane(900));

        // 5. fill the effluent water plane 
        shedPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(effluentWaterPlaneScript.movePlane(500));
        shedPipeAnimator.StopAnimation(this);

        // 6. Pump some water back to irrigation (effluent + pump rate of reuse) and also indicate some overflow
        StartCoroutine(effluentWaterPlaneScript.movePlane(0));
        StartCoroutine(reuseWaterPlaneScript.movePlane(900 - 500));
        reusePipeAnimator.StartAnimation(this);
        effluentPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(irrigationWaterPlaneScript.movePlane(500 + 100));
        reusePipeAnimator.StopAnimation(this);
        effluentPipeAnimator.StopAnimation(this);
    }

    // Maximum storage volume
    private IEnumerator AnimateLeastStorageCondition_2()
    {
        // 1. fill the irrigation water plane as per the weather condition for which the storage volume is being visualised for 
        //reusePipeAnimator.StartAnimation(this);
        yield return irrigationWaterPlaneScript.movePlane(1700);
        //reusePipeAnimator.StopAnimation(this);

        // 2. fill the paddock while draining the irrigation water plane
        StartCoroutine(irrigationWaterPlaneScript.movePlane(0));
        yield return StartCoroutine(paddockScript.AnimateFill());

        // 3. saturate the paddock
        yield return StartCoroutine(paddockScript.AnimateSaturation());

        // 4. fill the reuse water plane while draining the paddock which leads to overflow due to less storage 
        StartCoroutine(paddockScript.AnimateDrain());
        yield return StartCoroutine(reuseWaterPlaneScript.movePlane(800));

        // 5. fill the effluent water plane 
        shedPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(effluentWaterPlaneScript.movePlane(400));
        shedPipeAnimator.StopAnimation(this);

        // 6. Pump some water back to irrigation (effluent + pump rate of reuse) and also indicate some overflow
        StartCoroutine(effluentWaterPlaneScript.movePlane(0));
        StartCoroutine(reuseWaterPlaneScript.movePlane(800 - 150));
        reusePipeAnimator.StartAnimation(this);
        effluentPipeAnimator.StartAnimation(this);
        yield return StartCoroutine(irrigationWaterPlaneScript.movePlane(1700 + 150));
        reusePipeAnimator.StopAnimation(this);
        effluentPipeAnimator.StopAnimation(this);
    }

    public void Reset()
    {
        cowFactory.DestroyCows();

        irrigationWaterPlaneScript.Reset();
        reuseWaterPlaneScript.Reset();
        effluentWaterPlaneScript.Reset();
        paddockScript.Reset();
        overflowParticles.Stop();

        reusePipeAnimator.StopAnimation(this);
        shedPipeAnimator.StopAnimation(this);
        effluentPipeAnimator.StopAnimation(this);

        StopAllCoroutines();

        //get any renderer
        Renderer renderer = paddock.GetComponent<Renderer>();

        //If renderer is enabled
        if (renderer.enabled)
        {
            cowFactory.SpawnCows();
        }

        StartCoroutine(ChooseScenario(currentScenario));
    }

    public void SetScenario(int newScenario)
    {
        currentScenario = newScenario;
    }

    private IEnumerator ChooseScenario(int value)
    {
        if (value == 0)
        {
            return AnimateNormalWeather();
        }
        else if (value == 1)
        {
            return AnimateHeavyRainfall();
        }
        else if (value == 2)
        {
            return AnimateDroughtConditions();
        }
        else if (value == 3)
        {
            return AnimateLeastStorageConditions();
        }
        else
        {
            return AnimateNormalWeather();
        }
    } 
        

    private IEnumerator TestMaterialAnimation()
    {
        reusePipeAnimator.StartAnimation(this);
        shedPipeAnimator.StartAnimation(this);
        effluentPipeAnimator.StartAnimation(this);
        yield return new WaitForSeconds(5f);
        reusePipeAnimator.StopAnimation(this);
        shedPipeAnimator.StopAnimation(this);
        effluentPipeAnimator.StopAnimation(this);
    }

    // Method to get a random point on the plane
    public Vector3 GetRandomPointOnPlane(GameObject plane)
    {
        // Get the plane's mesh renderer bounds
        Bounds planeBounds = plane.GetComponent<MeshRenderer>().bounds;
        
        // Generate random point within bounds
        float randomX = Random.Range(planeBounds.min.x, planeBounds.max.x);
        float randomZ = Random.Range(planeBounds.min.z, planeBounds.max.z);
        
        // Use the plane's Y position
        float planeY = plane.transform.position.y;
        
        return new Vector3(randomX, planeY, randomZ);
    }
}

public class MaterialAnimator
{
    private Material material;
    private string propertyName;
    private Coroutine animationCoroutine;

    public MaterialAnimator(Material material, string propertyName)
    {
        this.material = material;
        this.propertyName = propertyName;
        ResetMaterial();
    }

    public void StartAnimation(MonoBehaviour owner)
    {
        if (animationCoroutine != null)
        {
            owner.StopCoroutine(animationCoroutine);
        }
        animationCoroutine = owner.StartCoroutine(AnimateMaterial());
    }

    public void StopAnimation(MonoBehaviour owner)
    {
        if (animationCoroutine != null)
        {
            owner.StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        ResetMaterial();
    }

    private IEnumerator AnimateMaterial()
    {
        float value = -0.5f;
        while (true)
        {
            value += Time.deltaTime;
            if (value > 1.5f)
            {
                value = -0.5f;
            }
            material.SetFloat(propertyName, value);
            yield return null;
        }
    }

    private void ResetMaterial()
    {
        material.SetFloat(propertyName, -0.5f);
    }
}

