/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class CameraFollow : MonoBehaviour
    {
#region Fields
    [ Title( "Event Listeners" ) ]
        [ SerializeField ] EventListenerDelegateResponse levelRevealEventListener;
        [ SerializeField ] MultipleEventListenerDelegateResponse levelEndEventListener;
        
    [ Title( "Setup" ) ]
        [ SerializeField ] SharedReferenceNotifier notifier_reference_transform_target;

        Transform transform_target;

        UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
        void OnEnable()
        {
            levelRevealEventListener.OnEnable();
            levelEndEventListener.OnEnable();
        }

        void OnDisable()
        {
			updateMethod = ExtensionMethods.EmptyMethod;

			levelRevealEventListener.OnDisable();
            levelEndEventListener.OnDisable();
        }

        void Awake()
        {
            levelRevealEventListener.response = LevelRevealedResponse;
            levelEndEventListener.response    = LevelCompleteResponse;

            updateMethod = ExtensionMethods.EmptyMethod;
        }

        private void Start()
        {
            transform_target      = ( notifier_reference_transform_target.SharedValue as Vehicle ).transform;
            transform.position    = transform_target.position + GameSettings.Instance.camera_follow_offset_position;
            transform.eulerAngles = GameSettings.Instance.camera_follow_offset_rotation;
        }

        void FixedUpdate()
        {
            updateMethod();
        }
#endregion

#region API
#endregion

#region Implementation
        void LevelRevealedResponse()
        {
            updateMethod = FollowTarget;
        }

        void LevelCompleteResponse()
        {
            updateMethod = ExtensionMethods.EmptyMethod;
        }

        void FollowTarget()
        {
            // Info: Simple follow logic.
			transform.position = Vector3.Lerp( transform.position, transform_target.position + GameSettings.Instance.camera_follow_offset_position, Time.fixedDeltaTime * GameSettings.Instance.camera_follow_speed );
#if UNITY_EDITOR
			transform.eulerAngles = GameSettings.Instance.camera_follow_offset_rotation;
#endif
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
        [ Button() ]
        void ResetOffset()
        {
			var targetPosition =  GameObject.FindWithTag( "CameraFocus" ).transform.position;

			transform.position    = targetPosition + GameSettings.Instance.camera_follow_offset_position;
			transform.eulerAngles = GameSettings.Instance.camera_follow_offset_rotation;
		}
#endif
#endregion
    }
}