/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using FFStudio;

public class Test_VehicleObserve : MonoBehaviour
{
#region Fields
    [ SerializeField ] Pool_Stickman pool_stickman;
    [ SerializeField ] VehicleData vehicle_data;
    [ SerializeField ] Transform vehicle_gfx;
#endregion

#region Properties
#endregion

#region Unity API
    private void Start()
    {
		SpawnStickmans();
	}
#endregion

#region API
    [ Button() ]
    public void SpawnStickmans()
    {
		vehicle_gfx.localPosition = vehicle_data.VehiclePosition;
		vehicle_gfx.DestroyAllChildren();

		for( var i = 0; i < vehicle_data.VehicleCountMax; i++ )
		{
			var stickman = pool_stickman.GetEntity();
			stickman.SpawnIntoVehicle( vehicle_gfx, vehicle_data.VehiclePartAtIndex( i ) );
		}

	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
