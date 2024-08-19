using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EasyPhysicsSurfaces
{
    public class PhysicsSurfaceData : MonoBehaviour
    {
        [SerializeField] private PhysicsSurface m_surface;


        /// <summary>
        /// Return needed audioclip in depending on the strength
        /// </summary>
        /// <param name="strength"></param>
        /// <returns></returns>
		public AudioClip GetFootstepSound( float strength = 0.5f ) => m_surface.GetFootstepSound( strength );
    }
}