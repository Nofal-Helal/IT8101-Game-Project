using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUniversal : MonoBehaviour
{
    // Start is called before the first frame update

    protected bool isAlive = true;
    public float health = 100f;
    public float speed = 1f;
    public float damage;
    public float attackRange = 1f;
    public float attackSpeed = 1f;
    public float attackCooldown = 1f;
    public float timeSinceLastAttack;
    protected float rotationSpeed = 5f;

    public test_player_movement_script player;
    protected Animator animator;
    public bool isPlayerClose = false;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogError("Animation component not found");
        }
    }

    protected virtual void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
    }
    // protected virtual void MoveTowardsPlayer(Vector3 playerPosition)
    // {
    //     transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
    //     // Rotate to look at the player
    //     transform.LookAt(playerPosition);
    //     //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    // }

    protected virtual void MoveTowardsPlayer(Vector3 playerPosition)
    {
        Vector3 direction = (playerPosition - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
    }



    protected virtual void AttackPlayer(test_player_movement_script playerScript)
    {
        if (isAlive && playerScript != null && CanAttack())
        {
            playerScript.TakeDamage(damage);
            // You can add additional logic here, like playing an attack animation or sound
            //UpdateAnimatorParameters();
            //TriggerAttackAnimation();
            timeSinceLastAttack = 0f;
            isPlayerClose = true;
            //UpdateAnimatorParameters();
            //TriggerAttackAnimation();
        }
    }

    private bool CanAttack()
    {
        return timeSinceLastAttack >= attackCooldown;
    }

    protected virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void UpdateAnimatorParameters()
    {
        if(animator != null)
        {
            animator.SetBool("isPlayerCloseLogSent", isPlayerClose);
        }
    }

    private void TriggerAttackAnimation()
    {
        if(animator != null)
        {
        animator.SetTrigger("Attack 1"); // Replace with your actual trigger name
        }
    }


    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
