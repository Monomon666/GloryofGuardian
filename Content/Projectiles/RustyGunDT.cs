using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class RustyGunDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 60;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 30;
            exdust = 0;

            TurnCenter = new Vector2(-4, -44);
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 AttackPos2 = Vector2.Zero;
        int Recoil = 0;
        bool crit = false;
        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -44);
            AttackPos2 = Projectile.Center + new Vector2(-6, -38);

            if (count == 3 && crit) {
                count += 12;
                crit = false;
            }
            if (count == 4) count += Main.rand.Next(12);

            if (Recoil > 0) Recoil--;

            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
                float bias = Main.rand.NextFloat(-0.1f, 0.1f);

                wrotation = wrotation + bias;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2 + nowvel * 32f, nowvel.RotatedBy(bias) * vel, ModContent.ProjectileType<RustyGunProj>(), lastdamage, 8, Owner.whoAmI);
                Recoil = 8;

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                float vel = Main.rand.NextFloat(0.9f, 1.15f) * 36f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2 + nowvel * 32f, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<RustyGunProj>(), lastdamage, 8, Owner.whoAmI, 1);
                
                for (int j = 0; j < 24; j++) {
                    int num = Dust.NewDust(AttackPos2 + new Vector2(0, -4) + nowvel * 46f, 0, 0, DustID.Flare, 0f, 0f, 10, Color.White, 1.5f);
                    Main.dust[num].velocity = nowvel.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
                    Main.dust[num].velocity *= Main.rand.NextFloat(0, 3f);
                    Main.dust[num].noGravity = true;
                }

                Recoil = 8;
                crit = true;

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 cen = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? new Vector2(-8, 0) : new Vector2(-16, 0);

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RustyGunDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -12);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RustyGunDT2").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(-4, -40);
            //枪管后坐力机制,开枪后枪管在4帧内后移,8帧后弹回
            Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
            Vector2 Recoilfix = Vector2.Zero;
            if (Recoil == 0) Recoilfix = Vector2.Zero;
            if (Recoil > 6) Recoilfix = (9 - Recoil) * -nowvel * 1;//最大后移8
            if (Recoil > 0 && Recoil <= 6) Recoilfix = (Recoil / 6f) * -nowvel * 2 * 1;

            Main.EntitySpriteDraw(texture, drawPosition + Recoilfix, null, lightColor, wrotation, texture.Size() * 0.5f + cen, Projectile.scale, spriteEffects, 0);


            return false;
        }
    }
}
