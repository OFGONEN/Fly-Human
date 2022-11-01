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
    [ SerializeField ] Vector2 level_peak_height;
    [ SerializeField ] int level_peak_count;
    [ SerializeField ] Vector2 level_peak_distance;
    [ SerializeField ] Vector2 level_drop_height;
    [ SerializeField ] Vector2 level_drop_placement;

    [ ShowInInspector, ReadOnly ] List< Vector3 > level_point_peak_list;
    [ ShowInInspector, ReadOnly ] List< Vector3 > level_point_drop_list;

    [ Button() ]
    public void ConstructLevel()
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

    public void SetSplineComputerPoints()
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
}