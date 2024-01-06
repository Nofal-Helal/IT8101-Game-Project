using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Moleman : MonoBehaviour
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
    public float jumpTime = 0.95f;
    public float jumpExtraUpTime = 0.75f;
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
        // animator.Play("Sleeping");

        // Debug only
        animator.Play("Idle");
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

        if (timeInState >= 3 * (2.2f + attackCooldown + 0.01f) && cooldown > 0)
        {
            animator.ResetTrigger("Throw");
            if (Random.Range(0, 2) == 0)
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
            navMeshAgent.isStopped = true;
            animator.SetTrigger("Punch");
            player.TakeDamage(1f);
            ChangeState(State.Retreat);
        }
    }

    /// <summary>
    /// Performs transitions to states, e.g. changing animation
    /// </summary>
    /// <param name="toState">state to change to</param>
    void ChangeState(State toState)
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
                damageCounter = 0;
                break;
            case State.Retreat:
                StartCoroutine(JumpBack());
                break;
        }

        timeInState = 0;
        state = toState;
    }

    IEnumerator JumpBack()
    {
        rigidbody.drag = 0; // set drag to 0 to not resist the force
        yield return new WaitForSeconds(1.9f); // wait for jump in animation
        navMeshAgent.enabled = false; // disable nav agent to not interfere with physics
        rigidbody.LaunchTo(spot1, jumpTime, jumpExtraUpTime); // apply force
        yield return new WaitForSeconds(jumpTime - 0.01f); // wait before jump ends 
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
        rigidbody.LaunchTo(spot, jumpTime * 0.8f, jumpExtraUpTime * 0.8f); // apply force
        yield return new WaitForSeconds(jumpTime - 0.01f); // wait before jump ends 
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

        if (timeInState >= 4 * (jumpTime + 0.84444444f + dodgeCooldown) && inSpot == 1)
        {
            ChangeState(State.MeleeAttack);
        }
    }

    public void TakeDamage(float damage) {
        health -= damage;
        damageCounter += damage;
        if (health <= 0) {
            /// TODO: DIE
        }
    }

}


static class RigidbodyExtension
{
    public static void LaunchTo(this Rigidbody rigidbody, Transform target, float t, float up_t = 0)
    {
        Vector3 toTarget = target.position - rigidbody.position;
        // Convert from time-to-hit to a launch velocity:
        Vector3 velocity = toTarget / t - Physics.gravity * (t + up_t) / 2f;
        rigidbody.AddForce(velocity, ForceMode.Impulse);
    }


}
