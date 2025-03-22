using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SwordNurturingGourdProj : GOGProj
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
            Projectile.localNPCHitCooldown = 30;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 6;//穿透数，1为攻击到第一个敌人就消失
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
        int mode = 0;
        NPC tar0 = null;
        Vector2 totar = new Vector2(0, 0);
        Vector2 firepos2 = new Vector2(0, 0);
        int rcolor = 0;
        public override void AI() {
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] == 0) {
                NPC target1 = Projectile.Center.InPosClosestNPC(3000, true, true);
                //旋量
                float smoothFactor = Main.rand.NextFloat(0.05f, 0.1f) + count / 2400f;
                if (smoothFactor > 0.25f) smoothFactor = 0.25f;
                float speed = 16f + count / 60f;

                if (target1 != null && target1.active && count > 10) {
                    // 获取弹幕和目标的位置
                    Vector2 projectilePosition = Projectile.Center;
                    Vector2 targetPosition = target1.Center;

                    // 计算弹幕到目标的方向
                    Vector2 direction = targetPosition - projectilePosition;
                    direction.Normalize(); // 归一化方向向量

                    // 计算弹幕当前的飞行方向
                    Vector2 currentVelocity = Projectile.velocity;
                    currentVelocity.Normalize();

                    // 使用线性插值（Lerp）平滑地调整弹幕的飞行方向
                    Vector2 newVelocity = Vector2.Lerp(currentVelocity, direction, smoothFactor);
                    newVelocity.Normalize();
                    Projectile.velocity = newVelocity * speed;
                }
            }

            if (Projectile.ai[0] == 1) {
                if (mode == 0) {
                    Projectile.friendly = false;

                    NPC target1 = Projectile.Center.InPosClosestNPC(3000, true, true);

                    if (count > 10) Projectile.velocity *= 0.98f;
                    if (target1 == null) count2 = 0;

                    //为了使攻击不会中断,一旦有敌人存活,我们就会记下它的位置
                    //如果还有敌人存活则更新目标,但是不会停止攻击
                    //如果附近敌人全部被驱逐,则攻击向刚才的位置
                    //如果始终找不到敌人就消失
                    if (target1 != null && target1.active) {
                        tar0 = target1;
                        totar = target1.Center;
                    }

                    if (tar0 != null && totar != new Vector2(0, 0)) {
                        Projectile.rotation = new Vector2(0, -1).ToRotation();
                        count2++;

                        Projectile.alpha = count2 * 5;

                        if (count2 > 60) {
                            mode = 1;
                            count2 = 0;
                            Projectile.alpha = 0;
                            Projectile.Center = totar + new Vector2(0, -540);
                            Projectile.penetrate = -1;
                            Projectile.friendly = true;
                        }
                    }

                    if (target1 == null && count > 120) Projectile.Kill();
                }

                if (mode == 1 || mode == -1) {
                    count2++;

                    if (tar0 != null && tar0.active && count2 < 30) {
                        if (!tar0.boss) Projectile.Center = new Vector2(tar0.Center.X, tar0.Center.Y - 540);
                        if (tar0.boss) Projectile.Center = new Vector2(tar0.Center.X, tar0.Center.Y - tar0.height - 160);
                    }

                    if (count2 < 30) Projectile.velocity = new Vector2(0, 0.1f);
                    if (count2 >= 30) Projectile.velocity = new Vector2(0, 32f);

                    if (Projectile.Center.Y < totar.Y) Projectile.tileCollide = false;
                    if (Projectile.Center.Y >= totar.Y) Projectile.tileCollide = true;

                    //打不到物块,触发飞剑剑阵
                    if (count2 >= 80) {
                        if (tar0 != null && tar0.active) ConcentrateAttack(tar0);
                        Projectile.Kill();
                    }
                }
            }

            if (Projectile.ai[0] == 2) {
                count2++;
            }

            if (count > 70) rcolor += 10;
            if (rcolor > 200) rcolor = 200;

            // 弹幕的发光效果
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1f); // 添加淡蓝色光效

            // 弹幕的粒子效果
            //if (Main.rand.NextBool(5)) {
            //    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.PinkFairy, Projectile.velocity * 0.5f);
            //    dust.noGravity = true;
            //}
        }

        void ConcentrateAttack(NPC target) {
            float a = Main.rand.NextFloat(0, 1.57f);
            for (int i = 0; i < 4; i++) {
                Vector2 projcen = target.Center + new Vector2(0, 600).RotatedBy(MathHelper.PiOver2 * i + a);
                Vector2 velfire = projcen.Toz(target.Center) * 64f;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<SwordNurturingGourdWarningLine>(), Projectile.damage, 2, Owner.whoAmI, 2);

                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
                    }
                }

                if (proj1.ModProjectile is SwordNurturingGourdWarningLine projw) {
                    projw.npc0 = target;
                    projw.npccen = target.Center;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (mode == 1) {
                if (target.boss) {
                    int maxIterations = 4;

                    for (int i = 0; i < maxIterations; i++) {
                        Attack(maxIterations, i);
                        Main.NewText(4);
                    }

                    ConcentrateAttack(target);

                    Projectile.Kill();
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        List<NPC> selectedNPCs = new List<NPC>();
        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (mode == 1) {
                int maxIterations = 4;

                for (int i = 0; i < maxIterations; i++) {
                    Attack(maxIterations, i);
                }

                Projectile.Kill();
            }
            return base.OnTileCollide(oldVelocity);
        }

        void Attack(int maxIterations, int j) {
            float maxDistance = 1200f; // 索敌范围

            // 遍历并选取 NPC
            for (int i = 0; i < maxIterations; i++) {
                NPC nearestNPC = FindNearestNPC(Projectile.Center, maxDistance, selectedNPCs);

                if (nearestNPC != null && nearestNPC.type != NPCID.TargetDummy) {
                    // 将找到的 NPC 添加到已选取列表中
                    selectedNPCs.Add(nearestNPC);
                } else {
                    // 如果没有更多 NPC，退出循环
                    break;
                }
            }

            for (int i = 0; i < maxIterations; i++) {
                if (selectedNPCs.Count == 0) {
                    break; // 如果列表为空，退出循环
                }

                // 从列表中选取 NPC（循环选取）
                NPC npc = selectedNPCs[i % selectedNPCs.Count];
                // 执行攻击
                Vector2 projcen = npc.Center + new Vector2(0, -540);
                Vector2 velfire = new Vector2(0, 48);

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<SwordNurturingGourdProj>(), Projectile.damage, 2, Owner.whoAmI, 1);
                proj1.penetrate = -1;

                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
                    }
                }

                if (proj1.ModProjectile is SwordNurturingGourdProj proj00) {
                    proj00.mode = -1;
                    proj00.totar = totar;
                    proj00.count2 = 0;
                    proj00.drawcount = 30 + j * 10;
                    proj00.tar0 = npc;
                }
            }
        }

        NPC FindNearestNPC(Vector2 point, float maxDistance, List<NPC> excludeList) {
            NPC nearestNPC = null;
            float nearestDistance = maxDistance;

            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC npc = Main.npc[i];

                if (npc != null && npc.active && npc.life > 0 && !excludeList.Contains(npc)) {
                    float distance = Vector2.Distance(point, npc.Center);

                    if (distance < nearestDistance) {
                        nearestNPC = npc;
                        nearestDistance = distance;
                    }
                }
            }

            return nearestNPC;
        }

        public override void OnKill(int timeLeft) {
            int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 2f);
            Main.dust[num2].noGravity = true;
            Main.dust[num2].velocity *= 10f;
        }

        int drawcount = 0;
        public override bool PreDraw(ref Color lightColor) {
            drawcount++;
            if (drawcount >= 60) drawcount = 0;
            float hue = (drawcount % 60) / 60f;
            Color rainbowColor = HsvToRgb(hue, 1f, 1f);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SwordNurturingGourdProj0").Value;
            if (Projectile.ai[0] == 1) texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SwordNurturingGourdProj").Value;
            if (Projectile.ai[0] == 2) texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SwordNurturingGourdProj2").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            //残影
            for (int i = 0; i < 1; i++) {
                float scale = 1f - (i / (float)Projectile.oldPos.Length);
                Vector2 drawPosition = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color color = Color.Lerp(Color.White, Color.Pink, scale) * scale * (rcolor / 100f);
                Main.spriteBatch.Draw(texture, drawPosition, null, rainbowColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPosition0, null, rainbowColor * (rcolor / 100f) * (255 - Projectile.alpha / 255f), rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

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

            if (Projectile.ai[0] == 1 && (mode == 0 || mode != 0 && count2 < 30)) return false;

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

        //生成渐变色
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
