/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using UnityEditor;
using DG.Tweening;

public class VehicleMovement : MonoBehaviour
{
#region Fields
  [ Title( "Setup" ) ]
	[ SerializeField ] SharedReferenceNotifier notif_vehicle_reference;
	[ SerializeField ] SharedReferenceNotifier notif_vehicle_target_reference;
	[ SerializeField ] SharedReferenceNotifier notif_finishLine_reference;
	[ SerializeField ] SharedFloatNotifier notif_level_progress;
	[ SerializeField ] GameEvent event_vehicle_eject_perfect;

  [ Title( "Components" ) ]
	[ SerializeField ] ParticleSystem _particleSystem;

// Private
    VehicleData vehicle_data;
	UnityMessage onFingerDown;
	UnityMessage onFingerUp;
    UnityMessage onFixedUpdateMethod;
    UnityMessage onUpdateMethod;
    UnityMessage onVehicleCollide;
    UnityMessage onVehicleEject;
    IntUnityMessage onVehicleChange;

    [ ShowInInspector, ReadOnly ] Vector3 vehicle_point_origin;
    [ ShowInInspector, ReadOnly ] Vector3 vehicle_point_target;

    int layerMask;
	float vehicle_position_start;
	float vehicle_position_end;

	Vehicle vehicle;
	TargetVehicle vehicle_target;
	VehicleMovementData vehicle_movement;
	[ ShowInInspector, ReadOnly ] float vehicle_movement_speed;
	[ ShowInInspector, ReadOnly ] float vehicle_movement_rotate_speed;

	RecycledTween    recycledTween    = new RecycledTween();
	RecycledSequence recycledSequence = new RecycledSequence();

	public Vector3 SlopeDirection => ( vehicle_point_target - vehicle_point_origin ).normalized;
	public float VehicleMovementSpeed => vehicle_movement_speed;
	public float VehicleRotateSpeed   => vehicle_movement_rotate_speed;
#endregion

#region Properties
#endregion

#region Unity API
	private void Awake()
    {
		EmptyOutDelegates();

		layerMask = LayerMask.GetMask( ExtensionMethods.Layer_Platform );

		onVehicleChange = VehicleInited;

		PlaceVehicleOnPlatform();
	}

    private void Start()
    {
		vehicle_position_start = transform.position.z;
		vehicle_position_end   = ( notif_finishLine_reference.sharedValue as Transform ).position.z;
		vehicle                = notif_vehicle_reference.sharedValue as Vehicle;
	}

	private void Update()
	{
		onUpdateMethod();
	}

    private void FixedUpdate()
    {
		onFixedUpdateMethod();
	}
#endregion

#region API
    public void OnLevelStarted()
    {
		vehicle_movement_speed = vehicle_movement.movement_ground_speed_default;

		onUpdateMethod      = UpdateLevelProgress;
		onFixedUpdateMethod = MoveOnPlatform;
		onFingerDown        = FingerDown_Platform;
		onVehicleEject      = VehicleEject;

		FingerDown_Platform();
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
		onVehicleEject();
	}

    public void OnVehicleChanged( IntGameEvent gameEvent )
    {
		onVehicleChange( gameEvent.eventValue );
	}

	public void OnVehicleCollidePlatform()
	{
		onVehicleCollide();
	}
	
	public void OnFinishLine()
	{
		EmptyOutDelegates();

		_particleSystem.Stop( true, ParticleSystemStopBehavior.StopEmitting );
		notif_level_progress.SharedValue = 1f;

		recycledTween.Kill();

		onFixedUpdateMethod = MoveForwardOnAir;
		InitTargetVehicle();

		var sequence = recycledSequence.Recycle( OnVehicleAlignComplete );

		sequence.Append( transform.DORotate( Vector3.zero, GameSettings.Instance.vehicle_landing_adjust_duration ) );
		sequence.Append( vehicle_target.transform.DOLocalMove( GameSettings.Instance.vehicle_target_offset,
			GameSettings.Instance.vehicle_target_spawn_duration ) );
		sequence.SetUpdate( UpdateType.Fixed );
	}

	public void OnLevelFinished()
	{
		EmptyOutDelegates();
	}
#endregion

#region Implementation
	void VehicleEject()
	{
		if( vehicle_movement_speed < vehicle_movement.movement_ground_speed_eject_min ) return;

		FFLogger.PopUpText( transform.position, "Ejected" );
		onFixedUpdateMethod = MoveOnAir;
		onFingerDown        = FingerDown_Air;
		onFingerUp          = ExtensionMethods.EmptyMethod;
		onVehicleCollide    = VehicleCollidedPlatform;
		onVehicleEject      = ExtensionMethods.EmptyMethod;

		vehicle_movement_rotate_speed = vehicle_movement.movement_air_rotate_speed;

		if( vehicle_movement_speed >= vehicle_movement.movement_ground_speed_eject_perfect )
		{
			event_vehicle_eject_perfect.Raise();
			_particleSystem.Play();
		}
	}

