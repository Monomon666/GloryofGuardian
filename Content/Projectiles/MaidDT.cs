using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static GloryofGuardian.Common.GOGUtils;

namespace GloryofGuardian.Content.Projectiles {
    public class MaidDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 60;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            findboss = true;
            ignoretile = true;
            Attackrange = 3000;

            count0 = 60;
            exdust = 0;

            TurnCenter = new Vector2(-4, -44);

            //过近筛除,枪管设计,不攻击过近的敌人
            closeignoredistance = 64;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 AttackPos2 = Vector2.Zero;
        int Recoil = 0;
        bool crit = false;

        Projectile magicproj = null;
        int modecount = 0;
        int backlightcount = 0;
        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -44);
            AttackPos2 = Projectile.Center + new Vector2(-6, -38);

            Lighting.AddLight((AttackPos + new Vector2(0, 16)), new Vector3(0.8f, 0.8f, 2.3f));

            if (Recoil > 0) Recoil--;

            if (mode == 0 && count < Gcount) {
                for (int j = 0; j < 2; j++) {
                    Vector2 firepos = AttackPos2 + new Vector2(0, -4) + new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation)) * 72f;
                    int num = Dust.NewDust(firepos, 0, 0, DustID.BlueFlare, 0f, 0f, 10, Color.White, 1.5f);
                    Main.dust[num].position += new Vector2(0, Main.rand.NextFloat(0, 64 * backlight)).RotatedBy(Main.rand.NextFloat(MathHelper.Pi * 2));
                    Main.dust[num].velocity = Main.dust[num].position.Toz(firepos) * 2f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (target0 != null && target0.active && backlightcount <= Gcount) backlightcount++;
            else backlightcount--;
            backlight = ((float)backlightcount / (float)Gcount);

            //持续瞄准
            if(magicproj != null && magicproj.active) {
                magicproj.ai[1] = wrotation;
            }

            if (mode == 1) {
                count = 0;
                modecount++;
                if (modecount > 250) {
                    mode = 0;
                    modecount = 0;
                    magicproj = null;
                }
            }

            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                float vel = 16f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<MaidProj0>(), lastdamage, 8, Owner.whoAmI,
                    0, wrotation);
                magicproj = proj1;

                crit = true;

                projlist.Add(proj1);
            }

            mode = 1;
            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                float vel = 16f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<MaidProj0>(), lastdamage, 8, Owner.whoAmI,
                    1, wrotation);
                magicproj = proj1;

                crit = true;

                projlist.Add(proj1);
            }

            mode = 1;
            FinishAttack = true;
            return projlist;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return base.Colliding(projHitbox, targetHitbox);
        }

        float backlight = 0;
        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 cen = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? new Vector2(-8, 0) : new Vector2(-16, 0);

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -24);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidDT2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidDT3").Value;
            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidDT2Shadow").Value;
            Texture2D texture2s = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidDT3Shadow").Value;
            Texture2D texturebs = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidDT2BlackShadow").Value;
            Texture2D texture2bs = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidDT3BlackShadow").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(-4, -40);
            //枪管后坐力机制,开枪后枪管在4帧内后移,8帧后弹回
            Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
            Vector2 Recoilfix = Vector2.Zero;
            if (Recoil == 0) Recoilfix = Vector2.Zero;
            if (Recoil > 6) Recoilfix = (9 - Recoil) * -nowvel * 1;//最大后移8
            if (Recoil > 0 && Recoil <= 6) Recoilfix = (Recoil / 6f) * -nowvel * 2 * 1;

            Main.EntitySpriteDraw(texturebs, drawPosition + Recoilfix, null, new Color(255, 255, 255, 50) * backlight, wrotation, texturebs.Size() * 0.5f + cen, Projectile.scale * 1.1f, spriteEffects, 0);
            Main.EntitySpriteDraw(textures, drawPosition + Recoilfix, null, new Color(0, 80, 200, 0) * backlight, wrotation, textures.Size() * 0.5f + cen, Projectile.scale * 1.1f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPosition + Recoilfix, null, lightColor, wrotation, texture.Size() * 0.5f + cen, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }

    public class MaidProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.localNPCHitCooldown = 12;

            Projectile.extraUpdates = 4;
            Projectile.penetrate = -1;
            Projectile.scale *= 2.5f;

            Projectile.tileCollide = false;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int j = 0; j < 1; j++) {
                int num = Dust.NewDust(Projectile.Center + new Vector2(0, -4), 0, 0, DustID.GemSapphire, 0f, 0f, 10, Color.White, 1f);
                Main.dust[num].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Main.dust[num].velocity *= Main.rand.NextFloat(4f, 12f);
                Main.dust[num].noGravity = true;
            }

            if (Projectile.ai[0] == 1){
                if(hitcount != 0 && count >= hitcount + 24) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, -Projectile.velocity, ModContent.ProjectileType<MaidProj0>(), Projectile.damage, 8, Owner.whoAmI, 2, Projectile.velocity.ToRotation());
                    proj1.extraUpdates += 1;

                    Projectile.Kill();
                }
            }

            base.AI();
        }

        bool hadhit = false;
        int hitcount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] == 1) {
                hadhit = true;
                hitcount = count;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            //Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            for (int j = 0; j < 4; j++) {
                int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemSapphire, 0f, 0f, 10, Color.White, 1f);
                Main.dust[num].velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-1f, 1f));
                Main.dust[num].velocity *= Main.rand.NextFloat(3f, 4f);
                Main.dust[num].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunProj").Value;
            Texture2D texture11 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusBlackShadow").Value;
            Texture2D texture12 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusShadow1").Value;
            Texture2D texture13 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Rhombus").Value;

            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunProj3").Value;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                new Color(255, 255, 255, 200) * 0.2f,
                Projectile.rotation,
                texture.Size() / 2,
                Projectile.scale * new Vector2(1.4f, 2f) * 1.4f,
                SpriteEffects.None,
                0);

            Main.EntitySpriteDraw(
                texture13,
                Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * 4,
                null,
                new Color(0, 80, 200, 0) * 2f,
                Projectile.rotation,
                texture13.Size() / 2,
                Projectile.scale * new Vector2(1f, 0.4f) * 1.4f,
                SpriteEffects.None,
                0);

            return false;
        }
    }

    //前体
    public class MaidProj0 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = -1;

            Projectile.friendly = false;
        }

        Player Owner => Main.player[Projectile.owner];

        //mode为0为子弹前体,1为强化子弹前体,2为返回魔弹前体
        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center -= Projectile.velocity;

            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 1) Projectile.velocity = new Vector2(16, 0).RotatedBy(Projectile.ai[1]);

            if (count > 100) {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                if (Projectile.ai[0] == 0 || Projectile.ai[0] == 2) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 72, Projectile.velocity.SafeNormalize(Vector2.Zero) * 24, ModContent.ProjectileType<MaidProj>(), Projectile.damage, 8, Owner.whoAmI);
                }

                //普通前体,强化子弹
                if (Projectile.ai[0] == 1) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 72, Projectile.velocity.SafeNormalize(Vector2.Zero) * 24, ModContent.ProjectileType<MaidProj>(), Projectile.damage, 8, Owner.whoAmI, 1);
                }

                Projectile.Kill();
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
        }

        bool finalcolor = false;
        float scaleshrink = 1;
        int drawcircount = 0;
        int drawcircount2 = 0;

        float FlipRotation = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidMagic").Value;
            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MaidMagicBlackShadow").Value;

            float magicalpha = (float)Math.Min(Math.Sin((drawcount - 10) / 90f), 0.2f);
            if (magicalpha >= 0.99f) finalcolor = true;
            if (finalcolor) magicalpha = 0.2f;

            if (finalcolor && scaleshrink > 0.5f) scaleshrink -= 0.005f;

            //扁平化顶点绘制
            {
                float rs = drawcount / 30f;
                List<VertexInfo2> vertices = new List<VertexInfo2>();
                float rotcount = drawcount / (80f - count / 5f);

                if (1 == 1) {
                    if (drawcircount < 70) drawcircount++;
                    float v0 = 4 - drawcircount / 33f;
                    FlipRotation = Projectile.ai[1];

                    //大环
                    Vector2 v2 =
                        new Vector2(-300, -300).RotatedBy(rotcount * 2) * v0;
                    vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(0, 0, 1), new Color(lightColor.R, lightColor.G, lightColor.B, 0) * magicalpha));
                    //记录纹理左上角的顶点坐标
                    v2 = new Vector2(300, -300).RotatedBy(rotcount * 2) * v0;
                    vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(1, 0, 1), new Color(lightColor.R, lightColor.G, lightColor.B, 0) * magicalpha));
                    //记录纹理右上角的顶点坐标
                    v2 = new Vector2(-300, 300).RotatedBy(rotcount * 2) * v0;
                    vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(0, 1, 1), new Color(lightColor.R, lightColor.G, lightColor.B, 0) * magicalpha));
                    //记录纹理左下角的顶点坐标
                    v2 = new Vector2(300, 300).RotatedBy(rotcount * 2) * v0;
                    vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(1, 1, 1), new Color(lightColor.R, lightColor.G, lightColor.B, 0) * magicalpha));

                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    if (vertices.Count >= 3) {
                        for (int j = 0; j < 3; j++) {
                            Main.graphics.GraphicsDevice.Textures[0] = textures;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

                            Main.graphics.GraphicsDevice.Textures[0] = texture;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                        }
                    }
                    vertices.Clear();
                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }

            if (Projectile.ai[0] == 1){
                float rs = drawcount / 30f;
                List<VertexInfo2> vertices = new List<VertexInfo2>();
                float rotcount = drawcount / (80 - count / 2f);

                if (1 == 1) {
                    if (drawcircount2 < 100) drawcircount2++;
                    float v0 = 4 - drawcircount2 / 40f;
                    FlipRotation = Projectile.ai[1];

                    //大环
                    Vector2 v2 =
                        new Vector2(-300, -300).RotatedBy(rotcount * 2) * v0;
                    vertices.Add(new VertexInfo2(Drwapos1(v2, FlipRotation), new Vector3(0, 0, 1), new Color(lightColor.R, lightColor.G, lightColor.B, 0) * magicalpha));
                    //记录纹理左上角的顶点坐标
                    v2 = new Vector2(300, -300).RotatedBy(rotcount * 2) * v0;
                    vertices.Add(new VertexInfo2(Drwapos1(v2, FlipRotation), new Vector3(1, 0, 1), new Color(lightColor.R, lightColor.G, lightColor.B, 0) * magicalpha));
                    //记录纹理右上角的顶点坐标
                    v2 = new Vector2(-300, 300).RotatedBy(rotcount * 2) * v0;
                    vertices.Add(new VertexInfo2(Drwapos1(v2, FlipRotation), new Vector3(0, 1, 1), new Color(lightColor.R, lightColor.G, lightColor.B, 0) * magicalpha));
                    //记录纹理左下角的顶点坐标
                    v2 = new Vector2(300, 300).RotatedBy(rotcount * 2) * v0;
                    vertices.Add(new VertexInfo2(Drwapos1(v2, FlipRotation), new Vector3(1, 1, 1), new Color(lightColor.R, lightColor.G, lightColor.B, 0) * magicalpha));

                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    if (vertices.Count >= 3) {
                        for (int j = 0; j < 3; j++) {
                            Main.graphics.GraphicsDevice.Textures[0] = textures;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

                            Main.graphics.GraphicsDevice.Textures[0] = texture;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                        }
                    }
                    vertices.Clear();
                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }

            return false;
        }
        Vector2 Drwapos1(Vector2 v1, float FlipRotation) {
            return Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * (135f - (drawcircount2 / 2f)) + new Vector2(0, 0) + new Vector2(v1.X / 24, v1.Y / 10).RotatedBy(FlipRotation);
        }

        Vector2 Drwapos2(Vector2 v1, float FlipRotation) {
            return Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * 72 + new Vector2(0, 0) + new Vector2(v1.X / 16, v1.Y / 8).RotatedBy(FlipRotation);
        }
    }

    //静态激光
    public class MaidLaserProj : GOGLaserProj, ILocalizedModType {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
        }
        public override void AI() {
            if (LaserWidth < 25) LaserWidth += 0.1f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor) {
            return base.PreDraw(ref lightColor);
        }
    }
}
