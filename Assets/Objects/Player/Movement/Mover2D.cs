using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Controller2D))]
public class Mover2D : MonoBehaviour
{
	// Parameters
	[Header("Jump")]
	public float fMaxJumpHeight = 1.25f;
	public float fMinJumpHeight = 1f;
	public float fTimeToJumpApex = 0.275f;
	[Header("Move")]
	public float fMoveSpeed = 4f;
	public float fAccelerationTimeGrounded = 0.05f;
	public float fAccelerationTimeAirborne = 0.4f;
	[Header("Input")]
	public float fJumpRem = 0.1f;
	public float fGroundedRem = 0.1f;
	[Header("Wall Jumping")]
	public float fWallSlideSpeedMax = 3f;
	public float fWallStickTime = 0.25f;
	[Space]
	public Vector2 v2WallJumpClimb;
	public Vector2 v2WallJumpOff;
	public Vector2 v2WallLeap;
	[Header("Other")]
	public UnityEvent onJump = new UnityEvent();
	public UnityEvent onLand = new UnityEvent();

	// Data
	[HideInInspector] public bool bIsControlable = true;
	[HideInInspector] public Vector2 v2Velocity;
	float fGravity;
	float fMaxJumpVelocity;
	float fMinJumpVelocity;
	float fVelocityXSmoothing;
	int iWallDirX;
	float fTimeToWallUnstick;
	bool bWallSliding;
	bool sendOnJump;
	bool sendOnLand;

	// Input
	[HideInInspector] public Vector2 v2MoveInput;
	[HideInInspector] public bool bJumpDown;
	[HideInInspector] public bool bJumpUp;
	Vector2 v2ActualInput;
	float fJumpMem;
	float fGroundedMem;
	public bool bIsGrounded { get => fGroundedMem > 0; }

	// Cache
	Controller2D controller;

	void Start()
	{
		controller = GetComponent<Controller2D>();

		fGravity = -(2 * fMaxJumpHeight) / Mathf.Pow(fTimeToJumpApex, 2);
		fMaxJumpVelocity = Mathf.Abs(fGravity) * fTimeToJumpApex;
		fMinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(fGravity) * fMinJumpHeight);
	}

	void Update()
	{
		if (sendOnJump) onJump.Invoke();
		else if (sendOnLand) onLand.Invoke();
		sendOnJump = false;
		sendOnLand = false;
	}

	void FixedUpdate()
	{
		v2ActualInput = v2MoveInput;

		CalculateVelocity();
		// HandleWallSliding();

		fGroundedMem -= Time.fixedDeltaTime;
		if (controller.collisions.below)
		{
			fGroundedMem = fGroundedRem;
		}

		fJumpMem -= Time.fixedDeltaTime;
		if (bJumpDown)
		{
			fJumpMem = fJumpRem;
		}

		if (fJumpMem > 0)
		{
			fJumpMem = -1;
			if (bWallSliding)
			{
				if (iWallDirX == v2ActualInput.x)
				{
					v2Velocity.x = -iWallDirX * v2WallJumpClimb.x;
					v2Velocity.y = v2WallJumpClimb.y;
				}
				else if (v2ActualInput.x == 0)
				{
					v2Velocity.x = -iWallDirX * v2WallJumpOff.x;
					v2Velocity.y = v2WallJumpOff.y;
				}
				else
				{
					v2Velocity.x = -iWallDirX * v2WallLeap.x;
					v2Velocity.y = v2WallLeap.y;
				}
			}
			if (fGroundedMem > 0)
			{
				fGroundedMem = -1;
				if (controller.collisions.slidingDownMaxSlope)
				{
					if (v2ActualInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
					{ // not jumping against max slope
						v2Velocity.y = fMaxJumpVelocity * controller.collisions.slopeNormal.y;
						v2Velocity.x = fMaxJumpVelocity * controller.collisions.slopeNormal.x;
					}
				}
				else
				{
					v2Velocity.y = fMaxJumpVelocity;
				}

				sendOnJump = true;
			}
		}

		if (bJumpUp)
		{
			if (v2Velocity.y > fMinJumpVelocity)
			{
				// fJumpMem = -1;
				v2Velocity.y = fMinJumpVelocity;
			}
		}
		bJumpUp = false;
		bJumpDown = false;

		controller.Move(v2Velocity * Time.fixedDeltaTime, v2ActualInput);

		if (controller.collisions.above || controller.collisions.below)
		{
			if (v2Velocity.y < -1f)
			{
				sendOnLand = true;
			}

			if (controller.collisions.slidingDownMaxSlope)
			{
				v2Velocity.y += controller.collisions.slopeNormal.y * -fGravity * Time.fixedDeltaTime;
			}
			else
			{
				v2Velocity.y = 0;
			}
		}
	}

	public void JumpDown()
	{
		bJumpDown = true;
		bJumpUp = false;
	}

	public void JumpUp()
	{
		bJumpDown = false;
		bJumpUp = true;
	}

	void HandleWallSliding()
	{
		iWallDirX = (controller.collisions.left) ? -1 : 1;
		bWallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && v2Velocity.y < 0)
		{
			bWallSliding = true;

			if (v2Velocity.y < -fWallSlideSpeedMax)
			{
				v2Velocity.y = -fWallSlideSpeedMax;
			}

			if (fTimeToWallUnstick > 0)
			{
				fVelocityXSmoothing = 0;
				v2Velocity.x = 0;

				if (v2ActualInput.x != iWallDirX && v2ActualInput.x != 0)
				{
					fTimeToWallUnstick -= Time.fixedDeltaTime;
				}
				else
				{
					fTimeToWallUnstick = fWallStickTime;
				}
			}
			else
			{
				fTimeToWallUnstick = fWallStickTime;
			}
		}
	}

	void CalculateVelocity()
	{
		float targetVelocityX = v2ActualInput.x * fMoveSpeed;
		v2Velocity.x = Mathf.SmoothDamp(v2Velocity.x, targetVelocityX, ref fVelocityXSmoothing, (controller.collisions.below) ? fAccelerationTimeGrounded : fAccelerationTimeAirborne);
		v2Velocity.y += fGravity * Time.fixedDeltaTime;
	}
}
