using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showTooltip : MonoBehaviour
{
    [SerializeField] public Tooltip type;

    private void OnTriggerEnter(Collider other)
    {
        TooltipManager.instance.showTooltipObj(type);
    }
}
