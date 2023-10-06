using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine;

public class TankBot : MonoBehaviour
{
    private GameObject wayPointObj;
    public Transform targets;//change this to list
    private Transform turrent;
    private TankController tankController;
    [Tooltip("offset")]
    public float rotationModifier = 90;
    private float turrentSpeed;
    private bool initChangeTarget = false;//to make change target not called too much
    

    //--------------detection----------------
    public float detectionRange = 30f;
    private Transform gunTip;

    private LayerMask objectLayer;

    private int detectionIndex = 0;//circular loop


    //--------------movement--------------------

    /*map range 
     x:  Range(-52.5f, 61.5f)
     y: Range(33f,-77.7f)
    */

    public bool moveable = true;
    private float traverseSpeed;
    private float movementSpeed;
    
    private Transform wayPoint;
    private Rigidbody2D rb;


    [Header("Read Only")]
    //replace with targets
    public Collider2D[] detectedTargets;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wayPointObj = new GameObject("Waypoint");
        wayPoint = wayPointObj.transform;
        objectLayer = LayerMask.GetMask("Physics Model");
        tankController = gameObject.GetComponent<TankController>();
        if(tankController.HasNoTurret){//tank destroyer
            turrent = this.transform;
            gunTip = transform.GetChild(0).GetChild(1);
            
        }else{
            turrent = transform.GetChild(0);
            gunTip = turrent.GetChild(2);
        }

        turrentSpeed = (float)tankController.TurretTraverseSpeed/55;//reduce speed
        traverseSpeed = (float)tankController.TraverseSpeed/55;
        movementSpeed = (float)tankController.TopSpeed/5;
        ChangeMovement();//set a waypoint
    }

    // Update is called once per frame
    void Update()
    {
       
        if(tankController == null)  Destroy(gameObject.GetComponent<TankBot>());//gone

        TurrentControls(targets);

        //does not move if have target or disabled
        if(moveable && targets == null){
            Movement();
        }

    }

    private void TurrentControls(Transform target){
        if(target == null){
            Detection(detectionIndex);
            return;
        }
        
       RotateTo(turrent,target,turrentSpeed);
       
        //-------------------------shooting---------------------------------------
        RaycastHit2D hit;
        
        LayerMask mask = LayerMask.GetMask("Plants");
        hit = Physics2D.Raycast(gunTip.position,turrent.up,detectionRange,~mask);
        Debug.DrawRay(gunTip.position,turrent.up*detectionRange, Color.green);

        if(hit.collider != null){//hit something
          
            if(hit.collider.gameObject.GetComponent<TankController>() != null){//check if tank
                TankController enemyTank = hit.collider.gameObject.GetComponent<TankController>();
             
                if(enemyTank.TankTeam != tankController.TankTeam && !enemyTank.CheckCamo()){
                    tankController.ShouldFire();
                    StopAllCoroutines();
                    StartCoroutine(DetectionDelay(2f));
                   
                }
            
            }
            if(hit.collider.gameObject.GetComponent<TankController>() == null){
                if(initChangeTarget){
                    initChangeTarget = false;
                    ChangeTarget();
                }
               
            } 

        }
    }

    private void ChangeTarget(){
        StopAllCoroutines();
        StartCoroutine(DetectionDelay(3f));
    }
    //decide if should shoot
    

    private void Detection(int i){//getting target
        detectedTargets = Physics2D.OverlapCircleAll(transform.position, detectionRange, objectLayer);
        if(detectedTargets.Length > 1){//make sure there is another object;
            if(detectedTargets[i].gameObject.GetComponent<TankController>() != null){//has the controller
                TankController enemyTank = detectedTargets[i].gameObject.GetComponent<TankController>();
                if(enemyTank.TankTeam != tankController.TankTeam && !tankController.CheckCamo()){
                    targets = detectedTargets[i].gameObject.transform;
                }
            }
        }else{
            targets = null;
        }
    }

    IEnumerator DetectionDelay(float delay){
        yield return new WaitForSeconds(delay);
        detectionIndex = (detectionIndex +1 )% detectedTargets.Length;
        Detection(detectionIndex);
        initChangeTarget = true;
    }

    IEnumerator ChangeWayPointDelay(float delay){
         yield return new WaitForSeconds(delay);
         ChangeMovement();
    }

    private void OnDrawGizmos()//draws the detection radius (view on gizmos)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void ChangeMovement(){
        wayPoint.position = new Vector3(Random.Range(-52.5f, 61.5f),Random.Range(33f,-77.7f),0);
        if(targets == null){//in case stuck
            StopAllCoroutines();
            StartCoroutine(ChangeWayPointDelay(12f));
        }
    }

    private bool RotateTo(Transform myself,Transform target,float speed){
         if (target != null)
        {
            
            // Calculate the direction from the object to the target.
            Vector3 vectorToTarget = target.position - myself.position;

            // Calculate the rotation angle needed to face the target.
            float angle = Mathf.Atan2(vectorToTarget.y,vectorToTarget.x)* Mathf.Rad2Deg - rotationModifier;

            // Create a rotation based on the calculated angle.
            Quaternion targetRotation = Quaternion.AngleAxis(angle,Vector3.forward);

            // Rotate the object smoothly towards the target rotation.
            myself.rotation = Quaternion.Slerp(myself.rotation,targetRotation,speed *Time.deltaTime);
            
            if (Quaternion.Angle(myself.rotation, targetRotation) < 1.0f){
                return true; 
            }
    }
     return false;
    }
    private void Movement(){


        if (Vector2.Distance(transform.position, wayPoint.position) < 1f){
                ChangeMovement();//changes waypoint
        }
        if( RotateTo(this.transform,wayPoint,traverseSpeed))
                MoveTowards(wayPoint.position);

        
        
    }

    private void MoveTowards(Vector3 targetPosition)
    {
       
       Vector2 direction = targetPosition - transform.position;

       direction.Normalize();

       Vector2 movement = direction * movementSpeed;
        if(!tankController.tracked){
            rb.velocity = movement;
        }else{
            rb.velocity = Vector2.zero;
        }

    }
}