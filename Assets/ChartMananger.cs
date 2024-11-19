using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public enum ChartMode
{
    Single,
    Multi
}



public class ChartManager : MonoBehaviour
{
    public static ChartManager Instance { get; private set; }
    public List<Color> colors = new List<Color> { Color.blue, Color.green, Color.yellow, Color.red };

    private ChartMode currentMode = ChartMode.Multi; // Store the current mode

    [SerializeField]
    private GameObject chartObject; // Reference to the chart GameObject
    [SerializeField]
    private TMP_Dropdown dropdownOne; // Reference to the new TMP_Dropdown UI element
    [SerializeField] 
    private TMP_Dropdown dropdownTwo;
    [SerializeField]
    private Button modeButton; // Reference to the Button UI element

    private List<string> siteIds = new List<string>(); // Store the site IDs
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>(); // Store the prefabs for each site ID
    private string singleSiteId; // Store the site ID for single mode
    private Dictionary<string, List<(string, Dictionary<string, string>)>> siteDataCache = new Dictionary<string, List<(string, Dictionary<string, string>)>>(); // Cache for site data

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        InteractionHandler.OnDataLoaded += OnDataLoaded;
        dropdownOne.onValueChanged.AddListener(OnDropdownValueChanged);
        dropdownTwo.onValueChanged.AddListener(OnDropdownValueChanged);
        modeButton.onClick.AddListener(OnModeButtonClicked);
    }

    private void OnDisable()
    {
        InteractionHandler.OnDataLoaded -= OnDataLoaded;
        dropdownOne.onValueChanged.RemoveListener(OnDropdownValueChanged);
        dropdownTwo.onValueChanged.RemoveListener(OnDropdownValueChanged);
        modeButton.onClick.RemoveListener(OnModeButtonClicked);
    }

    private async void OnDropdownValueChanged(int index)
    {
        string selectedParameterOne = dropdownOne.options[dropdownOne.value].text;
        string selectedParameterTwo = dropdownTwo.options[dropdownTwo.value].text;
        if (currentMode == ChartMode.Multi)
        {
            await UpdateMultiChartWithSelectedDataAsync(siteIds, selectedParameterOne);
        }
        else if (currentMode == ChartMode.Single)
        {
            await UpdateSingleChartWithSelectedDataAsync(singleSiteId, selectedParameterOne, selectedParameterTwo);

        }
    }

    private async void OnModeButtonClicked()
    {
        if (currentMode == ChartMode.Multi)
        {
            currentMode = ChartMode.Single;
            // Show dropdownTwo
            dropdownTwo.gameObject.SetActive(true);

            // Logic for switching to single mode will be added here
            singleSiteId = siteIds.FirstOrDefault();

            //deselect the other sites
            foreach (var item in prefabs)
            {
                if (item.Key != singleSiteId)
                {
                    item.Value.GetComponent<InteractionHandler>().deselect();
                }
            }

            modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "single site";

            await UpdateSingleChartAsync(singleSiteId);
        }
        else
        {
            currentMode = ChartMode.Multi;
            modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "multiple sites";
            // hide dropdownTwo
            dropdownTwo.gameObject.SetActive(false);

            siteIds = new List<string> { singleSiteId };

            await UpdateMultiChartAsync(siteIds);
        }
    }



    public async void OnDataLoaded(GameObject go)
    {
        IFeatureSetter featureSetter = go.GetComponent<IFeatureSetter>();
        InteractionHandler ih = go.GetComponent<InteractionHandler>();


        string siteId = featureSetter.SiteId;

        prefabs[siteId] = go;

        if (!siteDataCache.ContainsKey(siteId))
        {
            var data = await ReadCSVForSiteIDAsync(siteId);
            siteDataCache[siteId] = data;
        }

        if (currentMode == ChartMode.Single)
        {
            singleSiteId = siteId;
            await UpdateSingleChartAsync(singleSiteId);

            //select the site
            ih.select(Color.blue);

            //deselect the other sites
            foreach (var item in prefabs)
            {
                if (item.Key != siteId)
                {
                    item.Value.GetComponent<InteractionHandler>().deselect();
                }
            }
        }
        else if (currentMode == ChartMode.Multi)
        {
            if (siteIds.Contains(siteId))
            {
                siteIds.Remove(siteId);
                //siteDataCache.Remove(siteId);
            }
            else
            {
                siteIds.Add(siteId);
            }

            //deselect all sites
            foreach (var item in prefabs)
            {
                item.Value.GetComponent<InteractionHandler>().deselect();
            }

            //select the sites in order of list
            for (int i = 0; i < siteIds.Count; i++)
            {
                prefabs[siteIds[i]].GetComponent<InteractionHandler>().select(colors[i]);
            }

            await UpdateMultiChartAsync(siteIds);
        }

    }

    public async Task UpdateSingleChartAsync(string siteId)
    {
        Debug.Log($"Updating chart for Site ID: {siteId}");

        // get keys from siteDataCache
        var keys = siteDataCache[siteId][0].Item2.Keys.ToList();

        // Populate the dropdowns with the keys
        PopulateDropdown(dropdownOne, keys);
        PopulateDropdown(dropdownTwo, keys);

        //Change dropdownTwo value to the second key
        dropdownTwo.value = 1;

        if (keys.Count > 0)
        {
            await UpdateSingleChartWithSelectedDataAsync(siteId, keys[0], keys[1]);
        }
    }

    public async Task UpdateSingleChartWithSelectedDataAsync(string siteId, string dataParameter1, string dataParameter2)
    {
        var chartData = await Task.Run(() =>
        {
            Debug.Log($"Updating chart for Site ID: {siteId} with data parameters: {dataParameter1}, {dataParameter2}");

            var result = new List<(string date, float value1, float value2)>();

            // Read data for the site ID
            var data = siteDataCache[siteId];

            // Filter data to include only entries with valid data for both parameters
            var filteredData = data.Where(entry =>
                entry.Item2.ContainsKey(dataParameter1) && entry.Item2[dataParameter1] != "NA" &&
                entry.Item2.ContainsKey(dataParameter2) && entry.Item2[dataParameter2] != "NA"
            ).ToList();

            if (filteredData.Count > 0)
            {
                // Get the first and last valid entries
                var firstEntry = filteredData.First();
                var lastEntry = filteredData.Last();

                // Get the date range between the first and last valid entries
                var startDate = DateTime.Parse(firstEntry.Item1);
                var endDate = DateTime.Parse(lastEntry.Item1);

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dateStr = date.ToString("yyyy-MM-dd");
                    var entry = data.FirstOrDefault(e => DateTime.Parse(e.Item1) == date && e.Item2.ContainsKey(dataParameter1) && e.Item2[dataParameter1] != "NA" && e.Item2.ContainsKey(dataParameter2) && e.Item2[dataParameter2] != "NA");

                    if (entry != default)
                    {
                        var val1 = float.TryParse(entry.Item2[dataParameter1], out float value1) ? value1 : -1;
                        var val2 = float.TryParse(entry.Item2[dataParameter2], out float value2) ? value2 : -1;
                        result.Add((dateStr, val1, val2));
                    }
                    else
                    {
                        result.Add((dateStr, -1, -1)); // Use ignore value
                    }
                }
            }

            return result;
        });

        var chart = chartObject.GetComponent<LineChart>();
        var title = chart.EnsureChartComponent<Title>();
        title.text = $"Site ID: {siteId}";
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        var yAxis1 = chart.GetChartComponent<YAxis>(0);
        yAxis1.type = Axis.AxisType.Value;
        var yAxis2 = chart.GetChartComponent<YAxis>(1);
        yAxis2.type = Axis.AxisType.Value;
        yAxis2.show = true;

        yAxis1.axisName.name = dataParameter1;
        yAxis2.axisName.name = dataParameter2;


        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;

        chart.RemoveData();

        var serie1 = chart.GetSerie(dataParameter1) ?? chart.AddSerie<Line>(dataParameter1);
        var serie2 = chart.GetSerie(dataParameter2) ?? chart.AddSerie<Line>(dataParameter2);
        serie1.yAxisIndex = 0; // Assign serie1 to the first Y-axis
        serie2.yAxisIndex = 1; // Assign serie2 to the second Y-axis
        serie1.ignore = true;
        serie1.ignoreValue = -1;
        serie2.ignore = true;
        serie2.ignoreValue = -1;

        foreach (var (date, value1, value2) in chartData)
        {
            chart.AddXAxisData(date);
            serie1.AddData(value1);
            serie2.AddData(value2);
        }

    }


    public async Task UpdateMultiChartAsync(List<string> siteIds)
    {
        Debug.Log($"Updating chart for Site IDs: {string.Join(", ", siteIds)}");

        // if no site IDs are selected, return
        if (siteIds.Count == 0)
        {
            return;
        }

        // Read data for all site IDs
        var allData = new List<List<(string, Dictionary<string, string>)>>();

        foreach (var siteId in siteIds)
        {
            if (siteDataCache.ContainsKey(siteId))
            {
                var data = siteDataCache[siteId];
                if (data.Count > 0)
                {
                    allData.Add(data);
                }
            }
        }

        // Find common keys
        var commonKeys = new HashSet<string>(allData[0][0].Item2.Keys);
        foreach (var data in allData)
        {
            var keys = new HashSet<string>(data[0].Item2.Keys);
            commonKeys.IntersectWith(keys);
        }

        // Populate the dropdown with the common keys
        PopulateDropdown(dropdownOne, new List<string>(commonKeys));

        // Default to first in dropdown
        if (dropdownOne.options.Count > 0)
        {
            await UpdateMultiChartWithSelectedDataAsync(siteIds, dropdownOne.options[0].text);
        }
    }


    private void PopulateDropdown(TMP_Dropdown dropdown, List<string> keys)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(keys);
    }

    public async Task UpdateMultiChartWithSelectedDataAsync(List<string> siteIds, string dataParameter)
    {
        // Perform non-Unity operations on a background thread
        var chartData = await Task.Run(() =>
        {
            Debug.Log($"Updating chart for Site IDs: {string.Join(", ", siteIds)} with data parameter: {dataParameter}");

            // Data structure to hold the chart data
            var chartData = new List<(string date, Dictionary<string, float> siteData)>();

            // Find the latest starting date and latest ending date with valid data for the parameter
            DateTime latestStartDate = DateTime.MinValue;
            DateTime latestEndDate = DateTime.MinValue;

            foreach (var siteId in siteIds)
            {
                var data = siteDataCache[siteId];

                // Filter data to include only entries with valid data for the parameter
                var filteredData = data.Where(entry => entry.Item2.ContainsKey(dataParameter) && entry.Item2[dataParameter] != "NA").ToList();

                if (filteredData.Count > 0)
                {
                    var startDate = DateTime.Parse(filteredData.First().Item1);
                    var endDate = DateTime.Parse(filteredData.Last().Item1);

                    if (startDate > latestStartDate)
                    {
                        latestStartDate = startDate;
                    }

                    if (endDate > latestEndDate)
                    {
                        latestEndDate = endDate;
                    }
                }
            }

            Debug.Log($"Latest start date: {latestStartDate}, Latest end date: {latestEndDate}");

            // Iterate over the date range and prepare the chart data
            for (var date = latestStartDate; date <= latestEndDate; date = date.AddDays(1))
            {
                var dateStr = date.ToString("yyyy-MM-dd");
                var siteData = new Dictionary<string, float>();

                foreach (var siteId in siteIds)
                {
                    var data = siteDataCache[siteId];
                    var entry = data.FirstOrDefault(e => DateTime.Parse(e.Item1) == date && e.Item2.ContainsKey(dataParameter) && e.Item2[dataParameter] != "NA");
                    if (entry != default)
                    {
                        var val = float.TryParse(entry.Item2[dataParameter], out float value) ? value : -1;
                        siteData[siteId] = val;
                    }
                    else
                    {
                        siteData[siteId] = -1; // Use ignore value
                    }
                }

                chartData.Add((dateStr, siteData));
            }

            return chartData;
        });

        // Perform Unity-specific operations on the main thread
        var chart = chartObject.GetComponent<LineChart>();
        var title = chart.EnsureChartComponent<Title>();
        title.text = $"Comparison Chart";
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;
        yAxis.axisName.name = dataParameter;
        var yAxis2 = chart.GetChartComponent<YAxis>(1);
        yAxis2.show = false;
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;

        chart.RemoveData();

        foreach (var (date, siteData) in chartData)
        {
            chart.AddXAxisData(date);

            foreach (var siteId in siteIds)
            {
                var serie = chart.GetSerie(siteId) ?? chart.AddSerie<Line>(siteId);
                serie.ignore = true;
                serie.ignoreValue = -1;

                if (siteData.TryGetValue(siteId, out float value))
                {
                    serie.AddData(value);
                }
                else
                {
                    serie.AddData(-1); // Use ignore value
                }
            }
        }
    }

    private async Task<List<(string, Dictionary<string, string>)>> ReadCSVForSiteIDAsync(string siteID)
    {
        string folderPath = Path.Combine(Application.dataPath, "WQ_bysite");
        string filePath = Path.Combine(folderPath, $"{siteID}.csv");
        var result = new List<(string, Dictionary<string, string>)>();

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
                    var parameters = new Dictionary<string, string>();

                    for (int j = 0; j < headers.Length; j++)
                    {
                        if (j != dateIndex)
                        {
                            string key = headers[j].Trim('"');
                            string value = values[j].Trim('"');
                            parameters[key] = value;
                        }
                    }

                    result.Add((date, parameters));
                }
            }

            // Sort the result by date
            result = result.OrderBy(entry => DateTime.Parse(entry.Item1)).ToList();
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


