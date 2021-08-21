using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Graphs
{
    public static class Match
    {
        public static double degtorad(double deg)//������� � �������
        {
            return deg * Math.PI / 180;
        }

        public static double radtodeg(double rad)//������� � �������
        {
            return rad / Math.PI * 180;
        }

        public static double lengthdir_x(double len,double dir)//���������� �� X ��� ������������ �� �����������
        {
            return len * Math.Cos(degtorad(dir));
        }

        public static double lengthdir_y(double len, double dir)//���������� �� Y ��� ������������ �� �����������
        {
            return len * Math.Sin(degtorad(dir)) * (-1);
        }

        public static double point_direction(int x1,int y1,int x2,int y2)//���� ����������� ����� ����� ������� 
        {
            return 180 - radtodeg(Math.Atan2(y1 - y2, x1 - x2));
        }

        public static double point_distance(int x1, int y1, int x2, int y2)//���������� ����� ����� �������
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
    }

    public class Graph
    {
        public class Node
        {
            public int id;//���������� ������������� ����
            public int active;//������ ��������� ����
            public int prev;//���������� ���� (����� ��� �������������� DFS ������)
            public int chk;//����������� ���� (����� ��� �������������� DFS ������)
            public int x;//����������
            public int y;//��� ��������� �������
            public string name;//������������ ���
            public List<int> edges;//������ ���������

            public void AddEdge(int id)
            {
                if (!edges.Contains(id)) edges.Add(id);//�������� ���� � ������ ��������� ���� ��� ��� �� ����
            }

            public void RemoveEdge(int id)
            {
                edges.Remove(id);//�������� ���� �� ������ ���������
            }
        };

        public List<Node> nodes = new List<Node>();//���� �����
        private int maxid = 0;//��� ����������� ���������� ���������������
        public int x = 0;//����������
        public int y = 0;//��������� ����� �����
        public int sz = 32;//������ ����� (��� ���������)

        public Queue<int> nodes_q = new Queue<int>();//������� ��� BFS ������

        public void AddNode(string name)//���������� ���� � ����
        {
            bool find = false;//������� ������ ����� ����� 0 � ������������ ��������� ���������������
            int id = 0;//����� ���������� �������������
            for (int i = 0; i < maxid; i++)//��������� ��� ���� ��������������� �� 0 �� �������������
            {
                bool exist = false;//����� ������������� ��� ����������
                foreach (Node nd in nodes)
                {
                    if (nd.id == i)
                    {
                        exist = true;//������ ��������� �������������
                        break;
                    }
                }
                if (!exist)//���� �� ���������� ��������� �������������, �� 
                {
                    id = i;//�� ��� �����
                    find = true;//����� ��������� ����� ����
                    break;
                }
            }
            if (!find)//���� ������ ����� �� �������
            {
                id = maxid;
                maxid++;//������ �������� � �����
            }
            Node n = new Node();
            n.id = id;
            n.active = 0;
            n.prev = -1;
            n.chk = -1;
            n.x = x;
            n.y = y;
            //��� ��������� �������� �� ���������
            if (name != "")
                n.name = name;
            else
                n.name = id.ToString();//���� ��� �� �������, �� ��������� ���� �������������
            n.edges = new List<int>();//������ ������ ���������
            nodes.Add(n);
            nodes.Sort((x, y) => x.id.CompareTo(y.id));//���������� �� �������������� ��� �����������
        }

        public void RemoveNode(int id)//�������� ���� �� �����
        {
            Node n = null;
            foreach (Node nd in nodes)
            {
                nd.edges.Remove(id);//������� ���� �� ������� ��������� � ���� ������ �����
                if (nd.id == id)
                {
                    n = nd;//����� ��� ��������� ����
                }
            }
            nodes.Remove(n);
        }

        public void LoadNode(int id, int x, int y, string name, List<int> e)//���������� ���� ��� �������� �� �����
        {
            Node n = new Node();
            if (maxid <= id)
                maxid = id + 1;//��������� ����� ������������ �������������
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
            //��� ���������, ����������� ��� �������� ����, ���������� � �������, ������� ������ ���������
            nodes.Add(n);
            nodes.Sort((x, y) => x.id.CompareTo(y.id));
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
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