	void VehicleInited( int index )
	{
		vehicle_data     = CurrentLevelData.Instance.levelData.vehicle_data_array[ index ].vehicle_data;
		vehicle_movement = vehicle_data.VehicleMovementData;

		onVehicleChange = VehicleChanged;
	}

	void VehicleChanged( int index )
	{
		var currentSpeedRatio = Mathf.InverseLerp( vehicle_movement.movement_ground_speed_default, vehicle_movement.movement_ground_speed_max, vehicle_movement_speed );

		vehicle_data     = CurrentLevelData.Instance.levelData.vehicle_data_array[ index ].vehicle_data;
		vehicle_movement = vehicle_data.VehicleMovementData;

		vehicle_movement_speed        = Mathf.Lerp( vehicle_movement.movement_ground_speed_default, vehicle_movement.movement_ground_speed_max, currentSpeedRatio );
		vehicle_movement_rotate_speed = vehicle_movement.movement_air_rotate_speed;
	}

	void OnVehicleAlignComplete()
	{
		vehicle.SendStickmenToTargetVehicle( vehicle_target );
	}

	void MoveForwardOnAir()
	{
		var position = transform.position;
		transform.position = Vector3.Lerp( position, position + transform.forward * GameSettings.Instance.vehicle_movement_step * GameSettings.Instance.vehicle_fly_cofactor, Time.fixedDeltaTime * vehicle_movement_speed );
	}

	void InitTargetVehicle() 
	{
		vehicle_target = ( notif_vehicle_target_reference.sharedValue as TargetVehicle );
		vehicle_target.transform.SetParent( transform );
		vehicle_target.transform.localPosition = GameSettings.Instance.vehicle_target_spawn_offset;
		vehicle_target.transform.localRotation = Quaternion.identity;
	}

	void UpdateLevelProgress()
	{
		notif_level_progress.SharedValue = Mathf.InverseLerp( vehicle_position_start, vehicle_position_end, transform.position.z );
	}

	void FingerDown_Platform()
	{
		FFLogger.PopUpText( transform.position, "Finger DOWN Platform" );
		onFingerDown = ExtensionMethods.EmptyMethod;
		onFingerUp   = FingerUp_Platform;

		recycledTween.Recycle( 
			DOTween.To( GetVehicleMovementSpeed, SetVehicleMovementSpeed,
			vehicle_movement.movement_ground_speed_max, vehicle_movement.movement_ground_speed_accelerate_duration )
			.SetEase( vehicle_movement.movement_ground_speed_accelerate_ease )
		);
	}

	void FingerDown_Air()
	{
		FFLogger.PopUpText( transform.position, "Finger DOWN Air" );
		onFingerDown = ExtensionMethods.EmptyMethod;
		onFingerUp   = FingerUp_Air;

		vehicle_movement_rotate_speed = vehicle_movement.movement_air_rotate_speed_max;
	}

	void FingerUp_Platform()
	{
		FFLogger.PopUpText( transform.position, "Finger UP Platform" );
		onFingerDown = FingerDown_Platform;
		onFingerUp   = ExtensionMethods.EmptyMethod;

		recycledTween.Kill();
	}

	void FingerUp_Air()
	{
		FFLogger.PopUpText( transform.position, "Finger UP Air" );
		onFingerDown = FingerDown_Air;
		onFingerUp   = ExtensionMethods.EmptyMethod;

		vehicle_movement_rotate_speed = vehicle_movement.movement_air_rotate_speed;
	}

    void MoveOnPlatform()
    {
		var position = transform.position;
		position = Vector3.Lerp( transform.position, vehicle_point_target, Time.fixedDeltaTime * vehicle_movement_speed );

		transform.LookAtOverTimeAxis( vehicle_point_target, GameSettings.Instance.vehicle_movement_look_axis, Time.fixedDeltaTime * GameSettings.Instance.vehicle_movement_look_speed );
		// transform.LookAtAxis( vehicle_point_target, GameSettings.Instance.vehicle_movement_look_axis );
		transform.position = position;

		RaycastOntoPlatform( position );
	}

	void MoveOnAir()
	{
		//Move the vehicle
		var position = transform.position;
		transform.position = Vector3.Lerp( position, position + transform.forward * GameSettings.Instance.vehicle_movement_step * GameSettings.Instance.vehicle_fly_cofactor, Time.fixedDeltaTime * vehicle_movement_speed );
     
		transform.Rotate( GameSettings.Instance.vehicle_fly_rotate_axis * vehicle_movement_rotate_speed * Time.fixedDeltaTime, Space.World );
		//Rotate the vehicle
		var eulerAngle = transform.eulerAngles.x;

		if( eulerAngle < 270 )
			eulerAngle = Mathf.Min( eulerAngle, GameSettings.Instance.vehicle_fly_rotate_clamp );

		transform.eulerAngles = transform.eulerAngles.SetX( eulerAngle );
	}

