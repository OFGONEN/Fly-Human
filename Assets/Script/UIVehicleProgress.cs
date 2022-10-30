using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class UIVehicleProgress : MonoBehaviour
{
  [ Title( "Setup" ) ]
    [ SerializeField ] Image[] vehicle_icon_array;
    [ SerializeField ] Image vehicle_cursor;
    [ SerializeField ] TextMeshProUGUI vehicle_name;
    [ SerializeField ] RectTransform vehicle_ui_parent;

  [ Title( "Shared" ) ]
    [ SerializeField ] Pool_UI_Image pool_ui_image_progress;

// Private
    RecycledTween recycledTween = new RecycledTween();

    List< Image > vehicle_icon_list;
    List< Image > vehicle_progress_list;

	int vehicle_stickman_count_start;

	void Awake()
    {
		vehicle_icon_list     = new List< Image >( vehicle_icon_array.Length );
		vehicle_progress_list = new List< Image >( 128 );
	}

    public void OnLevelLoaded()
    {
		var levelData = CurrentLevelData.Instance.levelData;

		vehicle_name.text = levelData.vehicle_data_array[ levelData.vehicle_start_index ].VehicleName;

		vehicle_stickman_count_start = levelData.vehicle_data_array[ 0 ].VehicleCountMin;
		float position = 0;

		for( var i = 0; i < levelData.vehicle_data_array.Length; i++ )
        {
			var vehicleData = levelData.vehicle_data_array[ i ];
			vehicle_icon_list.Add( vehicle_icon_array[ i ] );
			vehicle_icon_array[ i ].sprite = vehicleData.VehicleIcon;

			var vehicleIconRectTransform = vehicle_icon_array[ i ].rectTransform;
			vehicleIconRectTransform.localPosition = Vector3.right.SetX( position );

			position += GameSettings.Instance.ui_vehicle_progress_distance;

			for( var x = 0; x < vehicleData.VehicleCountMax - vehicleData.VehicleCountMin; x++ )
            {
				var vehicleProgressIcon = pool_ui_image_progress.GetEntity();

				vehicleProgressIcon.rectTransform.SetParent( vehicle_ui_parent );
				vehicleProgressIcon.rectTransform.localPosition = Vector3.right.SetX( position );

				vehicle_progress_list.Add( vehicleProgressIcon );

				position += GameSettings.Instance.ui_vehicle_progress_distance;
			}
		}

		vehicle_progress_list.RemoveAt( vehicle_progress_list.Count - 1 );
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

		OnVehicleStickmanCountChanged( CurrentLevelData.Instance.levelData.vehicle_start_count );
	}

    public void OnLoadNewLevel()
    {
		recycledTween.Kill();

		vehicle_cursor.gameObject.SetActive( false );
		vehicle_name.gameObject.SetActive( false );

		for( var i = 0; i < vehicle_icon_list.Count; i++ )
			vehicle_icon_list[ i ].gameObject.SetActive( false );

		for( var i = 0; i < vehicle_progress_list.Count; i++ )
			pool_ui_image_progress.ReturnEntity( vehicle_progress_list[ i ] );

		vehicle_progress_list.Clear();
		vehicle_icon_list.Clear();
	}

    public void OnVehicleChanged( IntGameEvent gameEvent )
    {
		vehicle_name.text = CurrentLevelData.Instance.levelData.vehicle_data_array[ gameEvent.eventValue ].VehicleName;
	}

	public void OnVehicleStickmanCountChanged( int count )
    {
		var diff = count - vehicle_stickman_count_start;
		var targetPosition = Vector3.right.SetX( -diff * GameSettings.Instance.ui_vehicle_progress_distance );

		recycledTween.Recycle( vehicle_ui_parent.DOLocalMove( targetPosition, GameSettings.Instance.ui_vehicle_progress_travel_duration )
			.SetEase( GameSettings.Instance.ui_vehicle_progress_travel_ease ) );
	}
}