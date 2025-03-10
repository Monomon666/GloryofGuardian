using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SRFrostProj : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = true;

            Projectile.light = 1f;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        int projtype = 0;
        Vector2 spawnpos = new Vector2(0, 0);
        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int type = 0;
        public override void AI() {
            type = (int)Projectile.ai[0];
            if (count == 0 && type == 2) Projectile.velocity *= 2f;
            if (count == 0 && type == 3) Projectile.velocity *= 0f;

            count++;
            //0:礼物盒,仅在有boss时攻击,出现在boss的前进路线上,爆出一堆随机彩球
            //1:蓝球,从星星向外发射,短时间爆炸 2:红球,快速冲出之后开始自追踪 3.绿球:出现在附近,稍等后掉下来

            if (type == 2) {
                if (count < 60 && count % 10 == 0) {
                    Projectile.penetrate = -1;
                    Projectile.velocity *= 0.8f;
                }
                if (count > 60) {
                    Projectile.penetrate = 1;
                    NPC target1 = Projectile.Center.InPosClosestNPC(1600, true, true);
                    if (target1 != null && target1.active) Projectile.ChasingBehavior(target1.Center, 12f, 16);
                }
            }

            if (type == 3) {
                if (count < 120) {
                    Projectile.penetrate = -1;
                }
                if (count > 120) {
                    Projectile.penetrate = 1;
                    Projectile.tileCollide = true;
                    if (Projectile.velocity.Y < 8) Projectile.velocity.Y += 0.8f;
                }
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 2; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }

                int dust0 = DustID.FireworkFountain_Blue;

                if (type == 1) dust0 = DustID.Firework_Blue;
                if (type == 2) dust0 = DustID.Firework_Red;
                if (type == 3) dust0 = DustID.Firework_Green;

                for (int i = 0; i < 2 / 2; i++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dust0, 0f, 0f, 10, Color.White, 0.5f);
                    Main.dust[num2].velocity = new Vector2(0, Main.rand.NextFloat(-0.5f, 1.5f) * 1f);
                    Main.dust[num2].noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (target.HasBuff(BuffID.Frostburn) && !target.boss) {
                target.AddBuff(BuffID.Frostburn2, 180);
            }

            if (target.HasBuff(BuffID.Frostburn2) && !target.boss) {
                target.velocity *= 0;
            }

            //常态
            target.AddBuff(BuffID.Frostburn, 180);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            if (Projectile.ai[0] == 0) {

            }
            int dust0 = DustID.FireworkFountain_Blue;

            if (type == 1) dust0 = DustID.Firework_Blue;
            if (type == 2) dust0 = DustID.Firework_Red;
            if (type == 3) dust0 = DustID.Firework_Green;

            //爆炸
            for (int j = 0; j < 26; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dust0, 0, 0, 0, Color.White, 1f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 24 / 100f), (float)Math.Cos(j * 24 / 100f)) * Main.rand.NextFloat(6f, 7f);
            }

            SoundEngine.PlaySound(in SoundID.NPCHit3, Projectile.Center);
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

            //基本爆炸粒子
            for (int i = 0; i < 12; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SnowflakeIce, 0f, 0f, 50, Color.White, 0.5f);
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 1f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
                int num2 = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SnowSpray, 0f, 0f, 50, Color.White, 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrostProj").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRFrostProj04").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRFrostProj05").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRFrostProj06").Value;

            Texture2D text = texture;

            if (type == 1) text = texture1;
            if (type == 2) text = texture2;
            if (type == 3) text = texture3;

            Main.EntitySpriteDraw(
                    text,
                    Projectile.Center - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    text.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);

            return false;
        }
    }
}
