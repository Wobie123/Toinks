using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamoNet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        col.gameObject.GetComponent<TankController>().ChangeUI(true);
    }

     void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " exit");
        col.gameObject.GetComponent<TankController>().ChangeUI(false);
        
    }

}
