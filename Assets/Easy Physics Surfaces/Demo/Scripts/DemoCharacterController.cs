using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EasyPhysicsSurfaces
{
    [RequireComponent( typeof( Rigidbody ) )]
    [RequireComponent( typeof( AudioSource ) )]
    public class DemoCharacterController : MonoBehaviour
    {
    	[SerializeField] private float m_speed = 3;
    	[SerializeField] private float m_mouseSensitivity = 500;
    
    
        private Camera m_camera;
        private Rigidbody m_rigidbody;
        private AudioSource m_audioSource;
    
        private Vector2 m_input;
        private Vector2 m_mouseInput;
        private bool m_sprint;
        private float m_pitch;
        private float m_yaw;
        private float m_footstepDelay;
    
        void Start()
        {
            m_camera = GetComponentInChildren<Camera>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_audioSource = GetComponent<AudioSource>();
        }
    
        private void Update()
        {
            GetInput();
            ApplyRotation();
    
            UpdateSteps();
        }
    
    	private void FixedUpdate()
    	{
            ApplyMovement();
    	}
    
    
        private void GetInput()
    	{
            if( Input.GetMouseButtonDown( 0 ) )
                Cursor.lockState = CursorLockMode.Locked;

            if( Input.GetKeyDown( KeyCode.Escape ) )
                Cursor.lockState = CursorLockMode.None;

            if( Cursor.lockState != CursorLockMode.Locked )
			{
                m_input = Vector2.zero;
                m_mouseInput = Vector2.zero;
                return;
			}

            m_input.x = Input.GetAxis( "Horizontal" );
            m_input.y = Input.GetAxis( "Vertical" );
            m_input *= m_speed;
    
            m_mouseInput.x = Input.GetAxis( "Mouse X");
            m_mouseInput.y = Input.GetAxis( "Mouse Y");
            m_mouseInput *= m_mouseSensitivity;
    
            m_sprint = Input.GetKey( KeyCode.LeftShift );
            if( m_sprint )
                m_input *= 2.5f;
    
    	}
    
        private void ApplyRotation()
    	{
            // transform.eulerAngles += Vector3.up * m_mouseInput.x * Time.deltaTime;
    
            m_yaw += m_mouseInput.x * Time.deltaTime;
            m_pitch = Mathf.Clamp( m_pitch - m_mouseInput.y * Time.deltaTime, -89, 89 );
    
    		Vector3 newCameraRotation = new()
    		{
    			x = m_pitch,
                y = m_yaw,
                z = m_camera.transform.eulerAngles.z
    		};
    
    		m_camera.transform.eulerAngles = newCameraRotation;
    	}
    
    
        private void ApplyMovement()
    	{
            Vector3 forward = Vector3.ProjectOnPlane( m_camera.transform.forward, Vector3.up ).normalized;
            Vector3 right = Vector3.ProjectOnPlane( m_camera.transform.right, Vector3.up ).normalized;
    
            Vector3 newVelocity = forward * m_input.y + right * m_input.x;
            newVelocity.y = m_rigidbody.velocity.y;
    
            m_rigidbody.velocity = newVelocity;
    	}
    
    
        private void UpdateSteps()
    	{
            Vector3 horizonalVelocity = new( m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z );
    
            if( horizonalVelocity.magnitude > 0.1f )
    		{
                if( m_footstepDelay > 0 )
                    m_footstepDelay -= Time.deltaTime;
                else
    			{
                    float maxSpeed = m_speed * 2.5f; // speed in sprint
                    Footstep( horizonalVelocity.magnitude / maxSpeed );
    			}
    		}
    	}
    
    
        private void Footstep( float force )
    	{
            if( m_sprint )
                m_footstepDelay = 0.3f;
            else
                m_footstepDelay = 0.6f;
    
            if( Physics.Raycast( transform.position, Vector3.down, out RaycastHit hit, 2f ) )
    		{
                if( hit.collider.TryGetComponent( out PhysicsSurfaceData physicsSurfaceData ) )
                    m_audioSource.PlayOneShot( physicsSurfaceData.GetFootstepSound( force ) );
    		}
    	}
    }
}
