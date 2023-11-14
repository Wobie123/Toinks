using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;
    // Start is called before the first frame update
    void Awake()
    {

        serverButton.onClick.AddListener(() => {
            //NetworkManager.Singleton.StartServer();
            DataManager.instance.networkID = DataManager.NetworkID.Server;     
            SceneManager.LoadScene(1);
    
        });

        clientButton.onClick.AddListener(() => {
            //NetworkManager.Singleton.StartClient();
            DataManager.instance.networkID = DataManager.NetworkID.Client;
            SceneManager.LoadScene(1);
            
        });

        hostButton.onClick.AddListener(() => {
            //NetworkManager.Singleton.StartHost();
            DataManager.instance.networkID = DataManager.NetworkID.Host;
            SceneManager.LoadScene(1);
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
