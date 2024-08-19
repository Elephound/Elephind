using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EasyPhysicsSurfaces
{
	/// <summary>
	/// Contains surface audioclips and thresholds
	/// </summary>
#if UNITY_EDITOR
	[CreateAssetMenu( fileName = "physic_surface", menuName = "Physics/Surface" )]
#endif
	public class PhysicsSurface : ScriptableObject
	{
		public float HeavyStepThreshold = 0.7f;
	    public AudioClip[] Footsteps;
	    public AudioClip[] HeavyFootsteps;
	
	    public AudioClip GetFootstepSound( float strength = 0.5f )
		{
			if( strength < HeavyStepThreshold )
				return Footsteps.GetRandom();
			else if( HeavyFootsteps.Length > 0 )
				return HeavyFootsteps.GetRandom();
			else
				return Footsteps.GetRandom();
		}
	
#if UNITY_EDITOR
	
	[CustomEditor( typeof( PhysicsSurface ) )]
		public class PhysicsSurfaceEditor : Editor
		{
			SerializedProperty HeavyStepThreshold;
			SerializedProperty Footsteps;
			SerializedProperty HeavyFootsteps;
	
			private void OnEnable()
			{
				HeavyStepThreshold = serializedObject.FindProperty( "HeavyStepThreshold" );
				Footsteps = serializedObject.FindProperty( "Footsteps" );
				HeavyFootsteps = serializedObject.FindProperty( "HeavyFootsteps" );
			}
	
			public override void OnInspectorGUI()
			{
			    serializedObject.Update();
	
				GUILayout.BeginVertical( "Footsteps", "window" );
				EditorGUILayout.PropertyField( HeavyStepThreshold );
				EditorGUILayout.PropertyField( Footsteps );
				EditorGUILayout.PropertyField( HeavyFootsteps );
				GUILayout.EndVertical();
	
				GUILayout.Space( 10 );
				GUILayout.BeginVertical( "Impacts", "window" );
				GUILayout.Label( "Impacts available only in Pro version" );
				GUILayout.EndVertical();
	
				serializedObject.ApplyModifiedProperties();
			}
		}
	
#endif
	}
}