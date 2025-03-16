using GloryofGuardian.Content.Buffs;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Class
{
    public class GOGModPlayer : ModPlayer
    {
        //源提取
        public IEntitySource Source;

        //我决定把炮塔所要使用的数据通过玩家来统筹保存
        //这个决定对于现阶段的我来说能够相对集中装备等对变量的影响
        //避免过复杂的读取

        //变量
        #region 变量存储

        /// <summary>
        /// 炮塔格子
        /// </summary>
        public int Gslot = 0;

        /// <summary>
        /// 炮塔格子2
        /// </summary>
        public int Gslot2 = 0;

        /// <summary>
        /// 炮塔攻击间隔乘算因子
        /// </summary>
        public float GcountR = 1;

        /// <summary>
        /// 炮塔攻击间隔加算因子
        /// </summary>
        public int GcountEx = 0;

        /// <summary>
        /// 预留变量
        /// </summary>
        public int Todo;

        #endregion

        #region 变动开关1

        /// <summary>
        /// 测试饰品孤独残响
        /// </summary>
        public bool residuallonelinessechoes = false;

        #endregion

        #region 变动开关2

        /// <summary>
        /// 钛金屏障
        /// </summary>
        public bool TitaniumShield = false;

        #endregion

        #region 变动开关3

        /// <summary>
        /// 木套装效果
        /// </summary>
        public bool UndeadWood = false;

        /// <summary>
        /// 黯晶套装效果
        /// </summary>
        public bool DarkCrystal = false;

        /// <summary>
        /// 纳米套装效果
        /// </summary>
        public bool Nano = false;

        #endregion

        //player方法
        #region 固有变动

        //去除原有初始物品
        public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath) {
            //旅行模式的玩家初始装备更改
            //Todo
            //非旅行模式的玩家初始装备更改
            //itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.CopperShortsword);
            //itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.CopperAxe);

            //itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.CopperPickaxe);
        }

        //给予玩家新的初始物品，以及中核玩家复活后给予的物品
        //public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) {
        //    if (mediumCoreDeath) {
        //        return Enumerable.Empty<Item>();
        //    } else {
        //        return new[] {
        //        new Item(ItemID.LifeCrystal,5),
        //        new Item(ItemID.ManaCrystal,4),
        //        new Item(ModContent.ItemType<WildCalling>(),1),//开局给荒野炮塔杖
        //        };
        //    }
        //}

        //Todo

        #endregion

        #region 攻击特效

        //击中敌怪时触发，可以触发某些机制，可以修改对敌方单位造成的任何伤害
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            //职业混用惩罚
            //持有炮台核心饰品时自己召唤的炮台才会工作
            //Todo
            //操作modifiers.SourceDamage,或更改Gcount为9999

            //继承穿透,穿透不会自己继承
            //但是写在proj里面就行,因为这个职业目前不存在真近战攻击
        }

        //近战武器击中敌怪时触发，可以触发某些机制，可以修改对敌方单位造成的伤害，但是该mod大概率并没有近战武器（
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers) {
        }

        //弹幕击中敌怪时触发，可以触发某些机制，可以修改对敌方单位造成的伤害
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers) {
            //指向该敌人的单位向量
            Vector2 totarget = (target.Center - Player.Center).SafeNormalize(Vector2.Zero);
            base.ModifyHitNPCWithProj(proj, target, ref modifiers);

            // 如果当前伤害类型是 GuardianDamageClass
            if (proj.DamageType == ModContent.GetInstance<GuardianDamageClass>()) {
                // 继承 DamageClass.Generic 的护甲穿透
                modifiers.ArmorPenetration += Player.GetArmorPenetration(DamageClass.Generic);
            }
        }

        #endregion

        #region 受击特效

        int origindamage = 0;//伤害来源
        int comingdamage = 0;//计算防御
        //减伤?我不会算啊,先不算了吧(
        //从此获取受到伤害的量
        public override void OnHurt(Player.HurtInfo info) {
            // 获取玩家受到的伤害
            origindamage = info.SourceDamage;
            info.SourceDamage = 1;
            base.OnHurt(info);
        }

        //受到伤害时触发
        public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
            //黯晶护盾
            if (DarkCrystal) {
                // 获取玩家受到的原始伤害
                int damage = origindamage;

                // 如果单次伤害高于 60
                if (damage > 40) {
                    // 计算超出部分的伤害
                    int excessDamage = damage - 40;

                    // 将超出部分的伤害降低 50%
                    int reducedDamage = excessDamage / 2;

                    // 设置最终伤害为 40 + 降低后的超出部分
                    int FinalDamage = 40 + reducedDamage;
                    modifiers.SetMaxDamage(FinalDamage);

                    // 显示粒子
                    for (int i = 0; i < 32; i++) {
                        int dust0 = DustID.BlueCrystalShard;
                        if (Main.rand.NextBool(4)) dust0 = DustID.PinkCrystalShard;
                        if (Main.rand.NextBool(4)) dust0 = DustID.PurpleCrystalShard;
                        int num1 = Dust.NewDust(Player.position, Player.width, Player.height, dust0, 0f, 0f, 10, Color.White, 2f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity *= 2.5f;
                    }
                }
            }
        }

        //受到接触伤害时触发
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers) {

        }
        //受到弹幕伤害时触发
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers) {

        }

        #endregion

        #region 免伤方法

        public override bool FreeDodge(Player.HurtInfo info)//常规闪避前的闪避,优先触发
        {
            //Player.immune = true;//开启无敌帧
            //Player.SetImmuneTimeForAllTypes(80);//设置无敌帧长度
            //return true 来触发闪避
            if (TitaniumShield) {
                Player.immune = true;//开启无敌帧
                Player.SetImmuneTimeForAllTypes(90);//设置无敌帧长度
                Player.ClearBuff(ModContent.BuffType<TitaniumShieldBuff>());
                Player.AddBuff(ModContent.BuffType<TitaniumReloadBuff>(), 45 * 60);
                return true;
            }

            return base.FreeDodge(info);
        }

        public override bool ConsumableDodge(Player.HurtInfo info)//常规闪避后的闪避，最后触发
        {
            return base.ConsumableDodge(info);
        }

        #endregion

        #region 其它效果

        #endregion

        #region 每帧更新

        //每帧负面buff生命值更新
        public override void UpdateBadLifeRegen() {
        }

        //每帧生命值更新
        public override void UpdateLifeRegen() {
            base.UpdateLifeRegen();
        }

        public override void PreUpdate() {
            //更新前重置赋值
            Gslot = 0;
            GcountR = 1;
            GcountEx = 0;

            if (residuallonelinessechoes) {
                Gslot += 511;
            }
        }

        //每帧常规更新
        public override void PostUpdate() {
            //代行计时器，可以在这里对某些效果进行充能或冷却计时
        }

        //每帧重置
        public override void ResetEffects() {
            //这里不能够直接重置某些持续性作用的字段,不然会导致问题,它们被放置在了PreUpdate
            //饰品判定重置
            residuallonelinessechoes = false;//孤独残响

            //buff判定重置
            TitaniumShield = false;

            //套装效果重置
            UndeadWood = false;
            DarkCrystal = false;
            Nano = false;

            base.ResetEffects();
        }

        #endregion
    }
}