using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    private bool farmMode;
    private bool useOffset;
    private int currentFarm;
    private int currentScenario;
    private ParticleSystem rain;
    private GameObject directionalLight;

    // Table objects
    public GameObject smallTable;
    public GameObject table;
    public GameObject bigTable;
    public GameObject rainObject;

    // Start is called before the first frame update
    void Start()
    {
        rain = rainObject.GetComponent<ParticleSystem>();
        directionalLight = GameObject.Find("Directional Light");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleFarmMode(bool newMode)
    {
        farmMode = newMode;
        UpdateTables();
    }

    public void SetFarm(int newFarm)
    {
        currentFarm = newFarm;
        UpdateTables();
    }

    public void ToggleReuseOffset(bool newOffset)
    {
        useOffset = newOffset;
        table.GetComponent<FarmSimulator>().SetOffset(useOffset);
        smallTable.GetComponent<FarmSimulator>().SetOffset(useOffset);
        bigTable.GetComponent<FarmSimulator>().SetOffset(useOffset);
    }

    // Set the scenario for the tables
    public void SetScenario(int newScenario)
    {
        currentScenario = newScenario;

        // hardcoded logic to toggle rain
        if (currentScenario == 0)
        {
            rain.Stop();
            StartCoroutine(SetFogDensity(0.01f)); // Clear fog for scenario 0
            StartCoroutine(AnimateSun(Quaternion.Euler(75, 90, 0))); // Set light rotation for scenario 0
        }
        else
        {
            rain.Play();
            if (currentScenario == 1) StartCoroutine(SetFogDensity(0.2f));
            else StartCoroutine(SetFogDensity(0.3f));
            StartCoroutine(AnimateSun(Quaternion.Euler(45, 90, 0))); // Set light rotation for scenarios 1 and 2
        }
        UpdateTables();
    }


    // Reset the state of a table
    private void ResetTable(GameObject table)
    {
        FarmSimulator simulator = table.GetComponent<FarmSimulator>();
        if (simulator != null)
        {
            simulator.SetScenario(currentScenario);
            simulator.Reset();
        }
    }

    public void ResetAllTables()
    {
        ResetTable(smallTable);
        ResetTable(table);
        ResetTable(bigTable);
    }

    // Update the visibility of the tables based on the current farm and mode
    private void UpdateTables()
    {
        if (farmMode)
        {
            if (currentFarm == 0)
            {
                SetTableVisibility(smallTable, true);
                SetTableVisibility(table, false);
                SetTableVisibility(bigTable, false);
                
                
                ResetTable(smallTable);

                //smallTable.transform.localPosition = 3 * Vector3.left + 2 * Vector3.down;
            }
            else if (currentFarm == 1)
            {
                SetTableVisibility(smallTable, false);
                SetTableVisibility(table, true);
                SetTableVisibility(bigTable, false);

                ResetTable(table);
            }
            else if (currentFarm == 2)
            {
                SetTableVisibility(smallTable, false);
                SetTableVisibility(table, false);
                SetTableVisibility(bigTable, true);

                ResetTable(bigTable);
                //bigTable.transform.localPosition = 3 * Vector3.right + 2 * Vector3.down;
            }
        }
        else
        {
            //smallTable.transform.localPosition = 3 * Vector3.left;
            //bigTable.transform.localPosition = 3 * Vector3.right;

            SetTableVisibility(smallTable, true);
            SetTableVisibility(table, true);
            SetTableVisibility(bigTable, true);
            ResetAllTables();
        }
    }

    // Set the visibility of a table (renderers and colliders)
    private void SetTableVisibility(GameObject table, bool isVisible)
    {
        // table.SetActive(isVisible);


        Renderer[] renderers = table.GetComponentsInChildren<Renderer>();
        Collider[] colliders = table.GetComponentsInChildren<Collider>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isVisible;
        }

        foreach (Collider collider in colliders)
        {
            collider.enabled = isVisible;
        }
    }

    public IEnumerator SetFogDensity(float newFogDensity)
    {
        // Set the fog density over time
        float currentDensity = RenderSettings.fogDensity;
        float elapsedTime = 0f;
        float duration = 1f; // Duration of the transition in seconds
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(currentDensity, newFogDensity, elapsedTime / duration);
            yield return null;
        }
        RenderSettings.fogDensity = newFogDensity; // Ensure final value is set
    }

    public IEnumerator AnimateSun(Quaternion newRotation)
    {
        // Animate the sun rotation over time
        Quaternion currentRotation = directionalLight.transform.rotation;
        float elapsedTime = 0f;
        float duration = 1f; // Duration of the transition in seconds
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            directionalLight.transform.rotation = Quaternion.Slerp(currentRotation, newRotation, elapsedTime / duration);
            yield return null;
        }
        directionalLight.transform.rotation = newRotation; // Ensure final value is set
    }
}
