using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BuildingNetworkDestructionController : NetworkBehaviour
{
/*      Description:
     *      
     *      This script should be attached to destructible structures such as buildings and walls.
     *      This script makes structures vulnerable tank shells and ramming. 
     *      THis is a copy script for networking
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

   
    public float HP;

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

    private void OnHPChanged(float oldHP, float newHP)
    {
        // Hook method called when HP is changed
        if (newHP <= 0)
        {
            DestroyStructure();
        }
        else if (newHP < Strength * 0.33f && DamagedSprite != null)
        {
            DamagedSprite.enabled = true;
        }
    }
    private void DestroyStructure()
    {
        if (DestroySound != null)
            DestroySound.Play();

        GetComponent<BoxCollider2D>().enabled = false;

        if (DamagedSprite != null)
            DamagedSprite.enabled = false;

        if (!string.IsNullOrEmpty(DestroyedSprite))
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load("2D Tank Controller/" + DestroyedSprite, typeof(Sprite)) as Sprite;
            GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0.5f);
            GetComponent<SpriteRenderer>().sortingOrder = 1;
            GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        if (!string.IsNullOrEmpty(Particles))
            particles.Play();
    }

    void Start()
    {
        if (IsServer)
        {
            // Only initialize on the server to avoid duplicated instances
            HP = Strength;

            if (!string.IsNullOrEmpty(Particles))
                particles = Instantiate(Resources.Load("2D Tank Controller/" + Particles, typeof(ParticleSystem)), transform.position, transform.rotation, transform) as ParticleSystem;
        }
    }
//____________________________________________EVENTS_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________

    
    // CollisionEnter detection for incoming shells
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.GetComponent<ShellController>() != null)
                HP -= collision.gameObject.GetComponent<ShellController>().Damage;
        // If the tank is dead -> Constant value will be used instead
            else
                HP -= 50 * Time.deltaTime;
            
           UpdateHPServerRpc(HP);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.tag == TankTagName)
            {
                if (collision.gameObject.GetComponent<TankController>() != null)
                    HP -= collision.gameObject.GetComponent<TankController>().PushingForce * 0.5f * Time.deltaTime;
                else
                    HP -= 50 * Time.deltaTime;

                UpdateHPServerRpc(HP);
            }

        }
    }

     // Custom RPC to synchronize HP across the network
    [ServerRpc(RequireOwnership = false)]
    void UpdateHPServerRpc(float newHP)
    {
        HP = newHP;
        UpdateHPClientRpc(HP);
    }

    // RPC to notify clients when HP changes
    [ClientRpc]
    void UpdateHPClientRpc(float newHP)
    {
        OnHPChanged(HP, newHP);
    }
}