using System;
using System.Collections.Generic;
using GloryofGuardian.Content.Class;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace GloryofGuardian.Common
{
    //这是一个方法库,用于存放很多常用的方法
    public static class GOGUtils
    {
        #region System

        #endregion

        #region 基础
        /// <summary>
        /// 基础的跳字方法
        /// </summary>
        public static void WL(this object abc) {
            if(abc != null) Main.NewText(abc);
            else Main.NewText("错误");
        }

        /// <summary>
        /// 基础的粒子标点
        /// </summary>
        public static void Point(this Vector2 pos, int dustid = DustID.Wraith) { 
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(pos, 0, 0, dustid, 0f, 0f, 10, Color.White, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 0f;
            }
        }

        /// <summary>
        /// 更顺应方向感的两点之间向量
        /// </summary>
        /// <param name="vr1"></param>
        /// <param name="vr2"></param>
        /// <returns></returns>
        public static Vector2 To(this Vector2 vr1, Vector2 vr2) {
            return vr2 - vr1;
        }

        /// <summary>
        /// 两点之间的单位向量
        /// </summary>
        /// <param name="vr1"></param>
        /// <param name="vr2"></param>
        /// <returns></returns>
        public static Vector2 Toz(this Vector2 vr1, Vector2 vr2) {
            return (vr2 - vr1).SafeNormalize(Vector2.Zero);
        }

        /// <summary>
        /// 从任意个字符串中,随机选取一个
        /// params能够实现接受可变数量的参数
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string RandString(params string[] strings) {
            if (strings == null || strings.Length == 0) {
                throw new ArgumentException("至少需要提供一个字符串");
            }

            Random random = new Random();
            int randomIndex = random.Next(strings.Length);
            return strings[randomIndex];
        }

        public static string Full() {
            return RandString("我吃饱了", "多谢款待", "嗝~");
        }

        #endregion

        #region 绘制

        /// <summary>
        /// Shader结构体
        /// </summary>
        public static class VertexInfo2Helper {
            // 预定义的 VertexDeclaration（静态共享）
            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[3]
            {
        new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
        new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
        new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
            });

            // 静态工厂方法，快速创建 VertexInfo2
            public static VertexInfo2 Create(Vector2 position, Vector3 texCoord, Color color) {
                return new VertexInfo2(position, texCoord, color);
            }
        }

        // 原始 VertexInfo2 结构体（保持不变）
        public struct VertexInfo2 : IVertexType {
            public Vector2 Position;
            public Color Color;
            public Vector3 TexCoord;

            public VertexInfo2(Vector2 position, Vector3 texCoord, Color color) {
                Position = position;
                TexCoord = texCoord;
                Color = color;
            }

            public VertexDeclaration VertexDeclaration => VertexInfo2Helper.VertexDeclaration;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 检测玩家是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool Alives(this Player player) {
            if (player == null) return false;
            return player.active && !player.dead;
        }

        #endregion

        #region 判定
        /// <summary>
        /// 检查指定玩家是否按下了鼠标键,true左键false右键
        /// </summary>
        /// <param name="player">要检查的玩家</param>
        /// <param name="leftCed">是否检查左鼠标键，否则检测右鼠标键</param>
        /// <param name="netCed">是否进行网络同步检查</param>
        /// <returns>如果按下了指定的鼠标键，则返回true，否则返回false</returns>

        /// <summary>
        /// 判定弹幕是否能被物块阻挡
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="ignorePlatforms"></param>
        /// <param name="stickToEverything"></param>
        public static void StickToTiles(this Projectile projectile, bool ignorePlatforms, bool stickToEverything) {
            try {
                int xLeft = (int)(projectile.position.X / 16f) - 1;
                int xRight = (int)((projectile.position.X + projectile.width) / 16f) + 2;
                int yBottom = (int)(projectile.position.Y / 16f) - 1;
                int yTop = (int)((projectile.position.Y + projectile.height) / 16f) + 2;
                if (xLeft < 0) {
                    xLeft = 0;
                }
                if (xRight > Main.maxTilesX) {
                    xRight = Main.maxTilesX;
                }
                if (yBottom < 0) {
                    yBottom = 0;
                }
                if (yTop > Main.maxTilesY) {
                    yTop = Main.maxTilesY;
                }
                for (int x = xLeft; x < xRight; x++) {
                    for (int y = yBottom; y < yTop; y++) {
                        Tile tile = Main.tile[x, y];
                        bool platformCheck = true;
                        if (ignorePlatforms)
                            platformCheck = !TileID.Sets.Platforms[tile.TileType] && tile.TileType != TileID.PlanterBox;
                        bool tableCheck = false;
                        if (stickToEverything)
                            tableCheck = Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0;
                        if (tile != null && tile.HasUnactuatedTile && platformCheck && (Main.tileSolid[tile.TileType] || tableCheck)) {
                            Vector2 tileSize;
                            tileSize.X = x * 16;
                            tileSize.Y = y * 16;
                            if (projectile.position.X + projectile.width - 4f > tileSize.X && projectile.position.X + 4f < tileSize.X + 16f && projectile.position.Y + projectile.height - 4f > tileSize.Y && projectile.position.Y + 4f < tileSize.Y + 16f) {
                                projectile.velocity.X = 0f;
                                projectile.velocity.Y = -0.2f;
                            }
                        }
                    }
                }
            } catch {
            }
        }

        /// <summary>
        /// 辅助方法：世界实体坐标转物块坐标
        /// </summary>
        /// <param name="wePos"></param>
        /// <returns></returns>
        public static Vector2 WEPosToTilePos(Vector2 wePos) {
            int tilePosX = (int)(wePos.X / 16f);
            int tilePosY = (int)(wePos.Y / 16f);
            Vector2 tilePos = new Vector2(tilePosX, tilePosY);
            tilePos = TileHelper.PTransgressionTile(tilePos);
            return tilePos;
        }
        #endregion

        #region 索敌
        /// <summary>
        /// 寻找指定距离范围内位置最近的NPC
        /// </summary>
        /// <param name="origin">开始搜索的位置</param>
        /// <param name="minDistanceToCheck">搜索NPC的最小距离</param>
        /// <param name="maxDistanceToCheck">搜索NPC的最大距离</param>
        /// <param name="ignoreTiles">在检查障碍物时是否忽略物块</param>
        /// <param name="bossPriority">是否优先选择Boss</param>
        /// <param name="ignore">是否忽略给出目标</param>
        /// <returns>距离最近的NPC。</returns>
        public static NPC InPosClosestNPC(this Vector2 origin, float maxDistanceToCheck, float minDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false, List<int> ignore = null) {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority) {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++) {
                    if ((bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye) || !Main.npc[index2].CanBeChasedBy()) {
                        continue;
                    }
                    if (ignore != null && ignore.Contains(Main.npc[index2].whoAmI)) {//忽略组
                        continue;
                    }
                    if (minDistanceToCheck != 0 && Vector2.Distance(origin, Main.npc[index2].Center) < minDistanceToCheck) {//近距离忽略
                        continue;
                    }
                    float extraDistance2 = (Main.npc[index2].width / 2) + (Main.npc[index2].height / 2);
                    bool canHit2 = true;
                    if (extraDistance2 < distance && !ignoreTiles) {
                        canHit2 = Collision.CanHit(origin, 1, 1, Main.npc[index2].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[index2].Center) < distance + extraDistance2 && canHit2) {
                        if (Main.npc[index2].boss || Main.npc[index2].type == NPCID.WallofFleshEye) {
                            bossFound = true;
                        }
                        distance = Vector2.Distance(origin, Main.npc[index2].Center);
                        closestTarget = Main.npc[index2];
                    }
                }
            }
            else {
                for (int index = 0; index < Main.npc.Length; index++) {
                    if (Main.npc[index].CanBeChasedBy()) {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);
                        bool canHit = true;
                        if (ignore != null && ignore.Contains(Main.npc[index].whoAmI)) {//忽略组
                            continue;
                        }
                        if (minDistanceToCheck != 0 && Vector2.Distance(origin, Main.npc[index].Center) < minDistanceToCheck) {//近距离忽略
                            continue;
                        }
                        if (extraDistance < distance && !ignoreTiles) {
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);
                        }
                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance + extraDistance && canHit) {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// 寻找距离指定位置，在指定左右方向上最近的NPC
        /// </summary>
        /// <param name="origin">开始搜索的位置</param>
        /// <param name="maxDistanceToCheck">搜索NPC的最大距离</param>
        /// <param name="ignoreTiles">在检查障碍物时是否忽略物块</param>
        /// <param name="bossPriority">是否优先选择Boss</param>
        /// <param name="direction">从哪个方向搜索,-1为右,1为左</param>
        /// <param name="ignore">是否忽略给出目标</param>//弹射用
        /// <returns>距离最近的NPC。</returns>
        public static NPC InDirClosestNPC(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false, int direction = 0, List<int> ignore = null) {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority) {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++) {
                    if ((bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye) || !Main.npc[index2].CanBeChasedBy()) {
                        continue;
                    }
                    int dir = (Main.npc[index2].Center.X - origin.X) > 0 ? 1 : -1;
                    if (dir * direction == 1) {
                        continue;
                    }
                    if (ignore != null && ignore.Contains(Main.npc[index2].whoAmI)) {
                        continue;
                    }
                    float extraDistance2 = (Main.npc[index2].width / 2) + (Main.npc[index2].height / 2);
                    bool canHit2 = true;
                    if (extraDistance2 < distance && !ignoreTiles) {
                        canHit2 = Collision.CanHit(origin, 1, 1, Main.npc[index2].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[index2].Center) < distance + extraDistance2 && canHit2) {
                        if (Main.npc[index2].boss || Main.npc[index2].type == NPCID.WallofFleshEye) {
                            bossFound = true;
                        }
                        distance = Vector2.Distance(origin, Main.npc[index2].Center);
                        closestTarget = Main.npc[index2];
                    }
                }
            } else {
                for (int index = 0; index < Main.npc.Length; index++) {
                    if (Main.npc[index].CanBeChasedBy()) {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);
                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles) {
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);
                        }
                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance + extraDistance && canHit) {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// 寻找距离指定位置，在指定上下方向上最近的NPC
        /// </summary>
        /// <param name="origin">开始搜索的位置</param>
        /// <param name="maxDistanceToCheck">搜索NPC的最大距离</param>
        /// <param name="ignoreTiles">在检查障碍物时是否忽略物块</param>
        /// <param name="bossPriority">是否优先选择Boss</param>
        /// <param name="direction">从哪个方向搜索,1为右,2为左</param>
        /// <param name="ignore">是否忽略给出目标</param>//弹射用
        /// <returns>距离最近的NPC。</returns>
        public static NPC InDir2ClosestNPC(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false, int direction = 0, List<int> ignore = null) {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority) {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++) {
                    if ((bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye) || !Main.npc[index2].CanBeChasedBy()) {
                        continue;
                    }
                    int dir = (Main.npc[index2].Center.Y - origin.Y) > 0 ? 1 : -1;
                    if (dir * direction == 1) {
                        continue;
                    }
                    if (ignore != null && ignore.Contains(Main.npc[index2].whoAmI)) {
                        continue;
                    }
                    float extraDistance2 = (Main.npc[index2].width / 2) + (Main.npc[index2].height / 2);
                    bool canHit2 = true;
                    if (extraDistance2 < distance && !ignoreTiles) {
                        canHit2 = Collision.CanHit(origin, 1, 1, Main.npc[index2].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[index2].Center) < distance + extraDistance2 && canHit2) {
                        if (Main.npc[index2].boss || Main.npc[index2].type == NPCID.WallofFleshEye) {
                            bossFound = true;
                        }
                        distance = Vector2.Distance(origin, Main.npc[index2].Center);
                        closestTarget = Main.npc[index2];
                    }
                }
            } else {
                for (int index = 0; index < Main.npc.Length; index++) {
                    if (Main.npc[index].CanBeChasedBy()) {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);
                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles) {
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);
                        }
                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance + extraDistance && canHit) {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// 进行圆形的碰撞检测
        /// </summary>
        /// <param name="centerPosition">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="targetHitbox">碰撞对象的箱体结构</param>
        /// <returns></returns>
        public static bool CircularHitboxCollision(Vector2 centerPosition, float radius, Rectangle targetHitbox) {
            if (new Rectangle((int)centerPosition.X, (int)centerPosition.Y, 1, 1).Intersects(targetHitbox)) {
                return true;
            }

            float distanceToTopLeft = Vector2.Distance(centerPosition, targetHitbox.TopLeft());
            float distanceToTopRight = Vector2.Distance(centerPosition, targetHitbox.TopRight());
            float distanceToBottomLeft = Vector2.Distance(centerPosition, targetHitbox.BottomLeft());
            float distanceToBottomRight = Vector2.Distance(centerPosition, targetHitbox.BottomRight());
            float closestDistance = distanceToTopLeft;

            if (distanceToTopRight < closestDistance) {
                closestDistance = distanceToTopRight;
            }

            if (distanceToBottomLeft < closestDistance) {
                closestDistance = distanceToBottomLeft;
            }

            if (distanceToBottomRight < closestDistance) {
                closestDistance = distanceToBottomRight;
            }

            return closestDistance <= radius;
        }

        #endregion

        #region 行为

        /// <summary>
        /// 普通的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="Speed">速度</param>
        /// <param name="ShutdownDistance">停摆距离</param>
        /// <returns></returns>
        public static Vector2 ChasingBehavior(this Entity entity, Vector2 TargetCenter, float Speed, float ShutdownDistance = 16) {
            if (entity == null) {
                return Vector2.Zero;
            }

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);
            Vector2 speed = ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
            entity.velocity = speed;
            return speed;
        }

        /// <summary>
        /// 更加缓和的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="SpeedUpdates">速度的更新系数</param>
        /// <param name="HomingStrenght">追击力度</param>
        /// <returns></returns>
        public static Vector2 SmoothHomingBehavior(this Entity entity, Vector2 TargetCenter, float SpeedUpdates = 1, float HomingStrenght = 0.1f) {
            float targetAngle = entity.AngleTo(TargetCenter);
            float f = entity.velocity.ToRotation().RotTowards(targetAngle, HomingStrenght);
            Vector2 speed = f.ToRotationVector2() * entity.velocity.Length() * SpeedUpdates;
            entity.velocity = speed;
            return speed;
        }

        /// <summary>
        /// 计算一个渐进速度值
        /// </summary>
        /// <param name="thisCenter">本体位置</param>
        /// <param name="targetCenter">目标位置</param>
        /// <param name="speed">速度</param>
        /// <param name="shutdownDistance">停摆范围</param>
        /// <returns></returns>
        public static float AsymptoticVelocity(Vector2 thisCenter, Vector2 targetCenter, float speed, float shutdownDistance) {
            Vector2 toMou = targetCenter - thisCenter;
            float thisSpeed = toMou.LengthSquared() > shutdownDistance * shutdownDistance ? speed : MathHelper.Min(speed, toMou.Length());
            return thisSpeed;
        }

        /// <summary>
        /// 使当前角度转向目标角度,每次转动不超过一定值
        /// </summary>
        /// <param name="curAngle">现在角度</param>
        /// <param name="targetAngle">目标角度</param>
        /// <param name="maxChange">转动值</param>
        /// <returns></returns>
        public static float RotTowards(this float curAngle, float targetAngle, float maxChange) {
            curAngle = MathHelper.WrapAngle(curAngle);
            targetAngle = MathHelper.WrapAngle(targetAngle);
            if (curAngle < targetAngle) {
                if (targetAngle - curAngle > (float)Math.PI) {
                    curAngle += (float)Math.PI * 2f;
                }
            } else if (curAngle - targetAngle > (float)Math.PI) {
                curAngle -= (float)Math.PI * 2f;
            }

            curAngle += MathHelper.Clamp(targetAngle - curAngle, 0f - maxChange, maxChange);
            return MathHelper.WrapAngle(curAngle);
        }

        #endregion

        #region 新的前缀选取逻辑

        internal static int GetReworkedReforge(Item item, UnifiedRandom rand, int currentPrefix) {
            GloryofGuardianMod mod = GloryofGuardianMod.Instance;
            int GetCalPrefix(string name) {
                bool found = mod.TryFind(name, out ModPrefix ret);
                return found ? ret.Type : 0;
            }

            int prefix = -1;

            // 饰品
            if (item.accessory) {
                int accRerolls = 0;
                int[][] accessoryReforgeTiers = new int[][]
                {
                    new int[] { PrefixID.Hard, PrefixID.Jagged, PrefixID.Brisk, PrefixID.Wild }, //, GetCalPrefix("1") },
                    new int[] { PrefixID.Guarding, PrefixID.Spiked, PrefixID.Precise, PrefixID.Fleeting, PrefixID.Rash}, //, GetCalPrefix("2") },
                    new int[] { PrefixID.Armored, PrefixID.Angry, PrefixID.Hasty2, PrefixID.Intrepid, PrefixID.Arcane}, //, GetCalPrefix("3") },
                    new int[] { PrefixID.Warding, PrefixID.Menacing, PrefixID.Lucky, PrefixID.Quick2, PrefixID.Violent}, //, GetCalPrefix("4") },
                };

                // 尽量避免玩家连续重铸两次相同的前缀
                do {
                    int newPrefix = IteratePrefix(rand, accessoryReforgeTiers, currentPrefix);
                    if (newPrefix != currentPrefix) {
                        prefix = newPrefix;
                        break;
                    }
                    accRerolls++;
                } while (accRerolls < 20);
            }

            //对其它职业的修改暂不生效

            // 戍卫
            else if (item.CountsAsClass<GuardianDamageClass>()) {
                int[][] GuardianReforgeTiers = new int[][]
                {
                    new int[] { GetCalPrefix("Scrapped") , GetCalPrefix("Damaged") , GetCalPrefix("ShortCircuited") ,
                                    GetCalPrefix("Silent") , GetCalPrefix("Blooey") ,
                                    GetCalPrefix("Sensitive") , GetCalPrefix("Burdened") ,
                                    GetCalPrefix("Precise") , GetCalPrefix("Overclocked") , GetCalPrefix("Stainless") ,
                                    GetCalPrefix("Peerless") , GetCalPrefix("Excellent"), GetCalPrefix("Classic") },
                    };

                prefix = IteratePrefix(rand, GuardianReforgeTiers, currentPrefix);
            }

            return prefix;
        }

        private static int GetPrefixTier(int[][] tiers, int currentPrefix) {
            for (int checkingTier = 0; checkingTier < tiers.Length; ++checkingTier) {
                int[] tierList = tiers[checkingTier];
                for (int i = 0; i < tierList.Length; ++i)
                    if (tierList[i] == currentPrefix)
                        return checkingTier;
            }

            return -1;
        }

        private static int IteratePrefix(UnifiedRandom rand, int[][] reforgeTiers, int currentPrefix) {
            int currentTier = GetPrefixTier(reforgeTiers, currentPrefix);

            int newTier = currentTier == reforgeTiers.Length - 1 ? currentTier : currentTier + 1;
            return rand.Next(reforgeTiers[newTier]);
        }

        #endregion

        #region 前缀注册

        //在出现新的需要之前，这里只放置前缀
        public static int RandomGuardianPrefix() {
            Mod mod = ModContent.GetInstance<GloryofGuardianMod>();
            int GuardianPrefix = Utils.SelectRandom(Main.rand, new int[]
            {
                mod.Find<ModPrefix>("Peerless").Type,
                mod.Find<ModPrefix>("Excellent").Type,
                mod.Find<ModPrefix>("Classic").Type,

                mod.Find<ModPrefix>("Precise").Type,
                mod.Find<ModPrefix>("Overclocked").Type,
                mod.Find<ModPrefix>("Stainless").Type,

                mod.Find<ModPrefix>("Sensitive").Type,
                mod.Find<ModPrefix>("Burdened").Type,

                mod.Find<ModPrefix>("Silent").Type,
                mod.Find<ModPrefix>("Blooey").Type,

                mod.Find<ModPrefix>("Scrapped").Type,
                mod.Find<ModPrefix>("Damaged").Type,
                mod.Find<ModPrefix>("ShortCircuited").Type,

                //PrefixID.Annoying,//包含了神级和碎裂等一系列原版通用词缀
            });
            return GuardianPrefix;
        }

        public static bool NegativeGuardianPrefix(int prefix) {
            //Mod mod = ModContent.GetInstance<GloryofGuardianMod>();
            //List<int> badPrefixes = new List<int>()
            //{
            //    mod.Find<ModPrefix>("Bad!").Type,
            //};
            //return badPrefixes.Contains(prefix);
            return false;
        }

        #endregion
    }
}