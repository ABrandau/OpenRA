#region Copyright & License Information
/*
 * Copyright 2015- OpenRA.Mods.AS Developers (see AUTHORS)
 * This file is a part of a third-party plugin for OpenRA, which is
 * free software. It is made available to you under the terms of the
 * GNU General Public License as published by the Free Software
 * Foundation. For more information, see COPYING.
 */
#endregion

using OpenRA.Mods.Common.Traits;
using OpenRA.Primitives;
using OpenRA.Traits;

namespace OpenRA.Mods.AS.Traits
{
	[Desc("Spawns survivors when an actor is destroyed.")]
	public class SpawnSurvivorsInfo : ConditionalTraitInfo
	{
		[ActorReference, FieldLoader.Require]
		[Desc("The actors spawned.")]
		public readonly string[] Actors = { };

		[Desc("DeathType(s) that trigger spawning. Leave empty to always spawn.")]
		public readonly BitSet<DamageType> DeathTypes = default(BitSet<DamageType>);

		public override object Create(ActorInitializer actor) { return new SpawnSurvivors(this); }
	}

	public class SpawnSurvivors : ConditionalTrait<SpawnSurvivorsInfo>, INotifyKilled
	{
		public SpawnSurvivors(SpawnSurvivorsInfo info)
			: base(info) { }

		void INotifyKilled.Killed(Actor self, AttackInfo attack)
		{
			if (IsTraitDisabled)
				return;

			if (!Info.DeathTypes.IsEmpty && !attack.Damage.DamageTypes.Overlaps(Info.DeathTypes))
				return;

			foreach (var actorType in Info.Actors)
			{
				var td = new TypeDictionary();

				td.Add(new OwnerInit(self.Owner));
				td.Add(new LocationInit(self.World.Map.CellContaining(self.CenterPosition)));

				var unit = self.World.CreateActor(false, actorType.ToLowerInvariant(), td);
				self.World.AddFrameEndTask(w =>
				{
					w.Add(unit);
					var mobile = unit.TraitOrDefault<Mobile>();
					if (mobile != null)
						mobile.Nudge(unit, unit, true);
				});
			}
		}
	}
}
