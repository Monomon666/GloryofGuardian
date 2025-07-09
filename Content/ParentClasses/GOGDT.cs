using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using GloryofGuardian.Content.Classes;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.ParentClasses {
    public abstract class GOGDT : ModProjectile {
        //这是GOG的炮塔的基础模板
        //完成数据预声明与处理,下落,预索敌,旋转等功能

        /// <summary>
        /// 绝对时间
        /// </summary>
        public int globalcount = (int)Main.GameUpdateCount;
        /// <summary>
        /// 最早标记,用于栏位检查
        /// </summary>
        bool earliest = true;

        /// <summary>
        /// 攻击间隔的初始值
        /// </summary>
        protected int count0 = 60;
        /// <summary>
        /// 索敌位置
        /// </summary>
        /// <returns></returns>
        protected Vector2 AttackPos = Vector2.Zero;

        /// <summary>
        /// 炮塔是否为坠落型
        /// </summary>
        protected bool Drop = true;
        /// <summary>
        /// 炮塔是否会旋转炮管
        /// </summary>
        protected bool CanTurn = true;
        /// <summary>
        /// 炮管的额外转动次数
        /// </summary>
        protected int ExtraTurn = 0;

        /// <summary>
        /// 基于使下落正常的碰撞箱高度,用于补全完整贴图高度的高度差
        /// 用于生成基于贴图大小的粒子
        /// </summary>
        protected int OtherHeight = 0;

        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.height = 16;
            Projectile.tileCollide = true;

            Projectile.sentry = false;//哨兵身份证
            Projectile.minion = true;//仆从身份证

            Projectile.DamageType = GuardianDamageClass.Instance;

            Projectile.timeLeft = 36000;
            Projectile.scale *= 1f;

            AttackPos = Projectile.Center;

            exdpos = Projectile.position;
            exdx = Projectile.width;
            exdy = Projectile.height;

            SetProperty();
        }

        /// <summary>
        /// 用于设置额外的基础属性，在<see cref="SetDefaults"/>中被最后调用,能够覆盖靠前的设置
        /// </summary>
        public virtual void SetProperty() {
            //Projectile.width = 
            //Projectile.friendly = false;//true为可以造成接触伤害
            //Projectile.penetrate = -1;
        }

        Player Owner => Main.player[Projectile.owner];

        //通常情况下,我们不希望接触伤害型炮塔破坏地图道具,当然,也可以有例外
        public override bool? CanCutTiles() {
            return false;
        }

        /// <summary>
        /// 默认计时器
        /// </summary>
        protected int count = 0;
        /// <summary>
        /// 绘制用计时器(不重置)
        /// </summary>
        protected int drawcount = 0;
        /// <summary>
        /// 计算得出的最终攻击间隔
        /// </summary>
        protected int Gcount = 0;
        /// <summary>
        /// 计算得出的最终伤害值
        /// </summary>
        protected int lastdamage = 0;

        protected bool findboss = false;
        /// <summary>
        /// 预先设定的第一索敌目标
        /// </summary>
        protected NPC target0 = null;
        /// <summary>
        /// 索敌距离
        /// </summary>
        /// <returns></returns>
        protected int Attackrange = 800;

        //额外属性
        /// <summary>
        /// 额外的超频率,加算,100为100%
        /// </summary>
        protected int othercrit = 0;
        /// <summary>
        /// 额外的攻击间隔缩减,乘算,1为100%
        /// </summary>
        protected float otheratkspeed = 1;
        /// <summary>
        /// 状态机
        /// </summary>
        protected int mode = 0;

        //使用preai可以在不使用base的情况下进行预设
        public override bool PreAI() {
            count++;
            drawcount++;
            Projectile.timeLeft = 2;

            Rank();
            Orichalcum();

            Projectile.StickToTiles(false, false);//形成判定
            if (Drop) DTDrop();//坠落
            Calculate();//攻击间隔和伤害的重新计算
            //索敌与行动
            target0 = AttackPos.InPosClosestNPC(Attackrange, false, findboss);

            if (target0 != null) {
                if (CanTurn) {
                    for (int i = 0; i < 2 + ExtraTurn; i++) {
                        Turn(target0);
                    }
                }
                Attack0(target0);
            }

            if (Projectile.damage == 0) Attack0(null);

            return base.PreAI();
        }

        /// <summary>
        /// 栏位检查
        /// </summary>
        void Rank() {
            //栏位检查:
            //查询当前玩家的栏位，如果已经超出上限，则依据编号挤掉自己
            {
                //重置标记
                earliest = true;
                //编号
                int MaxIndex = 0;
                int index = 0;//定义序号为0
                foreach (var proj in Main.projectile)//遍历所有弹幕
                {
                    if (proj.active //活跃状态
                        && proj.ModProjectile is GOGDT gogdt1//遍历
                        && proj.owner == Owner.whoAmI //同属同一个主人
                        ) {

                        //总数
                        MaxIndex++;

                        ////特判占用更多栏位的炮台
                        //if (proj.type == ModContent.ProjectileType<PaleShotGunDT>()) MaxIndex += 1;
                        //
                        //if (proj.type == ModContent.ProjectileType<PalladiumDT>()) MaxIndex += 1;
                        //if (proj.type == ModContent.ProjectileType<OrichalcumDT>()) MaxIndex += 1;
                        //if (proj.type == ModContent.ProjectileType<TitaniumDT>()) MaxIndex += 1;
                        //if (proj.type == ModContent.ProjectileType<AdamantiteDT>()) MaxIndex += 2;
                        //
                        //if (proj.type == ModContent.ProjectileType<PlanteraDT>()) MaxIndex += 1;
                        //
                        //if (proj.type == ModContent.ProjectileType<SRFrostDT>()) MaxIndex += 1;
                        //if (proj.type == ModContent.ProjectileType<SRMeteorDT>()) MaxIndex += 2;
                        //
                        //if (proj.type == ModContent.ProjectileType<SoulBalanceDT>()) MaxIndex += 1;
                        //
                        //if (proj.type == ModContent.ProjectileType<FireDragonDT>()) MaxIndex += 2;
                        //
                        //if (proj.type == ModContent.ProjectileType<NanoGMissileDT>()) MaxIndex += 1;
                        //if (proj.type == ModContent.ProjectileType<NanoUAVDT>()) MaxIndex += 2;
                        //
                        //if (proj.type == ModContent.ProjectileType<SwordNurturingGourdDT>()) MaxIndex += 4;
                        //
                        //if (proj.type == ModContent.ProjectileType<FinalSpiralfDT>()) MaxIndex += 7;
                        //
                        ////特判不占用栏位的炮台
                        //if (proj.type == ModContent.ProjectileType<SlimeProj0>()) MaxIndex -= 1;

                        //序列号
                        if (gogdt1.globalcount < globalcount) {
                            //检测是否有比自己先生成的炮塔,如果有说明自己的序号要后移一位
                            index++;
                            earliest = false;
                        }
                    }
                }

                //如果召唤物超出上限，则
                if (MaxIndex > Owner.GetModPlayer<GOGModPlayer>().Gslot && earliest) {
                    Projectile.Kill();
                }
            }
        }

        /// <summary>
        /// 山铜印记强化
        /// </summary>
        void Orichalcum() {
            ////山铜印记强化
            //if (OrichalcumMarkDT2 == false) OrichalcumMarkDT = false;
            //if (OrichalcumMarkDT2 == true) OrichalcumMarkDT2 = false;
            //
            //if (OrichalcumMarkDT) {
            //    if (OrichalcumMarkCrit) {
            //        OrichalcumMarkCrit = true;
            //        Projectile.CritChance += 20;
            //    }
            //}
            //
            //if (OrichalcumMarkDT && OrichalcumMarkCrit) Projectile.CritChance -= 20;
        }

        public bool HasDropped = false;
        /// <summary>
        /// 坠落
        /// </summary>
        void DTDrop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (!HasDropped) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 4));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 2) * 4);
                        break;
                    }
                }
                HasDropped = true;
            }
        }

        /// <summary>
        /// 重新计算和赋值参数
        /// </summary>
        void Calculate() {
            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0] * otheratkspeed);//攻击间隔因子重新提取
            //伤害修正
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }


        //炮台转动参数
        protected float wrotation = 0;
        protected float projRot = 0;
        protected Vector2 TurnCenter = Vector2.Zero;
        /// <summary>
        /// 炮台旋转
        /// </summary>
        void Turn(NPC target1) {
            Vector2 tarpos = target1.Center;
            Vector2 projcen = Projectile.Center + TurnCenter;

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.04f;

            //转头
            if (wrotation != tarrot) {
                if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed) {
                    wrotation = tarrot;
                    return;
                }
                else {
                    Vector2 clockwise = (wrotation + rspeed).ToRotationVector2();
                    Vector2 anticlockwise = (wrotation - rspeed).ToRotationVector2();
                    //
                    if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length()) {
                        wrotation += rspeed;
                    }
                    else {
                        wrotation -= rspeed;
                    }
                }
            }
        }

        //攻击次数
        protected int attacknum = 0;
        //攻击完成,重置攻击
        protected bool FinishAttack = false;
        /// <summary>
        /// 预设的攻击行为
        /// </summary>
        /// <param name="target1"></param>
        void Attack0(NPC target1) {
            //发射
            if (count >= Gcount) {
                //普通攻击
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + othercrit) {
                    List<Projectile> ProjList = Attack1();

                    //foreach (Projectile proj1 in ProjList) {
                    //    if (proj1 != null && Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    //        if (proj1.ModProjectile is GOGProj proj2) {
                    //            //山铜炮塔的强化效果
                    //            proj2.OrichalcumMarkProj = true;
                    //            proj2.OrichalcumMarkProjcount = 300;
                    //        }
                    //    }
                    //}

                    ProjList.Clear();
                }

                //过载攻击
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + othercrit) {
                    List<Projectile> ProjList = Attack2();

                    //foreach (Projectile proj1 in ProjList) {
                    //    if (proj1 != null && Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    //        if (proj1.ModProjectile is GOGProj proj2) {
                    //            //山铜炮塔的强化效果
                    //            proj2.OrichalcumMarkProj = true;
                    //            proj2.OrichalcumMarkProjcount = 300;
                    //        }
                    //    }
                    //}

                    if (ProjList != null) ProjList.Clear();
                }

                //计时重置,通过更改这个值来重置攻击
                if (FinishAttack) {
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                    attacknum = 0;
                    FinishAttack = false;
                }
            }
        }

        /// <summary>
        /// 在此补充普通攻击逻辑
        /// 注意令FinishAttack为true来结束
        /// </summary>
        protected virtual List<Projectile> Attack1() {
            //List<Projectile> projlist = new List<Projectile>();
            //
            //for (int i = 0; i < 15; i++) {
            //    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), , , ModContent.ProjectileType<>(), lastdamage, 8, Owner.whoAmI, 0, 0, 1);
            //    projlist.Add(proj1);
            //}
            //FinishAttack = true;
            //return projlist;
            return null;
        }

        /// <summary>
        /// 在此补充过载攻击逻辑
        /// 注意令FinishAttack为true来结束
        /// </summary>
        protected virtual List<Projectile> Attack2() {
            return null;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }

        public override void OnKill(int timeLeft) {
            ExplosionDust(exdust);
        }

        //粒子
        protected int exdust = 0;
        protected Vector2 exdpos = Vector2.Zero;
        protected int exdx = 0;
        protected int exdy = 0;

        /// <summary>
        /// 收回炮台的爆炸粒子特效
        /// </summary>
        protected virtual void ExplosionDust(int dustid) {
            if (dustid == 0) dustid = DustID.Wraith;

            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position + new Vector2(0, -OtherHeight), Projectile.width, Projectile.height + OtherHeight, dustid, 0f, 0f, 10, Color.White, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position + new Vector2(0, -OtherHeight), Projectile.width, Projectile.height + OtherHeight, dustid, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        /// <summary>
        /// 预包装好的叠加绘制画布转换
        /// </summary>
        public virtual void AdDraw(Action codeBlock) {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            codeBlock();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
