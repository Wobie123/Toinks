using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorController : MonoBehaviour
{
    /*      Description:
     *      
     *      This script should be attached to all armor pieces (i.e. GameObjects with a collider that form the armor model).
     */


    [Tooltip("The thickness of the armor plate (mm)")]
    public int ArmorThickness;

    [Tooltip("The base angle of the armor plate (°). 0 means a completely flat armor plate (default) - 80 means a plate that is in a extremely steep angle - 60 is the angle of the upper front plate of the T-54 tank.")]
    public int ArmorAngle;
}
