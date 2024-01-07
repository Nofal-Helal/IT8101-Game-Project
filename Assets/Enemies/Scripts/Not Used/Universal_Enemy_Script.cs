using UnityEngine;

namespace YourGameNamespace
{
    public class Enemy : MonoBehaviour, IDamageTaker
    {
        public float health = 100f;
        public float speed = 1f;
        public float damage = 10f;
        public float attackRange = 1f;
        public float attackSpeed = 1f;

        public test_player_movement_script player;
        private bool isAlive = true;
        private Animator animator;
        private bool isPlayerClose = false;

        private void Start()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            //    if (isAlive)
            //    {
            //        GameObject player = FindPlayer();

            //        if (player != null)
            //        {
            //            HandlePlayerProximity(player);
            //        }
            //        else
            //        {

            //            Debug.Log(player);
            //        }
            //    }




            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.GetComponent<test_player_movement_script>();

                if (player == null)
                {
                    Debug.LogError("Player script not found on the player GameObject.");
                }
            }
            else
            {
                Debug.LogError("Player GameObject not found. Make sure the Player has the 'Player' tag.");
            }
        }

        private GameObject FindPlayer()
        {
            return GameObject.FindGameObjectWithTag("Player");
        }

        private void HandlePlayerProximity(GameObject player)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRange)
            {
                MoveTowardsPlayer(player.transform.position);
                isPlayerClose = true;
                UpdateAnimatorParameters();
                TriggerAttackAnimation();
                AttackPlayer(player.GetComponent<test_player_movement_script>());
            }
            else
            {
                isPlayerClose = false;
                UpdateAnimatorParameters();
                TriggerIdleBreakAnimation();
            }
        }

        private void UpdateAnimatorParameters()
        {
            animator.SetBool("isClose", isPlayerClose);
        }

        private void TriggerAttackAnimation()
        {
            animator.SetTrigger("Attack 1"); // Replace with your actual trigger name
        }

        private void TriggerIdleBreakAnimation()
        {
            animator.SetTrigger("Idle Break");
        }

        private void MoveTowardsPlayer(Vector3 playerPosition)
        {
            // Implement logic to move towards the player based on speed
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);

            // Rotate to look at the player
            transform.LookAt(playerPosition);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        private void AttackPlayer(test_player_movement_script playerScript)
        {
            if (isAlive && playerScript != null)
            {
                playerScript.takeDamage(damage);
                // You can add additional logic here, like playing an attack animation or sound
                UpdateAnimatorParameters();
                TriggerAttackAnimation();
            }
        }

        void IDamageTaker.TakeDamage(float damageAmount)
        {
            health -= damageAmount;

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isAlive = false;

            // Implement logic for enemy death, like playing a death animation or sound
            // Destroy(gameObject); // Removed for demonstration purposes
        }
    }

    public class test_player_movement_script : MonoBehaviour
    {
        public float playerHealth = 100f;

        public void takeDamage(float damageAmount)
        {
            playerHealth -= damageAmount;

            if (playerHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // Implement logic for player death, e.g., play death animation, show game over screen, etc.
            // For simplicity, we'll just destroy the player GameObject here.
            Destroy(gameObject);
        }
    }
}
