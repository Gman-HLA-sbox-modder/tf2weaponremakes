using Sandbox;
using System;

[Library( "tf_grenadelauncher", Title = "Grenade Launcher" )]
[Hammer.EditorModel( "weapons/rust_smg/rust_smg.vmdl" )]
partial class GrenadeLauncher: BaseDmWeapon
{ 
	public override string ViewModelPath => "models/weapons/v_models/v_grenadelauncher_demo.vmdl";

	public override float PrimaryRate => 1.0f;
	public override int ClipSize => 4;
	public override float ReloadTime => 0.5f;
	// reload time is 3.04
	public override int Bucket => 2;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/w_models/w_grenadelauncher.vmdl" );
		AmmoClip = 4;
	}

	public override void AttackPrimary()
	{
		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}

		TimeSincePrimaryAttack = 0.5f;
		TimeSinceSecondaryAttack = 0.5f;

		PlaySound( "grenade_launcher_shoot" );
		ShootGrenade();
	}

	private void ShootGrenade()
	{
		if (Host.IsClient)
			return;

		var grenade = new Prop
		{
			Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
			Rotation = Owner.EyeRot,
		};

		//TODO: Should be replaced with an actual grenade model
		grenade.SetModel("models/weapons/w_models/w_grenade_grenadelauncher.vmdl");
		grenade.Velocity = Owner.EyeRot.Forward * 1000;

		grenade.ExplodeAsync(3f);
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

	public override void Reload()
	{
		if (IsReloading)
			return;

		if (AmmoClip >= ClipSize)
			return;

		TimeSinceReload = 0;

		if (Owner is DeathmatchPlayer player)
		{
			if (player.AmmoCount(AmmoType) <= 0)
				return;

			StartReloadEffects();
		}

		IsReloading = true;

		(Owner as AnimEntity).SetAnimBool("b_reload", true);

		StartReloadEffects();
	}
}
