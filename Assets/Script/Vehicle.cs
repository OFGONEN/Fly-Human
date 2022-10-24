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
	int vehicle_index;

	void Awake()
    {
		var levelData = CurrentLevelData.Instance.levelData;
		vehicle_index = levelData.vehicle_start_index;
		vehicle_data  = levelData.vehicle_data_array[ vehicle_index ];

		SpawnStickman( levelData.vehicle_start_count );
	}

	public void OnStickmanTrigger( StickmanGameEvent gameEvent )
	{
		var stickman = gameEvent.event_stickman as Stickman;

		var stickman_count = vehicle_stickman.Count;

		if( stickman_count + 1 > vehicle_data.VehicleCountMax )
		{
			if( vehicle_index < CurrentLevelData.Instance.levelData.vehicle_data_array.Length - 1 )
				Evolve( stickman );
			else
				stickman.TurnIntoCurrency();
		}
		else
		{
			stickman.AttachToVehicle( transform, vehicle_data.VehiclePartAtIndex( stickman_count ) );
			vehicle_stickman.Add( stickman );
		}
	}

	void Evolve( Stickman incomingStickman )
	{
		vehicle_index++;
		vehicle_data = CurrentLevelData.Instance.levelData.vehicle_data_array[ vehicle_index ];

		for( var i = 0; i < vehicle_stickman.Count; i++ )
			vehicle_stickman[ i ].ChangeToAnoterPart( vehicle_data.VehiclePartAtIndex( i ) );

		incomingStickman.AttachToVehicle( transform, vehicle_data.VehiclePartAtIndex( vehicle_stickman.Count ) );
		vehicle_stickman.Add( incomingStickman );
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
