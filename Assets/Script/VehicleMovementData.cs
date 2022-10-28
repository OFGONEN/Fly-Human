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
    [ LabelText( "Movement Ground Speed Default" ) ] public float movement_ground_speed_default; 
    [ LabelText( "Movement Ground Speed Max" ) ] public float movement_ground_speed_max; 
    [ LabelText( "Movement Ground Speed Accelerate Duration" ) ] public float movement_ground_speed_accelerate_duration; 
    [ LabelText( "Movement Ground Speed Accelerate Ease" ) ] public Ease movement_ground_speed_accelerate_ease; 
//   [ Title( "Movement Air" ) ]
  [ Title( "Landing" ) ]
  	[ LabelText( "Landing Adjust Rotation Ease" ) ] public Ease landing_adjust_rotation_ease;
		[ LabelText( "Landing Adjust Jump Power" ) ] public float landing_adjust_jump_power;
		[ LabelText( "Landing Adjust Jump Ease" ) ] public Ease landing_adjust_jump_ease;
}