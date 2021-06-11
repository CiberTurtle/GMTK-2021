using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
	public static float DistSqr(Vector2 from, Vector2 to)
	{
		return (from - to).sqrMagnitude;
	}
}
