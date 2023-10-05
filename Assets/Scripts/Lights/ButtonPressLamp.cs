using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPressLamp : MonoBehaviour
{
    [SerializeField] GameObject lamp;
    [SerializeField] bool buttonBroken;

    AudioSource buttonPress;
    AudioSource buttonBrokenAudio;
    Animator animator;
    int isPressed;

    void Start()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        buttonPress = audios[0];
        buttonBrokenAudio = audios[1];
        animator = GetComponent<Animator>();
        isPressed = Animator.StringToHash("Pressed");
    }

    public void interact()
    {
        if (!animator.GetBool(isPressed) && !buttonBroken)
        {
            animator.SetBool(isPressed, true);
            lamp.GetComponent<ButtonLampToggle>().Toggle();
            buttonPress.Play();
        }

        if (buttonBroken && !buttonBrokenAudio.isPlaying) { buttonBrokenAudio.Play(); } 
    }
}
