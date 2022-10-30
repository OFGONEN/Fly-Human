using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using TMPro;
using Sirenix.OdinInspector;

public class UIVehicleProgress : MonoBehaviour
{
  [ Title( "Setup" ) ]
    [ SerializeField ] Image[] vehicle_icon_array;
    [ SerializeField ] Image vehicle_cursor;
    [ SerializeField ] TextMeshProUGUI vehicle_name;

  [ Title( "Shared" ) ]
    [ SerializeField ] Pool_UI_Image pool_ui_image_progress;

// Private
    List< Image > vehicle_icon_list;
    List< Image > vehicle_progress_list;
    
    void Awake()
    {
		vehicle_icon_list     = new List< Image >( vehicle_icon_array.Length );
		vehicle_progress_list = new List< Image >( 128 );
	}

    public void OnLevelLoaded()
    {
		var levelData = CurrentLevelData.Instance.levelData;

		vehicle_cursor.gameObject.SetActive( true );
		vehicle_name.gameObject.SetActive( true );
		vehicle_name.text = levelData.vehicle_data_array[ 0 ].VehicleName;

		float position = 0;

		for( var i = 0; i < levelData.vehicle_data_array.Length; i++ )
        {
			var vehicleData = levelData.vehicle_data_array[ i ];
			vehicle_icon_list.Add( vehicle_icon_array[ i ] );
			vehicle_icon_array[ i ].sprite = vehicleData.VehicleIcon;

			var vehicleIconRectTransform = vehicle_icon_array[ i ].rectTransform;
			vehicleIconRectTransform.localPosition = Vector3.right.SetX( position );

			position += GameSettings.Instance.ui_vehicle_progress_gapDistance;

			for( var x = 0; x < vehicleData.VehicleCountMax - vehicleData.VehicleCountMin; x++ )
            {
				var vehicleProgressIcon = pool_ui_image_progress.GetEntity();
				vehicleProgressIcon.rectTransform.localPosition = Vector3.right.SetX( position );

				vehicle_progress_list.Add( vehicleProgressIcon );

				position += GameSettings.Instance.ui_vehicle_progress_gapDistance;
			}
		}
	}

    public void OnLevelStart()
    {
		vehicle_cursor.gameObject.SetActive( true );
		vehicle_name.gameObject.SetActive( true );

		for( var i = 0; i < vehicle_icon_list.Count; i++ )
			vehicle_icon_list[ i ].gameObject.SetActive( true );

		for( var i = 0; i < vehicle_progress_list.Count; i++ )
			vehicle_progress_list[ i ].gameObject.SetActive( true );

		vehicle_progress_list.Clear();
		vehicle_icon_list.Clear();
    }

    public void OnLoadNewLevel()
    {
		vehicle_cursor.gameObject.SetActive( false );
		vehicle_name.gameObject.SetActive( false );

		for( var i = 0; i < vehicle_icon_list.Count; i++ )
			vehicle_icon_list[ i ].gameObject.SetActive( false );

		for( var i = 0; i < vehicle_progress_list.Count; i++ )
			pool_ui_image_progress.ReturnEntity( vehicle_progress_list[ i ] );

		vehicle_progress_list.Clear();
		vehicle_icon_list.Clear();
	}
}