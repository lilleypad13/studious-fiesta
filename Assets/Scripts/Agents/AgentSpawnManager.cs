using System.Collections.Generic;
using UnityEngine;

public class AgentSpawnManager : Initializer
{
    #region Variables
    [SerializeField] private GameObject agentPrefab; // Types of agents to spawn

    [Header("Spawn Parameters")]
    [SerializeField] private bool isSearchingForNearestWalkableSpawn = true;
    [SerializeField] private int iterationsSearchForWalkable = 3;

    public bool AllObjectsAsSpawnTargets { get => allObjectsAsSpawnTargets; }
    [Header("Determine Spawn Points")]
    [SerializeField] private bool allObjectsAsSpawnTargets = true;

    public Vector3 SpawnPosition
    {
        get => spawnPosition;
        set
        {
            spawnPosition = value;
            Debug.Log($"Spawn position was set to the location of {value} at {value}.");
        }
    }
    private Vector3 spawnPosition;

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
    private Vector3 target;

    public Dictionary<string, Affinity> spawnerAffinityTypes = new Dictionary<string, Affinity>();

    [Header("Slider UI References")]
    public GameObject sliderContainer;
    public GameObject affinitySliderPrefab;
    #endregion


    #region Unity Methods
    public override void Initialization()
    {
        foreach (KeyValuePair<string, ArchitecturalElementContainer> container in GlobalModelData.architecturalElementContainers)
        {
            if (!spawnerAffinityTypes.ContainsKey(container.Key))
            {
                Affinity affinity = new Affinity(container.Key);
                spawnerAffinityTypes.Add(container.Key, affinity);
            }
        }

        GenerateAffinitySlidersUI();
    }

    public bool SetAffinityByKey(string key, int valeToSet)
    {
        if (spawnerAffinityTypes.ContainsKey(key))
        {
            spawnerAffinityTypes[key].AffinityValue = valeToSet;
            return true;
        }
        else
        {
            Debug.LogWarning($"Spawn Manager does not have this affinity type: {key}");
            return false;
        }
    }

    private void GenerateAffinitySlidersUI()
    {
        foreach (KeyValuePair<string, ArchitecturalElementContainer> container in GlobalModelData.architecturalElementContainers)
        {
            affinitySliderPrefab.GetComponentInChildren<SliderAgentAffinity>().key = container.Key;
            Instantiate(affinitySliderPrefab, sliderContainer.transform);
        }
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
                agentAI.affinityTypes[container.Key].AffinityValue = spawnerAffinityTypes[container.Key].AffinityValue;
        }

        Debug.Log($"{agent.name} has target {target}.");

        DataRecorder.Instance.GenerateNewPathData();
        DataRecorder.Instance.SetCurrentPathSpawn(spawnPosition);

        // Tell agent to determine path after setting parameters
        agentAI.AgentRequestPath();
    }

    // Method for outside access to spawn objects with this spawner
    public void Spawn()
    {
        if (spawnPosition != Vector3.zero)
            SpawnAgent(spawnPosition);
        else
            Debug.Log("There is no current spawn point.");
    }

    #endregion
}
