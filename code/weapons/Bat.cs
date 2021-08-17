using Sandbox;


[Library( "tf_bat", Title = "Bat" )]
[Hammer.EditorModel("models/weapons/c_models/c_bat.vmdl")]
partial class Bat : BaseDmWeapon
{
	public override string ViewModelPath => "models/weapons/v_models/v_bat_scout.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 0.0f;

	public override int Bucket => 0;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/c_models/c_bat.vmdl" );
		AmmoClip = 1;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		if ( MeleeAttack() )
		{
			OnMeleeHit();
		}
		else
		{
			OnMeleeMiss();
		}

		PlaySound( "swing_crowbar" );

		//Make anims play
		
		(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

		ViewModelEntity?.SetAnimBool( "fire", true );

		TimeSincePrimaryAttack = 2.5f;
	}

	public override bool CanReload()
	{
		return false;
	}

	private bool MeleeAttack()
	{
		var forward = Owner.EyeRot.Forward;
		forward = forward.Normal;

		bool hit = false;

		foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * 80, 20.0f ) )
		{
			if ( !tr.Entity.IsValid() ) continue;

			tr.Surface.DoBulletImpact( tr );

			hit = true;

			if ( !IsServer ) continue;

			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100, 25 )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}

		return hit;
	}

	[ClientRpc]
	private void OnMeleeMiss()
	{
		Host.AssertClient();

		if ( IsLocalPawn )
		{
			_ = new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimBool( "attack", true );
	}

	[ClientRpc]
	private void OnMeleeHit()
	{
		Host.AssertClient();

		if ( IsLocalPawn )
		{
			_ = new Sandbox.ScreenShake.Perlin( 1.0f, 1.0f, 3.0f );
		}

		ViewModelEntity?.SetAnimBool( "attack_hit", true );

		PlaySound("bat_hit");
	}
	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		TimeSinceDeployed = 0;

		IsReloading = false;
		
		PlaySound( "bat_draw" );
	}
}
