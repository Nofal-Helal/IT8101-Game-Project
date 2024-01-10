using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Moleman : MonoBehaviour, IDamageTaker
{
    public enum State
    {
        Idle = 0,
        Dodging,
        RangedAttack,
        MeleeAttack,
        Retreat,
    }
    public State state = State.RangedAttack;
    public float maxHealth = 1000;
    public float health = 800;
    public float runSpeed = 3;
    public float meleeAttackRange = 5.7f;
    public float jumpSpeed = 20f;
    [Range(0, 1)]
    public float arc = 0.0f;
    public float cooldown = 2;
    public float attackCooldown = 2.2f;
    public float dodgeCooldown = 2;
    public float timeInState = 0;
    public float damageCounter = 0f;

    public Player player;
    private Transform cart;
    public BossBar bossBar;
    public Animator animator;
    private NavMeshAgent navMeshAgent;
    private new Rigidbody rigidbody;
    public Transform spot1;
    public Transform spot2;
    public Transform spot3;
    public int inSpot = 1;

    [NaughtyAttributes.Button] void ChangeStateRanged() { ChangeState(State.RangedAttack); }
    [NaughtyAttributes.Button] void ChangeStateMelee() { ChangeState(State.MeleeAttack); }
    [NaughtyAttributes.Button] void ChangeStateDodge() { ChangeState(State.Dodging); }

    void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
        cart = player.transform.GetChild(0);
        Assert.AreEqual("CartObject", cart.name);
        GetComponent<AudioSource>().volume *= Global.monstersVolume;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = runSpeed;
        navMeshAgent.stoppingDistance = meleeAttackRange;
        // start in sleeping animation
        animator.Play("Sleeping");

        // Debug only
        // animator.Play("Idle");
        // Global.inputActions.gameplay.RemoveObstacle.Disable(); // disable removing the obstacle in the boss fight
    }

    void Update()
    {
        bossBar.Fill = health / maxHealth;
        switch (state)
        {
            case State.Idle: break;
            case State.Dodging: DodgingUpdate(); break;
            case State.RangedAttack: RangedAttackUpdate(); break;
            case State.MeleeAttack: MeleeAttackUpdate(); break;
            case State.Retreat: break;
            default: throw new System.NotImplementedException();
        };

        timeInState += Time.deltaTime;
    }

    void DodgingUpdate()
    {
        if (cooldown <= 0)
        {
            if (inSpot == 1)
            {
                if (Random.Range(0, 2) == 0)
                {
                    StartCoroutine(JumpToSpot(spot2));
                    inSpot = 2;
                }
                else
                {
                    StartCoroutine(JumpToSpot(spot3));
                    inSpot = 3;
                }
            }
            else
            {
                StartCoroutine(JumpToSpot(spot1));
                inSpot = 1;
            }
            cooldown = dodgeCooldown;
        }
        cooldown -= Time.deltaTime;

        // state transitions in JumpToSpot->LookAtPlayer
    }
    void RangedAttackUpdate()
    {
        if (cooldown <= 0)
        {
            animator.SetTrigger("Throw");
            // TODO: throw rock
            // Debug.Log("HYaaaaaaaah");
            cooldown = attackCooldown;
        }
        cooldown -= Time.deltaTime;

        if (timeInState >= 3 * attackCooldown && cooldown > 0)
        {
            animator.ResetTrigger("Throw");
            if (Random.Range(0,1f) < 0.5f)
            {
                ChangeState(State.MeleeAttack);
            }
            else
            {
                ChangeState(State.Dodging);
            }
        }
    }
    void MeleeAttackUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, cart.position);
        if (damageCounter > 40f)
        {
            navMeshAgent.isStopped = true;
            animator.SetTrigger("EndRun");
            ChangeState(State.Retreat);
        }
        if (distanceToPlayer <= meleeAttackRange)
        {
            if (cooldown <= 0)
            {
                navMeshAgent.isStopped = true;
                StartCoroutine(PunchAndRetreat());
                cooldown = 10000; // start the coroutine once
            }
        }
    }

    /// <summary>
    /// Performs transitions to states, e.g. changing animation
    /// </summary>
    /// <param name="toState">state to change to</param>
    public void ChangeState(State toState)
    {
        switch (toState)
        {
            case State.Dodging:
                animator.SetTrigger("Idle");
                cooldown = 0;
                break;
            case State.RangedAttack:
                animator.SetTrigger("Idle");
                cooldown = 1f;
                break;
            case State.MeleeAttack:
                navMeshAgent.isStopped = false;
                navMeshAgent.destination = cart.position;
                animator.SetTrigger("Run");
                cooldown = 0f;
                damageCounter = 0;
                break;
            case State.Retreat:
                StartCoroutine(RetreatJump());
                break;
        }

        timeInState = 0;
        state = toState;
    }

    IEnumerator PunchAndRetreat()
    {
        animator.SetTrigger("Punch");
        yield return new WaitForSeconds(9 / 30f);
        ((IDamageTaker)player).TakeDamage(1f);
        yield return new WaitForSeconds((32 - 9) / 30f);
        ChangeState(State.Retreat);
    }
    IEnumerator RetreatJump()
    {
        rigidbody.drag = 0; // set drag to 0 to not resist the force
        yield return new WaitForSeconds(0.84444444f); // wait for jump in animation
        navMeshAgent.enabled = false; // disable nav agent to not interfere with physics
        float time = rigidbody.ApplyTargetedForce(spot1.position, jumpSpeed*3f/2f, arc);
        yield return new WaitForSeconds(time); // wait until jump ends 
        rigidbody.drag = 5f; // apply drag for friction
        navMeshAgent.enabled = true;
        ChangeState(State.RangedAttack);
    }

    IEnumerator JumpToSpot(Transform spot)
    {
        animator.SetTrigger("Jump");
        rigidbody.drag = 0; // set drag to 0 to not resist the force
        yield return new WaitForSeconds(0.8444444f); // wait for jump in animation
        navMeshAgent.enabled = false; // disable nav agent to not interfere with physics
        float time = rigidbody.ApplyTargetedForce(spot.position, jumpSpeed, arc);
        yield return new WaitForSeconds(time); // wait until jump ends 
        rigidbody.drag = 8f; // apply drag for friction
        navMeshAgent.enabled = true;
        yield return LookAtPlayer();
    }

    IEnumerator LookAtPlayer()
    {

        Quaternion initial = transform.rotation;
        Quaternion target = Quaternion.LookRotation(cart.position - transform.position);
        float duration = 0.5f;
        for (float t = 0; t <= duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(initial, target, t / duration);
            yield return 0;
        }

        if (timeInState >= 4 * dodgeCooldown && inSpot == 1)
        {
            ChangeState(State.MeleeAttack);
        }
    }

    void IDamageTaker.TakeDamage(float damage)
    {
        health -= damage;
        damageCounter += damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        cart.GetComponent<RailFollower>().OnRemoveObstacle();
    }

}


