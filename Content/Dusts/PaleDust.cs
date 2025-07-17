using System;
using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;

namespace GloryofGuardian.Content.Dusts {
    public class PaleDust : ModDust {
        public override string Texture => GOGConstant.nulls;

        Vector2 dustscale = Vector2.Zero;

        //白色条形弹幕,会随速度拉伸
        public override bool Update(Dust dust) {
            dust.fadeIn++;//粒子的计时器,为配合

            dust.position += dust.velocity;
            dust.rotation = dust.velocity.ToRotation();
            dust.velocity *= 0.92f;
            if (dust.velocity.Length() < 0.6f) dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Dusts + Name).Value;

            float angle = (float)Math.Atan2(dust.velocity.Y, dust.velocity.X);
            float speed = dust.velocity.Length();
            float stretchFactor = speed * 2f;//比例
            float stretchFactor2 = 1f + speed * 1f;//比例
            Vector2 dustscale2 = new Vector2(stretchFactor, 0.6f);//粗细

            Color drawColor = new Color(255, 255, 255, 255);
            Main.EntitySpriteDraw(texture, dust.position - Main.screenPosition, null, 
                drawColor * ((255 - dust.alpha) / 255f)
                , dust.rotation, texture.Size() / 2,
                dustscale2 * dust.scale,
                0, 0f);
            return false;
        }
    }
}
