using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentSpawnManager : MonoBehaviour
{
    #region Variables
    [Header("Spawn Parameters")]
    [SerializeField] private GameObject agentPrefab; // Types of agents to spawn
    private Transform spawnPoint; // Location where agents will spawn from
    public Transform SpawnPoint
    {
        get => spawnPoint;
        set
        {
            spawnPoint = value;
            Debug.Log($"Spawn point was set to the location of {value.gameObject.name}.");
        }
    }
    private Transform target; // Target passed on to agents spawned from this manager
    public Transform Target
    {
        get
        {
            if (target != null)
                return target;
            else
            {
                Debug.LogWarning("No target has been set for spawned agents to move towards.");
                return target;
            }
        }
        set => target = value;
    }

    // Agent Affinities
    [Header("Affinities for Spawned Agents")]
    [Range(0, 100)]
    [Tooltip("In Game UI Also")]
    public int windowAffinity = 0;
    [Range(0, 100)]
    [Tooltip("In Game UI Also")]
    public int connectivityAffinity = 0;

    // Accessories
    [Header("UI Elements")]
    public Text agentWindowAffinityText;
    public Text agentConnectivityAffinityText;
    #endregion


    #region Unity Methods
    private void SpawnAgent()
    {
        GameObject agent = (GameObject)Instantiate(agentPrefab, spawnPoint.position, spawnPoint.rotation);

        // Set this specific instantiated agent's parameters
        UnitSimple agentAI = agent.GetComponent<UnitSimple>();
        agentAI.target = target;
        agentAI.Window = windowAffinity;
        agentAI.Connectivity = connectivityAffinity;

        // Tell agent to determine path after setting parameters
        agentAI.AgentRequestPath();
    }

    private void SpawnAgent(Transform spawnPoint)
    {
        GameObject agent = (GameObject)Instantiate(agentPrefab, spawnPoint.position, spawnPoint.rotation);

        // Set this specific instantiated agent's parameters
        UnitSimple agentAI = agent.GetComponent<UnitSimple>();
        agentAI.target = target;
        agentAI.Window = windowAffinity;
        agentAI.Connectivity = connectivityAffinity;

        DataRecorder.Instance.GenerateNewPathData();
        DataRecorder.Instance.SetCurrentPathSpawn(spawnPoint);

        // Tell agent to determine path after setting parameters
        agentAI.AgentRequestPath();
    }

    public void Spawn()
    {
        if(spawnPoint != null)
            SpawnAgent(spawnPoint);
        else
            Debug.Log("There is no current spawn point.");
    }

    #endregion
}
