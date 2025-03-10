using GloryofGuardian.Common;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class PlanteraProj0 : GOGProj
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
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;//是否使用本地
            Projectile.localNPCHitCooldown = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = false;

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
        //重力
        bool drop = true;
        public override void AI() {
            count++;

            Drop();

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, 0f, 0f, 50, Color.White, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 10 == 0) {
                //int num = count / 10;

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, 16), new Vector2(0, -8), ModContent.ProjectileType<PlanteraProj1>(), Projectile.damage, 1, Owner.whoAmI);
                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
                    }
                }
            }

            if (Projectile.ai[0] == 1 && !Owner.ZoneJungle && count > 20) Projectile.Kill();
            if (Projectile.ai[0] == 1 && Owner.ZoneJungle && count > 30) Projectile.Kill();

            if (!Owner.ZoneJungle && count > 60) Projectile.Kill();
            if (Owner.ZoneJungle && count > 120) Projectile.Kill();
        }

        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 1) * 16);
                        break;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
        }

        public override bool PreDraw(ref Color lightColor) {
            //Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj01").Value;
            //Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj02").Value;
            //
            //Main.EntitySpriteDraw(
            //        texture,
            //        Projectile.Center - Main.screenPosition,
            //        null,
            //        lightColor,
            //        Projectile.rotation + MathHelper.PiOver2,
            //        texture.Size() / 2,
            //        Projectile.scale,
            //        SpriteEffects.None,
            //        0);
            //
            return false;
        }
    }
}
