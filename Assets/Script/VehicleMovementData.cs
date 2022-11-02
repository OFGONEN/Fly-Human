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
    [ LabelText( "Movement Ground Minimum Eject Speed" ) ] public float movement_ground_speed_eject_min; 
    [ LabelText( "Movement Ground Perfect Eject Speed" ) ] public float movement_ground_speed_eject_perfect; 

  [ Title( "Movement Air" ) ]
  	[ LabelText( "Movement Air Rotate Speed" ) ] public float movement_air_rotate_speed;
	[ LabelText( "Movement Air Rotate Speed Max" ) ] public float movement_air_rotate_speed_max;

  [ Title( "Landing" ) ]
  	[ LabelText( "Landing Adjust Rotation Ease" ) ] public Ease landing_adjust_rotation_ease;
	[ LabelText( "Landing Adjust Jump Power" ) ] public float landing_adjust_jump_power;
	[ LabelText( "Landing Adjust Jump Ease" ) ] public Ease landing_adjust_jump_ease;

#if UNITY_EDITOR
    private void OnValidate()
    {
		movement_ground_speed_eject_perfect = Mathf.Clamp( movement_ground_speed_eject_perfect, movement_ground_speed_default, movement_ground_speed_max );
	}

	[ Button() ]
	private void SetMinAndMaxEjectSpeed( float min, float max )
	{
		UnityEditor.EditorUtility.SetDirty( this );
		movement_ground_speed_eject_min = movement_ground_speed_max * min / 100f;
		movement_ground_speed_eject_perfect = movement_ground_speed_max * max / 100f;
	}
#endif
}