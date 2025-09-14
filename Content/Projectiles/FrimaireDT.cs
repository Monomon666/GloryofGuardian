using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class FrimaireDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 50;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.scale *= 1.4f;

            OtherHeight = 46;

            count0 = 45;

            findboss = true;
            exdust = DustID.SnowSpray;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 AttackPos2 = Vector2.Zero;

        int lightingmode = 0;//照明开关,分别亮灯,0,1,2
        public override void AI() {
            AttackPos = (Projectile.Center + new Vector2(-3, -80) + new Vector2(0, (float)Math.Sin(drawcount / 20f) * 4f));
            Lighting.AddLight(AttackPos, 0.6f, 0.6f, 0.6f);
            if (count % 24 == 0) {
                for (int j = 0; j < 1; j++) {
                    int gore1 = Gore.NewGore(null, AttackPos + new Vector2(-16, -16), Vector2.Zero, 16, 1);
                    Main.gore[gore1].velocity /= 2f;
                    Main.gore[gore1].rotation += Main.rand.NextFloat(-0.5f, 0.5f);
                    Main.gore[gore1].scale *= Main.rand.NextFloat(1f, 1.5f);
                    int gore2 = Gore.NewGore(null, AttackPos + new Vector2(-16, -16), Vector2.Zero, 17, 1);
                    Main.gore[gore2].velocity /= 2f;
                    Main.gore[gore2].rotation += Main.rand.NextFloat(-0.5f, 0.5f);
                    Main.gore[gore2].scale *= Main.rand.NextFloat(1f, 1.5f);
                }
            }

            if (drawcount % 40 == 0) lightingmode++;
            if (lightingmode == 3) lightingmode = 0;

            if (lightingmode == 0) Lighting.AddLight((Projectile.Center + new Vector2(-33, -35)), 1.2f, 0.5f, 0f);
            if (lightingmode == 1) Lighting.AddLight((Projectile.Center + new Vector2(-11, -43)), 0.8f, 0.8f, 0.2f);
            if (lightingmode == 2) Lighting.AddLight((Projectile.Center + new Vector2(20, -38)), 0.4f, 1f, 0f);

            //(Projectile.Center + new Vector2(-33, -35)).Point();
            //(Projectile.Center + new Vector2(-11, -43)).Point();
            //(Projectile.Center + new Vector2(20, -38)).Point();

            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos.Toz(target0.Center).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)) * 8f;
                if (Owner.ZoneSnow) vel *= 1.5f;
                
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, vel, ModContent.ProjectileType<FrimaireProj>(), lastdamage, 8, Owner.whoAmI);
                
                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireDT").Value;
            Texture2D texture000 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireDT2").Value;
            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireDT01").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireDT02").Value;
            Texture2D texture03 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireDT03").Value;

            Texture2D texture00 = texture01;
            Color color00 = lightColor;

            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj1").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj2").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj3").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj04").Value;
            Texture2D texture5 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj05").Value;
            Texture2D texture6 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj06").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -21);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (lightingmode == 0) texture00 = texture01;
            if (lightingmode == 1) texture00 = texture02;
            if (lightingmode == 2) texture00 = texture03;
            if (lightingmode == 0) color00 = new Color(1.2f, 0.5f, 0f);
            if (lightingmode == 1) color00 = new Color(1.2f, 1f, 0f);
            if (lightingmode == 2) color00 = new Color(0.4f, 1f, 0f);

            Main.EntitySpriteDraw(texture00, drawPosition0, null, color00 * 50f, Projectile.rotation, texture00.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(0, -60) + new Vector2(0, (float)Math.Sin(drawcount / 20f) * 4f), null, lightColor * 10, Projectile.rotation, texture1.Size() * 0.5f, 1.2f, SpriteEffects.None, 0);

            return false;
        }
    }

    public class FrimaireProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.tileCollide = false;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Lighting.AddLight(Projectile.Center, 0.6f, 0.6f, 0.6f);

            Projectile.rotation += 0.5f;

            if (Main.rand.NextBool(4)) {
                for (int j = 0; j < 1; j++) {
                    int gore1 = Gore.NewGore(null, Projectile.Center + new Vector2(-16, -16), Vector2.Zero, 16, 1);
                    Main.gore[gore1].velocity /= 2f;
                    Main.gore[gore1].rotation += Main.rand.NextFloat(-0.5f, 0.5f);
                    Main.gore[gore1].scale *= Main.rand.NextFloat(0.6f, 1f);
                    Main.gore[gore1].light += 2;
                    int gore2 = Gore.NewGore(null, Projectile.Center + new Vector2(-16, -16), Vector2.Zero, 17, 1);
                    Main.gore[gore2].velocity /= 2f;
                    Main.gore[gore2].rotation += Main.rand.NextFloat(-0.5f, 0.5f);
                    Main.gore[gore2].scale *= Main.rand.NextFloat(0.6f, 1f);
                    Main.gore[gore2].light += 2;
                }
            }

            if (target0 != null && target0.active && count > Projectile.ai[1]) {
                Projectile.velocity *= 0.98f;
                Projectile.velocity += Projectile.Center.Toz(target0.Center) * 0.5f;

                if ((int)(count / (Projectile.ai[1] * 4)) % 2 == 0) Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[2] * 1.5f);
                else Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[2] * -1.5f);

                if (Math.Abs(Projectile.velocity.ToRotation() - Projectile.Center.Toz(target0.Center).ToRotation()) > 1f) {
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity += Projectile.Center.Toz(target0.Center) * 1f;
                }

                if (Vector2.Distance(Projectile.Center, target0.Center) < 240) {
                    Projectile.velocity *= 0.8f;
                    Projectile.velocity += Projectile.Center.Toz(target0.Center) * 3f;
                }

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
            for (int j = 0; j < 12; j++) {
                int gore1 = Gore.NewGore(null, Projectile.Center + new Vector2(-16, -16), Vector2.Zero, 16, 1);
                Main.gore[gore1].velocity = new Vector2(0, Main.rand.NextFloat(0.3f, 2f)).RotatedByRandom(MathHelper.Pi * 2);
                Main.gore[gore1].rotation += Main.rand.NextFloat(-0.5f, 0.5f);
                Main.gore[gore1].scale *= Main.rand.NextFloat(0.6f, 1f);
                Main.gore[gore1].light += 2;
                int gore2 = Gore.NewGore(null, Projectile.Center + new Vector2(-16, -16), Vector2.Zero, 17, 1);
                Main.gore[gore2].velocity = new Vector2(0, Main.rand.NextFloat(0.3f, 2f)).RotatedByRandom(MathHelper.Pi * 2);
                Main.gore[gore2].rotation += Main.rand.NextFloat(-0.5f, 0.5f);
                Main.gore[gore2].scale *= Main.rand.NextFloat(0.6f, 1f);
                Main.gore[gore2].light += 2;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj1").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj12").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrimaireProj13").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 0);

            Main.EntitySpriteDraw(texture2, drawPosition0, null, new Color(255, 255, 255), Projectile.rotation, texture1.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture3, drawPosition0, null, new Color(251, 242, 54, 0), Projectile.rotation, texture1.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture1, drawPosition0, null, lightColor * 10f, Projectile.rotation, texture1.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return base.PreDraw(ref lightColor);
        }
    }

    public class FrimaireProj2 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            return base.PreDraw(ref lightColor);
        }
    }
}
