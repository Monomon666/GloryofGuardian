using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class NanoGuiderProj : GOGLaserBeamProj, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => GOGConstant.nulls;

        public bool playedSound = false;
        public const int ChargeupTime = 50;

        public Player Owner => Main.player[Projectile.owner];

        public override Color LightCastColor => new Color(204, 204, 255); //#CCCCFF
        public override float Lifetime => 18000f;
        public override float MaxScale => 0.6f;
        public override float MaxLaserLength => 1600f; //100 tiles

        private const float AimResponsiveness = 0.8f; //j激光旋转因子,越小越快

        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults() {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.timeLeft = 18000;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void DetermineScale() {
            Projectile.scale = Time < ChargeupTime ? 0f : MaxScale;
        }

        int count = 0;
        public override bool PreAI() {
            count++;
            // 客户端运行以确保多人联机同步
            if (Projectile.owner == Main.myPlayer) {
                Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
                UpdateAim(rrp);
                Projectile.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                Projectile.netUpdate = true;
            }

            int dir = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = Owner.Center + Projectile.velocity * 42f + new Vector2(0, 1);
            Projectile.timeLeft = 18000;
            Owner.ChangeDir(dir);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = ((Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * -Owner.direction).ToRotation();

            if (!Owner.channel) {
                Projectile.Kill();
                return false;
            }

            if (Time < ChargeupTime) {
                int dustCount = (int)(Time / 20f);
                Vector2 spawnPos = Projectile.Center;
                for (int k = 0; k < dustCount + 1; k++) {
                    Dust dust = Dust.NewDustDirect(spawnPos, 1, 1, DustID.Electric, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                    dust.position += Main.rand.NextVector2Square(-10f, 10f);
                    dust.velocity = Main.rand.NextVector2Unit() * (10f - dustCount * 2f) / 10f;
                    dust.scale = Main.rand.NextFloat(0.5f, 1f);
                    dust.noGravity = true;
                }
                Time++;
                return false;
            }

            if (!playedSound) {
                SoundEngine.PlaySound(SoundID.Item68, Projectile.Center);
                playedSound = true;
            }

            return true;
        }

        public override void PostAI() {
            // 确定帧
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 4f)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        private void UpdateAim(Vector2 source) {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), AimResponsiveness));

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        public override bool ShouldUpdatePosition() => false;

        //蓄力检测
        public override bool? CanDamage() => Time >= ChargeupTime;

        // Update CutTiles so the laser will cut tiles (like grass).
        public override void CutTiles() {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width + 16, DelegateMethods.CutTiles);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(ModContent.BuffType<NanoMarkDebuff1>(), 60);
        }

        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SparklingLaserBegin").Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SparklingLaserMid").Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SparklingLaserEnd").Value;

        public override bool PreDraw(ref Color lightColor) {
            if (Projectile.velocity == Vector2.Zero || Time < ChargeupTime)
                return false;

            Color beamColor = Color.White * 0.9f;
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            Vector2 laserBeginCenter = Projectile.Center - Main.screenPosition + Projectile.velocity * Projectile.scale * 50f;
            Main.EntitySpriteDraw(LaserBeginTexture,
                             laserBeginCenter,
                             startFrameArea,
                             beamColor,
                             Projectile.rotation,
                             LaserBeginTexture.Size() / 2f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);

            float laserBodyLength = LaserLength;
            laserBodyLength -= (startFrameArea.Height / 2 + endFrameArea.Height) * Projectile.scale;
            Vector2 centerOnLaser = Projectile.Center - Main.screenPosition;
            centerOnLaser += Projectile.velocity * Projectile.scale * 10.5f;

            if (laserBodyLength > 0f) {
                float laserOffset = middleFrameArea.Height * Projectile.scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength) {
                    centerOnLaser += Projectile.velocity * laserOffset;
                    incrementalBodyLength += laserOffset;
                    Main.EntitySpriteDraw(LaserMiddleTexture,
                                     centerOnLaser,
                                     middleFrameArea,
                                     beamColor,
                                     Projectile.rotation,
                                     LaserMiddleTexture.Width * 0.5f * Vector2.UnitX,
                                     Projectile.scale,
                                     SpriteEffects.None,
                                     0);
                }
            }

            if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f) {
                Main.EntitySpriteDraw(LaserEndTexture,
                                 centerOnLaser,
                                 endFrameArea,
                                 beamColor,
                                 Projectile.rotation,
                                 LaserEndTexture.Frame(1, 1, 0, 0).Top(),
                                 Projectile.scale,
                                 SpriteEffects.None,
                                 0);
            }
            return false;
        }

        //激光的计算方法
        public override float DetermineLaserLength() {
            return DetermineLaserLength_CollideWithTiles(12);
        }
    }
}
