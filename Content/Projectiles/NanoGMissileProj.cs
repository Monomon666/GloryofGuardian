using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace DreamJourney.Content.Projectiles.Ranged
{
    public class NanoGMissileProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;

            Projectile.tileCollide = false;

            Projectile.light = 1.5f;
            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        Vector2 tarpos;
        public override void OnSpawn(IEntitySource source) {
            tarpos = Main.npc[(int)Projectile.ai[0]].Center;
        }

        int count = 0;
        int mode = 0;
        bool thefirst = true;
        public override void AI() {
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            //粒子
            if (count % 1 == 0) {
                for (int i = 0; i < 1; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Electric, 0f, 0f, 50, Color.Red, 0.8f);
                    Main.dust[num].velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero) * 2;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 15 == 0) Projectile.velocity *= 1.05f;

            //制导
            if (count >= 30) {
                //如果敌怪存活就追踪
                if (Main.npc[(int)Projectile.ai[0]] != null && Main.npc[(int)Projectile.ai[0]].active) {
                    if (thefirst) Main.npc[(int)Projectile.ai[0]].AddBuff(ModContent.BuffType<NanoMarkDebuff2>(), 300);

                    if (Vector2.Distance(Owner.Center, Projectile.Center) > 120) Turn(Main.npc[(int)Projectile.ai[0]]);
                    if (Vector2.Distance(Owner.Center, Projectile.Center) < 120) Projectile.velocity = (Main.npc[(int)Projectile.ai[0]].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16;
                    Projectile.velocity = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation)).SafeNormalize(Vector2.Zero) * 16f;
                }
                //如果敌怪死了,太早就想着那个点飞,太晚就直线飞
                if (Main.npc[(int)Projectile.ai[0]] == null || !Main.npc[(int)Projectile.ai[0]].active) {
                    thefirst = false;
                    if (count < 32) Projectile.velocity = (tarpos - Projectile.Center).SafeNormalize(Vector2.Zero) * 16f;
                    if (count > 32) Projectile.velocity *= 1f;
                }
            }

            if (count > 1200) Projectile.Kill();
        }

        //炮台转动
        float wrotation = -MathHelper.PiOver2;
        float projRot = 0;
        /// <summary>
        /// 炮台旋转
        /// </summary>
        void Turn(NPC target1) {
            Vector2 tarpos = target1.Center + new Vector2(0, target1.height / 2);
            Vector2 projcen = Projectile.Center + new Vector2(0, 16);

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.04f + (count / 1200f);

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

        List<int> ignore = new List<int>();
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return false;
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Electric, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            //过载攻击爆炸
            for (int j = 0; j < 10; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 10, Color.Black, 1.7f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 3f;
                num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 10);
                Main.dust[num2].velocity *= 3f;
            }
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 120;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 0;
            Projectile.Damage();
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = null;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileProj2").Value;
            Texture2D texture2g = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileProj2Glow").Value;

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0 && Projectile.ai[0] == 1) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + texture0.Size() / 2 + new Vector2(0f, Projectile.gfxOffY);

                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture2, drawPos, null, color, Projectile.rotation, texture2.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + texture2.Size() / 2 + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw(texture2, drawPos, null, lightColor * 3, Projectile.rotation, texture2.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0 && Projectile.ai[0] == 1) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + texture0.Size() / 2 + new Vector2(0f, Projectile.gfxOffY);

                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture2g, drawPos, null, color, Projectile.rotation, texture2g.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + texture2g.Size() / 2 + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw(texture2g, drawPos, null, lightColor * 3, Projectile.rotation, texture2g.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return false;
        }
    }
}
