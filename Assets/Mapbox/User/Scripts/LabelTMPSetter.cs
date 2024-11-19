using Mapbox.Unity.MeshGeneration.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMesh Pro namespace

public class LabelTMPSetter : MonoBehaviour, IFeaturePropertySettable
{
    [SerializeField]
    private TextMeshProUGUI _textMeshProUGUI; // Use TextMeshProUGUI for UI text

    public void Set(Dictionary<string, object> props)
    {
        _textMeshProUGUI.text = "";

        if (props.ContainsKey("name"))
        {
            _textMeshProUGUI.text = props["name"].ToString();
        }
        else if (props.ContainsKey("house_num"))
        {
            _textMeshProUGUI.text = props["house_num"].ToString();
        }
        else if (props.ContainsKey("type"))
        {
            _textMeshProUGUI.text = props["type"].ToString();
        }
    }
}


