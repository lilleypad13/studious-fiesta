using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderAgentAffinity : MonoBehaviour
{
    public Text textBox;
    private string sliderValueInitialText;
    private string sliderValueCompleteText;

    public AgentSpawnManager spawnManager;

    public ArchitecturePathingData architectureType;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        switch (architectureType)
        {
            case ArchitecturePathingData.window:
                slider.onValueChanged.AddListener(delegate { SliderValueChangedWindow(slider); });
                sliderValueInitialText = "Agent Window Affinity: ";
                SliderValueChangedWindow(slider);
                break;
            case ArchitecturePathingData.connectivity:
                slider.onValueChanged.AddListener(delegate { SliderValueChangedConnectivity(slider); });
                sliderValueInitialText = "Agent Connectivity Affinity: ";
                SliderValueChangedConnectivity(slider);
                break;
            default:
                Debug.Log(name + "is missing an architectural type from available options for slider.");
                break;
        }

    }

    // SliderValueChange methods split like this to make it simpler to apply to multiple sliders quickly.
    // Allows for only a single switch statement call at Start for each individual slider this way.
    /*
     * Changes spawn manager value to match the slider so next agent spawned will assume slider value
     */
    private void SliderValueChangedWindow(Slider slider)
    {
        int value = (int)(slider.value * MathArchCost.Instance.MAX_AFFINITY);
        spawnManager.windowAffinity = value;
        textBox.text = sliderValueInitialText + value;
    }

    private void SliderValueChangedConnectivity(Slider slider)
    {
        int value = (int)(slider.value * MathArchCost.Instance.MAX_AFFINITY);
        spawnManager.connectivityAffinity = value;
        textBox.text = sliderValueInitialText + value;
    }
}
