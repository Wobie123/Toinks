using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CapController : MonoBehaviour
{
    /*      Description:
     *      
     *      This script is a base for a base capturing system.
     *      
     *      This is not a complete system, so for example the boolean `BaseCaptured` is never set true.
     *      You can use this script as a foundation for base capturing system - if you want to have one in you game.
     *      
     *      This script detects all tanks (GameObjects with TankController script attached to them) in the capture area
     *      If there's only one tank the capture area, the amount of capture points in the TankController script will be increased.   
     */


    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                Settings                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(10)]
    [Header("Settings")]
    [Space(10)]

    [Tooltip("The amount of capture points given each second when the base is being captured.")]
    public int CapSpeed;

    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                              GameObjects                                                                 
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(10)]
    [Header("GameObjects")]
    [Space(10)]

    [Tooltip("The sprite that changes its color when the base is being captured.")]
    public SpriteRenderer CapturedSprite;

    [Tooltip("The alarm sound that is played when the base is being captured.")]
    public AudioSource CapSound;

    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                              UI                                                                
    //------------------------------------------------------------------------------------------------------------------------------------------------
    [Space(10)]
    [Header("UI")]
    [Space(10)]

    private GameObject canvas;
    [Tooltip("UI/CapUI")]
    public string UiPath;
    [Tooltip("Add a empty gameobject position on the canvas worldspace or screenspace")]
    public GameObject UiTarget;
    private GameObject capUi;
    private CanvasGroup mainUICanvas;
    private Text teamNameText;
    private Text pointsText;
    private Slider pointsSlider;

    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                              Options                                                                 
    //------------------------------------------------------------------------------------------------------------------------------------------------
    [Space(10)]
    [Header("Options")]
    [Space(10)]

    [Tooltip("The total amount of points needed to win")]
    public int totalPoints;
    //points gained on base cap;
    public float points;



    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                               Variables                                                                 
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(10)]
    [Header("Read-Only Variables")]
    [Space(10)]

    [SerializeField]private bool BaseCaptured;

    [SerializeField]private Color CapSpriteColor;

    [SerializeField]private int tanksInCap;

    private float lightIntensity;
    [SerializeField]private float multiplier;

    public List<TankController> capturingTankController = new List<TankController>();

    private TankTeamEnum team;
    

    //____________________________________________START_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________


    void Start ()
    {
        tanksInCap = 0;
        BaseCaptured = false;

         if ( canvas == null)
                canvas = GameObject.Find("Canvas");

        // Priting error message to the console if UI is enabled but no canvas is assigned or no canvas is not found
        if (canvas == null)
            Debug.LogWarning("ERROR: You need to assign a Canvas GameObject");

        if(UiTarget != null){//setup
            capUi = Instantiate(Resources.Load("2D Tank Controller/" + UiPath, typeof(GameObject)), UiTarget.transform.position, Quaternion.identity, UiTarget.transform.parent.transform) as GameObject;
            mainUICanvas = capUi.GetComponent<CanvasGroup>();
            teamNameText = capUi.transform.Find("Slider").Find("TeamNameText").GetComponent<Text>();
            pointsText = capUi.transform.Find("Slider").Find("PointsText").GetComponent<Text>();
            pointsSlider = capUi.transform.Find("Slider").GetComponent<Slider>();

             mainUICanvas.alpha = 0f;
             pointsSlider.maxValue = totalPoints;
             pointsSlider.value = 0;

        }
    }




    //____________________________________________EVENTS_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________
    

    // Tank goes into the cap area
    private void OnTriggerEnter(Collider collision)
    {
        tanksInCap++;
        capturingTankController.Add(collision.GetComponentInParent<TankController>());
        //reset just if take damage before
        collision.GetComponentInParent<TankController>().ResetWasHit();
        // Playing alarm sound if only one tank in cap
        if (tanksInCap == 1)
        {
            CapSound.Play();
            mainUICanvas.alpha = 0.8f; 
            team = capturingTankController[0].TankTeam;
            teamNameText.text = team.ToString();
            int pointInInt = (int)points;
            pointsText.text = pointInInt.ToString() + "/" + totalPoints.ToString();
        }            
    }


    // Tank leaves the cap area
    private void OnTriggerExit(Collider collision)
    {
        tanksInCap--;
        for(int i = capturingTankController.Count-1; i >=0;i--){
            if(capturingTankController[i].ShortName == collision.GetComponentInParent<TankController>().ShortName){
                capturingTankController.RemoveAt(i);
            }
        }

        //Stopping alarm sound if only no tanks in cap
        if (tanksInCap == 0 && BaseCaptured)//won
        {
            CapSound.Stop();
                   
        }
        //not yet won but tank left -reset
        else if(tanksInCap == 0){
            CapSound.Stop();
            points = 0;//reset cap
            mainUICanvas.alpha = 0f; 
        }

        // Playing alarm sound if only one tank in cap
        else if (tanksInCap == 1)
        {
            CapSound.Play();
        }            
    }

    
    // The capturingTankController is assigned in OnCollisionStay to make sure that the right TankContoller is selected.
    private void OnTriggerStay(Collider collision)
    {
        if (tanksInCap >= 1)
        {
            //capturingTankController[tanksInCap-1] = collision.GetComponentInParent<TankController>();
            CapSpriteColor = capturingTankController[tanksInCap-1].UiColor;
            foreach(TankController tank in capturingTankController){
                if(tank.CheckHit()){
                    points = 0;
                    tank.ResetWasHit();
                }
            }
        }
    }




    //____________________________________________UPDATE_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________


    void Update ()
    {
        if((int)points >= totalPoints){
            BaseCaptured = true;
        }
        // Changing color multiplier (the alpha of the colored sprite)
        if (BaseCaptured == false)
            multiplier = Mathf.Sin(Time.time * 3.35f);
        else if (BaseCaptured == true)
            multiplier = 1;

        CapSpriteColor.a = multiplier;
        CapturedSprite.color = CapSpriteColor;

        
        
        // Give cap points 
        if (tanksInCap >= 1 && !BaseCaptured)
        {
            bool cont = true;//continue
            //team = capturingTankController[0].TankTeam;

            foreach(TankController tank in capturingTankController){
                if(tank.TankTeam != team){//non team mate enter base
                    CapturedSprite.enabled = false;
                    CapSound.Stop();
                    cont = false;
                    break;
                }
            }
            if(cont){
            points += (CapSpeed * tanksInCap)* Time.deltaTime;

            int pointInInt = (int)points;
            pointsText.text = pointInInt.ToString() + "/" + totalPoints.ToString();
            pointsSlider.value = points;
            CapturedSprite.enabled = true;
            }
        }

        
        //If there are many or no tanks in the capture area -> Stop base capturing
        else if (tanksInCap != 1 && !BaseCaptured)
        {
            CapturedSprite.enabled = false;

            CapSound.Stop();
        }
        
        
        //When the base is fully captured -> Stop sound but keep the sprite visible
        else if (BaseCaptured == true)
        {
            CapturedSprite.enabled = true;

            CapSound.Stop();
        }
    }






}
