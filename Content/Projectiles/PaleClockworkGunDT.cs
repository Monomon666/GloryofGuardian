using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.Dusts;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class PaleClockworkGunDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 80;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 24;
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
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2 + nowvel * 32f, nowvel.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)) * vel, ModContent.ProjectileType<PaleGunProj>(), lastdamage, 8, Owner.whoAmI, 0);

                for (int j = 0; j < 4; j++) {
                    int num = Dust.NewDust(AttackPos2 + new Vector2(0, -4) + nowvel * 46f, 0, 0, ModContent.DustType<PaleDust>(), 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = nowvel.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
                    Main.dust[num].velocity *= Main.rand.NextFloat(1f, 6f);
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
                float vel = Main.rand.NextFloat(0.9f, 1.15f) * 12f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
            
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2 + nowvel * 32f, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<PaleGunProj>(), lastdamage, 8, Owner.whoAmI, 1);
            
                for (int j = 0; j < 4; j++) {
                    int num = Dust.NewDust(AttackPos2 + new Vector2(0, -4) + nowvel * 46f, 0, 0, ModContent.DustType<PaleDust>(), 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = nowvel.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
                    Main.dust[num].velocity *= Main.rand.NextFloat(1f, 4f);
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

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleClockworkGunDT2").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(-4, -45);
            //枪管后坐力机制,开枪后枪管在4帧内后移,8帧后弹回
            Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
            Vector2 Recoilfix = Vector2.Zero;
            if (Recoil == 0) Recoilfix = Vector2.Zero;
            if (Recoil > 6) Recoilfix = (9 - Recoil) * -nowvel * 1;//最大后移8
            if (Recoil > 0 && Recoil <= 6) Recoilfix = (Recoil / 6f) * -nowvel * 2 * 1;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, new Color(255, 255, 255, 200), Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, drawPosition + Recoilfix, null, lightColor, wrotation, texture.Size() * 0.5f + cen, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPosition + Recoilfix, null, new Color(255, 255, 255, 150), wrotation, texture.Size() * 0.5f + cen, Projectile.scale, spriteEffects, 0);






            //Todo,RT2D学习实例
            Texture2D texturess = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusProj01").Value;

            ////固定
            //AdDraw(
            //    () => Main.EntitySpriteDraw(texturess, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 255), drawcount / 60f * 1f, texturess.Size() * 0.5f, 1f, spriteEffects, 0)
            //    );
            //
            ////跟随玩家
            //AdDraw(
            //    () => Main.EntitySpriteDraw(texturess, Owner.Center - Main.screenPosition, null, new Color(255, 255, 255, 255), drawcount / 60f * 1f, texturess.Size() * 0.5f, 0.4f, spriteEffects, 0)
            //    );


            return false;
        }
    }

    public class PaleGunProj : GOGProj {
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

            if (count == 1) {
                if (Projectile.ai[0] == 1) {
                    Projectile.extraUpdates = 1;
                    Projectile.penetrate = 3;
                }

                if (Projectile.ai[0] == 2) {
                    Projectile.extraUpdates = 2;
                    Projectile.penetrate = 3;
                }

                if (Projectile.ai[0] == 3) {
                    Projectile.extraUpdates = 2;
                    Projectile.penetrate = 1;
                }
            }

            if (Projectile.ai[0] == 2) {
                if (count <= 30 && count % 5 == 0) Projectile.damage = (int)(Projectile.damage * 0.8f);

                for (int j = 0; j < 1; j++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemDiamond, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Main.dust[num].velocity *= Main.rand.NextFloat(4f, 12f);
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[0] == 3) {
                if (Vector2.Distance(startpos, Projectile.Center) > Projectile.ai[1] - 160) {

                    for (int j = 0; j < 4; j++) {
                        Projectile proj2 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center,
                            Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * 12,
                            ModContent.ProjectileType<PaleGunProj>(), Projectile.damage, 8, Owner.whoAmI, 2);
                    }

                    Projectile.Kill();
                }

                for (int j = 0; j < 4; j++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemDiamond, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
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
            if (Projectile.ai[0] != 3) {
                for (int j = 0; j < 4; j++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<PaleDust>(), 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-1f, 1f));
                    Main.dust[num].velocity *= Main.rand.NextFloat(3f, 4f);
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[0] == 3) {
                //爆炸
                for (int j = 0; j < 13; j++) {
                    int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0, 0, 0, Color.White, 2f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = new Vector2((float)Math.Sin(j * 48 / 100f), (float)Math.Cos(j * 48 / 100f)) * Main.rand.NextFloat(3f, 6f);

                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0, 0, 0, Color.White, 1f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 48 / 100f), (float)Math.Cos(j * 48 / 100f)) * Main.rand.NextFloat(4f, 6f);
                }
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 40;
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
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunProj").Value;
            Texture2D texture11 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusBlackShadow").Value;
            Texture2D texture12 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusShadow1").Value;
            Texture2D texture13 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Rhombus").Value;

            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunProj3").Value;

            if (Projectile.ai[0] == 0) {
                Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    new Color(255, 255, 255, 200) * 0.6f,
                    Projectile.rotation,
                    texture.Size() / 2,
                    Projectile.scale * new Vector2(1f, 1.4f),
                    SpriteEffects.None,
                    0);

                Main.EntitySpriteDraw(
                    texture13,
                    Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * 4,
                    null,
                    new Color(200, 200, 200, 0) * 0.4f,
                    Projectile.rotation,
                    texture13.Size() / 2,
                    Projectile.scale * new Vector2(0.6f, 0.4f),
                    SpriteEffects.None,
                    0);
            }

            if (Projectile.ai[0] == 1) {
                for (int k = 0; k < Projectile.oldPos.Length - 2; k++) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(0f, Projectile.gfxOffY);
                    float color = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.4f;
                    if (k != 0) color = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.4f;
                    if (k == 0) color = 1;

                    Main.EntitySpriteDraw(
                        texture11,
                        drawPos,
                        null,
                        new Color(255, 255, 255, 100) * color,
                        Projectile.rotation,
                        texture11.Size() / 2,
                        Projectile.scale * new Vector2(0.6f, 0.4f),
                        SpriteEffects.None,
                        0);

                    Main.EntitySpriteDraw(
                        texture12,
                        drawPos,
                        null,
                        new Color(150, 150, 150, 0) * color,
                        Projectile.rotation,
                        texture12.Size() / 2,
                        Projectile.scale * new Vector2(0.6f, 0.4f),
                        SpriteEffects.None,
                        0);

                    Main.EntitySpriteDraw(
                        texture13,
                        drawPos,
                        null,
                        new Color(200, 200, 200, 0) * color,
                        Projectile.rotation,
                        texture13.Size() / 2,
                        Projectile.scale * new Vector2(0.6f, 0.4f),
                        SpriteEffects.None,
                        0);
                }
            }

            if (Projectile.ai[0] == 2) {
                Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    new Color(255, 255, 255, 200) * 0.2f,
                    Projectile.rotation,
                    texture.Size() / 2,
                    Projectile.scale * new Vector2(1.4f, 2f) * 1.4f,
                    SpriteEffects.None,
                    0);

                Main.EntitySpriteDraw(
                    texture13,
                    Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * 4,
                    null,
                    new Color(200, 200, 200, 0) * 2f,
                    Projectile.rotation,
                    texture13.Size() / 2,
                    Projectile.scale * new Vector2(1f, 0.4f) * 1.4f,
                    SpriteEffects.None,
                    0);
            }

            if (Projectile.ai[0] == 3) {
                Main.EntitySpriteDraw(
                    texture13,
                    Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * -4,
                    null,
                    new Color(200, 200, 200, 0) * 4f,
                    Projectile.rotation,
                    texture13.Size() / 2,
                    Projectile.scale * new Vector2(0.8f, 0.4f) * 1f,
                    SpriteEffects.None,
                    0);

                Main.EntitySpriteDraw(
                    texture2,
                    Projectile.Center - Main.screenPosition,
                    null,
                    new Color(255, 255, 255, 200) * 1f,
                    Projectile.rotation,
                    texture2.Size() / 2,
                    Projectile.scale * new Vector2(1.2f, 0.8f) * 1.4f,
                    SpriteEffects.None,
                    0);
            }

            return false;
        }
    }
}
