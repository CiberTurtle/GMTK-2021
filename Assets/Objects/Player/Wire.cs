using UnityEngine;

public class Wire : MonoBehaviour
{
	public AnimationCurve widthCurve;

	Player player;
	LineRenderer lineRenderer;

	void Awake()
	{
		player = transform.parent.GetComponent<Player>();
		lineRenderer = GetComponent<LineRenderer>();
	}

	void Update()
	{
		lineRenderer.SetPositions(new Vector3[] { transform.position, transform.parent.position });

		var width = widthCurve.Evaluate(Util.DistSqr(player.transform.position, transform.position) / (player.maxWireLength * player.maxWireLength));

		lineRenderer.startWidth = width;
		lineRenderer.endWidth = width;
	}
}
