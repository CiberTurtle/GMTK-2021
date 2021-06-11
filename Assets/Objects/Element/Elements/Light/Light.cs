using UnityEngine;

public class Light : Element
{
	public GameObject onState;
	public GameObject offState;

	protected override void OnPowerOn()
	{
		base.OnPowerOn();

		onState.SetActive(true);
		offState.SetActive(false);
	}

	protected override void OnPowerOff()
	{
		base.OnPowerOn();

		onState.SetActive(false);
		offState.SetActive(true);
	}
}