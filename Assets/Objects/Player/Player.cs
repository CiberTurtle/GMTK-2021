using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Mover2D))]
public class Player : MonoBehaviour
{
	public float maxWireLength = 5.0f;
	public float wirePullbackStrength = 2.0f;

	[Space]

	public LayerMask connectionMask;

	[Space]

	public Transform inputWire;
	public Transform outputWire;

	Transform inputWireConnection;
	Transform outputWireConnection;
	Element outputElement;

	Controls controls;
	Vector2 mousePos;

	Mover2D mover;

	void Awake()
	{
		mover = GetComponent<Mover2D>();

		controls = new Controls();

		controls.Player.Move.performed += x => mover.v2MoveInput = x.ReadValue<Vector2>();
		controls.Player.Jump.performed += _ => mover.JumpDown();
		controls.Player.Jump.canceled += _ => mover.JumpUp();
		controls.Player.MousePos.performed += x => mousePos = Camera.main.ScreenToWorldPoint(x.ReadValue<Vector2>());
		controls.General.Restart.performed += x => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

		controls.Player.Connect.performed += _ =>
		{
			var connection = Physics2D.OverlapPoint(mousePos, connectionMask);

			if (!connection) return;

			if (connection.CompareTag("Output"))
			{
				if (connection.transform == inputWireConnection)
				{
					// Disconnect
					inputWireConnection = null;

					outputElement?.SetPower(false);
				}
				else
				{
					if (Util.DistSqr(transform.position, connection.transform.position) > maxWireLength * maxWireLength) return;

					// Connect
					inputWireConnection = connection.transform;

					outputElement?.SetPower(true);
				}
			}
			else
			{
				if (connection.transform == outputWireConnection)
				{
					// Disconnect
					outputWireConnection = null;
					outputElement?.SetPower(false);
					outputElement = null;
				}
				else
				{
					if (Util.DistSqr(transform.position, connection.transform.position) > maxWireLength * maxWireLength) return;

					// Connect
					outputWireConnection = connection.transform;

					outputElement?.SetPower(false);

					outputElement = outputWireConnection.GetComponent<Inlet>().master.GetComponent<Element>();

					if (inputWireConnection)
						outputElement?.SetPower(true);
				}
			}
		};
	}

	void OnEnable()
	{
		controls.Enable();
	}

	void Update()
	{
		if (inputWireConnection)
		{
			if (!inputWire.gameObject.activeInHierarchy) inputWire.gameObject.SetActive(true);
			inputWire.position = inputWireConnection.position;
		}
		else
		{
			if (inputWire.gameObject.activeInHierarchy) inputWire.gameObject.SetActive(false);
		}

		if (outputWireConnection)
		{
			if (!outputWire.gameObject.activeInHierarchy) outputWire.gameObject.SetActive(true);
			outputWire.position = outputWireConnection.position;
		}
		else
		{
			if (outputWire.gameObject.activeInHierarchy) outputWire.gameObject.SetActive(false);
		}
	}

	void FixedUpdate()
	{
		if (inputWireConnection)
		{
			var dist = Util.DistSqr(transform.position, inputWireConnection.position);
			mover.controller.rb.position += ((Vector2)inputWireConnection.position - (Vector2)transform.position).normalized * (Mathf.Clamp(dist - maxWireLength * maxWireLength, 0, 1) * wirePullbackStrength);
		}

		if (outputWireConnection)
		{
			var dist = Util.DistSqr(transform.position, outputWireConnection.position);
			mover.controller.rb.position += ((Vector2)outputWireConnection.position - (Vector2)transform.position).normalized * (Mathf.Clamp(dist - maxWireLength * maxWireLength, 0, 1) * wirePullbackStrength);
		}
	}

	void OnDisable()
	{
		controls.Disable();
	}
}
