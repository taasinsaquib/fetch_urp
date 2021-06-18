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

    private Vector3 lastGaze = new Vector3(0, 0, 0);

    private float turnSpeed = 50f;

    // NN inference
    private float[][] onv;

    public float[][] getONV() {

        var onvCopy = new float[2][];
        onvCopy[0] = new float[11880];
        onvCopy[1] = new float[11880];

        onv[0].CopyTo(onvCopy[0], 0);
        onv[1].CopyTo(onvCopy[1], 0);

        return onvCopy;
    }

    private void printONV(string eye, Vector3 deltaGaze, Color[] c) {
        // if deltaGaze is outside a range, deltaGaze is 0's in data

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
        if (leftRetina == true)
            left.setup();
        if (rightRetina == true)
            right.setup();


        onv = new float[2][];
        onv[0] = new float[11880];
        onv[1] = new float[11880];
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: add boundaries of how much you can rotate, movement of head is weird

        // WASD, rotates head
        float rotateAngle = turnSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            head.transform.Rotate(Vector3.right, -rotateAngle);

        if (Input.GetKey(KeyCode.A))
            head.transform.Rotate(Vector3.up, -rotateAngle);

        if (Input.GetKey(KeyCode.S))
            head.transform.Rotate(Vector3.right, rotateAngle);

        if (Input.GetKey(KeyCode.D))
            head.transform.Rotate(Vector3.up, rotateAngle);
        
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

        // if (deltaGaze == lastGaze)
            // Debug.Log("Samw");
        // else {
            Debug.Log(deltaGaze);

            // left.run();
            // Color[] cL = left.getONV();
            // printONV("L", deltaGaze, cL);

            // right.run();
            // Color[] cR = right.getONV();
            // printONV("R", deltaGaze, cR);
        // }
        
        // write every 5 frames or something?
        // order of operations matters? take data and update retina position
        if (leftRetina == true) {
            left.run();
            
            // && deltaGaze != lastGaze
            // if (deltaGaze != lastGaze) {
            //     Color[] cL = left.getONV();
            //     printONV("L", deltaGaze, cL);
            // }
        }

        if (rightRetina == true) {
            right.run();
            
            // if (deltaGaze != lastGaze) {
            //     Color[] cR = right.getONV();
            //     printONV("R", deltaGaze, cR);
            // }
        }

        // for inference with NN
        // float[] leftONV = left.getONVFloat();
        // float[] rightONV = right.getONVFloat();

        // leftONV.CopyTo(onv[0], 0);
        // rightONV.CopyTo(onv[1], 0);

        // update for next frame
        lastGaze = deltaGaze;
    }
}
