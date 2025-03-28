using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Graphics.Effects;

namespace GloryofGuardian.Skies
{
    public class LightPinkSky : CustomSky //霓虹灯天空
    {
        public int Timeleft = 100; //弄一个计时器，让天空能自己消失

        int count = 0;
        public override void Update(GameTime gameTime)//天空激活时的每帧更新函数
        {
            count++;
            if (!Main.gamePaused)//游戏暂停时不执行
            {
                if (Timeleft > 0) Timeleft--;//只要激活时就会减少，这样就会在外部没赋值时自己消失了
                else
                {
                    if (SkyManager.Instance["NeonSky"].IsActive())
                    {
                        SkyManager.Instance.Deactivate("NeonSky");//消失
                    }
                }
            }
        }

        NPC master;
        float skycount0 = 0;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth) {
            skycount0 = GloryofGuardianMod.Instance.skycount;

            if (skycount0 <= 1) {
                Texture2D sky = ModContent.Request<Texture2D>("GloryofGuardian/Skies/SkyEffect").Value;
                spriteBatch.Draw(sky, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(233, 33, 89, 255) * (1 - skycount0));
            }

            if (skycount0 > 1) {
                skycount0 = 1;
            }
            //把一条带状的图片填满屏幕
        }

        #region 废弃绘制(请无视)
        void DrawNet(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0,
       Main.screenWidth, Main.screenHeight), Color.Gray);
            for (int i = 0; i < 5; i++)
            {
                float length = (i * Main.screenWidth / 5f + Main.LocalPlayer.Center.X) % Main.screenWidth;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(Main.screenWidth - length, 0),
                   new Rectangle(0, 0, 1200, 2), Color.LawnGreen, 1.5707f, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            }
            for (int i = 0; i < 3; i++)
            {
                float length = (i * Main.screenWidth / 3f + Main.LocalPlayer.Center.Y + Main.GameUpdateCount * 15) % Main.screenHeight;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(0, Main.screenHeight - length),
                    new Rectangle(0, 0, 2200, 2), Color.LawnGreen, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            }
        }
        #endregion
        public override float GetCloudAlpha()
        {
            return 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {

        }

        public override void Deactivate(params object[] args)
        {

        }

        public override void Reset()
        {

        }

        public override bool IsActive()
        {
            return Timeleft > 0;
        }

    }

}