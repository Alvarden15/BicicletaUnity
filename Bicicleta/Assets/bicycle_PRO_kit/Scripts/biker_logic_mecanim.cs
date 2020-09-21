// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;
using Lean.Pool;

public class biker_logic_mecanim : MonoBehaviour
{

    private Animator myAnimator;

    // variables for turn IK link off for a time
    private int IK_rightWeight = 1;
    private int IK_leftWeight = 1;
    //for legs
    // variables for turn IK link off for a time
    private float IK_rightLegWeight = 1;
    private int IK_leftLegWeight = 1;

    //variables for moving right/left and forward/backward
    private float bikerLeanAngle = 0.0f;
    private float bikerMoveAlong = 0.0f;

    //variables for moving reverse animation
    private float reverseSpeed = 0.0f;

    // point of head tracking for(you may disable it or put it on any object you want - the rider will look on that object)
    private Transform lookPoint;

    // standard point of interest for a head
    public Transform camPoint;

    // this point may be throwed to anything you want rider looking at. Find another Way to make it universal.
    //public Transform poi01;//for example now it's a gameObject "car". It means when rider near a "car"(distanceToPoi = 50 meters) he will look at "car" until he move far than 50 meters.

    //the distance rider will look for POI(point of interest)
    public float distanceToPoi; //in meters = 50 by default

    // variables for hand IK joint points
    public Transform IK_rightHandTarget;
    public Transform IK_leftHandTarget;
    //for legs
    // variables for hand IK joint points
    public Transform IK_rightLegTarget;
    public Transform IK_leftLegTarget;

    //ragdoll define (El Ragdoll que reemplaza al jugador al chocarse).
    public GameObject ragDoll;

    //variable for only one ragdoll create when crashed
    private bool ragdollLaunched = false;

    //fake joint for physical movement biker to imitate inertia
    public Transform fakeCharPhysJoint;
    //Transform fakeRotate;

    //we need to know bike we ride on
    public GameObject bikeRideOn;
    //why it's here ? because you might use this script with many bikes in one scene

    private bicycle_code bikeStatusCrashed;// making a link to corresponding bike's script
    //why it's here ? because you might use this script with many bikes in one scene

    [SerializeField]
    GameObject ctrlHub;// gameobject with script control variables 


    controlHub outsideControls;// making a link to corresponding bike's script

    Rigidbody cuerpoBicicleta;

    GameObject cuerpoRagdoll;

    GameObject cuerpoJugador;

    bool esPrincipal;
    private bool _isfakeCharPhysJointNotNull;

    /*
     *      Transform RDpilotHips = ragDoll.transform.Find("root/Hips");
     *      Transform pilotChest = transform.Find("root/Hips/Spine/Chest");
            Transform pilotHead = transform.Find("root/Hips/Spine/Chest/Neck/Head");
            Transform pilotLeftArm = transform.Find("root/Hips/Spine/Chest/lShoulder/lArm");
            Transform pilotLeftForeArm = transform.Find("root/Hips/Spine/Chest/lShoulder/lArm/lForearm");
            Transform pilotRightArm = transform.Find("root/Hips/Spine/Chest/rShoulder/rArm");
            Transform pilotRightForeArm = transform.Find("root/Hips/Spine/Chest/rShoulder/rArm/rForearm");
            Transform pilotLeftUpperLeg = transform.Find("root/Hips/lUpperLeg");
            Transform pilotLeftLeg = transform.Find("root/Hips/lUpperLeg/lLeg");
            Transform pilotRightUpperLeg = transform.Find("root/Hips/rUpperLeg");
            Transform pilotRightLeg = transform.Find("root/Hips/rUpperLeg/rLeg");

    Para la lista:
        0: Hips (Raiz)
        1: Pecho
        2: Cabeza
        3: Brazo Izquierdo
        4: Antebrazo Izquierdo (dentro de brazo)
        5: Brazo derecho
        6: Antebrazo Derecho (dentro de brazo)
        7: Muslo izquierdo
        8: Pierna izquierda
        9: Muslo derecho
        10: Pierna derecha

    [SerializeField]
    private Transform[] partesJugador;

    [SerializeField]
    private Transform[] partesRagdoll;
     */

    private Transform[] transformsRagdolls;

    private Transform[] transformsJugador;

