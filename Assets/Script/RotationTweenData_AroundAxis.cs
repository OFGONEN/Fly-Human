/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class RotationTweenData_Axis : TweenData
    {
        public enum RotationMode { Local, World }
        
#region Fields
    [ Title( "Rotation Tween" ) ]
#if UNITY_EDITOR
		[ InfoBox( "End Value is ABSOLUTE." ) ]
#endif
        [ BoxGroup( "Tween" ), PropertyOrder( int.MinValue ), SuffixLabel( "Degrees (°)" ) ] public float endValue;
        [ BoxGroup( "Tween" ), PropertyOrder( int.MinValue ), Min( 0 ) ] public float duration;
        [ BoxGroup( "Tween" ), PropertyOrder( int.MinValue ) ] public Vector3 originAsOffset = Vector3.zero;
        [ BoxGroup( "Tween" ), PropertyOrder( int.MinValue ), LabelText( "Rotate Around" ) ] public RotationAxisVector rotationAxisVector;
#endregion

#region Properties
        Vector3 Axis => rotationAxisVector == RotationAxisVector.Right
                            ? transform.right
                            : rotationAxisVector == RotationAxisVector.Forward
                                ? transform.forward
                                : transform.up;
#if UNITY_EDITOR
#endif
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
        protected override void CreateAndStartTween( UnityMessage onComplete, bool isReversed = false )
        {
            // Info: Move transform so the end point (.ie, head of the Stickman) matches the origin of rotation.
			var endPoint_worldSpace = transform.position + transform.up * 1.77f;
			var delta = originAsOffset - transform.parent.InverseTransformPoint( endPoint_worldSpace );
            transform.localPosition = transform.localPosition + delta;

			recycledTween.Recycle( DOVirtual.DelayedCall( duration, ExtensionMethods.EmptyMethod ), onComplete );

			recycledTween.Tween
				 .SetEase( easing )
				 .SetLoops( loop ? -1 : 0, loopType )
				 .OnUpdate( OnTweenUpdate );

#if UNITY_EDITOR
			recycledTween.Tween.SetId( "_ff_rotation_tween_axis___" + description );
#endif

			base.CreateAndStartTween( onComplete, isReversed );
		}
        
        void OnTweenUpdate()
        {
			var originInWorldSpace = transform.parent.TransformPoint( originAsOffset );
			var axisInWorldSpace = Axis;

			transform.RotateAround( originInWorldSpace, axisInWorldSpace, endValue * Time.deltaTime );
		}
#endregion

#region EditorOnly
#if UNITY_EDITOR
#endif
#endregion
    }
}
