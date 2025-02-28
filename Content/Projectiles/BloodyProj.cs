using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GloryofGuardian.Content.Projectiles
{
    public class BloodyProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
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
            Projectile.penetrate = 2;//穿透数，1为攻击到第一个敌人就消失

            Projectile.scale *= 1.2f;

            Projectile.hide = true;//触发Drawbehind，来使贴图隐藏在某些图层之后
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            if (Projectile.ai[1] > 1) {
                Projectile.extraUpdates += 1;

                Projectile.scale *= 2;
                Projectile.width *= 2;
                Projectile.height *= 2;
            }
        }

        int count = 0;
        public override void AI() {
            count++;

            float G = Projectile.ai[0];

            Projectile.velocity += new Vector2(0, G);
            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.8f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0 && Projectile.ai[1] > 1) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            //过载攻击爆炸
            if (Projectile.ai[1] > 1) {
                for (int j = 0; j < 10; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10, Color.Black, 1.7f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity *= 3f;
                    num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10);
                    Main.dust[num2].velocity *= 3f;
                }
                SoundEngine.PlaySound(in SoundID.NPCHit9, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 120;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color * 0.3f, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                    //Main.spriteBatch.ResetBlendState();
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return false;
        }
    }
}
