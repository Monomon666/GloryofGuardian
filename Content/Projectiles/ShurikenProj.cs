using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ShurikenProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 24;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;//穿透数，1为攻击到第一个敌人就消失

            Projectile.alpha = 255;

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
            if (count >= 120) Projectile.Kill();

            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            Projectile.alpha -= 30;
            if (count >= 60) Projectile.alpha += 33;
            Projectile.light = 2 * (255 - Projectile.alpha) / 255f;
            Projectile.scale = 0.5f + 0.5f * (255 - Projectile.alpha) / 255f;

            if (count <= 12) Projectile.velocity *= 1.1f;
            Projectile.rotation += Main.rand.NextFloat(1f);

            if (Projectile.ai[0] == 1 && Main.rand.NextBool(3)) {
                for (int i = 0; i <= 1; i++) {
                    Dust dust1 = Dust.NewDustDirect(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.ChlorophyteWeapon, 1f, 1f, 100, Color.Green, 1f);
                    dust1.velocity = Projectile.velocity * 0f;
                    dust1.noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] == 0) {
                for (int i = 0; i <= 4; i++) {
                    Dust dust1 = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Wraith, 1f, 1f, 100, Color.Black, 1f);
                    dust1.velocity = Projectile.velocity * 0.1f;
                    dust1.noGravity = true;
                }
            }
            if (Projectile.ai[0] == 1) {
                for (int i = 0; i <= 4; i++) {
                    Dust dust1 = Dust.NewDustDirect(target.position, target.width, target.height, DustID.PureSpray, 1f, 1f, 100, Color.Green, 1f);
                    dust1.velocity = Projectile.velocity * 0.1f;
                    dust1.noGravity = true;
                }
                target.AddBuff(BuffID.Poisoned, 300);
                target.AddBuff(ModContent.BuffType<ShadowbladeDebuff>(), 300);
                if (!target.HasBuff(BuffID.Poisoned)) Projectile.ai[0] = 0;
            }
            if (!target.HasBuff(ModContent.BuffType<ShadowbladeDebuff>())) Projectile.Kill();
            if (target.HasBuff(ModContent.BuffType<ShadowbladeDebuff>())) {
                Projectile.penetrate += 1;
            }
        }

        public override void OnKill(int timeLeft) {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
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
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenProj").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            Main.EntitySpriteDraw(texture, drawPos, null, lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
