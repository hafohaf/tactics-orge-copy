using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursormove : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private GameObject activeunit;
    // Start is called before the first frame update
    void Start()
    {
        activeunit = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = activeunit.transform.position;
        transform.position = new Vector3(playerPos.x, 0.55f, playerPos.z);
        startPosition = transform.position;

        targetPosition = transform.position;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
