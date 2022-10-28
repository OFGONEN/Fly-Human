using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "vehicle_data_movement_", menuName = "FF/Game/Vehicle Movement Data" ) ]
public class VehicleMovementData : ScriptableObject
{
  [ Title( "Movement Ground" ) ]
    public float movement_ground_speed_default; 
    public float movement_ground_speed_max; 
    public float movement_ground_speed_accelerate_duration; 
    public Ease movement_ground_speed_accelerate_ease; 
//   [ Title( "Movement Air" ) ]
//   [ Title( "Landing" ) ]
}