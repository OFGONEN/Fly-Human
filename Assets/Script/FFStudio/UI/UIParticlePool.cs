/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "pool_ui_particle", menuName = "FF/Data/Pool/UI Particle Pool" ) ]
public class UIParticlePool : ComponentPool< UIParticle > 
{
	[ Button() ]
	public void Spawn( Vector3 worldPosition, OnCompleteMessage onComplete = null )
	{
		GetEntity().Spawn( worldPosition, onComplete );
	}
}