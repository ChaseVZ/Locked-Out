using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("WhiteLightReveal"))
        {
            other.gameObject.layer = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WhiteLightReveal"))
        {
            other.gameObject.layer = 4 ;
        }
    }
}
