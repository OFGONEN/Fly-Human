/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class BoneSetter : MonoBehaviour
{
#region Fields
    public TransformData[] transform_data_array; 
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void CacheTheBones()
    {
        var transform_array = GetComponentsInChildren< Transform >();
		transform_data_array = new TransformData[ transform_array.Length ];

		for( var i = 0; i < transform_array.Length; i++ )
        {
			transform_data_array[ i ] = new TransformData( transform_array[ i ] );
		}
    }

    [ Button() ]
    public void SetTheBones()
    {
		var transform_array = GetComponentsInChildren< Transform >();

		for( var i = 0; i < transform_array.Length; i++ )
		{
			var child = transform_array[ i ];
			var data = transform_data_array[ i ];
			child.position = data.position;
			child.eulerAngles = data.rotation;
			child.localScale = data.scale;
		}
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
