using UnityEngine;
using UnityEngine.AI;

public class LevaController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupUI;
    [SerializeField] private GameObject objectToMove;
    [SerializeField] private Transform playerTransform; //Lo farò diventare figlio della leva per muoverlo in automatico 
                                                        //prendo così anche la navmesh per evitare conflitti al parenting

    [Header("Movement Configurazion")]
    [SerializeField] private float pushSpeed = 2f;
    [SerializeField] private float limitZ = -5f;

    [Header("ObjectMovement")]
    [SerializeField] private float targetY = -4f;
    [SerializeField] private float DownSpeed = 1f;

    [Header("NavMesh")]
    [SerializeField] private Unity.AI.Navigation.NavMeshSurface surface;

    [Header("NPC Reaction")]
    [SerializeField] private Animator npcAnim;
    [SerializeField] private GameObject npcCanvas;

    private bool navMeshUpdated = false;
    private bool playerNear = false;
    private NavMeshAgent playerAgent;

    private void Start()
    {
        if (playerTransform != null)
        {
            playerAgent = playerTransform.GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        //Se il player è vicino e mantiene E, attiva il movimento
        if (playerNear && Input.GetKey(KeyCode.E))
        {
            PushTheObject();
        }

        //Rilascio E
        else if (Input.GetKeyUp(KeyCode.E) && playerNear)
        {
            if (playerAgent != null) playerAgent.enabled = true;
            popupUI.SetActive(true);
        }

    }

    private void PushTheObject()
    {
        //Controllo se la leva non ha raggiunto il limite
        if (transform.position.z > limitZ)
        {
            //Disabilito l'agent del player
            if (playerAgent != null) playerAgent.enabled = false;

            float leverMovement = pushSpeed * Time.deltaTime;
            float objectMovement = DownSpeed * Time.deltaTime;

            //Muovo la leva in X positivo
            transform.Translate(Vector3.back * leverMovement, Space.World);

            //Muovo il player in X positivo con la medesima velocità
            playerTransform.Translate(Vector3.back * leverMovement, Space.World);

            //Muovo l'oggetto in Y negativo se non ha ancora raggiunto il target
            if (objectToMove.transform.position.y > targetY)
            {
                Vector3 currentPos = objectToMove.transform.position;
                float newY = Mathf.MoveTowards(currentPos.y, targetY, objectMovement);
                objectToMove.transform.position = new Vector3(currentPos.x, newY, currentPos.z);
            }

            popupUI.SetActive(false); //Spengo il canvas quando mantengo E
        }

        else if (!navMeshUpdated)
        {
            FreeNPC();
        }
    }

    private void FreeNPC()
    {
        //Qui ho testato il NavMeshObstacle quindi la navmesh già lo vede come calpestabile
        //va solo rimosso l'ostacolo quindi non aggiorno la navmesh

        if (npcAnim != null)
        {
            npcAnim.SetTrigger("IsFree");
        }

        if (npcCanvas != null)
        {
            npcCanvas.SetActive(true);
            Destroy(npcAnim.gameObject, 5f);
            Destroy(npcCanvas, 5f);
        }
        Debug.Log("NPC Liberato");
    }

    #region TRIGGERS ENTER AND EXIT
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            popupUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            popupUI.SetActive(false);

            //Per sicurezza riabilito anche qui l'agent del player
            if (playerAgent != null) playerAgent.enabled = true;
        }
    }
    #endregion
}








////Calcolo la Z attuale della leva
//float zAttuale = transform.position.z;

////Controllo se la leva NON ha ancora raggiunto il limitZ
//if (zAttuale > limitZ)
//{
//    if (playerAgent != null) playerAgent.enabled = false;

//    float movement = pushSpeed * Time.deltaTime;

//    //Calcolo la nuova Z 
//    float newZ = Mathf.MoveTowards(zAttuale, limitZ, movement);

//    //Applico la nuova z alla leva
//    transform.position = new Vector3(transform.position.x, transform.position.y, newZ);

//    //Muovo il player con la leva
//    float deltaMovement = newZ - zAttuale;
//    playerTransform.Translate(0, 0, deltaMovement, Space.World);

//    //Muovo l'oggetto che mi serve
//    if (objectToMove.transform.position.y > targetY)
//    {
//        Vector3 position = objectToMove.transform.position;
//        float newY = Mathf.MoveTowards(position.y, targetY, movement);
//        objectToMove.transform.position = new Vector3(position.x, newY, position.z);
//    }

//    popupUI.SetActive(false);
//}
//else
//{
//    if (playerAgent != null) playerAgent.enabled = true;
//}