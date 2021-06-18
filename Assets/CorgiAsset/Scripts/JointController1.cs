using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class JointController1 : Agent
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


   private List<HingeJoint> Parts; //all the parts made into list, initialized at start
   List<float> initAngle;
   List<Vector3> initPos;
   List<Quaternion> initRotation;
   private Vector3 initTransform;
   private float originalDistance;

   private void Start() {
      initTransform = transform.position;

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
      Debug.Log(Parts.Count);

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
       Debug.Log("begun");
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
      //list of angle
    }

    public override void CollectObservations(VectorSensor sensor)
    {

         // add position and angles
         sensor.AddObservation(center.localPosition);
         foreach (HingeJoint part in Parts)
            sensor.AddObservation(part.spring.targetPosition);


         //distance from start
         SetReward((int)(center.localPosition.z*1.3));
         Debug.Log(center.localPosition.z*1.3);

         // SetReward(center.localPosition.z);
         // Debug.Log(center.localPosition.z);

    }

    private bool applyHinge(HingeJoint part, float speed, float action ,float min, float max){
         JointSpring hingeSpring = part.spring;
         hingeSpring.targetPosition += speed * action;
         part.spring = hingeSpring;
         if(hingeSpring.targetPosition <= min || hingeSpring.targetPosition >= max ){
            return false;
         }
         return true;

    }

    public override void OnActionReceived(ActionBuffers actions)
    {

      if (
         applyHinge(Abdomen   ,1,actions.ContinuousActions[0],-50,50)   &&
         applyHinge(Pelvis    ,1,actions.ContinuousActions[1],-50,50)   &&
         applyHinge(FThigh[0] ,1,actions.ContinuousActions[2],-150,60)  &&
         applyHinge(FThigh[1] ,1,actions.ContinuousActions[3],-150,60)  &&
         applyHinge(FCalf[0]  ,1,actions.ContinuousActions[4],-90,50)   &&
         applyHinge(FCalf[1]  ,1,actions.ContinuousActions[6],-90,50)   &&
         applyHinge(FSole[0]  ,1,actions.ContinuousActions[7],-90,30)   &&
         applyHinge(FSole[1]  ,1,actions.ContinuousActions[8],-90,30)   &&
         applyHinge(FToe[0]   ,1,actions.ContinuousActions[9],-60,60)   &&
         applyHinge(FToe[1]   ,1,actions.ContinuousActions[10],-60,60)  &&
         applyHinge(BThigh[0] ,1,actions.ContinuousActions[11],-90,90)  &&
         applyHinge(BThigh[1] ,1,actions.ContinuousActions[12],-90,90)  &&
         applyHinge(BCalf[0]  ,1,actions.ContinuousActions[13],-90,90)  &&
         applyHinge(BCalf[1]  ,1,actions.ContinuousActions[14],-90,90)  &&
         applyHinge(BSole[0]  ,1,actions.ContinuousActions[15],-70,50)  &&
         applyHinge(BSole[1]  ,1,actions.ContinuousActions[16],-70,50)  &&
         applyHinge(BToe[0]   ,1,actions.ContinuousActions[17],-40,90)  &&
         applyHinge(BToe[1]   ,1,actions.ContinuousActions[10],-40,90) 
         ) {
         // //falling over
         // float zAngle = center.localRotation.eulerAngles.z;
         // if (zAngle < 280 && zAngle > 80){
         //    // Debug.Log("fell"+Mathf.Abs(center.localRotation.eulerAngles.z));
         //    SetReward(-1f);
         //    EndEpisode();
         //    resetAngle();
         // }
      } // no range past problem
      else { //gotta reset
            SetReward(-1f);
            EndEpisode();
            
            // foreach (Transform child in transform)
            //    child.GetComponent<Rigidbody>().isKinematic = true;
            

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
        continuousActions[11] = y;
        continuousActions[3] = y;
        continuousActions[12] = x;

        Debug.Log("Heur:"+x+""+y);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "ground") {
            SetReward(-1f);
            EndEpisode();
            resetAngle();
        }
    }
}
