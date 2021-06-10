using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// overall script to control the dog
// will eventually contain vision and locomotion information

public class Dog : MonoBehaviour
{
    // dog's head, will rotate
    public GameObject head;

    // target
    public GameObject bone;

    // eyes
    public bool leftRetina = true;
    public bool rightRetina = true;

    public retina left;
    public retina right;

    public GameObject leftEye;
    public GameObject rightEye;


    // Start is called before the first frame update
    void Start()
    {
        // TODO: save a distribution and keep using that to init (keep consistent)
        if (leftRetina == true)
            left.setup();
        if (rightRetina == true)
            right.setup();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: add boundaries of how much you can rotate, movement of head is weird

        // WASD, rotates head
        Vector3 rotateDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
            rotateDirection += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.A))
            rotateDirection += new Vector3(0, -1, 0);
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

        // get vector in direction of bone
        Vector3 dist = bone.transform.position - transform.position;
        Vector3 targ = leftEye.transform.forward + rightEye.transform.forward / 2;
        Vector3 dir = targ.normalized - dist.normalized;

        Vector3 deltaGaze = new Vector3(dir.x, dir.y, dist.z);

        // TODO: write this to file
        Debug.Log(deltaGaze);

        // order of operations matters? take data and update retina position
        if (leftRetina == true) {
            left.run();
            Color[] cL = left.getONV();
        }

        if (rightRetina == true) {
            right.run();
            Color[] cR = left.getONV();
        }
    }
}
