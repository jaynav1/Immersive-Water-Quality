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
    public GameObject overflowWater;
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
    private OverflowIrrigation overflowScript;

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
        overflowScript = overflowWater.GetComponent<OverflowIrrigation>();
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
            StartCoroutine(irrigationWaterPlaneScript.MovePlane(0));
            yield return StartCoroutine(paddockScript.AnimateFill());

            yield return cowFactory.MoveToPlane(paddock);

            // 3. saturate the paddock
            yield return StartCoroutine(paddockScript.AnimateSaturation());

            yield return cowFactory.ReturnCows();

            // 4. fill the reuse water plane while draining the paddock and no overflow due to sufficient storage
            StartCoroutine(paddockScript.AnimateDrain());
            yield return StartCoroutine(reuseWaterPlaneScript.ChangeVolumeByAmount(900));

            // 5. fill the effluent water plane
            shedPipeAnimator.StartAnimation(this);
            yield return StartCoroutine(effluentWaterPlaneScript.ChangeVolumeByAmount(500));
            shedPipeAnimator.StopAnimation(this);

            // 6. Pump water back to irrigation (effluent + pump rate of reuse)

            StartCoroutine(effluentWaterPlaneScript.MovePlane(0));
            StartCoroutine(reuseWaterPlaneScript.ChangeVolumeByAmount(-300));
            reusePipeAnimator.StartAnimation(this);
            effluentPipeAnimator.StartAnimation(this);
            //overflowParticles.Stop();
            yield return StartCoroutine(irrigationWaterPlaneScript.ChangeVolumeByAmount(1000));
        }
        
    }

    // Scenario 2: Heavy rainfall with overflow
    private IEnumerator AnimateHeavyRainfall()
    {
        while (true)
        {       
            // 1. fill the irrigation water plane
            //reusePipeAnimator.StartAnimation(this);
            yield return irrigationWaterPlaneScript.ChangeVolumeByAmount(1900);
            reusePipeAnimator.StopAnimation(this);
            effluentPipeAnimator.StopAnimation(this);

            // 2. fill the paddock while draining the irrigation water plane
            StartCoroutine(irrigationWaterPlaneScript.MovePlane(0));
            yield return StartCoroutine(paddockScript.AnimateFill());

            yield return cowFactory.MoveToPlane(paddock);

            // 3. saturate the paddock
            yield return StartCoroutine(paddockScript.AnimateSaturation());

            yield return cowFactory.ReturnCows();

            // 4. fill the reuse water plane while draining the paddock and overflow due to heavy rainfall
            StartCoroutine(paddockScript.AnimateDrain());
            yield return StartCoroutine(reuseWaterPlaneScript.ChangeVolumeByAmount(1200));

            // 5. fill the effluent water plane 
            shedPipeAnimator.StartAnimation(this);
            yield return StartCoroutine(effluentWaterPlaneScript.ChangeVolumeByAmount(500));
            shedPipeAnimator.StopAnimation(this);

            // 6. Pump water back to irrigation (effluent + pump rate of reuse)
            StartCoroutine(effluentWaterPlaneScript.MovePlane(0));
            StartCoroutine(reuseWaterPlaneScript.ChangeVolumeByAmount(-600));
            reusePipeAnimator.StartAnimation(this);
            effluentPipeAnimator.StartAnimation(this);
            yield return StartCoroutine(irrigationWaterPlaneScript.ChangeVolumeByAmount(600));
        }
    }

    // Scenario 3: Drought conditions with limited water
    private IEnumerator AnimateDroughtConditions()
    {
        while (true)
        {
            // 1. fill the irrigation water plane with limited water
            //reusePipeAnimator.StartAnimation(this);
            yield return irrigationWaterPlaneScript.ChangeVolumeByAmount(500);
            reusePipeAnimator.StopAnimation(this);
            effluentPipeAnimator.StopAnimation(this);

            // 2. fill the paddock while draining the irrigation water plane
            StartCoroutine(irrigationWaterPlaneScript.MovePlane(0));
            yield return StartCoroutine(paddockScript.AnimateFill());

            yield return cowFactory.MoveToPlane(paddock);

            // 3. saturate the paddock
            yield return StartCoroutine(paddockScript.AnimateSaturation());

            yield return cowFactory.ReturnCows();

            // 4. fill the reuse water plane while draining the paddock with limited water
            StartCoroutine(paddockScript.AnimateDrain());
            yield return StartCoroutine(reuseWaterPlaneScript.ChangeVolumeByAmount(500));

            // 5. fill the effluent water plane with limited water
            shedPipeAnimator.StartAnimation(this);
            yield return StartCoroutine(effluentWaterPlaneScript.ChangeVolumeByAmount(300));
            shedPipeAnimator.StopAnimation(this);

            // 6. Pump water back to irrigation (effluent + pump rate of reuse)
            StartCoroutine(effluentWaterPlaneScript.MovePlane(0));
            StartCoroutine(reuseWaterPlaneScript.ChangeVolumeByAmount(-500));
            reusePipeAnimator.StartAnimation(this);
            effluentPipeAnimator.StartAnimation(this);
            yield return StartCoroutine(irrigationWaterPlaneScript.ChangeVolumeByAmount(500));
            yield return new WaitForSeconds(3f);
        }
    }

    // Reset the farm simulator to the initial state
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

        //Restart the chosen scenario
        StartCoroutine(ChooseScenario(currentScenario));
    }

    public void SetScenario(int newScenario)
    {
        currentScenario = newScenario;
    }

    // Method to choose the scenario based on the value
    private IEnumerator ChooseScenario(int value)
    {
        if (value == 0)
        {
            return AnimateDroughtConditions();
        }
        else if (value == 1)
        {
            return AnimateNormalWeather();
        }
        else if (value == 2)
        {
            return AnimateHeavyRainfall();
        }
        else
        {
            return AnimateNormalWeather();
        }
    }
    
    public void SetOffset(bool newUseOffset)
    {
        paddockScript.SetOffset(newUseOffset);
        overflowScript.SetOffset(newUseOffset);
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


// MaterialAnimator class to animate material properties
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

