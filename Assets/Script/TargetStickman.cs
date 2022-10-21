using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class TargetStickman : MonoBehaviour
{
  [ Title( "Setup" ) ]
    [ SerializeField ] Pool_TargetStickman pool_stickman_target;

  [ Title( "Components" ) ]
    [ SerializeField ] ColorSetter _colorSetter;
    [ SerializeField ] Animator _animator;

	TargetVehiclePartData stickman_data;
	float stickman_count = 0;

	public void OnLevelUnloadStart()
    {
		pool_stickman_target.ReturnEntity( this );
	}

    public void SpawnIntoPart( Transform parent, TargetVehiclePartData data )
    {
		stickman_count = 0;
		stickman_data  = data;

		gameObject.SetActive( true );

		transform.parent = parent;
		transform.localPosition = data.position;
		transform.localEulerAngles = data.rotation;

		_colorSetter.SetColor( ReturnLerpedColor() );
		_animator.SetTrigger( data.pose );
	}

    public void IncreaseCount()
    {
		stickman_count = Mathf.Min( stickman_count + 1, stickman_data.count );
		_colorSetter.SetColor( ReturnLerpedColor() );
	}

    Color ReturnLerpedColor()
    {
		return Color.Lerp( stickman_data.color_start, stickman_data.color_end, stickman_count / stickman_data.count );
	}
}
