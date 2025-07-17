using GloryofGuardian.Common;
using System.Collections.Generic;
using System;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using System.Linq;

namespace GloryofGuardian.Content.Projectiles {
    public class HarpyDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 64;
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
            Projectile.Center += new Vector2(0, (float)Math.Sin(drawcount / 30f) * 0.5f);

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
}
