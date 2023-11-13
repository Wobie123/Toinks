using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public List<GameObject> ShellList = new List<GameObject>();

    //Singleton instance

    public static DataManager instance;

    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                // If no instance exists, find or create the DataManager object
                instance = FindObjectOfType<DataManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("DataManager");
                    instance = obj.AddComponent<DataManager>();
                }
            }
            return instance;
        }
    }

    public GameObject GrabShell(int index){
        return ShellList[index];
    }

    public int GrabShellIndex(GameObject obj){
        int index = 0;
        foreach(GameObject Shells in ShellList){
            if(obj.name == Shells.name){
                return index;
            }
            index++;
        }
        return -1;
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
