using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Dreamteck.Splines;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "tool_level_creator", menuName = "FF/Tool/Level Creator" ) ]
public class LevelCreator : ScriptableObject
{
    [ SerializeField ] Vector2 level_peak_height;
    [ SerializeField ] int level_peak_count;
    [ SerializeField ] Vector2 level_peak_distance;
    [ SerializeField ] Vector2 level_drop_height;
    [ SerializeField ] Vector2 level_drop_placement;

    [ ShowInInspector, ReadOnly ] Vector3[] level_point_peak_list;
    [ ShowInInspector, ReadOnly ] Vector3[] level_point_drop_list;

    [ Button() ]
    public void ConstructLevel()
    {
		level_point_peak_list = new Vector3[ level_peak_count ];
		level_point_drop_list = new Vector3[ level_peak_count - 1 ];

		float peakForward = 0;

		for( var i = 0; i < level_point_peak_list.Length; i++ )
        {
			level_point_peak_list[ i ] = Vector3.up * level_peak_height.ReturnRandom() + Vector3.forward * peakForward;
			peakForward += level_peak_distance.ReturnRandom();
		}
        
        for( var i = 0; i < level_point_drop_list.Length; i++ )
        {
			var forward = Mathf.Lerp( level_point_peak_list[ i ].z, level_point_peak_list[ i + 1 ].z, level_drop_placement.ReturnRandom() );
			level_point_drop_list[ i ] = Vector3.up * level_drop_height.ReturnRandom() + Vector3.forward * forward;
		}

		SetSplineComputerPoints();
	}

    public void SetSplineComputerPoints()
    {
        var spline = GameObject.Find( "spline" ).GetComponent< SplineComputer >();

        for( var i = 0; i < level_point_peak_list.Length; i++ )
        {
			var splinePointPeak = new SplinePoint( level_point_peak_list[ i ] );
			splinePointPeak.normal = Vector3.up;

			spline.SetPoint( i * 2, splinePointPeak );
		}

		for( var i = 0; i < level_point_drop_list.Length; i++ )
		{
			var splinePointDrop = new SplinePoint( level_point_drop_list[ i ] );
			splinePointDrop.normal = Vector3.up;

			spline.SetPoint( i * 2 + 1, splinePointDrop );
		}
    }
}