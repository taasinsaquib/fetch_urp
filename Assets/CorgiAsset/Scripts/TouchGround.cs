using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchGround : MonoBehaviour
{
    // Start is called before the first frame update
    public bool touchedGround = false;
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.name == "Ground") {
            // Debug.Log("Ground Touched");
            touchedGround = true;
        }
    }
    private void OnCollisionExit(Collision other) {
        if (other.gameObject.name == "Ground") {
            // Debug.Log("Ground Left");
            touchedGround = false;
        }
    }
}
