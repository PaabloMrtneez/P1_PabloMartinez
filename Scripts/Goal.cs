using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Goal : MonoBehaviour
{
    private const string VictorySceneName = "victoria";

    private bool goalReached;

    private void Awake()
    {
        Collider goalCollider = GetComponent<Collider>();
        goalCollider.isTrigger = true;
    }

    private void OnValidate()
    {
        Collider goalCollider = GetComponent<Collider>();
        if (goalCollider != null)
            goalCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (goalReached)
            return;

        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        goalReached = true;
        SceneManager.LoadScene("victoria");
    }
}
