using GloryofGuardian.Common;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class Corrupt2Proj1 : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = true;

            Projectile.light = 1f;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        int projtype = 0;
        Vector2 spawnpos = new Vector2(0, 0);
        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int height0 = 0;
        public override void AI() {
            count++;

            height0 = (int)(180 - 20 * Projectile.ai[1]);
            Projectile.velocity *= 0;

            float dying = count / 20f;

            if (Projectile.ai[0] == 0) {
                if (count % 1 == 0) {
                    for (int i = 0; i < 4; i++) {
                        int num = Dust.NewDust(Projectile.position + new Vector2(0, Projectile.height - height0 * dying - 16), 16, 32, DustID.CorruptGibs, 0f, 0f, 50, Color.White, 0.5f);
                        Main.dust[num].velocity = new Vector2(0, -8);
                        Main.dust[num].noGravity = true;
                    }
                }

                if (count % 1 == 0) {
                    for (int i = 0; i < 1; i++) {
                        int num = Dust.NewDust(Projectile.position + new Vector2(0, Projectile.height - height0 * dying - 16), 16, 32, DustID.CorruptGibs, 0f, 0f, 50, Color.White, 1.5f);
                        Main.dust[num].velocity = new Vector2(0, -2);
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            if (Projectile.ai[0] == 1) {
                if (count % 1 == 0) {
                    for (int i = 0; i < 4; i++) {
                        int num = Dust.NewDust(Projectile.position + new Vector2(0, Projectile.height - height0 * dying - 16), 16, 32, DustID.GreenFairy, 0f, 0f, 50, Color.Yellow, 1f);
                        Main.dust[num].velocity = new Vector2(0, -4);
                        Main.dust[num].noGravity = true;
                    }
                }

                if (count % 1 == 0) {
                    for (int i = 0; i < 1; i++) {
                        int num = Dust.NewDust(Projectile.position + new Vector2(0, Projectile.height - height0 * dying - 16), 16, 32, DustID.CursedTorch, 0f, 0f, 50, Color.White, 2f);
                        Main.dust[num].velocity = new Vector2(0, -2);
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            if (count > 20) Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (target.knockBackResist > 0) target.velocity *= 0;
            if (target.knockBackResist > 0) target.velocity += new Vector2(0, -height0 / 40f);
            if (target.knockBackResist > 0) target.velocity += new Vector2(Projectile.Center.Toz(target.Center).X * 24f, 0);

            if (Projectile.ai[0] == 1) target.AddBuff(BuffID.CursedInferno, 180);

            Projectile.damage = (int)(Projectile.damage * 2 / 3f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            Rectangle mergedCollisionArea = new Rectangle(
                (int)Projectile.position.X, // 左上角X坐标
                (int)Projectile.position.Y + Projectile.height - height0, // 左上角Y坐标 + 弹幕高度 - 80像素
                Projectile.width, // 宽度
                Projectile.height + 80 // 高度（弹幕高度 + 80像素）
            );

            // 检测目标是否在合并后的碰撞区域内
            if (targetHitbox.Intersects(mergedCollisionArea)) {
                return true;
            }

            // 如果不在碰撞区域内，返回false
            return false;

            //return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnKill(int timeLeft) {
        }

        public override bool PreDraw(ref Color lightColor) {
            return false;
        }
    }
}
