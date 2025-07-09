using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class CorruptCloudProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 6;//动图帧数

            Projectile.width = 54;
            Projectile.height = 24;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 1;

            Projectile.friendly = false;
            Projectile.tileCollide = false;

            if (Projectile.ai[1] == 1) Projectile.Size *= 1.5f;
        }

        Player Owner => Main.player[Projectile.owner];

        NPC target1 = null;
        bool startspeed = true;
        Vector2 chasingpos = Vector2.Zero;
        public override void AI() {
            //目标继承
            if(count == 1) target1 = Main.npc[(int)Projectile.ai[0]];

            //初始的加速
            if (count > 60) startspeed = false;

            //默认的摇晃
            chasingpos += new Vector2((float)Math.Cos(count / 40f), 0);
            if (Projectile.ai[1] == 1) chasingpos += new Vector2((float)Math.Cos(count / 40f), 0);

            if (target1 != null && target1.active && target1.whoAmI != 0) {
                if(count == 1) chasingpos = target1.Center + new Vector2(0, -240) + new Vector2(0, Main.rand.Next(-8, 8));
                
                if (startspeed) Projectile.ChasingBehavior(chasingpos, 12, 48);
                if (!startspeed) Projectile.ChasingBehavior(chasingpos, 4, 48);
            }
            else {
                target1 = Projectile.Center.InPosClosestNPC(400, false, true);
            }

            if (Projectile.ai[1] == 0) {
                if (count >= 90) Projectile.alpha += 1;
                if (Projectile.alpha >= 240) Projectile.Kill();
            }
            if (Projectile.ai[1] == 1) {
                if (count >= 150) Projectile.alpha += 1;
                if (Projectile.alpha >= 240) Projectile.Kill();
            }

            //发射雨滴
            if (Projectile.ai[1] == 0) {
                if (count > 60 && count % 20 == 0) {
                    int randx = Main.rand.Next(-6, 6);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(randx, 6), new Vector2(0, 12), ModContent.ProjectileType<CorruptRainProj>(), Projectile.damage, 1, Owner.whoAmI);
                    count += Main.rand.Next(10);
                }
            }
            if (Projectile.ai[1] == 1) {
                if (count > 60 && count % 12 == 0) {
                    int randx = Main.rand.Next(-6, 6);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(randx, 6), new Vector2(0, 12), ModContent.ProjectileType<CorruptRainProj>(), Projectile.damage, 1, Owner.whoAmI, 1);
                    count += Main.rand.Next(10);
                }
            }


            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 6)
                % 6;//要手动填，不然会出错

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {

        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {

        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CorruptCloudProj").Value;

            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            //Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY), lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(27, 23), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(
                texture,
                drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                new Vector2(27, 23),
                Projectile.scale,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
