using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class TitaniumDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 80;
            Projectile.height = 50;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;

            //本体具有伤害的炮塔，需要设置无敌帧
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.light = 3f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            mode = 2;//初始冷却
            Calculate();
            count = 0;
            count0 = 40 * 60;//默认发射间隔
            leastdistance = 800;//套盾范围，1.5倍就是检测范围,2倍是挣脱范围
            Gcount = Shieldcd = 3 * 60;
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int countcd = 0;
        //状态机
        int mode = 0;
        int reloadcount = 0;
        int Shieldcd = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int Acount = 0;
        int lastdamage = 0;
        float leastdistance = 0;
        //护盾锁定
        Player god = null;
        bool hadgod = false;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //护盾锁定状态，玩家受击获得另一个标记，玩家寄了将解除护盾并进入冷却
            if (mode == 1) {
                //buff清除，可能原因有:玩家消失，受击获得冷却buff，脱出范围5秒
                if (god == null || !god.active) {
                    mode = 2;
                    god = null;
                    reloadcount = 0;
                }
            
                if (god != null && god.active && god.HasBuff<TitaniumReloadBuff>()) {
                    mode = 2;
                    god = null;
                    reloadcount = 0;
                }
            
                if (god != null && god.active) god.AddBuff(ModContent.BuffType<TitaniumShieldBuff>(), 2);
            
                //范围脱出
                if (god != null && god.active && Vector2.Distance((Projectile.Center + new Vector2(0, -64)), god.Center) <= leastdistance) reloadcount = 0;
                if (god != null && god.active && Vector2.Distance((Projectile.Center + new Vector2(0, -64)), god.Center) > leastdistance && Vector2.Distance((Projectile.Center + new Vector2(0, -64)), god.Center) <= leastdistance * 2f) {
                    if (god != null && god.active && Vector2.Distance((Projectile.Center + new Vector2(0, -64)), god.Center) > leastdistance * 1.5f) reloadcount++;
                    //粒子：连线与玩家身体
                    Vector2 togod = (god.Center - (Projectile.Center + new Vector2(0, -64))).SafeNormalize(Vector2.Zero);
                    Vector2 maxdistance = ((Projectile.Center + new Vector2(0, -64)) + (god.Center - (Projectile.Center + new Vector2(0, -64))).SafeNormalize(Vector2.Zero) * leastdistance);
                    for (int j = 0; j < 12; j++) {
                        Vector2 maxdistancerand = ((Projectile.Center + new Vector2(0, -64)) + (god.Center - (Projectile.Center + new Vector2(0, -64))).SafeNormalize(Vector2.Zero) * leastdistance * Main.rand.NextFloat(0, 1));

                        int num1 = Dust.NewDust(maxdistancerand, 0, 0, DustID.GemDiamond, 0f, 0f, 10, Color.White, 1f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity *= 0;
                    }
                    for (int j = 0; j < 12; j++) {
                        int num1 = Dust.NewDust((maxdistance + (god.Center - maxdistance).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0, Vector2.Distance(god.Center, maxdistance))), 0, 0, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity *= 0f;
                    }
            
                    for (int j = 0; j < 8; j++) {
                        int num1 = Dust.NewDust(god.position, god.width, god.height, DustID.GemDiamond, 0f, 0f, 10, Color.White, 0.8f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity = new Vector2(0, -2);
                    }
                    if (reloadcount >= 300) {
                        god.AddBuff(ModContent.BuffType<TitaniumReloadBuff>(), Shieldcd);
                        god.ClearBuff(ModContent.BuffType<TitaniumShieldBuff>());
                        mode = 2;
                        god = null;
                        reloadcount = 0;
                    }
                }
                if (god != null && god.active && Vector2.Distance((Projectile.Center + new Vector2(0, -64)), god.Center) > leastdistance * 2f) {
                    god.AddBuff(ModContent.BuffType<TitaniumReloadBuff>(), Shieldcd);
                    god.ClearBuff(ModContent.BuffType<TitaniumShieldBuff>());
                    mode = 2;
                    god = null;
                    reloadcount = 0;
                }
            }

            //套盾
            if (mode == 0) {
                Player playgod = null;
                Player willgod = null;
                float distance = leastdistance;

                for (int i = 0; i < Main.maxPlayers; i++) {
                    Player player = Main.player[i];
                    if (player.active && !(player is null) && !player.HasBuff<TitaniumReloadBuff>()) {//安全性检测,不判自己
                        if (Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center) < distance) {//判距离,存储新的最近距离
                            distance = Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center);
                            playgod = player;
                        } else {
                            continue;
                        }
                    }
                }

                if (playgod != null && !playgod.active) playgod = null;
                if (playgod != null && playgod.active) {
                    playgod.AddBuff(ModContent.BuffType<TitaniumShieldBuff>(), 2);//这个buff慢速更新，防止最近位置快速变更
                    god = playgod;
                    mode = 1;
                    playgod = null;
                    willgod = null;
                }

                if (playgod == null) {
                    for (int i = 0; i < Main.maxPlayers; i++) {
                        Player player = Main.player[i];
                        if (player.active && !(player is null)) {//安全性检测,不判自己
                            if (Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center) <= leastdistance * 1.25f && Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center) > leastdistance) {//判距离,存储新的最近距离
                                distance = Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center);
                                willgod = player;
                            } else {
                                continue;
                            }
                        }
                    }

                    if(willgod != null && !playgod.active) playgod = null;
                    if (willgod != null && playgod.active) {
                        //粒子特效提醒
                        Vector2 maxdistance = ((Projectile.Center + new Vector2(0, -64)) + (god.Center - (Projectile.Center + new Vector2(0, -64))).SafeNormalize(Vector2.Zero) * leastdistance);

                        for (int j = 0; j < 30; j++) {
                            Vector2 maxdistancerand = ((Projectile.Center + new Vector2(0, -64)) + (god.Center - (Projectile.Center + new Vector2(0, -64))).SafeNormalize(Vector2.Zero) * Main.rand.Next(0, (int)leastdistance));

                            int num1 = Dust.NewDust(maxdistancerand, 0, 0, DustID.GemDiamond, 0f, 0f, 10, Color.White, 0.8f);
                            Main.dust[num1].noGravity = true;
                            Main.dust[num1].velocity *= 0f;
                        }
                        for (int j = 0; j < 30; j++) {
                            int num1 = Dust.NewDust((maxdistance + (god.Center - maxdistance).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0, Vector2.Distance(god.Center, maxdistance))), 0, 0, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                            Main.dust[num1].noGravity = true;
                            Main.dust[num1].velocity *= 0f;
                        }
                    }

                    //if (willgod == null) //啥也不干
                }
            }

            //重新装填
            if (mode == 2) {
                reloadcount++;
                Player playgod = null;
                Player willgod = null;
                float distance = leastdistance;
            
                for (int i = 0; i < Main.maxPlayers; i++) {
                    Player player = Main.player[i];
                    if (player.active && !(player is null) && !player.HasBuff<TitaniumReloadBuff>()) {//安全性检测,不判自己,避开冷却
                        if (Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center) < leastdistance) {//判距离,存储新的最近距离
                            distance = Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center);
                            playgod = player;
                        } else {
                            continue;
                        }
                    }
                }
            
                if (playgod != null && !playgod.active) playgod = null;
                if (playgod != null && playgod.active) {
                    if (reloadcount % 60 == 0) {
                        //范围内粒子
                        for (int j = 0; j < 240; j++) {
                            Vector2 maxdistancerand = ((Projectile.Center + new Vector2(0, -64)) + (playgod.Center - (Projectile.Center + new Vector2(0, -64))) * Main.rand.NextFloat(0, 1));

                            int num1 = Dust.NewDust(maxdistancerand, 0, 0, DustID.GemDiamond, 0f, 0f, 10, Color.White, 1f);
                            Main.dust[num1].noGravity = true;
                            Main.dust[num1].velocity *= 0;
                        }
                    }

                    if (count > Gcount) {
                        mode = 0;
                        count = 0;
                    }

                    playgod = null;
                    willgod = null;
                }
            
                if (playgod == null) {
                    float distance2 = leastdistance;
                    for (int i = 0; i < Main.maxPlayers; i++) {
                        Player player = Main.player[i];
                        if (player.active && !(player is null) && !player.HasBuff<TitaniumReloadBuff>()) {//安全性检测,不判自己
                            if (Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center) <= distance2 * 1.25f && Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center) > leastdistance) {//判距离,存储新的最近距离
                                distance2 = Vector2.Distance((Projectile.Center + new Vector2(0, -64)), player.Center);
                                willgod = player;
                            } else {
                                continue;
                            }
                        }
                    }
            
                    if (willgod != null && !willgod.active) willgod = null;
                    if (willgod != null && willgod.active) {
                        if (reloadcount % 60 == 0) {
                            //范围外粒子
                            //粒子：连线与玩家身体
                            Vector2 togod = (willgod.Center - (Projectile.Center + new Vector2(0, -64))).SafeNormalize(Vector2.Zero);
                            Vector2 maxdistance = ((Projectile.Center + new Vector2(0, -64)) + (willgod.Center - (Projectile.Center + new Vector2(0, -64))).SafeNormalize(Vector2.Zero) * leastdistance);
                            for (int j = 0; j < 240; j++) {
                                Vector2 maxdistancerand = ((Projectile.Center + new Vector2(0, -64)) + (willgod.Center - (Projectile.Center + new Vector2(0, -64))).SafeNormalize(Vector2.Zero) * leastdistance * Main.rand.NextFloat(0, 1));

                                int num1 = Dust.NewDust(maxdistancerand, 0, 0, DustID.GemDiamond, 0f, 0f, 10, Color.White, 1f);
                                Main.dust[num1].noGravity = true;
                                Main.dust[num1].velocity *= 0;
                            }
                            for (int j = 0; j < 240; j++) {
                                int num1 = Dust.NewDust((maxdistance + (willgod.Center - maxdistance).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0, Vector2.Distance(willgod.Center, maxdistance))), 0, 0, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                                Main.dust[num1].noGravity = true;
                                Main.dust[num1].velocity *= 0f;
                            }

                            count = 0;
                        }

                        willgod = null;
                        willgod = null;
                    }
            
                    //if (willgod == null) //啥也不干
                }
            }

            base.AI();
        }

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
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 2) * 16);
                        break;
                    }
                }
                drop = false;
            }
        }

        /// <summary>
        /// 重新计算和赋值参数
        /// </summary>
        void Calculate() {
            //Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0]);//攻击间隔因子重新提取
            Gcount = 3 * 60;
            //伤害修正
            int newDamage = (int)(Projectile.originalDamage);
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
            //辅助炮塔叠加计算
            Acount = 0;
            foreach (var proj in Main.projectile)//遍历所有弹幕
            {
                if (proj.active && proj.type == Projectile.type) {
                    Acount++;
                }
            }
            if (Acount > 4) Acount = 4;
            //最多叠加四个炮塔的计时,每个给予5秒正面buff时长,每个减少60秒负面buff中的10秒
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return false;
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }

            if (god != null && god.active) {
                god.ClearBuff(ModContent.BuffType<TitaniumShieldBuff>());
            }
        }

        //浮动因子
        int floatcount = 0;
        float float1 = 0;
        float float2 = 0;
        public override bool PreDraw(ref Color lightColor) {
            floatcount++;
            float1 = (float)Math.Sin(floatcount / 12f) + 1;
            float2 = (float)Math.Sin(floatcount / 12f + MathHelper.PiOver2) + 1;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "TitaniumDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "TitaniumDT2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "TitaniumDT3").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "TitaniumDT4").Value;



            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;

            //有一个图层低于炮台本体的部分
            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(14, -4 ) + new Vector2(0, 4 * float1), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            {
                Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(33, -32) + new Vector2(0, -4 * float2), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture3, drawPosition0 + new Vector2(14, -4) + new Vector2(0, 4 * float1), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            //护盾
            Texture2D shieldtexture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "LightPulse").Value;

            if (god != null && god.active) {
                Vector2 shieldpos = god.Center + new Vector2(0, 0) - Main.screenPosition;

                Main.EntitySpriteDraw(shieldtexture, shieldpos + new Vector2(0, -4 * float2), null, new Color(97, 113, 132) * 1f, Projectile.rotation, shieldtexture.Size() * 0.5f, 1.6f, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}