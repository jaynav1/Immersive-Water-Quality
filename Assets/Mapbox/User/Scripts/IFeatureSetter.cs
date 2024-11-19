using Mapbox.Unity.MeshGeneration.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMesh Pro namespace

public class IFeatureSetter : MonoBehaviour, IFeaturePropertySettable
{
    [SerializeField]
    private TextMeshProUGUI _textMeshProUGUI; // Use TextMeshProUGUI for UI text
    [SerializeField]
    private GameObject _scrollContent; // Reference to the Canvas ScrollView content
    [SerializeField]
    private GameObject _textPrefab; // Reference to the TextMeshProUGUI prefab

    // Public property to expose the Site Id
    public string SiteId { get; private set; }

    public void Set(Dictionary<string, object> props)
    {

        // Create a new TextMeshProUGUI element for each key-value pair in props
        foreach (var prop in props)
        {
            //Debug.Log($"{prop.Key}: {prop.Value}");
            GameObject textObject = Instantiate(_textPrefab, _scrollContent.transform);
            TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                // Ensure the value is converted to a string
                string valueString = prop.Value != null ? prop.Value.ToString() : "null";
                textComponent.text = $"{prop.Key}: {valueString}";
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the instantiated prefab.");
            }
        }

        SetName(props["Site Name"].ToString());

        // Set the Site Id property
        SiteId = props["Site Id"].ToString();
    }

    private void SetName(string name)
    {
        _textMeshProUGUI.text = name;
    }
}