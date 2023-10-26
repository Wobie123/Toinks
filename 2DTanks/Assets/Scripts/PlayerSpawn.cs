using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public enum Current{
        USA,
        Germany,
        Russia
    };
public class PlayerSpawn : NetworkBehaviour
{

    //[SerializeField] 
    private CameraAim camera;
    private Transform selectionButtons;
    private GameObject player = null;
    private Vector3 spawnpos;
    private Quaternion rotation;
    [SerializeField] private Transform usaGrid;
    [SerializeField] private Transform germanyGrid;
    [SerializeField] private Transform ussrGrid;
    public List<GameObject> tankList = new List<GameObject>();
    

    
    private Current current;

    // Start is called before the first frame update
    void Start()
    {
        current = Current.USA;
        camera = GameObject.Find("Main Camera").GetComponent<CameraAim>();
        //selectionButtons = this.gameObject.transform.GetChild(0);
        //usaGrid = this.gameObject.transform.GetChild(1);
        //germanyGrid = this.gameObject.transform.GetChild(2);
        //ussrGrid = this.gameObject.transform.GetChild(3);
        if (IsServer)
        {
            Debug.Log("I am the host (Client ID 0).");
        }
        else if (IsClient)
        {
            Debug.Log("I am a client with Client ID " + OwnerClientId);
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void ChangeGrid(int index){
        
        switch(index){
            case 1:
                current = Current.USA;
                usaGrid.gameObject.SetActive(true);
                germanyGrid.gameObject.SetActive(false);
                ussrGrid.gameObject.SetActive(false);
                break;
            case 2:
                current = Current.Germany;
                usaGrid.gameObject.SetActive(false);
                germanyGrid.gameObject.SetActive(true);
                ussrGrid.gameObject.SetActive(false);
                break;

            case 3:
                current = Current.Russia;
                usaGrid.gameObject.SetActive(false);
                germanyGrid.gameObject.SetActive(false);
                ussrGrid.gameObject.SetActive(true);
                break;
            default:
                Debug.Log("error in switch");
                break;
        }

    }
    private Quaternion rotationlogic(float xpos, float ypos){
        Quaternion rotation;

        if(xpos < 0 && ypos > 0){//top left
            rotation = Quaternion.Euler(0,0,-140);
        }else if(xpos > 0 && ypos > 0){//top right
            rotation =Quaternion.Euler(0,0,140);
        }else if(xpos < 0 && ypos < 0){//bottom left
            rotation =Quaternion.Euler(0,0,-30);
        }else //bottom right
            rotation =Quaternion.Euler(0,0,30);
        return rotation;
    }
     /*map range 
     x:  Range(-52.5f, 61.5f)
     y: Range(33f,-77.7f)
    */
    private void SelectTank(GameObject tank){
        /*
        float xpos = Random.Range(-58.4f, 52.5f);
        float ypos = Random.Range(56.2f,-57.4f);
        Vector3 spawnpos = new Vector3(xpos,ypos,0);
       
        rotationlogic(xpos,ypos);*/

        player = tank;
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        Debug.Log(clientId + "choose a tank:"+ tank.name);
        SpawnTankServerRpc();

        this.gameObject.SetActive(false);
    }

    public GameObject FindPrefabByName(string prefabName)
    {
       
        foreach (GameObject prefab in tankList)
        {
            if (prefab.name == prefabName)
            {
                return prefab;
            }
        }

        // If the prefab is not found, return null or handle the case as needed.
        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    // request from client, grabs the clinet id and send to client
    public void SpawnTankServerRpc(ServerRpcParams serverRpcParams = default){
        Debug.Log(OwnerClientId + "called serverRPC");
        
        var clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log(clientId + "is the client id");
    
        SpawnTankLocalClientRpc(clientId);
    }

    //spawns the player and send to client
    [ServerRpc(RequireOwnership = false)]
     public void tankSetupServerRpc(ulong ClientID,string name){
        Debug.Log("within tankSetupRPC");
        //GameObject prefab = NetworkManager.Singleton.SpawnManager.GetPrefabFromHash(prefabHash);
        
        float xpos = Random.Range(-58.4f, 52.5f);
        float ypos = Random.Range(56.2f,-57.4f);
        Vector3 spawnpos = new Vector3(xpos,ypos,0);

        GameObject player = Instantiate(FindPrefabByName(name), spawnpos,rotationlogic(xpos,ypos));
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(ClientID);

        tankSetupClientRPC(ClientID,player.name);
        
    }

    [ClientRpc]
    //recive form server and send the prefab;
    public void SpawnTankLocalClientRpc(ulong ID){
        
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        Debug.Log("within SpawnSetupClientRPC");
        Debug.Log("the passed perameter is: " +ID + "clientID is "+ clientId + " spawn " + player.name);
        if(clientId == ID){
            //player = Instantiate(player,spawnpos,rotation);
            //player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            tankSetupServerRpc(ID,player.name);

        }
    }
    [ClientRpc]
    //recive from server and set up the player
    public void tankSetupClientRPC(ulong ID,string name){
        ulong clientId = NetworkManager.Singleton.LocalClientId;

        GameObject tank = GameObject.Find(name);

        Debug.Log("within tankSetupClientRPC");
        Debug.Log("the passed perameter is: " +ID + "clientID is "+ clientId + " spawn " + tank.name);
        if(clientId == ID){
            Debug.Log("control seetup for" +ID + "setting up "+tank.name);
            TankController controller = tank.GetComponent<TankController>();
            controller.InputEnabled = true;
            camera.follow = tank;
        }
        }



}