using UnityEngine.AI;
using UnityEngine;
using Unity.VisualScripting;

public class MovingAgentPlayer : MonoBehaviour
{
    //private LineRenderer lineRenderer;
    private NavMeshAgent agent;
    private Camera cam;
    private NavMeshPath meshPath;

    private void Awake()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        //lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("hai premuto il mouse");
            HandleAgent();
            //HandleLineRendered();
        }

        if (Input.GetButton("Fire3"))
        {
            agent.isStopped = true;
        }
    }

    private void HandleAgent()
    {
        Ray pointToRay = cam.ScreenPointToRay(Input.mousePosition); //Indica il "raggio" dalla cam al mouse

        if (Physics.Raycast(pointToRay, out RaycastHit hit))
        {
            //Con CalculatePath l'agent si calcola automaticamente il percorso da fare
            meshPath = new NavMeshPath();
            agent.CalculatePath(hit.point, meshPath);
            agent.SetDestination(hit.point);
            Debug.Log("L'agente è sulla navmesh?" + agent.isOnNavMesh);
        }
    }
}

//    private void HandleLineRendered()
//    {
//        if (agent.hasPath)
//        {
//            lineRenderer.enabled = true;
//            lineRenderer.positionCount = agent.path.corners.Length;
//            lineRenderer.SetPositions(agent.path.corners);
//        }
//        lineRenderer.enabled = false;
//    }
//}

        //if (meshPath.status == NavMeshPathStatus.PathPartial)
        //{
        //    lineRenderer.startColor = Color.yellow;
        //    lineRenderer.endColor = Color.yellow;
        //}
        //else if (meshPath.status != NavMeshPathStatus.PathComplete)
        //{
        //    lineRenderer.startColor = Color.red;
        //    lineRenderer.endColor = Color.red;
        //}
        //else if (meshPath.status == NavMeshPathStatus.PathComplete)
        //{
        //    lineRenderer.startColor = Color.green;
        //    lineRenderer.endColor = Color.green;
        //}
    