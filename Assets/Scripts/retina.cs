using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class retina : MonoBehaviour
{
    // Determines how many photoreceptors to generate
    public int rho = 10;
    public int alpha = 10;

    public GameObject eyeball;      // connect the sphere model

    private Camera camera;
    private Vector3 position;

    private int numRays;
    private Ray[] rays;
    private float[] hitDistances;

    private float radius;

    private float maxDistance = 10;

    // https://stackoverflow.com/questions/218060/random-gaussian-variables
    private float normalDist(int mean, float stdDev) {
        
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
                rays[i].origin = position + offset;
                rays[i].direction = transform.forward;
            
                hitDistances[i] = maxDistance;
            }
        }

    }

    private void drawRays() {

        for (int i = 0; i < numRays; i++) {
            Debug.DrawRay(rays[i].origin, rays[i].direction * hitDistances[i], Color.red);
        }
    }

    void Start(){
        camera = GetComponent<Camera>();

        position = transform.position;

        numRays = rho * alpha;

        rays = new Ray[numRays];
        hitDistances = new float[numRays];

        radius = eyeball.GetComponent<SphereCollider>().radius;
        Debug.Log(radius);

        generateRetina();
        drawRays();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: not moving yet
        position = transform.position;

        Ray ray;
        RaycastHit hit;

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

                // TODO: optive nerve vector from this
                // Debug.Log(c);
            }
        }

        generateRetina();
        drawRays();
    }
}