    void PlaceVehicleOnPlatform()
    {
		RaycastOntoPlatform( transform.position );

		transform.position = vehicle_point_origin;
		transform.LookAtAxis( vehicle_point_target, GameSettings.Instance.vehicle_movement_look_axis );
	}

	void VehicleCollidedPlatform()
	{
		EmptyOutDelegates();
		onVehicleEject = VehicleEject;

		RaycastOntoPlatform( transform.TransformPoint( vehicle_data.VehicleLandingRaycastPosition ) );

		var vehicleDirection = transform.forward;

		var angle = Mathf.Acos( Vector3.Dot( vehicleDirection, SlopeDirection ) ) * Mathf.Rad2Deg;
		FFLogger.PopUpText( transform.position, "Slope Enter Angle: " + angle );

		_particleSystem.Stop( true, ParticleSystemStopBehavior.StopEmitting );

		if( angle >= GameSettings.Instance.vehicle_landing_angle )
			HandleLanding_Bad();
		else
			HandleLanding_Good();
	}

	void HandleLanding_Good()
	{
		onFixedUpdateMethod = MoveOnPlatform;
		onFingerDown        = FingerDown_Platform;
	}

	void HandleLanding_Bad()
	{
		recycledTween.Kill();
		vehicle.OnLooseStickman( 50 ); // percentage

		var vehicleRotation = Quaternion.LookRotation( SlopeDirection, Vector3.up ).eulerAngles;

		var sequence = recycledSequence.Recycle( OnVehicleAdjustComplete );
		sequence.Append( transform.DOJump( vehicle_point_origin, vehicle_movement.landing_adjust_jump_power,
			1, GameSettings.Instance.vehicle_landing_adjust_duration )
			.SetEase( vehicle_movement.landing_adjust_jump_ease ) );
		sequence.Join( transform.DORotate( vehicleRotation, GameSettings.Instance.vehicle_landing_adjust_duration )
			.SetEase( vehicle_movement.landing_adjust_rotation_ease ) );
	}

	void OnVehicleAdjustComplete()
	{
		vehicle_movement_speed = vehicle_movement.movement_ground_speed_default;
		HandleLanding_Good();
	}

    void RaycastOntoPlatform( Vector3 position )
    {
		//Info: We are presuming that vehicle always above the platform
		RaycastHit hitInfo_Origin;
		RaycastHit hitInfo_Target;

		var hitPosition_Origin = position.SetY(
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
		onUpdateMethod      = ExtensionMethods.EmptyMethod;
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;
		onFingerDown        = ExtensionMethods.EmptyMethod;
		onFingerUp          = ExtensionMethods.EmptyMethod;
		onVehicleCollide    = ExtensionMethods.EmptyMethod;
		onVehicleEject      = ExtensionMethods.EmptyMethod;
	}
#endregion

#region GetterAndSetter
	float GetVehicleMovementSpeed()
	{
		return vehicle_movement_speed;
	}

	void SetVehicleMovementSpeed( float value )
	{
		vehicle_movement_speed = value;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
		Gizmos.DrawWireSphere( vehicle_point_origin, 0.1f );
		Gizmos.DrawWireSphere( vehicle_point_target, 0.1f );

		if( Application.isPlaying )
		{
			Gizmos.DrawWireSphere( transform.TransformPoint( vehicle_data.VehicleLandingRaycastPosition ), 0.1f );
			Handles.Label( transform.TransformPoint( vehicle_data.VehicleLandingRaycastPosition ), "Raycast" );
		}

		Handles.Label( vehicle_point_origin, "Origin" );
		Handles.Label( vehicle_point_target, "Target" );
	}

	[ Button() ]
	void PlaceVehicleOnPlatform_Editor()
	{
		//Info: We are presuming that vehicle always above the platform
		RaycastHit hitInfo_Origin;
		RaycastHit hitInfo_Target;

		var mask = LayerMask.GetMask( ExtensionMethods.Layer_Platform );

		var hitPosition_Origin = transform.position.SetY(
			GameSettings.Instance.vehicle_rayCast_height
		);

		var hitPosition_Target = hitPosition_Origin + GameSettings.Instance.game_forward * GameSettings.Instance.vehicle_movement_step;

		var hitOrigin = Physics.Raycast( hitPosition_Origin, GameSettings.Instance.vehicle_rayCast_direction, out hitInfo_Origin, GameSettings.Instance.vehicle_rayCast_distance, mask );

		Physics.Raycast( hitPosition_Target, GameSettings.Instance.vehicle_rayCast_direction, out hitInfo_Target, GameSettings.Instance.vehicle_rayCast_distance, mask );

		transform.position = hitInfo_Origin.point;
		transform.LookAtAxis( hitInfo_Target.point, GameSettings.Instance.vehicle_movement_look_axis );
	}
#endif
#endregion
}