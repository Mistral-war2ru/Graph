using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Graphs
{
    public partial class Form1 : Form
    {
        public Graph Graf = new Graph();//Граф

        public int drag = -1;//перемещаемый узел
        public int drage = -1;//узел из которого происходит добавление/удаление рёбер

        public int dx1 = 0;//координаты двух точек
        public int dy1 = 0;//для отображения линии,
        public int dx2 = 0;//при добавлении
        public int dy2 = 0;//или удалении рёбер

        public bool act = false;//показывает, что обход графа запущен

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.Width = this.Width - 16;//выравнивание pictureBox под размеры формы
            pictureBox1.Height = this.Height - pictureBox1.Location.Y - 39;//выравнивание pictureBox под размеры формы
            Graf.x = pictureBox1.Width / 2;//задаёт координаты
            Graf.y = pictureBox1.Height / 2;//появления новых узлов

            Bitmap buffer = new Bitmap(Width, Height);//Дополнительный буффер
            Graphics gfx = Graphics.FromImage(buffer);//Двойная буфферизация

            SolidBrush myBrush = new SolidBrush(Color.Black);//кисть для текста
            SolidBrush myBrush2 = new SolidBrush(Color.White);//кисть для анимации
            Pen myPen = new Pen(Color.Black);//ручка для узлов
            Pen myPen2 = new Pen(Color.Green);//ручка для ребёр

            gfx.Clear(Color.White);//очистка поверхности
            myPen2.Color = Color.Green;//по умолчанию все рёбра
            myBrush2.Color = Color.Green;//зеленые
            foreach (Graph.Node n in Graf.nodes)//во всех узлах
            {
                foreach (int eg in n.edges)//проверить все рёбра
                {
                    foreach (Graph.Node m in Graf.nodes)//найти все узлы
                    {
                        if (m.id == eg)//находящие в списке смежности
                        {
                            double a = Match.point_direction(n.x, n.y, m.x, m.y);//направление между узлами
                            double dist = Match.point_distance(n.x, n.y, m.x, m.y);//расстояние между узлами
                            gfx.DrawLine(myPen2,
                                new Point(n.x + (int)Match.lengthdir_x(Graf.sz / 2, a), n.y + (int)Match.lengthdir_y(Graf.sz / 2, a)),
                                new Point(n.x + (int)Match.lengthdir_x(dist - (Graf.sz / 2), a), 
                                n.y + (int)Match.lengthdir_y(dist - (Graf.sz / 2), a)));//отрисовка ребра
                            gfx.FillEllipse(myBrush2,
                                new Rectangle(n.x + (int)Match.lengthdir_x(dist - (Graf.sz / 2), a) - 4,
                                n.y + (int)Match.lengthdir_y(dist - (Graf.sz / 2), a) - 4, 8, 8));//отрисовка конца ребра
                        }
                    }
                }
            }
            foreach (Graph.Node n in Graf.nodes)
            {
                myBrush2.Color = Color.White;//обычное состояние узла
                if (n.active == 1)
                    myBrush2.Color = Color.SteelBlue;//узел обрабатывается в данный момент
                if (n.active == 2)
                    myBrush2.Color = Color.Gray;//узел обработан
                if (n.active == 3)
                    myBrush2.Color = Color.Gold;//узел запомнен (не обрабатывается в данный момент и до конца обработан ещё не был) (DFS обход)
                gfx.FillEllipse(myBrush2, new Rectangle(n.x - Graf.sz / 2, n.y - Graf.sz / 2, Graf.sz, Graf.sz));//отрисовка фона
                gfx.DrawEllipse(myPen, new Rectangle(n.x - Graf.sz / 2, n.y - Graf.sz / 2, Graf.sz, Graf.sz));//отрисовка контура
                gfx.DrawString(n.name, new Font("Arial", 8, FontStyle.Regular), myBrush, new PointF(n.x - Graf.sz / 3, n.y - 10));//отрисовка названия
            }
            if (drage != -1)//если из какого-то узла происходит добавление/удаление ребра
            {
                myBrush2.Color = Color.Green;
                if (checkBox2.Checked)//красный цвет при удалении
                {
                    myPen2.Color = Color.Red;
                    myBrush2.Color = Color.Red;
                }
                double a1 = Match.point_direction(dx1, dy1, dx2, dy2);//направление от узла к указателю мыши
                double dist1 = Match.point_distance(dx1, dy1, dx2, dy2);//расстояние между узлом и указателем мыши
                gfx.DrawLine(myPen2,
                    new Point(dx1 + (int)Match.lengthdir_x(Graf.sz / 2, a1), dy1 + (int)Match.lengthdir_y(Graf.sz / 2, a1)),
                    new Point(dx1 + (int)Match.lengthdir_x(dist1, a1), dy1 + (int)Match.lengthdir_y(dist1, a1)));
                gfx.FillEllipse(myBrush2,
                    new Rectangle(dx1 + (int)Match.lengthdir_x(dist1, a1) - 4, dy1 + (int)Match.lengthdir_y(dist1, a1) - 4, 8, 8));
            }

            pictureBox1.Image = buffer;//Двойная буфферизация убирает мерцание
            myBrush.Dispose();
            myBrush2.Dispose();
            myPen.Dispose();
            myPen2.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!act)//нельзя добавлять узлы если обход графа уже идёт
                Graf.AddNode(textBox1.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer2.Enabled)
            {
                button3.Text = "Пауза";
            }
            else 
            {
                button3.Text = "Продолжить";
            }
            if (act)
            {
                button2.Text = "Стоп";
                button3.Enabled = true;
                trackBar1.Enabled = false;
                button1.Enabled = false;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                progressBar1.Visible = true;
                button5.Enabled = false;
            }
            else
            {
                button2.Text = "Обход графа";
                button3.Text = "";
                button3.Enabled = false;
                trackBar1.Enabled = true;
                button1.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                progressBar1.Visible = false;
                button5.Enabled = true;
            }
            if (Graf.nodes.Count == 0 || act)
            {
                button4.Enabled = false;
            }
            else
            {
                button4.Enabled = true;
            }

            Graf.sz = trackBar1.Value;//регулировка размера узлов для отрисовки

            for (int i = 0; i < Graf.nodes.Count; i++)
            {
                for (int j = 0; j < Graf.nodes.Count; j++)
                {
                    if (i != j)//узел не выталкивает сам себя
                    {
                        double dist = Match.point_distance(Graf.nodes[i].x, Graf.nodes[i].y, Graf.nodes[j].x, Graf.nodes[j].y);
                        int sz_in = 10;//дополнительное расстояние между узлами
                        if (dist <= (Graf.sz + sz_in))//если два разных узла оказались внутри друг друга
                        {
                            //рандомно вытолкнуть узлы если их координаты совпали
                            var rand = new Random();
                            if (Graf.nodes[i].x == Graf.nodes[j].x)
                            {
                                if (rand.Next(2) == 1)
                                    Graf.nodes[i].x += 1;
                                else
                                    Graf.nodes[i].x -= 1;
                            }
                            if (Graf.nodes[i].y == Graf.nodes[j].y)
                            {
                                if (rand.Next(2) == 1)
                                    Graf.nodes[i].y += 1;
                                else
                                    Graf.nodes[i].y -= 1;
                            }
                            //узлы выталкиваются в противоположных направлениях
                            if (Graf.nodes[i].x < Graf.nodes[j].x)
                            {
                                Graf.nodes[i].x -= (int)(Graf.sz + sz_in - dist);
                                Graf.nodes[j].x += (int)(Graf.sz + sz_in - dist);
                            }
                            else
                            {
                                Graf.nodes[i].x += (int)(Graf.sz + sz_in - dist);
                                Graf.nodes[j].x -= (int)(Graf.sz + sz_in - dist);
                            }
                            if (Graf.nodes[i].y < Graf.nodes[j].y)
                            {
                                Graf.nodes[i].y -= (int)(Graf.sz + sz_in - dist);
                                Graf.nodes[j].y += (int)(Graf.sz + sz_in - dist);
                            }
                            else
                            {
                                Graf.nodes[i].y += (int)(Graf.sz + sz_in - dist);
                                Graf.nodes[j].y -= (int)(Graf.sz + sz_in - dist);
                            }
                        }
                    }
                }
                if (Graf.nodes[i].x - Graf.sz / 2 < 0) Graf.nodes[i].x = Graf.sz / 2;
                if (Graf.nodes[i].y - Graf.sz / 2 < 0) Graf.nodes[i].y = Graf.sz / 2;
                if (Graf.nodes[i].x + Graf.sz / 2 > pictureBox1.Width) Graf.nodes[i].x = pictureBox1.Width - Graf.sz / 2 - 1;
                if (Graf.nodes[i].y + Graf.sz / 2 > pictureBox1.Height) Graf.nodes[i].y = pictureBox1.Height - Graf.sz / 2 - 1;
                //не позволяет узлу выйти за пределы экрана
            }

            Refresh();//перерисовка формы
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag != -1)//если перемещается какой-то узел
            {
                foreach (Graph.Node n in Graf.nodes)
                {
                    if (drag == n.id)//найти этот узел
                    {
                        n.x = e.X;//и переместить
                        n.y = e.Y;//по координатам указателя мыши
                        break;
                    }
                }
            }
            if (drage != -1)//если из какого-то узла происходит добавление/удаление рёбер
            {
                foreach (Graph.Node n in Graf.nodes)
                {
                    if (drage == n.id)//найти этот узел
                    {
                        dx1 = n.x;
                        dy1 = n.y;//запомнить его координаты
                        dx2 = e.X;
                        dy2 = e.Y;//и координаты указателя мыши
                        break;
                    }
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drage = -1;//отключить добавление/удаление рёбер
                if (drag == -1)//если никакой узел не перемещается
                {
                    foreach (Graph.Node n in Graf.nodes)
                    {
                        if (Match.point_distance(n.x, n.y, e.X, e.Y) < Graf.sz / 2)//найти узел на который нажали
                        {
                            drag = n.id;//захватить его
                            n.x = e.X;//и переместить
                            n.y = e.Y;//по координатам указателя мыши
                            break;
                        }
                    }
                }
            }
            if (!act)//нельзя удалять узлы и менять рёбра если обход графа уже запущен
            {
                if (e.Button == MouseButtons.Middle)
                {
                    drag = -1;//отключить перемещение
                    drage = -1;//отключить добавление/удаление рёбер
                    foreach (Graph.Node n in Graf.nodes)
                    {
                        if (Match.point_distance(n.x, n.y, e.X, e.Y) < Graf.sz / 2)//найти узел на который нажали
                        {
                            Graf.RemoveNode(n.id);//удалить его
                            break;
                        }
                    }
                }
                if (e.Button == MouseButtons.Right)
                {
                    drag = -1;//отключить перемещение
                    dx1 = 0;
                    dy1 = 0;
                    dx2 = 0;
                    dy2 = 0;//и сбросить координаты отрисовки
                    foreach (Graph.Node n in Graf.nodes)
                    {
                        if (Match.point_distance(n.x, n.y, e.X, e.Y) < Graf.sz / 2)//найти узел на который нажали
                        {
                            drage = n.id;//начать добавление/удаление рёбер из него
                            dx1 = n.x;
                            dy1 = n.y;//запомнить его координаты
                            dx2 = e.X;
                            dy2 = e.Y;//и координаты указателя мыши
                            break;
                        }
                    }
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                drag = -1;//отключить перемещение
            if (e.Button == MouseButtons.Right)
            {
                if (drage != -1)//если из какого то узла начато добавление/удаление рёбер
                {
                    foreach (Graph.Node n in Graf.nodes)
                    {
                        if (Match.point_distance(n.x, n.y, e.X, e.Y) < Graf.sz / 2)//найти узел на котором отпустили кнопку
                        {
                            if (n.id!=drage)//если это не тот же самый узел
                            {
                                foreach (Graph.Node m in Graf.nodes)
                                {
                                    if (m.id == drage)//найти узел из которого было запущено добавление/удаление рёбер
                                    {
                                        if (checkBox2.Checked)//удаление
                                        {
                                            m.RemoveEdge(n.id);
                                            if (!checkBox1.Checked)//если граф неориентированный
                                                n.RemoveEdge(m.id);//удалить в обоих узлах
                                        }
                                        else
                                        {
                                            m.AddEdge(n.id);
                                            if (!checkBox1.Checked)//если граф неориентированный
                                                n.AddEdge(m.id);//добавить и в обратную сторону тоже
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                drage = -1;//отключить добавление/удаление рёбер
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Interval = trackBar2.Value;//регулировка скорости обработки

            if (radioButton1.Checked)//обычный перебор узлов по списку
            {
                for (int i = 0; i < Graf.nodes.Count; i++)
                {
                    if (Graf.nodes[i].active == 1)//найти узел, который обрабатывается в данный момент
                    {
                        //** здесь можно провести необходимую обработку узла
                        //** do something
                        Graf.nodes[i].active = 2;//узел был обработан
                        if (i < Graf.nodes.Count - 1)//если это не конец списка
                            Graf.nodes[i + 1].active = 1;//активировать следующий узел
                        else
                        {
                            foreach (Graph.Node n in Graf.nodes)
                                n.active = 0;//вернуться в обычное состояние
                            timer2.Stop();//остановить таймер
                            act = false;//прекратить обход
                        }
                        break;
                    }
                }
            }
            if (radioButton2.Checked)
            {
                bool fa = false;//если в графе есть активные узлы
                for (int i = 0; i < Graf.nodes.Count; i++)
                {
                    if (Graf.nodes[i].active == 1)//найти активный узел
                    {
                        if (Graf.nodes[i].chk==-1)
                        {
                            //** здесь можно провести необходимую обработку узла
                            //** do something
                            //** (вызов при первом попадании в узел)
                        }
                        bool st = false;//есть ли необработанные узлы в списке смежности
                        while (!st && (Graf.nodes[i].chk < Graf.nodes[i].edges.Count - 1))//перебирать все узлы в списке смежности
                        {
                            Graf.nodes[i].chk++;//менять проверяемый элемент списка смежности
                            foreach (Graph.Node m in Graf.nodes)
                            {
                                if (m.id == Graf.nodes[i].edges[Graf.nodes[i].chk])//найти проверяемый узел
                                {
                                    if (m.active == 0)//если он не активный
                                    {
                                        m.active = 1;//активировать его
                                        m.prev = Graf.nodes[i].id;//указать себя в качестве родительского узла
                                        Graf.nodes[i].active = 3;//запомнить его
                                        st = true;//в списке смежности есть необработанные узлы
                                        break;
                                    }
                                }
                            }
                        }
                        if (!(Graf.nodes[i].chk < Graf.nodes[i].edges.Count - 1))//если список смежности закончился
                        {
                            bool noa = true;//в списке смежности нет активных узлов
                            foreach (Graph.Node m in Graf.nodes)
                            {
                                if (Graf.nodes[i].edges.Contains(m.id) && m.active == 1)//найти активный узел из списка смежности
                                    noa = false;//найден активный узел
                            }
                            if (noa)//если в списке смежности больше нет активных узлов
                            {
                                //** здесь можно провести необходимую обработку узла
                                //** do something
                                //** (вызов при выходе из узла, когда он был уже полностью обработан)
                                Graf.nodes[i].active = 2;//узел был обработан
                                if (Graf.nodes[i].prev != -1)//если есть родитель
                                {
                                    foreach (Graph.Node m in Graf.nodes)
                                    {
                                        if (m.id == Graf.nodes[i].prev)//найти его
                                            m.active = 1;//активировать предыдущий узел
                                    }
                                }
                            }
                        }
                        fa = true;//активный узел найден
                        break;
                    }
                }
                if (!fa)//если нет активных узлов
                {
                    fa = false;//в графе все ещё есть необработанные узлы
                    for (int i = 0; i < Graf.nodes.Count; i++)
                    {
                        if (Graf.nodes[i].active == 0)//попытаться найти все ещё необработанный узел
                                                      //на случай если в графе есть узлы не связанные с первым
                        {
                            Graf.nodes[i].active = 1;//активировать его
                            fa = true;//найден не обработанный узел
                            break;
                        }
                    }
                    if (!fa)//если абсолютно все узлы обработаны
                    {
                        foreach (Graph.Node n in Graf.nodes)
                            n.active = 0;//вернуться в обычное состояние
                        timer2.Stop();//остановить таймер
                        act = false;//прекратить обход
                    }
                }
            }
            if (radioButton3.Checked)//BFS обход
            {
                if (Graf.nodes_q.Count != 0)//если очередь не пустая
                {
                    int aid = Graf.nodes_q.Dequeue();//вытащить из очереди новый узел
                    for (int i = 0; i < Graf.nodes.Count; i++)
                    {
                        if (Graf.nodes[i].id == aid)//найти этот узел
                        {
                            //** здесь можно провести необходимую обработку узла
                            //** do something
                            Graf.nodes[i].active = 2;//узел был обработан
                            foreach (int eg in Graf.nodes[i].edges)
                            {
                                foreach (Graph.Node m in Graf.nodes)
                                {
                                    if (m.id == eg)
                                    {
                                        if (m.active == 0)
                                        {
                                            m.active = 1;//активировать
                                            Graf.nodes_q.Enqueue(m.id);//и добавить в очередь
                                            //все необработанные узлы из списка смежности
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                else
                {
                    //если очередь уже пустая
                    for (int i = 0; i < Graf.nodes.Count; i++)
                    {
                        if (Graf.nodes[i].active == 0)//попытаться найти все ещё необработанный узел
                                                      //на случай если в графе есть узлы не связанные с первым
                        {
                            Graf.nodes[i].active = 1;//активировать его
                            Graf.nodes_q.Enqueue(Graf.nodes[i].id);//и добавить в очередь
                            break;
                        }
                    }
                    if (Graf.nodes_q.Count == 0)//очередь все ещё пустая - абсолютно все узлы были обработаны
                    {
                        foreach (Graph.Node n in Graf.nodes)
                            n.active = 0;//вернуться в обычное состояние
                        timer2.Stop();//остановить таймер
                        act = false;//прекратить обход
                    }
                }
            }
            //следующий код просто обновляет полосу прогресса в зависимости от количества обработанных узлов
            int k = 0;
            foreach (Graph.Node n in Graf.nodes)
                if (n.active == 2) k += 1;
            progressBar1.Maximum = Graf.nodes.Count;
            if (progressBar1.Value != k)
                progressBar1.PerformStep();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            timer2.Interval = trackBar2.Value;//регулировка скорости обработки
            drag = -1;//отключить перемещение
            drage = -1;//отключить добавление/удаление рёбер
            foreach (Graph.Node n in Graf.nodes)
            {
                n.active = 0;//вернуть все узлы в обычное состояние
                n.prev = -1;
                n.chk = -1;//установить все параметры обхода DSF по умолчанию
            }
            Graf.nodes_q.Clear();//очистить очередь BFS
            if (!timer2.Enabled && !act)
            {
                if (Graf.nodes.Count > 0)//если граф не пустой
                {
                    Graf.nodes[0].active = 1;//активировать первый узел
                    Graf.nodes_q.Enqueue(Graf.nodes[0].id);//добавить первый узел в очередь BFS
                    timer2.Start();//запустить таймер обхода
                    act = true;//запустить обход
                    progressBar1.Value = 0;//обнулить полосу прогресса
                }
            }
            else
            {
                timer2.Stop();//остановить таймер если включен
                act = false;//прератить обход
            }
        }

        private void button3_Click(object sender, EventArgs e)//кнопка паузы
        {
            timer2.Interval = trackBar2.Value;//регулировка скорости обработки
            drag = -1;//отключить перемещение
            drage = -1;//отключить добавление/удаление рёбер
            if (!timer2.Enabled)//если таймер обхода не запущен
            {
                if (Graf.nodes.Count > 0)//если граф не пустой
                {
                    bool a = true;//есть ли в графе обрабатываемые узлы
                    foreach (Graph.Node n in Graf.nodes)
                        if (n.active != 0) a = false;//найти хотя бы одну
                    if (a)//если их нет, то начать обход с начала
                    {
                        Graf.nodes[0].active = 1;//активировать первый узел
                        Graf.nodes_q.Enqueue(Graf.nodes[0].id);//добавить первый узел в очередь BFS
                    }
                    timer2.Start();//запустить таймер обхода
                    act = true;//запустить обход
                }
            }
            else
            {
                timer2.Stop();//остановить таймер
                //важно что переменная act не меняется - обход не прекращен
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Graf.nodes.Count != 0)//сохранение возможно только если граф не пустой
                saveFileDialog1.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (File.Exists(saveFileDialog1.FileName))//если файл уже существует
                File.Delete(saveFileDialog1.FileName);//перезаписать его
            FileStream F = File.OpenWrite(saveFileDialog1.FileName);
            foreach (Graph.Node n in Graf.nodes)
            {
                string S = "";
                S += n.id.ToString() + ",";
                S += n.x.ToString() + ",";//добавить к строке все значимые параметры с разделителем ','
                S += n.y.ToString() + ",";
                if (n.edges.Count != 0)//если в узле есть рёбра
                {
                    foreach (int eg in n.edges)
                    {
                        S += eg.ToString() + ";";//добавить список смежности с разделителем ';'
                    }
                    S = S.Remove(S.Length - 1, 1);//удалить последний не нужный ';'
                }
                S += "," + n.name + "\n";
                byte[] info = new UTF8Encoding(true).GetBytes(S);
                F.Write(info, 0, info.Length);//записать узел в строку файла
            }
            F.SetLength(F.Length - 1);//удалить последний ненужный '\n' в конце файла
            F.Close();
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Graf.nodes.Clear();//очистить предыдущий граф
            StreamReader F = File.OpenText(openFileDialog1.FileName);
            while (!F.EndOfStream)//читать все строки из файла
            {
                string S = F.ReadLine();
                string[] SS = S.Split(",");//разделить строку (ожидается 4 элемента)
                List<int> L = new List<int>();//пустой список смежности
                if (SS[3] != "")
                {
                    string[] SSE = SS[3].Split(";");//отдельно разделить список смежности из файла на строки
                    foreach (string eg in SSE)
                        L.Add(int.Parse(eg));//заполнение списка смежности
                }
                Graf.LoadNode(int.Parse(SS[0]), int.Parse(SS[1]), int.Parse(SS[2]), SS[4], L);//добавить загруженный узел в граф
            }
            F.Close();
        }
    }
}
