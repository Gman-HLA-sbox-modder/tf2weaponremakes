using Sandbox;
using System;

[Library("tf_healthpacklarge", Title = "Health Pack Large")]
[Hammer.EditorModel("models/items/ammopack_large.vmdl")]
partial class HealthLarge : BaseDmWeapon
{
	
	public override void Spawn()
	{
		base.Spawn();

		SetModel("models/items/medkit_large.vmdl");
	}

	public bool IsUsable(Entity user)
	{
		return true;
	}

	public override void OnCarryStart(Entity carrier)
	{
		base.OnCarryStart(carrier);

		if (PickupTrigger.IsValid())
		{
			PickupTrigger.EnableTouch = false;
		}
	}

	public void Heal()
	{
		var MaxHealth = 125;
		if (Owner.Health < MaxHealth)
		{
			Owner.Health += 40;
			Owner.Health = Math.Clamp(Owner.Health, 0, MaxHealth);
		}
	}
}
