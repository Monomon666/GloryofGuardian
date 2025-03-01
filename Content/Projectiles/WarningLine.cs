﻿using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class WarningLine : ModProjectile // 预警线
    {
        public override string Texture => GOGConstant.Projectiles + Name;
        public override void SetStaticDefaults() {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//这一项代表弹幕超过屏幕外多少距离以内可以绘制
                                                                //用于长条形弹幕绘制
                                                                //激光弹幕建议4000左右
            base.SetStaticDefaults();
        }
        public override void SetDefaults() {
            Projectile.tileCollide = false; Projectile.ignoreWater = true;//穿墙并且不受水影响
            Projectile.width = Projectile.height = 4;//装饰性弹幕宽高随便写个小一点的数
            Projectile.timeLeft = 40;//预警线持续40帧，是BOSS完成蓄力的时间
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            if (Owner.active && Owner.PressKey())//确认NPC是不是我们的那个BOSS
            {
                //Vector2 firepos = 
                Projectile.Center = Owner.Center;//固定在NPC身上
                //Projectile.rotation = npc.rotation;//方向渐变到和NPC相同，营造一种渐入感
            } else {
                Projectile.active = false;//其他的情况不符合正常情况，直接舍去，灭活该弹幕。
            }

            //松手则消除
            if (!Owner.PressKey()) {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            ////这次我们做一个简单点的预警线，也就是蓝白色来回闪动
            //float factor = (1 + (float)Math.Sin(Projectile.timeLeft / 3f));//制作一个0~2波浪摆动的正弦函数
            //factor /= 2f;//除以二，也就是归一化
            //
            //Color color1 = Color.Lerp(Color.White, Color.DeepSkyBlue, factor);
            ////lerp是线性插值，可以根据第三个空代表的比率(0~1)，选择第一个和第二个参数之间指定比率的中间值
            //
            //var tex = TextureAssets.MagicPixel.Value;//这个东西是一个白色像素
            //Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition,
            //    new Rectangle(0, 0, 2, 2),//我们选取2x2的大小
            //    color1, Projectile.velocity.ToRotation()//以速度方向代表预警线方向
            //    , Vector2.Zero, new Vector2(1500, 1)//X轴拉长1500倍
            //    , SpriteEffects.None, 0);
            //return false;//return false阻止自动绘制

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