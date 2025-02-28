using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SRMeteorProj : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = false;

            Projectile.light = 1f;

            Projectile.scale *= 0.6f;

            Projectile.extraUpdates += 0;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        int projtype = 0;
        Vector2 spawnpos = new Vector2(0, 0);
        public override void OnSpawn(IEntitySource source) {
            spawnpos = Projectile.Center;
        }

        int count = 0;
        public override void AI() {
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation();

            //普攻的下坠式ai
            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 1) {
                float G = Projectile.ai[1];
                Projectile.velocity += new Vector2(0, G);
            }

            //高于目标高度则无视墙体
            if (Projectile.Center.Y < spawnpos.Y) Projectile.tileCollide = false;
            if (Projectile.Center.Y >= spawnpos.Y) Projectile.tileCollide = true;

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] == 0) {
                Projectile.damage /= 3;
                Projectile.ai[0] = -1;
            }
            if (Projectile.ai[0] == 1) {
                Projectile.damage /= 2;
                Projectile.ai[0] = -1;
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //普通
            if (Projectile.ai[0] == 0) {

            }
            //强化
            if (1 == 1) {
                //爆炸
                for (int j = 0; j < 52; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0, 0, 0, Color.White, 1.5f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(5.5f, 6f);
                }
                SoundEngine.PlaySound(in SoundID.Item14, Projectile.Center);
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
            //附属
            if (Projectile.ai[0] == 2) {

            }

            //基本爆炸粒子
            for (int i = 0; i < 12; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Flare, 0f, 0f, 50, Color.White, 2f);
                Main.dust[num].velocity *= 2f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 1f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRMeteorDT2").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRMeteorProj1").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRMeteorProj2").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRMeteorProj3").Value;

            Texture2D texture = texture1;
            int type = Main.rand.Next(3);
            if (type == 0)
                if (Projectile.ai[0] == 0) texture = texture1;
            if (Projectile.ai[0] == 1) texture = texture2;
            if (Projectile.ai[0] == 2) texture = texture3;

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color * 0.3f, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return false;
        }
    }
}
