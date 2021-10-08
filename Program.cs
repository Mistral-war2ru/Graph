using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Graphs
{
    public static class Match
    {
        public static double degtorad(double deg)//градусы в радианы
        {
            return deg * Math.PI / 180;
        }

        public static double radtodeg(double rad)//радианы в градусы
        {
            return rad / Math.PI * 180;
        }

        public static double lengthdir_x(double len,double dir)//расстояние по X при передвижении по направлению
        {
            return len * Math.Cos(degtorad(dir));
        }

        public static double lengthdir_y(double len, double dir)//расстояние по Y при передвижении по направлению
        {
            return len * Math.Sin(degtorad(dir)) * (-1);
        }

        public static double point_direction(int x1,int y1,int x2,int y2)//угол направления между двумя точками 
        {
            return 180 - radtodeg(Math.Atan2(y1 - y2, x1 - x2));
        }

        public static double point_distance(int x1, int y1, int x2, int y2)//расстояние между двумя точками
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
    }

    public class Graph
    {
        public class Node
        {
            public int id;//уникальный идентификатор узла
            public int active;//статус обработки узла
            public int prev;//предыдущий узел (нужен для нерекурсивного DFS обхода)
            public int chk;//проверяемый узел (нужен для нерекурсивного DFS обхода)
            public int x;//координаты
            public int y;//для отрисовки вершины
            public string name;//отображаемое имя
            public List<int> edges;//список смежности

            public void AddEdge(int id)
            {
                if (!edges.Contains(id)) edges.Add(id);//добавить узел в список смежности если его там не было
            }

            public void RemoveEdge(int id)
            {
                edges.Remove(id);//удаление узла из списка смежности
            }
        };

        public List<Node> nodes = new List<Node>();//узлы графа
        private int maxid = 0;//для запоминания уникальных идентификаторов
        public int x = 0;//координаты
        public int y = 0;//появления новых узлов
        public int sz = 32;//размер узлов (для отрисовки)

        public Queue<int> nodes_q = new Queue<int>();//очередь для BFS обхода

        public void AddNode(string name)//добавление узла в граф
        {
            bool find = false;//найдено пустое место между 0 и максимальным известным идентификатором
            int id = 0;//новый уникальный идентификатор
            for (int i = 0; i < maxid; i++)//проверить для всех идентификаторов от 0 до максимального
            {
                bool exist = false;//такой идентификатор уже существует
                foreach (Node nd in nodes)
                {
                    if (nd.id == i)
                    {
                        exist = true;//найден указанный идентификатор
                        break;
                    }
                }
                if (!exist)//если не существует указанный идентификатор, то 
                {
                    id = i;//на его место
                    find = true;//можно поместить новый узел
                    break;
                }
            }
            if (!find)//если пустое место не найдено
            {
                id = maxid;
                maxid++;//просто добавить в конец
            }
            Node n = new Node();
            n.id = id;
            n.active = 0;
            n.prev = -1;
            n.chk = -1;
            n.x = x;
            n.y = y;
            //все параметры задаются по умолчанию
            if (name != "")
                n.name = name;
            else
                n.name = id.ToString();//если имя не указано, то прописать туда идентификатор
            n.edges = new List<int>();//пустой список смежности
            nodes.Add(n);
            nodes.Sort((x, y) => x.id.CompareTo(y.id));//сортировка по идентификатору для оптимизации
        }

        public void RemoveNode(int id)//удаление узла из графа
        {
            Node n = null;
            foreach (Node nd in nodes)
            {
                nd.edges.Remove(id);//удалить узел из списков смежности у всех других узлов
                if (nd.id == id)
                {
                    n = nd;//найти сам удаляемый узел
                }
            }
            nodes.Remove(n);
        }

        public void LoadNode(int id, int x, int y, string name, List<int> e)//добавление узла при загрузке из файла
        {
            Node n = new Node();
            if (maxid <= id)
                maxid = id + 1;//запомнить новый максимальный идентификатор
            n.id = id;
            n.active = 0;
            n.prev = -1;
            n.chk = -1;
            n.x = x;
            n.y = y;
            if (name != "")
                n.name = name;
            else
                n.name = id.ToString();
            n.edges = e;
            //все параметры, необходимые для создания узла, передаются в функцию, включая список смежности
            nodes.Add(n);
            nodes.Sort((xx, yy) => xx.id.CompareTo(yy.id));
        }
    }
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
