using Sandbox;
using System;

[Library("tf_ammopacklarge", Title = "Ammo Pack Large")]
[Hammer.EditorModel("models/items/ammopack_large.vmdl")]
partial class AmmoLarge : BaseDmWeapon
{
	
	public override void Spawn()
	{
		base.Spawn();

		SetModel("models/items/ammopack_large.vmdl");
	}

	public bool IsUsable(Entity user)
	{
		return false;
	}

	public override void OnCarryStart(Entity carrier)
	{
		base.OnCarryStart(carrier);

		if (PickupTrigger.IsValid())
		{
			PickupTrigger.EnableTouch = false;
		}
	}
}
