using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDestructionController : MonoBehaviour
{
    /*      Description:
     *      
     *      This script should be attached to destructible structures such as buildings and walls.
     *      This script makes structures vulnerable tank shells and ramming. 
     */


    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                Settings                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(10)]
    [Header("Settings")]
    [Space(10)]

    [Tooltip("The HP of the structure. Strength of 500 means that that the building takes 5 shells with a damage of 100 to be destroyed.")]
    public int Strength;

    [Space(10)]
    [Tooltip("The name of the tag of the tank. All tanks should have this one specific tag.")]
    public string TankTagName;

    [Tooltip("[Read-Only] Current HP of the structure. Use the Strength value to set the HP to ensure correct behaviour.")]
    public float HP;


    
    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                              GameObjects                                                                 
    //------------------------------------------------------------------------------------------------------------------------------------------------

    [Space(10)]
    [Header("GameObjects")]
    [Space(10)]

    [Tooltip("[Optional] Path for the particles that will be shown when the structure is destroyed in the Resources folder.")]
    public string Particles;
    private ParticleSystem particles;

    [Tooltip("[Optional] The sprite that will be shown when the structure has less than 25% of its HP remaining.")]
    public SpriteRenderer DamagedSprite;

    [Tooltip("[Optional] Path for the sprite that will be shown when the structure is destroyed in the Resources folder. ")]
    public string DestroyedSprite;

    [Tooltip("[Optional] Sound effect played when the structure is destroyed. ")]
    public AudioSource DestroySound;


    //------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                               Functions                                                                  
    //------------------------------------------------------------------------------------------------------------------------------------------------


    // Function that handles everything when the structure needs to be destroyed
    private void DestroyStructure()
    {
        if (DestroySound != null)
            DestroySound.Play();

        GetComponent<BoxCollider2D>().enabled = false;

        if (DamagedSprite != null)
            DamagedSprite.enabled = false;


        if (DestroyedSprite != "")
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load("2D Tank Controller/" + DestroyedSprite, typeof(Sprite)) as Sprite;
            GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0.5f);
            GetComponent<SpriteRenderer>().sortingOrder = 1;
            GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        }

        else if (DestroyedSprite == "")
            GetComponent<SpriteRenderer>().enabled = false;


        if (Particles != "")
            particles.Play();
    }

    


    //____________________________________________START_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________


    void Start ()
    {
        HP = Strength;

        // Instantiating particles as a child object (if the path is set)
        if (Particles != "")
            particles = Instantiate(Resources.Load("2D Tank Controller/" + Particles, typeof(ParticleSystem)), transform.position, transform.rotation, transform) as ParticleSystem;
    }




    //____________________________________________EVENTS_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________

    
    // CollisionEnter detection for incoming shells
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reducing hp
        if (collision.gameObject.GetComponent<ShellController>() != null)
            HP -= collision.gameObject.GetComponent<ShellController>().Damage;
       
        // Displaying the damaged sprite if 33% of hp remaining
        if (HP < (float)Strength * 0.33f && DamagedSprite != null)
        {
            DamagedSprite.enabled = true;
        }

        
        // Destroying the structure if hp is 0
        if (HP <= 0)
        {
            DestroyStructure();
        }
    }


    // CollisionStay detection for ramming tanks
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Tank ramming the building/wall
        if (collision.gameObject.tag == TankTagName)
        {
            // If the tank is alive -> Damage will be calculated based on the tanks pushing force
            if (collision.gameObject.GetComponent<TankController>() != null)
                HP -= collision.gameObject.GetComponent<TankController>().PushingForce * 0.5f * Time.deltaTime;
            
            // If the tank is dead -> Constant value will be used instead
            else
                HP -= 50 * Time.deltaTime;
        }


        // Displaying the damaged sprite if 33% of hp remaining
        if (HP < (float)Strength * 0.33f && DamagedSprite != null)
        {
            DamagedSprite.enabled = true;
        }


        // Destroying the structure if hp is 0
        if (HP <= 0)
        {
            DestroyStructure();
        }
    }
}
