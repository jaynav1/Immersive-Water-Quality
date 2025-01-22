using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SiteDataColour : MonoBehaviour
{
    [SerializeField]
    private IFeatureSetter _featureSetter;
    [SerializeField]
    private GameObject sphere;

    private Dictionary<string, List<(string Date, string Value)>> _siteData;
    private string _currentParameter;
    private DateTime _currentDate;
    private DataManager _dataManager;

    // Start is called before the first frame update
    async void Start()
    {
        var siteId = _featureSetter.SiteId;
        _siteData = await ReadCSVForSiteIDAsync(siteId);

        if (DataManager.Instance != null)
        {
            _dataManager = DataManager.Instance;
            Debug.Log("DataManager instance found.");
        }
        else
        {
            Debug.LogError("DataManager instance not found.");
        }


        _dataManager.OnParameterChanged += HandleParameterChanged;
        _dataManager.OnDateChanged += HandleDateChanged;
        _dataManager.OnAllSitesRegistered += HandleAllSitesRegistered;

    }


    // Update is called once per frame
    void Update()
    {
    }

    private void OnDisable()
    {
        _dataManager.OnParameterChanged -= HandleParameterChanged;
        _dataManager.OnDateChanged -= HandleDateChanged;
        _dataManager.OnAllSitesRegistered -= HandleAllSitesRegistered;
    }

    private void HandleParameterChanged(string parameter)
    {
        _currentParameter = parameter;
        RegisterLocalMinMaxValues();
    }

    private void HandleDateChanged(DateTime date)
    {
        _currentDate = date;
        UpdateColor();
    }

    private void HandleAllSitesRegistered()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (_siteData.ContainsKey(_currentParameter))
        {
            var parameterData = _siteData[_currentParameter]
                .Select(entry => (Date: DateTime.Parse(entry.Date), Value: float.Parse(entry.Value)))
                .ToList();

            if (parameterData.Any())
            {
                // Get the closest value to the given date
                float value = GetClosestValue(parameterData, _currentDate);
                Color color = GetColorForValue(value);
                // Apply color to the sphere's renderer
                sphere.GetComponent<Renderer>().material.color = color;
            }
        }
        else
        {
            // Set color to white if parameter is not available
            sphere.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private void RegisterLocalMinMaxValues()
    {
        var values = _siteData[_currentParameter]
            .Select(entry => float.Parse(entry.Value))
            .ToList();

        if (values.Any())
        {
            float localMin = values.Min();
            float localMax = values.Max();
            _dataManager.RegisterSiteMinMax(localMin, localMax);
        }
    }

    private float GetClosestValue(List<(DateTime Date, float Value)> parameterData, DateTime targetDate)
    {
        var closestEntry = parameterData
            .OrderBy(entry => Math.Abs((entry.Date - targetDate).TotalDays))
            .First();

        return closestEntry.Value;
    }

    private Color GetColorForValue(float value)
    {
        float normalizedValue = (value - _dataManager.MinValue) / (_dataManager.MaxValue - _dataManager.MinValue);
        return Color.Lerp(Color.blue, Color.red, normalizedValue); // Example: blue to red gradient
    }

    private async Task<Dictionary<string, List<(string Date, string Value)>>> ReadCSVForSiteIDAsync(string siteID)
    {
        string folderPath = Path.Combine(Application.dataPath, "WQ_bysite");
        string filePath = Path.Combine(folderPath, $"{siteID}.csv");
        var result = new Dictionary<string, List<(string Date, string Value)>>();

        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file for Site ID {siteID} not found at path: {filePath}");
            }

            string[] lines = await Task.Run(() => File.ReadAllLines(filePath));
            if (lines.Length == 0)
            {
                throw new InvalidDataException("CSV file is empty.");
            }

            // Read the header to find the index of the date parameter
            var headers = lines[0].Split(',');
            int dateIndex = Array.IndexOf(headers, "\"Date\"");

            if (dateIndex == -1)
            {
                throw new InvalidDataException("Date parameter not found in the CSV headers.");
            }

            // Read the data lines
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                if (values.Length > dateIndex)
                {
                    string dateTime = values[dateIndex].Trim('"');
                    string date = dateTime.Split(' ')[1]; // Extract the date part

                    for (int j = 0; j < headers.Length; j++)
                    {
                        if (j != dateIndex)
                        {
                            string key = headers[j].Trim('"');
                            string value = values[j].Trim('"');

                            if (float.TryParse(value, out float parsedValue))
                            {
                                if (!result.ContainsKey(key))
                                {
                                    result[key] = new List<(string Date, string Value)>();
                                }

                                result[key].Add((date, value));
                            }
                        }
                    }
                }
            }

            // Sort the result by date for each parameter
            foreach (var key in result.Keys.ToList())
            {
                result[key] = result[key].OrderBy(entry => DateTime.Parse(entry.Date)).ToList();
            }
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError(ex.Message);
        }
        catch (InvalidDataException ex)
        {
            Debug.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An unexpected error occurred: {ex.Message}");
        }

        return result;
    }

}
