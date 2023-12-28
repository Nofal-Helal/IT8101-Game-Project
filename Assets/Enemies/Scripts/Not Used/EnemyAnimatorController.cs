using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;
    private bool isPlayerClose = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("isPlayerClose: " + isPlayerClose);
        animator.SetBool("isPlayerClose", isPlayerClose);
    }
}
