using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class PlayerSpawn : NetworkBehaviour
{

    //[SerializeField] 
    private CameraAim camera;
    private Transform selectionButtons;
    private GameObject player = null;
    TankController controller;
    private Vector3 spawnpos;
    private Quaternion rotation;
    private bool IsActive = false;//check if player is alive
    private TMP_Text countDownText;
    private GameObject countDown;
    private float timer = 5f;

    public Transform usaGrid;
    public Transform germanyGrid;
    public Transform ussrGrid;
    public List<GameObject> tankList = new List<GameObject>();
    
    
    // Start is called before the first frame update
    void Start()
    {
        countDown = this.gameObject.transform.GetChild(1).gameObject;
        countDownText = countDown.GetComponent<TextMeshProUGUI>();

        if(DataManager.instance.networkID == DataManager.NetworkID.Server){
            NetworkManager.Singleton.StartServer();
            countDown.SetActive(true);
            countDownText.text = "I am Server";
            transform.GetChild(0).gameObject.SetActive(false);
        }else if(DataManager.instance.networkID == DataManager.NetworkID.Client){
            NetworkManager.Singleton.StartClient();
            Debug.Log("IsClient: " + IsClient);
        }else if(DataManager.instance.networkID == DataManager.NetworkID.Host){
            NetworkManager.Singleton.StartHost();
            Debug.Log("IsHost: " + IsHost);
        }


        camera = GameObject.Find("Main Camera").GetComponent<CameraAim>();
        
    }

    // Update is called once per frame
    void Update()
    {

            if(controller == null && IsActive){//just died
                countDown.SetActive(true);

                countDownText.text = Mathf.RoundToInt(timer).ToString();
                timer -= Time.deltaTime;
                if(timer <= 0f){
                    countDown.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(true);
                    IsActive = false;
                    timer = 0f;
                }
                
        }
    }

  

    public void ChangeGrid(int index){
        
        switch(index){
            case 1:
                
                usaGrid.gameObject.SetActive(true);
                germanyGrid.gameObject.SetActive(false);
                ussrGrid.gameObject.SetActive(false);
                break;
            case 2:
                
                usaGrid.gameObject.SetActive(false);
                germanyGrid.gameObject.SetActive(true);
                ussrGrid.gameObject.SetActive(false);
                break;

            case 3:
                
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
        Debug.Log("selected " + tank);
        player = tank;
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        SpawnTankServerRpc();

        //this.gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
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
    private void SpawnTankServerRpc(ServerRpcParams serverRpcParams = default){
        Debug.Log(OwnerClientId + "called serverRPC");
        
        var clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log(clientId + "is the client id");
    
        SpawnTankLocalClientRpc(clientId);
    }

    //spawns the player and send to client
    [ServerRpc(RequireOwnership = false)]
     private void tankSetupServerRpc(ulong ClientID,string name){
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
    private void SpawnTankLocalClientRpc(ulong ID){
        
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        if(clientId == ID){
            //player = Instantiate(player,spawnpos,rotation);
            //player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            tankSetupServerRpc(ID,player.name);

        }
    }
    [ClientRpc]
    //recive from server and set up the player
    //need adjust for same tank spawn
    private void tankSetupClientRPC(ulong ID,string name){
        ulong clientId = NetworkManager.Singleton.LocalClientId;

        if(clientId == ID){
            //GameObject tank = GameObject.Find(name);
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Tank");
            foreach(GameObject tankObj in gameObjects){
                if(tankObj.name == name && tankObj.GetComponent<NetworkObject>().IsLocalPlayer){
                    
                    IsActive = true;
                    controller = tankObj.GetComponent<TankController>();
                    controller.myID = ID;
                    controller.InputEnabled = true;
                    camera.follow = tankObj;
                    return;
                }
            }
        }
    }



}