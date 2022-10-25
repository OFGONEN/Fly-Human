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
    [ SerializeField ] VehicleColliderData vehicle_collider;

	public string VehicleName                  => vehicle_name;
	public int VehicleCountMin                 => vehicle_count_min;
	public int VehicleCountMax                 => vehicle_data_array.Length;
	public VehicleColliderData VehicleCollider => vehicle_collider;

    public VehiclePartData VehiclePartAtIndex( int index )
    {
		return vehicle_data_array[ index ];
	}

#if UNITY_EDITOR
	public void SetVehicleDataArray( VehiclePartData[] array )
	{
		UnityEditor.EditorUtility.SetDirty( this );

		vehicle_data_array = array;
	}

	public void SetVehicleColliderData( SphereCollider collider )
	{
		UnityEditor.EditorUtility.SetDirty( this );

		vehicle_collider.position = collider.center;
		vehicle_collider.size     = collider.radius;
	}

    void OnValidate()
    {
		if( vehicle_data_array == null ) return;

		UnityEditor.EditorUtility.SetDirty( this );

		for( var i = 0; i < vehicle_data_array.Length; i++ )
        {
			vehicle_data_array[ i ].color = vehicle_data_array[ i ].color.SetAlpha( 1 );
		}
	}
#endif
}