using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI; 

public class AgentController : MonoBehaviour
{
   
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
