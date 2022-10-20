using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "vehicle_data_", menuName = "FF/Game/Vehicle Data" ) ]
public class VehicleData : ScriptableObject
{
    [ SerializeField ] VehiclePartData[] vehicle_data_array;
    [ SerializeField ] string vehicle_name;
    [ SerializeField, Min( 1 ) ] int vehicle_count_min;

	public string VehicleName => vehicle_name;
	public int VehicleCountMin => vehicle_count_min;
	public int VehicleCountMax => vehicle_data_array.Length;

    public VehiclePartData VehiclePartAtIndex( int index )
    {
		return vehicle_data_array[ index ];
	}
}