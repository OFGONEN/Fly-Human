/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace FFStudio
{
	/* This class holds references to ScriptableObject assets. These ScriptableObjects are singletons, so they need to load before a Scene does.
	 * Using this class ensures at least one script from a scene holds a reference to these important ScriptableObjects. */
	public class AssetManager : MonoBehaviour
	{
#region Fields
	[ Title( "UnityEvent" ) ]
	[ SerializeField ] UnityEvent onAwakeEvent;
	[ SerializeField ] UnityEvent onEnableEvent;
	[ SerializeField ] UnityEvent onStartEvent;

	[ Title( "Setup" ) ]
		[ SerializeField ] GameSettings gameSettings;
		[ SerializeField ] CurrentLevelData currentLevelData;

	[ Title( "Pool" ) ]
		[ SerializeField ] Pool_UIPopUpText pool_UIPopUpText;
		[ SerializeField ] Pool_TargetStickman pool_stickman_target;

	[ Title( "Shared Variables" ) ]
		[ SerializeField ] TargetVehicleData[] target_vehicle_data_array;
		[ SerializeField ] Set_TargetStickman set_target_stickman;

#endregion

#region UnityAPI
		void OnEnable()
		{
			onEnableEvent.Invoke();
		}

		void Awake()
		{
			Vibration.Init();

			pool_UIPopUpText.InitPool( transform, false );
			pool_stickman_target.InitPool( transform, false );

			onAwakeEvent.Invoke();

			for( var i = 0; i < target_vehicle_data_array.Length; i++ )
				target_vehicle_data_array[ i ].CheckIfUnLocked();

			set_target_stickman.InitSet();
		}

		void Start()
		{
			onStartEvent.Invoke();
		}
#endregion



#region API
		public void VibrateAPI( IntGameEvent vibrateEvent )
		{
			switch ( vibrateEvent.eventValue )
			{
				case 0:
					Vibration.VibratePeek();
					break;
				case 1:
					Vibration.VibratePop();
					break;
				case 2:
					Vibration.VibrateNope();
					break;
				default:
					Vibration.Vibrate();
					break;
			}
		}
#endregion

#region Implementation
#endregion
	}
}