using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//---------------------------------Global---------------------------------
public enum TankTeamEnum{
        NULL,Team1, Team2
    };
public class TankController : MonoBehaviour
{
    /*      Description:
     *      
     *      This script makes the tank work.
     *      
     *      There are many many GameObjects that can be attached, and if you want to make a simpler tank you can leave those slots empty that say [Optional] in the description without causing bugs.
     *      
     *      Also some of the variables or GameObjects only have effect if another setting is checked, and those say ´[If -randomBooleanNameHere- is set to true] Does something´...
     * 
     */ 

    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                  Settings                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(20)]
    [Header("______________________SETTINGS____________________________________________________________________________________________________________________________________________________________")]
    [Space(10)]

    //---------------------------------------------------
    //                      Input
    //---------------------------------------------------

    [Header("Input")]
    [Space(10)]

    [Tooltip("Simply enables/disalbes all input.")]
    public bool InputEnabled;

    [Space(10)]
    [Tooltip("Turret input mode for this tank.")]
    public TurretInputModeEnum TurretInputMode;

    public enum TurretInputModeEnum
    {
        Keyboard, Mouse
    };

    [Space(10)]
    [Tooltip("Axis name for forward/backward driving input. Input axes can be found in Edit -> Project Settings -> Input.")]
    public string VerticalInputAxisName;

    [Tooltip("Axis name for tank traverse input.")]
    public string HorizontalInputAxisName;

    [Space(10)]
    [Tooltip("[If TurretInputMode is set to Keyboard] Axis name for turret traverse input.")]
    public string TurretInputAxisName;

    [Space(10)]
    [Tooltip("Axis name for firing input.")]
    public string FireInputAxisName;

    [Space(10)]
    [Tooltip("Invert tank rotation direction when reversing.")]
    public bool InvertReverseSteering;



    //---------------------------------------------------
    //                        UI
    //---------------------------------------------------
    [Space(20)]
    [Header("UI")]
    [Space(10)]

    [Tooltip("Enables/Disables the tank UI panel.")]
    public bool UseUI;

    [Tooltip("[If UseUI is checked] Color of the UI.")]
    public Color UiColor;

    [Space(10)]
    [Tooltip("Whether the UI position is limited or not. Useful when the camera is fixed and the UI should stay inside the map area.")]
    public bool LimitUiPosition;

    [Tooltip("[If LimitUiPosition is checked] Limit for the tank UI X position. Leave empty if you don't want to limit the position on X axis.")]
    public float UiXMax;

    [Tooltip("[If LimitUiPosition is checked] Limit for the tank UI Y position. Leave empty if you don't want to limit the position on Y axis.")]
    public float UiYMax;



    //---------------------------------------------------
    //                   Raycasting
    //---------------------------------------------------
    [Space(20)]
    [Header("Raycasting")]
    [Space(10)]

    [Tooltip("Check all layers that block the shell on the hull layer i.e. both the wall and building layers.")]
    public LayerMask HullLayerCheckMask;

    [Tooltip("Check all layers that collide with the shell on the turret layer i.e. both the tank turret armor and the building layer.")]
    public LayerMask TurretLayerCheckMask;

    [Tooltip("The maximum raycast distance.")]
    public int ShellTargetRaycastDistance;

    [Tooltip("The index of the wall layer.")]
    public int WallLayerIndex;



    //---------------------------------------------------
    //                  Other Settings
    //---------------------------------------------------
    [Space(20)]
    [Header("Other Settings")]
    [Space(10)]

    [Tooltip("Enables/Disables the track marks. You may want to disable this option on low performance devices.")]
    public bool UseTrackMarks;

    [Tooltip("How long before the reloading is finished the reload sound effect should be played. Default is 1.7 (s), but you may need to adjust this if you use a custom reload sound.")]
    public float ReloadSoundLength;




    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                  Tank Stats                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(40)]
    [Header("______________________TANK STATS____________________________________________________________________________________________________________________________________________________________")]
    [Space(10)]


    //---------------------------------------------------
    //                    Basic Info
    //---------------------------------------------------
    [Header("Basic Info")]
    [Space(10)]

    [Tooltip("All tanks in the scene should have their own ID ...")] // For example the UI and ramming mechanics require each tank to have their own ID.
    public int tankID;

    [Tooltip("The full name of the tank. For example: ´Pz.Kpfw V Panther´.")]
    public string TankName;

    [Tooltip("Shorter version of the name shown in the UI. For example: ´Panther´.")]
    public string ShortName;

    [Tooltip("The type of the vehicle. Only effects the UI icon. LT = Light Tank, MT = Medium Tank, HT = Heavy Tank, TD = Tank Destroyer.")]
    public TankTypeEnum TankType;

    public enum TankTypeEnum
    {
        LT, MT, HT, TD
    };
    [Tooltip("Team tank is on")]
    public TankTeamEnum TankTeam;
    


    //---------------------------------------------------
    //                      Health
    //---------------------------------------------------
    [Space(20)]
    [Header("Health")]
    [Space(10)]

    [Tooltip("The maximum health of the tank.")]
    public int MaxHealth;

    [Tooltip("The maximum health of the tracks. The left and right tracks have their HPs calculated separately.")]
    public int MaxTrackHealth;

    [Tooltip("The time required to repair the tracks (s).")]
    public int TrackRepairTime;



    //---------------------------------------------------
    //                      Movement
    //---------------------------------------------------
    [Space(20)]
    [Header("Movement")]
    [Space(10)]

    [Tooltip("The top speed of the tank (km/h).")]
    public int TopSpeed;

    [Tooltip("The maximum reverse speed of the tank (km/h).")]
    public int ReverseSpeed;

    [Space(10)]
    [Tooltip("The traverse (i.e. turning) speed of the tank (°/s).")]
    public int TraverseSpeed;

    [Tooltip("The traverse (i.e. turning) speed of the turret (°/s).")]
    public int TurretTraverseSpeed;

    [Space(10)]
    [Tooltip("The engine power of the tank (hp).")]
    public int EnginePower;

    [Tooltip("The brake force multiplier. Default: 1.")]
    public float BrakeForce;

    [Tooltip("The acceleration force multiplier. Default: 1.")]
    public float AccelerationForce;

    [Space(10)]
    [Tooltip("Check this if the tank doesn't have a full 360° turret.")]
    public bool HasNoTurret;

    [Tooltip("[If HasNoTurret is checked] How much the gun can turn to left and right (°).")]
    public int GunTraverseLimit;



    //---------------------------------------------------
    //                      Firepower
    //---------------------------------------------------
    [Space(20)]
    [Header("Firepower")]
    [Space(10)]

    [Tooltip("How long each shell takes to reload (s).")]
    public float ReloadTime;

    [Tooltip("Inaccuracy of the gun. How far left/right the shell can go (°).")]
    public float DispersionAngle;




    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                  Gameobjects                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(40)]
    [Header("______________________GAME OBJECTS____________________________________________________________________________________________________________________________________________________________")]   
    [Space(10)]


    //---------------------------------------------------
    //                    Ammunition
    //---------------------------------------------------
    [Header("Ammunition")]
    [Space(10)]

    [Tooltip("Path for the shell in the Resources folder.")]
    public string ShellPath;



    //---------------------------------------------------
    //                   Tank Parts
    //---------------------------------------------------
    [Space(20)]
    [Header("Tank Parts")]
    [Space(10)]

    [Tooltip("Turret or gun object of the tank.")]
    public GameObject Turret;

    [Tooltip("Front left track.")]
    public GameObject TrackFL;
    [Tooltip("Front right track.")]
    public GameObject TrackFR;
    [Tooltip("Rear left track.")]
    public GameObject TrackRL;
    [Tooltip("Rear right track.")]
    public GameObject TrackRR;



    //---------------------------------------------------
    //                      Audio
    //---------------------------------------------------
    [Space(20)]
    [Header("Audio")]
    [Space(10)]

    [Tooltip("Parent GameObject of all audio sources.")]
    public GameObject AudioParent;


    [Tooltip("Engine idle sound. Played both when stationary and moving.")]
    public AudioSource AudioEngineIdle;

    [Tooltip("Engine running sound. Played when moving.")]
    public AudioSource AudioEngineRunning;

    [Tooltip("Track rattle sound. Played when moving.")]
    public AudioSource AudioTracks;

    [Tooltip("[Optional] Small crash sound. Played when hitting a building slowly.")]
    public AudioSource AudioCrashSmall;

    [Tooltip("[Optional] Large crash sound. Played when hitting a building hard.")]
    public AudioSource AudioCrashLarge;

    [Tooltip("[Optional] Tank crash sound. Played when hitting another tank.")]
    public AudioSource AudioCrashTank;

    [Tooltip("Gun shoot sound.")]
    public AudioSource AudioGunShoot;

    [Tooltip("Explosion sound effect. Played when the tank is destroyed.")]
    public AudioSource AudioDestroyed;

    [Tooltip("[Optional] Shell reload sound effect. Played when reloading is finished.")]
    public AudioSource AudioReload;



    //---------------------------------------------------
    //                Particles & Effects
    //---------------------------------------------------
    [Space(20)]
    [Header("Particles & Effects")]
    [Space(10)]

    [Tooltip("Parent GameObject of most particle effects.")]
    public GameObject ParticleParent;

    [Tooltip("[Optional if ShootParticles is left empty (array length is 0)] Parent GameObject of gun muzzle flash and smoke particles.")]
    public GameObject ShootParticleParent;

    [Tooltip("[Optional if ExhaustParticles is left empty] GameObject(s) that indicate the position for the exhaust particles. Max 2 GameObjects allowed in the array.")]
    public GameObject[] ExhaustParticlePosition;

    [Space(10)]
    [Tooltip("[Optional] Path for the dust particles in the Resources folder.")]
    public string DustParticles;

    [Tooltip("[Optional] Path for the exhaust smoke particles in the Resources folder.")]
    public string ExhaustParticles;

    [Tooltip("[Optional] Path(s) for the firing effect particles in the Resources folder.")]
    public string[] ShootParticles;

    [Tooltip("[Optional] Path(s) for the explosion effect particles in the Resources folder.")]
    public string[] ExplosionParticles;

    private ParticleSystem[] dustParticles;
    private ParticleSystem exhaustParticles;
    private ParticleSystem exhaustParticles2;
    private ParticleSystem[] shootParticles;
    private ParticleSystem[] explosionParticles;


    [Space(10)]
    [Tooltip("[If UseTrackMarks is set to true] Path for the track mark prefab GameObject in the Resources folder.")]
    public string TrackMarkPath;



    //---------------------------------------------------
    //                       UI
    //---------------------------------------------------
    [Space(20)]
    [Header("UI")]
    [Space(10)]

    [Space(10)]
    [Tooltip("[Optional if UseUI is set to false] Canvas for the tank UI. Canvas render mode must be set to ´World Space`.")]
    public GameObject Canvas;

    [Tooltip("[If UseUI is set to true] Path for UI prefab GameObject in the resources folder.")]
    public string UiPath;

    [Tooltip("[Optional. Has effect only if TurretInputMode is set to Mouse] Path for aimpoint/crosshair prefab GameObject in the resources folder.")]
    public string AimpointPath;



    //---------------------------------------------------
    //                 Dummy Gameobjects
    //---------------------------------------------------
    [Space(20)]
    [Header("Dummy Objects")]
    [Space(10)]

    [Tooltip("New dummy objects created by the script will be parented to this GameObject.")]
    public GameObject DummyObjectParent;

    [Tooltip("GameObject that indicates the position where the shell will be spawned. Should be a child of the turret GameObject.")]
    public GameObject ShellOrigin;

    [Tooltip("[Optional if UseUI is set to false] GameObject that indicates the position of the UI.")]
    public GameObject UiTarget;

    [Tooltip("[Optional] GameObject with a 3D collider and rigidbody (with IsKinematic turned on) that triggers the base capturing.")]
    public GameObject CapDetection;     

    [Space(40)]
    [Header("__________________________MachineGun____________________________________________________________________________________________________________________________________________________________")]
    [Space(10)]

    public bool machineGunEnable = false;
    public GameObject machineGunPort;
    public GameObject machineGunShell;
    public int machineGunRounds = 30;
    public int machineGunReloadSpeed;
    [Range(0.2f,0.06f)]
    public float machineGunRateFire =1f;
    public float machineGunSpread = 0.1f;

    public AudioSource machineGunFireAudio;
    public AudioSource machineGunReloadAudio;

    private bool machineGunReloaded = false;
    private float machineGunReloadTime = 0;
    private int machineGunAmmo = 0;
    private float machineGunRate;




    [Space(40)]
    [Header("__________________________READ-ONLY VARIABLES____________________________________________________________________________________________________________________________________________________________")]
    [Space(10)]

    [Tooltip("This is the collision force only against structures such as walls and buildings, not against other tanks.")]
    public float PushingForce; // This variable was moved here (was under ´Movement`) to make the header above work. For some reason the header doesn't appear in the editor if the next variable after it is not public...




    //------------------------------------------------------------------------------------------------------------
    //                                  Other GameObjects & Variables
    //------------------------------------------------------------------------------------------------------------



    //---------------------------------------------------
    //                      Input
    //---------------------------------------------------   

    private float verticalInput;
    private float horizontalInput;
    private float turretInput;
    private float fireInput;



    //---------------------------------------------------
    //                    Movement
    //---------------------------------------------------

    private Rigidbody2D Rigidbody;

    private float currentSpeed;

    private bool isDriving;
    private bool isRotating;

    private float accelerationMultiplier;
    private float speedMultiplier;
    private float speedRatio;

    private float targetHullRotation;
    private bool turning;
    private float invertReverseMultiplier;

    private Vector3 previousRotation;
    private float deltaRotation;

    private float correctedTurretRotation;

    private Vector2 mouseVector;    
    private float turretTargetAngle;
    private Quaternion turretTargetQuaternion;

    private float previousGunRotation;
    private float deltaGunRotation;

    
    private bool isColliding;


    //---------------------------------------------------
    //                    Firing
    //---------------------------------------------------

    private bool reloaded;
    private float timeReloaded;
    private bool reloadSoundPlayed;
    private int shellsRemaining;

    private float randomizedDispersion;


    private RaycastHit2D TurretShellOriginHit;
    private RaycastHit2D HullShellOriginHit;

    private float targetDistance;
    private float gunLength;

    private bool hullTargetBlocked;
    private bool turretTargetAvailable;

    private Vector3 shellPosition;
    private Vector3 hullOriginVector;

    private GameObject DummyGun;

    //[HideInInspector]public 
    private bool wasHit = false;

    //---------------------------------------------------
    //                    Health
    //---------------------------------------------------

    [Tooltip("The current health of the tank.")]
    public int Health;

    private bool IsExploded;



    //---------------------------------------------------
    //                  Track Health
    //---------------------------------------------------

    private bool tracked;

    private bool lTrackTracked;
    private bool rTrackTracked;

    private int lTrackHp;
    private int rTrackHp;

    private float timeLTracksRepaired;
    private float timeRTracksRepaired;

    private float trackRepairTimeUiFloat;
    private int trackRepairTimeUi;



    //---------------------------------------------------
    //                    Ramming
    //---------------------------------------------------

    private TankController RammingEnemyController;
    private int lastRammingEnemyId;
    private bool rammingDifferentTank;

    private float rammingTimeLimitRemaining;

    private float enemyMass;

    private float collisionVelocity;
    private float collisionForce;

    private float rammingDamage;  
    
    

    //---------------------------------------------------
    //                      UI
    //---------------------------------------------------

    private GameObject TankUI;
    private Vector3 UiPos;
    private float initialUiYMax;
    private bool camoNet = false;

    private RaycastHit TankUiHit;
    private bool tankUiHitHitting;
    private RaycastHit2D TankUiHit2;


    private int previousHp;

    private Image TankHpSliderBG;
    private Color SliderBGColor;

    private Slider TankHpSlider;
    private Text TankTypeText;
    private Text TankNameText;
    private Text TankHpText;

    private CanvasGroup TankReloadCG;
    private Text TankReloadText;
    private Image TankReloadImage;

    private GameObject TankDamageTextUI;

    private CanvasGroup TrackRepairCG;
    private CanvasGroup TrackImageCG;
    private Text TrackRepairText;

    // Base capturing
    [Tooltip("The amount of capture points. Also the time spent in the cap area in seconds, if the CapSpeed variable in the Cap Controller is 1.")]
    //public float CapturePoints;

    private GameObject Aimpoint;
    private Image AimpointUnaimedImage;
    private Image AimpointAimedImage;



    //---------------------------------------------------
    //                      Audio
    //---------------------------------------------------

    private float pitchValue;
    private float pitchRandomizer;

    // Smoothing
    private float audioLerpSmoothValue = 0.01f;



    //---------------------------------------------------
    //                Particles & Effects
    //---------------------------------------------------

    private ParticleSystem.MainModule DustParticlesFl;
    private ParticleSystem.MainModule DustParticlesFr;
    private ParticleSystem.MainModule DustParticlesRl;
    private ParticleSystem.MainModule DustParticlesRr;

    private ParticleSystem.MainModule ExhaustParticleSystem;
    private ParticleSystem.MainModule ExhaustParticleSystem2;


    // Trackmarks
    private float enableTrackMarks;

    private GameObject lTrackMarkPositionObject;
    private GameObject rTrackMarkPositionObject;

    private float lTrackMarkDistance;
    private float rTrackMarkDistance;






    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //                                                                                                                 FUNCTIONS
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    //------------------------------------------------------------------------------------------------------------
    //                                               Input
    //------------------------------------------------------------------------------------------------------------

    // Function for disabling all input
    private void DisableInput()
    {
        verticalInput = 0;
        horizontalInput = 0;
        turretInput = 0;
        fireInput = 0;

        if (Aimpoint != null)
            Destroy(Aimpoint);
    }



    // Function for firing the gun
    private void Fire()
    {
        camoNet = false;
        timeReloaded = 0;
        reloaded = false;

        randomizedDispersion = Random.Range(-DispersionAngle, DispersionAngle);


        //---------------------------------------------------
        //         Raycasting for obstacles/targets
        //---------------------------------------------------

        // Checking for hull armor targets and walls
        HullShellOriginHit = Physics2D.Raycast(ShellOrigin.transform.position, Turret.transform.up, ShellTargetRaycastDistance, HullLayerCheckMask.value);

        // Raycast hits a hull
        if (HullShellOriginHit == true && HullShellOriginHit.collider.gameObject.layer == WallLayerIndex)
        {
            hullTargetBlocked = true;
        }

        // Raycast not hitting a hull
        else if (HullShellOriginHit == false || HullShellOriginHit.collider.gameObject.layer != WallLayerIndex)
        {
            hullTargetBlocked = false;
        }



        // Checking for targets (turrets of other tanks) on the turret layer
        TurretShellOriginHit = Physics2D.Raycast(ShellOrigin.transform.position, Turret.transform.up, ShellTargetRaycastDistance, TurretLayerCheckMask.value);

        // Raycast hits a turret
        if (TurretShellOriginHit != false && TurretShellOriginHit.collider.gameObject.layer == Turret.gameObject.layer)
        {
            turretTargetAvailable = true;
            targetDistance = TurretShellOriginHit.distance;
        }

        // Raycast not hitting a turret
        else if (TurretShellOriginHit == false || TurretShellOriginHit.collider.gameObject.layer != Turret.gameObject.layer)
        {
            turretTargetAvailable = false;
        }



        // Setting shell position (and with that the layer of the shell) based on available target      

        // Hull blocked but turret visible -> Shoot the turret
        if (hullTargetBlocked == true && turretTargetAvailable == true)
            shellPosition = ShellOrigin.transform.position;

        // Hull blocked but no turret target -> Shoot the hull
        else if (hullTargetBlocked == true && turretTargetAvailable == false)
            shellPosition = ShellOrigin.transform.position + hullOriginVector;

        // Too close to shoot the hull -> Shoot the turret
        else if (targetDistance < gunLength && targetDistance != 0)
            shellPosition = ShellOrigin.transform.position;

        // Both visible -> Shoot the hull
        else if (hullTargetBlocked == false && turretTargetAvailable == true)           
            shellPosition = ShellOrigin.transform.position + hullOriginVector;

        else // Shoot the hull
            shellPosition = ShellOrigin.transform.position + hullOriginVector;



        
        
        // Firing shell
        Instantiate(Resources.Load("2D Tank Controller/" + ShellPath, typeof(GameObject)), shellPosition, Quaternion.Euler(0, 0 , Turret.transform.eulerAngles.z + randomizedDispersion));


        // Sound and particle and effects
        AudioGunShoot.Play();

        if (shootParticles != null)
        {
            foreach (ParticleSystem objects in shootParticles)
            {
                objects.Play();
            }
        }
        

        
        // Adding recoil force to the tank
        Rigidbody.AddForce(Turret.transform.rotation * new Vector2(0, Rigidbody.mass * -0.5f), ForceMode2D.Impulse);

        
        // Showing reload UI
        if (UseUI == true)
            TankReloadCG.alpha = 1;
    }




    //------------------------------------------------------------------------------------------------------------
    //                                              Movement
    //------------------------------------------------------------------------------------------------------------

    // This function makes the tank move forward
    private void MoveForward()
    {
        isDriving = true;

        // If going backward -> Slow down first
        if (currentSpeed < -1)
        {
            accelerationMultiplier += BrakeForce * 1.5f * Time.deltaTime;
        }

        // Coming to full stop
        else if (currentSpeed > -1 && currentSpeed < -0.4f)
        {
            accelerationMultiplier = 0;
        }

        // Increasing accelerationMultiplier if not driving too fast
        else if (currentSpeed >= -0.4 && currentSpeed * 3.6f < TopSpeed && accelerationMultiplier < 1)
        {
            accelerationMultiplier += (Mathf.Log(speedRatio * (1 / AccelerationForce), 0.001f) * verticalInput) * Time.deltaTime;
        }        


        // Calculating speed multiplier. Speed multiplier is responsible for slowing down the tank when the tank hits objects.
        // It prevents the tank from preserving its accelerationMultiplier i.e. its speed after hitting an obstacle and slowing down.
        speedMultiplier = speedRatio / Mathf.Abs(accelerationMultiplier) + 0.05f;
        
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.0f, 1.0f);

        // Colliding and decreasing accelerationMultiplier based on speedMultiplier
        if (isColliding == true)
        {            
            accelerationMultiplier *= speedMultiplier;
        }

        //Clamping acceleration multiplier
        accelerationMultiplier = Mathf.Clamp(accelerationMultiplier, -1.0f, 1.0f);



        // Setting engine sound volume
        AudioEngineRunning.volume = Mathf.Lerp(AudioEngineRunning.volume, 0.9f, audioLerpSmoothValue);       
    }



    // This makes the tank move backward
    private void MoveBackward()
    {
        isDriving = true;

        // If going forward -> Slow down first
        if (currentSpeed > 1)
        {
            accelerationMultiplier -= BrakeForce * 1.5f * Time.deltaTime;
        }

        // Coming to full stop
        else if (currentSpeed < 1 && currentSpeed > 0.4f)
        {
            accelerationMultiplier = 0;
        }

        // Decreasing accelerationMultiplier if not reversing too fast
        else if (currentSpeed <= 0.4f && currentSpeed * 3.6f > -ReverseSpeed && accelerationMultiplier > -1)
        {
            accelerationMultiplier -= (Mathf.Log(speedRatio * (1 / AccelerationForce), 0.001f) * -verticalInput) * Time.deltaTime;
        }


        // Calculating speed multiplier. Speed multiplier is responsible for slowing down the tank when the tank hits objects.
        // It prevents the tank from preserving its accelerationMultiplier i.e. its speed after hitting an obstacle and slowing down.
        speedMultiplier = speedRatio / Mathf.Abs(accelerationMultiplier) + 0.05f;

        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.0f, 1.0f);

        // Colliding and decreasing accelerationMultiplier based on speedMultiplier
        if (isColliding == true)
        {
            accelerationMultiplier *= speedMultiplier;
        }

        //Clamping acceleration multiplier
        accelerationMultiplier = Mathf.Clamp(accelerationMultiplier, -1.0f, 1.0f);



        // Setting engine sound volume
        AudioEngineRunning.volume = Mathf.Lerp(AudioEngineRunning.volume, 0.9f, audioLerpSmoothValue);
    }



    // This function slows down the tank when no vertical input is applied
    private void SlowDown()
    {
        isDriving = false;

        // If driving forward -> Apply backward force
        if (currentSpeed > 0 && accelerationMultiplier > 0)
        {
            accelerationMultiplier -= BrakeForce * Time.deltaTime;
        }

        // If driving backward -> Apply forward force
        else if (currentSpeed < 0 && accelerationMultiplier < 0)
        {
            accelerationMultiplier += BrakeForce * Time.deltaTime;
        }

        // If not driving and not moving -> Reset accelerationMultiplier instantly
        if (currentSpeed < 0.1f && currentSpeed > -0.1f)
        {
            accelerationMultiplier = 0;
        }

        // Setting engine sound volume
        AudioEngineRunning.volume = Mathf.Lerp(AudioEngineRunning.volume, 0.2f, audioLerpSmoothValue);
    }



    // This function makes the tank turn
    private void RotateTank()
    {
        // If applying input -> Continue
        if (horizontalInput != 0)
        {
            isRotating = true;

            // If rotating has just started -> Set targetHullRotation to current rotation to prevent bugs
            if (turning == false)
            {
                targetHullRotation = transform.eulerAngles.z;
                turning = true;
            }

            // If the tank is stuck, but input is applied -> Stop turning to prevent bugs
            if (deltaRotation == 0 && Mathf.Abs(horizontalInput) == 1.0f && isColliding == true)
            {
                targetHullRotation = transform.eulerAngles.z;
            }
            
            // If the tank is not stuck -> Rotate the tank (or set the target rotation, actual rotating happens later)    
            else
            {
                targetHullRotation -= TraverseSpeed * horizontalInput * invertReverseMultiplier * Time.deltaTime;
            }            

            // Change turning direction smoothly if invert reverse steering is enabled
            if (verticalInput < 0 && InvertReverseSteering == true)
            {
                invertReverseMultiplier = Mathf.Lerp(invertReverseMultiplier, -1, 0.05f);
            }
            
            // Set invertReverseMultiplier back to 1
            else if (verticalInput >= 0 || InvertReverseSteering == false && invertReverseMultiplier < 0.95f)
            {
                invertReverseMultiplier = Mathf.Lerp(invertReverseMultiplier, 1, 0.05f);
            }

            // If inverReverseMultiplier is 1 -> Do not lerp
            else if (verticalInput >= 0 || InvertReverseSteering == false && invertReverseMultiplier > 0.95f)
            {
                invertReverseMultiplier = 1;
            }


            // Calculating delta rotation to check wheter the tank is stuck
            deltaRotation = Vector2.Angle(transform.up, previousRotation);

            previousRotation = transform.up;


            // Setting engine sound volume
            AudioEngineRunning.volume = Mathf.Lerp(AudioEngineRunning.volume, 0.9f, audioLerpSmoothValue);
        }

        // Set variables if no input is applied
        else if (horizontalInput == 0)
        {
            isRotating = false;
            turning = false;            
        }
    }



    // This function makes the turret or the gun rotate and also sets the aimpoint position
    private void RotateTurret()
    {
        // Setting correctedTurretRotation value
        if (Turret.transform.localEulerAngles.z < 180)
            correctedTurretRotation = Turret.transform.localEulerAngles.z;
        else if (Turret.transform.localEulerAngles.z > 180)
            correctedTurretRotation = Turret.transform.localEulerAngles.z - 360;




        //---------------------------------------------------
        //                  Mouse Input
        //---------------------------------------------------

        if (TurretInputMode == TurretInputModeEnum.Mouse && InputEnabled == true && IsExploded == false)
        {
            // Calculating vectors and angles to get the input from the mouse position
            mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Turret.transform.position;

            turretTargetAngle = Mathf.Atan2(mouseVector.y, mouseVector.x) * Mathf.Rad2Deg - 90;
            turretTargetQuaternion = Quaternion.AngleAxis(turretTargetAngle, Vector3.forward);

            // If the tank has no turret -> Making sure that target rotation is within the gun traverse limits
            if (HasNoTurret == true)
            {
                // Calculating dummy gun values for correct gun rotation
                previousGunRotation = DummyGun.transform.localEulerAngles.z;
                DummyGun.transform.rotation = Quaternion.RotateTowards(DummyGun.transform.rotation, turretTargetQuaternion, TurretTraverseSpeed * Time.deltaTime);
                deltaGunRotation = DummyGun.transform.localEulerAngles.z - previousGunRotation;

                // Resetting dummy gun rotation if rotating in the wrong direction or just within the rotation limits
                if (correctedTurretRotation >= GunTraverseLimit && deltaGunRotation > 0 || correctedTurretRotation <= -GunTraverseLimit && deltaGunRotation < 0 || correctedTurretRotation < GunTraverseLimit || correctedTurretRotation > -GunTraverseLimit)
                {
                    DummyGun.transform.rotation = Turret.transform.rotation;
                }


                // If turret rotation is within limits or outside limits but the dummy gun turning in the right direction -> Turn turret
                if (correctedTurretRotation < GunTraverseLimit && correctedTurretRotation > -GunTraverseLimit || correctedTurretRotation > GunTraverseLimit && deltaGunRotation < 0 || correctedTurretRotation < -GunTraverseLimit && deltaGunRotation > 0)
                {
                    Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, turretTargetQuaternion, TurretTraverseSpeed * Time.deltaTime);
                }                
            }         


            // If the tank has a 360° turret
            else if (HasNoTurret == false)
                Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, turretTargetQuaternion, TurretTraverseSpeed * Time.deltaTime);

            // Aimpoint settings

            // Turret close to aimpoint
            if (Vector2.Angle(mouseVector, Turret.transform.up) < 1 && Aimpoint != null)
            {
                AimpointAimedImage.enabled = true;
                AimpointUnaimedImage.enabled = false;
            }
                

            // Turret not close to aimpoint
            else if (Vector2.Angle(mouseVector, Turret.transform.up) > 1 && Aimpoint != null)
            {
                AimpointAimedImage.enabled = false;
                AimpointUnaimedImage.enabled = true;
            }
                
        }






        //---------------------------------------------------
        //                  Keyboard Input
        //---------------------------------------------------

        if (HasNoTurret == false)
        {
            // Rotating left
            if (turretInput < 0)
                Turret.transform.Rotate(Vector3.forward, -turretInput * TurretTraverseSpeed * Time.deltaTime);

            // Rotating right
            else if (turretInput > 0)
                Turret.transform.Rotate(Vector3.forward, -turretInput * TurretTraverseSpeed * Time.deltaTime);
        }

        
        // TDs
        else if (HasNoTurret == true)
        {
            // Rotating left
            if (turretInput < 0 && correctedTurretRotation < GunTraverseLimit)
                Turret.transform.Rotate(Vector3.forward, -turretInput * TurretTraverseSpeed * Time.deltaTime);

            // Rotating right
            else if (turretInput > 0 && correctedTurretRotation > -GunTraverseLimit)
                Turret.transform.Rotate(Vector3.forward, -turretInput * TurretTraverseSpeed * Time.deltaTime);
        }
    }
    



    //------------------------------------------------------------------------------------------------------------
    //                                       Health & Taking Damage
    //------------------------------------------------------------------------------------------------------------

    // Function that reduces the tank's HP. This function is called by the shell controller.
    public void TakeDamage(int amount)
    {
        wasHit = true;
        camoNet = false;
        previousHp = Health;

        Health -= amount;
        
        // If UI is enabled -> Show the amount of damage taken
        if (UseUI == true)
        {
            if (Health > 0)
            {
                TankDamageTextUI.GetComponent<Text>().text = "-" + amount.ToString();
            }
            else if (Health <= 0)
            {
                TankDamageTextUI.GetComponent<Text>().text = "-" + previousHp;
            }

            // Play animation
            TankDamageTextUI.GetComponent<Animator>().Play("TankDamageText", -1, 0.0f);
        }
        
    }



    // Function that reduces track hp when the tracks are shot. This function is called by the shell controller.
    public void TakeTrackDamage(int amount, string side, string wheel)
    {
        camoNet = false;
        
        // Left track is being damaged
        if (side == "l")
        {
            lTrackHp -= amount;

            if (lTrackHp <= 0)
                lTrackTracked = true;
        }

        // Right track is being damaged   
        else if (side == "r")
        {
            rTrackHp -= amount;

            if (rTrackHp <= 0)
                rTrackTracked = true;
        }

        // Resetting the time repaired variable if the tracks were already damaged and being repaired
        if (timeLTracksRepaired > 0 && side == "l")
            timeLTracksRepaired = 0;
        if (timeRTracksRepaired > 0 && side == "r")
            timeRTracksRepaired = 0;



        // Checking if either of the tracks are damaged
        if (lTrackHp <= 0 || rTrackHp <= 0)
        {
            tracked = true;

            // Showing UI
            if (UseUI == true)
            {
                TrackRepairCG.alpha = 1;
                TrackRepairText.text = "";
            }
            
        }


        // Showing broken track piece
        if (lTrackTracked == true)
        {
            switch (wheel)
            {
                case "fl":
                    TrackFL.GetComponentInChildren<SpriteRenderer>().enabled = true;
                    break;
                case "rl":
                    TrackRL.GetComponentInChildren<SpriteRenderer>().enabled = true;
                    break;
            }
        }

        if (rTrackTracked == true)
        {
            switch (wheel)
            {
                case "fr":
                    TrackFR.GetComponentInChildren<SpriteRenderer>().enabled = true;
                    break;
                case "rr":
                    TrackRR.GetComponentInChildren<SpriteRenderer>().enabled = true;
                    break;
            }
        }
    }



    // Function for destroying the tank
    private void DestroyTank()
    {
        IsExploded = true;

        // Playing effects
        if (ExplosionParticles.Length != 0)
        {
            foreach (ParticleSystem objects in explosionParticles)
            {
                objects.Play();
            }
        }
        

        AudioDestroyed.Play();

        // Chaning the tank's color
        GetComponent<SpriteRenderer>().color = new Vector4(0.4f, 0.4f, 0.4f, 1);
        Turret.GetComponent<SpriteRenderer>().color = new Vector4(0.4f, 0.4f, 0.4f, 1);

        // Disabling exhaust particles
        if (exhaustParticles != null)
            exhaustParticles.gameObject.SetActive(false);
        if (exhaustParticles2 != null)
            exhaustParticles2.gameObject.SetActive(false);


        // Disabling UI
        if (UseUI == true)
        {
            Health = 0;
            TankUI.GetComponent<CanvasGroup>().alpha = 0;
            TankReloadCG.alpha = 0;
        }
        

        // Changing cap detection position (underground) to stop capping
        if (CapDetection != null)
            CapDetection.GetComponent<BoxCollider>().transform.position = new Vector3 (0,0,-20);


        // Disabling this script
        StartCoroutine(DisableScriptInTime(1));        
    }



    //------------------------------------------------------------------------------------------------------------
    //                                                  UI
    //------------------------------------------------------------------------------------------------------------

    // Function that handles most of the UI features (value changes, alpha chages if over other UI or tank, position clamping).
    private void UpdateUI()
    {
        // Adding aimpoint if it set but not instantiated (for example when tank input is enabled after the game is started)
        if (AimpointPath != "" && Aimpoint == null && InputEnabled == true && TurretInputMode == TurretInputModeEnum.Mouse)
        {
            Cursor.visible = false;
            Aimpoint = Instantiate(Resources.Load("2D Tank Controller/" + AimpointPath, typeof(GameObject)), transform.position, Quaternion.identity, Canvas.transform) as GameObject;
            AimpointAimedImage = Aimpoint.transform.Find("Aimed").GetComponent<Image>();
            AimpointUnaimedImage = Aimpoint.GetComponent<Image>();
        }


        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                                           Changing UI values
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------
        //                   Basic values
        //------------------------------------------------------------------------------------------------------

        // Tank HP text
        TankHpText.text = Health + "/" + MaxHealth;

        // Health slider
        TankHpSlider.value = (float)Health / (float)MaxHealth;

        // Reload text & image
        TankReloadText.text = ((int)ReloadTime - (int)timeReloaded).ToString();
        TankReloadImage.fillAmount = (float)timeReloaded / (float)ReloadTime;


        //------------------------------------------------------------------------------------------------------
        //                 Track repair UI
        //------------------------------------------------------------------------------------------------------

        // Only the left track is damaged -> Show left track repair time
        if (lTrackTracked == true && rTrackTracked == false)
        {
            trackRepairTimeUiFloat = TrackRepairTime - timeLTracksRepaired;
        }

        // Only the right track is damaged -> Show right track repair time
        else if (lTrackTracked == false && rTrackTracked == true)
        {
            trackRepairTimeUiFloat = TrackRepairTime - timeRTracksRepaired;
        }

        // If both tracks are damaged -> Show the repair time of the track that is more damaged
        else if (lTrackTracked == true && rTrackTracked == true)
        {
            // Left track is more repaired
            if (timeLTracksRepaired > timeRTracksRepaired)
            {
                trackRepairTimeUiFloat = TrackRepairTime - timeRTracksRepaired;
            }

            // Right track is more repaired
            else if (timeRTracksRepaired > timeLTracksRepaired)
            {
                trackRepairTimeUiFloat = TrackRepairTime - timeLTracksRepaired;
            }
        }

        trackRepairTimeUi = (int)trackRepairTimeUiFloat;

        TrackRepairText.text = trackRepairTimeUi.ToString();

        // Track image flashing
        TrackImageCG.alpha = Mathf.Sin(Time.time * 8) * 4;

        



        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                                   Changing alpha if over other tank or UI
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //---------------------------------------------------
        //                UI over other UI
        //---------------------------------------------------

        if (Physics.Raycast(TankUI.transform.position + new Vector3(0, 0, -1), new Vector3(0, 0, 10), out TankUiHit) && IsExploded == false)
        {               
            // If hitting other tanks UI -> Set UI opacity to 40% and move the UI over the other UI, so that the transparent UI is over the opaque one
            if (TankUiHit.collider.gameObject.layer == 5 && TankUiHit.transform.parent.name != TankUI.transform.name)
            {
                TankUI.GetComponent<CanvasGroup>().alpha = 0.4f;
                tankUiHitHitting = true;

                TankUI.transform.SetSiblingIndex(TankUiHit.collider.transform.parent.GetSiblingIndex() + 1);
            }

            // If not hitting other UI
            else if (TankUiHit.collider.gameObject.layer == 5 && TankUiHit.transform.parent.name == TankUI.transform.name)
            {
                tankUiHitHitting = false;
            }
        }


        //---------------------------------------------------
        //               UI over other tank
        //---------------------------------------------------

        TankUiHit2 = Physics2D.Raycast(TankUI.transform.position + new Vector3(0, 0, 1), new Vector3(0, 0, 10));

        // If hitting other tank -> Set opacity to 40%
        if (TankUiHit2 == true && TankUiHit2.collider.gameObject.layer == gameObject.layer && TankUiHit2.collider.gameObject.GetComponent<TankController>() != null)
        {
            TankUI.GetComponent<CanvasGroup>().alpha = 0.4f;
        }



        // If the UI is not over another UI or tank -> Set opacity back to 100%
        else if (TankUiHit2 == false && tankUiHitHitting == false && !camoNet)
        {
            TankUI.GetComponent<CanvasGroup>().alpha = 1f;
        }


        // If the tank is dead -> Set opacity to 0
        if (IsExploded == true)
            TankUI.GetComponent<CanvasGroup>().alpha = 0f;




        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                            Limiting UI position (avoid UI going over camera limits)
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Changing vertical (y) limit value if reloading or tracked (those icons need more space)
        if (reloaded == true && tracked == false)
            UiYMax = initialUiYMax;
        else if (reloaded == false || tracked == true)
            UiYMax = initialUiYMax - 1.7f;


        // If UI position is limited
        if (LimitUiPosition == true)
        {
            // The UI is inside the limits
            if (UiTarget.transform.position.x > -UiXMax + 0.1f && UiTarget.transform.position.x < UiXMax - 0.1f && UiTarget.transform.position.y > -UiYMax + 0.1f && UiTarget.transform.position.y < UiYMax - 0.1f && reloaded == true && tracked == false /**/ || /**/ UiTarget.transform.position.x > -UiXMax + 0.1f && UiTarget.transform.position.x < UiXMax - 0.1f && UiTarget.transform.position.y > -UiYMax - 1.6f && UiTarget.transform.position.y < UiYMax - 0.1f && (reloaded == false || tracked == true))
            {
                UiPos = new Vector3(UiTarget.transform.position.x, UiTarget.transform.position.y, -5);
            }

            // The UI is outside the horizontal axis limit
            else if (UiTarget.transform.position.x <= -UiXMax && UiTarget.transform.position.y >= -UiYMax && UiTarget.transform.position.y <= UiYMax || UiTarget.transform.position.x >= UiXMax && UiTarget.transform.position.y >= -UiYMax && UiTarget.transform.position.y <= UiYMax)
            {
                // Over the limit
                if (UiTarget.transform.position.x > 0)
                    UiPos = new Vector3(UiXMax, UiTarget.transform.position.y, -5);

                // Below the limit
                else if (UiTarget.transform.position.x < 0)
                    UiPos = new Vector3(-UiXMax, UiTarget.transform.position.y, -5);
            }

            // The UI is outside the vertical axis limit
            else if (UiTarget.transform.position.x >= -UiXMax && UiTarget.transform.position.x <= UiXMax && UiTarget.transform.position.y >= UiYMax || UiTarget.transform.position.x >= -UiXMax && UiTarget.transform.position.x <= UiXMax && UiTarget.transform.position.y <= -UiYMax && reloaded == true && tracked == false || UiTarget.transform.position.x >= -UiXMax && UiTarget.transform.position.x <= UiXMax && UiTarget.transform.position.y <= -UiYMax - 1.7f && (reloaded == false || tracked == true))
            {
                // Over the limit
                if (UiTarget.transform.position.y > 0)
                    UiPos = new Vector3(UiTarget.transform.position.x, UiYMax, -5);

                // Below the limit
                else if (UiTarget.transform.position.y < 0 && reloaded == true && tracked == false)
                    UiPos = new Vector3(UiTarget.transform.position.x, -UiYMax, -5);

                // Below the limit and reloading or tracked -> Move UI lower than the Y limit because there are no extra icons below the UI. Reload and tracked icons are at the top of the UI.
                else if (UiTarget.transform.position.y < 0 && reloaded == false || UiTarget.transform.position.y < 0 && tracked == false)
                    UiPos = new Vector3(UiTarget.transform.position.x, -UiYMax - 1.7f, -5);
            }
        }

        
        
        // If UI position is not limited
        else if (LimitUiPosition == false)
        {
            UiPos = new Vector3(UiTarget.transform.position.x, UiTarget.transform.position.y, -5);
        }
    }


    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                               Effects & Particles
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    // Function that handles most particles
    private void EmitParticles()
    {
        // Chaning exhaust particles speed and size based on vertical input
        if (ExhaustParticles != "")
        {
            ExhaustParticleSystem.simulationSpeed = Mathf.Clamp(verticalInput * 1, 0.7f, 1.0f);
            ExhaustParticleSystem.startSize = Mathf.Clamp(verticalInput * 2, 0.7f, 1.2f);
            ExhaustParticleSystem.startSpeed = Mathf.Clamp(verticalInput * 4, 2, 3);

            // Two exhaust particle effects
            if (ExhaustParticlePosition.Length == 2)
            {
                ExhaustParticleSystem2.simulationSpeed = Mathf.Clamp(verticalInput * 1, 0.7f, 1.0f);
                ExhaustParticleSystem2.startSize = Mathf.Clamp(verticalInput * 2, 0.7f, 1.2f);
                ExhaustParticleSystem2.startSpeed = Mathf.Clamp(verticalInput * 4, 2, 3);
            }
        }
        


        
        // Dust particle size

        if (DustParticles != "")
        {
            // When driving
            if (isDriving == true || currentSpeed > 0.1f)
            {
                DustParticlesFl.startSize = Mathf.Clamp((Mathf.Abs((float)currentSpeed) * 40f) / TopSpeed, 2.0f, 12.0f);
                DustParticlesFr.startSize = Mathf.Clamp((Mathf.Abs((float)currentSpeed) * 40f) / TopSpeed, 2.0f, 12.0f);
                DustParticlesRl.startSize = Mathf.Clamp((Mathf.Abs((float)currentSpeed) * 40f) / TopSpeed, 2.0f, 12.0f);
                DustParticlesRr.startSize = Mathf.Clamp((Mathf.Abs((float)currentSpeed) * 40f) / TopSpeed, 2.0f, 12.0f);
            }

            // When rotating on point (not driving forward/backward)
            else if (isRotating == true && isDriving == false && currentSpeed < 0.1f)
            {
                DustParticlesFl.startSize = 5;
                DustParticlesFr.startSize = 5;
                DustParticlesRl.startSize = 5;
                DustParticlesRr.startSize = 5;
            }

            // When not moving
            else if (isRotating == false && isDriving == false)
            {
                DustParticlesFl.startSize = 0;
                DustParticlesFr.startSize = 0;
                DustParticlesRl.startSize = 0;
                DustParticlesRr.startSize = 0;
            }
        }
        
    }



    // Function for placing track marks
    private void PlaceTrackMarks()
    {
        if (UseTrackMarks == true)
        {
            // Making sure that tank is moving
            if (currentSpeed > 0.1f || currentSpeed < -0.1f)
            {
                lTrackMarkDistance += Mathf.Abs((float)currentSpeed) * enableTrackMarks * Time.deltaTime;
                rTrackMarkDistance += Mathf.Abs((float)currentSpeed) * enableTrackMarks * Time.deltaTime;
            }

            // If the right track has moved the length of one track mark piece -> Place track mark Prefab
            if (lTrackMarkDistance > 0.64f)
            {
                Instantiate(Resources.Load("2D Tank Controller/" + TrackMarkPath, typeof(GameObject)), lTrackMarkPositionObject.transform.position, transform.rotation);

                lTrackMarkDistance = 0;
            }

            // If the left track has moved the length of one track mark piece -> Place track mark Prefab
            if (rTrackMarkDistance > 0.64f)
            {
                Instantiate(Resources.Load("2D Tank Controller/" + TrackMarkPath, typeof(GameObject)), rTrackMarkPositionObject.transform.position, transform.rotation);

                rTrackMarkDistance = 0;
            }
        }
    }




    // Function that changes engine and track sound ppitch and volume based on the tank's movement
    private void AudioManager()
    {
        // Setting AudioParent position so that it is always in the origin of the world (for consitent volume)
        AudioParent.transform.position = new Vector3(0, 0, 0);

        // If the tank is driving -> Pitch is based on the speed ratio
        if (Mathf.Abs(currentSpeed) > 0.5f)
            pitchValue = Mathf.Lerp(pitchValue, speedRatio, audioLerpSmoothValue);

        // If the tank is not driving -> Pitch is based on the horizontal input
        else if (Mathf.Abs(currentSpeed) < 0.5f)
            pitchValue = Mathf.Lerp(pitchValue, 0.5f * Mathf.Abs(horizontalInput), audioLerpSmoothValue);

        
        // Setting engine sound pitch
        AudioEngineRunning.pitch = 0.5f + (pitchValue);

        // Setting track sound pitch
        AudioTracks.pitch = 1 + pitchValue * 1.5f;
        AudioTracks.volume = 2 * pitchValue;
    }

    // Function that playes the small crash sound
    private void PlaySmallCrashSound()
    {
        // Randomizing pitch
        pitchRandomizer = UnityEngine.Random.Range(0.8f, 1);

        // Applying pitch and volume
        AudioCrashSmall.volume = 1;
        AudioCrashSmall.pitch = pitchRandomizer;

        // Playing sound
        AudioCrashSmall.Play();
    }


    // Function that playes the large crash sound
    private void PlayLargeCrashSound()
    {
        // Randomizing pitch
        pitchRandomizer = UnityEngine.Random.Range(0.8f, 1);

        // Applying pitch and volume
        AudioCrashLarge.volume = 1;
        AudioCrashLarge.pitch = pitchRandomizer;

        // Playing sound
        AudioCrashLarge.Play();
    }


    // Function that playes the tank crash sound
    private void PlayTankCrashSound()
    {
        // Randomizing pitch
        pitchRandomizer = UnityEngine.Random.Range(0.7f, 0.9f);

        // Applying pitch and volume
        AudioCrashTank.volume = speedRatio + 0.5f;
        AudioCrashTank.pitch = pitchRandomizer;

        // Playing sound
        AudioCrashTank.Play();
    }


    
    //------------------------------------------------------------------------------------------------------------
    //                                               Coroutines
    //------------------------------------------------------------------------------------------------------------


    // Disable script in time
    IEnumerator DisableScriptInTime(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(TankUI);
        Destroy(gameObject.GetComponent<TankController>());
        

        yield return null;
    }

    //------------------------------------------------------------------------------------------------------------
    //                                                Events
    //------------------------------------------------------------------------------------------------------------


    // Tank collision (ramming mechanics and collision sounds)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Slow collision sound
        if (Mathf.Abs(currentSpeed) > 3 && Mathf.Abs(currentSpeed) < 7 && AudioCrashSmall != null)
        {
            PlaySmallCrashSound();
        }

        // Hard collision sound
        else if (Mathf.Abs(currentSpeed) > 7 && AudioCrashLarge != null)
        {
            PlayLargeCrashSound();
        }




        //---------------------------------------------------
        //                    Ramming
        //---------------------------------------------------

        if (collision.gameObject.layer == gameObject.layer && collision.gameObject.GetComponent<TankController>() != null)
        {
            RammingEnemyController = collision.gameObject.GetComponent<TankController>();

            // Checking if ramming a different tank
            if (lastRammingEnemyId != 0 && lastRammingEnemyId != RammingEnemyController.tankID)
            {
                rammingDifferentTank = true;
            }
            else
                rammingDifferentTank = false;

            lastRammingEnemyId = RammingEnemyController.tankID;



            // If ramming time limit is over or ramming other tank -> Continue to ramming damage calculation
            // Ramming time limit prevents ramming damage being applied too often for example when two fast moving tanks are continously colliding with each other when one is chasing the other

            if (rammingTimeLimitRemaining <= 0 || rammingDifferentTank == true)
            {
                // Playing sound effect
                if (AudioCrashTank != null)
                PlayTankCrashSound();

                enemyMass = collision.gameObject.GetComponent<Rigidbody2D>().mass;

                collisionVelocity = collision.relativeVelocity.magnitude;
  

                // If going fast enough for ramming damage
                if (collisionVelocity > 2)
                {
                    // Collision force = velocity * mass
                    collisionForce = ((collisionVelocity) * (Rigidbody.mass + enemyMass)) * 0.0001f;

                    // Ramming damage depens on the mass of the vehicle relative to the other tank's mass
                    rammingDamage = (enemyMass / (enemyMass + Rigidbody.mass)) * collisionForce;

                    rammingDamage *= Random.Range(0.8f, 1.2f);

                    // Applying damage
                    if (rammingDamage > 0)
                        TakeDamage((int)rammingDamage);
                }
            }

            rammingTimeLimitRemaining = 2.0f;
        }        
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        isColliding = true;
    }








    //____________________________________________START_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________



    private void Start()
    {
        //---------------------------------------------------
        //                     Shooting
        //---------------------------------------------------
        reloaded = true;
        machineGunAmmo = machineGunRounds;

        reloadSoundPlayed = false;
        hullOriginVector = new Vector3(0, 0, 1);

        // Setting gunLength variable based on the position of the ´ShootParticleParent´ GameObject because it is at the end of the barrel and therefore is a good way to determine the length of the gun.
        gunLength = ShootParticleParent.transform.localPosition.y;

        // Setting up dummy gun object if tank has no turret
        if (HasNoTurret == true)
        {
            DummyGun = new GameObject("Dummy Gun");
            DummyGun.transform.SetParent(DummyObjectParent.transform);
            DummyGun = Instantiate(DummyGun, transform.position, transform.rotation, DummyObjectParent.transform) as GameObject;
        }


        //---------------------------------------------------
        //                      Health
        //---------------------------------------------------
        Health = MaxHealth;
        lTrackHp = MaxTrackHealth;
        rTrackHp = MaxTrackHealth;
        lTrackTracked = false;
        rTrackTracked = false;
        tracked = false;
        IsExploded = false;


        //---------------------------------------------------
        //                     Movment
        //---------------------------------------------------
        Rigidbody = GetComponent<Rigidbody2D>();

        accelerationMultiplier = 0.5f;
        invertReverseMultiplier = 1.0f;

        targetHullRotation = transform.eulerAngles.z;
        turning = false;


        //---------------------------------------------------
        //                   Track Marks
        //---------------------------------------------------
        if (UseTrackMarks == true)
            enableTrackMarks = 1;
            
        else if (UseTrackMarks == false)
            enableTrackMarks = 0;

        // Instantiating empty GameObjects which indicate the position for track marks
        if (UseTrackMarks == true)
        {
            lTrackMarkPositionObject = new GameObject("Left Track Mark Position");
            lTrackMarkPositionObject.transform.SetParent(DummyObjectParent.transform);
            lTrackMarkPositionObject = Instantiate(lTrackMarkPositionObject, transform.position, transform.rotation, DummyObjectParent.transform) as GameObject;
            lTrackMarkPositionObject.transform.localPosition = new Vector3(TrackFL.GetComponent<BoxCollider2D>().offset.x, 0, 0);


            rTrackMarkPositionObject = new GameObject("Right Track Mark Position");
            rTrackMarkPositionObject.transform.SetParent(DummyObjectParent.transform);
            rTrackMarkPositionObject = Instantiate(rTrackMarkPositionObject, transform.position, transform.rotation, DummyObjectParent.transform) as GameObject;
            rTrackMarkPositionObject.transform.localPosition = new Vector3(TrackFR.GetComponent<BoxCollider2D>().offset.x, 0, 0);
        }
        



        //---------------------------------------------------
        //                     Ramming
        //---------------------------------------------------
        rammingTimeLimitRemaining = 0;
        rammingDifferentTank = false;      

        
        
        
        //---------------------------------------------------
        //                      UI
        //---------------------------------------------------

        // Setting up UI only if it's enabled
        if (UseUI == true)
        {
            // Searching for a canvas if UI is enabled, but no canvas GameObject is assigned
            if (UseUI == true && Canvas == null)
                Canvas = GameObject.Find("Canvas");

            // Priting error message to the console if UI is enabled but no canvas is assigned or no canvas is not found
            if (UseUI == true && Canvas == null)
                Debug.LogWarning("ERROR: You need to assign a Canvas GameObject in the TankController script if UseUI is set to true! This can be automated if the canvas is called ´Canvas´ so the script can find it. Tank causing the error: " + gameObject.name + ".");


            //Instantiating the UI
            TankUI = Instantiate(Resources.Load("2D Tank Controller/" + UiPath, typeof(GameObject)), UiTarget.transform.position, Quaternion.identity, Canvas.transform) as GameObject;

            //Instantiating aimpoint
            if (AimpointPath != "" && InputEnabled == true && TurretInputMode == TurretInputModeEnum.Mouse)
            {
                Cursor.visible = false;
                Aimpoint = Instantiate(Resources.Load("2D Tank Controller/" + AimpointPath, typeof(GameObject)), transform.position, Quaternion.identity, Canvas.transform) as GameObject;
                AimpointAimedImage = Aimpoint.transform.Find("Aimed").GetComponent<Image>();
                AimpointUnaimedImage = Aimpoint.GetComponent<Image>();
            }
                

            //Giving UI unique name based on the tank's name and its ID
            TankUI.gameObject.name = TankUI.gameObject.name + " " + ShortName + " (" + tankID + ")";

            // Assigning UI objects
            TankHpSliderBG = TankUI.transform.Find("Slider").Find("Background").GetComponent<Image>();
            TankHpSlider = TankUI.transform.Find("Slider").GetComponent<Slider>();
            TankTypeText = TankUI.transform.Find("Slider").Find("TankTypeText").GetComponent<Text>();
            TankNameText = TankUI.transform.Find("Slider").Find("TankNameText").GetComponent<Text>();
            TankHpText = TankUI.transform.Find("Slider").Find("TankHPText").GetComponent<Text>();

            // Assigning reload text & images
            TankReloadCG = TankUI.transform.Find("Slider").Find("TankReload").GetComponent<CanvasGroup>();
            TankReloadText = TankUI.transform.Find("Slider").Find("TankReload").Find("TankReloadText").GetComponent<Text>();
            TankReloadImage = TankUI.transform.Find("Slider").Find("TankReload").Find("TankReloadImage").GetComponent<Image>();

            // Assigning damage UI
            TankDamageTextUI = TankUI.transform.Find("Slider").Find("TankDamageText").gameObject;

            // Assigning track repair UI
            TrackRepairCG = TankUI.transform.Find("Slider").Find("TrackRepair").GetComponent<CanvasGroup>();
            TrackImageCG = TankUI.transform.Find("Slider").Find("TrackRepair").Find("TrackImage").GetComponent<CanvasGroup>();
            TrackRepairText = TankUI.transform.Find("Slider").Find("TrackRepair").Find("TrackRepairText").GetComponent<Text>();

            // Changing the color of the UI         
            TankNameText.color = UiColor;
            TankTypeText.color = UiColor;
            TankHpSlider.fillRect.GetComponent<Image>().color = UiColor;
            TankReloadImage.color = UiColor;

            // Setting slider background transparency to 60%
            SliderBGColor = UiColor;
            SliderBGColor.a = 0.6f;
            TankHpSliderBG.color = SliderBGColor;


            //---Setting UI initial values-------------------------------

            // Health slider
            TankHpSlider.value = 1;

            // Tank type
            if (TankType == TankTypeEnum.LT)
                TankTypeText.text = "◇";
            else if (TankType == TankTypeEnum.MT)
                TankTypeText.text = "◆";
            else if (TankType == TankTypeEnum.HT)
                TankTypeText.text = "◈";
            else if (TankType == TankTypeEnum.TD)
                TankTypeText.text = "▽";

            // Tank name
            TankNameText.text = ShortName;

            // Tank HP text
            TankHpText.text = Health + "/" + MaxHealth;

            // Reload CG, text & image
            TankReloadCG.alpha = 0;
            TankReloadText.text = "-";
            TankReloadImage.fillAmount = 0;

            // Track repair
            TrackRepairCG.alpha = 0;

            // Other variables
            tankUiHitHitting = false;
            initialUiYMax = UiYMax;
        }



        //---------------------------------------------------
        //                      Audio
        //---------------------------------------------------

        // Starting engine and track sounds
        AudioEngineIdle.Play();
        AudioEngineRunning.Play();
        AudioTracks.Play();

        AudioEngineIdle.volume = 0.3f;
        AudioEngineRunning.volume = 0.0f;

        AudioEngineRunning.pitch = 0.5f;

        AudioTracks.volume = 0;
        AudioTracks.pitch = 1;



        //---------------------------------------------------
        //                    Particles
        //---------------------------------------------------

        // Dust Particles
        if (DustParticles != "")
        {
            dustParticles = new ParticleSystem[4];

            // Front Left
            dustParticles[0] = Instantiate(Resources.Load("2D Tank Controller/" + DustParticles, typeof(ParticleSystem)), transform.position, transform.rotation, ParticleParent.transform) as ParticleSystem;
            dustParticles[0].transform.localPosition = new Vector3(TrackFL.transform.GetChild(0).transform.localPosition.x, TrackFL.transform.GetChild(0).transform.localPosition.y - 1.5f, 0);

            // Front Right
            dustParticles[1] = Instantiate(Resources.Load("2D Tank Controller/" + DustParticles, typeof(ParticleSystem)), transform.position, transform.rotation, ParticleParent.transform) as ParticleSystem;
            dustParticles[1].transform.localPosition = new Vector3(TrackFR.transform.GetChild(0).transform.localPosition.x, TrackFR.transform.GetChild(0).transform.localPosition.y - 1.5f, 0);

            // Rear Left
            dustParticles[2] = Instantiate(Resources.Load("2D Tank Controller/" + DustParticles, typeof(ParticleSystem)), transform.position, transform.rotation, ParticleParent.transform) as ParticleSystem;
            dustParticles[2].transform.localPosition = new Vector3(TrackRL.transform.GetChild(0).transform.localPosition.x, TrackRL.transform.GetChild(0).transform.localPosition.y + 1.5f, 0);

            // Rear Right
            dustParticles[3] = Instantiate(Resources.Load("2D Tank Controller/" + DustParticles, typeof(ParticleSystem)), transform.position, transform.rotation, ParticleParent.transform) as ParticleSystem;
            dustParticles[3].transform.localPosition = new Vector3(TrackRR.transform.GetChild(0).transform.localPosition.x, TrackRR.transform.GetChild(0).transform.localPosition.y + 1.5f, 0);


            // Setting particle system main components
            DustParticlesFl = dustParticles[0].main;
            DustParticlesFr = dustParticles[1].main;
            DustParticlesRl = dustParticles[2].main;
            DustParticlesRr = dustParticles[3].main;


            //Start particles
            foreach (ParticleSystem objects in dustParticles)
            {
                objects.Play();
            }
        }




        // Exhaust Particles
        if (ExhaustParticles != "")
        {
            // Only one exhaust pipe
            exhaustParticles = Instantiate(Resources.Load("2D Tank Controller/" + ExhaustParticles, typeof(ParticleSystem)), ExhaustParticlePosition[0].transform.position, transform.rotation, ParticleParent.transform) as ParticleSystem;
            ExhaustParticleSystem = exhaustParticles.main;

            // Starting particles
            exhaustParticles.Play();

            // Two exhaust pipes
            if (ExhaustParticlePosition.Length == 2)
            {
                exhaustParticles2 = Instantiate(Resources.Load("2D Tank Controller/" + ExhaustParticles, typeof(ParticleSystem)), ExhaustParticlePosition[1].transform.position, transform.rotation, ParticleParent.transform) as ParticleSystem;
                ExhaustParticleSystem2 = exhaustParticles2.main;

                // Starting particles
                exhaustParticles2.Play();
            }       
        }
            



        // Shoot Particles
        if (ShootParticles.Length != 0)
        {
            shootParticles = new ParticleSystem[ShootParticles.Length];

            for (int i = 0; i < ShootParticles.Length; i++)
            {
                shootParticles[i] = Instantiate(Resources.Load("2D Tank Controller/" + ShootParticles[i], typeof(ParticleSystem)), ShootParticleParent.transform.position, transform.rotation, ShootParticleParent.transform) as ParticleSystem;
            }
        }
        

        
        
        // Explosion Particles
        if (ExplosionParticles.Length != 0)
        {
            explosionParticles = new ParticleSystem[ExplosionParticles.Length];

            for (int i = 0; i < ExplosionParticles.Length; i++)
            {
                explosionParticles[i] = Instantiate(Resources.Load("2D Tank Controller/" + ExplosionParticles[i], typeof(ParticleSystem)), ParticleParent.transform.position, transform.rotation, ParticleParent.transform) as ParticleSystem;
            }
        }         
    }





    //_________________________________________________UPDATE_______________________________________________________________________________________________________________________________________________________________________________________________________________________________



    private void Update()
    {
        
        //------------------------------------------------------------------------------------------------------------
        //                                             Input
        //------------------------------------------------------------------------------------------------------------

        verticalInput = Input.GetAxis(VerticalInputAxisName);
        horizontalInput = Input.GetAxis(HorizontalInputAxisName);        
        fireInput = Input.GetAxis(FireInputAxisName);

        if (TurretInputMode == TurretInputModeEnum.Keyboard)
            turretInput = Input.GetAxis(TurretInputAxisName);


        // Calling the Fire function if alive and reloaded
        if (fireInput == 1 && reloaded == true && IsExploded == false && InputEnabled == true)
        {
            Fire();
        }


        // Disabling input if destroyed
        if (IsExploded == true)
            DisableInput();


        // Disabling input if ´InputEnabled` is false
        if (InputEnabled == false)
            DisableInput();

        //--------machine gun---------------------------
        if (Input.GetKey(KeyCode.Space) && machineGunEnable && machineGunReloaded && machineGunRate >= machineGunRateFire && InputEnabled == true){
            FireMachineGun();
        }



        //------------------------------------------------------------------------------------------------------------
        //                                           Background Checking
        //------------------------------------------------------------------------------------------------------------


        //---------------------------------------------------
        //                    Reloading
        //---------------------------------------------------

        // Increasing timeReloaded
        if (reloaded == false && timeReloaded < ReloadTime)
        {
            timeReloaded += Time.deltaTime;
        }

        // Playing reload sound before reloading
        if (timeReloaded >= (ReloadTime - ReloadSoundLength) && reloadSoundPlayed == false && IsExploded == false && AudioReload != null)
        {
            AudioReload.Play();
            reloadSoundPlayed = true;
        }

        // When reloading is finished
        if (timeReloaded >= ReloadTime)
        {
            reloaded = true;
            timeReloaded = 0;
            reloadSoundPlayed = false;

            if (UseUI == true)
            TankReloadCG.alpha = 0;
        }

        //---------reloading machine gun------------------
        if(machineGunReloaded == false && machineGunEnable && machineGunReloadTime < machineGunReloadSpeed){
            machineGunReloadTime += Time.deltaTime;
        }

        if(machineGunReloadTime >= machineGunReloadSpeed && !machineGunReloaded){
            machineGunReloaded = true;
            machineGunReloadTime = 0;
            machineGunRounds = machineGunAmmo;
        }

        if( machineGunRate < machineGunRateFire){
            machineGunRate += Time.deltaTime;
        }



        //---------------------------------------------------
        //                    Ramming
        //---------------------------------------------------

        // Decreasing ramming time limit value
        if (rammingTimeLimitRemaining > 0)
        {
            rammingTimeLimitRemaining -= Time.deltaTime;
        }

        


        //---------------------------------------------------
        //                    Health
        //---------------------------------------------------

        // Checking HP if tank needs to be destroyed
        if (Health <= 0 && IsExploded == false)
        {
            DestroyTank();
        }



        //---------------------------------------------------
        //                Track repairing
        //---------------------------------------------------

        // When the tank is tracked (i.e. immobilized)
        if (tracked == true)
        {
            verticalInput = 0;
            horizontalInput = 0;

            if (lTrackTracked == true)
            {
                timeLTracksRepaired += Time.deltaTime;
            }

            if (rTrackTracked == true)
            {
                timeRTracksRepaired += Time.deltaTime;
            }
            
        }

        
        // When tracks are repaired

        // Right track
        if (timeLTracksRepaired >= TrackRepairTime)
        {
            lTrackHp = MaxTrackHealth;
            lTrackTracked = false;          
            timeLTracksRepaired = 0;

            TrackFL.GetComponentInChildren<SpriteRenderer>().enabled = false;
            TrackRL.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // Left track
        if (timeRTracksRepaired >= TrackRepairTime)
        {
            rTrackHp = MaxTrackHealth;
            rTrackTracked = false;
            timeRTracksRepaired = 0;

            TrackFR.GetComponentInChildren<SpriteRenderer>().enabled = false;
            TrackRR.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // If both tracks are back working
        if (lTrackTracked == false && rTrackTracked == false)
        {
            tracked = false;
            if (UseUI == true)
                TrackRepairCG.alpha = 0;
        }

        
        //------------------------------------
        //               UI
        //------------------------------------

        if (UseUI == true)
            UpdateUI();



        //------------------------------------------------------------------------------------------------------------
        //                                               Effects
        //------------------------------------------------------------------------------------------------------------

        // Audio
        AudioManager();       

        
        
        // Particles
        EmitParticles();



        // Trackmarks
        PlaceTrackMarks();
        


        //------------------------------------------------------------------------------------------------------------
        //                                               Movement
        //------------------------------------------------------------------------------------------------------------

        // Tank's speed
        currentSpeed = transform.InverseTransformDirection(Rigidbody.velocity).y;

        //Speed ratio
        if (currentSpeed > 0)
            speedRatio = 0.01f + currentSpeed * 3.6f / TopSpeed;
        else if (currentSpeed < 0)
            speedRatio = 0.01f + Mathf.Abs(currentSpeed) * 3.6f / ReverseSpeed;
        else if (currentSpeed == 0)
            speedRatio = 0.01f;
            


        // Driving forward (increasing accelerationMultiplier)
        if (verticalInput > 0.1f)
        {
            MoveForward();
        }


        // Driving backward (decrasing accelerationMultiplier)
        else if (verticalInput < 0)
        {
            MoveBackward();
        }


        // Not driving (decrasing accelerationMultiplier)
        else if (verticalInput == 0)
        {
            SlowDown();                
        }




        // Rotating tank (setting the target rotation)
        RotateTank();        


        // Rotating turret
        RotateTurret(); 
        
                
        
        // Moving the tank forward/backward
        Rigidbody.AddRelativeForce(new Vector2(0, EnginePower * accelerationMultiplier * 200000 * Time.deltaTime));



        
        // Calculating pushing force for destroying structures (buildings and walls)
        PushingForce = (Mathf.Pow(Rigidbody.velocity.magnitude, 2) * Rigidbody.mass + EnginePower * (Mathf.Abs(verticalInput) + (Mathf.Abs(horizontalInput) * 0.1f)) * Rigidbody.mass) / 10000;
    }//end of update
    private void FireMachineGun(){
            machineGunRounds-=1;
            machineGunFireAudio.Play();

            Vector3 PortPosition = machineGunPort.transform.position;
            int random = Random.Range(0,3);//random turrent to hull
            if(random == 0){
                PortPosition = machineGunPort.transform.position + hullOriginVector;
            }
            float RandomSpread = Random.Range(-machineGunSpread,machineGunSpread);
            Instantiate(machineGunShell, PortPosition, Quaternion.Euler(0, 0 , Turret.transform.eulerAngles.z + RandomSpread));
            if(machineGunRounds <= 0){
                machineGunReloaded = false;
                machineGunReloadAudio.Play();
                machineGunRounds=0;
            }
            machineGunRate = 0;//reset 
    }


    //____________________________________________FIXED UPDATE______________________________________________________________________________________________________________________________________________________________________________________________________________________________



    void FixedUpdate()
    {
        // Applying hull rotation
        if (horizontalInput != 0)
            Rigidbody.MoveRotation(targetHullRotation);

        isColliding = false;
    }








    //_________________________________________________LATE UPDATE___________________________________________________________________________________________________________________________________________________________________________________________________________________________


    
    private void LateUpdate()
    {
        // Setting tank UI position in late update for smoother motion
        if (UseUI == true)
            TankUI.transform.position = UiPos;

        // Setting aimpoint position
        if (Aimpoint != null && TurretInputMode == TurretInputModeEnum.Mouse)
            Aimpoint.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3 (0,0,-1);
    }
    //----------------------------------------Additional added stuff
    public void ChangeUI(bool result){
        if(result){
            camoNet = true;
            TankUI.GetComponent<CanvasGroup>().alpha = 0f;
        }else{
            camoNet = false;
            TankUI.GetComponent<CanvasGroup>().alpha = 1f;
        }
    }

    public bool CheckCamo(){
        return camoNet;
    }

    public bool CheckHit(){
        //resets it
        if(wasHit){
            return true;
        }else{
            return false;
        }
    }

    public void ResetWasHit(){
        wasHit = false;
    }

    public void ShouldFire(){
        // Calling the Fire function if alive and reloaded
        if (reloaded == true && IsExploded == false)
        {
            Fire();
        }else if(reloaded == false&& timeReloaded+1 < ReloadTime &&IsExploded == false && machineGunEnable && machineGunReloaded && machineGunRate >= machineGunRateFire){
            FireMachineGun();
        }
    }
}
