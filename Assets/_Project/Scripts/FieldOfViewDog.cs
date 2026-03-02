using System.Collections;
using UnityEngine;

public class FieldOfViewDog : MonoBehaviour
{
    private Mesh mesh;
    private EnemyController enemy;

    [Header("Settings")]
    [SerializeField] private int rayCount = 50;         //Numero di raggi emessi
    [SerializeField] private float heightOffset = 0.2f; //Variabile per mantenere un altezza consona nelle equazioni ed evitare anche il Z Conflit 
    public LayerMask layerMask;                         //Layer degli ostacoli. Non inserito nel FOV Bear per provare la differenza
    private const int indicesPerQuad = 6;

    void Start()
    {
        mesh = new Mesh();                                   //Crea una nuova meshFilter per non intaccare l'originale
        mesh.name = "DogRectangularMesh";                    //Nome della nuova mesh
        GetComponent<MeshFilter>().mesh = mesh;              //Collego la mesh creata al componente MeshFilter dell'oggetto (assicuro l'utilizzo della nuova mesh)
        enemy = GetComponentInParent<EnemyController>();     //Riferimento al controller per le stats

        StartCoroutine(UpdateMeshCoroutine());               //Utilizzo la coroutine per evitare il sovraccarico del raycast e del gioco
    }

    private IEnumerator UpdateMeshCoroutine()
    {
        while (true)
        {
            DrawMeshRectangular();

            yield return new WaitForSeconds(0.1f);
        }
    }

    void DrawMeshRectangular()
    {
        // Recupero i dati dal SO_EnemyData
        float boxWidth = enemy.stats.viewWidth;    //Larghezza totale rettangolo
        float distance = enemy.stats.viewDistance; //Lunghezza totale rettangolo
        float halfWidth = boxWidth * 0.5f;         //Metà larghezza per centrare il rettangolo in negativo e positivo

        //Definisco gli estremi negativo e positivo, inizio e fine. Fuori dal ciclo for per ottimizzazione
        Vector3 localLeft = Vector3.left * halfWidth;
        Vector3 localRight = Vector3.right * halfWidth;
        Vector3 forwardDistance = Vector3.forward * distance;
        Vector3 heightVec = Vector3.up * heightOffset;

        //Ogni raggio ha un inizio ed una fine quindi raggio * 2
        Vector3[] vertices = new Vector3[rayCount * 2];

        //Ogni segmento tra due raggi è un quadrato (in pratica 2 triangoli da 3 punti = 6 indici)
        int[] triangles = new int[(rayCount - 1) * 6];

        for (int i = 0; i < rayCount; i++)
        {
                                                 //t è un valore da 0 a 1 che indica "a che punto sono" della larghezza
            float t = i / (float)(rayCount - 1); //Se i = 0 è l'inizio, t = 0. Se i = il numero più alto, t = 1
 
            //Calcolo la posizione del vertice
            Vector3 startLocal = Vector3.Lerp(localLeft, localRight, t) + heightVec;

            //Sostituisco il punto di start da locale a world per proiettare il raggio nello spazio 3D
            Vector3 startWorld = transform.TransformPoint(startLocal);
            Vector3 targetLocal;

            if (Physics.Raycast(startWorld, transform.forward, out RaycastHit hit, distance, layerMask))
            {
                // Se colpisce, il punto "lontano" è dove il raggio rileva l'ostacolo
                targetLocal = transform.InverseTransformPoint(hit.point);
                Debug.DrawRay(startWorld, transform.forward * hit.distance, Color.red);
            }
            else
            {
                // Se non colpisce, il punto "lontano" è alla distanza massima
                targetLocal = startLocal + forwardDistance;
                Debug.DrawRay(startWorld, transform.forward * distance, Color.green);
            }

            // Assegnazione vertici
            vertices[i] = startLocal;                //Vertice fila inizio   
            vertices[i + rayCount] = targetLocal;    //Vertice fila lontano

            // Creazione triangoli
            if (i < rayCount - 1)
            {

                int startIndex = i * indicesPerQuad;

                //Formo gli angoli del primo rettangolo
                int vNearLeft = i;
                int vNearRight = i + 1;
                int vFarLeft = i + rayCount;
                int vFarRight = i + rayCount + 1;

                // Triangolo 1
                triangles[startIndex + 0] = vNearLeft;
                triangles[startIndex + 1] = vFarLeft;
                triangles[startIndex + 2] = vNearRight;

                // Triangolo 2
                triangles[startIndex + 3] = vNearRight;
                triangles[startIndex + 4] = vFarLeft;
                triangles[startIndex + 5] = vFarRight;
            }
        }
        //Pulizia ed aggiornamento mesh con i dati calcolati
        mesh.Clear();                   //Rimuove i dati del frame precedente
        mesh.vertices = vertices;       //Applica i nuovi vertici
        mesh.triangles = triangles;     //Applica la nuova struttura dei triangoli
        mesh.RecalculateNormals();      //Ricalcola l'illuminazione sulla superficie
    }
}

