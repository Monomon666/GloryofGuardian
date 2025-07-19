using GloryofGuardian.Common;
using System.Collections.Generic;
using System;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using System.Linq;
using GloryofGuardian.Content.Buffs;
using static GloryofGuardian.Common.GOGUtils;

namespace GloryofGuardian.Content.Projectiles {
    public class HarpyDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 82;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.light = 1.5f;

            Projectile.scale *= 1.4f;

            OtherHeight = 34;

            count0 = 20;
            overclockinterval = 6;
            
            exdust = DustID.Cloud;
            Drop = false;
        }

        Player Owner => Main.player[Projectile.owner];
        public override void AI() {
            Lighting.AddLight(Projectile.Center, 2.5f, 2.5f, 2.5f);

            AttackPos = Projectile.Center + new Vector2(-2, 0);
            Projectile.Center += new Vector2(0, (float)Math.Sin(drawcount / 30f) * 0.3f);
            Projectile.Center += new Vector2((float)Math.Sin(drawcount / 120f) * 0.2f, 0);

            if (!Main.raining) {
                //Todo
            }

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            if (Main.rand.NextBool(4)) {
                for (int i = 0; i < 1; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                        AttackPos + new Vector2(Main.rand.NextFloat(-8, 8), 0), new Vector2(0, 24).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)).RotatedBy(-Main.windSpeedCurrent / 4f), ModContent.ProjectileType<HarpyProj2>(), lastdamage, 6, Owner.whoAmI, Goverclockinterval, 0, overclockcount);

                    projlist.Add(proj1);
                }
            }else {
                for (int i = 0; i < 1; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                        AttackPos + new Vector2(Main.rand.NextFloat(-8, 8), 0), new Vector2(0, 12).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)).RotatedBy(-Main.windSpeedCurrent / 4f), ModContent.ProjectileType<HarpyProj>(), lastdamage, 6, Owner.whoAmI);

                    projlist.Add(proj1);
                }
            }

            FinishAttack = true;
            return projlist;
        }

        List<int> numberList = [];
        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            overclock = true;
            overclockcount = 9;
            count -= Goverclockinterval;

            for (int i = 1; i <= 9; i++) {
                numberList.Add(i);
            }

            // 使用Random打乱顺序
            Random random = new Random();
            for (int i = numberList.Count - 1; i > 0; i--) {
                int j = random.Next(0, i + 1); // 随机索引
                                               // 交换元素
                int temp = numberList[i];
                numberList[i] = numberList[j];
                numberList[j] = temp;
            }

            return projlist;
        }

        protected override List<Projectile> AttackOverclock() {
            List<Projectile> projlist = new List<Projectile>();

            {
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                AttackPos + new Vector2(0, -12), new Vector2(0, 18).RotatedBy(MathHelper.PiOver2 / 9f * (5 - numberList[overclockcount - 1])), ModContent.ProjectileType<HarpyProj2>(), lastdamage, 6, Owner.whoAmI, Goverclockinterval, 0, overclockcount);

                projlist.Add(proj1);
            }

            if (overclockcount > 0) {
                overclockcount -= 1;
                count -= Goverclockinterval;
            }
            if (overclockcount == 0) {
                overclock = false;
                FinishAttack = true;
            }
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyDT").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -16);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class HarpyProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 2;

            Projectile.alpha = 255;

            Projectile.scale *= 0.6f;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.7f, 0.8f);
            //渐入
            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            else if (Projectile.alpha > 0) Projectile.alpha -= 15;
            // 旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + 0.05f;

            if (mode == 0) ai1();
            if (mode == 1) ai2();

            base.AI();
        }

        //常态
        void ai1() {

        }

        //钉入
        void ai2() {
            if (sticknpc == null || !sticknpc.active) Projectile.Kill();
            else {
                Projectile.Center = sticknpc.Center + relapos;
            }

            if (count > fadetime + 180) Projectile.Kill();
        }

        Vector2 relapos = Vector2.Zero;
        NPC sticknpc = null;
        int fadetime = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (mode == 0) {
                mode = 1;//改变状态机为钉入状态
                fadetime = count;
                relapos = target.Center.To(Projectile.Center);
                sticknpc = target;
                pmode = 1;//标记为已击中
                Projectile.ai[1] = target.whoAmI;//标记正确的受debuff的目标
                Projectile.friendly = false;

                target.AddBuff(ModContent.BuffType<JavelinDebuff>(), 60);
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 10, Color.White, 2f);
                Main.dust[num].velocity *= 0.5f;
                Main.dust[num].noGravity = true;
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2Shadow").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2BlackShadow").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);

            Main.EntitySpriteDraw(texture2, drawPos, null, new Color(148, 216, 255) * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture2.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPos, null, new Color(148, 216, 255, 0) * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture1.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, drawPos, null, lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class HarpyProj2 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;

            Projectile.alpha = 255;

            Projectile.scale *= 0.6f;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        //三种状态:0:飞行 1:钉入 2:收回
        //等一秒后收回
        public override void AI() {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0f);

            //注!!!ai1 被占用!!!
            if (count > 60 + Projectile.ai[0] * Projectile.ai[2]) {
                mode = 2;

                Projectile.friendly = true;
                Projectile.penetrate = -1;
                Projectile.localNPCHitCooldown = 10;

                Projectile.velocity = Projectile.Center.To(startpos) / 10f;
            }

            //渐入
            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            else if (Projectile.alpha > 0) Projectile.alpha -= 15;

            if (mode == 0) ai1();
            if (mode == 1) ai2();
            if (mode == 2) ai3();

            base.AI();
        }

        //常态
        void ai1() {
            // 旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            //if (drawcount < 10) drawcount += Main.rand.Next(10, 40);
            //Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sin(drawcount / 2f) / 6f);
        }

        //钉墙
        void ai2() {
            // 旋转
            Projectile.rotation = Projectile.Center.Toz(startpos).ToRotation() - MathHelper.PiOver2;
        }

        //返回
        void ai3() {
            // 旋转
            Projectile.rotation = Projectile.Center.Toz(startpos).ToRotation() + MathHelper.PiOver2;

            if (Vector2.Distance(Projectile.Center, startpos) < 64) Projectile.Kill();
        }

        Vector2 relapos = Vector2.Zero;
        NPC sticknpc = null;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            mode = 2;
            Projectile.velocity *= 0;
            return false;
        }

        public override void OnKill(int timeLeft) {
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 10, Color.Yellow, 2f);
                Main.dust[num].velocity *= 0.5f;
                Main.dust[num].noGravity = true;
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2Shadow").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2BlackShadow").Value;

            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Effects + "Extra_197").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8) + Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f;

            Main.EntitySpriteDraw(texture2, drawPos, null, new Color(255, 255, 0) * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture2.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPos, null, new Color(255, 255, 0, 0) * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture1.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            //////////////////拖尾
            if (mode == 0 || mode == 2) {
                //切回原画布
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                //注入
                List<VertexInfo2> vertices3 = new List<VertexInfo2>();
                for (int i = 0; i < Projectile.oldPos.Length; i++) {
                    if (Projectile.oldPos[i] != Vector2.Zero) {
                        vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(3, 1) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(180)),
                        new Vector3(i / 8f, 0, 1 - (i / 8f)), new Color(255, 255, 0) * (1 - 0.12f * i)));
                        vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(3, 1) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(10)),
                        new Vector3(i / 8f, 1, 1 - (i / 8f)), new Color(255, 255, 0) * (1 - 0.12f * i)));
                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                if (vertices3.Count >= 3) {
                    for (int i = 0; i < 4; i++) {
                        Main.graphics.GraphicsDevice.Textures[0] = texture4;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices3.ToArray(), 0, vertices3.Count - 2);
                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                //切回原画布
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                vertices3.Clear();
            }

            Main.EntitySpriteDraw(texture0, drawPos, null, lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture0.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return base.PreDraw(ref lightColor);
        }
    }
}
