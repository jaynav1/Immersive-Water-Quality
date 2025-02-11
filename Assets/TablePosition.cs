using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TablePosition : MonoBehaviour
{
    private bool farmMode;
    private int currentFarm;

    // Table objects
    public GameObject smallTable;
    public GameObject table;
    public GameObject bigTable;

    // Start is called before the first frame update
    void Start()
    {

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

    private void ResetTable(GameObject table)
    {
        FarmSimulator simulator = table.GetComponent<FarmSimulator>();
        if (simulator != null)
        {
            simulator.Reset();
        }
    }

    private void ResetAllTables()
    {
        ResetTable(smallTable);
        ResetTable(table);
        ResetTable(bigTable);
    }

    private void UpdateTables()
    {
        if (farmMode)
        {
            if (currentFarm == 0)
            {
                ResetTable(smallTable);

                SetTableVisibility(smallTable, true);
                SetTableVisibility(table, false);
                SetTableVisibility(bigTable, false);

                //smallTable.transform.localPosition = 3 * Vector3.left + 2 * Vector3.down;
            }
            else if (currentFarm == 1)
            {
                ResetTable(table);

                SetTableVisibility(smallTable, false);
                SetTableVisibility(table, true);
                SetTableVisibility(bigTable, false);
            }
            else if (currentFarm == 2)
            {
                ResetTable(bigTable);

                SetTableVisibility(smallTable, false);
                SetTableVisibility(table, false);
                SetTableVisibility(bigTable, true);

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
        }
    }

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
