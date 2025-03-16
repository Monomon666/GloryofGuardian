using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class NanoUAVProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

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
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int mode = 0;
        public override void AI() {
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation();

            //过载带电子弹
            if (Projectile.ai[0] == 1) {
                for (int i = 0; i < 1; i++) {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 0.5f);
                    dust.noGravity = true;
                    dust.velocity *= 1f;
                    dust.velocity -= Projectile.velocity * 0.4f;
                }
            }

            //带电子弹
            if (Projectile.ai[0] == 2) {
                // 随机改变方向，模拟闪电轨迹
                if (Main.rand.NextBool(3)) // 每帧有 1/3 的概率改变方向
                {
                    // 限制方向改变的角度范围
                    float maxAngle = MathHelper.PiOver2; // 最大偏转角度（90度）
                    float angle = Main.rand.NextFloat(-maxAngle, maxAngle); // 随机角度

                    // 限制方向改变的范围，确保弹幕整体趋势是向下的
                    Vector2 newVelocity = Projectile.velocity.RotatedBy(angle);
                    if (newVelocity.Y > 0) // 确保新速度的 Y 分量是向下的
                    {
                        Projectile.velocity = newVelocity;
                    }
                }

                // 提速降低性能消耗
                if (count % 300 == 0) Projectile.velocity *= 1.5f;
                if (count >= 1500) Projectile.Kill();

                // 生成粒子效果
                for (int i = 0; i < 2; i++) {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 0.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                    dust.velocity += Projectile.velocity * 0.1f;
                    dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            //产生子弹
            if (Projectile.ai[0] == 1) {
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NanoUAVProj>(), Projectile.damage, 2, Owner.whoAmI, 2);
                proj1.penetrate = -1;
                proj1.extraUpdates = 40;
                proj1.velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(0.5f, 1f)).SafeNormalize(Vector2.UnitY) * 2f;
            }

            if (!target.boss) target.velocity *= 0;
            if (target.boss) target.velocity *= 0.96f;

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor) {
            if (Projectile.ai[0] == 2) return false;

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoUAVProj").Value;

            Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);

            return false;
        }
    }
}
