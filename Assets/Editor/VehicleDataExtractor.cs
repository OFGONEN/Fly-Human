/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;

[ CreateAssetMenu( fileName = "tool_vehicle_data_extractor", menuName = "FFEditor/Tool/Vehicle Data Extractor" ) ]
public class VehicleDataExtractor : ScriptableObject
{
#region Fields
    [ SerializeField ] VehicleData vehicle_data_target;
    [ SerializeField ] StickmanPoseData[] stickman_pose_array;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
    [ Button() ]
    public void ExtractData()
    {
		var planeGameObject = Selection.activeGameObject.transform;

		var vehicle_data_array = new VehiclePartData[ planeGameObject.childCount ];

        for( var i = 0; i < vehicle_data_array.Length; i++ )
        {
			var child = planeGameObject.GetChild( i );
			var prefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot( child.gameObject );
			var prefabName = Path.GetFileNameWithoutExtension( prefab );
			string pose;

            if( !ReturnStickmanPose( prefabName, out pose ) )
            {
                FFLogger.LogError( "Stickman Pose didn't found: " + child.name, child );
				return;
			}

			vehicle_data_array[ i ] = new VehiclePartData( child, pose );
		}

		vehicle_data_target.SetVehicleDataArray( vehicle_data_array );
		vehicle_data_target.SetVehicleColliderData( planeGameObject.GetComponent< SphereCollider >() );
	}

    bool ReturnStickmanPose( string name, out string pose )
    {
		for( var i = 0; i < stickman_pose_array.Length; i++ )
		{
            if( name.Equals( stickman_pose_array[ i ].stickman_name ) )
            {
				pose = stickman_pose_array[ i ].stickman_pose;
				return true;
			}
		}

		pose = string.Empty;
		return false;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}

[System.Serializable]
public struct StickmanPoseData
{
	public string stickman_name;
	public string stickman_pose;
}
