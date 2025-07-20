using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using GloryofGuardian.Content.Classes;
using GloryofGuardian.Content.Dusts;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class AdamantiteDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 110;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = -1;
            Projectile.scale *= 1.4f;

            OtherHeight = 34;

            count0 = 8;
            exdust = DustID.CrimsonTorch;

            TurnCenter = new Vector2(-4, -44);

            //过近筛除,枪管设计,不攻击过近的敌人
            closeignoredistance = 64;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 AttackPos2 = Vector2.Zero;
        int Recoil = 0;
        bool crit = false;

        //不继承ai,重写
        int count2 = 0;
        //重力
        bool drop = true;
        //炮台转动
        float wrotation = 0;
        float projRot = 0;

        float wrotation2 = 0;
        float projRot2 = 0;
        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(6, -40);
            AttackPos2 = Projectile.Center + new Vector2(-14, -40);

            //count++;不用加,父类里加过了
            count2++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            if (drawcount == 1) {
                wrotation = 0.3f;
                wrotation2 = -MathHelper.Pi - 0.3f;
            }

            //索敌与行动
            NPC target1 = (Projectile.Center.InDirClosestNPC(1600, false, true, -1));
            if (target1 != null) {
                Attack1(target1);
                Turn(target1);
            }

            if (target1 == null) {
                count2 = 0;
            }

            NPC target2 = (Projectile.Center.InDirClosestNPC(1600, false, true, 1));
            if (target2 != null) {
                Attack2(target2);
                Turn2(target2);
            }

            if (target2 == null) {
                count = 0;
            }

            base.AI();
        }

        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 2) * 16);
                        break;
                    }
                }
                drop = false;
            }
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
        /// 炮台旋转
        /// </summary>
        void Turn(NPC target1) {

            Vector2 tarpos = target1.Center;
            Vector2 projcen = AttackPos;

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.1f;

            //转头
            if (wrotation != tarrot) {
                if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed) {
                    wrotation = tarrot;
                    //
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
        void Turn2(NPC target2) {
            Vector2 tarpos = target2.Center;
            Vector2 projcen = AttackPos2;

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot2 + degree2 * Projectile.spriteDirection);
            float rspeed = 0.1f;

            //转头
            if (wrotation2 != tarrot) {
                if (Math.Abs(wrotation2 - tarrot) % Math.PI <= rspeed) {//如果方向差小于单次转量则本次旋转将超过目标,取余很重要
                    wrotation2 = tarrot;//那么直接让方向与到目标方向防止抖动
                                        //
                    return;
                }
                else {
                    Vector2 clockwise = (wrotation2 + rspeed).ToRotationVector2();//这是假设NPC顺时针转动后的单位方向向量
                    Vector2 anticlockwise = (wrotation2 - rspeed).ToRotationVector2();//这是假设NPC逆时针转动后的单位方向向量
                                                                                      //
                    if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length()) {
                        wrotation2 += rspeed;
                    }
                    else {
                        wrotation2 -= rspeed;
                    }
                }
            }
        }

        void Attack1(NPC target1) {
            //发射
            if (count2 >= Gcount) {
                for (int i = 0; i < 1; i++) {
                    float vel = Main.rand.NextFloat(0.9f, 1.15f) * 6f;
                    Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos + nowvel * 64f, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * vel, ModContent.ProjectileType<AdamantiteProj>(), lastdamage, 2, Owner.whoAmI);
                    //if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    //    if (proj1.ModProjectile is GOGProj proj2) {
                    //        proj2.OrichalcumMarkProj = true;
                    //        proj2.OrichalcumMarkProjcount = 300;
                    //    }
                    //}

                }

                //计时重置,通过更改这个值来重置攻击,枪械不受ex缩减影响
                count2 = 0;
            }
        }

        void Attack2(NPC target2) {
            Vector2 tarpos = target2.Center + new Vector2(0, target2.height / 2);
            Vector2 projcen = Projectile.Center + new Vector2(0, 16);

            //发射
            if (count >= Gcount) {
                for (int i = 0; i < 1; i++) {
                    float vel = Main.rand.NextFloat(0.9f, 1.15f) * 6f;
                    Vector2 nowvel = new Vector2((float)Math.Cos(wrotation2), (float)Math.Sin(wrotation2));

                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                    //普通
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2 + nowvel * 64f, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * vel, ModContent.ProjectileType<AdamantiteProj>(), lastdamage, 2, Owner.whoAmI);
                    //if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    //    if (proj1.ModProjectile is GOGProj proj2) {
                    //        proj2.OrichalcumMarkProj = true;
                    //        proj2.OrichalcumMarkProjcount = 300;
                    //    }
                    //}
                }

                //计时重置,通过更改这个值来重置攻击,枪械不受ex缩减影响
                count = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return base.Colliding(projHitbox, targetHitbox);
        }


        //浮动因子
        int floatcount = 0;
        float float1 = 0;
        bool canfloat = false;
        public override bool PreDraw(ref Color lightColor) {
            //浮动因子计算
            floatcount++;
            float colorsca = 0.7f;
            colorsca = 1f;
            float1 = (float)Math.Sin(floatcount / 48f) + 1;

            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AdamantiteDT").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AdamantiteDT2").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AdamantiteDT3").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AdamantiteDT4").Value;

            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition + new Vector2(0, -25);


            Texture2D texture333 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AdamantiteProj").Value;

            Main.EntitySpriteDraw(texture1, drawPosition1, null, lightColor, Projectile.rotation, texture1.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            //右炮
            Vector2 drawPosition2 = drawPosition1 + new Vector2(18, -12);
            Main.EntitySpriteDraw(texture2, drawPosition2, null, lightColor, wrotation, texture2.Size() * 0.5f + new Vector2(-12, 0), Projectile.scale, SpriteEffects.None, 0);
            //左炮
            Vector2 drawPosition3 = drawPosition1 + new Vector2(-18, -12);
            Main.EntitySpriteDraw(texture2, drawPosition3, null, lightColor, wrotation2 + MathHelper.Pi, texture2.Size() * 0.5f + new Vector2(12, 0), Projectile.scale, SpriteEffects.FlipHorizontally, 0);

            Main.EntitySpriteDraw(texture3, drawPosition1, null, lightColor, Projectile.rotation, texture3.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            //宝石
            Vector2 drawPosition4 = Projectile.Center - Main.screenPosition + new Vector2(-28, -12);
            Main.EntitySpriteDraw(texture4, drawPosition1 + new Vector2(0, -32) + new Vector2(0, -12) * float1, null, lightColor * colorsca, Projectile.rotation, texture4.Size() * 0.5f, Projectile.scale * (float1 * 0.04f + 1) * 0.6f, SpriteEffects.None, 0);
            return false;
        }
    }
    public class AdamantiteProj : GOGProj {
        public override string Texture => GOGConstant.nulls;
        public override void SetProperty() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            base.SetProperty();

            Projectile.scale *= 1.4f;
        }
        
        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            //if (0 != 3) {
            //    for (int j = 0; j < 4; j++) {
            //        int num = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<PaleDust>(), 0f, 0f, 10, Color.White, 1f);
            //        Main.dust[num].velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-1f, 1f));
            //        Main.dust[num].velocity *= Main.rand.NextFloat(3f, 4f);
            //        Main.dust[num].noGravity = true;
            //    }
            //}
            //
            //if (3 == 3) {
            //    //爆炸
            //    for (int j = 0; j < 13; j++) {
            //        int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0, 0, 0, Color.White, 2f);
            //        Main.dust[num1].noGravity = true;
            //        Main.dust[num1].velocity = new Vector2((float)Math.Sin(j * 48 / 100f), (float)Math.Cos(j * 48 / 100f)) * Main.rand.NextFloat(3f, 6f);
            //
            //        int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0, 0, 0, Color.White, 1f);
            //        Main.dust[num2].noGravity = true;
            //        Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 48 / 100f), (float)Math.Cos(j * 48 / 100f)) * Main.rand.NextFloat(4f, 6f);
            //    }
            //    Projectile.position = Projectile.Center;
            //    Projectile.width = Projectile.height = 40;
            //    Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            //    Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
            //    Projectile.maxPenetrate = -1;
            //    Projectile.penetrate = -1;
            //    Projectile.usesLocalNPCImmunity = false;
            //    Projectile.usesIDStaticNPCImmunity = true;
            //    Projectile.idStaticNPCHitCooldown = 0;
            //    Projectile.Damage();
            //}
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunProj").Value;
            Texture2D texture11 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusBlackShadow").Value;
            Texture2D texture12 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusShadow1").Value;
            Texture2D texture13 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Rhombus").Value;

            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunProj3").Value;

            if (Projectile.ai[0] == 0) {
                for (int k = 0; k < Projectile.oldPos.Length - 3; k++) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(0f, Projectile.gfxOffY);
                    float color = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.4f;
                    if (k != 0) color = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.4f;
                    if (k == 0) color = 1;

                    Main.EntitySpriteDraw(
                        texture11,
                        drawPos,
                        null,
                        new Color(255, 0, 0) * color,
                        Projectile.rotation,
                        texture11.Size() / 2,
                        Projectile.scale * new Vector2(0.4f, 0.4f),
                        SpriteEffects.None,
                        0);

                    Main.EntitySpriteDraw(
                        texture12,
                        drawPos,
                        null,
                        new Color(255, 10, 10, 0) * 1f * color,
                        Projectile.rotation,
                        texture12.Size() / 2,
                        Projectile.scale * new Vector2(0.4f, 0.4f),
                        SpriteEffects.None,
                        0);

                    Main.EntitySpriteDraw(
                        texture13,
                        drawPos,
                        null,
                        new Color(255, 20, 20, 0) * 1f * color,
                        Projectile.rotation,
                        texture13.Size() / 2,
                        Projectile.scale * new Vector2(0.4f, 0.4f),
                        SpriteEffects.None,
                        0);
                }
            }

            return base.PreDraw(ref lightColor);
        }
    }
}
