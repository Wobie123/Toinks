using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoSceneController : MonoBehaviour
{

    private RaycastHit2D raycastHit;
	

	void Update ()
    {
        // Enable/disable tanks
        if (Input.GetMouseButtonDown(1))
        {
            raycastHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 10));

            // Raycast hits a tank
            if (raycastHit == true && raycastHit.collider.gameObject.GetComponent<TankController>() != null)
            {
                // Enabling input
                if (raycastHit.collider.gameObject.GetComponent<TankController>().InputEnabled == false)
                    raycastHit.collider.gameObject.GetComponent<TankController>().InputEnabled = true;

                // Disabling input
                else if (raycastHit.collider.gameObject.GetComponent<TankController>().InputEnabled == true)
                {
                    raycastHit.collider.gameObject.GetComponent<TankController>().InputEnabled = false;
                    Cursor.visible = true;
                }
                    
            }
        }        
        
        
        // ECS - Scene reset
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene(0);
        }

        // ALT - Show/hide cursor
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            // Showing cursor
            if (Cursor.visible == false)
                Cursor.visible = true;

            //Hiding cursor
            else if (Cursor.visible == true)
                Cursor.visible = false;
        }
    }
}
