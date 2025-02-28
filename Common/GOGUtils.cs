using System;
using System.Collections.Generic;
using Terraria.Chat;
using Terraria.GameContent.Prefixes;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;

namespace GloryofGuardian.Common
{
    //这是一个方法库,用于存放很多常用的方法
    public static class GOGUtils
    {
        #region System

        #endregion

        #region 绘制

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

        #endregion

        #region 判定
        /// <summary>
        /// 检查指定玩家是否按下了鼠标键,true左键false右键
        /// </summary>
        /// <param name="player">要检查的玩家</param>
        /// <param name="leftCed">是否检查左鼠标键，否则检测右鼠标键</param>
        /// <param name="netCed">是否进行网络同步检查</param>
        /// <returns>如果按下了指定的鼠标键，则返回true，否则返回false</returns>
        public static bool PressKey(this Player player, bool leftCed = true, bool netCed = true) {
            return (!netCed || Main.myPlayer == player.whoAmI) && (leftCed ? PlayerInput.Triggers.Current.MouseLeft : PlayerInput.Triggers.Current.MouseRight);
        }

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
        /// 寻找距离指定位置最近的NPC
        /// </summary>
        /// <param name="origin">开始搜索的位置</param>
        /// <param name="maxDistanceToCheck">搜索NPC的最大距离</param>
        /// <param name="ignoreTiles">在检查障碍物时是否忽略物块</param>
        /// <param name="bossPriority">是否优先选择Boss</param>
        /// <param name="ignore">是否忽略给出目标</param>//弹射用
        /// <returns>距离最近的NPC。</returns>
        public static NPC InPosClosestNPC(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false, List<int> ignore = null) {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority) {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++) {
                    if ((bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye) || !Main.npc[index2].CanBeChasedBy()) {
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

            // 默认选择
            int prefix = -1;

            // 饰品
            if (item.accessory) {
                int accRerolls = 0;
                int[][] accessoryReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { PrefixID.Hard, PrefixID.Jagged, PrefixID.Brisk, PrefixID.Wild }, //, GetCalPrefix("Quiet") },
            /* 1 */ new int[] { PrefixID.Guarding, PrefixID.Spiked, PrefixID.Precise, PrefixID.Fleeting, PrefixID.Rash}, //, GetCalPrefix("Cloaked") },
            /* 2 */ new int[] { PrefixID.Armored, PrefixID.Angry, PrefixID.Hasty2, PrefixID.Intrepid, PrefixID.Arcane}, //, GetCalPrefix("Camouflaged") },
            /* 3 */ new int[] { PrefixID.Warding, PrefixID.Menacing, PrefixID.Lucky, PrefixID.Quick2, PrefixID.Violent}, //, GetCalPrefix("Silent") },
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

            // 近战 包括工具和鞭子
            else if (item.CountsAsClass<MeleeDamageClass>() || item.CountsAsClass<SummonMeleeSpeedDamageClass>()) {
                if (PrefixLegacy.ItemSets.ItemsThatCanHaveLegendary2[item.type]) {
                    int[][] terrarianReforgeTiers = new int[][]
                    {
                /* 0 */ new int[] { PrefixID.Keen, PrefixID.Forceful, PrefixID.Strong },
                /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous },
                /* 2 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Godly },
                /* 3 */ new int[] { PrefixID.Legendary2 },
                    };
                    prefix = IteratePrefix(rand, terrarianReforgeTiers, currentPrefix);
                }
            
                // 剑，鞭，工具，支持传奇前缀的
                else if (PrefixLegacy.ItemSets.SwordsHammersAxesPicks[item.type] || (item.ModItem != null && item.ModItem.MeleePrefix())) {
                    int[][] meleeReforgeTiers = new int[][]
                    {
                /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Light, PrefixID.Heavy, PrefixID.Light, PrefixID.Forceful, PrefixID.Strong },
                /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky },
                /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp },
                /* 3 */ new int[] { PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior },
                /* 4 */ new int[] { PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly },
                /* 5 */ new int[] { PrefixID.Legendary } // for non-tools, Light is a mediocre low-tier reforge
                    };
                    int[][] toolReforgeTiers = new int[][]
                    {
                /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Heavy, PrefixID.Forceful, PrefixID.Strong },
                /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky },
                /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp },
                /* 3 */ new int[] { PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior },
                /* 4 */ new int[] { PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly },
                /* 5 */ new int[] { PrefixID.Legendary, PrefixID.Light } // for some tools, light is better than legendary. for others, it's equal
                    };
            
                    var tierListToUse = (item.pick > 0 || item.axe > 0 || item.hammer > 0) ? toolReforgeTiers : meleeReforgeTiers;
                    prefix = IteratePrefix(rand, tierListToUse, currentPrefix);
                }
            
                // 悠悠球，连枷，长矛等
                else {
                    int[][] meleeNoSpeedReforgeTiers = new int[][]
                    {
                /* 0 */ new int[] { PrefixID.Keen, PrefixID.Forceful, PrefixID.Strong },
                /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous },
                /* 2 */ new int[] { PrefixID.Superior, PrefixID.Demonic },
                /* 3 */ new int[] { PrefixID.Godly }
                    };
                    prefix = IteratePrefix(rand, meleeNoSpeedReforgeTiers, currentPrefix);
                }
            }
            
            // 远程
            else if (item.CountsAsClass<RangedDamageClass>()) {
                int[][] rangedReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Powerful, PrefixID.Forceful, PrefixID.Strong },
            /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Intimidating },
            /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Hasty, PrefixID.Staunch, PrefixID.Unpleasant },
            /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Sighted },
            /* 4 */ new int[] { PrefixID.Godly, PrefixID.Rapid, /* ranged Deadly */ PrefixID.Deadly, /* universal Deadly */ PrefixID.Deadly2 },
            /* 5 */ new int[] { PrefixID.Unreal }
                };
                prefix = IteratePrefix(rand, rangedReforgeTiers, currentPrefix);
            }
            
            // 魔法
            else if (item.CountsAsClass<MagicDamageClass>() || item.CountsAsClass<MagicSummonHybridDamageClass>()) {
                int[][] magicReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Furious, PrefixID.Forceful, PrefixID.Strong },
            /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic },
            /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Adept, PrefixID.Celestial, PrefixID.Unpleasant },
            /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic },
            /* 4 */ new int[] { PrefixID.Godly, PrefixID.Masterful, PrefixID.Deadly2 },
            /* 5 */ new int[] { PrefixID.Mythical }
                };
                prefix = IteratePrefix(rand, magicReforgeTiers, currentPrefix);
            }
            
            // 召唤(不含鞭子)
            else if (item.CountsAsClass<SummonDamageClass>()) {
                int[][] summonReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { PrefixID.Nimble, PrefixID.Furious },
            /* 1 */ new int[] { PrefixID.Forceful, PrefixID.Strong, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic },
            /* 2 */ new int[] { PrefixID.Hurtful, PrefixID.Adept, PrefixID.Celestial },
            /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic, PrefixID.Deadly2 },
            /* 4 */ new int[] { PrefixID.Masterful, PrefixID.Godly },
            /* 5 */ new int[] { PrefixID.Mythical, PrefixID.Ruthless }
                };
                prefix = IteratePrefix(rand, summonReforgeTiers, currentPrefix);
            }

            // 戍卫
            else if (item.CountsAsClass<GuardianDamageClass>()) {
                int[][] GuardianReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { GetCalPrefix("Scrapped") , GetCalPrefix("Damaged") , GetCalPrefix("ShortCircuited") ,
                                GetCalPrefix("Silent") , GetCalPrefix("Blooey") ,
                                GetCalPrefix("Sensitive") , GetCalPrefix("Burdened") ,
                                GetCalPrefix("Precise") , GetCalPrefix("Overclocked") , GetCalPrefix("Stainless") ,
                                GetCalPrefix("Peerless") , GetCalPrefix("Excellent"), GetCalPrefix("Classic") },
                                

            // /* 0 */ new int[] { GetCalPrefix("Scrapped") , GetCalPrefix("Damaged") , GetCalPrefix("ShortCircuited") },
            // /* 1 */ new int[] { GetCalPrefix("Silent") , GetCalPrefix("Blooey") },
            // /* 2 */ new int[] { GetCalPrefix("Sensitive") , GetCalPrefix("Burdened") },
            // /* 3 */ new int[] { GetCalPrefix("Precise") , GetCalPrefix("Overclocked") , GetCalPrefix("Stainless") },
            // /* 4 */ new int[] { GetCalPrefix("Peerless") , GetCalPrefix("Excellent"), GetCalPrefix("Classic") },
            // /* 5 */ //
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

            // 检测到无效前缀则从头开始循环
            return -1;
        }

        private static int IteratePrefix(UnifiedRandom rand, int[][] reforgeTiers, int currentPrefix) {
            int currentTier = GetPrefixTier(reforgeTiers, currentPrefix);

            // 稳步上升的重铸
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