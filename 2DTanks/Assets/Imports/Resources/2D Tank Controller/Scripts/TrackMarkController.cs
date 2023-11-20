using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TrackMarkController : NetworkBehaviour
{
    /*      Description:
     *      
     *      This script makes the track mark fade away.
     *      Once the alpha is down to 0, the track mark will be destroyed. 
     */


    private SpriteRenderer spriteRenderer;
    private Color trackColor;
    private float alpha;


    private void Start()
    {
        // Hides itself from the Hierarchy list in the editor
        gameObject.hideFlags = HideFlags.HideInHierarchy;

        alpha = 1;
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        trackColor = spriteRenderer.color;
    }


    void Update ()
    {
        alpha -= 0.1f * Time.deltaTime;
        trackColor.a = alpha;
        spriteRenderer.color = trackColor;

        if (alpha <= 0)
            DestroyServerRpc();
	}

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc(){
        //ChildSpawn.Despawn(true);
        gameObject.GetComponent<NetworkObject>().Despawn();
        GameObject.Destroy(gameObject);
    }
}
