using GloryofGuardian.Common;
using GloryofGuardian.Content.Items.Armor;
using Terraria.ID;
using Terraria.Localization;

namespace GloryofGuardian.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class UndeadWoodHead : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 22;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = 0;

            Item.defense = 2;
        }

        public override void ArmorSetShadows(Player player) {
            //player.armorEffectDrawShadow = true;

            //player.armorEffectDrawShadowBasilisk = true;
            //player.armorEffectDrawShadowEOCShield = true;
            //player.armorEffectDrawShadowLokis = true;
            //player.armorEffectDrawShadowSubtle = true;
            //player.armorEffectDrawOutlines = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            if (body.type == ModContent.ItemType<UndeadWoodBody>()) {
                return legs.type == ModContent.ItemType<UndeadWoodLegs>();
            }
            return false;
        }

        public override void UpdateEquip(Player player) {
            player.GetModPlayer<GOGModPlayer>().Gslot += 1;
        }

        public override void UpdateArmorSet(Player player) {
            player.setBonus = Language.GetTextValue("Mods.GloryofGuardian.ArmorSetBonuses.UndeadWood.Description");
            player.GetModPlayer<GOGModPlayer>().UndeadWood = true;

            if (IsOnGround(player)) {
                if (player.velocity.Y == 0f) player.runAcceleration *= 0.4f;
                if (player.velocity.X == 0f) player.statDefense += 10;
                if (player.velocity.X == 0f) player.noKnockback = true;
            }
        }

        bool IsOnGround(Player player) {
            // 玩家垂直速度为 0（未跳跃或下落）
            if (player.velocity.Y != 0f) {
                return false;
            }

            // 计算玩家脚底的物块坐标
            int tileX = (int)(player.position.X + player.width / 2) / 16;
            int tileY = (int)(player.position.Y + player.height) / 16;

            // 获取脚底的物块
            Tile tile = Main.tile[tileX, tileY];

            // 检测脚底是否有固体物块
            return tile != null && tile.HasTile && Main.tileSolid[tile.TileType];
        }

        //public override Color? GetAlpha(Color lightColor) {
        //    return base.GetAlpha(lightColor);
        //}

        //public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        //    base.DrawArmorColor(drawPlayer, shadow, ref color, ref glowMask, ref glowMaskColor);
        //}

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalShard, 20)
                .AddIngredient(ItemID.SoulofNight, 12)
                .AddIngredient(ItemID.UnicornHorn, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
