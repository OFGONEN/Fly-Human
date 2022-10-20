using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Stickman : MonoBehaviour
{

  [ Title( "Components" ) ]
    [ SerializeField ] Animator _animator;
    [ SerializeField ] ColorSetter _colorSetter;

	// Private 
	RecycledSequence recycledSequence = new RecycledSequence();

    void OnDisable()
    {
		recycledSequence.Kill();
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
			GameSettings.Instance.ui_PopUp_duration ) );
		sequence.Join( transform.DOLocalRotate( vehiclePartData.rotation, GameSettings.Instance.stickman_pose_duration ) );
		sequence.Join( colorTween );
	}

    public void ChangeToAnoterPart( VehiclePartData vehiclePartData )
    {
		_animator.SetTrigger( vehiclePartData.pose );
		var colorTween = _colorSetter.LerpColor( vehiclePartData.color, GameSettings.Instance.stickman_pose_duration );

		var sequence = recycledSequence.Recycle();
		sequence.Append( transform.DOLocalMove( vehiclePartData.position, GameSettings.Instance.ui_PopUp_duration ) );
		sequence.Join( transform.DOLocalRotate( vehiclePartData.rotation, GameSettings.Instance.stickman_pose_duration ) );
		sequence.Join( colorTween );
	}
}