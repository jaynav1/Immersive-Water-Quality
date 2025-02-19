using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    private bool farmMode;
    private int currentFarm;
    private int currentScenario;
    private ParticleSystem rain;

    // Table objects
    public GameObject smallTable;
    public GameObject table;
    public GameObject bigTable;
    public GameObject rainObject;

    // Start is called before the first frame update
    void Start()
    {
        rain = rainObject.GetComponent<ParticleSystem>();
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

    // Set the scenario for the tables
    public void SetScenario(int newScenario)
    {
        currentScenario = newScenario;

        // hardcoded logic to toggle rain
        if (currentScenario == 0)
        {
            rain.Stop();
        } 
        else
        {
            rain.Play();
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
}
