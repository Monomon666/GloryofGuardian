using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using GloryofGuardian.Content.Classes;
using GloryofGuardian.Content.NPCs.Special;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.WeaponClasses {
    public abstract class GOGCalling : ModItem {
        //这是GOG武器的基础模板

        //允许进行当前伤害类型的重铸操作
        public override bool WeaponPrefix() => true;

        /// <summary>
        /// 分流武器类型,默认true为弹幕式武器,false为npc式武器
        /// </summary>
        protected bool ProjOrNpc = true;
        /// <summary>
        /// 武器对应的炮塔
        /// </summary>
        protected abstract int ProjType { get; }
        /// <summary>
        /// npc索引
        /// </summary>
        protected int NPCType = 0;

        /// <summary>
        /// 炮塔占用的栏位数目
        /// </summary>
        protected abstract int ProjSlot { get; }
        /// <summary>
        /// 该类炮塔的数量上限,-1为无限制
        /// </summary>
        protected abstract int ProjlNumLimit { get; }

        /// <summary>
        /// 右键的行为模式为收回该类炮塔,默认为true
        /// </summary>
        protected bool RightClear = true;

        /// <summary>
        /// 使用音效
        /// </summary>
        protected SoundStyle usesound = SoundID.DD2_DefenseTowerSpawn;

        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;//旅行模式下研究数目
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;//手柄状态自由移动
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;//原版自带的隔墙索敌能力
        }

        //基础数据录入
        public override void SetDefaults() {
            Item.DamageType = GuardianDamageClass.Instance;//戍卫伤害
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = usesound;//哨兵召唤声音
            Item.autoReuse = false;

            Item.shoot = ProjType;
            Item.shootSpeed = 5f;

            Item.channel = true;
            Item.noUseGraphic = false;
            Item.noMelee = true;
            Item.mana = 0;
            Item.scale = 1f;

            Item.staff[Item.type] = true;//法杖式斜45度读图

            SetProperty();
        }

        /// <summary>
        /// 用于设置额外的基础属性，在<see cref="SetDefaults"/>中被最后调用,能够覆盖靠前的设置
        /// </summary>
        public virtual void SetProperty() {
            //Item.damage =
            //Item.width =
            //Item.height =
            //Item.knockBack =
            //Item.value =
            //Item.rare =
        }

        public override bool AltFunctionUse(Player player) {
            return true;
        }

        public override bool CanUseItem(Player player) {
            //弹幕类
            if (ProjOrNpc) {
                if (ProjlNumLimit == 1) {
                    int projnum = 0;
                    for (int i = 0; i < 1000; i++) {
                        Projectile p = Main.projectile[i];
                        if (p.active) {
                            if (p.type == ProjType) {
                                p.Kill();
                            }
                        }
                    }
                }

                if (player.altFunctionUse == 0) {
                    //当炮塔栏位少于消耗栏位,将不能召唤
                    if (ProjSlot > 0 && player.GetModPlayer<GOGModPlayer>().Gslot < ProjSlot) {
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

                //左键
                if (player.altFunctionUse == 0) {
                    Item.UseSound = usesound;
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

                //右键
                if (player.altFunctionUse == 2) {
                    if (RightClear) {
                        Item.UseSound = null;
                        Item.noUseGraphic = true;
                        for (int i = 0; i < Main.maxProjectiles; i++) {
                            Projectile proj = Main.projectile[i];
                            if (proj.type == ProjType && proj.owner == player.whoAmI) {
                                proj.Kill();
                            }
                        }
                    }
                }

                return base.CanUseItem(player);
            } else {
                //左键
                if (player.altFunctionUse == 0) {
                    int type = NPCType;
                    if (player.whoAmI == Main.myPlayer) {
                        int x = (int)Main.MouseWorld.X - 9;
                        int y = (int)Main.MouseWorld.Y - 20;

                        if (Main.netMode == NetmodeID.SinglePlayer) {
                            NPC.NewNPC(new EntitySource_ItemUse(player, Item), x, y, NPCType);
                        }
                        else {
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);//发包，用来联机同步
                        }
                    }
                }

                //右键
                if (player.altFunctionUse == 2) {
                    Item.UseSound = null;
                    Item.noUseGraphic = true;//右键使用时收回炮塔，不需要展现法杖
                    for (int i = 0; i < Main.maxNPCs; i++) {
                        NPC npc = Main.npc[i];
                        if (npc.type == NPCType
                            //&& Vector2.Distance(Main.MouseWorld, npc.Center) < 32
                            ) {
                            npc.active = false;
                        }
                    }
                }

                return base.CanUseItem(player);
            }
        }

        //Shoot函数，召唤炮塔
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (!ProjOrNpc) return false;
            
            if (player.altFunctionUse == 0) {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, PrefixCD(Item), PrefixCrit(Item));
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
                player.UpdateMaxTurrets();
            }
            return false;
        }

        /// <summary>
        /// 用来特判和传递来自武器前缀的攻速加成
        /// </summary>
        public static float PrefixCD(Item item) {
            if (item.prefix == ModContent.PrefixType<Stainless0>()
                || item.prefix == ModContent.PrefixType<Sensitive0>()
                || item.prefix == ModContent.PrefixType<Silent0>()
                ) return 0.85f;
            if (item.prefix == ModContent.PrefixType<Peerless0>()
                || item.prefix == ModContent.PrefixType<Excellent0>()
                || item.prefix == ModContent.PrefixType<Classic0>()
                || item.prefix == ModContent.PrefixType<Precise0>()
                ) return 0.9f;
            if (item.prefix == ModContent.PrefixType<Overclocked0>()
                || item.prefix == ModContent.PrefixType<Blooey0>()
                ) return 1f;
            if (item.prefix == ModContent.PrefixType<Scrapped0>()
                ) return 1.05f;
            if (item.prefix == ModContent.PrefixType<Burdened0>()
                || item.prefix == ModContent.PrefixType<Damaged0>()
                ) return 1.1f;
            if (item.prefix == ModContent.PrefixType<ShortCircuited0>()
                ) return 1.15f;
            return 1;
        }

        /// <summary>
        /// 用来特判和传递来自武器前缀的暴击加成
        /// </summary>
        public static float PrefixCrit(Item item) {
            if (item.prefix == ModContent.PrefixType<Excellent0>()
                || item.prefix == ModContent.PrefixType<Sensitive0>()
                ) return 0f;
            if (item.prefix == ModContent.PrefixType<Peerless0>()
                || item.prefix == ModContent.PrefixType<Classic0>()
                || item.prefix == ModContent.PrefixType<Silent0>()
                ) return 5f;
            if (item.prefix == ModContent.PrefixType<Overclocked0>()
                || item.prefix == ModContent.PrefixType<Burdened0>()
                ) return 10f;
            if (item.prefix == ModContent.PrefixType<Blooey0>()
                || item.prefix == ModContent.PrefixType<ShortCircuited0>()
                ) return 20f;
            if (item.prefix == ModContent.PrefixType<Scrapped0>()
                ) return -5f;
            if (item.prefix == ModContent.PrefixType<Precise0>()
                || item.prefix == ModContent.PrefixType<Damaged0>()
                ) return -10f;
            if (item.prefix == ModContent.PrefixType<Stainless0>()
                ) return -20f;
            return 0;
        }

        //武器的发光能力
        public override Color? GetAlpha(Color lightColor) {
            //return Color.White;发光
            return null;//不发光
        }
    }
}
