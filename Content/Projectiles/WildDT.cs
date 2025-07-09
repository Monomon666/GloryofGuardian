using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class WildDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 32;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 120;
            exdust = 0;

            TurnCenter = new Vector2(-4, -30);
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -26);

            //Vector2 pos = Projectile.Center + new Vector2(-4, -30);
            //for (int j = 0; j < 15; j++) {
            //    int num1 = Dust.NewDust(pos, 0, 0, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
            //    Main.dust[num1].noGravity = true;
            //    Main.dust[num1].velocity *= 0f;
            //}
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos + nowvel * 20f, nowvel * vel, ModContent.ProjectileType<WildProj>(), lastdamage, 8, Owner.whoAmI);

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = -1; i < 2; i++) {
                float vel = Main.rand.NextFloat(0.9f, 1.15f) * 18f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos + nowvel * 20f, nowvel.RotatedBy(i * 0.1f + Main.rand.NextFloat(-0.2f, 0.2f)) * vel, ModContent.ProjectileType<WildProj>(), lastdamage, 8, Owner.whoAmI);

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "WildDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -4);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "WildDT2").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -26);
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, wrotation, texture.Size() * 0.5f + new Vector2(-16, 0), Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}