//private LineRenderer lineRenderer;
//private EnemyController enemyController;

//[Header("Setup")]
//public float heightOffset = 0.1f; //Sicurezza per non farlo mai penetrare nel pavimento
//public int segments = 50; //Quanto è definita l'arco

//private void Awake()
//{
//    lineRenderer = GetComponent<LineRenderer>();
//    enemyController = GetComponent<EnemyController>();

//    lineRenderer.useWorldSpace = true;
//    lineRenderer.loop = true; //Chiude il triangolo tornando all'origine
//}

//private void LateUpdate()
//{
//    if (enemyController == null || enemyController.stats == null) return;

//    DrawFOV();
//}

//private void DrawFOV()
//{
//    float angle = enemyController.stats.viewAngle;
//    float distance = enemyController.stats.viewDistance;

//    lineRenderer.positionCount = segments + 1;

//    //Punto 0 = la posizione dell'orso
//    Vector3 origin = transform.position + Vector3.up * heightOffset;
//    lineRenderer.SetPosition(0, origin);

//    for (int i = 0; i < segments; i++)
//    {
//        float currentAngle = -angle / 2 + (angle / (segments - 1)) * i;

//        //Calcolo la direzione basata sulla rotazione dell'orso
//        Vector3 direction = Quaternion.AngleAxis(currentAngle, Vector3.up) * transform.forward;

//        Vector3 targetPoint;

//        //Creo un blocco per non far passare il line attraverso i muri
//        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
//        {
//            targetPoint = hit.point + Vector3.up * heightOffset;
//        }
//        else
//        {
//            targetPoint = origin + direction * distance;
//        }

//        lineRenderer.SetPosition(i + 1, targetPoint);
//    }
//}
























//[Header("Setup")]
//[SerializeField] private float radius;
//[SerializeField] private float angle;

//[Header("Reference")]
//[SerializeField] private GameObject playerRef;

//[Header("LayerMasks")]
//[SerializeField] private LayerMask targetMask;
//[SerializeField] private LayerMask obstacleMask;

//private bool canSeePlayer;

//private void Start()
//{
//    playerRef = GameObject.FindGameObjectWithTag("Player");
//    StartCoroutine(FOVRoutine());
//}

//private IEnumerator FOVRoutine()
//{
//    float delay = 0.2f;
//    WaitForSeconds wait = new WaitForSeconds(delay);

//    while (true)
//    {
//        yield return wait;
//        FieldOfViewCheck();
//    }
//}

//private void FieldOfViewCheck()
//{

//}













//[Header("Cone Setup")]
//[SerializeField] private Material VisionConeMaterial;
//[SerializeField] private float VisionRange;
//[SerializeField] private float VisionAngle;
//[SerializeField] private LayerMask VisionObstructingLayer;//Setto i layer che ostruiscono il cono
//[SerializeField] private int VisionConeResolution = 120;//Setto i poligoni che il cono ha per poter cambiare anche la forma

//private Mesh VisionConeMesh;
//private MeshFilter MeshFilter_;

//void Start()
//{
//    transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
//    MeshFilter_ = transform.AddComponent<MeshFilter>();
//    VisionConeMesh = new Mesh();
//    VisionAngle *= Mathf.Deg2Rad;
//}

//void Update()
//{
//    DrawVisionCone();//calling the vision cone function everyframe just so the cone is updated every frame
//}

//void DrawVisionCone()//this method creates the vision cone mesh
//{
//    int[] triangles = new int[(VisionConeResolution - 1) * 3];
//    Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
//    Vertices[0] = Vector3.zero;
//    float Currentangle = -VisionAngle / 2;
//    float angleIcrement = VisionAngle / (VisionConeResolution - 1);
//    float Sine;
//    float Cosine;

//    for (int i = 0; i < VisionConeResolution; i++)
//    {
//        Sine = Mathf.Sin(Currentangle);
//        Cosine = Mathf.Cos(Currentangle);
//        Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
//        Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
//        if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
//        {
//            Vertices[i + 1] = VertForward * hit.distance;
//        }
//        else
//        {
//            Vertices[i + 1] = VertForward * VisionRange;
//        }


//        Currentangle += angleIcrement;
//    }
//    for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
//    {
//        triangles[i] = 0;
//        triangles[i + 1] = j + 1;
//        triangles[i + 2] = j + 2;
//    }
//    VisionConeMesh.Clear();
//    VisionConeMesh.vertices = Vertices;
//    VisionConeMesh.triangles = triangles;
//    MeshFilter_.mesh = VisionConeMesh;
//}

