/* Created by and for usage of FF Studios (2021). */

using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class ToggleRagdoll : MonoBehaviour
    {
#region Fields
        [ SerializeField ] Rigidbody[] ragdoll_rigidbody_array;
        [ SerializeField ] Collider[] ragdoll_rigidbody_collider_array;
        [ SerializeField ] TransformData[] ragdoll_rigidbody_transformData_array;
        
        Rigidbody ragdollRigidbody_main;

		public Rigidbody MainRigidbody => ragdollRigidbody_main;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
        [ Button() ]
        public void SwitchRagdoll( bool on )
        {
			for( var i = 0; i < ragdoll_rigidbody_array.Length; i++ )
			{
				ragdoll_rigidbody_array [ i ].isKinematic = !on;
				ragdoll_rigidbody_array [ i ].useGravity  = on;
			}
        }

		[ Button() ]
		public void ToggleCollider( bool enable )
		{
			for( var i = 0; i < ragdoll_rigidbody_collider_array.Length; i++ )
			{
				ragdoll_rigidbody_collider_array[ i ].enabled = enable;
			}
		}
        
        [ Button() ]
		public void ApplyForce( Vector3 force, ForceMode mode )
		{
			ragdollRigidbody_main.AddForce( force, mode );
		}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
        [ Button( "Cache Rigidbodies" ) ]
        void CacheRigidbodies()
        {
            ragdoll_rigidbody_array = GetComponentsInChildren< Rigidbody >();
			CacheColliders();
		}

        [ Button( "Cache Rigidbodies On Children" ) ]
		void CacheRigidbodiesOnlyChildren()
		{
			ragdoll_rigidbody_array = GetComponentsInChildren< Rigidbody >();
			ragdoll_rigidbody_array = ragdoll_rigidbody_array.Skip( 1 ).Take( ragdoll_rigidbody_array.Length - 1 ).ToArray();

			CacheColliders();
		}

        void CacheColliders()
        {
			ragdoll_rigidbody_collider_array = new Collider[ ragdoll_rigidbody_array.Length ];

			for( var i = 0; i < ragdoll_rigidbody_array.Length; i++ )
				ragdoll_rigidbody_collider_array[ i ] = ragdoll_rigidbody_array[ i ].GetComponent< Collider >();
        }

        [ Button() ]
        void CacheTransformDatas()
        {
			ragdoll_rigidbody_transformData_array = new TransformData[ ragdoll_rigidbody_array.Length ];

            for( var i = 0; i < ragdoll_rigidbody_array.Length; i++ )
            {
				ragdoll_rigidbody_transformData_array[ i ] = new TransformData( ragdoll_rigidbody_array[ i ] );
			}
		}
#endif
#endregion
    }
}