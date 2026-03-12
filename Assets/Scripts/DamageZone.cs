using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private readonly HashSet<PlayerController> playersInContact = new HashSet<PlayerController>();

    private void OnTriggerEnter(Collider other)
    {
        HandleEnter(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleExit(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleEnter(collision.collider.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        HandleExit(collision.collider.gameObject);
    }

    private void OnDisable()
    {
        playersInContact.Clear();
    }

    private void HandleEnter(GameObject otherObject)
    {
        PlayerController player = otherObject.GetComponentInParent<PlayerController>();
        if (player == null)
            return;

        if (!playersInContact.Add(player))
            return;
        player.ReceiveDamage();
    }

    private void HandleExit(GameObject otherObject)
    {
        PlayerController player = otherObject.GetComponentInParent<PlayerController>();
        if (player == null)
            return;

        playersInContact.Remove(player);
    }
}
