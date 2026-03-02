using UnityEngine.AI;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //Riferimenti dei componenti
    public NavMeshAgent agent;
    public Transform player;
    private FSMController fsm;

    //Riferimento dei dati SO_EnemyData
    public SO_EnemyData stats;

    #region Patrol del cane
    [Header("Patrol Settings")]
    public bool isDog = false; //Da spuntare solo sul cane
    public Transform[] waypoint; //Inserisco i punti di navigazione
    public int currentWaypointIndex = 0;
    public float waypointThreshold = 0.2f; //Distanza minima per dire "sono arrivato" così evito un errore per tendenza allo 0
    #endregion


    [HideInInspector] public Vector3 initialPosition; //Serve publica per poter essere presa in EnemyBasicStates ma non modificabile da inspector
    [HideInInspector] public Quaternion initialRotation; //Serve publica per poter essere presa in EnemyBasicStates ma non modificabile da inspector

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fsm = GetComponent<FSMController>();
        initialPosition = transform.position; //Salva la posizione iniziale
        initialRotation = transform.rotation; //Salva la rotazione iniziale
    }

    private void Start()
    {
        // Parte in IdleState
        fsm.ChangeState(new IdleState(this, fsm));
    }

    public bool CanSeePlayer()
    {
        //Uso le stats del SO_EnemyData per leggere i valori
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > stats.viewDistance) return false;

        if (isDog) //Questa logica mi serve perchè non uso un cono (vedere poi con l'EREDITARIETà se posso cambiarla)
        {
            Vector3 playerPosLocal = transform.InverseTransformPoint(player.position);
            float halfWidth = stats.viewWidth * 0.5f;

            //Se il player è dentro i limiti del rettangolo
            if (playerPosLocal.z > 0 && playerPosLocal.z < stats.viewDistance && playerPosLocal.x > -halfWidth && playerPosLocal.x < halfWidth)
            {
                Vector3 dir = (player.position - transform.position).normalized;
                if (Physics.Raycast(transform.position + Vector3.up, dir, out RaycastHit hit, distance))
                {
                    return hit.transform == player;
                }
            }
        }



        else //Logica cono
             {
                    Vector3 dirToPlayer = (player.position - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, dirToPlayer) < stats.viewAngle / 2)
                    {
                        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, stats.viewDistance))
                        {
                            return hit.transform == player;
                        }
                    }
                }
        return false;
    }

    //Gizmo momentaneo per capire 
    private void OnDrawGizmos()
    {
        if (stats == null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stats.viewDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere (transform.position, stats.viewAngle);
        }
        return;
    }
}
