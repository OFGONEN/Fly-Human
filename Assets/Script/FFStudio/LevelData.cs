/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "LevelData", menuName = "FF/Data/LevelData" ) ]
	public class LevelData : ScriptableObject
    {
	[ Title( "Scene Setup" ) ]
		[ ValueDropdown( "SceneList" ), LabelText( "Scene Index" ) ] public int scene_index;
        [ LabelText( "Override As Active Scene" ) ] public bool scene_overrideAsActiveScene;

	[ Title( "Data Setup" ) ]
		[ LabelText( "Vehicle Evolve List" ) ] public List< VehicleData > vehicle_data_array; 
		[ LabelText( "Target Vehicle Data" ) ] public TargetVehicleData vehicle_target;
		[ LabelText( "Vehicle's Last Level" ) ] public bool vehicle_level_last;
		[ LabelText( "Replay Scene Indexes " ), ShowIf( "vehicle_level_last" ) ] public int[] vehicle_level_replay;
		[ LabelText( "Next Level of Vehicle" ), HideIf( "vehicle_level_last" ) ] public int vehicle_level_next;
		[ LabelText( "Start Vehicle Index" ) ] public int vehicle_start_index;
		[ LabelText( "Start Vehicle Part Count" ) ] public int vehicle_start_count;

#if UNITY_EDITOR
		static IEnumerable SceneList()
        {
			var list = new ValueDropdownList< int >();

			var scene_count = SceneManager.sceneCountInBuildSettings;

			for( var i = 0; i < scene_count; i++ )
				list.Add( Path.GetFileNameWithoutExtension( SceneUtility.GetScenePathByBuildIndex( i ) ) + $" ({i})", i );

			return list;
		}

		private void OnValidate()
		{
			if( vehicle_level_next > GameSettings.Instance.maxLevelCount )
			{
				UnityEditor.EditorUtility.SetDirty( this );

				FFLogger.LogError( "Next Vehicle level cannot be bigger than MaxLevelCount" );
				FFLogger.LogError( "Create Level Data assets and go into Play mode to update MaxLevelCount value of GameSettings" );
				vehicle_level_next = GameSettings.Instance.maxLevelCount;
			}
		}
#endif
    }
}
