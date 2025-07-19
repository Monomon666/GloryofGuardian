using GloryofGuardian.Common;
using GloryofGuardian.Content.Items.Materials;
using GloryofGuardian.Content.NPCs.GuardNPCs;
using GloryofGuardian.Content.WeaponClasses;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapons.Calling2 {
    public class Titanium : GOGCalling {
        public override string Texture => GOGConstant.Weapons + Name;

        protected override int ProjType => 1;

        protected override int ProjSlot => 0;

        protected override int ProjlNumLimit => 1;

        public override void SetProperty() {
            Item.damage = 0;
            Item.width = 56;
            Item.height = 56;
            Item.knockBack = 1;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.LightPurple;

            //独一炮塔召唤速度减缓
            Item.useTime = 60;
            Item.useAnimation = 60;

            ProjOrNpc = false;
            NPCType = ModContent.NPCType<TitaniumDTN>();
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.TitaniumBar, 12);
            recipe.AddIngredient(ItemID.HallowedBar, 6);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ModContent.ItemType<SoulofAnotherWorld>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
