using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Followed this tutorial - https://vilbeyli.github.io/Projectile-Motion-Tutorial-for-Arrows-and-Missiles-in-Unity3D
Code hosted here -       https://github.com/vilbeyli/TrajectoryTutorial/blob/master/Assets/Projectile.cs

*/

public class bone : MonoBehaviour
{
    public GameObject ground;
    public GameObject treePrefab;

    // random points will add Vector3 to this Vector3 to stay in range
    private Vector3 bottomCorner;

    public int numTrees = 10;

    public float firingAngle = 30.0f;
    public float gravity = 9.8f;

    private float groundX;
    private float groundZ;
    private Vector3 targetPos;

    // launch variables
    [SerializeField] private Transform TargetObject;
    [Range(1.0f, 6.0f)] public float TargetRadius;
    [Range(20.0f, 75.0f)] public float LaunchAngle;

    // state
    private bool bTargetReady;
    private bool bTouchingGround;

    // cache
    private Rigidbody rigid;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // for data collection purposes
    float dist = 25.0f; //distance of the plane from the camera

    // launches the object towards the TargetObject with a given LaunchAngle
    void Launch() { 
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetXZPos = new Vector3(TargetObject.position.x, 0.0f, TargetObject.position.z);
        
        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
        // float H = (TargetObject.position.y + GetPlatformOffset()) - transform.position.y;
        float H = (TargetObject.position.y) - transform.position.y;

        // calculate initial speed required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)) );
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rigid.velocity = globalVelocity;
        bTargetReady = false;
    }

    // Sets a random target around the object based on the TargetRadius
    void SetNewTarget() {
        Transform targetTF = TargetObject.GetComponent<Transform>(); // shorthand
        
        TargetObject.SetPositionAndRotation(getRandPosInBounds(), targetTF.rotation);
        bTargetReady = true;
    }

    // resets the projectile to its initial position
    void ResetToInitialState() {
        rigid.velocity = Vector3.zero;
        this.transform.SetPositionAndRotation(initialPosition, initialRotation);
        bTargetReady = false;
    }

    // returns Vector3
    Vector3 getRandPosInBounds() {
        float randX = Random.value * groundX;
        float randZ = Random.value * groundX * 2 / 3;

        return bottomCorner + new Vector3(randX, 0, randZ);
    }

    // Start is called before the first frame update
    void Start() {
        rigid = GetComponent<Rigidbody>();

        bTargetReady = true;

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // get ground plane size
        groundX = ground.GetComponent<Renderer>().bounds.size.x;
        groundZ = ground.GetComponent<Renderer>().bounds.size.z;

        // can tune the division factors later
        bottomCorner = ground.transform.position - new Vector3(groundX/2, 0, groundZ/6);

        // random trees
        for (int i = 0; i < numTrees; i++){

            GameObject tree =  Instantiate(treePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            tree.transform.localPosition = getRandPosInBounds();
        }
    }

    void OnCollisionEnter()
    {
        bTouchingGround = true;
    }

    void OnCollisionExit()
    {
        bTouchingGround = false;
    }


    // Update is called once per frame
    void Update() {
        
        // For data collection ******
        // if (Input.GetKey(KeyCode.Z)) {
        //     dist -= 4f * Time.deltaTime;
        // }
        // if (Input.GetKey(KeyCode.X)) {
        //     dist += 4f * Time.deltaTime;
        // }

        // var screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
        // transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        
        // **************************

        /*
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (bTargetReady) {
                Launch();
            }
            else {
                ResetToInitialState();
                SetNewTarget();
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            ResetToInitialState();
        }

        if (!bTouchingGround && !bTargetReady)
        {
            // update the rotation of the projectile during trajectory motion
            transform.rotation = Quaternion.LookRotation(rigid.velocity) * initialRotation;;
        }
        */

        // transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        // Debug.Log("X: " + groundX + " Y: " + groundZ);
    }
}
