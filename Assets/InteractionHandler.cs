using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System;

public class InteractionHandler : XRSimpleInteractable
{
    [SerializeField]
    private GameObject _scrollView; // Reference to the Canvas ScrollView
    [SerializeField]
    private IFeatureSetter _featureSetter;
    [SerializeField]
    private GameObject sphere;

    public static event Action<GameObject> OnDataLoaded;

    private void Start()
    {
        // Ensure the chart is initially hidden
        if (_scrollView != null)
        {
            _scrollView.SetActive(false);
        }


    }

    public void deselect()
    {
        if (_scrollView != null)
        {
            _scrollView.SetActive(false);
        }

        // get sphere renderer
        Renderer sphereRenderer = sphere.GetComponent<Renderer>();

        if (sphereRenderer != null)
        {

            // get the material
            Material sphereMaterial = sphereRenderer.material;

            // set the color
            sphereMaterial.color = Color.white;
        }


    }

    public void select(Color color)
    {
        if (_scrollView != null)
        {
            _scrollView.SetActive(true);
        }
        // get sphere renderer
        Renderer sphereRenderer = sphere.GetComponent<Renderer>();

        if (sphereRenderer != null)
        {
            // get the material
            Material sphereMaterial = sphereRenderer.material;

            // set the color
            sphereMaterial.color = color;
        }
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        // Handle hover enter event
        //_textMeshProUGUI.text = "Hovered!";
        //Debug.Log("Hover Entered");
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        // Handle hover exit event
        //_textMeshProUGUI.text = "";
        //Debug.Log("Hover Exited");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // Handle select enter event
        //_textMeshProUGUI.text = "Selected!";
        Debug.Log("Select Entered");

        // Toggle the chart Canvas visibility
        if (_scrollView != null)
        {
            _scrollView.SetActive(!_scrollView.activeSelf);
        }

        // Read the Site ID from the IFeatureSetter component
        var siteID = _featureSetter.SiteId;

        if (siteID != null)
        {
            // Set the chart data
            OnDataLoaded?.Invoke(this.gameObject);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // Handle select exit event
        //_textMeshProUGUI.text = "";
        Debug.Log("Select Exited");
    }
}
