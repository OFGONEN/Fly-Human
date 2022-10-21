using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "target_vehicle_data_", menuName = "FF/Game/Target Vehicle Data" ) ]
public class TargetVehicleData : ScriptableObject
{
    [ SerializeField ] TargetVehiclePartData[] vehicle_data_array;
    [ SerializeField ] string vehicle_name;
    [ SerializeField ] bool vehicle_is_unlocked;

	public string VehicleName     => vehicle_name;
	public bool VehicleIsUnlocked => vehicle_is_unlocked;

    public void CheckIfUnLocked()
    {
		vehicle_is_unlocked = PlayerPrefs.HasKey( vehicle_name );
	}

    public void Unlock()
    {
		vehicle_is_unlocked = true;
		PlayerPrefsUtility.Instance.SetInt( vehicle_name, 1 );
	}

#if UNITY_EDITOR
	[ Button() ]
    void CreateDataFromVehicleData( VehicleData vehicleData )
    {
		UnityEditor.EditorUtility.SetDirty( this );

		vehicle_name       = vehicleData.VehicleName;
		vehicle_data_array = new TargetVehiclePartData[ vehicleData.VehicleCountMax ];

		for( var i = 0; i < vehicle_data_array.Length; i++ )
		{
			var data = vehicleData.VehiclePartAtIndex( i );
			vehicle_data_array[ i ] = new TargetVehiclePartData( data );
		}

		UnityEditor.AssetDatabase.SaveAssetIfDirty( this );
	}
#endif
}