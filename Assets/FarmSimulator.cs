using System.Collections;
using UnityEngine;

public class FarmSimulator : MonoBehaviour
{
    // Public variables for the game objects
    public GameObject irrigationWaterPlane;
    public GameObject reuseWaterPlane;
    public GameObject effluentWaterPlane;
    public GameObject paddock;
    public GameObject overflow;

    // Pipe materials
    public Material reusePipe;
    public Material shedPipe;
    public Material effluentPipe;

    // Animation variables
    private AnimatePlane irrigationWaterPlaneScript;
    private AnimatePlane reuseWaterPlaneScript;
    private AnimatePlane effluentWaterPlaneScript;
    private PaddockIrrigation paddockScript;
    private ParticleSystem overflowParticles;

    private MaterialAnimator reusePipeAnimator;
    private MaterialAnimator shedPipeAnimator;
    private MaterialAnimator effluentPipeAnimator;

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

        // Initialize material animators
        reusePipeAnimator = new MaterialAnimator(reusePipe, "_Panner");
        shedPipeAnimator = new MaterialAnimator(shedPipe, "_Panner");
        effluentPipeAnimator = new MaterialAnimator(effluentPipe, "_Panner");

        // Start the animation
        StartCoroutine(Animate());
        //StartCoroutine(TestMaterialAnimation());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator Animate()
    {
        // Use multiple subroutines to animate the farm

        // 1. fill the irrigation water plane
        reusePipeAnimator.StartAnimation(this);
        yield return irrigationWaterPlaneScript.movePlane(1000);
        reusePipeAnimator.StopAnimation(this);

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