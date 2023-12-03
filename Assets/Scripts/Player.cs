using UnityEngine;
using UnityEngine.Assertions;


public class Player : MonoBehaviour
{
    public float health = 20f;
    public float maxHealth = 20f;
    private bool inWave;
    private RailFollower cartObject;

    private void Start()
    {
        Assert.AreEqual("CartObject", transform.GetChild(0).name);
        cartObject = transform.GetChild(0).GetComponent<RailFollower>();
    }

    private void Update()
    {
        if (cartObject.speed == 0f) {
            if (!inWave) {
                inWave = true;
                cartObject.nextObstacle?.OnStopAtObstacle();
            }
        }
    }

    // TODO: Bettor obstacle removal input
    // press o to remove obstacle
    private void OnRemoveObstacle()
    {
        cartObject.OnRemoveObstacle();
        inWave = false;
    }
}
