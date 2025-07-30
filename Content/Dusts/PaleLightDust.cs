using System;
using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;

namespace GloryofGuardian.Content.Dusts {
    public class PaleLightDust : ModDust //这是一个RT粒子的范例
    {
        public override string Texture => GOGConstant.nulls;
        public override void OnSpawn(Dust dust)//粒子生成时执行的函数
        {
            dust.noLight = true;//粒子无光
            dust.noGravity = true;//粒子无重力
            base.OnSpawn(dust);
        }
        public override bool Update(Dust dust)//ban了原版的粒子更新，用自己的
        {
            dust.position += dust.velocity;
            dust.velocity = dust.velocity.RotatedByRandom(0.1f);//速度方向每帧随机变换一点点
            
            dust.velocity *= 0.97f;//速度不断降低
            dust.scale -= 0.05f;//大小不断减小
            if (dust.scale > 1f) dust.scale -= 0.05f;//大小不断减小
            if (dust.velocity == Vector2.Zero) dust.scale -= 0.1f;//大小不断减小
            if (dust.velocity != Vector2.Zero) dust.velocity += new Vector2(0, -0.02f);
            if (dust.scale <= 0)//如果大小低于0就杀掉
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Effects + "CircleLS").Value;

            Color drawColor = new Color(255, 255, 255, 0);
            drawColor = dust.color;
            Main.EntitySpriteDraw(texture, dust.position - Main.screenPosition, null,
                drawColor * ((255 - dust.alpha) / 255f)
                , dust.rotation, texture.Size() / 2,
                dust.scale * 0.01f,
                0, 0f);
            return false;
        }
    }
}