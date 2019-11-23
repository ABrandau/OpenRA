#region Copyright & License Information
/*
 * Copyright 2015- OpenRA.Mods.AS Developers (see AUTHORS)
 * This file is a part of a third-party plugin for OpenRA, which is
 * free software. It is made available to you under the terms of the
 * GNU General Public License as published by the Free Software
 * Foundation. For more information, see COPYING.
 */
#endregion

using System.Linq;
using OpenRA.Activities;
using OpenRA.Mods.AS.Traits;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.AS.Activities
{
	public class FallToEarthAS : Activity
	{
		readonly Aircraft aircraft;
		readonly FallsToEarthASInfo info;
		int spinAcceleration = 0;
		int spin = 0;
		int spinAccelerationDelay;
		int velocityAccelerationDelay;
		WDist velocity;

		public FallToEarthAS(Actor self, FallsToEarthASInfo info)
		{
			this.info = info;
			aircraft = self.Trait<Aircraft>();
			velocity = info.Velocity;
			velocityAccelerationDelay = info.VelocityAccelerationDelay;

			if (info.Spins)
			{
				spin = info.SpinInitial;
				spinAcceleration = info.SpinAcceleration;
				spinAccelerationDelay = info.SpinAccelerationDelay;
			}
		}

		public override bool Tick(Actor self)
		{
			if (self.World.Map.DistanceAboveTerrain(self.CenterPosition).Length <= 0)
			{
				if (info.ExplosionWeapon != null)
				{
					// Use .FromPos since this actor is killed. Cannot use Target.FromActor
					info.ExplosionWeapon.Impact(Target.FromPos(self.CenterPosition), self, Enumerable.Empty<int>());
				}

				self.Dispose();
				return true;
			}

			if (info.Spins)
			{
				if (--spinAccelerationDelay <= 0)
				{
					spin += spinAcceleration;
					spinAccelerationDelay = info.SpinAccelerationDelay;
				}

				aircraft.Facing = (aircraft.Facing + spin) % 256;
			}

			var move = info.Moves ? aircraft.FlyStep(aircraft.Facing) : WVec.Zero;

			if (--velocityAccelerationDelay <= 0)
			{
				velocity += info.VelocityAcceleration;
				velocityAccelerationDelay = info.VelocityAccelerationDelay;
			}

			move -= new WVec(WDist.Zero, WDist.Zero, velocity);
			aircraft.SetPosition(self, aircraft.CenterPosition + move);

			return false;
		}
	}
}
