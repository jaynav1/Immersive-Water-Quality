using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }

    public event Action<string> OnParameterChanged;
    public event Action<DateTime> OnDateChanged;
    public event Action OnAllSitesRegistered;
    private int totalSites = 18;
    public int SiteCount { get; set; }
    private int registeredSites = 0;

    public bool AllSitesRegistered => registeredSites == totalSites;

    private void Awake()
    {
        Debug.Log("DataManager awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(WaitForPrefabs());
    }

    private IEnumerator WaitForPrefabs()
    {
        yield return new WaitForSeconds(10);
        Debug.Log("Waited for prefabs");
        ExampleEvent();
    }

    public void RegisterSiteMinMax(float siteMin, float siteMax)
    {
        MinValue = Math.Min(siteMin, MaxValue);
        MaxValue = Math.Max(siteMax, MaxValue);
        registeredSites++;
        if (registeredSites == totalSites)
        {
            SendDateEvent(DateTime.Parse("14/10/2020"));
        }
    }

    public void SendParameterEvent(string parameter)
    {
        MinValue = float.MaxValue;
        MaxValue = float.MinValue;
        registeredSites = 0;
        OnParameterChanged?.Invoke(parameter);
    }

    public void SendDateEvent(DateTime date)
    {
        if (AllSitesRegistered)
        {
            OnDateChanged?.Invoke(date);
        }
    }


    public void ExampleEvent()
    {
        Debug.Log("Example event");
        SendParameterEvent("WaterTemperature");

    }
}