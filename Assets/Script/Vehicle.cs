using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Vehicle : MonoBehaviour
{
  [ Title( "Setup" ) ]
    [ SerializeField ] Pool_Stickman pool_stickman;
    [ SerializeField ] Set_TargetStickman set_stickman_target;
    [ SerializeField ] SharedReferenceNotifier notif_vehicle_target;
    [ SerializeField ] IntGameEvent event_vehicle_changed;

  [ Title( "Setup" ) ]
    [ SerializeField ] Transform vehicle_gfx;
    [ SerializeField ] SphereCollider vehicle_collider;

// Private
    [ ShowInInspector, ReadOnly ] List< Stickman > vehicle_stickman = new List< Stickman >( 64 );
    [ ShowInInspector, ReadOnly ] List< Vector3 > vehicle_stickman_position = new List< Vector3 >( 64 );
	[ ShowInInspector, ReadOnly ] VehicleData vehicle_data;
	[ ShowInInspector, ReadOnly ] int vehicle_index;

	void Start()
    {
		var levelData = CurrentLevelData.Instance.levelData;

		ChangeVehicleData( levelData.vehicle_start_index );

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
			stickman.AttachToVehicle( vehicle_gfx, vehicle_data.VehiclePartAtIndex( stickman_count ) );
			vehicle_stickman.Add( stickman );
		}
	}

	public void OnLooseStickman( float percentage )
	{
		var count = vehicle_stickman.Count * percentage / 100f;
		count = Mathf.Max( 1, Mathf.FloorToInt( count ) );

		OnLooseStickman( count );
	}

	[ Button() ]
	public void OnLooseStickman( int count )
	{
		var stickmanCount   = vehicle_stickman.Count;
		var surplusStickman = stickmanCount - vehicle_data.VehicleCountMin;
		var looseCount      = Mathf.Min( surplusStickman, count );
		var devolve         = stickmanCount - looseCount == vehicle_data.VehicleCountMin;
	
#if UNITY_EDITOR
		if( stickmanCount - looseCount < vehicle_data.VehicleCountMin )
		{
			FFLogger.LogError( "This shouldn't happen!!" +
				"Stickman Count: " + stickmanCount +
				"Loose Count: " + count +
				"Real Loose Count: " + looseCount );
		}
#endif
		int startIndex = stickmanCount - 1;

		for( var i = startIndex; i > startIndex - looseCount; i-- )
		{
			vehicle_stickman[ i ].FallFromVehicle();
			vehicle_stickman.RemoveAt( i );
		}

		if( devolve && vehicle_index > 0 )
			Devolve();
	}

	[ Button() ]
	public void OnFinishLine()
	{
		var targetVehicle = notif_vehicle_target.sharedValue as TargetVehicle;
		var stickmanIndex = 0;

		for( var i = 0; i < set_stickman_target.ListCount; i++ )
		{
			var targetStickman = set_stickman_target.GetFromList( i );
			var vehicleIndexTarget = stickmanIndex + targetStickman.StickmanDeficiency;

			for( ; stickmanIndex < vehicleIndexTarget && stickmanIndex < vehicle_stickman.Count; stickmanIndex++ )
			{
				vehicle_stickman[ stickmanIndex ].MoveTowardsTargetVehicle( targetVehicle, targetStickman );
			}
		}

		var targetVehiclePosition = targetVehicle.TargetVehiclePosition;

		for( var i = stickmanIndex; i < vehicle_stickman.Count; i++ )
		{
			vehicle_stickman[ i ].MoveTowardsPosition( targetVehiclePosition );
		}
	}

	void Evolve( Stickman incomingStickman )
	{
		ChangeVehicleData( vehicle_index + 1 );

		ChangeAllStickmenToParts();

		incomingStickman.AttachToVehicle( vehicle_gfx, vehicle_data.VehiclePartAtIndex( vehicle_stickman.Count ) );
		vehicle_stickman.Add( incomingStickman );
	}

	void Devolve()
	{
		ChangeVehicleData( vehicle_index - 1 );
		ChangeAllStickmenToParts();
	}

	void ChangeAllStickmenToParts()
	{
		for( var i = 0; i < vehicle_stickman.Count; i++ )
			vehicle_stickman[ i ].ChangeToAnoterPart( vehicle_data.VehiclePartAtIndex( i ) );
	}

    void SpawnStickman( int count )
    {
        for( var i = 0; i < count; i++ )
        {
		    var stickman = pool_stickman.GetEntity();
			vehicle_stickman.Add( stickman );

			stickman.SpawnIntoVehicle( vehicle_gfx, vehicle_data.VehiclePartAtIndex( i ) );
		}
	}

	void ChangeVehicleData( int index )
	{
		vehicle_index = index;
		vehicle_data  = CurrentLevelData.Instance.levelData.vehicle_data_array[ vehicle_index ];

		CacheStickmanPositions();
		vehicle_gfx.localPosition = vehicle_data.VehiclePosition;
		SetStickmanPositions();

		event_vehicle_changed.Raise( vehicle_index );

		var colliderData                             = vehicle_data.VehicleCollider;
		    vehicle_collider.transform.localPosition = colliderData.position;
		    vehicle_collider.radius                  = colliderData.size;
	}

	void CacheStickmanPositions()
	{
		vehicle_stickman_position.Clear();

		for( var i = 0; i < vehicle_stickman.Count; i++ )
		{
			vehicle_stickman_position.Add( vehicle_stickman[ i ].transform.position );
		}
	}

	void SetStickmanPositions()
	{
		for( var i = 0; i < vehicle_stickman.Count; i++ )
		{
			vehicle_stickman[ i ].transform.position = vehicle_stickman_position[ i ];
		}
	}
}
