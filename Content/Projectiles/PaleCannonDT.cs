using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.Dusts;
using GloryofGuardian.Content.ParentClasses;
using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class PaleCannonDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 80;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 180;
            exdust = DustID.GemDiamond;

            TurnCenter = new Vector2(-4, -44);

            //过近筛除,枪管设计,不攻击过近的敌人
            closeignoredistance = 64;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 AttackPos2 = Vector2.Zero;
        int Recoil = 0;
        bool crit = false;
        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -48);
            AttackPos2 = Projectile.Center + new Vector2(-6, -42);

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

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos + nowvel * 56f, nowvel.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)) * vel, ModContent.ProjectileType<PaleCannonProj>(), lastdamage, 8, Owner.whoAmI, 0);
                
                for (int j = 0; j < 4; j++) {
                    int num = Dust.NewDust(AttackPos + new Vector2(0, -4) + nowvel * 56f, 0, 0, ModContent.DustType<PaleLineDust>(), 0f, 0f, 10, Color.White, 1.6f);
                    Main.dust[num].velocity = nowvel.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
                    Main.dust[num].velocity *= Main.rand.NextFloat(1f, 8f);
                    Main.dust[num].noGravity = true;
                }

                Recoil = 8;
                crit = true;

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos + nowvel * 56f, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<PaleCannonProj>(), lastdamage * 2, 16, Owner.whoAmI, 1);

                for (int j = 0; j < 4; j++) {
                    int num = Dust.NewDust(AttackPos + new Vector2(0, -4) + nowvel * 56f, 0, 0, ModContent.DustType<PaleLineDust>(), 0f, 0f, 10, Color.White, 1.6f);
                    Main.dust[num].velocity = nowvel.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
                    Main.dust[num].velocity *= Main.rand.NextFloat(1f, 8f);
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
            bool collBool = false;
            float point = 0f;

            Vector2 startPos = Projectile.position + new Vector2(Projectile.width / 2, Projectile.height);
            Vector2 endPos = Projectile.position + new Vector2(Projectile.width / 2, -32);

            collBool = Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                startPos,
                endPos,
                Projectile.width,
                ref point
                );

            return collBool;
        }

        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 cen = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? new Vector2(-8, 0) : new Vector2(-16, 0);

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -17);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleConnonDT2").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(-4, -45);
            //枪管后坐力机制,开枪后枪管在4帧内后移,8帧后弹回
            Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
            Vector2 Recoilfix = Vector2.Zero;
            if (Recoil == 0) Recoilfix = Vector2.Zero;
            if (Recoil > 6) Recoilfix = (9 - Recoil) * -nowvel * 5;//最大后移8
            if (Recoil > 0 && Recoil <= 6) Recoilfix = (Recoil / 6f) * -nowvel * 2 * 5;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, new Color(255, 255, 255, 200), Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, drawPosition + Recoilfix, null, lightColor, wrotation, texture.Size() * 0.5f + cen, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPosition + Recoilfix, null, new Color(255, 255, 255, 150), wrotation, texture.Size() * 0.5f + cen, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }

    public class PaleCannonProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 1;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] == 0) {
                for (int j = 0; j < 1; j++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<PaleLightDust>(), 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Main.dust[num].velocity *= Main.rand.NextFloat(4f, 12f);
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[0] == 1) {
                for (int j = 0; j < 3; j++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<PaleLightDust>(), 0f, 0f, 10, Color.White, 1.5f);
                    Main.dust[num].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Main.dust[num].velocity *= Main.rand.NextFloat(4f, 12f);
                    Main.dust[num].noGravity = true;
                }
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] == 1) {
                if (target.knockBackResist > 0) target.AddBuff(ModContent.BuffType<PaleSuppressedDebuff>(), 300);

                for (int j = 0; j < 4; j++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemDiamond, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
                    Main.dust[num].velocity *= Main.rand.NextFloat(3f, 4f);
                    Main.dust[num].noGravity = true;
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            if (Projectile.ai[0] == 0) {
                //爆炸
                for (int j = 0; j < 50; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(0, -48).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)) * Main.rand.NextFloat(0, 1), 0, 0, ModContent.DustType<PaleLightDust>(), 0, 0, 0, Color.White, 4f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = Projectile.Center.Toz(Main.dust[num1].position) * Main.rand.NextFloat(2);

                    int num2 = Dust.NewDust(Projectile.Center + new Vector2(0, -48).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)) * Main.rand.NextFloat(0, 1), 0, 0, ModContent.DustType<PaleLightDust>(), 0, 0, 0, Color.White, 3f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = Projectile.Center.Toz(Main.dust[num2].position) * Main.rand.NextFloat(2);

                    int num3 = Dust.NewDust(Projectile.Center + new Vector2(0, -48).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)) * 1, 0, 0, ModContent.DustType<PaleLightDust>(), 0, 0, 0, Color.White, 3f);
                    Main.dust[num3].noGravity = true;
                    Main.dust[num3].velocity *= 0;
                }
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 180;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }

            if (Projectile.ai[0] == 1) {
                //爆炸
                for (int j = 0; j < 50; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(0, -120).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)) * Main.rand.NextFloat(0, 1), 0, 0, ModContent.DustType<PaleLightDust>(), 0, 0, 0, Color.White, 4f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = Projectile.Center.Toz(Main.dust[num1].position) * Main.rand.NextFloat(2, 4);

                    int num2 = Dust.NewDust(Projectile.Center + new Vector2(0, -120).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)) * Main.rand.NextFloat(0, 1), 0, 0, ModContent.DustType<PaleLightDust>(), 0, 0, 0, Color.White, 3f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = Projectile.Center.Toz(Main.dust[num2].position) * Main.rand.NextFloat(2, 4);

                    int num3 = Dust.NewDust(Projectile.Center + new Vector2(0, -150).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)) * Main.rand.NextFloat(0.9f, 1), 0, 0, ModContent.DustType<PaleLightDust>(), 0, 0, 0, Color.White, 3f);
                    Main.dust[num3].noGravity = true;
                    Main.dust[num3].velocity = Projectile.Center.Toz(Main.dust[num2].position) * 1;
                }
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 320;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunProj3").Value;
            Texture2D texture13 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Rhombus").Value;

            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 1) {
                Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    new Color(255, 255, 255, 255) * 1f,
                    Projectile.rotation,
                    texture.Size() / 2,
                    Projectile.scale * new Vector2(1f, 0.8f) * 1.5f,
                    SpriteEffects.None,
                    0);

                Main.EntitySpriteDraw(
                    texture13,
                    Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * 4,
                    null,
                    new Color(200, 200, 200, 0) * 1f,
                    Projectile.rotation,
                    texture13.Size() / 2,
                    Projectile.scale * new Vector2(0.8f, 0.4f) * 1.5f,
                    SpriteEffects.None,
                    0);
            }

            return false;
        }
    }
}
