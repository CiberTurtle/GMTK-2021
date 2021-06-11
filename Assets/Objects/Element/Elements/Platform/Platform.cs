using UnityEngine;

public class Platform : Element
{
	public float speed;
	public float waitTime;

	[Space]

	public Transform platform;
	public Transform endPos;

	Vector2 startPos;
	Vector2 positionChange;

	bool goingBack;
	bool waiting;
	float timeWaiting;

	void Awake()
	{
		startPos = platform.position;
	}

	void FixedUpdate()
	{
		if (powered)
		{
			if (waiting)
			{
				if (waitTime > 0)
					timeWaiting += Time.fixedDeltaTime;

				if (timeWaiting > waitTime)
				{
					goingBack = !goingBack;
					waiting = false;
				}
			}
			else
			{
				if (goingBack)
				{
					positionChange = Vector2.MoveTowards(platform.position, startPos, speed * Time.fixedDeltaTime);
					platform.position = positionChange;

					if (Util.DistSqr(platform.position, startPos) < 0.05f)
					{
						timeWaiting = 0;
						waiting = true;
					}
				}
				else
				{
					positionChange = Vector2.MoveTowards(platform.position, endPos.position, speed * Time.fixedDeltaTime);
					platform.position = positionChange;

					if (Util.DistSqr(platform.position, endPos.position) < 0.05f)
					{
						timeWaiting = 0;
						waiting = true;
					}
				}
			}
		}
	}

	public void KeepOn(Rigidbody2D rb)
	{
		// TODO:
		// rb.position -= positionChange * Time.fixedDeltaTime;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);

#if UNITY_EDITOR
		Gizmos.DrawLine(platform.position, endPos.position);
#else
		Gizmos.DrawLine(startPos, endPos.position);
#endif
	}
}
