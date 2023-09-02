using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    // this is a test agent to see if we can use dynamic navigation meshes easily
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RandomMoveNear();
    }

    void RandomMoveNear()
    {
        agent.SetDestination(new Vector3(Random.Range(0, PlanetGenerator.instance.PlanetSize), 0, Random.Range(0, PlanetGenerator.instance.PlanetSize)));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (agent)
        Gizmos.DrawSphere(agent.destination, 0.5f);
    }
}
