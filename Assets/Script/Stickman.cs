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
	[ SerializeField ] GameEvent stickman_collected;
	[ SerializeField ] UIParticlePool pool_ui_particle;

  [ Title( "Components" ) ]
    [ SerializeField ] Animator _animator;
    [ SerializeField ] ColorSetter _colorSetter;
    [ SerializeField ] ToggleRagdoll _toggleRagdoll;
    [ SerializeField ] Collider _collider;
    [ SerializeField ] TweenChain _tweenChain;
    [ SerializeField ] ParticleSpawner _particleSpawner;

// Private 
	bool is_pooled;

	VehiclePartData vehicle_part_data;
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

		vehicle_part_data = vehiclePartData;

		var colorTween = _colorSetter.LerpColor( vehiclePartData.color, GameSettings.Instance.stickman_pose_duration );

		var sequence = recycledSequence.Recycle( OnStickmanPoseComplete );
		sequence.Append( transform.DOLocalJump( vehiclePartData.position,
			GameSettings.Instance.stickman_jump_power,
			1,
			GameSettings.Instance.stickman_pose_duration ) );
		sequence.Join( transform.DOLocalRotate( vehiclePartData.rotation, GameSettings.Instance.stickman_pose_duration ) );
		sequence.Join( colorTween );
	}

    public void ChangeToAnoterPart( VehiclePartData vehiclePartData )
    {
		vehicle_part_data = vehiclePartData;

		_animator.SetTrigger( vehiclePartData.pose );

		var colorTween = _colorSetter.LerpColor( vehiclePartData.color, GameSettings.Instance.stickman_pose_duration );

		var sequence = recycledSequence.Recycle( OnStickmanPoseComplete );
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

    public void MoveTowardsTargetVehicle( int index, TargetVehicle targetVehicle, TargetStickman stickman )
	{
		target_vehicle  = targetVehicle;
		target_stickman = stickman;

		DetachFromVehicle();
		_toggleRagdoll.BecomeMovableRagdollWithoutGravity();

		stickman_movement_start.Raise();

		recycledTween.Recycle( _toggleRagdoll.MainRigidbody.DOMove( stickman.transform.position,
			GameSettings.Instance.stickman_targetMove_duration.ReturnRandom() ),OnMoveTowardsTargetComplete );
	}

	public void MoveTowardsPosition( int index, Vector3 position )
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

		vehicle_part_data = data;

		is_pooled = true;

		_toggleRagdoll.SwitchRagdoll( false );
		_toggleRagdoll.ToggleCollider( true );
		_toggleRagdoll.SetTransformDatas();

		transform.parent           = vehicle;
		transform.localPosition    = data.position;
		transform.localEulerAngles = data.rotation;

		_collider.enabled = false;
		_colorSetter.SetColor( data.color );

		_animator.enabled = true;
		_animator.SetTrigger( data.pose );

		OnStickmanPoseComplete();
	}

	public void TurnIntoCurrency()
	{
		_particleSpawner.Spawn( 0 );
		stickman_collected.Raise();

		pool_ui_particle.Spawn( CurrentLevelData.Instance.levelData.stickman_currency, transform.position + Vector3.up * 1.4f );

		OnDelayedDisableComplete();
	}

	void OnStickmanPoseComplete()
	{
		if( vehicle_part_data.tweener )
		{
			_tweenChain.AddTweenData( vehicle_part_data.tween_data );
			_tweenChain.Play( 0 );
		}
	}

	void OnMoveTowardsTargetComplete()
	{
		target_stickman.IncreaseCount();
		target_vehicle.IncreaseCount();

		stickman_movement_end.Raise();
		OnDelayedDisableComplete();
	}

	void OnMovePositionComplete()
	{
		stickman_movement_end.Raise();
		OnDelayedDisableComplete();
	}

	void DetachFromVehicle()
    {
		transform.parent = null;

		_animator.enabled = false;

		_toggleRagdoll.SwitchRagdoll( true );
		_toggleRagdoll.ToggleCollider( true );
		_tweenChain.ClearAndKill();
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