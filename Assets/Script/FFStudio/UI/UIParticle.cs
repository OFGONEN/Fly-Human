/* Created by and for usage of FF Studios (2021). */
/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

public class UIParticle : MonoBehaviour
{
#region Fields
[ Title( "SharedVariable" ) ]
    [ SerializeField ] SharedReferenceNotifier notif_camera_transform;
    [ SerializeField ] SharedReferenceNotifier notif_target_transform;
    [ SerializeField ] SharedReferenceNotifier notif_target_PunchScale;
    [ SerializeField ] UIParticlePool pool_ui_particle;

    RecycledSequence recycledSequence = new RecycledSequence();
	float font_size_original;
	OnCompleteMessage oComplete_additional;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
	[ Button() ]
    public void Spawn( Vector3 worldPosition, OnCompleteMessage onComplete = null )
    {
		gameObject.SetActive( true );

		oComplete_additional = onComplete;

		var screenPosition      = ( notif_camera_transform.SharedValue as Transform ).GetComponent< Camera >().WorldToScreenPoint( worldPosition );
        var spawnTargetPosition = screenPosition + Random.insideUnitCircle.ConvertV3() * GameSettings.Instance.uiParticle_spawn_width_percentage * Screen.width / 100f;
        var targetPosition      = ( notif_target_transform.SharedValue as RectTransform ).position;

		transform.position = screenPosition;

		var sequence = recycledSequence.Recycle( OnSequenceComplete )
							.Append( transform
										.DOMove( spawnTargetPosition, GameSettings.Instance.uiParticle_spawn_duration )
										.SetEase( GameSettings.Instance.uiParticle_spawn_ease ) )
							.AppendInterval( GameSettings.Instance.uiParticle_target_waitDuration )
							.Append( transform
										.DOMove( targetPosition, GameSettings.Instance.uiParticle_target_duration )
										.SetEase( GameSettings.Instance.uiParticle_target_ease ) );
	}
#endregion

#region Implementation
	void OnSequenceComplete()
	{
		pool_ui_particle.ReturnEntity( this );
		( notif_target_PunchScale.SharedValue as UI_PunchScale_Int ).PunchScale();
		oComplete_additional?.Invoke();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}