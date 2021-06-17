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

	// float[][] onv;

	void Start() 
	{
        var model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        // uiText = GetComponent<UnityEngine.UI.Text>(); 
        
		// uiText.text = "Sup";
	}

	void Update()
	 {
		// (1, 1, 2, 11880) NCHW
		// (1, 2, 11880, 1) NHWC

		// Note: The native ONNX data layout is NCHW, or channels-first. Barracuda automatically converts ONNX models to NHWC layout.
		var onv = dog.getONV();
		var input = new Tensor(1, 2, 11880, 1);

		for (int i = 0; i < 2; i++)
			for (int j = 0; j < 11880; j++)
				input[0, i, j, 0] = onv[i][j];

		worker.Execute(input);
		Tensor output = worker.PeekOutput();
		
		input.Dispose();
		worker.Dispose();

		List<float> temp = output.ToReadOnlyArray().ToList();
		string message = ((float) temp[0]).ToString() + ", " + ((float) temp[1]).ToString() + ", " + ((float) temp[2]).ToString();
		Debug.Log(message);
		
		// Debug.Log(uiText.text);
	}
}
