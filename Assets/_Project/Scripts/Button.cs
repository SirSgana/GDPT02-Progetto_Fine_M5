using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject objectToRotate;
    [SerializeField] private Vector3 rotateAmount = new Vector3(90, 0, 0);
    [SerializeField] private float duration = 1.0f;

    [Header("Navigation")]
    [SerializeField] private NavMeshSurface surface;

    private bool active = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !active)
        {
            active = true;
            Debug.Log("hai premuto il buton");
            StartCoroutine(RotateSmoothly());
        }
    }

    private IEnumerator RotateSmoothly()
    {
        if (objectToRotate == null) yield break;

        Quaternion startRotation = objectToRotate.transform.rotation;

        //Calcolo la rotazione finale sommando quella attuale alla finale
        Quaternion endRotation = startRotation * Quaternion.Euler(rotateAmount);

        float elapsed = 0;

        while (elapsed < duration)
        {
            //Interpolazione lineare tra la rotazione iniiale e finale
            objectToRotate.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        //Assicuro che la rotazione finisca alla posizione di target
        objectToRotate.transform.rotation = endRotation;
        
        if (surface != null)
        {
            Debug.Log("Aggiornamento NavMesh");
            surface.BuildNavMesh();
        }
    }
}
