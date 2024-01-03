using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBehaviour : MonoBehaviour
{
    private Enemy enemyController;
    // Start is called before the first frame update
    void Start()
    {
       enemyController = GetComponent<Enemy>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
