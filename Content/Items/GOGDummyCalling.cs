using GloryofGuardian.Common;
using GloryofGuardian.Content.NPCs.Special;
using GloryofGuardian.Content.WeaponClasses;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Items {
    public class GOGDummyCalling : GOGCalling {
        public override string Texture => GOGConstant.Items + Name;

        protected override int ProjType => 1;

        protected override int ProjSlot => 0;

        protected override int ProjlNumLimit => -1;

        public override void SetProperty() {
            Item.damage = 0;
            Item.width = 38;
            Item.height = 58;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = -13;

            ProjOrNpc = false;
            NPCType = ModContent.NPCType<GOGDummy>();
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.TargetDummy, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
