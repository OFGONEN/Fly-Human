using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FFStudio;
using Sirenix.OdinInspector;

public class TargetVehicle : MonoBehaviour
{
  [ Title( "Shared" ) ]
    [ SerializeField ] Pool_TargetStickman pool_stickman;
    [ SerializeField ] Set_TargetStickman set_stickman;
    [ SerializeField ] TargetVehicleData vehicle_data;
    [ SerializeField ] GameEvent event_vehicle_unlocked;
    [ SerializeField ] GameEvent event_vehicle_progressed;

    int vehicle_stickman_count;

// Private

    void Awake()
    {
		set_stickman.ClearSet();

		SpawnStickmans();

		vehicle_stickman_count = PlayerPrefsUtility.Instance.GetInt( ExtensionMethods.Key_Plane_Count, 0 );

		int index = 0;
		int counter = 0;

        while( counter < vehicle_stickman_count )
        {
			var stickman = set_stickman.GetFromList( index );
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

	public void OnStickmanMovementDone()
	{
		if( vehicle_stickman_count >= vehicle_data.VehicleStickmanCount )
		{
			vehicle_data.Unlock();
			event_vehicle_unlocked.Raise();
		}
		else
			event_vehicle_progressed.Raise();
	}

    void SpawnStickmans()
    {
		for( var i = 0; i < vehicle_data.VehiclePartCount; i++ )
        {
			var stickman = pool_stickman.GetEntity();
			stickman.SpawnIntoPart( transform, vehicle_data.GetTargetVehiclePartData( i ) );

			set_stickman.AddList( stickman );
		}
    }
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		var localPosition = transform.TransformPoint( vehicle_data.VehiclePosition );
		Handles.DrawWireCube( localPosition, Vector3.one / 2f );
		Handles.Label( localPosition, vehicle_data.VehicleName + " Position" );
	}
#endif
}