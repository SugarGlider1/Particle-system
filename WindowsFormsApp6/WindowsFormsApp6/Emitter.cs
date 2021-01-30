using System;
using System.Collections.Generic;
using System.Drawing;

namespace WindowsFormsApp6
{
   public class Emitter
    {
        public int X; // координата X центра эмиттера, будем ее использовать вместо MousePositionX
        public int Y; // соответствующая координата Y 
        public int Direction = 0; // вектор направления в градусах куда сыпет эмиттер
        public int Spreading = 360; // разброс частиц относительно Direction
        public int SpeedMin = 1; // начальная минимальная скорость движения частицы
        public int SpeedMax = 10; // начальная максимальная скорость движения частицы
        public int RadiusMin = 2; // минимальный радиус частицы
        public int RadiusMax = 10; // максимальный радиус частицы
        public int LifeMin = 20; // минимальное время жизни частицы
        public int LifeMax = 100; // максимальное время жизни частицы


        public List<ColorPoint> colorPoints = new List<ColorPoint>();
        public List<ParticleColorful> particles = new List<ParticleColorful>();
        public float GravitationX = 0;
        public float GravitationY = 1;
        public int ParticlesCount = 1000;

        public void UpdateState()
        {

            foreach (var particle in particles)
            {
                particle.X += particle.SpeedX;
                particle.Y += particle.SpeedY;

                particle.Life -= 1; // уменьшаю здоровье
                                    // если здоровье кончилось
                if (particle.Life < 0)
                {
                    ResetParticle(particle);
                }
                else
                {
                    foreach (var point in colorPoints)
                    {
                        point.ColorParticle(particle);
                    }

                    
                    particle.SpeedX += GravitationX;
                    particle.SpeedY += GravitationY;

                }
            }

            for (var i = 0; i < 10; ++i)
            {
                if (particles.Count < ParticlesCount)
                {

                    var particle = new ParticleColorful();
                    particle.FromColor = Color.White;
                    particle.ToColor = Color.FromArgb(0, Color.Wheat);

                    ResetParticle(particle); 

                    particles.Add(particle);
                }
                else
                {
                    break;
                }
            }
        }
        public void Render(Graphics g)
        {
            foreach (var particle in particles)
            {
                particle.Draw(g);
            }

            foreach (var point in colorPoints)
            {
                point.Render(g);
            }
        }
        public virtual void ResetParticle(ParticleColorful particle)
        {
            particle.Life = Particle.rand.Next(LifeMin, LifeMax);

            particle.X = X;
            particle.Y = Y;
            particle.FromColor = Color.White;
            particle.ToColor = Color.FromArgb(0, Color.Wheat);
            var direction = Direction
                + (double)Particle.rand.Next(Spreading)
                - Spreading / 2;

            var speed = Particle.rand.Next(SpeedMin, SpeedMax);

            particle.SpeedX = (float)(Math.Cos(direction / 180 * Math.PI) * speed);
            particle.SpeedY = -(float)(Math.Sin(direction / 180 * Math.PI) * speed);

            particle.Radius = Particle.rand.Next(RadiusMin, RadiusMax);
        }
       
    }
    public class ColorPoint
    {
        public float X; // ну точка же, вот и две координаты
        public float Y;

        public int Radius; // Радиус нашей тчк
        public Color pColor; //Цвет нашей красящей тчк
        

        
        public void ColorParticle(ParticleColorful particle)
        {
            float gX = X - particle.X;
            float gY = Y - particle.Y;

            double r = Math.Sqrt(gX * gX + gY * gY); // считаем расстояние от центра точки до центра частицы
            if (r + particle.Radius < Radius / 2) // если частица оказалось внутри окружности
            {
                // то меняем ее диапазон рассеивания цветов
                particle.FromColor = pColor;
                particle.ToColor = Color.FromArgb(0, pColor);
            }
        }

        // базовый класс для отрисовки точечки
        public virtual void Render(Graphics g)
        {
            g.DrawEllipse(
                   new Pen(pColor),
                   X - Radius / 2,
                   Y - Radius / 2,
                   Radius,
                   Radius
               );
        }
    }

    public class TopEmitter : Emitter
    {
        public int Width; // длина экрана

        public override void ResetParticle(ParticleColorful particle)
        {
            base.ResetParticle(particle); // вызываем базовый сброс частицы, там жизнь переопределяется и все такое

            // а теперь тут уже подкручиваем параметры движения
            particle.X = Particle.rand.Next(Width); // позиция X -- произвольная точка от 0 до Width
            particle.Y = 0;  // ноль -- это верх экрана 

            particle.FromColor = Color.White;
            particle.ToColor = Color.FromArgb(0, Color.Wheat);
            particle.SpeedY = 1; // падаем вниз по умолчанию
            particle.SpeedX = Particle.rand.Next(-2, 2); // разброс влево и вправа у частиц 
        }
    }
}
