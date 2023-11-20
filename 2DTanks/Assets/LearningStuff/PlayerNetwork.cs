using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


//Server RPC -Sends messages to the server;
//Client RPC - Server to client
public class PlayerNetwork : NetworkBehaviour
{
    //network varriable need to be initilized on networkBehavior
    //networkVarriable(init value, read, write)
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    
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
