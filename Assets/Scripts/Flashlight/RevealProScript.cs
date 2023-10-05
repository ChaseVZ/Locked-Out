using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RevealProScript : MonoBehaviour
{
    public Material reveal;
    public Light _light;


    // Update is called once per frame
    void Update()
    {
        //if (_light.spotAngle == 1) { reveal.SetVector("_LightDirection", _light.transform.forward); }
        //else { reveal.SetVector("_LightDirection", -_light.transform.forward); }
        //if (_light.tag == "Fixed") { Debug.Log(_light.spotAngle); }

        reveal.SetVector("_LightDirection", -_light.transform.forward);
        reveal.SetVector("_LightPosition", _light.transform.position);       
        reveal.SetFloat("_LightAngle", _light.spotAngle);
    }
}
