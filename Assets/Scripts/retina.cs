using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class retina : MonoBehaviour
{
    public int numRays = 10;

    private Camera camera;

    private Vector3 position;

    private Ray[] rays;
    private float[] hitDistances;

    private float maxDistance = 10;

    private void generateRays() {

        for (int i = 0; i < numRays; i++) {
            rays[i].origin = position;
            rays[i].direction = Quaternion.Euler(25*(-numRays/2 + i), 25*(-numRays/2 + i), 0) * transform.forward;
            
            hitDistances[i] = maxDistance;
        }

        // Mathf.Exp(6)
        // Mathf.Sin()
        // Mathf.Cos()
        // Need a normal distribution function
    }

    private void drawRays() {

        for (int i = 0; i < numRays; i++) {
            Debug.DrawRay(rays[i].origin, rays[i].direction * hitDistances[i], Color.red);
        }
    }

    void Start(){
        camera = GetComponent<Camera>();

        position = transform.position;

        rays = new Ray[numRays];
        hitDistances = new float[numRays];

        generateRays();
        Debug.Log("hi");
        for (int i = 0; i < numRays; i++) {
            Debug.Log(rays[i].direction);
        }
        Debug.Log("hi");
        drawRays();
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;

        RaycastHit hit;
        // Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Ray ray;

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

                Debug.Log(c);
                
                // Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10);
            }

        }
        
        /*
        // https://forum.unity.com/threads/trying-to-get-color-of-a-pixel-on-texture-with-raycasting.608431/
        // https://www.youtube.com/watch?v=P_nyEPAcWKE
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit)) {

            Renderer renderer = hit.transform.GetComponent<MeshRenderer>();
            Texture2D texture2D = renderer.material.mainTexture as Texture2D;
            Vector2 pCoord = hit.textureCoord;
            pCoord.x *= texture2D.width;
            pCoord.y *= texture2D.height;        

            Vector2 tiling = renderer.material.mainTextureScale;
            Color color = texture2D.GetPixel(Mathf.FloorToInt(pCoord.x * tiling.x) , Mathf.FloorToInt(pCoord.y * tiling.y));

            Debug.Log(color);

            // Transform objectHit = hit.transform;

            // Debug.Log(hit.textureCoord);   
            // Do something with the object that was hit by the raycast.
        }  
        */



        // Debug.DrawRay(position, Vector3.up, Color.yellow);
        generateRays();
        drawRays();
    }

    // https://www.youtube.com/watch?v=Nplcqwq_oJU&t=636s
    // private void OnDrawGizmosSelected() {
        // Gizmos.color = Color.red;
        // Debug.DrawLine()
    // }
    
}
