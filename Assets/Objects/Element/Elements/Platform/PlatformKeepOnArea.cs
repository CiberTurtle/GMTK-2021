using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformKeepOnArea : MonoBehaviour
{
	public Platform platform;

	void OnTriggerStay2D(Collider2D other)
	{
		platform.KeepOn(other.GetComponent<Rigidbody2D>());
	}
}
