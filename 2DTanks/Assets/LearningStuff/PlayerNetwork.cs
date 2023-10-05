using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private TankController tankController;

    // Start is called before the first frame update
    void Start()
    {
        tankController = this.gameObject.transform.GetChild(0).GetComponent<TankController>();
        if(!IsOwner){
            tankController.InputEnabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
