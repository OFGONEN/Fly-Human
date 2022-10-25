/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
    public class CurrentLevelData : ScriptableObject
    {
#region Fields
		public int currentLevel_Real;
		public int currentLevel_Shown;
		public LevelData levelData;

        static CurrentLevelData instance;

        delegate CurrentLevelData ReturnCurrentLevel();
        static ReturnCurrentLevel returnInstance = LoadInstance;

        public static CurrentLevelData Instance => returnInstance();
#endregion

#region API
        public void IncreaseLevelCount()
        {
            if( levelData.vehicle_level_last && !levelData.vehicle_target.VehicleIsUnlocked )
            {
				CurrentLevelData.Instance.currentLevel_Real = levelData.vehicle_level_replay.ReturnRandom();
				CurrentLevelData.Instance.currentLevel_Shown++;
            }
            else if( !levelData.vehicle_level_last && levelData.vehicle_target.VehicleIsUnlocked )
            {
				CurrentLevelData.Instance.currentLevel_Real = levelData.vehicle_level_next;
				CurrentLevelData.Instance.currentLevel_Shown++;
            }
            else 
            {
			    CurrentLevelData.Instance.currentLevel_Real++;
			    CurrentLevelData.Instance.currentLevel_Shown++;
            }

			if( currentLevel_Real > GameSettings.Instance.maxLevelCount )
            {
				currentLevel_Real = Random.Range( 1, GameSettings.Instance.maxLevelCount );
				PlayerPrefs.DeleteAll();
			}

        }
		public void LoadCurrentLevelData()
		{
#if UNITY_EDITOR
			if( currentLevel_Real > GameSettings.Instance.maxLevelCount )
				currentLevel_Real = Random.Range( 1, GameSettings.Instance.maxLevelCount );
#endif
			levelData = Resources.Load< LevelData >( "level_data_" + currentLevel_Real );
		}
#endregion

#region Implementation
        static CurrentLevelData LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< CurrentLevelData >( "level_current" );

			returnInstance = ReturnInstance;

            return instance;
        }

        static CurrentLevelData ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}