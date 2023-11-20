using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetection : MonoBehaviour
{
    public float detectionRadius = 5f; // The radius for detecting objects.
    public LayerMask objectLayer; // The layer mask to filter which objects to detect.

    [Header("Read only")]
    public Collider2D[] hitColliders;
    private void Update()
    {
        // Cast a circle in 360 degrees around the player to detect objects.
        hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, objectLayer);

        // Loop through all the detected objects and print their names.
        foreach (Collider2D collider in hitColliders)
        {
            GameObject detectedObject = collider.gameObject;
            Debug.Log("Detected Object: " + detectedObject.name);

            // You can also perform actions on the detected objects here.
            // For example, you could destroy or interact with them.
        }
    }

    // Visualize the detection radius in the Unity Editor.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

