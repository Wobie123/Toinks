using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamoNet : MonoBehaviour
{
    [HideInInspector]public float setTimer = 0;
    private float length = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.GetComponent<TankController>() != null &&  !col.gameObject.GetComponent<TankController>().CheckCamo()){
            setTimer+= Time.deltaTime;
            if(setTimer >= length){
                col.gameObject.GetComponent<TankController>().ChangeUI(true);
                setTimer = 0;
            }
        }

        if(col.gameObject.GetComponent<TankController>() != null &&col.gameObject.GetComponent<TankController>().PushingForce != 0){
            col.gameObject.GetComponent<TankController>().ChangeUI(false);
        }

    }

}
