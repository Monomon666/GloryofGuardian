using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.ParentClasses {
    public abstract class GOGProj : ModProjectile {
        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = GuardianDamageClass.Instance;

            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 1;

            Projectile.aiStyle = -1;

            Projectile.extraUpdates = 0;
            Projectile.scale *= 1f;

            SetProperty();
        }

        /// <summary>
        /// 用于设置额外的基础属性，在<see cref="SetDefaults"/>中被最后调用,能够覆盖靠前的设置
        /// </summary>
        public virtual void SetProperty() {
            //Projectile.width = 
            //Projectile.height = 
            //Projectile.localNPCHitCooldown = 
            //Projectile.penetrate = 
        }

        Player Owner => Main.player[Projectile.owner];

        /// <summary>
        /// 默认计时器
        /// </summary>
        protected int count = 0;
        /// <summary>
        /// 默认状态机
        /// </summary>
        protected int mode = 0;
        /// <summary>
        /// 供全局读取的mode
        /// </summary>
        public int pmode = 0;

        /// <summary>
        /// 绘制计时器,不重置
        /// </summary>
        protected int drawcount = 0;
        /// <summary>
        /// 预先设定的第一索敌目标,不轻易变动
        /// </summary>
        protected NPC target0 = null;
        /// <summary>
        /// 预先设定的第一索敌目标,持续变动
        /// </summary>
        protected NPC target01 = null;
        /// <summary>
        /// 用于记录索敌目标的位置,实现延迟效果
        /// </summary>
        protected Vector2 oldtargetpos = Vector2.Zero;
        /// <summary>
        /// 索敌距离
        /// </summary>
        /// <returns></returns>
        protected int Attackrange = 1200;
        /// <summary>
        /// 启用穿墙追踪
        /// </summary>
        protected bool throughtile = false;
        /// <summary>
        /// 启用boss优先
        /// </summary>
        protected bool bossfirst = false;

        /// <summary>
        /// 记录弹幕的出现位置
        /// </summary>
        public Vector2 startpos = new Vector2(0, 0);
        public override bool PreAI() {
            count++;
            drawcount++;
            //第一帧
            if (drawcount == 1) {
                startpos = Projectile.Center;
            }
            //索敌与行动
            if (target0 == null || !target0.active) target0 = Projectile.Center.InPosClosestNPC(Attackrange, 0, throughtile, bossfirst);
            target01 = Projectile.Center.InPosClosestNPC(Attackrange, 0, throughtile, bossfirst);

            return base.PreAI();
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        /// <summary>
        /// 预包装好的叠加绘制画布转换
        /// </summary>
        public virtual void AdDraw(Action codeBlock) {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            codeBlock();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //使用:
            //AdDraw( () => 代码 )
        }
    }
}
