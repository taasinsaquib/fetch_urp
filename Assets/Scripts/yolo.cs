using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class yolo : MonoBehaviour
{

    // https://github.com/Third-Aurora/UnityBarracudaImageClassification/blob/main/Assets/Scripts/Classification.cs
    // https://docs.unity3d.com/Packages/com.unity.barracuda@1.0/manual/Worker.html
    
    public NNModel modelAsset;
    private Model m_RuntimeModel;

    // Start is called before the first frame update
    void Start()
    {
        m_RuntimeModel = ModelLoader.Load(modelAsset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
