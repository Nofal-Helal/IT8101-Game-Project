using UnityEngine;
using UnityEngine.Assertions;


public class Player : MonoBehaviour
{
    public float health = 20f;
    public float maxHealth = 20f;

    RailFollower cartObject;
    void Start()
    {
        Assert.AreEqual("CartObject", transform.GetChild(0).name);
        cartObject = transform.GetChild(0).GetComponent<RailFollower>();
    }

    void Update()
    {
    }

    // press o to remove obstacle
    void OnRemoveObstacle() => cartObject.OnRemoveObstacle();

}
