using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBot : MonoBehaviour
{
    public Transform targets;
    private Transform turrent;
    private TankController tankController;
    public float rotationModifier;
    private int turrentSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
        tankController = gameObject.GetComponent<TankController>();
        if(tankController.HasNoTurret){
            turrent = this.transform;
            
        }else
            turrent = transform.GetChild(0);
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
    }
}
