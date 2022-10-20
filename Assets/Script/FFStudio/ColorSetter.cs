/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	public class ColorSetter : MonoBehaviour
	{
#region Fields
	  [ Title( "Setup" ) ]
		[ SerializeField ] Color color;

		static int SHADER_ID_COLOR = Shader.PropertyToID( "_BaseColor" );

		Renderer theRenderer;
		MaterialPropertyBlock propertyBlock;
		
		public Color Color => color;
#endregion

#region Properties
#endregion

#region Unity API
		void Awake()
		{
			theRenderer   = GetComponent< Renderer >();
			propertyBlock = new MaterialPropertyBlock();
		}
#endregion

#region API
		public void SetColor( Color color )
		{
			this.color = color;

			SetColor();
		}

		[ Button ]
		public void SetColor() // Info: This may be more "Unity-Event-friendly".
		{
			theRenderer.GetPropertyBlock( propertyBlock );
			propertyBlock.SetColor( SHADER_ID_COLOR, color );
			theRenderer.SetPropertyBlock( propertyBlock );
		}
		
		public void SetAlpha( float alpha )
		{
			theRenderer.GetPropertyBlock( propertyBlock );
			var currentColor = theRenderer.sharedMaterial.GetColor( SHADER_ID_COLOR );
			propertyBlock.SetColor( SHADER_ID_COLOR, currentColor.SetAlpha( alpha ) );
			theRenderer.SetPropertyBlock( propertyBlock );
		}

		public Tween LerpColor( Color endColor, float duration )
		{
			return DOTween.To( GetCurrentColor, SetCurrentColor, endColor, duration );
		}
#endregion

#region Implementation
		Color GetCurrentColor()
		{
			return color;
		}

		void SetCurrentColor( Color color )
		{
			this.color = color;
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}