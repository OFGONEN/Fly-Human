using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class TargetVehicle : MonoBehaviour
{
  [ Title( "Setup" ) ]
    [ SerializeField ] Pool_TargetStickman pool_stickman;
    [ SerializeField ] TargetVehicleData vehicle_data;

    int vehicle_stickman_count;

// Private
    List< TargetStickman > stickman_list;

    void Awake()
    {
		SpawnStickmans();

		vehicle_stickman_count = PlayerPrefsUtility.Instance.GetInt( ExtensionMethods.Key_Plane_Count, 0 );

		int index = 0;
		int counter = 0;

        while( counter < vehicle_stickman_count )
        {
			var stickman = stickman_list[ index ];
			stickman.IncreaseCount();

			counter++;

			if( stickman.StickmanMaxed )
				index++;
		}
	}

    public void IncreaseCount()
    {
		vehicle_stickman_count++;
	}

    void SpawnStickmans()
    {
		stickman_list = new List< TargetStickman >( vehicle_data.VehiclePartCount );

		for( var i = 0; i < vehicle_data.VehiclePartCount; i++ )
        {
			var stickman = pool_stickman.GetEntity();
			stickman.SpawnIntoPart( transform, vehicle_data.GetTargetVehiclePartData( i ) );

			stickman_list.Add( stickman );
		}
    }
}