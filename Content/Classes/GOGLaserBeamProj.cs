using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.Enums;
using Terraria.ID;

namespace GloryofGuardian.Content.Class
{
    public abstract class GOGLaserBeamProj : ModProjectile
    {
        #region Auto-Properties
        public float RotationalSpeed {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Time {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float LaserLength {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        #endregion

        #region Virtual Methods

        /// <summary>
        /// 处理激光的AI
        /// </summary>
        public virtual void Behavior() {
            // 附着
            AttachToSomething();

            Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY);

            Time++;
            if (Time >= Lifetime) {
                Projectile.Kill();
                return;
            }

            DetermineScale();

            UpdateLaserMotion();

            float idealLaserLength = DetermineLaserLength();
            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f); // 延长速度

            if (LightCastColor != Color.Transparent) {
                DelegateMethods.v3_1 = LightCastColor.ToVector3();
                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.width * Projectile.scale, DelegateMethods.CastLight);
            }
        }

        /// <summary>
        /// 处理激光的运动逻辑
        /// </summary>
        public virtual void UpdateLaserMotion() {
            float updatedVelocityDirection = Projectile.velocity.ToRotation() + RotationalSpeed;
            Projectile.rotation = updatedVelocityDirection - MathHelper.PiOver2;
            Projectile.velocity = updatedVelocityDirection.ToRotationVector2();
        }

        /// <summary>
        /// 计算激光比例尺
        /// </summary>
        public virtual void DetermineScale() {
            Projectile.scale = (float)Math.Sin(Time / Lifetime * MathHelper.Pi) * ScaleExpandRate * MaxScale;
            if (Projectile.scale > MaxScale)
                Projectile.scale = MaxScale;
        }

        /// <summary>
        /// 处理附着
        /// </summary>
        public virtual void AttachToSomething() { }

        /// <summary>
        /// 处理激光长度,默认不碰撞tile
        /// </summary>
        /// <returns>激光长度是浮点数</returns>
        public virtual float DetermineLaserLength() => MaxLaserLength;

        /// <summary>
        /// PostAI.
        /// </summary>
        public virtual void ExtraBehavior() { }
        #endregion

        #region Helper Methods

        /// <summary>
        /// 与tile碰撞的激光的长度
        /// </summary>
        /// <param name="samplePointCount">计算的样本数量</param>
        public float DetermineLaserLength_CollideWithTiles(int samplePointCount) {
            float[] laserLengthSamplePoints = new float[samplePointCount];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.scale, MaxLaserLength, laserLengthSamplePoints);
            return laserLengthSamplePoints.Average();
        }

        protected internal void DrawBeamWithColor(Color beamColor, float scale, int startFrame = 0, int middleFrame = 0, int endFrame = 0) {
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, startFrame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, middleFrame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, endFrame);

            Main.EntitySpriteDraw(LaserBeginTexture,
                             Projectile.Center - Main.screenPosition,
                             startFrameArea,
                             beamColor,
                             Projectile.rotation,
                             LaserBeginTexture.Size() / 2f,
                             scale,
                             SpriteEffects.None,
                             0);

            float laserBodyLength = LaserLength;
            laserBodyLength -= (startFrameArea.Height / 2 + endFrameArea.Height) * scale;
            Vector2 centerOnLaser = Projectile.Center;
            centerOnLaser += Projectile.velocity * scale * startFrameArea.Height / 2f;

            if (laserBodyLength > 0f) {
                float laserOffset = middleFrameArea.Height * scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength) {
                    Main.EntitySpriteDraw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrameArea,
                                     beamColor,
                                     Projectile.rotation,
                                     LaserMiddleTexture.Width * 0.5f * Vector2.UnitX,
                                     scale,
                                     SpriteEffects.None,
                                     0);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += Projectile.velocity * laserOffset;
                }
            }

            if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f) {
                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
                Main.EntitySpriteDraw(LaserEndTexture,
                                 laserEndCenter,
                                 endFrameArea,
                                 beamColor,
                                 Projectile.rotation,
                                 LaserEndTexture.Frame(1, 1, 0, 0).Top(),
                                 scale,
                                 SpriteEffects.None,
                                 0);
            }
        }
        #endregion

        #region Hook Overrides
        public override void AI() {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;

            Behavior();
            ExtraBehavior();
        }
        public override void CutTiles() {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackMelee;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.Size.Length() * Projectile.scale, DelegateMethods.CutTiles);
        }
        public override bool PreDraw(ref Color lightColor) {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            DrawBeamWithColor(LaserOverlayColor, Projectile.scale);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.Size.Length() * Projectile.scale, ref _);
        }

        public override bool ShouldUpdatePosition() => false;
        #endregion

        #region Overridable Properties
        public abstract float Lifetime { get; }
        public abstract float MaxScale { get; }
        public abstract float MaxLaserLength { get; } // Be careful with this. Going too high will cause lag.
        public abstract Texture2D LaserBeginTexture { get; }
        public abstract Texture2D LaserMiddleTexture { get; }
        public abstract Texture2D LaserEndTexture { get; }
        public virtual float ScaleExpandRate => 4f;
        public virtual Color LightCastColor => Color.White;
        public virtual Color LaserOverlayColor => Color.White * 0.9f;
        #endregion
    }
}
