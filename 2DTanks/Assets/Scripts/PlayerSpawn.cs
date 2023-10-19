using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum Current{
        USA,
        Germany,
        Russia
    };
public class PlayerSpawn : MonoBehaviour
{

    private CameraAim camera;
    private Transform selectionButtons;
    [SerializeField] private Transform usaGrid;
    [SerializeField] private Transform germanyGrid;
    [SerializeField] private Transform ussrGrid;
    
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
     /*map range 
     x:  Range(-52.5f, 61.5f)
     y: Range(33f,-77.7f)
    */
    public void SelectTank(GameObject tank){
       
        float xpos = Random.Range(-58.4f, 52.5f);
        float ypos = Random.Range(56.2f,-57.4f);
        Vector3 spawnpos = new Vector3(xpos,ypos,0);
        Quaternion rotation;

        if(xpos < 0 && ypos > 0){//top left
            rotation = Quaternion.Euler(0,0,-140);
        }else if(xpos > 0 && ypos > 0){//top right
            rotation =Quaternion.Euler(0,0,140);
        }else if(xpos < 0 && ypos < 0){//bottom left
            rotation =Quaternion.Euler(0,0,-30);
        }else //bottom right
            rotation =Quaternion.Euler(0,0,30);

        GameObject player = Instantiate(tank,spawnpos,rotation);
        TankController controller = player.GetComponent<TankController>();
        controller.InputEnabled = true;
        camera.follow = player;
        this.gameObject.SetActive(false);
    }


}
