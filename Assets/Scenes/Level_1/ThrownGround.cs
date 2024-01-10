using UnityEngine;

public class ThrownGround : MonoBehaviour, IDamageTaker
{
    public float damage = 1f;
    private new Rigidbody rigidbody;
    private bool dealtDamage = false;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 target, float speed, float arc, float extraUpTime)
    {
        Debug.Log("Throw at " + target.ToString());
        rigidbody.ApplyTargetedForce(target, speed, arc, extraUpTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (!dealtDamage && other.collider.CompareTag("Player"))
        {
            other.gameObject.transform.parent.GetComponent<Player>().TakeDamage(damage);
            dealtDamage = true;
        }
    }

    public void TakeDamage(float damage)
    {
        Destroy(gameObject);
    }
}
