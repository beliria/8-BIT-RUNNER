using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Cantidad de fuerza añadida cuando el jugador salta.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Cantidad de velocidad aplicada al movimiento de agacharse. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // Parametro para hacer el movimiento mas limpio
	[SerializeField] private bool m_AirControl = false;                         // Si un jugador puede o no puede moverse mientras salta;
	[SerializeField] private LayerMask m_WhatIsGround;                          // Layer para dererminar que es el terreno
	[SerializeField] private Transform m_GroundCheck;                           // Posición que marca dónde comprobar si el jugador está en tierra.
	[SerializeField] private Transform m_CeilingCheck;                          // Una posición que marca dónde comprobar los techos
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // Collider que se desactivará al agacharse 
	[SerializeField] private float fuerza = 10f;								//Fuerza al recibir golpes
	[SerializeField]private AudioSource pasos;									//Sonido de lo pasos


	const float k_GroundedRadius = .2f;		// El radio para determinar si el jugador está conectado a tierra
	private bool m_Grounded;				// Booleano para saber si el jugador esta agachado o no
	const float k_CeilingRadius = .2f;		// El radio para determinar si el jugador puede ponerse de pie
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;		// Para saber hacia donde mira el jugador
	private Vector3 m_Velocity = Vector3.zero;
	public bool onLadder = false;			// Booleano para saber si el jugador esta en una escalera


	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	public void setOnLadder(bool mode)
	{
		this.onLadder = mode;
	}

	private void Awake()
	{
		m_Rigidbody2D	= GetComponent<Rigidbody2D>();
		pasos = GetComponent<AudioSource>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// El jugador está tocando suelo si su circulo golpea cualquier cosa designada como tierra.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

	public void Move(float move, float moveVertical, bool crouch, bool jump)
	{
		// Si está agachado, comprueba si el personaje puede ponerse de pie.
		if (!crouch && !onLadder)
		{
			// Si el personaje tiene un techo que le impide ponerse de pie, lo agachamos
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//Controlar si está en tierra o si el control aéreo está activado.
		if (m_Grounded || m_AirControl)
		{

			// Si se agacha
			if (crouch && !onLadder)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reducir la velocidad con el multiplicador de velocidad 
				move *= m_CrouchSpeed;

                // Deshabilita uno de los colisionadores cuando se agacha
                if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Deshabilita uno de los colisionadores cuando se agacha
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Mueve el personaje encontrando la velocidad del objetivo
			Vector3 targetVelocity;
			if (onLadder)
			{
				//Si esta en una escalera aplicamos otro tipo de fuerza
				targetVelocity = new Vector2(move * 10f, moveVertical * 10f);
			}
			else
			{
				targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

			}
			// Suavizar el movimiento del personaje
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// Si vamos a la derecha y el jugador está mirando a la izquierda...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// O la inversa
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// Si el jugador debe saltar
		if (m_Grounded && jump)
		{
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		
	}

	public void Hurt(float xEnemigo, float miX) {
		//empujar al personaje a un lado u otro dependiendo de su X y la de el enemigo que le ha golpeado
		if (xEnemigo > miX)
		{
			m_Rigidbody2D.velocity = new Vector2(-fuerza, m_Rigidbody2D.velocity.y);
		}
		else
		{
			m_Rigidbody2D.velocity = new Vector2(fuerza, m_Rigidbody2D.velocity.y);
		}

	}

	private void Footstep()
	{
		pasos.Play();
	}


	private void Flip()
	{
		//Le da la vuelta al jugador
		m_FacingRight = !m_FacingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}


}