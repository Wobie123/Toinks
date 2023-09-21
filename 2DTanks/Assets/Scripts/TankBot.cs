using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBot : MonoBehaviour
{
    public Transform targets;//change this to list
    private Transform turrent;
    private TankController tankController;
    [Tooltip("offset")]
    public float rotationModifier = 90;
    private int turrentSpeed;
    private bool initChangeTarget = false;//to make change target not called too much


    //--------------detection----------------
    public float detectionRange = 30f;
    private Transform gunTip;

    public LayerMask objectLayer;

    private int detectionIndex = 0;//circular loop

    [Header("Read Only")]
    //replace with targets
    public Collider2D[] detectedTargets;

    // Start is called before the first frame update
    void Start()
    {
        
        tankController = gameObject.GetComponent<TankController>();
        if(tankController.HasNoTurret){//tank destroyer
            turrent = this.transform;
            gunTip = transform.GetChild(0).GetChild(1);
            
        }else{
            turrent = transform.GetChild(0);
            gunTip = turrent.GetChild(2);
        }
        turrentSpeed = tankController.TurretTraverseSpeed/20;
       
    }

    // Update is called once per frame
    void Update()
    {
       
        if(tankController == null)  Destroy(gameObject.GetComponent<TankBot>());//gone

        RotateTo(targets);
        
    }

    private void RotateTo(Transform target){
        if(target == null){
            Detection(detectionIndex);
            Debug.Log("switch1");
            return;
        }

       Vector3 vectorToTarget = target.position - turrent.position;
      
       float angle = Mathf.Atan2(vectorToTarget.y,vectorToTarget.x)* Mathf.Rad2Deg - rotationModifier;
       Quaternion q = Quaternion.AngleAxis(angle,Vector3.forward);
       turrent.rotation = Quaternion.Slerp(turrent.rotation,q,Time.deltaTime* turrentSpeed);
       
        //-------------------------shooting---------------------------------------
        RaycastHit2D hit;
        
        LayerMask mask = LayerMask.GetMask("Plants");
        hit = Physics2D.Raycast(gunTip.position,turrent.up,detectionRange,~mask);
        Debug.DrawRay(gunTip.position,turrent.up*detectionRange, Color.green);

        if(hit.collider != null){//hit something
          
            if(hit.collider.gameObject.GetComponent<TankController>() != null){//check if tank
                TankController enemyTank = hit.collider.gameObject.GetComponent<TankController>();
             
                if(enemyTank.TankTeam != tankController.TankTeam && !tankController.CheckCamo()){
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
        StartCoroutine(DetectionDelay(4f));
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}