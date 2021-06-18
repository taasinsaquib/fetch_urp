using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Long2Controller : Agent
{

   [SerializeField] private List<Transform> center;

   public List<Rigidbody> Feet;

    public HingeJoint Abdomen, Pelvis; //-50,50,     -50,50 spring 100
    public List<HingeJoint> FThigh; //-150,60 spring 100
    public List<HingeJoint> FCalf; //-120,50, spring 100
    public List<HingeJoint> FSole; //-90,30
    public List<HingeJoint> FToe; //-60,60
    public List<HingeJoint> BThigh; //-90 150
    public List<HingeJoint> BCalf; //-90 90
    public List<HingeJoint> BSole; //-70,50
    public List<HingeJoint> BToe; //-40,0,90
    // Start is called before the first frame update

    public List<TouchGround> TG;
    public TouchTarget TT;
    public Transform Target;


   private List<HingeJoint> Parts; //all the parts made into list, initialized at start
   private List<Transform> PartsTransform;
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
      Parts.AddRange(FSole);
      Parts.AddRange(FToe);
      Parts.AddRange(BThigh);
      Parts.AddRange(BCalf);
      Parts.AddRange(BSole);
      Parts.AddRange(BToe);

      //list of parts transform
      PartsTransform = new List<Transform>();
      foreach(HingeJoint Part in Parts){
         PartsTransform.Add(Part.GetComponent<Transform>());
      }

      // list of angles
      initAngle = new List<float>();
      foreach (HingeJoint part in Parts)
         initAngle.Add(part.spring.targetPosition);
         

      // list of pos
      initPos = new List<Vector3>();
      initRotation = new List<Quaternion>();
      foreach (Transform child in PartsTransform)
      {
         initPos.Add(child.transform.localPosition);
         initRotation.Add(child.transform.localRotation);
      }
   }

    public override void OnEpisodeBegin()
    {

      //list of pos
      int i = 0;
      foreach (Transform child in PartsTransform)
      {
         child.transform.localPosition = initPos[i];
         child.transform.localRotation = initRotation[i];
         i++;
      }

      //set target position
      float theta = 0;//Random.Range(-45,45);
      float dist = Random.Range(10,15);
      Target.localPosition = Quaternion.Euler(0, theta, 0) * (transform.right * -dist) + Vector3.up * 0.8f;

      // print(theta + " "+Target.localPosition);
      //unfreeze
      // foreach (Transform child in transform)
      //    child.GetComponent<Rigidbody>().isKinematic = false;


      // list of direction
      direction = new List<int>();
      for (int j = 0; j < 18; j++){
         if (Random.value > 0.5) direction.Add(1);
         else direction.Add(-1);
      }

      recentRewards = 0;
    }

   float previousPos = 0;
    public override void CollectObservations(VectorSensor sensor)
    {

         // add position and angles
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


         //Direction of target needs to be known

         Vector3 targetDirection = (Target.localPosition-center[0].localPosition);
         targetDirection = Vector3.Normalize(new Vector3(targetDirection.x,0,targetDirection.z));
         sensor.AddObservation(targetDirection);

    }

    private bool applyHinge(HingeJoint part, float speed, float action ,float min, float max, int id){

         JointSpring hingeSpring = part.spring;
         float current = hingeSpring.targetPosition;

         

         // if (current >)
         float newAngle = current + speed * action * 10;
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
         applyHinge(FSole[0]  ,1,actions.ContinuousActions[6],-90,30,6)   &&
         applyHinge(FSole[1]  ,1,actions.ContinuousActions[7],-90,30,7)   &&
         applyHinge(FToe[0]   ,1,actions.ContinuousActions[8],-60,60,8)   &&
         applyHinge(FToe[1]   ,1,actions.ContinuousActions[9],-60,60,9)  &&
         applyHinge(BThigh[0] ,1,actions.ContinuousActions[10],-90,150,10)&& 
         applyHinge(BThigh[1] ,1,actions.ContinuousActions[11],-90,150,11)&& 
         applyHinge(BCalf[0]  ,1,actions.ContinuousActions[12],-90,90,12)&&
         applyHinge(BCalf[1]  ,1,actions.ContinuousActions[13],-90,90,13) &&
         applyHinge(BSole[0]  ,1,actions.ContinuousActions[14],-70,50,14)  &&
         applyHinge(BSole[1]  ,1,actions.ContinuousActions[15],-70,50,15)  &&
         applyHinge(BToe[0]   ,1,actions.ContinuousActions[16],-40,90,16)  &&
         applyHinge(BToe[1]   ,1,actions.ContinuousActions[17],-40,90,17) 

      ) {} else {
         // SetReward(-1f);
         EndEpisode();
         resetAngle();
         }

      //falling over
      float zAngle = center[0].localRotation.eulerAngles.z;
      float yAngle = center[0].localRotation.eulerAngles.y;
      if (zAngle < 240 && zAngle > 120){
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

      //what is the target's direction?
      Vector3 targetDirection = (Target.localPosition-center[0].localPosition);
      targetDirection = Vector3.Normalize(new Vector3(targetDirection.x,0,targetDirection.z));


      // is it facing the right direction
      float correctDirection = (Vector3.Dot(center[0].forward, targetDirection) + 1) * .5F;
      float Propulsion = Vector3.Dot(center[0].GetComponent<Rigidbody>().velocity, transform.forward);
      float bodyOrientation = Vector3.Dot(center[1].up,transform.up);
      float headConnection = center[0].localPosition.y;

      // feet reward
      float FeetPropulsion = 0;
      FeetPropulsion += Vector3.Dot(Feet[0].velocity, transform.forward);
      FeetPropulsion += Vector3.Dot(Feet[1].velocity, transform.forward);
      FeetPropulsion += Vector3.Dot(Feet[2].velocity, transform.forward)*2;
      FeetPropulsion += Vector3.Dot(Feet[3].velocity, transform.forward)*2;
      // FeetPropulsion *= 0.1f;


      float reward = correctDirection*Propulsion*headConnection;
      reward += FeetPropulsion;
      reward += bodyOrientation * 0.1f;
      // print(FeetPropulsion);
      AddReward(reward);


      // if it is not improving for longer than such, restart
      recentRewards++;
      if (reward > 1) {
         // print("Reset");
         recentRewards = 0;
      }
      if (recentRewards > 1000) {
         // print("END");
         EndEpisode();
         resetAngle();
      }

      //target reached
      if(TT.touchedTarget){
         print("REACHED");
         AddReward(1);
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
