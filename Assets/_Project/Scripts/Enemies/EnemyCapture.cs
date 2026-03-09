using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCapture : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            RestartLevel();
        }
    }

    private void RestartLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        Debug.Log("Player catturato! Riavvio livello");
    }
}
