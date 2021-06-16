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
	// private Model model;
	IWorker worker;
	public Text uiText;

	public Dog dog;

	float[][] onv;

	void Start() 
	{
        var model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        uiText = GetComponent<Text>(); 
        
		onv = new float[2][];
        onv[0] = new float[11880];
        onv[1] = new float[11880];
	}

	void Update()
	 {
		// Tensor input = new Tensor (1, 1, 2, 11880);

		onv = dog.getONV();



		Tensor input = new Tensor(new TensorShape(1, 1, 2, 11880), onv);

		worker.Execute(input);
		Tensor output = worker.PeekOutput();
		input.Dispose();
		worker.Dispose();
		List<float> temp = output.ToReadOnlyArray().ToList();
		uiText.text = ((int) temp[0]).ToString(); 
		
	}

	
}
