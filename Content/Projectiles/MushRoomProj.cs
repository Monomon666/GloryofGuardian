using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class MushRoomProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;//穿透数，1为攻击到第一个敌人就消失

            Projectile.alpha = 255;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            Projectile.tileCollide = false;
            Projectile.penetrate = 4;
        }

        int count = 0;
        public override void AI() {
            count++;
            if (count >= 120) Projectile.Kill();

            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            Projectile.alpha -= 30;
            if (count >= 60) Projectile.alpha += 33;
            Projectile.light = 2 * (255 - Projectile.alpha) / 255f;
            Projectile.scale = 0.5f + 0.5f * (255 - Projectile.alpha) / 255f;

            Projectile.velocity *= 0.9f;
            Projectile.rotation += Main.rand.NextFloat(0.2f);
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            for (int i = 0; i <= 32; i++) {
                Dust dust1 = Dust.NewDustDirect(target.position, target.width, target.height, DustID.MushroomSpray, 1f, 1f, 100, Color.White, 1.5f);
                dust1.velocity *= 1;
                dust1.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft) {
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 1f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MushRoomProj").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            Main.EntitySpriteDraw(texture, drawPos, null, lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
