using GloryofGuardian.Common;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Accessories
{
    public class HolyBond : GOGItem
    {
        public override string Texture => GOGConstant.Items + "Accessories/" + Name;
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() {
            Item.width = 48;
            Item.height = 48;
            Item.maxStack = 1;
            Item.value = 10000;
            Item.rare = ItemRarityID.Lime;

            Item.accessory = true;
        }

        int mode = 0;
        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.GetModPlayer<GOGModPlayer>().wildernessseed = true;
            player.GetDamage<GuardianDamageClass>() += 0f;
            player.GetCritChance<GenericDamageClass>() += 0;

            base.UpdateAccessory(player, hideVisual);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void PostUpdate() {
        }
    }
}
