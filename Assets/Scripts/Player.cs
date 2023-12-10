using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    public float health = 20f;
    public float maxHealth = 20f;
    private bool inWave;
    private RailFollower cartObject;
    public Wave nextWave;

    private void Start()
    {
        Assert.AreEqual("CartObject", transform.GetChild(0).name);
        cartObject = transform.GetChild(0).GetComponent<RailFollower>();
        nextWave ??= cartObject.NextWave;
    }

    private void Update()
    {
        if (!inWave && nextWave)
        {
            if (cartObject.distance >= nextWave.Distance)
            {
                inWave = true;
                nextWave.SpawnNextSubWave();
                _ = cartObject.railTrack.Spline.TryGetObjectData("waves", out var waves);
                _ = waves.RemoveDataPoint(nextWave.Distance);
                nextWave = cartObject.NextWave;
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
