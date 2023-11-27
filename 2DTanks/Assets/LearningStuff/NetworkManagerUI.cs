using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;

    private string input = "127.0.0.1 ";
    private string portInput = "7777";

    public UnityTransport transport;
    // Start is called before the first frame update
    void Awake()
    {

        serverButton.onClick.AddListener(() => {
            //NetworkManager.Singleton.StartServer();
            transport.ConnectionData.Address = checkValid(input);
            //transport.ConnectionData.Port = UInt16.Parse(portInput);
        
            DataManager.instance.networkID = DataManager.NetworkID.Server;
            SceneManager.LoadScene(1);
    
        });

        clientButton.onClick.AddListener(() => {
            //NetworkManager.Singleton.StartClient();
            transport.ConnectionData.Address = checkValid(input);
            //transport.ConnectionData.Port = UInt16.Parse(portInput);
            DataManager.instance.networkID = DataManager.NetworkID.Client;
            SceneManager.LoadScene(1);
            
        });

        hostButton.onClick.AddListener(() => {
            //NetworkManager.Singleton.StartHost();
            transport.ConnectionData.Address = checkValid(input);
            //transport.ConnectionData.Port = UInt16.Parse(portInput);
            DataManager.instance.networkID = DataManager.NetworkID.Host;
            SceneManager.LoadScene(1);
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void readInput(string s){
        input = s;
        Debug.Log("IP: "+ input);
    }

     public void readPortInput(string s){
        portInput = s;
        Debug.Log("Port: "+ portInput);
    }

    private string checkValid(string IP){//if empty
        if(string.IsNullOrEmpty(IP)){
            Debug.Log("empty ip");
            return "127.0.0.1";
        }
        string[] splitValues = IP.Split('.');
        
        if (splitValues.Length != 4){//proper length
            Debug.Log("bad length");
            return "127.0.0.1";
        }
        for(int i = 0; i < splitValues.Length;i++){
            
            if(int.TryParse(splitValues[i], out int value)){//check int
                if(value > 255 || value < 0){//within range
                     Debug.Log("not within range");
                    return "127.0.0.1";
                }
            }else{
                 Debug.Log("not int");
                return "127.0.0.1";
            }
        }

        return IP;
    }
}
