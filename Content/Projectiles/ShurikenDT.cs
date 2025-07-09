using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class ShurikenDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 40;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 46;

            count0 = 25;

            findboss = true;
            exdust = DustID.Wraith;
            frame0 = Main.rand.Next(2);

            Attackrange = 1200;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 AttackPos2 = Vector2.Zero;

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-3, -38);
            
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos.Toz(target0.Center) * 18f;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, vel.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)), ModContent.ProjectileType<ShurikenProj>(), lastdamage, 8, Owner.whoAmI);

                frame0 += 1;
                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos.Toz(target0.Center) * 18f;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, vel.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), ModContent.ProjectileType<ShurikenProj>(), lastdamage, 8, Owner.whoAmI, 1);

                frame0 += 1;
                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        int frame0 = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -15);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT2").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT3").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -43);
            if (frame0 >= 2) frame0 = 0;
            if (frame0 == 0) Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            if (frame0 == 1) Main.EntitySpriteDraw(texture1, drawPosition + new Vector2(2.5f, 0), null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            return false;
        }
    }
}
