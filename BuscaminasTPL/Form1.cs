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

namespace BuscaminasTPL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void GenerateButtonArrays(int num)
        {
            int mv = 5, mh = 0;
            for (int i = 0; i < num; i++)
            {
                mh = 5;
                for (int j = 0; j < num; j++)
                {
                    Button btn = new Button();
                    btn.Height = 24;
                    btn.Width = 24;
                    btn.Top = mv;
                    btn.Left = mh;
                    char mn = cmatriz[i, j];
                    btn.Tag = i.ToString() + "," + j.ToString();
                    //btn.Text = matriz[i, j].ToString();
                    btn.Click += (s, ev) =>
                    {
                        //int a = int.Parse(btn.Tag.ToString().Split(',')[0]), b = int.Parse(btn.Tag.ToString().Split(',')[1]);
                        //if (mn.Equals('0'))
                        //    findLimits(a, b, num);
                        //else
                            ShowValue(btn, mn);
                    };
                    panel1.Controls.Add(btn);

                    mh += 24;
                    bmatriz[i, j] = btn;
                }
                mv += 24;
            }
        }
        public void ShowValue(Button b, char value)
        {
            casillasDescubiertas++;
            int totalCasillas = num * num;
            b.Text = value.ToString();
            if (value.ToString() == "*")
            {
                MessageBox.Show("Perdiste wey");
                foreach (var btn in bmatriz)
                {
                    btn.Enabled = false;
                }
            }
            else if (casillasDescubiertas == totalCasillas - totalMinas)
            {
                MessageBox.Show("Ganaste wey");
                foreach (var btn in bmatriz)
                {
                    btn.Enabled = false;
                }
            }
        }
        public void findLimits(int i, int j, int num)
        {
            ShowValue(bmatriz[i, j], cmatriz[i, j]);
            if (j + 1 < num)
            {
                if (cmatriz[i, j + 1].Equals('0'))
                    findLimits(i, j + 1, num);
                if (i + 1 < num && cmatriz[i + 1, j + 1].Equals('0'))
                    findLimits(i + 1, j + 1, num);
                if (i > 0 && cmatriz[i - 1, j + 1].Equals('0'))
                    findLimits(i - 1, j + 1, num);
            }
            if (j > 0)
            {
                if (cmatriz[i, j - 1].Equals('0'))
                    findLimits(i, j - 1, num);
                if (i + 1 < num && cmatriz[i + 1, j - 1].Equals('0'))
                    findLimits(i + 1, j - 1, num);
                if (i > 0 && cmatriz[i - 1, j - 1].Equals('0'))
                    findLimits(i - 1, j - 1, num);
            }
            if (i + 1 < num && cmatriz[i + 1, j].Equals('0'))
                findLimits(i + 1, j, num);
            if (i > 0 && cmatriz[i - 1, j].Equals('0'))
                findLimits(i - 1, j, num);
        }
        static ReaderWriterLockSlim padlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public int[,] matriz;
        public Char[,] cmatriz;
        public Button[,] bmatriz;
        int num;
        public int totalMinas;
        public int casillasDescubiertas;
        private async void btnGenerar_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            num = int.Parse(this.textBox1.Text);
            bmatriz = new Button[num, num];
            totalMinas = 0;
            casillasDescubiertas = 0;
            cmatriz = await Task.Run(() =>
                {
                    padlock.EnterReadLock();
                    int n = num;
                    char[,] m = new char[n, n];
                    padlock.ExitReadLock();
                    Console.WriteLine(n);
                    Random r = new Random();
                    for (int i = 0; i < num; i++)
                    {
                        for (int j = 0; j < num; j++)
                        {
                            if(r.Next(10) == 1)
                            {
                                m[i, j] = '*';
                                totalMinas++;
                            }
                        }
                    }
                    //padlock.EnterWriteLock();
                    //matriz = m;
                    //padlock.ExitWriteLock();
                    return m;
                }
            );
            cmatriz = await Task.Run(() =>
            {
                padlock.EnterReadLock();
                int n = num;
                char[,] m = cmatriz;
                padlock.ExitReadLock();
                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < num; j++)
                    {
                        if (!m[i, j].Equals('*'))
                        {
                            int val = 0;
                            if (j + 1 < num)
                            {
                                if (m[i, j + 1].Equals('*'))
                                    val++;
                                if (i + 1 < num && m[i + 1, j + 1].Equals('*'))
                                    val++;
                                if (i > 0 && m[i - 1, j + 1].Equals('*'))
                                    val++;
                            }
                            if (j - 1 >= 0)
                            {
                                if (m[i, j - 1].Equals('*'))
                                    val++;
                                if (i + 1 < num && m[i + 1, j - 1].Equals('*'))
                                    val++;
                                if (i > 0 && m[i - 1, j - 1].Equals('*'))
                                    val++;
                            }
                            if (i + 1 < num && m[i + 1, j].Equals('*'))
                                val++;
                            if (i > 0 && m[i - 1, j].Equals('*'))
                                val++;
                            m[i, j] = Convert.ToChar(val+48);
                        }
                        Console.WriteLine(m[i, j]);
                    }
                }
                return m;
            });
            GenerateButtonArrays(num);
            
        }
    }
}
