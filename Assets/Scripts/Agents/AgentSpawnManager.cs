using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentSpawnManager : MonoBehaviour
{
    #region Variables
    [Header("Spawn Parameters")]
    public GameObject agentPrefab; // Types of agents to spawn
    public Transform[] spawnPoints; // Location where agents will spawn from
    public Transform target; // Target passed on to agents spawned from this manager

    // Agent Affinities
    [Header("Affinities for Spawned Agents")]
    [Range(0, 100)]
    public int windowAffinity = 0;
    [Range(0, 100)]
    public int connectivityAffinity = 0;

    // Accessories
    [Header("UI Elements")]
    public Text agentWindowAffinityText;
    public Text agentConnectivityAffinityText;
    #endregion


    #region Unity Methods
    private void Update()
    {
        agentWindowAffinityText.text = "Current Agent Window Affinity: " + windowAffinity.ToString();
        agentConnectivityAffinityText.text = "Current Agent Connectivity Affinity: " + connectivityAffinity.ToString();

        //if (Input.GetKeyDown(KeyCode.Space))
        //    SpawnAgent();
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

    // All SpawnAtPoint# methods for button testing purposes
    public void SpawnAtPoint1()
    {
        SpawnAgent(spawnPoints[0]);
    }

    public void SpawnAtPoint2()
    {
        SpawnAgent(spawnPoints[1]);
    }

    public void SpawnAtPoint3()
    {
        SpawnAgent(spawnPoints[2]);
    }


    #endregion
}
