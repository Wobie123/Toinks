using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAim : MonoBehaviour
{
    public GameObject follow;
    [SerializeField]private Vector3 currentPos;
    [SerializeField]private Vector3 targetPos;
    public float aimDistance = 90;
    public float aimSpeed = 1;

    private Vector3 worldPosition;

    // Start is called before the first frame update
    void Start()
    {
        currentPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(follow != null){
            Vector3 followTarget = follow.transform.position;
            followTarget.z = -10;
            currentPos = followTarget;
        }
        if ((Input.GetKey(KeyCode.Mouse1))){
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            //mousePos.z = -10;
            //get position of worldspace
            worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            
            Vector3 direction = (worldPosition - currentPos).normalized;//get unit points (circle)
            
            targetPos = new Vector3((direction.x * aimDistance)+ currentPos.x,(direction.y *aimDistance)+currentPos.y,-10);
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos,aimSpeed * Time.deltaTime);
        }else{//not aiming
            transform.position = currentPos;
        }
        

    }
}
