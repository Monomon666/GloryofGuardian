using GloryofGuardian.Common;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class Blood2Proj1 : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = true;

            Projectile.light = 1f;

            Projectile.scale *= 1.5f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        int projtype = 0;
        Vector2 spawnpos = new Vector2(0, 0);
        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        public override void AI() {
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (count >= 20) Projectile.tileCollide = true;

            Projectile.velocity.Y += 0.35f;
            if (Projectile.velocity.Y >= 4) {
                Projectile.velocity.Y = 8;
                Projectile.velocity.X *= 0.95f;
            }

            if (Projectile.ai[0] == 0) {
                for (int j = 0; j < 2; j++) {
                    int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = Projectile.velocity;
                }
            }

            if (Projectile.ai[0] == 1) {
                for (int j = 0; j < 2; j++) {
                    int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = Projectile.velocity;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] == 1) target.AddBuff(BuffID.Ichor, 180);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            //基本爆炸粒子
            if (Projectile.ai[0] == 0) {
                for (int i = 0; i < 12; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.DarkRed, 1f);
                    Main.dust[num].velocity *= 2f;
                    if (Main.rand.NextBool(2)) {
                        Main.dust[num].scale = 1f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }

            if (Projectile.ai[0] == 1) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Ichor, 0f, 0f, 50, Color.White, 1f);
                    if (Main.rand.NextBool(2)) {
                        Main.dust[num].scale = 1f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            return false;
        }
    }
}
