using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using GloryofGuardian.Content.WeaponClasses;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapons.Calling {
    public class Plantera : GOGCalling {
        public override string Texture => GOGConstant.Weapons + Name;

        protected override int ProjType => ModContent.ProjectileType<PlanteraDT>();

        protected override int ProjSlot => 1;

        protected override int ProjlNumLimit => -1;

        public override void SetProperty() {
            Item.damage = 15;
            Item.width = 56;
            Item.height = 56;
            Item.knockBack = 8;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
