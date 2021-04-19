using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;

namespace ClientServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpListener serverSocket = new TcpListener(IPAddress.Any, 7000);
                TcpClient clientSocket = default(TcpClient);
                Console.WriteLine(">>Server started");
                serverSocket.Start();
                clientSocket = serverSocket.AcceptTcpClient();
                NetworkStream stream = clientSocket.GetStream();
                byte[] messageFromClient = new byte[256];
                stream.Read(messageFromClient, 0, messageFromClient.Length);
                string resultOfRead = Encoding.ASCII.GetString(messageFromClient);
                ArrayList resArrayX = new ArrayList();
                int[] arrX;
                (from Match m in new Regex(@"\d+").Matches(resultOfRead)
                 select m.Value).ToList().ForEach(i => 
                 {
                     int num;
                     if (int.TryParse(i, out num)) resArrayX.Add(num);
                 });
                arrX = (int[])resArrayX.ToArray(typeof(int));
                Array.Sort(arrX);
                //ArrayList resArrayY = new ArrayList();
                double[] arrY = new double[arrX.Length];
                for (int i = 0; i < arrY.Length; i++)
                    arrY[i] = F(arrX[i]);
                Random rd = new Random();
                double x = rd.Next(arrX[0] * 1000, arrX[1] * 1000) * 0.001;
                double f1 = F(x);
                double f2 = Interpolation(arrX, arrY, x);
                double result = Math.Abs(Math.Round(f2, 3) - Math.Round(f1, 3));
                string resultOfWrite = x + ";" + Math.Round(f2, 3) + ";" + result + ";";
                byte[] bytes = Encoding.ASCII.GetBytes(resultOfWrite);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                clientSocket.Close();
                serverSocket.Stop();
                Console.WriteLine(">>Server stopped");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.ReadKey();
            }
        }
        static private double F(double x)
        {
            return Math.Exp(-Math.Atan(x)) * (Math.Exp(x) - Math.Exp(-x)) * Math.Sin(2 * x);
        }
        static private double Interpolation(int[] arrX, double[] arrY, double x)
        {
            double y = 0;
            for (int i = 0; i < arrX.Length; i++)
            {
                double basicsPol = 1;
                for (int j = 0; j < arrX.Length; j++)
                {
                    if (j != i)
                        basicsPol *= ((x - arrX[j]) / (arrX[i] - arrX[j]));
                }
                y += (basicsPol * arrY[i]);
            }
            return y;
        }
    }
}
