using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTarget : MonoBehaviour
{
    // Start is called before the first frame update
    public bool touchedTarget = false;

    private void OnTriggerEnter(Collider other) {
       if(other.tag == "Target") {
            touchedTarget = true;
       }
    }
    private void OnTriggerExit(Collider other) {
       if(other.tag == "Target") {
            touchedTarget = false;
       }
    }
}
