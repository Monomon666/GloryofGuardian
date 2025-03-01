using GloryofGuardian.Common;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class NanoWings : GOGItem
    {
        public override string Texture => GOGConstant.Items + "Accessories/" + Name;
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // 旅行模式下研究数目
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(1800, 8, 4.5f, true);
        }

        public override void SetDefaults() {
            Item.width = 36;
            Item.height = 28;
            Item.value = 10000;
            Item.rare = -12;
            Item.accessory = true;
            Item.scale *= 1.5f;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            Lighting.AddLight(player.Center, 0.5f, 2.25f, 2.25f);
            player.wingTime = player.wingTimeMax;//无限飞

            if (player.controlDown && player.controlJump && !player.mount.Active) {
                player.position.Y -= player.velocity.Y;
                if (player.velocity.Y > 0.1f)
                    player.velocity.Y = 0.1f;
                else if (player.velocity.Y < 0.1f)
                    player.velocity.Y = -0.1f;

                player.maxRunSpeed *= 5f;
                player.runAcceleration *= 2.5f;
            }

            int downrushswitch = 0;
            int downdistence = 0;

            if (player.controlDown && !player.controlJump && !player.mount.Active) {
                int downdash = 8;
                downrushswitch = 1;
                downdistence = 60;
                downdistence--;
                if (downrushswitch == 1 && player.controlDown && !player.controlJump && !player.mount.Active) {
                    if (downdash <= 48) downdash++;
                    Vector2 pos = player.BottomLeft;
                    Vector2 tilePos = GOGUtils.WEPosToTilePos(pos + new Vector2(0, 8));
                    if (!TileHelper.GetTile(tilePos).HasSolidTile()) player.position += new Vector2(0, downdash);
                } else downdash = 8;
                if (!player.controlDown && downdistence < 0) downrushswitch = 0;
            }
        }

        //饰品作用
        public override void UpdateEquip(Player player) {
        }

        //飞行相关参数
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
            ascentWhenFalling = 1.2f;
            ascentWhenRising = 1.8f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 4.5f;
            constantAscend = 0.135f;
        }

        public override bool WingUpdate(Player player, bool inUse) {
            return base.WingUpdate(player, inUse);
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration) {

        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.DirtBlock, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

