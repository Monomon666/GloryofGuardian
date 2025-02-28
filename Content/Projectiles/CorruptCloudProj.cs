using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class CorruptCloudProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 6;//动图帧数
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 54;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;//穿透数，1为攻击到第一个敌人就消失

            Projectile.alpha = 0;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            if (Projectile.ai[0] == 1) Projectile.scale *= 1.5f;
            Projectile.frame += Main.rand.Next(6);
        }

        int count = 0;
        public override void AI() {
            count++;
            if (Projectile.ai[0] == 0) {
                if (count >= 90) Projectile.Kill();
                if (count >= 60) Projectile.alpha += 10;
            }
            if (Projectile.ai[0] == 1) {
                if (count >= 120) Projectile.Kill();
                if (count >= 90) Projectile.alpha += 10;
            }
            if (count % 15 == 0) {
                int randx = Main.rand.Next(-6, 6);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(randx, 6), new Vector2(0, 12), ModContent.ProjectileType<CorruptRainProj>(), Projectile.damage, 1, Owner.whoAmI);
                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
                    }
                }
            }

            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 6)
                % 6;//要手动填，不然会出错
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {

        }

        public override void OnKill(int timeLeft) {
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CorruptCloudProj").Value;
            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            //Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY), lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(27, 23), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(
                texture,
                drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                new Vector2(27, 23),
                Projectile.scale,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
