using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;

namespace GloryofGuardian.Content.Dusts
{
    public class FireLightDust2 : ModDust
    {
        public override string Texture => GOGConstant.Dusts + "Dust1";

        public override void OnSpawn(Dust dust) {
            dust.fadeIn = 0;//可以用作计时器
            dust.scale *= Main.rand.NextFloat(0.8f, 1.5f);
        }

        public override bool Update(Dust dust) {
            dust.fadeIn++;
            dust.color *= 0.01f;
            if (dust.fadeIn > 5) {
                dust.active = false;
            }
            dust.position += dust.velocity;
            //dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            return false;
        }

        public override bool PreDraw(Dust dust) {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color drawColor = new Color(86, 29, 123, 120);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(texture, dust.position - Main.screenPosition, null, drawColor, 0, texture.Size() / 2, 0.08f * dust.scale, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
