using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using Terraria.GameContent.Creative;

namespace DreamJourney.Content.Items.Accessories
{
    public class ResidualLonelinessEchoes : GOGItem
    {
        public override string Texture => GOGConstant.Items + "Accessories/" + Name;
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() {
            Item.width = 32;
            Item.height = 44;
            Item.maxStack = 999;
            Item.value = 10000;
            Item.rare = -12;

            Item.accessory = true;

            Item.defense = 5;
        }

        int mode = 0;
        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.GetModPlayer<GOGModPlayer>().residuallonelinessechoes = true;
            player.GetDamage<GuardianDamageClass>() += 1f;

            player.GetCritChance<GenericDamageClass>() += 0;

            base.UpdateAccessory(player, hideVisual);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) {
            if (equippedItem == null) return false;
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void PostUpdate() {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
        }
    }
}
