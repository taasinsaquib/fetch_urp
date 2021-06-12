using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private void printONV(string eye, Vector3 deltaGaze, Color[] c) {
        // TODO: if deltaGaze is outside a range, just write Vec of 0s
        // 0.05 box in x and y directions

        // print ONV to a file
        using (StreamWriter sw = File.AppendText("Assets/Data/onv.txt")) {

            if (eye == "L")
                sw.WriteLine(deltaGaze.ToString("F10"));
            
            for (int i = 0; i < left.getNumRays(); i++) {
                sw.Write(c[i].ToString("F10"));
                sw.Write(" ");
            }
            sw.WriteLine();
        }
    }

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

        Debug.Log(deltaGaze);
        
        // write every 5 frames or something?
        // order of operations matters? take data and update retina position
        if (leftRetina == true) {
            left.run();
            Color[] cL = left.getONV();
            // printONV("L", deltaGaze, cL);
        }

        if (rightRetina == true) {
            right.run();
            Color[] cR = right.getONV();
            // printONV("R", deltaGaze, cR);
        }
    }
}
