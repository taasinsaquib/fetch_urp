using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MyScript : Agent
{

    [SerializeField] private Transform targetTransform;
    private Vector3 init;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero + Vector3.up * 0.3f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        print(transform.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0]+actions.ContinuousActions[2]+actions.ContinuousActions[4]+actions.ContinuousActions[6]+actions.ContinuousActions[8];
        float moveZ = actions.ContinuousActions[1]+actions.ContinuousActions[3]+actions.ContinuousActions[5]+actions.ContinuousActions[7]+actions.ContinuousActions[9];

        float moveSpeed = 2f;
        transform.localPosition += new Vector3(moveX,0,moveZ) *Time.deltaTime * moveSpeed;

        // Debug.Log(actions.ContinuousActions[0]);
        // base.OnActionReceived(actions);
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions =  actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "wall") {
            SetReward(-1f);
            EndEpisode();
        }
        if (other.tag == "goal") {
            SetReward(1f);
            EndEpisode();
        }
    }
}
