using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class FallOver : NetworkBehaviour
{
   public BoxCollider2D col;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision){
        Debug.Log("collide with" + collision.gameObject.name);
        //Vector3 eulerRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, collision.transform.eulerAngles.z);

        //transform.rotation = Quaternion.Euler(eulerRotation);
        //Destroy(col);

        foreach(ContactPoint2D contact in collision.contacts){
            float angle = Mathf.Atan2(contact.normal.x,contact.normal.y) * 180 / Mathf.PI;
            if(angle < 0){
                angle = Mathf.Abs(angle);
            }else
                angle = angle * -1;
            //transform.rotation = Quaternion.Euler(0f,0f,angle);
            //Destroy(col);
            FallOverServerRpc(angle);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void FallOverServerRpc(float angle){
        transform.rotation = Quaternion.Euler(0f,0f,angle);
        FallOverClientRPC();
    
    }   

    [ClientRpc]

    private void FallOverClientRPC(){
        Destroy(col);
    }

}
