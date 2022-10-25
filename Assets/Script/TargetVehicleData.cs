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
	[ LabelText( "Vehicle Stickman Move Position" ), SerializeField ] Vector3 vehicle_position;
	[ SerializeField, ReadOnly ] int vehicle_stickman_count;

	public string VehicleName       => vehicle_name;
	public int VehiclePartCount     => vehicle_data_array.Length;
	public int VehicleStickmanCount => vehicle_stickman_count;
	public Vector3 VehiclePosition  => vehicle_position;
	public bool VehicleIsUnlocked   => vehicle_is_unlocked;

	public TargetVehiclePartData GetTargetVehiclePartData( int index )
	{
		return vehicle_data_array[ index ];
	}

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
    void CreateDataFromVehicleData( VehicleData vehicleData, Color startColor )
    {
		UnityEditor.EditorUtility.SetDirty( this );

		vehicle_name       = vehicleData.VehicleName;
		vehicle_data_array = new TargetVehiclePartData[ vehicleData.VehicleCountMax ];

		for( var i = 0; i < vehicle_data_array.Length; i++ )
		{
			var data = vehicleData.VehiclePartAtIndex( i );
			vehicle_data_array[ i ] = new TargetVehiclePartData( data );
			vehicle_data_array[ i ].color_start = startColor;
		}
	}

	[ Button() ]
	void GetPoseDataFromVehicleData( VehicleData vehicleData )
	{
		UnityEditor.EditorUtility.SetDirty( this );

		for( var i = 0; i < vehicle_data_array.Length; i++ )
		{
			vehicle_data_array[ i ].pose = vehicleData.VehiclePartAtIndex( i ).pose;
		}
	}

	[ Button() ]
	void ChangeStartColors( Color color )
	{
		UnityEditor.EditorUtility.SetDirty( this );

		for( var i = 0; i < vehicle_data_array.Length; i++ )
		{
			vehicle_data_array[ i ].color_start = color;
		}
	}

	void OnValidate()
	{
		UnityEditor.EditorUtility.SetDirty( this );

		vehicle_stickman_count = 0;

		for( var i = 0; i < vehicle_data_array.Length; i++ )
		{
			vehicle_stickman_count += vehicle_data_array[ i ].count;
		}
	}
#endif
}