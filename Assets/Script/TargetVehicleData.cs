using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

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
}