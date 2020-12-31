using UnityEngine;
using UnityEngine.UI;

public class SliderAgentAffinity : MonoBehaviour
{
    public string key;

    private Slider slider;
    private AgentSpawnManager spawnManager;
    public Text typeText;
    public Text valueText;
    private string sliderTypeText;
    

    private void Awake()
    {
        slider = GetComponent<Slider>();
        spawnManager = FindObjectOfType<AgentSpawnManager>();
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(delegate { SliderValueChanged(slider); });
        sliderTypeText = key + ": ";
        typeText.text = sliderTypeText;
        SliderValueChanged(slider);
    }

    // SliderValueChange methods split like this to make it simpler to apply to multiple sliders quickly.
    // Allows for only a single switch statement call at Start for each individual slider this way.
    /*
     * Changes spawn manager value to match the slider so next agent spawned will assume slider value
     */
    private void SliderValueChanged(Slider slider)
    {
        int value = (int)(slider.value * MathArchCost.Instance.MAX_AFFINITY);
        if (spawnManager.SetAffinityByKey(key, value))
        {
            valueText.text = value.ToString();
        }
    }
}
