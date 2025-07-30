//global using InnoVault;
global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.ModLoader;
using System;
using GloryofGuardian.Content.Dusts;
using GloryofGuardian.Content.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace GloryofGuardian {
    public class GloryofGuardianMod : Mod {
        internal static GloryofGuardianMod Instance;

        public override void Load()//模组加载时执行
        {
            //接下来讲RT2D的实现
            On_Main.LoadWorlds += On_Main_LoadWorlds;//加载世界
            Main.OnResolutionChanged += Main_OnResolutionChanged;//分辨率更改
            On_FilterManager.EndCapture += On_FilterManager_EndCapture;//原版滤镜处理结束后的函数，RT绘制插在这里

            //load时读取shader
            RT2DMask = ModContent.Request<Effect>("GloryofGuardian/Content/Effects/RT2DMask", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;//图片覆盖
            RT2DCircleLS = ModContent.Request<Effect>("GloryofGuardian/Content/Effects/RT2DCircleLS", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;//圆形光源现形
            DistortShader = ModContent.Request<Effect>("GloryofGuardian/Content/Effects/DistortShader",
                ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            base.Load();
        }
        #region RT2D
        Effect RT2DMask;
        Effect RT2DCircleLS;
        Effect DistortShader;

        RenderTarget2D render;//定义一个RT2D实例
        RenderTarget2D render2;

        private void CreateRender()//定义一个创建rt画布的函数，因为我们的render一开始是null，需要进行赋值，这段会在后面用到
        {
            if (render == null) {
                GraphicsDevice g = Main.instance.GraphicsDevice;
                render = new RenderTarget2D(Main.graphics.GraphicsDevice, g.PresentationParameters.BackBufferWidth,
                    g.PresentationParameters.BackBufferHeight, false, g.PresentationParameters.BackBufferFormat, 0);
            }

            if (render2 == null) {
                GraphicsDevice g = Main.instance.GraphicsDevice;
                render2 = new RenderTarget2D(Main.graphics.GraphicsDevice, g.PresentationParameters.BackBufferWidth,
                    g.PresentationParameters.BackBufferHeight, false, g.PresentationParameters.BackBufferFormat, 0);
            }
        }
        public static int GapEffectProj = 0;//定义一个弹幕的索引，我们的特效弹幕(RenderGap弹幕)把索引传入这里
        private void On_FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor) {
            //RT2D
            GraphicsDevice gd = Main.instance.GraphicsDevice;//把这一长串用gd代替，方便写
            SpriteBatch sb = Main.spriteBatch;//这个同理

            //在介绍具体写法之前，我想先讲一下RT2D的基本概念。
            //1.RenderTarget本身可以被当作图片，就相当于你在画一个屏幕，同时也可以传入Shader
            //2.RenderTarget和屏幕滤镜一样会被照明模式干碎
            //GraphicsDevice.SetRenderTarget可以理解为切换到指定的RenderTarget上面，之后开始绘制就会在这上面绘制了
            //原版自带一个备用RenderTarget(也就是Main.screenTargetSwap)，用于储存场景
            //Main.screenTarget就是最终要画在屏幕上的Render,除非你已经进行完所有的处理要把最终的图像画在上面，否则不要在这画
            //当你SetRenderTarget(XXX)后，绘制的东西就全在这个“XXX”的RenderTarget上面
            //之后你在别的地方绘制这个RT，就会画出绘制了东西的这个RT
            //举个例子，gd.SetRenderTarget(Main.screenTargetSwap),这时候就切换到另一个RT上
            //你绘制了一些东西，然后end，这些东西是保留在这个RT上面的，可以被整体当成一个图片绘制出来

            //下面简单讲几个RT2D配合Shader制作

            //Texture2D textureuniverse = ModContent.Request<Texture2D>("GloryofGuardian/Assets/Effects/CircleLS").Value;
            Texture2D textureuniverse = ModContent.Request<Texture2D>("GloryofGuardian/Assets/Effects/Universe").Value;
            //RT2D亮度蒙版
            RT2DMaskShader(
                () => 
                {
                    Texture2D texd = ModContent.Request<Texture2D>("GloryofGuardian/Assets/Dusts/RenderDust").Value;//别把声明放到遍历里面，不然每循环一次都声明一次
                    foreach (var d in Main.dust)//遍历每个粒子
                        {
                        if (d.type == ModContent.DustType<RenderDust>() && d.active)//如果符合条件就绘制
                        {
                            sb.Draw(texd, d.position - Main.screenPosition, null, Color.White, 0, texd.Size() / 2, d.scale, SpriteEffects.None, 0);
                        }
                    }
                },
                
                //调用目标的静态方法
                () => sb.Draw(textureuniverse, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0)
                );

            //Texture2D texturecls = ModContent.Request<Texture2D>("GloryofGuardian/Assets/Effects/CircleLS").Value;
            ////RT2D圆形光源
            //if (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.MultiplayerClient) {
            //    RT2DCircleLSShader(
            //        () => sb.Draw(texturecls, Main.LocalPlayer.Center - new Vector2(texturecls.Width / 2, texturecls.Height / 2) - Main.screenPosition, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0),
            //
            //        () => 
            //        {
            //            Texture2D texd = ModContent.Request<Texture2D>("GloryofGuardian/Assets/Projectiles/ImmortalLotusProj01").Value;//别把声明放到遍历里面，不然每循环一次都声明一次
            //            foreach (var d in Main.projectile)//遍历每个粒子
            //                {
            //                if (d.type == ModContent.ProjectileType<PaleClockworkGunDT>())//如果符合条件就绘制
            //                {
            //                    sb.Draw(texd, d.position - Main.screenPosition, null, Color.White, 0, texd.Size() / 2, 1, SpriteEffects.None, 0);
            //                }
            //            }
            //        }
            //    );
            //}

            #region 扭曲特效
            ////利用RT+shader制作一个"震波"的效果
            ////原理是利用素材图的颜色代替向量，在shader里结合画了素材图的空屏幕对原屏幕进行处理
            //gd.SetRenderTarget(Main.screenTargetSwap);//切换到原版的备用RT上去
            //gd.Clear(Color.Transparent);//把这个RT清空成透明
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//开始绘制
            //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//把现在的屏幕画到备用画布上
            //sb.End();//结束，这时候备用RT上记录了目前屏幕的样子
            //
            //gd.SetRenderTarget(render);//切换到我们自己的RT
            //gd.Clear(Color.Transparent);//先把RT清空成透明
            //sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //ModProj.RenderDistort.DrawAllDistortProjectile(sb);//把所有的扭曲球弹幕绘制到我们的RT上面来
            //sb.End();//结束，这时候我们有了一张只画了扭曲素材图在上面,其余部分全是透明的"屏幕"
            //
            //
            //gd.SetRenderTarget(Main.screenTarget);//接下来我们切换到screenTarget这个Render，要做最终的绘制了
            //gd.Clear(Color.Transparent);//照样先清空掉这个Render
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);//开始绘制
            //gd.Textures[1] = render;//让Tex1赋值为我们的render，也就是只绘制了一堆扭曲图片弹幕其余什么也没有的屏幕
            //DistortShader.Parameters["Length"].SetValue(0.03f);//扭曲的程度
            //
            ////赋值好参数后再应用shader
            //DistortShader.CurrentTechnique.Passes[0].Apply();//应用我们的shader,将扭曲图片转化成扭曲,具体实现原理请参见对应fx文件
            //sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//再在shader的加持下绘制原本的屏幕，就可以将屏幕进行扭曲处理了
            //                                                          // ModProj.RenderDistort.DrawAllDistortProjectile(sb);//把所有的扭曲球弹幕绘制到我们的RT上面来
            //sb.End();//结束！大功告成
            #endregion

            #region 空间割裂
            ////有时候我们只需要一个特效弹幕，而非多个，就不要老是遍历了，直接用索引
            //Projectile projectile = Main.projectile[GapEffectProj];
            //if (projectile.type == ModContent.ProjectileType<ModProj.RenderGap>() && projectile.active)//取得这个弹幕符合条件就绘制
            //{
            //    //利用RT+shader制作一个"空间割裂"的效果,这次我们还可以利用上一个扭曲用的shader
            //    //原理还是和扭曲差不多，与扭曲不同，这次我们真的要将屏幕“劈开”，也就是说我们要将整个屏幕都按照素材图进行扭曲
            //    //所以我们本次要绘制两大块图片，这两块图片颜色不同(例如红绿)，分别代表相反方向的偏移方向
            //    gd.SetRenderTarget(Main.screenTargetSwap);//切换到原版的备用RT上去
            //    gd.Clear(Color.Transparent);//把这个RT清空成透明
            //    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//开始绘制
            //    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//把现在的屏幕画到备用画布上
            //    sb.End();//结束，这时候备用RT上记录了目前屏幕的样子
            //
            //    gd.SetRenderTarget(render);//切换到我们自己的RT
            //    gd.Clear(Color.Transparent);//先把RT清空成透明
            //    sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //
            //    //屏幕对角线长2202，也就是说边长2250的正方形铁定能覆盖完,能将屏幕切成两份！
            //    sb.Draw(TextureAssets.MagicPixel.Value, projectile.Center - Main.screenPosition,
            //            new Rectangle(0, 0, 2250, 2250), new Color(128, 255, 0, 255), projectile.rotation, new Vector2(0, 1000),
            //            1, SpriteEffects.None, 0);//画一个半红色+全绿色的方形出来(因为我们shader写的是满红色一圈，所以半红色就是半圈)
            //    sb.Draw(TextureAssets.MagicPixel.Value, projectile.Center - Main.screenPosition,
            //            new Rectangle(0, 0, 2250, 2250), Color.Green, projectile.rotation + 3.1416f, new Vector2(0, 1000),
            //  1, SpriteEffects.None, 0);//往反方向画一个绿色的方形
            //
            //    sb.End();//结束，这时候我们有了一张只画了扭曲素材图在上面,其余部分全是透明的"屏幕"
            //
            //    gd.SetRenderTarget(Main.screenTarget);//接下来我们切换到screenTarget这个Render，要做最终的绘制了
            //    gd.Clear(Color.Transparent);//照样先清空掉这个Render
            //    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);//开始绘制
            //    gd.Textures[1] = render;//让Tex1赋值为我们的render，也就是只绘制了扭曲图片弹幕其余什么也没有的屏幕
            //    DistortShader.Parameters["Length"].SetValue(0.05f * projectile.ai[0]);//扭曲的程度
            //    DistortShader.Parameters["Rot"].SetValue(projectile.rotation + 1.5707f);//扭曲的方向
            //                                                                            //赋值好参数后再应用我们的shader
            //    DistortShader.CurrentTechnique.Passes[0].Apply();//应用我们的shader,将扭曲图片转化成扭曲,具体实现原理请参见对应fx文件
            //    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//再在shader的加持下绘制原本的屏幕，就可以将屏幕进行扭曲处理了
            //    sb.End();//结束
            //    sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //    //我们还需要给裂开的这一段加上遮挡，来让屏幕像真的裂开了一样,其实我这里留一个小练习，怎么让这个遮挡的渐变光效变成岩浆呢？
            //    Texture2D tex = ModContent.Request<Texture2D>("MyMod/Images/Ex1").Value;
            //    sb.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation + 1.5707f, tex.Size() / 2,
            //        new Vector2(1000, 14
            //        * projectile.ai[0]), SpriteEffects.None, 0);
            //    sb.End();
            //}
            #endregion

            //在最后执行原本的函数
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }

        /// <summary>
        /// RT2D亮度蒙版
        /// </summary>
        /// <param name="texture"></param>
        void RT2DMaskShader(Action codeBlock, Action codeBlock2) {
            //RT2D
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            //屏幕储存
            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            //在这里占下显形点
            gd.SetRenderTarget(render);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            codeBlock();

            sb.End();

            //待显形的贴图放在这儿
            gd.SetRenderTarget(render2);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //绘制
            codeBlock2();

            sb.End();

            gd.SetRenderTarget(Main.screenTarget);//最终绘制
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//屏幕
            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);//开始绘制

            gd.Textures[1] = render2;
            RT2DMask.CurrentTechnique.Passes[0].Apply();
            RT2DMask.Parameters["m"].SetValue(0.62f);
            RT2DMask.Parameters["n"].SetValue(0.01f);
            RT2DMask.Parameters["OffsetX"].SetValue(0);//沿着X轴移动
            //RT2DMask.Parameters["OffsetX"].SetValue((float)((Main.GlobalTimeWrappedHourly) * 0.001f));//沿着X轴移动,想要动态的必须连续
            RT2DMask.Parameters["color0"].SetValue(new Vector4(0.2f, 0.1f, 0.09f, 1f));//描边

            sb.Draw(render, Vector2.Zero, Color.White);//画出我们处理过的屏幕
            sb.End();
        }

        /// <summary>
        /// RT2D圆形光源Shader
        /// </summary>
        /// <param name="texture"></param>
        void RT2DCircleLSShader(Action codeBlock, Action codeBlock2) {
            //RT2D
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            //屏幕储存
            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            //光效
            gd.SetRenderTarget(render);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            codeBlock();

            sb.End();

            //待显形的贴图放在这儿
            gd.SetRenderTarget(render2);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            codeBlock2();

            sb.End();

            gd.SetRenderTarget(Main.screenTarget);//最终绘制
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//屏幕
            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);//开始绘制

            gd.Textures[1] = render2;
            RT2DCircleLS.CurrentTechnique.Passes[0].Apply();
            RT2DCircleLS.Parameters["OffsetX"].SetValue(0);//参数
            RT2DCircleLS.Parameters["OffsetY"].SetValue(0);//参数

            sb.Draw(render, Vector2.Zero, Color.White);//画出我们处理过的屏幕
            sb.End();
        }

        private void Main_OnResolutionChanged(Vector2 obj)//在分辨率更改时，重建render
        {
            CreateRender();
        }

        private void On_Main_LoadWorlds(On_Main.orig_LoadWorlds orig)//加载世界时创建render,确保不是null
        {
            CreateRender();
            orig.Invoke();
        }
        #endregion

        public override void Unload()//模组卸载时执行
        {
            //卸载时要把On钩子卸载
            On_Main.LoadWorlds -= On_Main_LoadWorlds;
            Main.OnResolutionChanged -= Main_OnResolutionChanged;
            base.Unload();
        }
    }
}
