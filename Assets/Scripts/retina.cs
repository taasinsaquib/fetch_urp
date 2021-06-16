using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class retina : MonoBehaviour
{
    // Determines how many photoreceptors to generate
    public int rho = 41;
    public int alpha = 360;

    public GameObject eyeball;      // connect the sphere model

    private Camera camera;
    private Vector3 position;

    private Vector2[] noiseOffsets;
    private int numRays;
    private Ray[] rays;
    private float[] hitDistances;

    private Color[] onv;    // optic nerve vector

    private float radius;

    private float maxDistance = 10f;
    private float turnSpeed = 50f;
    
    private float normalDist(int mean, float stdDev) {
        // taken from: https://stackoverflow.com/questions/218060/random-gaussian-variables
        
        float u1 = 1.0f - Random.value; // want uniform(0,1] random doubles, but range is inclusive
        float u2 = 1.0f - Random.value;

        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                    Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        float randNormal =
                    mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

        return randNormal;
    }

    private void generateRetina() {
        // from Arjun's paper
        // are a lot of rays at the same spot? at (0,0,0)? print higher precision?

        int mean = 0;
        float var = 0.0025f;
        float stdDev = Mathf.Sqrt(var);

        for (int p = 1; p < rho; p++ ) {         // rho = 41

            float mult = radius * Mathf.Exp(p) / Mathf.Exp(rho-1);  // normalize to radius of eyeball sphere

            for (int a = 0; a < alpha; a++) {     // alpha = 360

                Vector3 direction = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);

                float rand1 = normalDist(mean, var);
                float rand2 = normalDist(mean, var);
                Vector3 noise = new Vector3(rand1, rand2, 0);

                Vector3 offset = mult * direction + noise;

                int i = p * alpha + a;

                noiseOffsets[i] = new Vector2(offset.x, offset.y);      // store offset to re-create the same retina
                
                rays[i].origin = position + offset;
                rays[i].direction = transform.forward;
            
                hitDistances[i] = maxDistance;
            }
        }
    }

    private void updateRetina(Vector3 rotateAxis, float rotateAngle) {
        for (int i = 0; i < numRays; i++) {
            rays[i].origin -= (position - transform.position);
            rays[i].direction = Quaternion.AngleAxis(rotateAngle, rotateAxis) * transform.forward;
        }
    }

    // to see the ray distribution
    private void drawRays() {
        for (int i = 0; i < numRays; i++) {
            Debug.DrawRay(rays[i].origin, rays[i].direction * hitDistances[i], Color.red);
        }
    }

    public int getNumRays() {
        return numRays;
    }

    public Color[] getONV() {
        // return optic nerve vector
        var onvCopy = new Color[numRays];

        onv.CopyTo(onvCopy, 0);

        return onvCopy;
    }

    public float[] getONVFloat() {
        // probably slows it down a good amount
        var onvCopy = new float[3*numRays];

        for (int i = 0; i < numRays; i++) {
            Color c = onv[i];

            // each pixel value
            for (int j = 0; j < 3; j++) {
                int idx = j * numRays + i;
                onvCopy[idx] = c[j];
            }
        }

        return onvCopy;
    }

    private void writeOffsets() {
        using (StreamWriter sw = new StreamWriter("Assets/Data/retinaDistribution.txt"))
            for (int i = 0; i < numRays; i++)
                sw.WriteLine(noiseOffsets[i].ToString("F10"));
    }

    private void readOffsets() {
        // use offsets from a file to initialize the retina distribution
        string path = "Assets/Data/retinaDistribution.txt";

        // Create an instance of StreamReader to read from a file.
        // The using statement also closes the StreamReader.
        using (StreamReader sr = new StreamReader(path))
        {
            string line;

            int i = 0;
            // Read and display lines from the file until the end of
            // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Replace("(", "");
                line = line.Replace(")", "");

                var cord2D = line.Split(',');

                float x = float.Parse(cord2D[0]);
                float y = float.Parse(cord2D[1]);

                rays[i].origin = position + new Vector3(x, y, 0);
                rays[i].direction = transform.forward;
                
                hitDistances[i] = maxDistance;

                i += 1;
            }

            numRays = i;
        }
    }

    public void setup() {
        camera = GetComponent<Camera>();

        position = transform.position;

        numRays = rho * alpha;

        noiseOffsets = new Vector2[numRays];
        rays = new Ray[numRays];
        hitDistances = new float[numRays];

        onv = new Color[numRays];

        radius = eyeball.GetComponent<SphereCollider>().radius;

        // generateRetina();
        // writeOffsets();

        readOffsets();
        drawRays();
    }

    public void run() {

        Ray ray;
        RaycastHit hit;

        float rotateAngle = 0;
        Vector3 rotateAxis = Vector3.forward;

        if (Input.GetKey(KeyCode.W)) {
            rotateAngle = -turnSpeed * Time.deltaTime;
            rotateAxis = Vector3.right;
        }

        if (Input.GetKey(KeyCode.A)) {
            rotateAngle = -turnSpeed * Time.deltaTime;
            rotateAxis = Vector3.up; 
        }

        if (Input.GetKey(KeyCode.S)) {
            rotateAngle = turnSpeed * Time.deltaTime;
            rotateAxis = Vector3.right; 
        }

        if (Input.GetKey(KeyCode.D)) {
            rotateAngle = turnSpeed * Time.deltaTime;
            rotateAxis = Vector3.up; 
        }

        for (int i = 0; i < numRays; i++) {
            ray = rays[i];

            // https://gist.github.com/cslroot/ddcff7c39476e19da582
            // if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit)) {
            if (Physics.Raycast(ray, out hit)) {

                hitDistances[i] = hit.distance;

                var material = hit.collider.GetComponent<Renderer>().material;
                Color c = material.color;
                
                var tex2D = material.mainTexture as Texture2D;
                if (tex2D != null) {
                    c = c * tex2D.GetPixelBilinear(hit.textureCoord[0], hit.textureCoord[1]);
                }

                // if (c != onv[i])
                //     Debug.Log("hi");

                onv[i] = c;
                // Debug.Log(c);
            }
        }

        updateRetina(rotateAxis, rotateAngle);
        drawRays();

        position = transform.position;
    }

    // *****
    // Put Start() and Update() into other functions that can be used in diff scripts

    void Start() {
        setup();
    }

    // Update is called once per frame
    void Update() {
        run();
    }

    // *****

}
