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

// Private
    VehicleData vehicle_data;
	UnityMessage onFingerDown;
	UnityMessage onFingerUp;
    UnityMessage onFixedUpdateMethod;
    UnityMessage onVehicleCollide;

    [ ShowInInspector, ReadOnly ] Vector3 vehicle_point_origin;
    [ ShowInInspector, ReadOnly ] Vector3 vehicle_point_target;

    int layerMask;

	Vehicle vehicle;
	[ ShowInInspector, ReadOnly ] float vehicle_movement_speed;
	[ ShowInInspector, ReadOnly ] float vehicle_movement_rotate_speed;

	RecycledTween    recycledTween    = new RecycledTween();
	RecycledSequence recycledSequence = new RecycledSequence();

	public Vector3 SlopeDirection => ( vehicle_point_target - vehicle_point_origin ).normalized;
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
		vehicle = notif_vehicle_reference.sharedValue as Vehicle;

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
		FFLogger.Log( "Vehicle Level Started" );
		vehicle_movement_speed = GameSettings.Instance.vehicle_movement_speed;

		onFixedUpdateMethod = MoveOnPlatform;
		onFingerDown        = FingerDown_Platform;

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
		FFLogger.PopUpText( transform.position, "Ejected" );
		onFixedUpdateMethod = MoveOnAir;
		onFingerDown        = FingerDown_Air;
		onFingerUp          = ExtensionMethods.EmptyMethod;
		onVehicleCollide    = VehicleCollidedPlatform;

		vehicle_movement_rotate_speed = GameSettings.Instance.vehicle_fly_rotate_speed;
	}

    public void OnVehicleChanged( IntGameEvent gameEvent )
    {
		vehicle_data = CurrentLevelData.Instance.levelData.vehicle_data_array[ gameEvent.eventValue ];
        //todo ? speed etc.
	}

	public void OnVehicleCollidePlatform()
	{
		onVehicleCollide();
	}
#endregion

#region Implementation
	void FingerDown_Platform()
	{
		FFLogger.PopUpText( transform.position, "Finger DOWN Platform" );
		onFingerDown = ExtensionMethods.EmptyMethod;
		onFingerUp   = FingerUp_Platform;

		recycledTween.Recycle( 
			DOTween.To( GetVehicleMovementSpeed, SetVehicleMovementSpeed,
			GameSettings.Instance.vehicle_movement_speed_max, GameSettings.Instance.vehicle_movement_speed_duration )
			.SetEase( GameSettings.Instance.vehicle_movement_speed_ease )
		);
	}

	void FingerDown_Air()
	{
		FFLogger.PopUpText( transform.position, "Finger DOWN Air" );
		onFingerDown = ExtensionMethods.EmptyMethod;
		onFingerUp   = FingerUp_Air;

		vehicle_movement_rotate_speed = GameSettings.Instance.vehicle_fly_rotate_speed_max;
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

		vehicle_movement_rotate_speed = GameSettings.Instance.vehicle_fly_rotate_speed;
	}

    void MoveOnPlatform()
    {
		var position = transform.position;

		transform.LookAtOverTimeAxis( vehicle_point_target, GameSettings.Instance.vehicle_movement_look_axis, Time.fixedDeltaTime * GameSettings.Instance.vehicle_movement_look_speed );
		transform.position = Vector3.Lerp( transform.position, vehicle_point_target, Time.fixedDeltaTime * vehicle_movement_speed );

		RaycastOntoPlatform();
	}

	void MoveOnAir()
	{
		//Move the vehicle
		var position = transform.position;
		transform.position = Vector3.Lerp( position, position + transform.forward * GameSettings.Instance.vehicle_movement_step, Time.fixedDeltaTime * vehicle_movement_speed );

		transform.Rotate( GameSettings.Instance.vehicle_fly_rotate_axis * vehicle_movement_rotate_speed * Time.fixedDeltaTime, Space.World );
		//Rotate the vehicle
		var eulerAngle = transform.eulerAngles.x;

		if( eulerAngle < 270 )
			eulerAngle = Mathf.Min( eulerAngle, GameSettings.Instance.vehicle_fly_rotate_clamp );

		transform.eulerAngles = transform.eulerAngles.SetX( eulerAngle );
	}

    void PlaceVehicleOnPlatform()
    {
		RaycastOntoPlatform();

		transform.position = vehicle_point_origin;
		transform.LookAtAxis( vehicle_point_target, GameSettings.Instance.vehicle_movement_look_axis );
	}

	void VehicleCollidedPlatform()
	{
		EmptyOutDelegates();
		RaycastOntoPlatform();

		var vehicleDirection = transform.forward;

		var angle = Mathf.Acos( Vector3.Dot( vehicleDirection, SlopeDirection ) ) * Mathf.Rad2Deg;
		FFLogger.PopUpText( transform.position, "Slope Enter Angle: " + angle );

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

		sequence.Append( transform.DOJump( vehicle_point_origin, GameSettings.Instance.vehicle_landing_adjust_jump_power,
			1, GameSettings.Instance.vehicle_landing_adjust_duration )
			.SetEase( GameSettings.Instance.vehicle_landing_adjust_jump_ease ) );
		sequence.Join( transform.DORotate( vehicleRotation, GameSettings.Instance.vehicle_landing_adjust_duration )
			.SetEase( GameSettings.Instance.vehicle_landing_adjust_rotation_ease ) );
	}

	void OnVehicleAdjustComplete()
	{
		//todo Is this Enough ?
		HandleLanding_Good();
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
		onVehicleCollide    = ExtensionMethods.EmptyMethod;
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
		Gizmos.DrawWireSphere( vehicle_point_origin, 0.25f );
		Gizmos.DrawWireSphere( vehicle_point_target, 0.25f );

		Handles.Label( vehicle_point_origin, "Origin" );
		Handles.Label( vehicle_point_target, "Target" );
	}
#endif
#endregion
}