static class RigidbodyExtension
{
    /// <summary>
    /// Applies an impulse that will have the rigidbody land at the specified target. The arch is the percent 
    /// of arch to provide between the min/max (0 to 1).  
    /// </summary>
    public static float ApplyTargetedForce(this Rigidbody rRigidBody, Vector3 rTarget, float rSpeed, float rArc = 0.5f, bool rUseMinSpeed = true)
    {
        float lGravity = Physics.gravity.magnitude;
        Vector3 lObjectPosition = rRigidBody.position;
        Vector3 lToTarget = rTarget - lObjectPosition;

        // Find the minimum speed to hit the target
        float lDiscriminantSqrRt = Mathf.Sqrt((lGravity * lGravity) * lToTarget.sqrMagnitude);
        float lMinSpeed = Mathf.Sqrt(lDiscriminantSqrRt) - Vector3.Dot(lToTarget, Physics.gravity);
        if (rSpeed == 0f || (rUseMinSpeed && rSpeed < lMinSpeed)) { rSpeed = lMinSpeed + 0.5f; }

        // Using the speed, our factor for reaching the target
        float b = (rSpeed * rSpeed) + Vector3.Dot(lToTarget, Physics.gravity);
        float lDiscriminant = (b * b) - (lGravity * lGravity) * lToTarget.sqrMagnitude;
        if (lDiscriminant < 0)
        {
            Debug.Log("Not enough force to reach target");
            return 0;
        }

        // Determine the min and max time it will take to reach the object.
        lDiscriminantSqrRt = Mathf.Sqrt(lDiscriminant);
        float lMinTime = Mathf.Sqrt((b - lDiscriminantSqrRt) * 2f) / lGravity;
        float lMaxTime = Mathf.Sqrt((b + lDiscriminantSqrRt) * 2f) / lGravity;

        // Determine the force based on our arc value
        Mathf.Clamp(rArc, 0f, 1f);
        float lTime = lMinTime + ((lMaxTime - lMinTime) * rArc);
        Vector3 lForce = new Vector3(lToTarget.x / lTime, (lToTarget.y / lTime) + (lTime * lGravity / 2f), lToTarget.z / lTime);

        // Apply the force
        rRigidBody.AddForce(lForce, ForceMode.Impulse);

        return lTime;
    }



}
