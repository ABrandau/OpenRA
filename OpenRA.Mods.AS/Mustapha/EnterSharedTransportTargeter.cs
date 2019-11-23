using System;
using OpenRA.Mods.AS.Traits;
using OpenRA.Mods.Common.Orders;
using OpenRA.Traits;

namespace OpenRA.Mods.AS.Orders
{
	public class EnterSharedTransportTargeter : EnterAlliedActorTargeter<SharedCargoInfo>
	{
		public EnterSharedTransportTargeter(string order, int priority,
            Func<Actor, TargetModifiers, bool> canTarget, Func<Actor, bool> useEnterCursor)
            : base(order, priority, canTarget, useEnterCursor) { }

		public override bool CanTargetActor(Actor self, Actor target, TargetModifiers modifiers, ref string cursor)
		{
			return base.CanTargetActor(self, target, modifiers, ref cursor);
		}
	}
}
