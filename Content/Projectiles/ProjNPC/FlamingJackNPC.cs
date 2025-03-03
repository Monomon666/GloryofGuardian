using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles.ProjNPC
{
    public class FlamingJackNPC : ModNPC
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[NPC.type] = 1;//帧图未加载
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        }

        public override void SetDefaults() {
            NPC.width = 48;
            NPC.height = 48;
            NPC.damage = 100;//普通模式下的碰撞攻击力
            NPC.defense = 0;//防御力(2防=1点免疫)
            NPC.lifeMax = 400;//最大生命值
            NPC.HitSound = SoundID.NPCHit29;//受伤音效
            NPC.DeathSound = SoundID.NPCDeath32;//死亡音效
            NPC.value = Item.buyPrice(1, 0, 0, 0);//掉落金钱（普通模式为基准）
            NPC.knockBackResist = 1f;//击退接受性（0为彻底免疫击退）
            NPC.alpha = 0;//透明度，史莱姆为105
            NPC.noGravity = true;//无重力
            NPC.noTileCollide = false;//穿墙
            NPC.boss = false;//令其为boss
            NPC.npcSlots = 1f; //占用1个NPC槽位
            NPC.lavaImmune = false;//免疫岩浆伤害
            NPC.friendly = true;//敌对关系

            //旗帜
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<EbonianBlightSlimeBanner>();

            NPC.aiStyle = -1;
            AIType = -1;
        }

        public override Color? GetAlpha(Color drawColor) {
            return base.GetAlpha(drawColor);
        }

        int spideremain = 0;
        int spiderdamage = 0;
        NPC target0;
        public override void OnSpawn(IEntitySource source) {
            base.OnSpawn(source);
        }

        int count = 0;
        int noDamageTimer = 0;
        bool drop = true;
        //跳跃状态关闭其它功能以保证正常跳跃
        bool jumpmode = false;
        int jumpcount = 0;
        //发射倒数
        int firecount = 0;
        int firelife = 0;
        public override void AI() {
            count++;
            firecount++;
            // 应用重力
            if (NPC.velocity.Y < 10f) // 限制最大下落速度
            {
                NPC.velocity.Y += 0.35f;
            }

            //// 检测 NPC 下方一格的 Tile
            int tileX = (int)(NPC.Center.X / 16);
            int tileY = (int)((NPC.position.Y + NPC.height) / 16);

            // 检查 Tile 是否为固体
            if (Main.tile[tileX, tileY] != null && Main.tile[tileX, tileY].HasTile && Main.tileSolid[Main.tile[tileX, tileY].TileType]) {
                if (NPC.velocity.Y != 0) {
                    if (NPC.velocity.X > 0 || NPC.velocity.X < 0) {
                        NPC.velocity.X *= 0.9f;
                        if (NPC.velocity.X > -1 && NPC.velocity.X < 1) {
                            NPC.velocity.X = 0;
                        }
                    }
                }
            }

            if (firecount == 2) firelife = NPC.lifeMax;

            if (firecount > 180) {
                // 检测生命值变化
                if (NPC.life < firelife) {
                    // NPC 受到伤害
                    OnHurt();
                }
            }

            // 更新上一帧的生命值
            firelife = NPC.life;

            // 更新未受伤害计时器
            if (NPC.life == NPC.lifeMax) {
                noDamageTimer++;
            } else {
                noDamageTimer = 0;
            }

            // 如果超过 2 秒未受伤害，尝试跳到目标头上
            if (noDamageTimer > 120) // 60 帧 = 1 秒
            {
                JumpToTarget();
                noDamageTimer = 0; // 重置计时器
            }

            // 处理反弹
            HandleBounce();

            // 更新 NPC 位置
            //NPC.position += NPC.velocity;

            // 简单朝向目标
            NPC target1 = NPC.Center.InPosClosestNPC(600, true, true);
            if (target1 != null && target1.active) {
                target0 = target1;
                if (NPC.Center.X < target1.Center.X) {
                    NPC.direction = 1;
                } else {
                    NPC.direction = -1;
                }
            } else {
                target0 = null;
            }

            base.AI();
        }

        //处理受伤
        void OnHurt() {
            // 如果 firecount 大于 180，执行 A() 方法
            if (firecount > 180) {
                Attack();
                firecount = 0;
            }
        }

        private void HandleBounce() {
            // 速度阈值
            float speedThreshold = 2f;

            // 检测 NPC 下方一格的 Tile
            int tileX = (int)(NPC.Center.X / 16);
            int tileY = (int)((NPC.position.Y + NPC.height) / 16);

            // 检查 Tile 是否为固体
            if (Main.tile[tileX, tileY] != null && Main.tile[tileX, tileY].HasTile && Main.tileSolid[Main.tile[tileX, tileY].TileType]) {
                // 如果 NPC 即将接触 Tile，且速度大于阈值，则反弹
                if (NPC.velocity.Y > speedThreshold) // 向下移动
                {
                    NPC.velocity.Y *= -0.8f; // 反弹
                } else if (NPC.velocity.Y <= speedThreshold && NPC.velocity.Y >= -speedThreshold) {
                    // 如果速度过小，则停止移动
                    NPC.velocity.Y = 0f;
                    NPC.position.Y = tileY * 16 - NPC.height; // 修正位置
                }
            }

            // 检测 NPC 左右两侧的 Tile
            tileX = (int)((NPC.position.X + (NPC.velocity.X > 0 ? NPC.width : 0)) / 16);
            tileY = (int)(NPC.Center.Y / 16);

            if (Main.tile[tileX, tileY] != null && Main.tile[tileX, tileY].HasTile && Main.tileSolid[Main.tile[tileX, tileY].TileType]) {
                // 如果 NPC 即将接触 Tile，且速度大于阈值，则反弹
                if (Math.Abs(NPC.velocity.X) > speedThreshold) // 水平移动
                {
                    NPC.velocity.X *= -0.8f; // 反弹
                    NPC.position.X = tileX * 16 - (NPC.velocity.X > 0 ? NPC.width : 0); // 修正位置
                } else {
                    // 如果速度过小，则停止移动
                    NPC.velocity.X = 0f;
                    NPC.position.X = tileX * 16 - (NPC.velocity.X > 0 ? NPC.width : 0); // 修正位置
                }
            }
        }

        // 跳到目标头上
        private void JumpToTarget() {
            if (target0 == null || !target0.active) {
                return;
            }

            // 计算目标位置
            Vector2 targetPosition = target0.Center - new Vector2(0, target0.height);

            // 计算初速度
            Vector2 velocity = CalculateJumpVelocity(NPC.Center, targetPosition);

            // 设置 NPC 速度
            NPC.velocity = velocity;
        }

        // 计算跳跃初速度
        private Vector2 CalculateJumpVelocity(Vector2 start, Vector2 end) {
            // 重力加速度
            float gravity = 0.35f;

            // 计算水平距离和垂直距离
            float distanceX = end.X - start.X;
            float distanceY = end.Y - start.Y;

            // 假设飞行时间为固定值（可以根据需要调整）
            float time = 60f; // 60 帧（1 秒）

            // 计算水平速度和垂直速度
            float velocityX = distanceX / time;
            float velocityY = (distanceY - 0.5f * gravity * time * time) / time;

            return new Vector2(velocityX, velocityY);
        }

        void Attack() {
            Vector2 vel = new Vector2(0, -8);
            Projectile proj0 = Projectile.NewProjectileDirect(new EntitySource_Parent(NPC), NPC.Center, new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), ModContent.ProjectileType<FlamingJackProj>(), 100, 1, 0, 0);
            proj0.velocity = new Vector2(0, -8).RotatedBy(0.5);

            spideremain -= 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit) {
            base.OnHitNPC(target, hit);
        }

        public override void HitEffect(NPC.HitInfo hit) {

            base.HitEffect(hit);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
            base.OnHitByProjectile(projectile, hit, damageDone);
        }

        public override bool CheckDead() {//濒死判定,
            for (int i = 0; i < 3; i++) {
                Attack();
            }
            return true;//空血处理
        }

        public override void OnKill()//在确认被杀后执行的一系列操作，例如推进世界进度，开启某些条件等,死亡效果不是用这个完成的
        {
            //击杀记录
            //GOGDownedBossSystem.downedNightWatcher = true;
            base.OnKill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + Name).Value;

            return true;
        }

        //Todo要写联机同步哟
    }
}
