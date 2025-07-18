using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Dusts;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class PaleShotGunDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 80;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 120;
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

            //1 + 3
            for (int i = 0; i < 1; i++) {
                float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;
                Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item36, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2 + nowvel * 32f,
                    nowvel * vel,
                    ModContent.ProjectileType<PaleGunProj>(), lastdamage, 8, Owner.whoAmI, 2);

                for (int j = 0; j < 3; j++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item36, Projectile.Center);
                    Projectile proj2 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2 + nowvel * 32f,
                        nowvel.RotatedBy(-0.1f + j * 0.1f).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * vel,
                        ModContent.ProjectileType<PaleGunProj>(), lastdamage, 8, Owner.whoAmI, 2);

                    projlist.Add(proj2);
                }

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

            if (Vector2.Distance(AttackPos, target0.Center) < 160) Attack1();
            else {
                for (int i = 0; i < 1; i++) {
                    float vel = Main.rand.NextFloat(0.9f, 1.15f) * 12f;
                    Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item36, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2 + nowvel * 32f, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<PaleGunProj>(), lastdamage, 8, Owner.whoAmI, 3, Vector2.Distance(AttackPos, target0.Center));

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

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleShotGunDT2").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(-4, -45);
            //枪管后坐力机制,开枪后枪管在4帧内后移,8帧后弹回
            Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
            Vector2 Recoilfix = Vector2.Zero;
            if (Recoil == 0) Recoilfix = Vector2.Zero;
            if (Recoil > 6) Recoilfix = (9 - Recoil) * -nowvel * 3f;//最大后移
            if (Recoil > 0 && Recoil <= 6) Recoilfix = (Recoil / 6f) * -nowvel * 2 * 3f;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, new Color(255, 255, 255, 200), Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, drawPosition + Recoilfix, null, lightColor, wrotation, texture.Size() * 0.5f + cen, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPosition + Recoilfix, null, new Color(255, 255, 255, 150), wrotation, texture.Size() * 0.5f + cen, Projectile.scale, spriteEffects, 0);


            return false;
        }
    }
}
