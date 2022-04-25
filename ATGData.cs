using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New All Data", menuName = "Data/ATG Data")]
public class ATGData : ScriptableObject
{
    public float walkSpeed, chaseSpeed, chaseTime, radius, timeToWait, angle, hearValue;
}
