/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using UnityEditor;

public class VehicleMovement : MonoBehaviour
{
#region Fields

// Private
    VehicleData vehicle_data;
	UnityMessage onFingerDown;
	UnityMessage onFingerUp;
    UnityMessage onFixedUpdateMethod;

    [ ShowInInspector, ReadOnly ] Vector3 vehicle_point_origin;
    [ ShowInInspector, ReadOnly ] Vector3 vehicle_point_target;

    int layerMask;    
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		EmptyOutDelegates();
		layerMask = LayerMask.GetMask( ExtensionMethods.Layer_Platform );
	}

    private void Start()
    {
		PlaceVehicleOnPlatform();
	}

    private void FixedUpdate()
    {
		onFixedUpdateMethod();
	}
#endregion

#region API
    public void OnLevelStarted()
    {
		onFingerDown = FingerDown;
	}

	public void OnFingerDown()
	{
		onFingerDown();
	}

	public void OnFingerUp()
	{
		onFingerUp();
	}
	
	public void OnVehicleEject()
	{
		onFixedUpdateMethod = MoveOnAir;
		onFingerDown        = FingerDown_Air;
		onFingerUp          = ExtensionMethods.EmptyMethod;
	}

    public void OnVehicleChanged( IntGameEvent gameEvent )
    {
		vehicle_data = CurrentLevelData.Instance.levelData.vehicle_data_array[ gameEvent.eventValue ];
        //todo ? speed etc.
	}
#endregion

#region Implementation
	void FingerDown()
	{
		onFixedUpdateMethod = MoveOnPlatform;
		onFingerDown        = ExtensionMethods.EmptyMethod;
		onFingerUp          = FingerUp;
	}

	void FingerDown_Air()
	{

	}

	void FingerUp()
	{
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;
		onFingerDown        = FingerDown;
		onFingerUp          = ExtensionMethods.EmptyMethod;
	}

	void FingerUp_Air()
	{

	}

    void MoveOnPlatform()
    {
		var position = transform.position;

		transform.position = Vector3.Lerp( transform.position, vehicle_point_target, Time.fixedDeltaTime * GameSettings.Instance.vehicle_movement_look_speed );

		transform.LookAtOverTimeAxis( vehicle_point_target, GameSettings.Instance.vehicle_movement_look_axis, Time.fixedDeltaTime * GameSettings.Instance.vehicle_movement_look_speed );

		RaycastOntoPlatform();
	}

	void MoveOnAir()
	{
		var position = transform.position;
		transform.position = Vector3.Lerp( position, position + transform.forward, Time.fixedDeltaTime * GameSettings.Instance.vehicle_movement_speed );
	}

    void PlaceVehicleOnPlatform()
    {
		RaycastOntoPlatform();

		transform.position = vehicle_point_origin;
		transform.LookAtAxis( vehicle_point_target, GameSettings.Instance.vehicle_movement_look_axis );
	}

    void RaycastOntoPlatform()
    {
		//Info: We are presuming that vehicle always above the platform
		RaycastHit hitInfo_Origin;
		RaycastHit hitInfo_Target;

		var hitPosition_Origin = transform.position.SetY(
			GameSettings.Instance.vehicle_rayCast_height
        );

		var hitPosition_Target = hitPosition_Origin + GameSettings.Instance.game_forward * GameSettings.Instance.vehicle_movement_step;

		var hitOrigin = Physics.Raycast( hitPosition_Origin, GameSettings.Instance.vehicle_rayCast_direction, out hitInfo_Origin, GameSettings.Instance.vehicle_rayCast_distance, layerMask );

		Physics.Raycast( hitPosition_Target, GameSettings.Instance.vehicle_rayCast_direction, out hitInfo_Target, GameSettings.Instance.vehicle_rayCast_distance, layerMask );

		vehicle_point_origin = hitInfo_Origin.point;
		vehicle_point_target = hitInfo_Target.point;
	}
	
	void EmptyOutDelegates()
	{
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;
		onFingerDown        = ExtensionMethods.EmptyMethod;
		onFingerUp          = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
		Gizmos.DrawWireSphere( vehicle_point_origin, 0.25f );
		Gizmos.DrawWireSphere( vehicle_point_target, 0.25f );

		Handles.Label( vehicle_point_origin, "Origin" );
		Handles.Label( vehicle_point_target, "Target" );
	}
#endif
#endregion
}
