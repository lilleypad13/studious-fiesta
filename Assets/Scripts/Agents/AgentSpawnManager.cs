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
            //Debug.Log($"Spawn point was set to the location of {value.gameObject.name} at {value.position}.");
        }
    }

    private Vector3 spawnPosition; // Location where agents will spawn from
    public Vector3 SpawnPosition
    {
        get => spawnPosition;
        set
        {
            spawnPosition = value;
            Debug.Log($"Spawn position was set to the location of {value} at {value}.");
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
        set
        {
            target = value;
            Debug.Log($"Target transform was set to the location of {value} at {value.position}.");
        }
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
        if (AGridRuntime.Instance.NodeFromWorldPoint(spawnPoint.position).walkable)
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
        else
            Debug.LogWarning("Tried to spawn new agent onto unwalkable space.");
    }

    private void SpawnAgent(Vector3 spawnPosition)
    {
        if (AGridRuntime.Instance.NodeFromWorldPoint(spawnPosition).walkable)
        {
            GameObject agent = (GameObject)Instantiate(agentPrefab, spawnPosition, Quaternion.identity);

            // Set this specific instantiated agent's parameters
            UnitSimple agentAI = agent.GetComponent<UnitSimple>();
            agentAI.target = target;
            agentAI.Window = windowAffinity;
            agentAI.Connectivity = connectivityAffinity;

            Debug.Log($"{agent.name} has target {target.name} at {target.position}.");

            DataRecorder.Instance.GenerateNewPathData();
            DataRecorder.Instance.SetCurrentPathSpawn(spawnPosition);

            // Tell agent to determine path after setting parameters
            agentAI.AgentRequestPath();
        }
        else
            Debug.LogWarning("Tried to spawn new agent onto unwalkable space.");
    }

    public void Spawn()
    {
        //if(spawnPoint != null)
        //    SpawnAgent(spawnPoint);
        if (spawnPosition != Vector3.zero)
            SpawnAgent(spawnPosition);
        else
            Debug.Log("There is no current spawn point.");
    }

    #endregion
}
