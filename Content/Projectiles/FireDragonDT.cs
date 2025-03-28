using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FireDragonDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 4;//动图帧数
        }

        public sealed override void SetDefaults() {
            Projectile.width = 136;
            Projectile.height = 24;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 60;//默认发射间隔
            interval = 60;
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int countsin = 0;
        int countpeace = 0;
        int interval = 0;
        //重力
        bool drop = true;
        //炮台转动
        float wrotation = 0;
        float projRot = 0;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //重置判定
        bool firstatk = false;
        int numatk = 0;
        //初始
        Vector2 center0 = new Vector2(0, 0);
        int dir = 0;
        public override void AI() {
            if (countsin == 0) {
                if (Owner.direction > 0) wrotation = 0;
                else wrotation = -MathHelper.Pi;
            }

            count++;
            countsin++;
            Projectile.timeLeft = 2;
            Projectile.direction = dir;
            Projectile.Center += new Vector2(0, (float)((Math.Sin(countsin / 12f) - 0f) * 0.3f));

            Projectile.StickToTiles(false, false);//形成判定
            Calculate();

            Lighting.AddLight(Projectile.Center, 188 * 0.01f, 62 * 0.01f, 68 * 0.01f);//战斗状态发亮

            //索敌与行动
            NPC target1 = null;
            dir = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? 1 : -1;
            target1 = ((Projectile.Center + new Vector2(0, 6)).InDirClosestNPC(540, true, true, dir));
            if (target1 == null) target1 = Projectile.Center.InPosClosestNPC(540, true, true);
            if (target1 == null) count = 0;

            if (target1 != null) {
                Attack(target1);
                Turn(target1);
            }

            if (target1 == null) {
                count = 0;
                interval = Gcount;
                firstatk = false;
            }

            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 8)
                % 4;//要手动填，不然会出错

            base.AI();
        }

        /// <summary>
        /// 重新计算和赋值参数
        /// </summary>
        void Calculate() {
            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0]);//攻击间隔因子重新提取
            //伤害修正
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            Vector2 tarpos = target1.Center;
            Vector2 projcen = Projectile.Center + new Vector2(0, 0);

            //发射
            //普通
            if (count >= interval) {
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        float vel = Main.rand.NextFloat(0.9f, 1.15f) * 4f;
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + nowvel * 56f, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<FireDragonProj>(), lastdamage, 0, Owner.whoAmI, 1);
                        proj1.extraUpdates += 2;
                        proj1.timeLeft = 120;
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        float vel = Main.rand.NextFloat(0.9f, 1.15f) * 4f;
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + nowvel * 56f, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<FireDragonProj>(), lastdamage, 1, Owner.whoAmI, 2);
                        proj1.extraUpdates += 2;
                        proj1.timeLeft = 120;
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                count -= 2;
            }
        }

        /// <summary>
        /// 炮台旋转
        /// </summary>
        void Turn(NPC target1) {
            Vector2 tarpos = target1.Center;
            Vector2 projcen = Projectile.Center + new Vector2(0, -20);

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);

            float rspeed = 0.2f;

            //转头
            if (wrotation != tarrot) {
                if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed) {//如果方向差小于单次转量则本次旋转将超过目标,取余很重要
                    wrotation = tarrot;//那么直接让方向与到目标方向防止抖动
                                       //tarrot = wrotation;
                    return;
                } else {
                    Vector2 clockwise = (wrotation + rspeed).ToRotationVector2();//这是假设NPC顺时针转动后的单位方向向量
                    Vector2 anticlockwise = (wrotation - rspeed).ToRotationVector2();//这是假设NPC逆时针转动后的单位方向向量
                                                                                     //显然，要比较两个向量哪个与目标夹角更近，就是比较他们与目标向量相减后的长度
                    if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length())//如果顺时针的差值更小
                    {
                        wrotation += rspeed;
                    } else {
                        wrotation -= rspeed;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return false;
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FireDragonDT").Value;

            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, 0);

            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Main.spriteBatch.Draw(
                texture,
                drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                wrotation,
                new Vector2(68, 62),
                Projectile.scale,
                spriteEffects,
                0);
            return false;
        }
    }
}
