using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Dusts;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static GloryofGuardian.Common.GOGUtils;

namespace GloryofGuardian.Content.Projectiles {
    public class PlanteraDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 64;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.scale *= 1.4f;

            OtherHeight = 80;

            count0 = 60;

            exdust = DustID.Plantera_Green;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -60);

            //大范围的毒
            {
                // 遍历并选取 NPC
                if (drawcount > 60) {
                    for (int i = 0; i < Main.maxNPCs; i++) {
                        NPC npc = Main.npc[i];
                        if (npc != null && npc.active && !npc.friendly
                            && npc.type != NPCID.CultistDevote
                            && npc.type != NPCID.CultistArcherBlue
                            && Vector2.Distance(Projectile.Center, npc.Center) < 2000) {
                            npc.AddBuff(BuffID.Poisoned, 60);
                            npc.AddBuff(BuffID.Venom, 60);

                            if (drawcount % 60 == 0) {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), npc.Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 16)), Vector2.Zero, ModContent.ProjectileType<ChlorophyteProj>(), 1, 8, Owner.whoAmI, 1, npc.whoAmI);
                            } 

                            for (int j = 0; j < 1; j++) {
                                int num1 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Plantera_Pink, 0f, 0f, 10, Color.White, 1f);
                                Main.dust[num1].noGravity = true;
                                Main.dust[num1].velocity = new Vector2(0, -2);
                            }

                            if (count % 5 == 0) {
                                for (int j = 0; j < 1; j++) {
                                    int num1 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Plantera_Green, 0f, 0f, 10, Color.White, 1f);
                                    Main.dust[num1].noGravity = false;
                                    Main.dust[num1].velocity *= 0.5f;
                                }
                            }
                        }
                    }
                }
            }

            //自动的荆棘缠绕
            if (drawcount % Gcount == 0) {
                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit9, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, 0), new Vector2(4, 0), ModContent.ProjectileType<PlanteraProj0>(), lastdamage, 1, Owner.whoAmI);
                    Projectile proj3 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, 0), new Vector2(-4, 0), ModContent.ProjectileType<PlanteraProj0>(), lastdamage, 1, Owner.whoAmI);
                }
            }

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < Main.rand.Next(2, 4); i++) {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                    AttackPos + new Vector2(14, 0),
                    new Vector2(0, -12).RotatedBy(Main.rand.NextFloat(-1f, 1f)),
                    ModContent.ProjectileType<PlanteraSeedProj>(), lastdamage, 8, Owner.whoAmI, 0, Main.rand.Next(15), Main.rand.NextFloat(-0.05f, 0.05f));
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < Main.rand.Next(2, 4); i++) {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                    AttackPos + new Vector2(14, 0),
                    new Vector2(0, -12).RotatedBy(Main.rand.NextFloat(-1f, 1f)),
                    ModContent.ProjectileType<PlanteraSeedProj>(), lastdamage, 8, Owner.whoAmI, 1, Main.rand.Next(15), Main.rand.NextFloat(-0.05f, 0.05f));
            }

            if ((Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + othercrit < 100 && (Owner.ownedProjectileCounts[ModContent.ProjectileType<PlanteraCallProj>()] == 0))
                || (Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + othercrit >= 100 && (Owner.ownedProjectileCounts[ModContent.ProjectileType<PlanteraCallProj>()] < 2))){

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                    AttackPos + new Vector2(14, 0),
                    new Vector2(0, -12).RotatedBy(Main.rand.NextFloat(-1f, 1f)),
                    ModContent.ProjectileType<PlanteraCallProj>(), lastdamage, 8, Owner.whoAmI);

            }

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -34);

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class PlanteraCallProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 30;
            Projectile.height = 28;

            Projectile.localNPCHitCooldown = 12;
            Projectile.penetrate = -1;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 1.5f;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        NPC target1;
        int reboundcount = 0;
        public override void AI() {
            if (Projectile.ai[0] == 0 && count <= 60) reboundcount = 5;

            Projectile.rotation += 0.1f;
            Projectile.velocity.Y += 0.35f;

            //索敌和弱追踪
            if (target0 != null && target0.active) {
                target1 = target0;
            }
            else target1 = null;

            if (target1 != null && target1.active) {
                //横向方向矫速
                if (Projectile.velocity.X * (Projectile.Center.Toz(target1.Center).X) < 1) {
                    //Projectile.velocity *= new Vector2(0, 0.999f);
                    Projectile.velocity += new Vector2(Projectile.Center.Toz(target1.Center).X * 0.1f, 0);
                }
            }

            if (count > 1200) Projectile.Kill();
            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            // 遍历并选取 NPC
            if (drawcount > 60) {
                for (int i = 0; i < Main.maxNPCs; i++) {
                    NPC npc = Main.npc[i];
                    if (npc != null && npc.active && !npc.friendly
                        && npc.type != NPCID.CultistDevote
                        && npc.type != NPCID.CultistArcherBlue
                        && Vector2.Distance(Projectile.Center, npc.Center) < 160) {

                        //垂直的
                        Vector2 vec = Vector2.Zero;
                        //碰撞竖直墙壁
                        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                            vec = (oldVelocity.X > 0) ? new Vector2(-1, 0) : new Vector2(1, 0);
                        }
                        //碰撞水平地面
                        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                            vec = (oldVelocity.Y > 0) ? new Vector2(0, -1) : new Vector2(0, 1);
                        }

                        if (vec != Vector2.Zero) Attack(true, vec);
                    }
                }
            }

            reboundcount -= 1;//反弹次数
            if (reboundcount <= 0) {
                Projectile.Kill();
            }
            else {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                    Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
                }
            }

            return false;
        }

        /// <summary>
        /// 生长荆棘
        /// </summary>
        /// <param name="velorpos">true为垂直型,直接使用注入的vel;false为导向型,从弹幕尾部指向vek</param>
        /// <param name="vel"></param>
        void Attack(bool velorpos, Vector2 vel) {
            //在目标点范围内寻找最近的实心物块,并返回它们的集合
            Vector2 cen = velorpos ? Projectile.Center : vel;
            List<Vector2> tileCoordsList = TileHelper.FindTilesInRectangle(cen, 6, 6, 0, 8, true);
            if (tileCoordsList != null && tileCoordsList.Count != 0) {
                Random random = new Random();
                int countToReturn = Math.Min(random.Next(2, 3), tileCoordsList.Count);

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                for (int i = 0; i < countToReturn; i++) {
                    int randomIndex = random.Next(tileCoordsList.Count);
                    // 获取选中的元素,向下偏移一格多
                    Vector2 selectedTile = tileCoordsList[randomIndex] + new Vector2(0, 16);
                    // 对选中的元素执行A()方法
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), selectedTile,
                    velorpos ? vel : selectedTile.Toz(vel)
                        , ModContent.ProjectileType<PlanteraProj2>(), Projectile.damage, 8, Owner.whoAmI);
                }
            }
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Pink, 0f, 0f, 10, Color.White, 2f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green, 0f, 0f, 10, Color.White, 2f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraCallProj").Value;

            Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0); ;

            return false;
        }
    }

    public class PlanteraProj0 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 12;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = false;

            Projectile.light = 1f;

            Projectile.scale *= 1.5f;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Drop();

            if (count % 4 == 0) {
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, Main.rand.Next(-4, 4)), new Vector2(0, -1), ModContent.ProjectileType<PlanteraProj>(), Projectile.damage, 1, Owner.whoAmI, count % 8);
            }

            if (Projectile.ai[0] == 1 && !Owner.ZoneJungle && count > 30) Projectile.Kill();
            if (Projectile.ai[0] == 1 && Owner.ZoneJungle && count > 45) Projectile.Kill();

            if (!Owner.ZoneJungle && count > 60) Projectile.Kill();
            if (Owner.ZoneJungle && count > 90) Projectile.Kill();

            base.AI();
        }

        bool drop = true;
        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile && Main.tileSolid[tile0.TileType]) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 1) * 16);
                        break;
                    }
                }
            }
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
            return false;
            return base.PreDraw(ref lightColor);
        }
    }

    public class PlanteraProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 6;//动图帧数

            Projectile.width = 30;
            Projectile.height = 48;

            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;

            Projectile.light = 1f;

            Projectile.scale *= 0.6f;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        int drawtype = 0;
        public override void AI() {
            //if(drawcount ==1) {
            //    for (int i = 0; i < 20; i++) {
            //        int num = Dust.NewDust(Projectile.position + new Vector2(8, 0), 12, Projectile.height, DustID.CorruptGibs, 0f, 0f, 50, Color.White, 1f);
            //        Main.dust[num].velocity = new Vector2(0, -2);
            //        Main.dust[num].noGravity = true;
            //    }
            //}

            if (drawcount == 1) Projectile.scale *= Main.rand.NextFloat(0.8f, 1.2f);

            if (mode == 0 && count == 0) {
                Projectile.Center += new Vector2(0, -16);
            }
            count++;

            Projectile.velocity *= 0;

            //生长效果
            if(mode == 0){
                if (count <= 10) grow = Math.Max(0.4f, count / 10f);
                if (count >= 55) grow = Math.Max(0.4f, (65 - count) / 10f);
                if (count >= 60) Projectile.Kill();
            }
            if(mode == 1) {
                if (count <= 10) grow = Math.Max(0.4f, count / 10f);
            }

            if (mode == 1 && count > 180) Projectile.Kill();
            base.AI();
        }

        int hitnum = 0;
        int height0 = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            hitnum++;
            if (target.knockBackResist > 0) target.velocity *= 0;
            if (target.knockBackResist > 0) target.velocity += new Vector2(0, -height0 / 40f);

            if (hitnum == 2 && mode == 0 && Projectile.ai[0] != 4) {
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, 36),
                    new Vector2(0, -1)
                        , ModContent.ProjectileType<PlanteraProj2>(), Projectile.damage, 8, Owner.whoAmI);
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
            base.OnKill(timeLeft);
        }

        float grow = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture00 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj20").Value;
            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj21").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj22").Value;


            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj01").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj02").Value;

            if (mode == 0) {
                Main.EntitySpriteDraw(
                    texture0,
                    Projectile.position + new Vector2(16, 20) - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture0.Size() / 2,
                    new Vector2(grow, grow * 1.5f) * Projectile.scale,
                    SpriteEffects.None,
                    0);
            }
            return base.PreDraw(ref lightColor);
        }
    }

    public class PlanteraProj2 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 6;//动图帧数

            Projectile.width = 204;
            Projectile.height = 48;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.scale *= 1f;
            drawtype = Main.rand.Next(6);
        }

        //比帧大一的数,用于一次性生成随机帧
        int drawtype = 0;
        float grow = 0;
        public override void AI() {
            Lighting.AddLight(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 96f, 3f, 0.5f, 0.5f);

            if (drawtype == 0) drawtype = Main.rand.Next(6) + 1;
            if (count <= 10) grow = Math.Max(0.4f, count / 10f);
            if (count >= 15) grow = Math.Max(0.4f, (25 - count) / 10f);
            if (count >= 20) Projectile.Kill();

            // 计算点积,速度向量的模,如果点积小于阈值，说明夹角大于45度
            float dotProduct = Vector2.Dot(Projectile.velocity, new Vector2(0, -1));
            float velocityMagnitude = Projectile.velocity.Length();
            float threshold = 0.7071f * velocityMagnitude;
            if (dotProduct < threshold) {
                Projectile.velocity = new Vector2(0, -1).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center -= Projectile.velocity;

            if (count < 15) {
                for (int i = 0; i < 2; i++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.Plantera_Green, 0f, 0f, 50, Color.White, 1f);
                    int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Plantera_Green, 0f, 0f, 50, Color.White, 1.2f);
                    int num2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Plantera_Green, 0f, 0f, 50, Color.White, 1f);
                    //Main.dust[num].velocity = Main.dust[num1].velocity = Main.dust[num2].velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 4f;
                    Main.dust[num].position += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-24, 24);

                    Main.dust[num].velocity *= 2f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num1].velocity *= 1f;
                    Main.dust[num1].noGravity = true;
                    Main.dust[num2].velocity *= 1.2f;
                    Main.dust[num2].noGravity = true;
                }
            }

            if (count < 15) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(60), 0, 0, DustID.Plantera_Green, 0f, 0f, 50, Color.White, 1f);
                    int num1 = Dust.NewDust(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(120), 0, 0, DustID.Plantera_Green, 0f, 0f, 50, Color.White, 1.2f);
                    int num2 = Dust.NewDust(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(180), 0, 0, DustID.Plantera_Green, 0f, 0f, 50, Color.Green, 1f);
                    Main.dust[num].velocity = Main.dust[num1].velocity = Main.dust[num2].velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 4f;
                    Main.dust[num].position += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-24, 24);

                    Main.dust[num].velocity *= 2f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num1].velocity *= 1f;
                    Main.dust[num1].noGravity = true;
                    Main.dust[num2].velocity *= 1.2f;
                    Main.dust[num2].noGravity = true;
                }
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            bool collBool = false;
            float point = 0f;

            int length = Projectile.width;
            Vector2 startPos = Projectile.Center;
            Vector2 endPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * length * Projectile.scale;

            collBool = Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                startPos,
                endPos,
                48,
                ref point
                );

            return collBool;
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraProj03").Value;
            int singleFrameY = texture.Height / Main.projFrames[Type];

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * 0,
                new Rectangle(0, singleFrameY * (drawtype - 1), texture.Width, singleFrameY),//动图读帧,
                lightColor, Projectile.rotation, new Vector2(0, texture.Height / 2 / 6), new Vector2(1, grow), SpriteEffects.None, 0);

            return false;
        }
    }

    //花瓣弹幕
    public class PlanteraSeedProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;

            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 1;
            base.SetProperty();

            Projectile.scale *= 1.2f;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, 1, 1, 1);

            if (Projectile.ai[0] == 0) {
                if ((Projectile.ai[1] == 0 || Projectile.ai[1] == 0.5f) && hadhit) {
                    Projectile.alpha += 9;
                    if (Projectile.alpha >= 234) Projectile.Kill();
                }

                if (Projectile.ai[1] == 0.5f && Vector2.Distance(Projectile.Center, startpos) > 480) Projectile.Kill();
            }

            if (Projectile.ai[0] == 1) {
                Projectile.extraUpdates = 0;
                Projectile.tileCollide = false;
                Projectile.localNPCHitCooldown = 1;
                Projectile.penetrate = 1;
                throughtile = true;

                if (target0 != null && target0.active && count > Projectile.ai[1]) {
                    Projectile.velocity *= 0.95f;
                    Projectile.velocity += Projectile.Center.Toz(target0.Center) * 1f;

                    if ((int)(count / (Projectile.ai[1] * 4)) % 2 == 0) Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[2] * 1.5f);
                    else Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[2] * -1.5f);

                    if (Math.Abs(Projectile.velocity.ToRotation() - Projectile.Center.Toz(target0.Center).ToRotation()) > 1f) {
                        Projectile.velocity *= 0.98f;
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * 1f;
                    }

                    if (Vector2.Distance(Projectile.Center, target0.Center) < 240) {
                        Projectile.velocity *= 0.8f;
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * 3f;
                    }

                }
            }

            base.AI();
        }

        bool hadhit = false;
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
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraSeedProj1").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraSeedProj2").Value;

            if (Projectile.ai[0] == 0) {
                Vector2 drawPos = Projectile.position - Main.screenPosition;

                Main.EntitySpriteDraw(
                    texture0,
                    drawPos,
                    null,
                    lightColor,
                    Projectile.rotation + MathHelper.PiOver2,
                    texture02.Size() / 2,
                    Projectile.scale * new Vector2(0.8f, 0.8f),
                    SpriteEffects.None,
                    0);
            }

            if (Projectile.ai[0] == 1) {
                Vector2 drawPos = Projectile.position - Main.screenPosition;

                Main.EntitySpriteDraw(
                    texture02,
                    drawPos,
                    null,
                    lightColor,
                    Projectile.rotation + MathHelper.PiOver2,
                    texture02.Size() / 2,
                    Projectile.scale * new Vector2(0.8f, 0.8f),
                    SpriteEffects.None,
                    0);

                Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Effects + "Extra_197").Value;
                
                //////////////////拖尾
                {
                    //切回原画布
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                    // 注入
                    List<VertexInfo2> vertices3 = new List<VertexInfo2>();
                    for (int i = 0; i < Projectile.oldPos.Length; i++) {
                        if (Projectile.oldPos[i] != Vector2.Zero) {
                            vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(0, 0) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(-90)),
                            new Vector3(i / 6f, 0, 1 - (i / 6f)), new Color(255, 128, 206) * (1 - 0.18f * i) * 0.6f));
                            vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(0, 0) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(90)),
                            new Vector3(i / 6f, 1, 1 - (i / 6f)), new Color(255, 128, 206) * (1 - 0.18f * i) * 0.6f));
                        }
                    }

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                    if (vertices3.Count >= 3) {
                        for (int i = 0; i < 4; i++) {
                            Main.graphics.GraphicsDevice.Textures[0] = texture3;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices3.ToArray(), 0, vertices3.Count - 2);
                        }
                    }

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                    //切回原画布
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                    vertices3.Clear();
                }
            }
            

            return false;
        }
    }
}
