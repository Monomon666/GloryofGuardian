using GloryofGuardian.Common;
using Terraria.GameContent.Creative;

namespace GloryofGuardian.Content.Items.Accessories
{
    public class ReverseHookArrow : GOGItem
    {
        public override string Texture => GOGConstant.Items + "Accessories/" + Name;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.maxStack = 1;
            Item.value = 10000;
            Item.rare = 1;

            Item.accessory = true;
        }

        int mode = 0;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GOGModPlayer>().reversehookarrow = true;

            base.UpdateAccessory(player, hideVisual);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostUpdate()
        {
        }
    }
}
