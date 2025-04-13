using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using GloryofGuardian.Content.Projectiles.ProjNPC;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class FlamingJackCalling : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + Name;

        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 50;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = 7;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<GarrisonDT>();
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
                if (Main.myPlayer == player.whoAmI) {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        foreach (NPC npc in Main.ActiveNPCs) {
                            if (npc.type == ModContent.NPCType<FlamingJackNPC>()) {
                                npc.life = 0;
                                npc.active = false;

                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            }
                        }
                    else {
                        //网络同步放在这里
                    }
                }
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 0) {
                if (player.whoAmI == Main.myPlayer) {
                    int type0 = ModContent.NPCType<SpiderNPC>();
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        int x = (int)Main.MouseWorld.X - 9;
                        int y = (int)Main.MouseWorld.Y - 20;
                        int npc1 = NPC.NewNPC(new EntitySource_ItemUse(player, Item), x, y, ModContent.NPCType<FlamingJackNPC>());
                        Main.npc[npc1].defense = player.statDefense;
                    } else {
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type0);//发包，用来联机同步
                    }
                }
            }
            return false;
        }



        /// <summary>
        /// 用来特判和传递来自武器前缀的攻速加成
        /// </summary>
        float PrefixCD() {
            if (Item.prefix == ModContent.PrefixType<Stainless0>()
                || Item.prefix == ModContent.PrefixType<Sensitive0>()
                || Item.prefix == ModContent.PrefixType<Silent0>()
                ) return 0.85f;
            if (Item.prefix == ModContent.PrefixType<Peerless0>()
                || Item.prefix == ModContent.PrefixType<Excellent0>()
                || Item.prefix == ModContent.PrefixType<Classic0>()
                || Item.prefix == ModContent.PrefixType<Precise0>()
                ) return 0.9f;
            if (Item.prefix == ModContent.PrefixType<Overclocked0>()
                || Item.prefix == ModContent.PrefixType<Blooey0>()
                ) return 1f;
            if (Item.prefix == ModContent.PrefixType<Scrapped0>()
                ) return 1.05f;
            if (Item.prefix == ModContent.PrefixType<Burdened0>()
                || Item.prefix == ModContent.PrefixType<Damaged0>()
                ) return 1.1f;
            if (Item.prefix == ModContent.PrefixType<ShortCircuited0>()
                ) return 1.15f;
            return 1;
        }

        /// <summary>
        /// 用来特判和传递来自武器前缀的暴击加成
        /// </summary>
        float PrefixCrit() {
            if (Item.prefix == ModContent.PrefixType<Excellent0>()
                || Item.prefix == ModContent.PrefixType<Sensitive0>()
                ) return 0f;
            if (Item.prefix == ModContent.PrefixType<Peerless0>()
                || Item.prefix == ModContent.PrefixType<Classic0>()
                || Item.prefix == ModContent.PrefixType<Silent0>()
                ) return 5f;
            if (Item.prefix == ModContent.PrefixType<Overclocked0>()
                || Item.prefix == ModContent.PrefixType<Burdened0>()
                ) return 10f;
            if (Item.prefix == ModContent.PrefixType<Blooey0>()
                || Item.prefix == ModContent.PrefixType<ShortCircuited0>()
                ) return 20f;
            if (Item.prefix == ModContent.PrefixType<Scrapped0>()
                ) return -5f;
            if (Item.prefix == ModContent.PrefixType<Precise0>()
                || Item.prefix == ModContent.PrefixType<Damaged0>()
                ) return -10f;
            if (Item.prefix == ModContent.PrefixType<Stainless0>()
                ) return -20f;
            return 0;
        }

        public override Color? GetAlpha(Color lightColor) {
            //return Color.White;
            return null;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
