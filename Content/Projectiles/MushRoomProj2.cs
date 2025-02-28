using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class MushRoomProj2 : GOGProj
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
            if (count >= 120) Projectile.Kill();

            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            Projectile.alpha -= 30;
            if (count >= 60) Projectile.alpha += 33;

            Projectile.velocity *= 0.9f;
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
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MushRoomProj2").Value;
            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);

            float aiTracker = Projectile.ai[0];
            float trackerClamp = MathHelper.Clamp(aiTracker / 30f, 0f, 1f);
            if (aiTracker > 540f) {
                trackerClamp = MathHelper.Lerp(1f, 0f, (aiTracker - 540f) / 60f);
            }
            Point centerPoint = Projectile.Center.ToTileCoordinates();
            int sizeModding;
            int sizeModding2;
            Collision.ExpandVertically(centerPoint.X, centerPoint.Y, out sizeModding, out sizeModding2, 15, 15);
            sizeModding++;
            sizeModding2--;
            float vectorMult = 0.2f;
            Vector2 sizeModdingVector = new Vector2(centerPoint.X, sizeModding) * 16f + new Vector2(8f);
            Vector2 sizeModdingVector2 = new Vector2(centerPoint.X, sizeModding2) * 16f + new Vector2(8f);
            Vector2.Lerp(sizeModdingVector, sizeModdingVector2, 0.5f);
            Vector2 sizeModdingPos = new Vector2(0f, sizeModdingVector2.Y - sizeModdingVector.Y);
            sizeModdingPos.X = sizeModdingPos.Y * vectorMult;
            new Vector2(sizeModdingVector.X - sizeModdingPos.X / 2f, sizeModdingVector.Y);
            Texture2D texture2D23 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle drawRectangle = texture2D23.Frame(1, 1, 0, 0);
            Vector2 smallRect = drawRectangle.Size() / 2f;
            float aiTrackMult = -0.06283186f * aiTracker;
            Vector2 spinningpoint2 = Vector2.UnitY.RotatedBy((double)(aiTracker * 0.1f), default);
            float incrementStorage = 0f;
            float increment = 5.1f;
            Color sandYellow = new Color(225, 225, 100);
            for (float k = (int)sizeModdingVector2.Y; k > (int)sizeModdingVector.Y; k -= increment) {
                incrementStorage += increment;
                float colorChanger = incrementStorage / sizeModdingPos.Y;
                float incStorageMult = incrementStorage * 6.28318548f / -20f;
                float lowerColorChanger = colorChanger - 0.15f;
                Vector2 spinArea = spinningpoint2.RotatedBy((double)incStorageMult, default);
                Vector2 colorChangeVector = new Vector2(0f, colorChanger + 1f);
                colorChangeVector.X = colorChangeVector.Y * vectorMult;
                Color newSandYellow = Color.Lerp(Color.Transparent, sandYellow, colorChanger * 2f);
                if (colorChanger > 0.5f) {
                    newSandYellow = Color.Lerp(Color.Transparent, sandYellow, 2f - colorChanger * 2f);
                }
                newSandYellow.A = (byte)(newSandYellow.A * 0.5f);
                newSandYellow *= trackerClamp;
                spinArea *= colorChangeVector * 100f;
                spinArea.Y = 0f;
                spinArea.X = 0f;
                spinArea += new Vector2(sizeModdingVector2.X, k) - Main.screenPosition;
                Main.EntitySpriteDraw(texture2D23, spinArea, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), newSandYellow, aiTrackMult + incStorageMult, smallRect, 1f + lowerColorChanger, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
