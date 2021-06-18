using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureDistance : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> Dogs;
    private List<GameObject> Scores;
    public List<TextMesh> ScoresText;
    private List <float> initx;

    private List<GameObject> MaxScores;
    public List<TextMesh> MaxScoresText;
    private List <float> max;
    void Start()
    {
        Scores = new List<GameObject>();
        ScoresText = new List<TextMesh>();
        initx = new List<float>();
    
        MaxScores = new List<GameObject>();
        MaxScoresText = new List<TextMesh>();
        max = new List<float>();

        foreach(Transform dog in Dogs){
            GameObject text = new GameObject();
            TextMesh t = text.AddComponent<TextMesh>();
            Scores.Add(text);
            ScoresText.Add(t);

            t.transform.position = dog.transform.position+Vector3.left*10+Vector3.forward*0.5f;
            t.transform.localEulerAngles = new Vector3(90, 0, 0);
            t.fontSize = 15;
            initx.Add(dog.transform.position.x);




            // max score
            GameObject MaxScore = new GameObject();
            TextMesh MaxScoreText = MaxScore.AddComponent<TextMesh>();
            MaxScores.Add(MaxScore);
            MaxScoresText.Add(MaxScoreText);
            max.Add(-10);

            MaxScoreText.transform.position = dog.transform.position+Vector3.left*20+Vector3.forward*0.5f;
            MaxScoreText.transform.localEulerAngles = new Vector3(90, 0, 0);
            MaxScoreText.fontSize = 15;
        }

    }

    // Update is called once per frame
    private int resetCount = 0;
    void FixedUpdate()
    {
        for(int i =0; i < Dogs.Count;i++){
            float dist = Dogs[i].transform.position.x-initx[i];
            ScoresText[i].text = ""+(float)(dist);
            
            if (dist > max[i]) max[i] = dist;
            MaxScoresText[i].text = ""+(float)(max[i]);
        }
        
    }
}
