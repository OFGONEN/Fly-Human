/* Created by and for usage of FF Studios (2021). */

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
#region Fields
      [ Title( "Fired Events" ) ]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;
        public GameEvent event_stickman_movement_done;

      [ Title( "Level Releated" ) ]
        public SharedProgressNotifier notifier_progress;

// Private
        int stickman_movement_count;
#endregion

#region UnityAPI
#endregion

#region API
        // Info: Called from Editor.
        public void LevelLoadedResponse()
        {
			notifier_progress.SetNumerator( 0 );
			notifier_progress.SetDenominator( 1 );

			var levelData = CurrentLevelData.Instance.levelData;
            // Set Active Scene.
			if( levelData.scene_overrideAsActiveScene )
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 1 ) );
            else
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 0 ) );
		}

        // Info: Called from Editor.
        public void LevelRevealedResponse()
        {

        }

        // Info: Called from Editor.
        public void LevelStartedResponse()
        {

        }

        public void OnStickmanMovementStart()
        {
			stickman_movement_count++;
		}

        public void OnStickmanMovementEnd()
        {
			stickman_movement_count--;

			if( stickman_movement_count == 0)
            {
				event_stickman_movement_done.Raise();
			}
#if UNITY_EDITOR
            else if( stickman_movement_count < 0  )
            {
                FFLogger.LogError( "This value cannot be below zero" );
            }
#endif
		}
#endregion

#region Implementation
#endregion
    }
}