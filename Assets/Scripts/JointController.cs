using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class JointController : Agent
{

   [SerializeField] private Transform center;

    public HingeJoint Abdomen, Pelvis; //-50,50,     -50,50 spring 100
    public List<HingeJoint> FThigh; //-150,60 spring 100
    public List<HingeJoint> FCalf; //-90,50, spring 100
    public List<HingeJoint> FSole; //-90,30
    public List<HingeJoint> FToe; //-60,60
    public List<HingeJoint> BThigh; //-90 90
    public List<HingeJoint> BCalf; //-90 90
    public List<HingeJoint> BSole; //-70,50
    public List<HingeJoint> BToe; //-40,0,90
    // Start is called before the first frame update
    [SerializeField] private Transform targetTransform;


   List<float> initAngle;
   List<Vector3> initPos;

   private void Start() {
      // list of angles
      initAngle = new List<float>();
      initAngle.Add(Abdomen.spring.targetPosition);
      initAngle.Add(Pelvis.spring.targetPosition);

      // list of pos
      initPos = new List<Vector3>();
      foreach (Transform child in transform)
      {
         initPos.Add(child.transform.localPosition);
      }

   }

    public override void OnEpisodeBegin()
    {
       Debug.Log("ended");
      //list of pos
      int i = 0;
      foreach (Transform child in transform)
      {
         child.transform.localPosition = initPos[i];
         i++;
      }

      //list of angle
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(center.localPosition);
        sensor.AddObservation(targetTransform.position);
        float distance = Vector3.Distance(center.localPosition,targetTransform.localPosition);
        SetReward(-distance);
        Debug.Log(-distance);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
         float moveX = actions.ContinuousActions[0]+actions.ContinuousActions[2]+actions.ContinuousActions[4]+actions.ContinuousActions[6]+actions.ContinuousActions[8];
         float moveZ = actions.ContinuousActions[1]+actions.ContinuousActions[3]+actions.ContinuousActions[5]+actions.ContinuousActions[7]+actions.ContinuousActions[9];

         JointSpring hingeSpring = Abdomen.spring;
         hingeSpring.targetPosition += 1 * actions.ContinuousActions[0];
         Abdomen.spring = hingeSpring;
         if(hingeSpring.targetPosition < -50 || hingeSpring.targetPosition > 50 ){
            SetReward(-1f);
            EndEpisode();
            resetAngle();
            return;
         }

         hingeSpring = Pelvis.spring;
         hingeSpring.targetPosition += 1 * actions.ContinuousActions[1];
         Pelvis.spring = hingeSpring;
         if(hingeSpring.targetPosition < -50 || hingeSpring.targetPosition > 50 ){
            SetReward(-1f);
            EndEpisode();
            resetAngle();
            return;
         }

   // FThigh; //-150,60 spring 100
         hingeSpring = FThigh[0].spring;
         hingeSpring.targetPosition += 1 * actions.ContinuousActions[2];
         FThigh[0].spring = hingeSpring;
         if(hingeSpring.targetPosition < -50 || hingeSpring.targetPosition > 50 ){
            SetReward(-1f);
            EndEpisode();
            resetAngle();
            return;
         }

         hingeSpring = FThigh[1].spring;
         hingeSpring.targetPosition += 1 * actions.ContinuousActions[3];
         FThigh[1].spring = hingeSpring;
         if(hingeSpring.targetPosition < -50 || hingeSpring.targetPosition > 50 ){
            SetReward(-1f);
            EndEpisode();
            resetAngle();
            return;
         }


         // FCalf; //-90,50, spring 100
         hingeSpring = FCalf[0].spring;
         hingeSpring.targetPosition += 1 * actions.ContinuousActions[4];
         FCalf[0].spring = hingeSpring;
         if(hingeSpring.targetPosition < -90 || hingeSpring.targetPosition > 50 ){
            SetReward(-1f);
            EndEpisode();
            resetAngle();
            return;
         }

         hingeSpring = FCalf[1].spring;
         hingeSpring.targetPosition += 1 * actions.ContinuousActions[5];
         FCalf[1].spring = hingeSpring;
         if(hingeSpring.targetPosition < -90 || hingeSpring.targetPosition > 50 ){
            SetReward(-1f);
            EndEpisode();
            resetAngle();
            return;
         }


   //  public List<HingeJoint> FSole; //-90,30
         hingeSpring = FSole[0].spring;
         hingeSpring.targetPosition += 1 * actions.ContinuousActions[6];
         FSole[0].spring = hingeSpring;
         if(hingeSpring.targetPosition < -90 || hingeSpring.targetPosition > 30 ){
            SetReward(-1f);
            EndEpisode();
            resetAngle();
            return;
         }
         hingeSpring = FSole[1].spring;
         hingeSpring.targetPosition += 1 * actions.ContinuousActions[7];
         FSole[1].spring = hingeSpring;
         if(hingeSpring.targetPosition < -90 || hingeSpring.targetPosition > 30 ){
            SetReward(-1f);
            EndEpisode();
            resetAngle();
            return;
         }


    }

   void resetAngle(){
            JointSpring hingeSpring = Abdomen.spring;
               hingeSpring.targetPosition = 0;
               Abdomen.spring = hingeSpring;
            hingeSpring = Pelvis.spring;
               hingeSpring.targetPosition = 0;
               Pelvis.spring = hingeSpring;
            hingeSpring = FSole[0].spring;
               hingeSpring.targetPosition = 0;
               FSole[0].spring = hingeSpring;
            hingeSpring = FSole[1].spring;
               hingeSpring.targetPosition = 0;
               FSole[1].spring = hingeSpring;
            hingeSpring = FCalf[0].spring;
               hingeSpring.targetPosition = 0;
               FCalf[0].spring = hingeSpring;
            hingeSpring = FCalf[1].spring;
               hingeSpring.targetPosition = 0;
               FCalf[1].spring = hingeSpring;
            hingeSpring = Pelvis.spring;
               hingeSpring.targetPosition = 0;
               Pelvis.spring = hingeSpring;
            hingeSpring = FThigh[0].spring;
               hingeSpring.targetPosition = 0;
               FThigh[0].spring = hingeSpring;
            hingeSpring = FThigh[1].spring;
               hingeSpring.targetPosition = 0;
               FThigh[1].spring = hingeSpring;
   }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions =  actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

        Debug.Log("Heur:"+continuousActions);
    }
}
