using GloryofGuardian.Common;
using GloryofGuardian.Content.Classes;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace GloryofGuardian.Content.Items.Accessories {
    public class Shroom : GOGAccessories {
        public override string Texture => GOGConstant.Items + "Accessories/" + Name;
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() {
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 10000;
            Item.rare = -12;

            Item.accessory = true;
        }

        int mode = 0;
        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.GetCritChance<GenericDamageClass>() = 100;

            base.UpdateAccessory(player, hideVisual);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void PostUpdate() {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
        }
    }
}
