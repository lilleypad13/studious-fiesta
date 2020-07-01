using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentSpawnManager : MonoBehaviour
{
    #region Variables
    [Header("Spawn Parameters")]
    public GameObject agentPrefab; // Types of agents to spawn
    private List<Transform> spawnPoints; // Location where agents will spawn from
    public Transform spawnPointContainer; // Just the single spawnManageer parent object holding all the spawn points as children
    public Transform target; // Target passed on to agents spawned from this manager

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

    private void Awake()
    {
        spawnPoints = new List<Transform>();

        foreach (Transform child in spawnPointContainer)
        {
            spawnPoints.Add(child);
        }
    }

    private void SpawnAgent()
    {
        GameObject agent = (GameObject)Instantiate(agentPrefab, spawnPoints[0].position, spawnPoints[0].rotation);

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
        //agent.GetComponent<UnitSimple>().target = target;

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

    public void SpawnAgent(int index)
    {
        GameObject agent = (GameObject)Instantiate(agentPrefab, spawnPoints[index].position, spawnPoints[index].rotation);

        // Set this specific instantiated agent's parameters
        UnitSimple agentAI = agent.GetComponent<UnitSimple>();
        agentAI.target = target;
        agentAI.Window = windowAffinity;
        agentAI.Connectivity = connectivityAffinity;

        DataRecorder.Instance.GenerateNewPathData();
        DataRecorder.Instance.SetCurrentPathSpawn(spawnPoints[index]);

        // Tell agent to determine path after setting parameters
        agentAI.AgentRequestPath();
    }

    // SpawnAtPoint# setup this way to quickly work with buttons
    // All SpawnAtPoint# methods for button testing purposes
    public void SpawnAtPoint1()
    {
        if (CheckListLength(1))
            SpawnAgent(spawnPoints[0]);
        else
            Debug.Log("There are not enough spawn points to use this command.");
    }

    public void SpawnAtPoint2()
    {
        if(CheckListLength(2))
            SpawnAgent(spawnPoints[1]);
        else
            Debug.Log("There are not enough spawn points to use this command.");
    }

    public void SpawnAtPoint3()
    {
        if(CheckListLength(3))
            SpawnAgent(spawnPoints[2]);
        else
            Debug.Log("There are not enough spawn points to use this command.");
    }

    public void SpawnAtPoint4()
    {
        if (CheckListLength(4))
            SpawnAgent(spawnPoints[3]);
        else
            Debug.Log("There are not enough spawn points to use this command.");
    }

    public void SpawnAtPoint5()
    {
        if (CheckListLength(5))
            SpawnAgent(spawnPoints[4]);
        else
            Debug.Log("There are not enough spawn points to use this command.");
    }

    /*
     * Checks if a number of spawnPoints exist before trying to activate button to spawn an agent from this point
     */
    private bool CheckListLength(int index)
    {
        return spawnPoints.Count >= index;
    }


    #endregion
}
