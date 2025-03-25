using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ImmortalLotusProj : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = false;

            Projectile.light = 1f;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int count2 = 0;
        int count3 = 0;

        //状态机
        int mode = 0;
        bool orbitPhase = false;//圆周运动状态

        int rcolor = 0;
        NPC tar0 = null;
        Player player120 = null;
        float sx = 0;
        Vector2 sxpos = new Vector2(0, 0);

        Vector2 orinpos = new Vector2(0, 0);
        Vector2 orinvec = new Vector2(0, 0);

        public override void AI() {
            count++;

            float t = (drawcount % 30) / 30f; // t在 [0, 1) 范围内循环
            Color rainbowColor = WhiteToRedGradient(t); // 生成渐变色

            Projectile.rotation = Projectile.velocity.ToRotation();

            //治愈追踪
            if (Projectile.ai[0] == 120) {
                Projectile.friendly = false;

                player120 = FindLowestHealthPlayer();

                //旋量
                float smoothFactor = Main.rand.NextFloat(0.15f, 0.2f) + count / 2400f;
                float speed = 16f;

                if (player120 != null && player120.active && count > 10) {
                    Vector2 projectilePosition = Projectile.Center;
                    Vector2 targetPosition = player120.Center;

                    Vector2 direction = targetPosition - projectilePosition;
                    direction.Normalize();

                    Vector2 currentVelocity = Projectile.velocity;
                    currentVelocity.Normalize();

                    Vector2 newVelocity = Vector2.Lerp(currentVelocity, direction, smoothFactor);
                    newVelocity.Normalize();
                    Projectile.velocity = newVelocity * speed;
                }

                if (player120 != null && player120.active && Vector2.Distance(Projectile.Center, player120.Center) < 120) {
                    Projectile.velocity = Projectile.Center.Toz(player120.Center) * speed;
                }

                if (player120 != null && player120.active && Vector2.Distance(Projectile.Center, player120.Center) < 16) {
                    player120.statLife += 20;
                    CombatText.NewText(player120.Hitbox,//跳字生成的矩形范围
                            Color.LightGreen,//跳字的颜色
                            "20",//这里是你需要展示的文字
                            false,//dramatic为true可以使得字体闪烁，
                            false //dot为true可以使得字体略小，跳动方式也不同(原版debuff扣血格式)
                            );
                    Projectile.Kill();
                }
            }

            //常态追踪
            if (Projectile.ai[0] == 0
                || Projectile.ai[0] == 1) {

                NPC target1 = null;
                if (Projectile.ai[0] == 0) {
                    target1 = Projectile.Center.InPosClosestNPC(3000, true, true);
                    Projectile.penetrate = 1;
                }

                if (Projectile.ai[0] == 1) {
                    for (int i = 0; i < Main.maxNPCs; i++) {
                        NPC target0 = Projectile.Center.InPosClosestNPC(3000, true, true);
                        if (target0 != null && target0.active && !target0.HasBuff(ModContent.BuffType<WitheredDebuff>())) {
                            target1 = target0;
                            break;
                        }
                    }
                    if (target1 == null) target1 = Projectile.Center.InPosClosestNPC(3000, true, true);
                }

                //旋量
                float smoothFactor = Main.rand.NextFloat(0.15f, 0.2f) + count / 2400f;
                float speed = 16f + count / 6f;

                if (target1 != null && target1.active && count > 10) {
                    tar0 = target1;

                    Vector2 projectilePosition = Projectile.Center;
                    Vector2 targetPosition = target1.Center;

                    Vector2 direction = targetPosition - projectilePosition;
                    direction.Normalize();

                    Vector2 currentVelocity = Projectile.velocity;
                    currentVelocity.Normalize();

                    Vector2 newVelocity = Vector2.Lerp(currentVelocity, direction, smoothFactor);
                    newVelocity.Normalize();
                    Projectile.velocity = newVelocity * speed;
                }

                if (target1 != null && target1.active && Vector2.Distance(Projectile.Center, target1.Center) < 120) {
                    Projectile.velocity *= 0.8f;
                    Projectile.velocity += Projectile.Center.Toz(target1.Center) * speed;
                }
            }

            //闪烁
            if (Projectile.ai[0] == 2) {
                ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
                Projectile.friendly = false;
                if (count2 == 0) orinpos = Projectile.Center;
                count2++;
                count3++;
                if (count2 > 1200) Projectile.Kill();

                NPC target1 = Projectile.Center.InPosClosestNPC(3000, true, true);

                if (target1 != null && target1.active && count > 10) {
                    if (sx == 0) {
                        sxpos = Projectile.Center + Projectile.Center.To(target1.Center).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.3f, 0.5f);
                        sx = 1.5f;
                        //Projectile.velocity *= 0;
                        count3 = 0;
                    }

                    if (sx == 1.5f) {
                        if (count3 > 10) Projectile.velocity = Projectile.Center.Toz(sxpos) * 12f;
                        if (Vector2.Distance(Projectile.Center, sxpos) < 16) sx = 2;
                    }

                    if (sx == 2) {
                        sxpos = Projectile.Center + Projectile.Center.To(target1.Center).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.45f, 0.55f);
                        sx = 2.5f;
                        //Projectile.velocity *= 0;
                        count3 = 0;
                    }

                    if (sx == 2.5f) {
                        if (count3 > 10) Projectile.velocity = Projectile.Center.Toz(sxpos) * 12f;
                        if (Vector2.Distance(Projectile.Center, sxpos) < 16) sx = 3;
                    }

                    if (sx == 3) {
                        Projectile.ai[1] = Main.rand.Next(2);
                        if (Projectile.ai[1] == 0) sxpos = orinpos + orinpos.To(target1.Center).RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f)) * Main.rand.NextFloat(0.7f, 0.8f);
                        if (Projectile.ai[1] == 1) sxpos = orinpos + orinpos.To(target1.Center);
                        sx = 3.5f;
                        //Projectile.velocity *= 0;
                        count3 = 0;
                    }

                    if (sx == 3.5f) {
                        if (count3 > 10) Projectile.velocity = Projectile.Center.Toz(sxpos) * 12f;
                        if (Vector2.Distance(Projectile.Center, sxpos) < 16) {

                            if (Projectile.ai[1] == 0) {
                                float b = Main.rand.NextFloat(0.05f, 0.15f);
                                for (int i = 0; i < 5; i++) {
                                    Vector2 vel = Projectile.Center.Toz(target1.Center) * 64f;
                                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, vel.RotatedBy((i - 2) * b), ModContent.ProjectileType<ImmortalLotusProj>(), Projectile.damage / 2, 2, Owner.whoAmI, -2);

                                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                        if (proj1.ModProjectile is GOGProj proj2) {
                                            proj2.OrichalcumMarkProj = true;
                                            proj2.OrichalcumMarkProjcount = 300;
                                        }
                                    }
                                }
                            }

                            if (Projectile.ai[1] == 1) {
                                for (int i = 0; i < 5; i++) {
                                    Vector2 vel = Projectile.Center.Toz(target1.Center) * 24f;
                                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, vel.RotatedBy(i * MathHelper.Pi * 2 / 5f), ModContent.ProjectileType<ImmortalLotusProj>(), Projectile.damage / 2, 2, Owner.whoAmI, -2);

                                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                        if (proj1.ModProjectile is GOGProj proj2) {
                                            proj2.OrichalcumMarkProj = true;
                                            proj2.OrichalcumMarkProjcount = 300;
                                        }
                                    }
                                }
                            }

                            Projectile.Kill();
                        }
                    }
                }
            }

            //衍生-旋转
            if (Projectile.ai[0] == -1) {
                if (count2 == 0) {
                    orinpos = Projectile.Center;
                    orinvec = Projectile.velocity;
                    //Projectile.velocity *= 0.1f;
                }

                //if(count % 10 == 0)
                count2++;

                if (!orbitPhase) {
                    Projectile.velocity = Projectile.velocity.RotatedBy(-0.1f - count2 / 90f);
                    if (Vector2.Distance(Projectile.Center, orinpos) >= 80) {
                        // 获取指向圆心的方向
                        Vector2 toCenter = orinpos - Projectile.Center;
                        Vector2 tangent = toCenter.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);

                        // 保持原速度大小，方向改为切线方向
                        float speed = Projectile.velocity.Length();
                        Projectile.velocity = tangent * speed;

                        orbitPhase = true;
                    }
                } else {
                    // 初始自由飞行阶段的随机转向（示例）
                    Projectile.velocity = Projectile.velocity.RotatedBy(-0.1f);
                }

                foreach (NPC npc in Main.npc) {
                    if (npc != null && npc.active && !npc.boss && npc.knockBackResist == 0 && Vector2.Distance(Projectile.Center, npc.Center) < 120) {
                        if (count2 % 6 == 0) {
                            npc.velocity *= 0.98f;
                        }

                        if (count % 12 == 0) {
                            int num1 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RainbowMk2, 0f, 0f, 10, rainbowColor, 1f);
                            Main.dust[num1].noGravity = true;
                            Main.dust[num1].velocity *= 1f;
                        }
                    }

                    if (npc != null && npc.active && !npc.boss && npc.knockBackResist > 0 && Vector2.Distance(Projectile.Center, npc.Center) < 120) {
                        npc.velocity *= 0.9f;
                        if (Vector2.Distance(npc.Center, orinpos) > 16) npc.Center += npc.Center.Toz(orinpos) * 1f;


                        if (count % 12 == 0) {
                            int num1 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RainbowMk2, 0f, 0f, 10, rainbowColor, 1f);
                            Main.dust[num1].noGravity = true;
                            Main.dust[num1].velocity *= 1f;
                        }
                    }
                }

                if (count2 > 120) Projectile.Kill();
            }

            //衍生-直线
            if (Projectile.ai[0] == -2) {
                Projectile.penetrate = -1;
                ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
                Projectile.velocity *= 0.9f;
                if (count > 30) Projectile.Kill();
            }

            // 弹幕的发光效果
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1f); // 添加淡蓝色光效

            // 弹幕的粒子效果
            float tt = (drawcount % 30) / 30f; // t在 [0, 1) 范围内循环
            Color rainbow = Color.White; // 生成渐变色

            if (Projectile.ai[0] * Projectile.ai[0] == 1) rainbow = WhiteToRedGradient(tt);
            if (Projectile.ai[0] * Projectile.ai[0] == 4) rainbow = WhiteToBlueGradient(tt);
            if (Projectile.ai[0] == 120) rainbow = WhiteToGreenGradient(tt);

            Dust0(rainbow);
        }

        void Dust0(Color color) {
            if (Main.rand.NextBool(2)) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 10, color, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 1f;
            }
        }

        Player FindLowestHealthPlayer() {
            Player lowestHealthPlayer = null;
            float lowestHealth = float.MaxValue;

            for (int i = 0; i < Main.maxPlayers; i++) {
                Player player = Main.player[i];

                // 检查玩家是否活跃且存活
                if (player.active && !player.dead) {
                    // 获取玩家当前血量
                    float currentHealth = player.statLife;

                    // 更新最低血量玩家
                    if (currentHealth < lowestHealth) {
                        lowestHealth = currentHealth;
                        lowestHealthPlayer = player;
                    }
                }
            }

            return lowestHealthPlayer;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.damage = (int)(Projectile.damage * 0.8f);

            if ((tar0 != null && tar0.active && target != tar0) || target.type == NPCID.TargetDummy
                && Projectile.penetrate != -1 && Projectile.ai[0] != 0) Projectile.penetrate += 1;

            if ((tar0 != null && tar0.active && target == tar0) && Projectile.ai[0] == 1) {
                int dtnum0 = 0;
                foreach (var proj in Main.projectile)//遍历所有弹幕
                {
                    if (proj.active //活跃状态
                        && proj.type == ModContent.ProjectileType<ImmortalLotusProj>() //同类
                        && Vector2.Distance(Projectile.Center, proj.Center) < 180
                        ) {
                        //总数
                        dtnum0++;
                    }
                }

                if (dtnum0 <= 18) {
                    Vector2 projcen = target.Center;
                    Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;

                    for (int i = 0; i < 3; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, vel.RotatedBy(MathHelper.Pi * 2 / 3 * i), ModContent.ProjectileType<ImmortalLotusProj>(), Projectile.damage / 2, 2, Owner.whoAmI, -1);
                        proj1.penetrate = -1;
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                Projectile.Kill();
            }

            if (Projectile.ai[0] == -1) {
                target.AddBuff(ModContent.BuffType<WitheredDebuff>(), 12);
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (Projectile.ai[0] == -1) {
                if (GOGUtils.CircularHitboxCollision(orinpos, 90, targetHitbox)) {
                    return true;
                } else return base.Colliding(projHitbox, targetHitbox);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnKill(int timeLeft) {
            drawcount = Main.rand.Next(20, 60);
            float hue = (drawcount % 60) / 60f;
            Color rainbowColor = WhiteToRedGradient(hue);
            if (Projectile.ai[0] * Projectile.ai[0] == 1) rainbowColor = WhiteToRedGradient(hue);
            if (Projectile.ai[0] * Projectile.ai[0] == 4) rainbowColor = WhiteToBlueGradient(hue);
            if (Projectile.ai[0] == 120) rainbowColor = WhiteToGreenGradient(hue);

            for (int i = 0; i < 12; i++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 10, rainbowColor, 1f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 4f;
            }

            for (int i = 0; i < 6; i++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 10, rainbowColor, 1.5f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        int drawcount = 0;
        Color rainbowColor;
        public override bool PreDraw(ref Color lightColor) {
            drawcount++;
            if (drawcount >= 30) drawcount = 0;
            float t = (drawcount % 30) / 30f; // t在 [0, 1) 范围内循环
            rainbowColor = Color.White; // 生成渐变色

            if (Projectile.ai[0] * Projectile.ai[0] == 1) rainbowColor = WhiteToRedGradient(t);
            if (Projectile.ai[0] * Projectile.ai[0] == 4) rainbowColor = WhiteToBlueGradient(t);
            if (Projectile.ai[0] == 120) rainbowColor = WhiteToGreenGradient(t);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusProj1").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            //残影
            for (int i = 0; i < 1; i++) {
                float scale = 1f - (i / (float)Projectile.oldPos.Length);
                Vector2 drawPosition = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Main.spriteBatch.Draw(texture, drawPosition, null, rainbowColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale * scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPosition0, null, rainbowColor * (rcolor / 100f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Shader部分//

            //注入
            List<VertexInfo2> vertices = new List<VertexInfo2>();
            for (int i = 0; i < Projectile.oldPos.Length; i++) {
                if (Projectile.oldPos[i] != Vector2.Zero) {
                    vertices.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(12, 12) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(-90)),
                    new Vector3(i / 8f, 0, 1 - (i / 8f)), rainbowColor * (1 - 0.12f * i)));
                    vertices.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(12, 12) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(90)),
                    new Vector3(i / 8f, 1, 1 - (i / 8f)), rainbowColor * (1 - 0.12f * i)));
                }

                int num1 = Dust.NewDust(Projectile.oldPos[i] - Main.screenPosition + new Vector2(30 - i, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(-90)), 0, 0, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 50f;
            }

            //绘制
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp,
            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(GOGConstant.Effects + "Extra_197").Value;

            if (vertices.Count >= 3) {
                for (int i = 0; i < 4; i++) {
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        // 生成白色和红色之间的渐变色
        Color WhiteToRedGradient(float t) {
            t = t % 1f; // 确保t在 [0, 1) 范围内
            if (t < 0) t += 1f;

            // 在白色 (1,1,1) 和红色 (1,0,0) 之间插值
            float r = 1f;          // 红色通道始终为1
            float g = 1f - t;      // 绿色通道从1降到0
            float b = 1f - t;      // 蓝色通道从1降到0

            return new Color(r, g, b);
        }

        // 生成白色和蓝色之间的渐变色
        Color WhiteToBlueGradient(float t) {
            t = t % 1f; // 确保t在 [0, 1) 范围内
            if (t < 0) t += 1f;

            // 在白色 (1,1,1) 和红色 (1,0,0) 之间插值
            float r = 1f - t;          // 红色通道始终为1
            float g = 1f - t;      // 绿色通道从1降到0
            float b = 1f;      // 蓝色通道从1降到0

            return new Color(r, g, b);
        }

        // 生成白色和绿色之间的渐变色
        Color WhiteToGreenGradient(float t) {
            t = t % 1f; // 确保t在 [0, 1) 范围内
            if (t < 0) t += 1f;

            // 在白色 (1,1,1) 和红色 (1,0,0) 之间插值
            float r = 1f - t;          // 红色通道始终为1
            float g = 1f;      // 绿色通道从1降到0
            float b = 1f - t;      // 蓝色通道从1降到0

            return new Color(r, g, b);
        }

        Color HsvToRgb(float h, float s, float v) {
            h = h % 1f; // 确保色相在 [0, 1) 范围内
            if (h < 0) h += 1f;

            int hi = (int)(h * 6);
            float f = h * 6 - hi;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            switch (hi) {
                case 0: return new Color(v, t, p);
                case 1: return new Color(q, v, p);
                case 2: return new Color(p, v, t);
                case 3: return new Color(p, q, v);
                case 4: return new Color(t, p, v);
                default: return new Color(v, p, q);
            }
        }

        //Shader结构体
        public struct VertexInfo2 : IVertexType
        {
            private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
            {
            new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
            new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
            new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
            });
            public Vector2 Position;
            public Color Color;
            public Vector3 TexCoord;
            public VertexInfo2(Vector2 position, Vector3 texCoord, Color color) {
                Position = position;
                TexCoord = texCoord;
                Color = color;
            }
            public VertexDeclaration VertexDeclaration {
                get => _vertexDeclaration;
            }
        }
    }
}
