using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;

namespace GloryofGuardian.Content.Dusts {
    public class Todo : ModDust {
        public override string Texture => GOGConstant.nulls;

        public override bool Update(Dust dust) {
            //dust.fadeIn++;//粒子的计时器
            //dust.color *= 0.01f;
            //if (dust.fadeIn > 5) {
            //    dust.active = false;
            //}
            //dust.rotation += MathHelper.PiOver2;
            //dust.position += dust.velocity;
            ////dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            return false;
        }

        public override bool PreDraw(Dust dust) {
            //Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Dusts + Name).Value;
            //Color drawColor = new Color(255, 50, 50, 120);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            //Main.EntitySpriteDraw(texture, dust.position - Main.screenPosition, null, drawColor, 0, texture.Size() / 2, 0.08f * dust.scale, 0, 0f);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
