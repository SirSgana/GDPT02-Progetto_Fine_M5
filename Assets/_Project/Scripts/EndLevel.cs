using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class EndLevel : MonoBehaviour
{

    [SerializeField] private GameObject canvasEndLevel;
    [SerializeField] private float awaitTime = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("HaiVinto");
            StartCoroutine(SequenceEndLevel());
        }
    }

    IEnumerator SequenceEndLevel()
    {
        if (canvasEndLevel != null)
        {
            canvasEndLevel.SetActive(true);
        }

        yield return new WaitForSeconds(awaitTime);

        SceneManager.LoadScene(0);
    }
}
