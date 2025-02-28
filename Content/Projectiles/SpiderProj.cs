using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SpiderProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 10;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;//穿透数，1为攻击到第一个敌人就消失

            Projectile.alpha = 0;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            Projectile.tileCollide = true;
        }

        int count = 0;
        public override void AI() {
            count++;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        }

        public override void OnKill(int timeLeft) {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig);
            //基本爆炸粒子
            for (int i = 0; i < 3; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Wraith, 0f, 0f, 50, Color.White, 1f);
                Main.dust[num].velocity = Projectile.velocity * 0.1f;
                Main.dust[num].scale = 0.5f;
                Main.dust[num].noGravity = true;
                Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SpiderProj").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            Main.EntitySpriteDraw(texture, drawPos, null, lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
