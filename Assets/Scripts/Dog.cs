using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// overall script to control the dog
// will eventually contain vision and locomotion information

public class Dog : MonoBehaviour
{

    // dog's head, will rotate
    public GameObject head;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: make continuous, elif makes it only one at a time right?
        // boundaries of how much you can rotate

        // WASD, rotates head
        Vector3 rotateDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
            rotateDirection += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.A))
            rotateDirection += new Vector3(0 -1, 0);
        if (Input.GetKey(KeyCode.S))
            rotateDirection += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.D))
            rotateDirection += new Vector3(0, 1, 0);

        head.transform.Rotate(rotateDirection, Space.Self);
        
        // move the dog itself
        Vector3 moveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow))
            moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.DownArrow))
            moveDirection += Vector3.back;

        if (Input.GetKey(KeyCode.LeftArrow))
            moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.RightArrow))
            moveDirection += Vector3.right;

        transform.Translate(moveDirection * Time.deltaTime);

    }
}
