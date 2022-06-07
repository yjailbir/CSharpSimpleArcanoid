using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arcanoid
{
    public partial class Form1 : Form
    {
        // делаем зазор в 2 пикселя между кирпичами, чтобы отделить их друг от друга
        const int brickGap = 2;
        // размеры каждого кирпича
        const int brickWidth = 25;
        const int brickHeight = 12;
        //Размеры стен
        const int wallSize = 12;
        //Шарик и платформа
        Ball ball = new Ball();
        Paddle paddle = new Paddle();

        // каждый ряд состоит из 14 кирпичей.На уровне будут 6 пустых рядов, а затем 8 рядов с кирпичами
        // цвета кирпичей: красный, оранжевый, зелёный и жёлтый
        // буква в массиве означает цвет кирпича (N - значит отсутствие кирпича)

        char[,] level =
            {
                {'N','N','N','N','N','N','N','N','N','N','N','N','N','N' },
                {'N','N','N','N','N','N','N','N','N','N','N','N','N','N' },
                {'N','N','N','N','N','N','N','N','N','N','N','N','N','N' },
                {'N','N','N','N','N','N','N','N','N','N','N','N','N','N' },
                {'N','N','N','N','N','N','N','N','N','N','N','N','N','N' },
                {'N','N','N','N','N','N','N','N','N','N','N','N','N','N' },
                {'R','R','R','R','R','R','R','R','R','R','R','R','R','R' },
                {'R','R','R','R','R','R','R','R','R','R','R','R','R','R' },
                {'O','O','O','O','O','O','O','O','O','O','O','O','O','O' },
                {'O','O','O','O','O','O','O','O','O','O','O','O','O','O' },
                {'G','G','G','G','G','G','G','G','G','G','G','G','G','G' },
                {'G','G','G','G','G','G','G','G','G','G','G','G','G','G' },
                {'Y','Y','Y','Y','Y','Y','Y','Y','Y','Y','Y','Y','Y','Y' },
                {'Y','Y','Y','Y','Y','Y','Y','Y','Y','Y','Y','Y','Y','Y' }
            };

        // сопоставляем буквы (R, O, G, Y) с цветами кирпичей
        Dictionary<char, Color> colors = new Dictionary<char, Color>()
            {
                {'R', Color.Red },
                {'O', Color.Orange },
                {'G', Color.Green },
                {'Y', Color.Yellow },
                {'N', Color.Transparent }
            };
        // основной массив для игры
        List<Brick> gameList = new List<Brick>();
        public class gameElement
        {
            private protected int x;
            private protected int y;
            private protected int width;
            private protected int height;
            private protected Color color;

            public int X
            {
                set { x = value; }
                get { return x; }
            }

            public int Y
            {
                set { y = value; }
                get { return y; }
            }
            public int WIDTH
            {
                set { width = value; }
                get { return width; }
            }

            public int HEIGHT
            {
                set { height = value; }
                get { return height; }
            }
            public Color COLOR
            {
                set { color = value; }
                get { return color; }
            }

        }
        //Платформа
        class Paddle : gameElement
        {

            private int dx;

            public Paddle()
            {
                x = 200;
                y = 400;
                dx = 0;
                width = brickWidth;
                height = brickHeight;
                color = Color.BlueViolet;
            }


            public int DX
            {
                set { dx = value; }
                get { return dx; }
            }
        }
        //Шарик
        class Ball : gameElement
        {
            private int dx;
            private int dy;
            private int speed;

            public Ball()
            {
                x = 130;
                y = 260;
                dx = 0;
                dy = 0;
                width = 5;
                height = 5;
                speed = 10;
                color = Color.Aqua;
            }

            public int DX
            {
                set { dx = value; }
                get { return dx; }
            }

            public int DY
            {
                set { dy = value; }
                get { return dy; }
            }

            public int SPEED
            {
                set { speed = value; }
                get { return speed; }
            }
        }
        //Кирпичик
        class Brick : gameElement
        {
            public Brick(int x, int y, int width, int height, Color color)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.color = color;
            }


        }
        public bool isCollides(gameElement obj1, gameElement obj2)
        {
            return obj1.X < obj2.X + obj2.WIDTH &&
          obj1.X + obj1.WIDTH > obj2.X &&
          obj1.Y < obj2.Y + obj2.HEIGHT &&
          obj1.Y + obj1.HEIGHT > obj2.Y && obj1.COLOR != Color.Transparent && obj2.COLOR != Color.Transparent;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    char colorChar = level[i, j];
                    if (colorChar != 'N')
                        gameList.Add(new Brick(wallSize + (brickWidth + brickGap) * j, wallSize + (brickHeight + brickGap) * i, brickWidth, brickHeight, colors[colorChar]));
                }
            }
            timer1.Start();
        }
        public void game()
        {
            // двигаем платформу с нужной скоростью 
            paddle.X += paddle.DX;

            // при этом смотрим, чтобы она не уехала за стены
            if (paddle.X < wallSize)
                paddle.X = wallSize;
            else if (paddle.X  > 360)
                paddle.X = 360;

            // шарик тоже двигается со своей скоростью
            ball.X += ball.DX;
            ball.Y += ball.DY;

            // и его тоже нужно постоянно проверять, чтобы он не улетел за границы стен
            // смотрим левую и правую стенки
            if (ball.X < wallSize)
            {
                ball.X = wallSize;
                ball.DX *= -1;
            }
            else if (ball.X + ball.WIDTH > 400 - wallSize)
            {
                ball.X = 400 - wallSize - ball.WIDTH;
                ball.DX *= -1;
            }
            // проверяем верхнюю границу
            if (ball.Y < wallSize)
            {
                ball.Y = wallSize;
                ball.DY *= -1;
            }

            // перезагружаем шарик, если он улетел вниз, за край игрового поля
            if (ball.Y > 600)
            {
                ball.X = 130;
                ball.Y = 260;
                ball.DX = 0;
                ball.DY = 0;
            }

            // проверяем, коснулся ли шарик платформы, которой управляет игрок. Если коснулся — меняем направление движения шарика по оси Y на противоположное
            if (isCollides(ball, paddle))
            {
                ball.DY *= -1;

                // сдвигаем шарик выше платформы, чтобы на следующем кадре это снова не засчиталось за столкновение
                ball.Y = paddle.Y - ball.HEIGHT;
            }
            Invalidate();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    paddle.DX = -10;
                    Invalidate();
                    break;
                case Keys.Right:
                    paddle.DX = 10;
                    Invalidate();
                    break;
                case Keys.Space:
                    if (ball.DX == 0 && ball.DY == 0)
                    {
                        ball.DX = ball.SPEED;
                        ball.DY = ball.SPEED;
                    }
                    Invalidate();
                    break;
                case Keys.Escape:
                    Environment.Exit(0);
                    break;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
            {
                paddle.DX = 0;
                Invalidate();
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            game();
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = CreateGraphics();


            g.Clear(Color.Black);

            // проверяем, коснулся ли шарик цветного кирпича 
            // если коснулся — меняем направление движения шарика в зависимости от стенки касания
            // для этого в цикле проверяем каждый кирпич на касание
            for (int i = 0; i < gameList.Count; i++)
            {
                // берём очередной кирпич
                Brick brick = gameList[i];

                // если было касание
                if (isCollides(ball, brick))
                {
                    // убираем кирпич из массива
                    gameList.RemoveAt(i);

                    // если шарик коснулся кирпича сверху или снизу — меняем направление движения шарика по оси Y
                    if ((ball.Y + ball.HEIGHT - ball.SPEED <= brick.Y ||
                        ball.Y >= brick.Y + brick.HEIGHT - ball.SPEED) && brick.COLOR != Color.Transparent)
                    {
                        ball.DY *= -1;
                    }
                    // в противном случае меняем направление движения шарика по оси X
                    else
                    {
                        ball.DX *= -1;
                    }
                    // как нашли касание — сразу выходим из цикла проверки
                    break;
                }
            }
            //g.FillRectangle(new SolidBrush(Color.Aqua), 0, 200, 412, 600);
            g.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, 400, wallSize);
            g.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, wallSize, 600);
            g.FillRectangle(new SolidBrush(Color.LightGray), 388, 0, wallSize, 600);

            g.FillRectangle(new SolidBrush(Color.Aqua), ball.X, ball.Y, ball.WIDTH, ball.HEIGHT);
            // рисуем платформу
            g.FillRectangle(new SolidBrush(Color.BlueViolet), paddle.X, paddle.Y, paddle.WIDTH, paddle.HEIGHT);

            // рисуем кирпичи
            foreach (Brick b in gameList)
                g.FillRectangle(new SolidBrush(b.COLOR), b.X, b.Y, b.WIDTH, b.HEIGHT);
        }

        //Это просто очень важная штука, которая уменьшает мерцание. К сожалению, не до конца(
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }
    }
}
