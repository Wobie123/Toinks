using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShellController : MonoBehaviour
{
    /*      Description:
     *      
     *      This script controls the shell and all its functionality, so all shells should have this script attached to them.   *      
     *      
     */ 
    
        
    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                  Stats                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(20)]
    [Header("______________________STATS____________________________________________________________________________________________________________________________________________________________")]
    [Space(20)]

    [Tooltip("The caliber of the shell (mm).")]
    public int Caliber;    

    [Tooltip("The type of the shell. AP = Armor Piercing, HE = High Explosive.")]
    public shellTypeEnum shellType;

    public enum shellTypeEnum
    {
        AP, HE
    };

    [Tooltip("The average damage of the shell.")]
    public int Damage;

    [Tooltip("The average penetration of the shell (mm).")]
    public int Penetration;

    [Tooltip("The velocity of the shell (m/s).")]
    public int Velocity;

    [Tooltip("Maximum lifetime of the shell (s).")]
    public float Lifetime;




    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                             Layers & tags                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(20)]
    [Header("______________________LAYERS & TAGS____________________________________________________________________________________________________________________________________________________________")]
    [Space(20)]

    [Tooltip("The index of the hull armor layer.")]
    public int HullArmorLayerIndex;

    [Tooltip("The index of the turret armor layer.")]
    public int TurretArmorLayerIndex;

    [Tooltip("The index of the layer on which the shell collides with hull armor layer")]
    public int ShellHullLayerIndex;

    [Tooltip("The name of the tag of the tracks. All tracks should have this tag.")]
    public string TrackTagName;

    [Tooltip("Check the tank physics model layer here.")]
    public LayerMask TankPhysicsModelLayerMask;




    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                              GameObjects                                                                 
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(20)]
    [Header("_______________________GAME OBJECTS____________________________________________________________________________________________________________________________________________________________")]
    [Space(20)]

    [Tooltip("Visual model of the shell")]
    public GameObject VisualModel;

    private TankController TankController;


    //---------------------------------------------------
    //                     Audio
    //---------------------------------------------------

    [Header("Audio")]
    [Space(10)]

    [Tooltip("The sound that will be played when the shell hits anything but a tank")]
    public GameObject AudioHitGround;

    [Tooltip("The sound that will be played when the shell ricochets off another tank (hits in a very steep angle).")]
    public GameObject AudioRicochet;

    [Tooltip("The sound that will be played when the shell bounces off another tank.")]
    public GameObject AudioBounce;

    [Tooltip("The sound that will be played when the shell goes through the armor of another tank")]
    public GameObject AudioPenetrate;

    [Tooltip("[Optional] The sound that will be played when the shell hits the track of another tank")]
    public GameObject AudioTrackHit;


    //---------------------------------------------------
    //                     Particles
    //---------------------------------------------------

    [Header("Particles")]
    [Space(10)]

    [Tooltip("The parent GameObject for particle effects ...")] // You don't have to have a specific GameObject for this, so you can just use the main GameObject. However, having a separate empty GameObject keeps things nice and organized.
    public GameObject ParticleParent;

    [Tooltip("Path for the tracer particles of the shell in the Resources folder.")]
    public String TrailParticles;

    [Tooltip("Path(s) for the particles that will be shown when the shell penetrates another tank in the Resources folder.")]
    public String[] PenetrationParticles;

    [Tooltip("Path(s) for the particles that will be shown when the shell bounces off another tank in the Resources folder.")]
    public String[] BounceParticles;

    [Tooltip("Path(s) for the particles that will be shown when the shell ricochets off another tank in the Resources folder.")]
    public String[] RicochetParticles;

    [Tooltip("Path(s) for the particles that will be shown when the shell hits anything but a tank in the Resources folder.")]
    public String[] DustParticles;

    private ParticleSystem trailParticles;
    private ParticleSystem[] penetrationParticles;
    private ParticleSystem[] bounceParticles;
    private ParticleSystem[] ricochetParticles;
    private ParticleSystem[] dustParticles;



    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                              Private Variables                                                                
    //------------------------------------------------------------------------------------------------------------------------------------------------

    private float timeExisted;

    private bool damageGiven;
    private bool targetHit;
    private bool collisionDetected;

    private RaycastHit2D tankHit;
    private bool shootOnHullLayer;
    private float distanceTraveled;



    private Vector3 bulletTransformUp;
    
    private int impactAngle;
    private int impactArmorAngle;
    private int compoundAngle;

    private int impactArmor;
    private float effectiveArmor;
    private int overmatchArmor;

    private int rngPen;
    private float rngPenF;
    private int rngDmg;
    private float rngDmgF;

    private float hePenetrationRatio;    

    private float originalPen;
    private float reducedPen;

    private string paramSide;
    private string paramWheel;    

    private Color transparentColor;

    private float pitchRandomizer;

    
    



    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                  Functions                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------

    // Function that will be executed when the shell successfully penetrates the armor of another tank
    private void Penetrate()
    {
        // Show particles & play sounds
        foreach (ParticleSystem objects in penetrationParticles)
        {
            objects.Play();
        }

        // Randomizing damage
        rngDmgF = (float)Damage * UnityEngine.Random.Range(0.9f, 1.1f);
        rngDmg = (int)rngDmgF;

        TankController.TakeDamage(rngDmg);

        // Sound effects
        AudioPenetrate.GetComponent<AudioSource>().pitch *= pitchRandomizer;
        AudioPenetrate.GetComponent<AudioSource>().Play();

        // Deleting shell
        EndLife();
    }


    // Function that will be executed when the (AP) shell bounces off a tank
    private void Bounce()
    {
        // Show particles & play sounds
        foreach (ParticleSystem objects in bounceParticles)
        {
            objects.Play();
        }

        // Sound effects
        AudioBounce.GetComponent<AudioSource>().pitch *= pitchRandomizer;
        AudioBounce.GetComponent<AudioSource>().Play();

        // Deleting shell
        EndLife();
    }

    // Function that will be executed when the (AP) shell ricochets off a tank. Ricochet occures when the angle is 70° or higher.
    private void Ricochet()
    {
        // Show particles & play sounds
        foreach (ParticleSystem objects in ricochetParticles)
        {
            objects.Play();
        }

        // Sound effect
        AudioRicochet.GetComponent<AudioSource>().pitch *= pitchRandomizer;
        AudioRicochet.GetComponent<AudioSource>().Play();

        damageGiven = true;
        targetHit = true;
    }

    // Function that will be executed when the shell this anyhing but a tank
    private void HitGround()
    {
        // Show particles & play sounds
        foreach (ParticleSystem objects in dustParticles)
        {
            objects.Play();
        }

        // Sound effect
        AudioHitGround.GetComponent<AudioSource>().pitch *= pitchRandomizer;
        AudioHitGround.GetComponent<AudioSource>().Play();        

        // Deleting shell
        EndLife();
    }


    // Function that will be executed when a High Explosive shell fails to penetrate. HE shell causes ´splash damage` even if it fails to fully penetrate the armor.
    private void HeBounce()
    {
        // Show particles & play sounds
        foreach (ParticleSystem objects in bounceParticles)
        {
            objects.Play();
        }       

        
        // Damage will be reduced by 50% and then the depending on the nominal thickness of armor it will be reduced further
        Damage = (int)((float)Damage * 0.5f);

        
        // Calculating the ratio between the penetration of the shell and thickness of the armor and decreasing the damage accordingly
        hePenetrationRatio = rngPenF / impactArmor;
        Damage = (int)((float)Damage * hePenetrationRatio);


        // Randomizing damage
        rngDmgF = (float)Damage * UnityEngine.Random.Range(0.9f, 1.1f);
        rngDmg = (int)rngDmgF;

        TankController.TakeDamage(rngDmg);

        
        // Sound effect
        AudioBounce.GetComponent<AudioSource>().pitch *= pitchRandomizer;
        AudioBounce.GetComponent<AudioSource>().Play();

        // Deleting shell
        EndLife();
    }


    // Function that hides the shell and prevents it from interacting with anything without destroying it right immediately. If the shell was deleted right away, all sound effects would stop too.
    private void EndLife()
    {
        VisualModel.GetComponent<SpriteRenderer>().enabled = false;

        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Rigidbody2D>().isKinematic = true;

        trailParticles.GetComponent<ParticleSystem>().Stop();
    }

    
    

    
    //____________________________________________EVENTS_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________  




    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Randomizing a value for sounds
        pitchRandomizer = UnityEngine.Random.Range(0.9f, 1.1f);

        //-----------------------------------------------------------------------------------------
        //  AP shell type
        //-----------------------------------------------------------------------------------------

        if (shellType == shellTypeEnum.AP)
        {
            // Shell hits something that is not a tank
            if (collision.collider.gameObject.layer != HullArmorLayerIndex && collision.collider.gameObject.layer != TurretArmorLayerIndex && collision.collider.gameObject.tag != TrackTagName)
            {
                HitGround();
            }            
            

            
            // Shell hits a tank
            else if (collision.collider.gameObject.layer == HullArmorLayerIndex || collision.collider.gameObject.layer == TurretArmorLayerIndex)
            {
                // Assigning TankController
                TankController = collision.gameObject.GetComponentInParent<TankController>();

                
                // Bounce if tank is dead
                if (TankController == null)
                    Bounce();

                
                // If tank is alive -> Continue
                else if (TankController != null)
                {
                    // Base angle of the armor plate
                    impactArmorAngle = collision.collider.GetComponent<ArmorController>().ArmorAngle;


                    // Calculating the angle of impact
                    if (collisionDetected == false)
                        impactAngle = 180 - (int)Vector2.Angle(collision.contacts[collision.contacts.Length - 1].normal, bulletTransformUp) + 1;


                    // Calculating the compund angle (combining the base angle of the plate with the actual visible angle of the plate)
                    compoundAngle = (int)((Mathf.Acos(Mathf.Cos((impactAngle) * Mathf.PI / 180) * Mathf.Cos((impactArmorAngle) * Mathf.PI / 180)) / Mathf.PI * 180));

                    
                    // Thickness of the armor plate
                    impactArmor = collision.collider.GetComponent<ArmorController>().ArmorThickness;

                    
                    // Calculating the effective thickness of the armor plate
                    effectiveArmor = impactArmor / Mathf.Cos(Mathf.Deg2Rad * compoundAngle);

                    
                    // Calculating the amount of armor the shell is able to overmatch.
                    // Overmatch occures when a shell hits a very thin armor plate that simply can't ricochet the shell no matter how steep the angle is. In this case the caliber has to be 3 times the thickness of the armor plate.
                    overmatchArmor = Caliber / 3;

                    
                    // Calculating overmatch
                    if (overmatchArmor > impactArmor && damageGiven == false)
                    {
                        Penetrate();
                    }

                    
                    // Calculating penetration, bounce or ricochet if cannot overmatch
                    else if (overmatchArmor < effectiveArmor)
                    {
                        // Randomizing penetration
                        rngPenF = (float)Penetration * UnityEngine.Random.Range(0.9f, 1.1f);
                        rngPen = (int)rngPenF;

                        // Penetration
                        if (rngPen > effectiveArmor && compoundAngle < 70 && damageGiven == false)
                        {
                            Penetrate();
                        }

                        // Bounce
                        else if (rngPen < effectiveArmor && compoundAngle < 70 || damageGiven == true)
                        {
                            Bounce();
                        }

                        // Ricochet
                        else if (compoundAngle > 70)
                        {
                            Ricochet();
                        }
                        else
                            Bounce();
                    }

                    else
                        Bounce();
                }                
            }
        }




        //-----------------------------------------------------------------------------------------
        //  HE shell type
        //-----------------------------------------------------------------------------------------

        else if (shellType == shellTypeEnum.HE)
        {
            // Shell hits something that is not a tank
            if (collision.collider.gameObject.layer != HullArmorLayerIndex && collision.collider.gameObject.layer != TurretArmorLayerIndex && collision.collider.gameObject.tag != TrackTagName)
            {
                HitGround();
            }
            


            // Shell hits a tank
            else if (collision.collider.gameObject.layer == HullArmorLayerIndex || collision.collider.gameObject.layer == TurretArmorLayerIndex)
            {
                // Assigning TankController
                TankController = collision.gameObject.GetComponentInParent<TankController>();

                
                // Bounce if tank is already dead
                if (TankController == null)
                    Bounce();

                
                // If tank is alive -> Continue
                else if (TankController != null)
                {
                    // Base angle of the armor plate
                    impactArmorAngle = collision.collider.GetComponent<ArmorController>().ArmorAngle;


                    // Calculating the angle of impact
                    if (collisionDetected == false)
                        impactAngle = 180 - (int)Vector2.Angle(collision.contacts[collision.contacts.Length - 1].normal, bulletTransformUp) + 1;


                    // Calculating the compund angle (combining the base angle of the plate with the actual visible angle of the plate)
                    compoundAngle = (int)((Mathf.Acos(Mathf.Cos((impactAngle) * Mathf.PI / 180) * Mathf.Cos((impactArmorAngle) * Mathf.PI / 180)) / Mathf.PI * 180));


                    // Thickness of the armor plate
                    impactArmor = collision.collider.GetComponent<ArmorController>().ArmorThickness;


                    // Calculating the effective thickness of the armor plate
                    effectiveArmor = impactArmor / Mathf.Cos(Mathf.Deg2Rad * compoundAngle);


                    // Calculating the amount of armor the shell can overmatch
                    overmatchArmor = Caliber / 3;

                    
                    // Calculating overmatch
                    if (overmatchArmor > impactArmor && damageGiven == false)
                    {
                        Penetrate();
                    }

                    
                    // Calculating penetration, bounce or ricochet if cannot overmatch
                    else if (overmatchArmor < effectiveArmor)
                    {
                        // Randomizing penetration
                        rngPenF = (float)Penetration * UnityEngine.Random.Range(0.9f, 1.1f);
                        rngPen = (int)rngPenF;

                        // Penetration
                        if (rngPen > effectiveArmor && damageGiven == false)
                        {
                            Penetrate();
                        }

                        // Not a penetration
                        else if (rngPen < effectiveArmor)
                        {
                            HeBounce();
                        }
                    }
                }
            }
        }

        collisionDetected = true;
    }

    
    
    
    // Shell hits a track
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Track" && collider.gameObject.GetComponentInParent<TankController>() != null)
        {
            // Setting TankController and decreasing the penetration of the shell
            TankController = collider.gameObject.GetComponentInParent<TankController>();
            Penetration -= collider.GetComponent<ArmorController>().ArmorThickness;
            targetHit = true;

            // Playing particle effect
            foreach (ParticleSystem objects in penetrationParticles)
            {
                objects.Play();
            }

            // Playing sound effect
            if (AudioTrackHit != null)
                AudioTrackHit.GetComponent<AudioSource>().Play();

            // Randomizing damage
            rngDmgF = (float)Damage * UnityEngine.Random.Range(0.9f, 1.1f);
            rngDmg = (int)rngDmgF;


            // Clamping penetration minimum value to 0
            if (Penetration < 0)
                Penetration = 0;


            // Setting parameters for TakeTrackDamage function to cause damage to the correct track
            switch (collider.gameObject.name)
            {
                case "TrackFL":
                    paramSide = "l";
                    paramWheel = "fl";
                    break;
                case "TrackFR":
                    paramSide = "r";
                    paramWheel = "fr";
                    break;
                case "TrackRL":
                    paramSide = "l";
                    paramWheel = "rl";
                    break;
                case "TrackRR":
                    paramSide = "r";
                    paramWheel = "rr";
                    break;
            }

            // Calling the function in TankController to cause the damage
            TankController.TakeTrackDamage(rngDmg, paramSide, paramWheel);
        }
    }






    //____________________________________________START_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________


    void Start()
    {
        // Changing between hull and turret layers based on the z position of the shell. The position is set by the TankController when this shell is instantiated.

        // Hull
        if (transform.position.z == 0)
        {
            shootOnHullLayer = true;
        }

        // Turret
        else if (transform.position.z == -1)
        {
            shootOnHullLayer = false;
        }

        // Setting the forward vector of the shell
        bulletTransformUp = transform.up;

        // Adding the force that shoots the shell
        GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, Velocity * GetComponent<Rigidbody2D>().mass), ForceMode2D.Impulse);


        // Other variables
        damageGiven = false;
        collisionDetected = false;

        originalPen = Penetration;
        transparentColor = VisualModel.GetComponent<SpriteRenderer>().color;



        //---------------------------------------------------
        //                     Particles
        //---------------------------------------------------

        // Instantiating particles as a child of the ParticleParent GameObject.

        // Penetration Particles
        penetrationParticles = new ParticleSystem[PenetrationParticles.Length];

        for (int i = 0; i < PenetrationParticles.Length; i++)
        {
            penetrationParticles[i] = Instantiate(Resources.Load("2D Tank Controller/" + PenetrationParticles[i], typeof(ParticleSystem)), ParticleParent.transform.position, ParticleParent.transform.rotation, ParticleParent.transform) as ParticleSystem;
        }

        // Bounce Particles
        bounceParticles = new ParticleSystem[BounceParticles.Length];

        for (int i = 0; i < BounceParticles.Length; i++)
        {
            bounceParticles[i] = Instantiate(Resources.Load("2D Tank Controller/" + BounceParticles[i], typeof(ParticleSystem)), ParticleParent.transform.position, ParticleParent.transform.rotation, ParticleParent.transform) as ParticleSystem;
        }

        // Ricochet Particles
        ricochetParticles = new ParticleSystem[RicochetParticles.Length];

        for (int i = 0; i < RicochetParticles.Length; i++)
        {
            ricochetParticles[i] = Instantiate(Resources.Load("2D Tank Controller/" + RicochetParticles[i], typeof(ParticleSystem)), ParticleParent.transform.position, ParticleParent.transform.rotation, ParticleParent.transform) as ParticleSystem;
        }

        // Dust Particles
        dustParticles = new ParticleSystem[DustParticles.Length];

        for (int i = 0; i < DustParticles.Length; i++)
        {
            dustParticles[i] = Instantiate(Resources.Load("2D Tank Controller/" + DustParticles[i], typeof(ParticleSystem)), ParticleParent.transform.position, ParticleParent.transform.rotation, ParticleParent.transform) as ParticleSystem;
        }

        //Trail Particles
        trailParticles = Instantiate(Resources.Load("2D Tank Controller/" + TrailParticles, typeof(ParticleSystem)), ParticleParent.transform.position, ParticleParent.transform.rotation, ParticleParent.transform) as ParticleSystem;
    }




    //____________________________________________UPDATE_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________


    void Update ()
    {
        // Calculating lifetime
        timeExisted += Time.deltaTime;

        
        // If lifetime is used -> Destroy shell
        if (timeExisted >= Lifetime)
        {
            GameObject.Destroy(gameObject);
        }

        
        // Reducing the penetration value gradually after penetrating a spaced layer (i.e. tracks) or ricocheting off a tank
        if (targetHit == true)
        {
            if (Penetration > 0)
            {
                // Penetration is reduced by 100 mm every second
                reducedPen += 200 * Time.deltaTime;

                if (reducedPen > 1)
                {
                    Penetration -= (int)reducedPen;
                    reducedPen = 0;

                    // Changing alpha based on the ratio of the current and original penetration values
                    transparentColor.a = Penetration / originalPen;
                    VisualModel.GetComponent<SpriteRenderer>().color = transparentColor;
                }
            } 
            
            // Deleting shell when penetration is 0 or shell has stopped (or goes under 5 m/s)
            else if (Penetration <= 0 || GetComponent<Rigidbody>().velocity.magnitude < 5)
            {
                EndLife();
            }           
        }


        // Changing to hull layer if the shell "is told to do so" by the TankController. Layer will be changed after the shell is at least 2 meters away from the tank that shot it.
        if (shootOnHullLayer == true)
        {
            tankHit = Physics2D.Raycast(transform.position - transform.up * 0.2f, new Vector3(0, 0, 10), TankPhysicsModelLayerMask.value);

            if (!tankHit)
            {
                distanceTraveled += Time.deltaTime * Velocity;

                if (distanceTraveled > 2.0f)
                    gameObject.layer = ShellHullLayerIndex;
            }                     
        }       
    }
}
