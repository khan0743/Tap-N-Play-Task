using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    public Transform target; 
    private NavMeshAgent agent; 
    private Animator animator; 
    public bool isWalking, buying; 
    public float randomRadius;
    public int buyAmount;

    public event Action OnBuy;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); 
        animator = GetComponent<Animator>(); 
        SetDestination(); 
    }

    void Update()
    {
        if (agent.remainingDistance < 0.1f) 
        {
            if(target == null)
                SetDestination(); 
            else{
                isWalking = false;
                agent.isStopped = true; 
                transform.rotation = target.rotation;

                if (buying && OnBuy != null)
                {
                    OnBuy.Invoke();
                }
            }
        }

        UpdateAnimation(); 
    }

    public void SetDestination()
    {
        agent.isStopped = false;
        if (target == null) 
        {
            Vector3 targetPosition = RandomNavmeshLocation(randomRadius); 
            agent.SetDestination(targetPosition); 
            isWalking = true; 
        }
        else{
            agent.SetDestination(target.position);
        }
    }

    Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius; 
        randomDirection += transform.position; 
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas); 
        return hit.position; 
    }

    void UpdateAnimation()
    {
        if (isWalking)
        {
            animator.SetBool("run", true); 
        }
        else 
        {
            animator.SetBool("run", false); 
        }
    }
}
