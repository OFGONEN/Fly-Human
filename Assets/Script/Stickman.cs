using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Stickman : MonoBehaviour
{
  [ Title( "Setup" ) ]
	[ SerializeField ] Pool_Stickman pool_stickman;
	[ SerializeField ] GameEvent stickman_movement_start;
	[ SerializeField ] GameEvent stickman_movement_end;

  [ Title( "Components" ) ]
    [ SerializeField ] Animator _animator;
    [ SerializeField ] ColorSetter _colorSetter;
    [ SerializeField ] ToggleRagdoll _toggleRagdoll;
    [ SerializeField ] Collider _collider;

// Private 
	bool is_pooled;

	TargetVehicle target_vehicle;
	TargetStickman target_stickman;

	RecycledSequence recycledSequence = new RecycledSequence();
	RecycledTween    recycledTween    = new RecycledTween();

    void OnDisable()
    {
		recycledSequence.Kill();
	}

    void Awake()
    {
		_toggleRagdoll.SwitchRagdoll( false );
		_toggleRagdoll.ToggleCollider( true );
    }

    public void AttachToVehicle( Transform parent, VehiclePartData vehiclePartData )
    {
		transform.parent = parent;

		_animator.SetTrigger( vehiclePartData.pose );
		var colorTween = _colorSetter.LerpColor( vehiclePartData.color, GameSettings.Instance.stickman_pose_duration );

		var sequence = recycledSequence.Recycle();
		sequence.Append( transform.DOLocalJump( vehiclePartData.position,
			GameSettings.Instance.stickman_jump_power,
			1,
			GameSettings.Instance.stickman_pose_duration ) );
		sequence.Join( transform.DOLocalRotate( vehiclePartData.rotation, GameSettings.Instance.stickman_pose_duration ) );
		sequence.Join( colorTween );
	}

    public void ChangeToAnoterPart( VehiclePartData vehiclePartData )
    {
		_animator.SetTrigger( vehiclePartData.pose );
		var colorTween = _colorSetter.LerpColor( vehiclePartData.color, GameSettings.Instance.stickman_pose_duration );

		var sequence = recycledSequence.Recycle();
		sequence.Append( transform.DOLocalMove( vehiclePartData.position, GameSettings.Instance.stickman_pose_duration ) );
		sequence.Join( transform.DOLocalRotate( vehiclePartData.rotation, GameSettings.Instance.stickman_pose_duration ) );
		sequence.Join( colorTween );
	}

    public void FallFromVehicle()
    {
		recycledSequence.Kill();
		recycledTween.Kill();

		DetachFromVehicle();
		DelayedDisable();
	}

    public void MoveTowardsTargetVehicle( TargetVehicle targetVehicle, TargetStickman stickman )
	{
		target_vehicle  = targetVehicle;
		target_stickman = stickman;

		DetachFromVehicle();
		_toggleRagdoll.BecomeMovableRagdollWithoutGravity();

		stickman_movement_start.Raise();

		recycledTween.Recycle( _toggleRagdoll.MainRigidbody.DOMove( stickman.transform.position,
			GameSettings.Instance.stickman_targetMove_duration.ReturnRandom() ),OnMoveTowardsTargetComplete );
	}

	public void MoveTowardsPosition ( Vector3 position )
	{
		DetachFromVehicle();
		_toggleRagdoll.BecomeMovableRagdollWithoutGravity();

		stickman_movement_start.Raise();
		recycledTween.Recycle( _toggleRagdoll.MainRigidbody.DOMove( position,
			GameSettings.Instance.stickman_targetMove_duration.ReturnRandom() ), OnMovePositionComplete );
	}

	public void SpawnIntoVehicle( Transform vehicle, VehiclePartData data )
	{
		gameObject.SetActive( true );

		is_pooled = true;

		_toggleRagdoll.SwitchRagdoll( false );
		_toggleRagdoll.ToggleCollider( true );

		transform.parent           = vehicle;
		transform.localPosition    = data.position;
		transform.localEulerAngles = data.rotation;

		_collider.enabled = false;
		_colorSetter.SetColor( data.color );
		_animator.SetTrigger( data.pose );
	}

	public void TurnIntoCurrency()
	{
		//todo gain currency
		//todo spawn particle effect - coin splash particle
		OnDelayedDisableComplete();
	}

	void OnMoveTowardsTargetComplete()
	{
		target_stickman.IncreaseCount();
		target_vehicle.IncreaseCount();

		stickman_movement_end.Raise();

		gameObject.SetActive( false );
	}

	void OnMovePositionComplete()
	{
		stickman_movement_end.Raise();
		gameObject.SetActive( false );
	}

	void DetachFromVehicle()
    {
		transform.parent = null;

		_animator.enabled = false;

		_toggleRagdoll.SwitchRagdoll( true );
		_toggleRagdoll.ToggleCollider( true );
	}

	void DelayedDisable()
	{
		recycledTween.Recycle( DOVirtual.DelayedCall( GameSettings.Instance.stickman_disableDuration,
			OnDelayedDisableComplete
        ) );
	}

	void OnDelayedDisableComplete()
	{
		if( is_pooled )
		{
			is_pooled = false;
			pool_stickman.ReturnEntity( this );
		}
		else
			gameObject.SetActive( false );
	}
}