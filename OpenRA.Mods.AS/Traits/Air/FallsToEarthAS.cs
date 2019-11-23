#region Copyright & License Information
/*
 * Copyright 2015- OpenRA.Mods.AS Developers (see AUTHORS)
 * This file is a part of a third-party plugin for OpenRA, which is
 * free software. It is made available to you under the terms of the
 * GNU General Public License as published by the Free Software
 * Foundation. For more information, see COPYING.
 */
#endregion

using OpenRA.GameRules;
using OpenRA.Mods.AS.Activities;
using OpenRA.Mods.Common;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.AS.Traits
{
	[Desc("Causes aircraft husks that are spawned in the air to crash to the ground.")]
	public class FallsToEarthASInfo : ITraitInfo, IRulesetLoaded, Requires<AircraftInfo>
	{
		[WeaponReference]
		public readonly string Explosion = "UnitExplode";

		public readonly bool Spins = true;
		public readonly int SpinInitial = 10;
		public readonly int SpinAcceleration = 0;
		public readonly int SpinAccelerationDelay = 1;
		public readonly bool Moves = false;
		public readonly WDist Velocity = new WDist(0);
		public readonly WDist VelocityAcceleration = new WDist(22);
		public readonly int VelocityAccelerationDelay = 1;

		public WeaponInfo ExplosionWeapon { get; private set; }

		public object Create(ActorInitializer init) { return new FallsToEarthAS(init, this); }
		public void RulesetLoaded(Ruleset rules, ActorInfo ai)
		{
			ExplosionWeapon = string.IsNullOrEmpty(Explosion) ? null : rules.Weapons[Explosion.ToLowerInvariant()];
		}
	}

	public class FallsToEarthAS : IEffectiveOwner, INotifyCreated
	{
		readonly FallsToEarthASInfo info;
		readonly Player effectiveOwner;

		public FallsToEarthAS(ActorInitializer init, FallsToEarthASInfo info)
		{
			this.info = info;
			effectiveOwner = init.Contains<EffectiveOwnerInit>() ? init.Get<EffectiveOwnerInit, Player>() : init.Self.Owner;
		}

		// We return init.Self.Owner if there's no effective owner
		bool IEffectiveOwner.Disguised { get { return true; } }
		Player IEffectiveOwner.Owner { get { return effectiveOwner; } }

		void INotifyCreated.Created(Actor self)
		{
			self.QueueActivity(false, new FallToEarthAS(self, info));
		}
	}
}
