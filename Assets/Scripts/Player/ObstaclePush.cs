using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach to character controller object

public class ObstaclePush : MonoBehaviour
{
    public float forceMagnitude;
    string move = "Pushable";

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer(move))
        {
            Rigidbody rigidbody = hit.collider.attachedRigidbody;

            if (rigidbody != null)
            {
                Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                forceDirection.y = 0;
                forceDirection.Normalize();

                rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
            }
        }
    }
}
