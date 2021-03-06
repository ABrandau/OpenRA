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
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Traits.Render;
using OpenRA.Traits;

namespace OpenRA.Mods.AS.Traits
{
	[Desc("Renders an animation when when the actor is leaving from a production building.")]
	public class WithExitOverlayInfo : ConditionalTraitInfo, Requires<RenderSpritesInfo>, Requires<BodyOrientationInfo>
	{
		[Desc("Sequence name to use")]
		[SequenceReference] public readonly string Sequence = "exit-overlay";

		[Desc("Position relative to body")]
		public readonly WVec Offset = WVec.Zero;

		[Desc("Custom palette name")]
		[PaletteReference("IsPlayerPalette")] public readonly string Palette = null;

		[Desc("Custom palette is a player palette BaseName")]
		public readonly bool IsPlayerPalette = false;

		public override object Create(ActorInitializer init) { return new WithExitOverlay(init.Self, this); }
	}

	public class WithExitOverlay : ConditionalTrait<WithExitOverlayInfo>, INotifyDamageStateChanged, INotifyProduction, ITick
	{
		readonly Animation overlay;
		bool enable;
		CPos exit;

		public WithExitOverlay(Actor self, WithExitOverlayInfo info)
			: base(info)
		{
			var rs = self.Trait<RenderSprites>();
			var body = self.Trait<BodyOrientation>();

			overlay = new Animation(self.World, rs.GetImage(self));
			overlay.PlayRepeating(info.Sequence);

			var anim = new AnimationWithOffset(overlay,
				() => body.LocalToWorld(info.Offset.Rotate(body.QuantizeOrientation(self, self.Orientation))),
				() => IsTraitDisabled || !enable);

			rs.Add(anim, info.Palette, info.IsPlayerPalette);
		}

		public void DamageStateChanged(Actor self, AttackInfo e)
		{
			overlay.ReplaceAnim(RenderSprites.NormalizeSequence(overlay, e.DamageState, overlay.CurrentSequence.Name));
		}

		public void UnitProduced(Actor self, Actor other, CPos exit)
		{
			this.exit = exit;
			enable = true;
		}

		void ITick.Tick(Actor self)
		{
			if (enable)
				enable = self.World.ActorMap.GetActorsAt(exit).Any(a => a != self);
		}
	}
}
