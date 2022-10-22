using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Vehicle : MonoBehaviour
{
  [ Title( "Setup" ) ]
    [ SerializeField ] Pool_Stickman pool_stickman;

// Private
    List< Stickman > vehicle_stickman = new List< Stickman >( 64 );
	VehicleData vehicle_data;

	void Awake()
    {
		var levelData = CurrentLevelData.Instance.levelData;
		vehicle_data = levelData.vehicle_data_array[ levelData.vehicle_start_index ];

		SpawnStickman( levelData.vehicle_start_count );
	}

    void SpawnStickman( int count )
    {
        for( var i = 0; i < count; i++ )
        {
		    var stickman = pool_stickman.GetEntity();
			vehicle_stickman.Add( stickman );

			stickman.SpawnIntoVehicle( transform, vehicle_data.VehiclePartAtIndex( i ) );
		}
	}
}
