using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isActivated = false;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isAlive = true;
    public float health = 100f;
    public float speed = 1f;
    public float damage;
    public float attackRange = 1f;
    public float attackSpeed = 1f;
    public float attackCooldown = 1f;
    public float timeSinceLastAttack;
    protected float rotationSpeed = 5f;
    private GameObject player;
    private bool isPlayerClose = false;
    protected Animator animator;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void Update()
    {
        if (!isActivated)
        {
            return;
        }
        var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > attackRange)
        {
            isMoving = true;
            MoveTowardsPlayer(player.transform.position);
        }
        else if (distanceToPlayer <= attackRange)
        {
            isMoving = false;
            Player playerData = player.GetComponent<Player>();
            AttackPlayer(playerData);
        }
    }
    protected virtual void MoveTowardsPlayer(Vector3 playerPosition)
    {
        Vector3 direction = (playerPosition - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
    }
    protected virtual void AttackPlayer(Player playerScript)
    {
        if (isAlive && playerScript != null && CanAttack())
        {
            playerScript.TakeDamage(damage);
            // You can add additional logic here, like playing an attack animation or sound
            // UpdateAnimatorParameters();
            // TriggerAttackAnimation();
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("isPlayerCloseLogSent", isPlayerClose);
    }

    private void TriggerAttackAnimation()
    {
        animator.SetTrigger("Attack 1"); // Replace with your actual trigger name
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
