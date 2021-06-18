using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ShortController : Agent
{

   [SerializeField] private List<Transform> center;

   public List<Rigidbody> Feet;

    public HingeJoint Abdomen, Pelvis; //-50,50,     -50,50 spring 100
    public List<HingeJoint> FThigh; //-150,60 spring 100
    public List<HingeJoint> FCalf; //-120,50, spring 100
    public List<HingeJoint> BThigh; //-90 150
    public List<HingeJoint> BCalf; //-90 90
    // Start is called before the first frame update

    public List<TouchGround> TG;


   private List<HingeJoint> Parts; //all the parts made into list, initialized at start
   List<float> initAngle;
   List<Vector3> initPos;
   List<Quaternion> initRotation;
   private double initTransformZ;
   private float originalDistance;

   private int recentRewards;

   private List<int> direction; //direction new action moves toward
   private void Start() {
      initTransformZ = center[0].transform.position.z;

      //list of parts
      Parts = new List<HingeJoint>();
      Parts.Add(Abdomen);
      Parts.Add(Pelvis);
      Parts.AddRange(FThigh);
      Parts.AddRange(FCalf);
      Parts.AddRange(BThigh);
      Parts.AddRange(BCalf);

      // list of angles
      initAngle = new List<float>();
      foreach (HingeJoint part in Parts)
         initAngle.Add(part.spring.targetPosition);
         

      // list of pos
      initPos = new List<Vector3>();
      initRotation = new List<Quaternion>();
      foreach (Transform child in transform)
      {
         initPos.Add(child.transform.localPosition);
         initRotation.Add(child.transform.localRotation);
      }
   }

    public override void OnEpisodeBegin()
    {
      //list of pos
      int i = 0;
      foreach (Transform child in transform)
      {
         child.transform.localPosition = initPos[i];
         child.transform.localRotation = initRotation[i];
         i++;
      }

      //unfreeze
      // foreach (Transform child in transform)
      //    child.GetComponent<Rigidbody>().isKinematic = false;


      //list of direction
      // direction = new List<int>();
      // for (int j = 0; j < 10; j++){
      //    if (Random.value > 0.5) direction.Add(1);
      //    else direction.Add(-1);
      // }

      recentRewards = 0;
    }

   float previousPos = 0;
    public override void CollectObservations(VectorSensor sensor)
    {

         // add position and angles
         sensor.AddObservation(center[0].localPosition);
         sensor.AddObservation(center[0].localEulerAngles);

         // sensor.AddObservation(center[1].localPosition);
         // sensor.AddObservation(center[1].localEulerAngles);

         // sensor.AddObservation(center[2].localPosition);
         // sensor.AddObservation(center[2].localEulerAngles);

         // sensor.AddObservation(center[3].localPosition);
         // sensor.AddObservation(center[3].localEulerAngles);

         // sensor.AddObservation(center[4].localPosition);
         // sensor.AddObservation(center[4].localEulerAngles);

         // sensor.AddObservation(center[5].localPosition);
         // sensor.AddObservation(center[5].localEulerAngles);

         foreach (HingeJoint part in Parts)
            sensor.AddObservation(part.spring.targetPosition);


         foreach(TouchGround tg in TG)
            sensor.AddObservation(tg.touchedGround);
         
         // foreach (float d in direction)
         //    sensor.AddObservation(d);


         //distance from start
         // double reward = (center[0].position.x-initTransformX)*1.3;
         // AddReward((int)(reward));
         // Debug.Log(reward);

         // SetReward(center.localPosition.z);
         // Debug.Log(center.localPosition.z);

    }

    private bool applyHinge(HingeJoint part, float speed, float action ,float min, float max, int id){

         JointSpring hingeSpring = part.spring;
         float current = hingeSpring.targetPosition;

         

         // if (current >)
         float newAngle = current + speed * action * 4;
         if (newAngle <= min || newAngle >= max) {
            // direction[id] *= -1;
            return false;
         }
         hingeSpring.targetPosition = newAngle;
         part.spring = hingeSpring;
         return true;

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
      // print actions
      // string a ="";
      // for (int i = 0; i < actions.ContinuousActions.Length; i++)
      //    a += actions.ContinuousActions[i] + " ";
      // Debug.Log(a);

      if (
      applyHinge(Abdomen   ,1,actions.ContinuousActions[0],-50,50,0) &&  
      applyHinge(Pelvis    ,1,actions.ContinuousActions[1],-50,50,1) &&
      applyHinge(FThigh[0] ,1,actions.ContinuousActions[2],-150,60,2) && 
      applyHinge(FThigh[1] ,1,actions.ContinuousActions[3],-150,60,3) &&
      applyHinge(FCalf[0]  ,1,actions.ContinuousActions[4],-120,50,4) &&
      applyHinge(FCalf[1]  ,1,actions.ContinuousActions[5],-120,50,5) && 
      applyHinge(BThigh[0] ,1,actions.ContinuousActions[6],-90,150,6)&& 
      applyHinge(BThigh[1] ,1,actions.ContinuousActions[7],-90,150,7)&& 
      applyHinge(BCalf[0]  ,1,actions.ContinuousActions[8],-90,90,8)&&
      applyHinge(BCalf[1]  ,1,actions.ContinuousActions[9],-90,90,9)
      ) {} else {
         // SetReward(-1f);
         EndEpisode();
         resetAngle();
         }

      //falling over
      float zAngle = center[0].localRotation.eulerAngles.z;
      float yAngle = center[0].localRotation.eulerAngles.y;
      if (zAngle < 280 && zAngle > 80){
         // Debug.Log("fell"+Mathf.Abs(center.localRotation.eulerAngles.z));
         // SetReward(-1f);
         EndEpisode();
         resetAngle();
      }
      // if (yAngle < 300 && yAngle > 60){
      //    // Debug.Log("fell"+Mathf.Abs(center.localRotation.eulerAngles.z));
      //    // SetReward(-1f);
      //    EndEpisode();
      //    resetAngle();
      // }


      // is it facing the right direction
      float correctDirection = (Vector3.Dot(center[0].forward, transform.forward) + 1) * .5F;
      float Propulsion = Vector3.Dot(center[0].GetComponent<Rigidbody>().velocity, transform.forward);
      float headConnection = center[0].localPosition.y;

      // feet reward
      float FeetPropulsion = 0;
      FeetPropulsion += Vector3.Dot(Feet[0].velocity, transform.forward);
      FeetPropulsion += Vector3.Dot(Feet[1].velocity, transform.forward)*2;
      FeetPropulsion += Vector3.Dot(Feet[2].velocity, transform.forward)*2;
      FeetPropulsion += Vector3.Dot(Feet[3].velocity, transform.forward);
      FeetPropulsion *= 0.1f;


      float reward = correctDirection*Propulsion*headConnection;
      reward += FeetPropulsion;
      print(FeetPropulsion);
      AddReward(reward);


      // if it is not improving for longer than such, restart
      recentRewards++;
      if (reward > 1) {
         print("Reset");
         recentRewards = 0;
      }
      if (recentRewards > 1000) {
         print("END");
         EndEpisode();
         resetAngle();
      }
      
    }

   void resetAngle(){
      foreach (HingeJoint Parts in Parts){
            JointSpring hingeSpring = Parts.spring;
               hingeSpring.targetPosition = 0;
               Parts.spring = hingeSpring;
      }
   }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions =  actionsOut.ContinuousActions;
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        continuousActions[2] = x;
        continuousActions[6] = y;
        continuousActions[3] = y;
        continuousActions[7] = x;

      //   Debug.Log("Heur:"+x+""+y);
    }

}
