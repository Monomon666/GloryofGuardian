using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class PlanteraProj1 : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 30;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失
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
        int height0 = 0;
        int mode = 0;
        //重力
        bool drop = true;
        public override void AI() {
            if (mode == 0 && count == 0) {
                Projectile.Center += new Vector2(0, -16);
            }
            count++;

            Projectile.velocity *= 0;

            if (mode == 0) {
                Projectile.width = 30;
                Projectile.height = 32;
            }

            if (mode == 1) {
                Projectile.width = 30;
                Projectile.height = 96;
            }

            if (mode == 0 && count > 60) Projectile.Kill();
            if (mode == 1 && count > 180) Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        int hitnum = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            hitnum++;
            if (target.knockBackResist > 0) target.velocity *= 0;
            if (target.knockBackResist > 0) target.velocity += new Vector2(0, -height0 / 40f);

            if (hitnum == 3 && mode == 0) {
                mode = 1;
                count = 0;
                Projectile.Center += new Vector2(0, -56);

                for (int j = 0; j < 60; j++) {
                    int num1 = Dust.NewDust(Projectile.position + new Vector2(0, 0), 30, 90, DustID.Plantera_Green, 0f, 0f, 10, Color.White, 2f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity *= 2f;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture00 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj20").Value;
            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj21").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj22").Value;


            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj01").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj02").Value;

            if (mode == 0) {
                Main.EntitySpriteDraw(
                    texture0,
                    Projectile.position + new Vector2(16, 16) - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture0.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);
            }

            if (mode == 1) {
                //4
                Main.EntitySpriteDraw(
                    texture1,
                    Projectile.position + new Vector2(16, 80) - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture1.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);
                //3
                Main.EntitySpriteDraw(
                    texture1,
                    Projectile.position + new Vector2(16, 50) - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture1.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);
                //2
                Main.EntitySpriteDraw(
                    texture1,
                    Projectile.position + new Vector2(16, 20) - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture1.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);
                //1
                Main.EntitySpriteDraw(
                    texture0,
                    Projectile.position + new Vector2(16, -10) - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture0.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);
            }


            return false;
        }
    }
}
