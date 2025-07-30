using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class PlanteraDT : GOGDT {
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

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraDT").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraCallProj").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj01").Value;
            Texture2D texture5 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj02").Value;
            Texture2D texture6 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj20").Value;
            Texture2D texture7 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj21").Value;
            Texture2D texture8 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj22").Value;

            //Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -16);
            //Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class PlanteraCallProj : GOGProj {
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

    public class PlanteraProj0 : GOGProj {
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

    public class PlanteraProj : GOGProj {
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
