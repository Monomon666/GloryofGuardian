using DreamJourney.Content.Items.Accessories;
using GloryofGuardian.Content.Buffs;
using System.Collections.Generic;
using System.Linq;
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
        }

        //近战武器击中敌怪时触发，可以触发某些机制，可以修改对敌方单位造成的伤害，但是该mod大概率并没有近战武器（
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers) {
        }

        //弹幕击中敌怪时触发，可以触发某些机制，可以修改对敌方单位造成的伤害
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers) {
            //指向该敌人的单位向量
            Vector2 totarget = (target.Center - Player.Center).SafeNormalize(Vector2.Zero);
            base.ModifyHitNPCWithProj(proj, target, ref modifiers);
        }

        #endregion

        #region 受击特效

        //受到伤害时触发
        public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
            
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
                Gslot += 8;
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
            base.ResetEffects();
        }

        #endregion
    }
}