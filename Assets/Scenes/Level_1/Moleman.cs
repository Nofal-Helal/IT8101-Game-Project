using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moleman : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Sleeping");
        GetComponent<AudioSource>().volume *= Global.monstersVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
