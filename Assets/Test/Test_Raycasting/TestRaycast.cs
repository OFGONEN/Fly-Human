/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class TestRaycast : MonoBehaviour
{
#region Fields
    public Vector3 startPosition;
    public Vector3 endPosition;
    public int lineCount;

    [ ShowInInspector, ReadOnly ] List< Vector3 > hitPositions = new List< Vector3 >();
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void CastRaycasts()
    {
		hitPositions.Clear();
		var layerMask = ~LayerMask.GetMask( "Platform" );

		for( var i = 0; i < lineCount; i++ )
        {
			var origin = Vector3.Lerp( startPosition, endPosition, ( float )i / lineCount );
			RaycastHit hitInfo;
			var hit = Physics.Raycast( origin, Vector3.down, out hitInfo, 1000, layerMask );

            if( hit )
				hitPositions.Add( hitInfo.point );
		}
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
		Handles.DrawWireCube( startPosition, Vector3.one * 0.25f );
		Handles.Label( startPosition, "Start Position" );
		Handles.DrawWireCube( endPosition, Vector3.one * 0.25f );
		Handles.Label( endPosition, "End Position" );

		for( var i = 0; i < hitPositions.Count - 1; i++ )
			Gizmos.DrawLine( hitPositions[ i ], hitPositions[ i + 1 ] );
	}
#endif
#endregion
}