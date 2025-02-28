using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ChlorophyteProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 5;//动图帧数
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 28;
            Projectile.height = 32;
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
            Projectile.frame += Main.rand.Next(5);
        }

        int count = 0;
        public override void AI() {
            count++;
            if (count >= 240) Projectile.Kill();

            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            Projectile.alpha -= 30;
            if (count >= 60) Projectile.alpha += 31;

            if (count <= 30) Projectile.velocity *= 0.98f;
            //Projectile.rotation += Main.rand.NextFloat(0.2f);

            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 6)
                % 5;//要手动填，不然会出错
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {

        }

        public override void OnKill(int timeLeft) {
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.ChlorophyteWeapon, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + Name).Value;
            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            //Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY), lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(27, 23), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(
                texture,
                drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                new Vector2(14, 16),
                Projectile.scale,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
