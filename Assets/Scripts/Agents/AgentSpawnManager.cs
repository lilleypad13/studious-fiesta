using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class AgentSpawnManager : MonoBehaviour
{
    #region Variables
    [Header("Spawn Parameters")]
    [SerializeField] private GameObject agentPrefab; // Types of agents to spawn
    [SerializeField] private bool isSearchingForNearestWalkableSpawn = false;
    [SerializeField] private int iterationsSearchForWalkable = 1;

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

    private Vector3 target; // Target passed on to agents spawned from this manager
    public Vector3 Target
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
            Debug.Log($"Target position was set to the location {value}.");
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

    private void SpawnAgent(Vector3 spawnPosition)
    {
        Node spawnNode = null;

        if (isSearchingForNearestWalkableSpawn)
        {
            spawnNode = AGridRuntime.Instance.FindNearestWalkableNodeFromWorldPoint(spawnPosition, iterationsSearchForWalkable);
        }
        else
        {
            if (!AGridRuntime.Instance.NodeFromWorldPoint(spawnPosition).walkable)
            {
                Debug.LogWarning("Tried to spawn new agent onto unwalkable space.");
                return;
            }
            else
                spawnNode = AGridRuntime.Instance.NodeFromWorldPoint(spawnPosition);
        }

        if(spawnNode == null || !spawnNode.walkable)
        {
            Debug.LogWarning("Could not find a proper node to spawn new agent onto.");
            return;
        }

        GameObject agent = (GameObject)Instantiate(agentPrefab, spawnNode.worldPosition, Quaternion.identity);

        // Set this specific instantiated agent's parameters
        UnitSimple agentAI = agent.GetComponent<UnitSimple>();
        agentAI.PopulateDictionaryFromGlobal();
        agentAI.target = target;

        foreach (KeyValuePair<string, ArchitecturalElementContainer> container in GlobalModelData.architecturalElementContainers)
        {
            if (agentAI.affinityTypes.ContainsKey(container.Key))
                agentAI.affinityTypes[container.Key].AffinityValue = Random.Range(0, 50);
        }
        //agentAI.Window = windowAffinity;
        //agentAI.Connectivity = connectivityAffinity;

        Debug.Log($"{agent.name} has target {target}.");

        DataRecorder.Instance.GenerateNewPathData();
        DataRecorder.Instance.SetCurrentPathSpawn(spawnPosition);

        // Tell agent to determine path after setting parameters
        agentAI.AgentRequestPath();
    }

    public void Spawn()
    {
        if (spawnPosition != Vector3.zero)
            SpawnAgent(spawnPosition);
        else
            Debug.Log("There is no current spawn point.");
    }

    #endregion
}
