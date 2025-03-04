using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    //这是GRmod的第一个武器，它将成为许多未来武器的模板，附带许多注释
    //也会附带许多甚至是被注释掉的多余的功能用于将来发挥作用
    //它要继承GRCalling这个Mod特有的武器类
    public class WildCalling : GOGCalling
    {
        //用于加载在Assets文件夹中的贴图
        public override string Texture => GOGConstant.Weapons + Name;

        //SSD,对于大部分不变的基础属性进行设置，仅在打开游戏时加载一次
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;//旅行模式下研究数目
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;//必带
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;//隔墙索敌能力，仅适用于原版自带的索敌逻辑
        }

        //SD，在生成该武器时设定和确认的数值
        public override void SetDefaults() {
            Item.damage = 15;
            Item.DamageType = GuardianDamageClass.Instance;//戍卫伤害
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;//哨兵召唤声音
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<WildDT>();
            Item.shootSpeed = 5f;

            Item.channel = true;
            Item.noUseGraphic = false;
            Item.noMelee = true;
            Item.mana = 0;
            Item.scale = 1f;

            Item.staff[Item.type] = true;//法杖式斜45度读图
        }

        //两用开关，大部分炮塔召唤杖要有右键召回炮塔的能力
        public override bool AltFunctionUse(Player player) {
            return true;
        }

        //两用内容，左键判定鼠标位置是否能召唤炮塔，右键收回
        public override bool CanUseItem(Player player) {
            if (player.altFunctionUse == 0) {
                if (player.GetModPlayer<GOGModPlayer>().Gslot == 0) {
                    CombatText.NewText(player.Hitbox,//跳字生成的矩形范围
                            Color.Red,//跳字的颜色
                            "戍卫栏不足",//这里是你需要展示的文字
                            false,//dramatic为true可以使得字体闪烁，
                            true //dot为true可以使得字体略小，跳动方式也不同(原版debuff扣血格式)
                            );
                    return false;
                }
            }

            //属性重置
            Item.noUseGraphic = false;
            //左键
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
            //右键
            if (player.altFunctionUse == 2) {
                Item.UseSound = null;
                Item.noUseGraphic = true;//右键使用时收回炮塔，不需要展现法杖
                for (int i = 0; i < Main.maxProjectiles; i++) {
                    Projectile proj = Main.projectile[i];
                    if (proj.type == ModContent.ProjectileType<WildDT>() && proj.owner == player.whoAmI) {
                        proj.Kill();
                    }
                }
            }

            return base.CanUseItem(player);
        }

        //Shoot函数，召唤炮塔
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 0) {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, PrefixCD(), PrefixCrit());
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
                player.UpdateMaxTurrets();
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

        //武器的发光能力
        public override Color? GetAlpha(Color lightColor) {
            //return Color.White;发光
            return null;//不发光
        }

        //物品合成表
        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddRecipeGroup("GloryofGuardian.AnyCopperBar", 2);
            recipe.AddIngredient(ItemID.WoodenArrow, 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
