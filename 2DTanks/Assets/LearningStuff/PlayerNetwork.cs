using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


//Server RPC -code that is run on the server, called by a client
//Client RPC -code that is run on the client, called by the server. //from server to clients
public class PlayerNetwork : NetworkBehaviour
{
    //network varriable need to be initilized on networkBehavior
    //networkVarriable(init value, read, write)
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    //randomNumber.Value = RandomRange(0,100); // changes the value
    private TankController tankController;

    /*
    public override void OnNetwrokSpawn(){ //OnAwake for networking
        randomNumber.OnValueChanged +=(int previousValue,int newValue) =>{
            Debug.Log(OwnerClientId + "; randomNumber: " + randomNumber.Value);
        }
    }*/

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
