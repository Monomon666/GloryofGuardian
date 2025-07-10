using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.Localization;

namespace GloryofGuardian.Content.Projectiles {
    public class Corrupt2DT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 64;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 60;

            exdust = DustID.Crimson;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -30);

            //Main.rainTime = 43200.0;
            //Main.raining = true;
            //Main.maxRaining = (Main.cloudAlpha = 0.9f);
            //if (Main.netMode == 2) {
            //    NetMessage.SendData(7, -1, -1, (NetworkText)null, 0, 0f, 0f, 0f, 0, 0, 0);
            //    Main.SyncRain();
            //}

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            ////发射参数计算
            //float dx = target0.Center.X - AttackPos.X;
            //float dy = target0.Center.Y - AttackPos.Y;
            ////设置一个相对标准的下落加速度
            //float G = 0.3f;
            ////设置一个相对标准的初始垂直速度
            //float vy = 16;
            //for (int i = 0; i < 1; i++) {
            //    G *= Main.rand.NextFloat(0.8f, 1.2f);
            //    vy *= Main.rand.NextFloat(0.9f, 1.1f);
            //    vy *= (dy >= 0 ? 0.75f : 1.2f);
            //    float vx = dx / ((vy + (float)Math.Sqrt(vy * vy + 2 * G * dy)) / G);
            //    Vector2 velfire = new Vector2(vx * Main.rand.NextFloat(0.9f, 1.1f), -vy * Main.rand.NextFloat(0.98f, 1.02f));//降低精度
            //
            //    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
            //    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, velfire, ModContent.ProjectileType<BloodyProj>(), lastdamage, 6, Owner.whoAmI, G);
            //
            //    projlist.Add(proj1);
            //}

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Corrupt2DT").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -24);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
