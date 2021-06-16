using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using System;

public class Classification : MonoBehaviour 
{

	public NNModel modelAsset;
	IWorker worker;
	public Text uiText;

	void Start() 
	{
        var model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        uiText = GetComponent<Text>(); 
        
	}

	

	void Update()
	 {
		Tensor input = new Tensor (1, 1, 2, 11880);
		worker.Execute(input);
		Tensor output = worker.PeekOutput();
		input.Dispose();
		worker.Dispose();
		List<float> temp = output.ToReadOnlyArray().ToList();
		uiText.text = ((int) temp[0]).ToString(); 
		
	}

	
}
