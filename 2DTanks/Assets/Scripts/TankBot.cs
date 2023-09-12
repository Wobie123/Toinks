using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBot : MonoBehaviour
{
    public Transform targets;
    private Transform turrent;
    private TankController tankController;
    [Tooltip("offset")]
    public float rotationModifier = 90;
    private int turrentSpeed;

    //--------------detection----------------
    public float detectionRange = 100f;
    private Transform gunTip;

    // Start is called before the first frame update
    void Start()
    {
        
        tankController = gameObject.GetComponent<TankController>();
        if(tankController.HasNoTurret){
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
        RotateTo(targets);
        
    }

    void RotateTo(Transform target){
       Vector3 vectorToTarget = target.position - turrent.position;
      
       float angle = Mathf.Atan2(vectorToTarget.y,vectorToTarget.x)* Mathf.Rad2Deg - rotationModifier;
       Quaternion q = Quaternion.AngleAxis(angle,Vector3.forward);
       turrent.rotation = Quaternion.Slerp(turrent.rotation,q,Time.deltaTime* turrentSpeed);
       if(ShouldShoot()){
            tankController.ShouldFire();
        }
    }

    public bool ShouldShoot(){
        RaycastHit2D hit;
        
        LayerMask mask = LayerMask.GetMask("Plants");//ignore plant layer
        hit = Physics2D.Raycast(gunTip.position,turrent.up,detectionRange,~mask);
        Debug.DrawRay(gunTip.position,turrent.up*detectionRange, Color.green);
        Debug.Log(hit.collider);
        if(hit.collider.gameObject.GetComponent<TankController>() != null){//check if tank
            TankController enemyTank = hit.collider.gameObject.GetComponent<TankController>();
            if(enemyTank.TankTeam != tankController.TankTeam && !tankController.CheckCamo()){
                return true;
            }
        }
        return false;
    }

    

}
