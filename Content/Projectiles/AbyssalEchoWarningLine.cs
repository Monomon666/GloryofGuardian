using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class AbyssalEchoWarningLine : ModProjectile // 预警线
    {
        public override string Texture => GOGConstant.Projectiles + "WarningLine";
        public override void SetStaticDefaults() {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//这一项代表弹幕超过屏幕外多少距离以内可以绘制
                                                                //用于长条形弹幕绘制
                                                                //激光弹幕建议4000左右
            base.SetStaticDefaults();
        }
        public override void SetDefaults() {
            Projectile.tileCollide = false; Projectile.ignoreWater = true;//穿墙并且不受水影响
            Projectile.width = Projectile.height = 4;//装饰性弹幕宽高随便写个小一点的数
            Projectile.timeLeft = 60;//预警线持续40帧，是BOSS完成蓄力的时间
        }
        Player Owner => Main.player[Projectile.owner];

        int count = 0;
        public override void AI() {
            if (count == 0) {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.velocity *= 0;
            }

            count++;
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            float n = Projectile.timeLeft / 40f;//把剩余时间归一化便于乘以颜色
            //要想将灰度图的黑底去掉，不仅可以采用颜色A值赋0的方法，也可以直接开启加算混合模式,具体如下，可以照抄
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.Additive,//重点是第二个参数的这个Additive，将图片的RGB值直接加算到屏幕上
                SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);
            Texture2D line = TextureAssets.Projectile[Type].Value;//获取到弹幕材质
            Main.EntitySpriteDraw(line, Projectile.Center - Main.screenPosition, null,
                Color.Red * n,//塑造一种渐渐减淡的效果
                Projectile.rotation,
                new Vector2(0, line.Height / 2),//向右无限拉长，所以原点选在左侧中点
                new Vector2(100, 1),//X长度拉长100倍
                SpriteEffects.None, 0);
            //别忘了把绘制模式弄回来
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,//Alphablend是默认的混合模式
                SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);
            return false;//return false阻止自动绘制
        }
    }
}