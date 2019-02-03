#region Copyright & License Information
/*
 * Copyright 2015- OpenRA.Mods.AS Developers (see AUTHORS)
 * This file is a part of a third-party plugin for OpenRA, which is
 * free software. It is made available to you under the terms of the
 * GNU General Public License as published by the Free Software
 * Foundation. For more information, see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Primitives;
using OpenRA.Traits;

namespace OpenRA.Mods.AS.Traits
{
	[Desc("Modifies the production time of this actor for a specific queue or when a prerequisite is granted.")]
	public class ProductionTimeMultiplierInfo : TraitInfo<ProductionTimeMultiplier>, IProductionTimeModifierInfo
	{
		[Desc("Percentage modifier to apply.")]
		public readonly int Multiplier = 100;

		[Desc("After applying the multiplier, also add this to the build time.")]
		public readonly int ExtraBuildTime = 0;

		[Desc("Only apply this time change if owner has these prerequisites.")]
		public readonly string[] Prerequisites = { };

		[Desc("Queues that this time will apply.")]
		public readonly HashSet<string> Queue = new HashSet<string>();

		Pair<int, int> IProductionTimeModifierInfo.GetProductionTimeModifier(TechTree techTree, string queue)
		{
			if ((!Queue.Any() || Queue.Contains(queue)) && (!Prerequisites.Any() || techTree.HasPrerequisites(Prerequisites)))
				return new Pair<int, int>(Multiplier, ExtraBuildTime);

			return new Pair<int, int>(100, 0);
		}
	}

	public class ProductionTimeMultiplier { }
}
