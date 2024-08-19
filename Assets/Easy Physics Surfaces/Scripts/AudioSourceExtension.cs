using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EasyPhysicsSurfaces
{
	/// <summary>
	/// Make use AudioSource easier
	/// </summary>
	public static class AudioSourceExtension
	{
		public static AudioClip GetRandom( this AudioClip[] clips )
		{
			if( clips == null ) return null;
			if( clips.Length == 0 ) return null;
	
			return clips[ Random.Range( 0, clips.Length ) ];
		}
	
		public static AudioClip GetRandom( this List<AudioClip> clips )
		{
			if( clips == null ) return null;
			if( clips.Count == 0 ) return null;
			
			return clips[ Random.Range( 0, clips.Count ) ];
		}
		

		/// <summary>
		/// Play one shot random sound from array
		/// </summary>
	    public static void PlayOneShot( this AudioSource audioSource, AudioClip[] clips )
		{
			if( !audioSource )
				return;
	
			AudioClip clip = clips.GetRandom();
	
			if( !clip )
				return;
	
			audioSource.PlayOneShot( clip );
		}
	
		
		/// <summary>
		/// Play one shot clip if it's not null
		/// </summary>
		public static void PlayAtPosition( AudioClip clip, Vector3 position, float volume = 1 )
		{
			if( !clip )
				return;
	
			AudioSource.PlayClipAtPoint( clip, position, volume );
		}
	
	
		/// <summary>
		/// Play one shot clip from array if it's not null
		/// </summary>
		public static void PlayAtPosition( AudioClip[] clips, Vector3 position, float volume = 1 )
		{
			AudioClip clip = clips.GetRandom();
	
			if( !clip )
				return;
	
			AudioSource.PlayClipAtPoint( clip, position, volume );
		}
	}
}