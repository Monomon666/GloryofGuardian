using System;
using System.Linq;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using Terraria.ID;

namespace GloryofGuardian.Content.ParentClasses {
    public abstract class GOGLaserProj : ModProjectile {
        //这里是激光的基本模板,用于制作激光弹幕

        protected bool tilecollide = true;//记录初始的穿墙状态
        protected float LaserLength = 0;//长度
        protected float LaserWidth = 0;//宽度
        protected int drawcount = 0;//永久计时器
        protected int count = 0;//默认计时器

        protected int mode = 0;//将来用于控制样式,颜色等


        public override void SetStaticDefaults() {
            Main.projFrames[Type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
            base.SetStaticDefaults();
        }

        #region 不穿墙激光
        void SetLaserPosition() {//不穿墙的判断
            LaserLength = 20;
            Vector2 unit = Projectile.velocity.SafeNormalize(Vector2.Zero);
            while (LaserLength <= 1500)//长度还没超过1500时进行循环
            {
                Vector2 range = Projectile.Center + unit * LaserLength;//这是当前激光最远端
                if (!Collision.CanHit(Projectile.Center, 1, 1, range, 1, 1))//如果远端和起点隔着墙
                {
                    LaserLength -= 5;//距离-5
                    return;//跳出该函数
                }
                LaserLength += 2;//距离+2
            }
        }
        #endregion

        public override void SetDefaults() {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
            Projectile.aiStyle = -1;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;

            LaserLength = 4000;
            SetProperty();
            base.SetDefaults();
        }

        /// <summary>
        /// 用于设置额外的基础属性，在<see cref="SetDefaults"/>中被最后调用,能够覆盖靠前的设置
        /// </summary>
        public virtual void SetProperty() {
        }

        public override bool ShouldUpdatePosition() {
            return false;
        }

        public override void AI() {
            count++;

            ////撞墙检测
            //if (tilecollide) SetLaserPosition();
            //
            ////简单变粗-消失/变粗-变细激光
            //if (Projectile.ai[0] == 0) {
            //    //进行粗细变化的控制
            //    {
            //        if (Projectile.ai[1] == 0) {//变粗-消失
            //            if (LaserWidth < 25)
            //                LaserWidth += 0.1f;
            //        }
            //
            //        if (Projectile.ai[1] == 1) {//变粗-变细
            //            //if (LaserWidth < 25 && Projectile.timeLeft > 26)
            //            //    LaserWidth += 0.1f;
            //            //if (Projectile.timeLeft < 26) LaserWidth--;
            //        }
            //    }
            //}
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            int Length = (int)LaserLength;//定义激光长度
            //这个函数用于控制弹幕碰撞判断，符合你的碰撞条件时返回真即可
            float point = 0f;//这个照抄就行
            Vector2 startPoint = Projectile.Center;
            Vector2 endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Length;
            //结束点在弹幕速度方向上距离1500像素处的位置
            bool K =
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), //对方碰撞箱的位置
                targetHitbox.Size(),//对方碰撞箱的大小 
                startPoint,//线形碰撞箱起始点 
                endPoint,//结束点
                LaserWidth//线的宽度
                , ref point);
            if (K) return true;//如果满足这个碰撞判断，返回真，也就是进行碰撞伤害
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            int Length = (int)LaserLength;//定义激光长度
            //黑色背景的图片如果不对A值赋予0，或者启动Additive模式的话，画出来是黑色，效果很差
            //接下来是简单的延长绘制
            Color color1 = Color.White;//白色绘制就是图片原色
            color1.A = 0;//A赋予0使得图片颜色变为加算,可以去掉黑色部分
            //下面是激光头部的绘制
            Texture2D head = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RayHead").Value;
            Main.EntitySpriteDraw(head, Projectile.Center - Main.screenPosition, null,//不需要选框
            color1,//修改后的颜色
            Projectile.velocity.ToRotation(),//让图片朝向为弹幕速度方向
            new Vector2(0, head.Height / 2),//参考原点选择图片左边中点
            new Vector2(1, LaserWidth / 25f),//为使得激光更加自然，调整激光宽度
            SpriteEffects.None, 0);
            //下面是激光身体的绘制
            Texture2D tex = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Ray").Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition
                + Projectile.velocity.SafeNormalize(Vector2.Zero) * head.Width,//接在头部后面，所以加上头部长度的方向向量
                new Rectangle(0, 0, Length, tex.Height),//在高度不变的基础上，X轴延长到length
                color1,//修改后的颜色
                Projectile.velocity.ToRotation(),//让图片朝向为弹幕速度方向
                new Vector2(0, tex.Height / 2),//参考原点选择图片左边中点
                new Vector2(1, LaserWidth / 25f),//为使得激光更加自然，调整激光宽度
                SpriteEffects.None, 0);
            //下面是激光尾部的绘制
            Texture2D Tail = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RayTail").Value;
            Main.EntitySpriteDraw(Tail, Projectile.Center - Main.screenPosition
             + Projectile.velocity.SafeNormalize(Vector2.Zero) * (head.Width + Length),//接在身体末端的后面
            null,//不需要选框
            color1,//修改后的颜色
            Projectile.velocity.ToRotation(),//让图片朝向为弹幕速度方向
            new Vector2(0, Tail.Height / 2),//参考原点选择图片左边中点
           new Vector2(1, LaserWidth / 25f),//为使得激光更加自然，调整激光宽度    
            SpriteEffects.None, 0);
            return false;//return false阻止自动绘制
        }
    }
}
