using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ShadowFlameScrollDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 44;
            Projectile.height = 32;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;

            //本体具有伤害的炮塔，需要设置无敌帧
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.light = 3f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 240;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int Acount = 0;
        int lastdamage = 0;
        int atknum = 0;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();

            foreach (Projectile proj in Main.ActiveProjectiles) {
                // 检测距离是否小于 160，并且 proj 不属于 GOGDT 类
                if (Vector2.Distance(proj.Center, Projectile.Center) < 160 && !(proj.ModProjectile is GOGDT)) {
                    // 获取 GlobalProjectile 实例
                    GOGGlobalProj globalProj = proj.GetGlobalProjectile<GOGGlobalProj>();

                    // 如果 ShadowAngleOffset 为 0，初始化一个随机角度
                    if (globalProj.ShadowAngleOffset == 0) {
                        // 生成一个 -0.3 到 0.3 之间的随机角度
                        float a = Main.rand.NextFloat(-0.2f, -0.05f);
                        float b = Main.rand.NextFloat(0.05f, 0.2f);
                        globalProj.ShadowAngleOffset = Main.rand.NextBool(2) ? a : b;
                    }

                    // 获取当前 proj 的速度
                    Vector2 velocity = proj.velocity;

                    // 获取角度偏移值
                    float angleOffset = globalProj.ShadowAngleOffset;

                    // 应用角度偏移
                    velocity = velocity.RotatedBy(angleOffset);

                    // 更新 proj 的速度
                    proj.velocity = velocity;
                } else {
                    // 如果 proj 离开了范围，重置 ShadowAngleOffset
                    GOGGlobalProj globalProj = proj.GetGlobalProjectile<GOGGlobalProj>();
                    globalProj.ShadowAngleOffset = 0f;
                }
            }

            base.AI();
        }

        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 2) * 16);
                        break;
                    }
                }
                drop = false;
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return false;
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        //浮动因子
        int floatcount = 0;
        float float1 = 0;
        float float2 = 0;
        float float3 = 0;
        float float4 = 0;
        float ringscale = 0;
        float ringlastscale = 0;
        Vector2 DTcenter = Vector2.Zero;
        //向绘制的数据传输
        bool findplayer = false;
        public override bool PreDraw(ref Color lightColor) {
            floatcount++;

            if (floatcount == 0) ringscale = 0.02f;
            if (floatcount >= 0 && floatcount <= 120) ringscale += 0.002f;
            if (floatcount >= 120 && floatcount <= 240) ringscale += 0.001f;
            if (floatcount >= 240 && floatcount <= 360) ringscale += 0.0003f;
            if (floatcount >= 360 && ringscale < 0.4f) ringscale += 0.0001f;

            ringlastscale = 0.5f;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShadowFlameScrollDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -18);

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000").Value;
            Texture2D textures2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000P").Value;
            DTcenter = drawPosition0 + new Vector2(-0, -0) + new Vector2(0, -4 * float1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(textures, DTcenter, null, new Color(177, 60, 255) * ringscale, Projectile.rotation, textures.Size() * 0.5f, Projectile.scale * 0.1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textures2, DTcenter, null, new Color(177, 60, 255) * ringscale, Projectile.rotation, textures2.Size() * 0.5f, Projectile.scale * 0.1f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}