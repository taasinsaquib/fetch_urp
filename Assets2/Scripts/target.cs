using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(3, 5), 0, Random.Range(-5, 5));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
