#region Copyright & License Information
/*
 * Copyright 2015- OpenRA.Mods.AS Developers (see AUTHORS)
 * This file is a part of a third-party plugin for OpenRA, which is
 * free software. It is made available to you under the terms of the
 * GNU General Public License as published by the Free Software
 * Foundation. For more information, see COPYING.
 */
#endregion

using OpenRA.Mods.Common;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.AS.Traits
{
	[Desc("Turns actor randomly when idle.")]
	class TurnOnIdleInfo : ConditionalTraitInfo, Requires<MobileInfo>
	{
		public readonly int MinDelay = 15;
		public readonly int MaxDelay = 35;
		public override object Create(ActorInitializer init) { return new TurnOnIdle(init, this); }
	}

	class TurnOnIdle : ConditionalTrait<TurnOnIdleInfo>, INotifyIdle
	{
		int currDelay;
		int turnTicks;
		int targetFacing;
		Mobile mobile;

		public TurnOnIdle(ActorInitializer init, TurnOnIdleInfo info) : base(info)
		{
			currDelay = init.World.SharedRandom.Next(Info.MinDelay, Info.MaxDelay);
			mobile = init.Self.Trait<Mobile>();
			targetFacing = mobile.Facing;
		}

		void INotifyIdle.TickIdle(Actor self)
		{
			if (IsTraitDisabled)
				return;

			if (--currDelay > 0)
				return;

			if (--turnTicks >= 0)
			{
				mobile.Facing = Util.TickFacing(mobile.Facing, targetFacing, mobile.TurnSpeed);
				return;
			}

			if (turnTicks == -1)
			{
				targetFacing = self.World.SharedRandom.Next(256);
				turnTicks = 1 + System.Math.Abs(mobile.Facing - targetFacing) / mobile.TurnSpeed;
			}

			currDelay = self.World.SharedRandom.Next(Info.MinDelay, Info.MinDelay);
		}
	}
}