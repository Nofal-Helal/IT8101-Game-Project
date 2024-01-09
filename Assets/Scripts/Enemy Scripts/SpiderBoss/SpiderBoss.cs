using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBoss : MonoBehaviour, IDamageTaker
{
    private float maxHealth = 300;
    private float health = 300;
    private SpiderBossState state;
    private Animation spiderAnimation;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        spiderAnimation = GetComponent<Animation>();
        state = SpiderBossState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            state = SpiderBossState.Death;
            StartCoroutine(Die());
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
    }
    public IEnumerator Die()
    {
        spiderAnimation.Play("Death");
        yield return new WaitForSeconds(spiderAnimation.clip.averageDuration);
        Destroy(gameObject);
    }
    public void SpawnMinions()
    {
        spiderAnimation.Play("Attack");
    }
}

public enum SpiderBossState
{
    Idle,
    Attack,
    Attack_Left,
    Attack_Right,
    Walk,
    Run,
    Death
}