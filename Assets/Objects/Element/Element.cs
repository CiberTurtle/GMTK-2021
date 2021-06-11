using NaughtyAttributes;
using UnityEngine;

public class Element : MonoBehaviour
{
	[ReadOnly] public bool powered = false;

	public void SetPower(bool state)
	{
		if (state == powered) return;

		powered = state;

		if (powered)
			OnPowerOn();
		else
			OnPowerOff();
	}

	protected virtual void OnPowerOn() { }

	protected virtual void OnPowerOff() { }
}
