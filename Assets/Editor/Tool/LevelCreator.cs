using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;

[ CreateAssetMenu( fileName = "tool_level_creator", menuName = "FF/Tool/Level Creator" ) ]
public class LevelCreator : ScriptableObject
{
  [ Title( "Construct Platform" ) ]
    [ SerializeField ] Vector2 level_peak_height;
    [ SerializeField ] int level_peak_count;
    [ SerializeField ] Vector2 level_peak_distance;
    [ SerializeField ] Vector2 level_drop_height;
    [ SerializeField ] Vector2 level_drop_placement;
  [ Title( "Place Eject Points" ) ]
    [ SerializeField ] float level_eject_placement_ratio;
	[ SerializeField ] GameObject level_object_eject;
  [ Title( "Place Stickmen" ) ]
	[ SerializeField ] GameObject level_object_stickman;
	[ SerializeField ] int level_stickmen_count;
	[ SerializeField ] int level_stickman_step;

    [ ShowInInspector, ReadOnly ] List< Vector3 > level_point_peak_list;
    [ ShowInInspector, ReadOnly ] List< Vector3 > level_point_drop_list;
    [ ShowInInspector, ReadOnly ] List< GameObject > level_stickman_cache = new List< GameObject >();

	[ Button() ]
	void PlaceStickmen()
	{
		level_stickman_cache.Clear();
		var index   = GameObject.Find( "--- Entities_Start ---" ).transform.GetSiblingIndex();
		var forward = GameObject.Find( "cursor_stickman" ).transform.position.z;

		for( var i = 0; i < level_stickmen_count; i++ )
		{
			var stickmanObject = PrefabUtility.InstantiatePrefab( level_object_stickman ) as GameObject;
			PlaceTransformOnPlatform( stickmanObject.transform, forward );
			stickmanObject.transform.SetSiblingIndex( index + 1 );

			level_stickman_cache.Add( stickmanObject );

			forward += level_stickman_step;
		}
	}

	[ Button() ]
	void DeleteStickmanCache()
	{
		for( var i = 0; i < level_stickman_cache.Count; i++ )
			GameObject.DestroyImmediate( level_stickman_cache[ i ] );

		level_stickman_cache.Clear();
	}

    [ Button() ]
    void ConstructLevel()
    {
		EditorSceneManager.MarkAllScenesDirty();

		level_point_peak_list = new List< Vector3 >();
		level_point_drop_list = new List< Vector3 >();

		float peakForward = 0;

		for( var i = 0; i < level_peak_count; i++ )
        {
			level_point_peak_list.Add( Vector3.up * level_peak_height.ReturnRandom() + Vector3.forward * peakForward );
			peakForward += level_peak_distance.ReturnRandom();
		}

        for( var i = 0; i < level_peak_count - 1; i++ )
        {
			var forward = Mathf.Lerp( level_point_peak_list[ i ].z, level_point_peak_list[ i + 1 ].z, level_drop_placement.ReturnRandom() );
			level_point_drop_list.Add( Vector3.up * level_drop_height.ReturnRandom() + Vector3.forward * forward );
        }

		level_point_drop_list.Insert( 0, Vector3.up * level_drop_height.ReturnRandom() + Vector3.forward * -1f * level_peak_distance.ReturnRandom() * level_drop_placement.ReturnRandom() );

		level_point_drop_list.Add( Vector3.forward * level_point_peak_list[ level_point_peak_list.Count - 1 ].z + Vector3.forward * level_peak_distance.ReturnRandom() * level_drop_placement.ReturnRandom() );

		SetSplineComputerPoints();
	}

    void SetSplineComputerPoints()
    {
        var spline = GameObject.Find( "spline" ).GetComponent< SplineComputer >();

		SplinePoint[] splinePointArray = new SplinePoint[ level_point_drop_list.Count + level_point_peak_list.Count ];
		// spline.SetPoints( null );

		for( var i = 0; i < level_point_drop_list.Count; i++ )
		{
			var splinePointDrop = new SplinePoint( level_point_drop_list[ i ] );
			splinePointDrop.normal = Vector3.up;

			splinePointArray[ i * 2 ] = splinePointDrop;
			// spline.SetPoint( i * 2, splinePointDrop );
		}

        for( var i = 0; i < level_point_peak_list.Count; i++ )
        {
			var splinePointPeak = new SplinePoint( level_point_peak_list[ i ] );
			splinePointPeak.normal = Vector3.up;

			splinePointArray[ i * 2 + 1 ] = splinePointPeak;
			// spline.SetPoint( i * 2 + 1, splinePointPeak );
		}

		spline.SetPoints( splinePointArray );
	}

	[ Button() ]
	void PlaceEjectPoints()
	{
		EditorSceneManager.MarkAllScenesDirty();

		for( var i = 0; i < level_point_peak_list.Count - 1; i++ )
		{
			var currentPeak = level_point_peak_list[ i ].z;
			var nextPeak    = level_point_peak_list[ i + 1 ].z;

			var forwardPosition = currentPeak + ( nextPeak - currentPeak ) * level_eject_placement_ratio;
			var ejectGameObject = PrefabUtility.InstantiatePrefab( level_object_eject ) as GameObject;
			PlaceTransformOnPlatform( ejectGameObject.transform , forwardPosition );
		}
	}

	void PlaceTransformOnPlatform( Transform transform, float forward )
	{
		//Info: We are presuming that vehicle always above the platform
		RaycastHit hitInfo_Origin;
		RaycastHit hitInfo_Target;

		var mask = LayerMask.GetMask( ExtensionMethods.Layer_Platform );

		var hitPosition_Origin = new Vector3( 0, GameSettings.Instance.vehicle_rayCast_height, forward );
		var hitPosition_Target = hitPosition_Origin + GameSettings.Instance.game_forward * GameSettings.Instance.vehicle_movement_step;

		Physics.Raycast( hitPosition_Origin, GameSettings.Instance.vehicle_rayCast_direction, out hitInfo_Origin, GameSettings.Instance.vehicle_rayCast_distance, mask );
		Physics.Raycast( hitPosition_Target, GameSettings.Instance.vehicle_rayCast_direction, out hitInfo_Target, GameSettings.Instance.vehicle_rayCast_distance, mask );

		transform.position = hitInfo_Origin.point;
		transform.LookAtAxis( hitInfo_Target.point, GameSettings.Instance.vehicle_movement_look_axis );
		transform.Rotate( Vector3.up * 180f, Space.Self );
	}
}