using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButtonController : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private AgentSpawnManager spawnManager;

    private void Awake()
    {
        spawnManager = GetComponent<AgentSpawnManager>();
    }

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];

            int buttonIndex = i;
            button.onClick.AddListener(() => SpawnAgent(buttonIndex));
        }
    }

    private void SpawnAgent(int buttonIndex)
    {
        spawnManager.SpawnAgent(buttonIndex);
    }
}
