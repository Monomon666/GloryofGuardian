using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class AncientBattleProj2 : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + "Wind";

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            windnum = 15;
            windnum = 5;
        }

        int count = 0;
        float count0 = 0;
        int windnum = 0;
        int mode = 0;
        public override void AI() {
            count++;
            count0++;

            if (windnum <= 15 && count % 10 == 0) {
                if (windnum == 15) count = 0;
                windnum += 1;
            }

            if (windnum == 12 && count > 60) Projectile.velocity = new Vector2(8, 8);

            float lifeSpan = 900f;
            if (Projectile.soundDelay == 0) {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
            }

            if (count0 >= lifeSpan) {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] >= 30f) {
                Projectile.damage = 0;
                if (count0 < lifeSpan - 120f) {
                    float aiDecrement = count0 % 60f;
                    count0 = lifeSpan - 120f + aiDecrement;
                    Projectile.netUpdate = true;
                }
            }
            Point point8 = Projectile.Center.ToTileCoordinates();
            int sizeMod;
            int sizeMod2;
            Collision.ExpandVertically(point8.X, point8.Y, out sizeMod, out sizeMod2, windnum, windnum);
            sizeMod++;
            sizeMod2--;
            Vector2 sizeModVector = new Vector2(point8.X, sizeMod) * 16f + new Vector2(8f);
            Vector2 sizeModVector2 = new Vector2(point8.X, sizeMod2) * 16f + new Vector2(8f);
            Vector2 centering = Vector2.Lerp(sizeModVector, sizeModVector2, 0.5f);
            Vector2 sizeModPos = new Vector2(0f, sizeModVector2.Y - sizeModVector.Y);
            sizeModPos.X = sizeModPos.Y * 0.2f;
            Projectile.width = (int)(sizeModPos.X * 0.65f);
            Projectile.height = (int)sizeModPos.Y;
            Projectile.Center = centering;
            if (Projectile.owner == Main.myPlayer) {
                bool breakFlag = false;
                Vector2 playerCenter = Main.player[Projectile.owner].Center;
                Vector2 top = Main.player[Projectile.owner].Top;
                for (float i = 0f; i < 1f; i += 0.05f) {
                    Vector2 position2 = Vector2.Lerp(sizeModVector, sizeModVector2, i);
                    if (Collision.CanHitLine(position2, 0, 0, playerCenter, 0, 0) || Collision.CanHitLine(position2, 0, 0, top, 0, 0)) {
                        breakFlag = true;
                        break;
                    }
                }
                if (!breakFlag && count0 < lifeSpan - 120f) {
                    float aiDecrement2 = count0 % 60f;
                    count0 = lifeSpan - 120f + aiDecrement2;
                    Projectile.netUpdate = true;
                }
            }
            if (count0 < lifeSpan - 120f) {
                return;
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Gold;
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            float aiTracker = count0;
            float trackerClamp = MathHelper.Clamp(aiTracker / 30f, 0f, 1f);
            if (aiTracker > 540f) {
                trackerClamp = MathHelper.Lerp(1f, 0f, (aiTracker - 540f) / 60f);
            }
            Point centerPoint = Projectile.Center.ToTileCoordinates();
            int sizeModding;
            int sizeModding2;
            Collision.ExpandVertically(centerPoint.X, centerPoint.Y, out sizeModding, out sizeModding2, windnum, windnum);
            sizeModding++;
            sizeModding2--;
            float vectorMult = 0.2f;
            Vector2 sizeModdingVector = new Vector2(centerPoint.X, sizeModding) * 16f + new Vector2(8f);
            Vector2 sizeModdingVector2 = new Vector2(centerPoint.X, sizeModding2) * 16f + new Vector2(8f);
            Vector2.Lerp(sizeModdingVector, sizeModdingVector2, 0.5f);
            Vector2 sizeModdingPos = new Vector2(0f, sizeModdingVector2.Y - sizeModdingVector.Y);
            sizeModdingPos.X = sizeModdingPos.Y * vectorMult;
            new Vector2(sizeModdingVector.X - sizeModdingPos.X / 2f, sizeModdingVector.Y);
            Texture2D texture2D23 = ModContent.Request<Texture2D>(Texture).Value;
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
