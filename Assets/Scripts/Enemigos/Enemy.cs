using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (target != null && agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            agent.SetDestination(targetPosition);
        }
    }
}
