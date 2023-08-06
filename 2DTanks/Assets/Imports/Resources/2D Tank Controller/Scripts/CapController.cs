using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //                                                               Variables                                                                 
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(10)]
    [Header("Read-Only Variables")]
    [Space(10)]

    private bool BaseCaptured;

    private Color CapSpriteColor;

    private int tanksInCap;

    private float lightIntensity;
    private float multiplier;

    public TankController capturingTankController;



    //____________________________________________START_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________


    void Start ()
    {
        tanksInCap = 0;
        BaseCaptured = false;
    }




    //____________________________________________EVENTS_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________
    

    // Tank goes into the cap area
    private void OnTriggerEnter(Collider collision)
    {
        tanksInCap++;

        // Playing alarm sound if only one tank in cap
        if (tanksInCap == 1)
        {
            CapSound.Play();
        }            
    }


    // Tank leaves the cap area
    private void OnTriggerExit(Collider collision)
    {
        tanksInCap--;

        //Stopping alarm sound if only no tanks in cap
        if (tanksInCap == 0)
        {
            CapSound.Stop();            
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
        if (tanksInCap == 1)
        {
            capturingTankController = collision.GetComponentInParent<TankController>();
            CapSpriteColor = capturingTankController.UiColor;
        }
    }




    //____________________________________________UPDATE_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________


    void Update ()
    {
        // Changing color multiplier (the alpha of the colored sprite)
        if (BaseCaptured == false)
            multiplier = Mathf.Sin(Time.time * 3.35f);
        else if (BaseCaptured == true)
            multiplier = 1;

        CapSpriteColor.a = multiplier;
        CapturedSprite.color = CapSpriteColor;

        
        
        // Give cap points only if one tank is in the cap
        if (tanksInCap == 1)
        {
            capturingTankController.CapturePoints += CapSpeed * Time.deltaTime;

            CapturedSprite.enabled = true;
        }

        
        //If there are many or no tanks in the capture area -> Stop base capturing
        else if (tanksInCap != 1)
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