    private void Start()
    {
        _isfakeCharPhysJointNotNull = fakeCharPhysJoint != null;

        //ctrlHub = GameObject.Find("gameScenario");
        
        cuerpoJugador = transform.GetChild(1).GetChild(0).gameObject;

        //Debug.Log(cuerpoJugador.name);
        //link to GameObject with script "controlHub"
        outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one


        myAnimator = GetComponent<Animator>();
        lookPoint = camPoint;//use it if you want to rider look at anything when riding

        //need to know when bike crashed to launch a ragdoll
        bikeStatusCrashed = bikeRideOn.GetComponent<bicycle_code>();
        myAnimator.SetLayerWeight(2, 0); //to turn off layer with reverse animation which override all other
        cuerpoBicicleta = bikeRideOn.GetComponent<Rigidbody>();

        esPrincipal = bikeRideOn.CompareTag("Player");

        transformsRagdolls = ragDoll.GetComponentsInChildren<Transform>();

        transformsJugador = gameObject.GetComponentsInChildren<Transform>();

    }

    //fundamental mecanim IK script
    //just keeps hands on wheelbar :)
    private void OnAnimatorIK(int layerIndex)
    {
        if (IK_rightHandTarget != null)
        {
            myAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, IK_rightWeight);
            myAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, IK_rightWeight);
            myAnimator.SetIKPosition(AvatarIKGoal.RightHand, IK_rightHandTarget.position);
            myAnimator.SetIKRotation(AvatarIKGoal.RightHand, IK_rightHandTarget.rotation);
        }
        if (IK_leftHandTarget != null)
        {
            myAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IK_leftWeight);
            myAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, IK_leftWeight);
            myAnimator.SetIKPosition(AvatarIKGoal.LeftHand, IK_leftHandTarget.position);
            myAnimator.SetIKRotation(AvatarIKGoal.LeftHand, IK_leftHandTarget.rotation);
        }
        if (IK_rightLegTarget != null)
        {
            myAnimator.SetIKPositionWeight(AvatarIKGoal.RightFoot, IK_rightLegWeight);
            myAnimator.SetIKRotationWeight(AvatarIKGoal.RightFoot, IK_rightLegWeight);
            myAnimator.SetIKPosition(AvatarIKGoal.RightFoot, IK_rightLegTarget.position);
            myAnimator.SetIKRotation(AvatarIKGoal.RightFoot, IK_rightLegTarget.rotation);
        }
        if (IK_leftLegTarget != null)
        {
            myAnimator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, IK_leftLegWeight);
            myAnimator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, IK_leftLegWeight);
            myAnimator.SetIKPosition(AvatarIKGoal.LeftFoot, IK_leftLegTarget.position);
            myAnimator.SetIKRotation(AvatarIKGoal.LeftFoot, IK_leftLegTarget.rotation);
        }
        //same for a head
        myAnimator.SetLookAtPosition(lookPoint.transform.position);
        myAnimator.SetLookAtWeight(0.35f);//0.5f - means it rotates head 50% mixed with real animations
    }

    private void Update()
    {
        //moves character with fake inertia
        if (_isfakeCharPhysJointNotNull)
        {
            var tmp_cs1 = transform.localEulerAngles;
            tmp_cs1.x = fakeCharPhysJoint.localEulerAngles.x;
            tmp_cs1.y = fakeCharPhysJoint.localEulerAngles.y;
            //tmp_cs1.z = fakeCharPhysJoint.localEulerAngles.z;
            transform.localEulerAngles = tmp_cs1;
        }
        else return;

        //the character should play animations when player press control keys
        //horizontal movement
        if (outsideControls.Horizontal < 0 && bikerLeanAngle > -1.0f)
        {
            bikerLeanAngle = bikerLeanAngle -= 8 * Time.deltaTime;//8 - "magic number" of speed of pilot's body movement across. Just 8 - face it :)
            if (bikerLeanAngle < outsideControls.Horizontal) bikerLeanAngle = outsideControls.Horizontal;//this string seems strange but it's necessary for mobile version
            myAnimator.SetFloat("lean", bikerLeanAngle);//the character play animation "lean" for bikerLeanAngle more and more
        }
        if (outsideControls.Horizontal > 0 && bikerLeanAngle < 1.0f)
        {
            bikerLeanAngle = bikerLeanAngle += 8 * Time.deltaTime;
            if (bikerLeanAngle > outsideControls.Horizontal) bikerLeanAngle = outsideControls.Horizontal;
            myAnimator.SetFloat("lean", bikerLeanAngle);
        }
        //vertical movement
        if (outsideControls.Vertical > 0 && bikerMoveAlong < 1.0f)
        {
            bikerMoveAlong = bikerMoveAlong += 3 * Time.deltaTime;
            if (bikerMoveAlong > outsideControls.Vertical) bikerMoveAlong = outsideControls.Vertical;
            myAnimator.SetFloat("moveAlong", bikerMoveAlong);
        }
        if (outsideControls.Vertical < 0 && bikerMoveAlong > -1.0f)
        {
            bikerMoveAlong = bikerMoveAlong -= 3 * Time.deltaTime;
            if (bikerMoveAlong < outsideControls.Vertical) bikerMoveAlong = outsideControls.Vertical;
            myAnimator.SetFloat("moveAlong", bikerMoveAlong);
        }

        //pilot's mass tranform movement
        if (outsideControls.HorizontalMassShift < 0 && bikerLeanAngle > -1.0f)
        {
            bikerLeanAngle = bikerLeanAngle -= 6 * Time.deltaTime;
            if (bikerLeanAngle < outsideControls.HorizontalMassShift) bikerLeanAngle = outsideControls.HorizontalMassShift;
            myAnimator.SetFloat("lean", bikerLeanAngle);
        }
        if (outsideControls.HorizontalMassShift > 0 && bikerLeanAngle < 1.0f)
        {
            bikerLeanAngle = bikerLeanAngle += 6 * Time.deltaTime;
            if (bikerLeanAngle > outsideControls.HorizontalMassShift) bikerLeanAngle = outsideControls.HorizontalMassShift;
            myAnimator.SetFloat("lean", bikerLeanAngle);
        }
        if (outsideControls.VerticalMassShift > 0 && bikerMoveAlong < 1.0f)
        {
            bikerMoveAlong = bikerMoveAlong += 3 * Time.deltaTime;
            if (bikerLeanAngle > outsideControls.VerticalMassShift) bikerLeanAngle = outsideControls.VerticalMassShift;
            myAnimator.SetFloat("moveAlong", bikerMoveAlong);
        }
        if (outsideControls.VerticalMassShift < 0 && bikerMoveAlong > -1.0f)
        {
            bikerMoveAlong = bikerMoveAlong -= 3 * Time.deltaTime;
            if (bikerLeanAngle < outsideControls.VerticalMassShift) bikerLeanAngle = outsideControls.VerticalMassShift;
            myAnimator.SetFloat("moveAlong", bikerMoveAlong);
        }

        //in a case of restart

        if (outsideControls.restartBike)
        {
            //delete ragdoll when restarting scene
            //GameObject RGtoDestroy = GameObject.Find("char_ragDoll(Clone)");
            LeanPool.Despawn(cuerpoRagdoll);
            //Destroy(RGtoDestroy);
            //make character visible again
            //Transform riderBodyVis = transform.Find("root/Hips");
            cuerpoJugador.SetActive(true);
            //now we can crash again
            ragdollLaunched = false;
        }

        //function for avarage rider pose
        bikerComeback();

        //in case of crashed call ragdoll
        //Debe haber una forma más eficiente de implementar esto.
        //if (bikeRideOn.transform.name.Equals("rigid_bike"))
        if(esPrincipal)
        {
            if (bikeStatusCrashed.crashed && !ragdollLaunched)
            {
                createRagDoll();
            }
        }

        //scan do rider see POI
        /*
        if (poi01.gameObject.activeSelf && distanceToPoi > Vector3.Distance(transform.position, poi01.transform.position))
        {
            lookPoint = poi01;
            //if not - still looking forward for a rigidbody POI right before bike
        }
        else lookPoint = camPoint;
        */
        


        // pull leg(s) down when bike stopped
        float legOffValue = 0.0f;
        if (Mathf.Round((cuerpoBicicleta.velocity.magnitude * 3.6f) * 10) * 0.1f <= 5 && !bikeStatusCrashed.isReverseOn)
        {//no reverse speed
            reverseSpeed = 0.0f;
            myAnimator.SetFloat("reverseSpeed", reverseSpeed);

            if (bikeRideOn.transform.localEulerAngles.z <= 15 || bikeRideOn.transform.localEulerAngles.z >= 345)
            {
                if (bikeRideOn.transform.localEulerAngles.x <= 10 || bikeRideOn.transform.localEulerAngles.x >= 350)
                {
                    legOffValue = (5 - (Mathf.Round((cuerpoBicicleta.velocity.magnitude * 3.6f) * 10) * 0.1f)) / 5;//need to define right speed to begin put down leg(s)
                    myAnimator.SetLayerWeight(3, legOffValue);//leg is no layer 3 in animator
                    IK_rightLegWeight = 0;
                }
            }

        }
        else
        {

            if (IK_rightLegWeight < 1)
            {
                IK_rightLegWeight = IK_rightLegWeight + 0.01f;
            }
        }
        //when using reverse speed
        if (Mathf.Round((cuerpoBicicleta.velocity.magnitude * 3.6f) * 10) * 0.1f <= 5 && bikeStatusCrashed.isReverseOn)
        {//reverse speed

            myAnimator.SetLayerWeight(3, legOffValue);
            myAnimator.SetLayerWeight(2, 1); //to turn on layer with reverse animation which override all other

            //turn off IK link when want legs animatuion
            IK_rightLegWeight = 0;
            IK_leftLegWeight = 0;

            reverseSpeed = bikeStatusCrashed.bikeSpeed / 3;
            myAnimator.SetFloat("reverseSpeed", reverseSpeed);
            if (reverseSpeed >= 1.0f)
            {
                reverseSpeed = 1.0f;
            }

            myAnimator.speed = reverseSpeed;
        }
        else
        if (Mathf.Round((cuerpoBicicleta.velocity.magnitude * 3.6f) * 10) * 0.1f > 5)
        {
            reverseSpeed = 0.0f;
            myAnimator.SetFloat("reverseSpeed", reverseSpeed);
            myAnimator.SetLayerWeight(3, legOffValue);
            myAnimator.SetLayerWeight(2, 0); //to turn off layer with reverse animation which override all other
            myAnimator.speed = 1;

            ////turn on IK link when want legs animatuion
            IK_rightLegWeight = 1;
            IK_leftLegWeight = 1;
        }


    }

    private void bikerComeback()
    {
        if (outsideControls.Horizontal == 0 && outsideControls.HorizontalMassShift == 0)
        {
            if (bikerLeanAngle > 0)
            {
                bikerLeanAngle = bikerLeanAngle -= 6 * Time.deltaTime;//6 - "magic number" of speed of pilot's body movement back across. Just 6 - face it :)
                myAnimator.SetFloat("lean", bikerLeanAngle);
            }
            if (bikerLeanAngle < 0)
            {
                bikerLeanAngle = bikerLeanAngle += 6 * Time.deltaTime;
                myAnimator.SetFloat("lean", bikerLeanAngle);
            }
        }
        if (outsideControls.Vertical == 0 && outsideControls.VerticalMassShift == 0)
        {
            if (bikerMoveAlong > 0)
            {
                bikerMoveAlong = bikerMoveAlong -= 2 * Time.deltaTime;//3 - "magic number" of speed of pilot's body movement back along. Just 3 - face it :)
                myAnimator.SetFloat("moveAlong", bikerMoveAlong);
            }
            if (bikerMoveAlong < 0)
            {
                bikerMoveAlong = bikerMoveAlong += 2 * Time.deltaTime;
                myAnimator.SetFloat("moveAlong", bikerMoveAlong);
            }
        }
    }
    //creating regdoll(we need to scan every bone of character when crashed and copy that preset to created ragdoll)

    //Reempaza todos los Find por acceso por array (O por buscar child)
    //Reemplaza esto por ultimo por un sistema de Ragdoll aparte y eliminas esto
    void createRagDoll()
    {
        if (!ragdollLaunched)
        {
            /*
            
            //Instantiate(ragDoll, currentPilotPosition, currentPilotRotation);
            // new empty varables to fill it with learned angles later
            
             */

            //hiding the rider

            /*
             *Vector3 currentPilotPosition = transform.position;
            Quaternion currentPilotRotation = transform.rotation;
             *
             */
            Vector3 currentPilotPosition = transform.position;
            Quaternion currentPilotRotation = transform.rotation;
            cuerpoJugador.SetActive(false);
            // creating ragdoll

            //Reemplaza con LeanPool.
            
            cuerpoRagdoll = LeanPool.Spawn(ragDoll, currentPilotPosition, currentPilotRotation);
            //Instantiate(ragDoll, currentPilotPosition, currentPilotRotation);
            

            for (int i = 0; i < transformsJugador.Length; i++)
            {
                transformsRagdolls[i].localRotation = transformsJugador[i].localRotation;
            }
            ragdollLaunched = true;

            //if (bikeRideOn.transform.name == "rigid_bike" && !bikeStatusCrashed.crashed)
            if (!esPrincipal || bikeStatusCrashed.crashed) return; //check for crahsed status
            bikeStatusCrashed.crashed = true;
            bikeStatusCrashed.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.2f, 0);
        }
    }
    public void PlayA(string stunt)
    {
        switch (stunt)
        {
            case "bannyhope":
                myAnimator.SetTrigger("bunnyHop");
                ; break;

            case "manual":
                myAnimator.SetTrigger("manual");
                ; break;

            case "backflip360":
                myAnimator.SetTrigger("backflip360");
                ; break;
            case "rightflip180":
                myAnimator.SetTrigger("rightflip180");
                ; break;
        }

        /*
        if (stunt == "bannyhope")
        {
            myAnimator.SetTrigger("bunnyHop");
        }
        if (stunt == "manual")
        {
            //myAnimator.SetFloat("moveAlong", -1);
            myAnimator.SetTrigger("manual");
        }
        if (stunt == "backflip360")
        {
            //myAnimator.SetFloat("moveAlong", -1);
            myAnimator.SetTrigger("backflip360");
        }
        if (stunt == "rightflip180")
        {
            //myAnimator.SetFloat("moveAlong", -1);
            myAnimator.SetTrigger("rightflip180");
        }
         
         */


    }
}