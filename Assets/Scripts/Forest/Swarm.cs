using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Swarm : MonoBehaviour
{
    [SerializeField] private List<GameObject> waypoints;
    [SerializeField] private float WAYPOINT_THRESHOLD = 1.0f;

    private int waypointIndex = 0;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(waypoints[0].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // If close enough to waypoint, then advance index
        if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < WAYPOINT_THRESHOLD)
        {
            waypointIndex++;

            // Wrap index around to 0 if we reach the end of our list
            if (waypointIndex == waypoints.Count)
            {
                waypointIndex = 0;
            }

            agent.SetDestination(waypoints[waypointIndex].transform.position);
        }
    }
}
