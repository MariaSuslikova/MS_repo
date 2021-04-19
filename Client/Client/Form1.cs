using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Client
{
    public partial class Form1 : Form
    {
        static int value = 4;
        static int num;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.RowCount = 4;
            dataGridView1.ColumnCount = 1;
        }
        public void buttonMath_Click(object sender, EventArgs e)
        {
            string res="";
            TcpClient client = null;
            try
            {
                for (int j = 0; j < value; j++)
                    res += dataGridView1[0, j].Value.ToString() + ",";
                ArrayList baseArrayX = new ArrayList();
                int[] baseArrX;
                (from Match m in new Regex(@"\d+").Matches(res)
                 select m.Value).ToList().ForEach(i =>
                 {
                     int num;
                     if (int.TryParse(i, out num)) baseArrayX.Add(num);
                 });
                baseArrX = (int[])baseArrayX.ToArray(typeof(int));
                Draw(baseArrX);
                byte[] bytes = Encoding.ASCII.GetBytes(res);
                client = new TcpClient("127.0.0.1", 7000);
                NetworkStream stream = client.GetStream();
                stream.Write(bytes, 0, bytes.Length);
                byte[] messageFromServer = new byte[256];
                stream.Read(messageFromServer, 0, messageFromServer.Length);
                string resultOfRead = Encoding.ASCII.GetString(messageFromServer);
                ArrayList resArrayX = new ArrayList();
                double[] arrX;
                string[] digits = Regex.Split(resultOfRead, @";");
                foreach (string value in digits)
                {
                    double number;
                    if (double.TryParse(value, out number))
                        resArrayX.Add(number);
                }
                arrX = (double[])resArrayX.ToArray(typeof(double));
                stream.Flush();
                client.Close();
                textBox2.Text = arrX[0].ToString();
                textBox1.Text = arrX[2].ToString();
                chart1.Series[1].Points.AddXY(arrX[0], arrX[1]);
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
        public void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            value = (int)numericUpDown1.Value;
            num = 0;
            dataGridView1.RowCount = value;
        }
        public void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            num++;
            if (num == value)
                dataGridView1.AllowUserToAddRows = false;
        }
        public void Draw(int[] X)
        {
            chart1.ChartAreas[0].AxisX.Maximum = 12;
            chart1.ChartAreas[0].AxisY.Maximum = 40;
            chart1.ChartAreas[0].AxisX.Minimum = -12;
            chart1.ChartAreas[0].AxisY.Minimum = -40;
            double a = -10, b = 10, h = 0.1, x;
            chart1.Series[0].Points.Clear();
            x = a;
            for (int i = 0; i < X.Length; i++)
                chart1.Series[2].Points.AddXY(X[i], Math.Round(F(X[i]), 2));
            while (x <= b)
            {
                chart1.Series[0].Points.AddXY(x, Math.Round(F(x), 2));
                x += h;
            }        
        }
        static private double F(double x)
        {
            return Math.Exp(-Math.Atan(x)) * (Math.Exp(x) - Math.Exp(-x)) * Math.Sin(2 * x);
        }
        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
        }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
        }
        private void chart1_Click(object sender, EventArgs e)
        {
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
