using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class GarrisonDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 32;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 120;
            exdust = 0;

            TurnCenter = new Vector2(-4, -34);
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -34);
            Lighting.AddLight(AttackPos, 1 * 1f, 1 * 1f, 1 * 1f);

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
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                    AttackPos + nowvel * 20f + (attacknum == 0 ? new Vector2 (0, 0) : new Vector2(0, 8)),
                    nowvel * vel * (attacknum == 0 ? 1 : 0.85f),
                    ModContent.ProjectileType<GarrisonProj>(), lastdamage, 8, Owner.whoAmI);

                projlist.Add(proj1);

                attacknum += 1;
            }

            if (attacknum == 1) count -= 15;
            if (attacknum >= 2) FinishAttack = true;

            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);

                for (int j = 0; j < 8; j++) {
                    int num2 = Dust.NewDust(AttackPos + nowvel * 24f, 0, 0, DustID.GoldCoin, 0f, 0f, 10, Color.White, 2f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity *= 0.5f;
                    Main.dust[num2].velocity += nowvel * 4f;
                }

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                    AttackPos + nowvel * 20f + (attacknum == 0 ? new Vector2(0, 0) : new Vector2(0, 8)),
                    nowvel * vel * (attacknum == 0 ? 1 : 0.85f),
                    ModContent.ProjectileType<GarrisonProj>(), lastdamage, 8, Owner.whoAmI, 0, 0, 1);

                projlist.Add(proj1);

                attacknum += 1;
            }

            if (attacknum == 1) count -= 15;
            if (attacknum >= 2) FinishAttack = true;

            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GarrisonDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -6);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GarrisonDT2").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -28);
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, wrotation, texture.Size() * 0.5f + new Vector2(-16, 0), Projectile.scale * 1.1f, spriteEffects, 0);

            return false;
        }
    }
}
