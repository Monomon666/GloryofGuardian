using GloryofGuardian.Common;
using GloryofGuardian.Content.Items.Materials;
using GloryofGuardian.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class OrichalcumCalling : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + Name;

        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 0;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<OrichalcumDT>();
            Item.shootSpeed = 5f;

            Item.channel = true;
            Item.noUseGraphic = false;
            Item.noMelee = true;
            Item.mana = 0;
            Item.scale = 1f;

            Item.staff[Item.type] = true;
        }

        public override bool AltFunctionUse(Player player) {
            return true;
        }

        public override bool CanUseItem(Player player) {
            if (player.altFunctionUse == 0) {
                if (player.GetModPlayer<GOGModPlayer>().Gslot == 0) {
                    CombatText.NewText(player.Hitbox,
                            Color.Red,
                            "戍卫栏不足",
                            false,
                            true
                            );
                    return false;
                }
            }

            Item.noUseGraphic = false;

            if (player.altFunctionUse == 0) {
                Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
                int wid = 3;
                int hig = 5;
                Vector2 offset = new Vector2(wid, hig) / -2 * 16;
                Vector2 mouPos = Main.MouseWorld + offset;
                for (int y = 0; y < hig; y++) {
                    for (int x = 0; x < wid; x++) {
                        Tile tile = TileHelper.GetTile(GOGUtils.WEPosToTilePos(mouPos + new Vector2(x, y) * 16));
                        if (tile.HasSolidTile()) {
                            return false;
                        }
                    }
                }
            }

            if (player.altFunctionUse == 2) {
                Item.UseSound = null;
                Item.noUseGraphic = true;
                for (int i = 0; i < Main.maxProjectiles; i++) {
                    Projectile proj = Main.projectile[i];
                    if (proj.type == ModContent.ProjectileType<OrichalcumDT>() && proj.owner == player.whoAmI) {
                        proj.Kill();
                    }
                }
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 0) {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, PrefixCD(Item), PrefixCrit(Item));
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
                player.UpdateMaxTurrets();
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor) {
            //return Color.White;
            return null;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.OrichalcumBar, 12);
            recipe.AddIngredient(ItemID.HallowedBar, 6);
            recipe.AddIngredient(ItemID.SoulofNight, 12);
            recipe.AddIngredient(ModContent.ItemType<SoulofAnotherWorld>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
