using Sandbox;
using System;

[Library("tf_pistol", Title = "Pistol")]
[Hammer.EditorModel("models/weapons/w_models/w_pistol.vmdl")]
partial class Pistol : BaseDmWeapon
{
	public override string ViewModelPath => "models/weapons/v_models/v_pistol_scout.vmdl";

	public override float PrimaryRate => 7.5f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 12 ;
	public override float ReloadTime => 1.0f;
	public override int Bucket => 2;

	public override void Spawn()
	{
		base.Spawn();

		SetModel("models/weapons/w_models/w_pistol.vmdl");
		AmmoClip = 12;
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if (!TakeAmmo(1))
		{
			DryFire();
			return;
		}

		(Owner as AnimEntity).SetAnimBool("b_attack", true);

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound("pistol_shoot");

		//
		// Shoot the bullets
		//
		ShootBullet(0.1f, 1.5f, 5.0f, 3.0f);

	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
		Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

		if (Owner == Local.Pawn)
		{
			new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
		}

		ViewModelEntity?.SetAnimBool("fire", true);
		CrosshairPanel?.CreateEvent("fire");
	}

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 2); // TODO this is shit
		anim.SetParam("aimat_weight", 1.0f);
	}

	public override void Reload()
	{
		PlaySound( "pistol_reload_scout" );

		if ( IsReloading )
			return;

		if ( AmmoClip >= ClipSize )
			return;

		TimeSinceReload = 0;

		if ( Owner is DeathmatchPlayer player )
		{
			if ( player.AmmoCount( AmmoType ) <= 0 )
				return;

			StartReloadEffects();
		}

		IsReloading = true;

		(Owner as AnimEntity).SetAnimBool( "b_reload", true );

		StartReloadEffects();
	}
}
