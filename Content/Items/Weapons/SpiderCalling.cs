﻿using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using GloryofGuardian.Content.Projectiles.ProjNPC;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class SpiderCalling : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + Name;

        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 30;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Orange;
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
                if (player.statLife < 100) {
                    CombatText.NewText(player.Hitbox,
                            Color.Red,
                            "鲜血不足",//这里是你需要展示的文字
                            true,//dramatic为true可以使得字体闪烁，
                            true
                            );
                    return false;
                }
            }

            if (player.altFunctionUse == 0) {
                // 检测世界中存活的 SpiderNPC 数量
                int spiderCount = CountSpiderNPCs();

                // 如果 SpiderNPC 数量大于等于 4，返回 false
                if (spiderCount >= 4) {
                    CombatText.NewText(player.Hitbox,
                            Color.Red,
                            "到达上限",//这里是你需要展示的文字
                            true,//dramatic为true可以使得字体闪烁，
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
                            if (npc.type == ModContent.NPCType<SpiderNPC>()) {
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

                for (int i = 0; i < Main.maxProjectiles; i++) {
                    Projectile proj = Main.projectile[i];
                    if (proj.type == ModContent.ProjectileType<SpiderProj0>() && proj.owner == player.whoAmI) {
                        proj.Kill();
                    }
                }
            }

            return base.CanUseItem(player);
        }

        private int CountSpiderNPCs() {
            int count = 0;

            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC npc = Main.npc[i];

                // 检查 NPC 是否存活且类型为 SpiderNPC
                if (npc.active && npc.type == ModContent.NPCType<SpiderNPC>()) {
                    count++;
                }
            }

            return count;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 0) {
                if (player.whoAmI == Main.myPlayer) {
                    int type0 = ModContent.NPCType<SpiderNPC>();
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        int x = (int)Main.MouseWorld.X - 9;
                        int y = (int)Main.MouseWorld.Y - 20;
                        int npc1 = NPC.NewNPC(new EntitySource_ItemUse(player, Item), x, y, ModContent.NPCType<SpiderNPC>());
                        Main.npc[npc1].defense = player.statDefense;
                        Main.npc[npc1].damage = damage;

                        player.statLife -= 100;
                    } else {
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type0);//发包，用来联机同步
                    }
                }
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor) {
            //return Color.White;
            return null;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Cobweb, 6);
            recipe.AddIngredient(ItemID.Bone, 6);
            recipe.AddRecipeGroup("GloryofGuardian.AnyEvilMaterials", 6);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }
}
