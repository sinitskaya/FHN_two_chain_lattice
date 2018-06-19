using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using System.Windows.Media;

namespace FHN_Chain
{
    delegate double Del(int flag_first_elem_exist, int rank, int eqNumber, double t, List<double> parameter, List<double> value, double resh_start_value, double elem);
    class Program
    {
        static void Main(string[] args)
        {
            int flag_first_elem_exist = 0;//1-то считать не надо первый, он известен из пред файла
            String inFileName, outFileNameDat, outFileNameOsc;
            inFileName = "inputFHN.txt";
            if (args.Length == 1)
                inFileName = args[0];

            List<double> value = new List<double>(), parameter = new List<double>(), data = new List<double>();

            bool fullLog;
            int rank;
            double t, dt, tEst, tMin, tInterval, tMax, tFin;
            string boundaryConditions;//граничные условия

            StreamReader sr = new StreamReader(inFileName);
            String str = sr.ReadToEnd();//считывает вплоть до конца файла в одной операции.
            String[] strs;
            String[] tmpStrs;

            strs = str.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);//удалить пустые записи

            outFileNameDat = strs[0];
            outFileNameOsc = strs[1];

            tmpStrs = strs[2].Split(new Char[] { ' ' });
            fullLog = Convert.ToBoolean(tmpStrs[2]);
            tmpStrs = strs[3].Split(new Char[] { ' ' });
            rank = Convert.ToInt32(tmpStrs[2]);

            tmpStrs = strs[4].Split(new Char[] { ' ' });
            t = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[5].Split(new Char[] { ' ' });
            dt = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[6].Split(new Char[] { ' ' });
            tEst = Convert.ToDouble(tmpStrs[2]);//время установления
            tmpStrs = strs[7].Split(new Char[] { ' ' });
            tMin = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[8].Split(new Char[] { ' ' });
            tInterval = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[9].Split(new Char[] { ' ' });
            tMax = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[10].Split(new Char[] { ' ' });
            tFin = Convert.ToDouble(tmpStrs[2]);

            bool j = true;
            int i = 0;
            while (j)
            {
                try
                {
                    parameter.Add(Convert.ToDouble(strs[12 + i]));
                    i++;//кол парам
                }
                catch (FormatException)
                {
                    j = false;
                }
            }
            boundaryConditions = Convert.ToString(strs[12 + i]);
            Console.WriteLine("Boundary: {0}", boundaryConditions);

            Del rightPart = rightPartFree;
            if (rank == 2)
            {
                rightPart = rightPart1;
            }
            else
            {
                rightPart = rightPartFree;
            }
            /////////////////////////////////////////////////
            int k1 = 0;//кол НУ
            for (k1 = 0; k1 < rank; k1++)
                value.Add(Convert.ToDouble(strs[14 + i + k1]));

            int size_line, size_column;
            tmpStrs = strs[14 + i + k1].Split(new Char[] { ' ','x' });
            size_line = Convert.ToInt16(tmpStrs[2]);
            size_column = Convert.ToInt16(tmpStrs[3]);

            //List<double> param_a = new List<double>();
            double[,] param_a = new double[size_line, size_column];
            int k2;
            for (k2 = 0; k2 < size_line; k2++)
            {
                tmpStrs = strs[16 + i + k1 + k2].Split(new Char[] { ' ' });
                for (int j1 = 0; j1 < tmpStrs.Length; j1++)
                    param_a/*.Add*/[k2, j1] = Convert.ToDouble(tmpStrs[j1]) ;
            }

            double [,] valueX = new double[size_line, size_column];
            //List<double> valueX = new List<double>();
            int k3;
            for (k3 = 0; k3 < size_line; k3++)
            {
                tmpStrs = strs[17 + i + k1 + k2 + k3].Split(new Char[] { ' ' });
                for (int j1 = 0; j1 < tmpStrs.Length; j1++)
                    valueX/*.Add*/[k3, j1] =( Convert.ToDouble(tmpStrs[j1]));
            }

            double[,] valueY = new double[size_line, size_column];
            //List<double> valueY = new List<double>();
            int k4 = 0;
            for (k4 = 0; k4 < size_line; k4++)
            {
                tmpStrs = strs[18 + i + k1 + k2 + k3 + k4].Split(new Char[] { ' ' });
                for (int j1 = 0; j1 < tmpStrs.Length; j1++)
                    valueY/*.Add*/[k4, j1] =( Convert.ToDouble(tmpStrs[j1]));
            }

            int rank2;
            tmpStrs = strs[18 + i + k1 + k2 + k3 + k4].Split(new Char[] { ' ' });
            rank2 = Convert.ToInt32(tmpStrs[2]);
            List<double> value2 = new List<double>(), parameter2 = new List<double>();
            int k5 = 0;
            for (k5 = 0; k5 < (int)(rank2/2) + 2; k5++)
                parameter2.Add(Convert.ToDouble(strs[20 + i + k1 + k2 + k3 + k4 + k5]));
            int k6 = 0;
            for (k6 = 0; k6 < rank2; k6++)
                value2.Add(Convert.ToDouble(strs[21 + i + k1 + k2 + k3 + k4 + k5 + k6]));
            ///////////////////////
            List<double> x = new List<double>();
            if (flag_first_elem_exist == 1)
            {
                StreamReader sr1 = new StreamReader("output_osc1.txt");
                String str1 = sr1.ReadToEnd();//считывает вплоть до конца файла в одной операции.
                String[] strs1;
                String[] tmpStrs1;
                strs1 = str1.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);//удалить пустые записи
                for (int ij = 0; ij < strs1.Count(); ij++)
                {
                    tmpStrs1 = strs1[ij].Split(new Char[] { ' ' });
                    x.Add(Convert.ToDouble(tmpStrs1[4]));//3 пробела,+ 1-время
                }
                sr1.Close();
                //double alpha = 0.5;
                //for (int ij = 0; ij < x.Count(); ij++)
                //{ x[ij] = alpha * x[ij]; }//для первого элемента
            }
            //value[0] = x[0];
            /////////////////
            double t_ = t;
            int ijk = 0; double d = 0;///12412
            //double omega1 = 0, omega2 = 0;
            //начальные значения
            File.WriteAllText(outFileNameOsc, string.Empty);//очистка файла
            File.WriteAllText("output_osc_resh.txt", string.Empty);//очистка файла
            File.WriteAllText("output_osc_second_chain.txt", string.Empty);//очистка файла
            if (t_ == 0)
            {
                StreamWriter swOsc = new StreamWriter(outFileNameOsc, true);
                StreamWriter swOsc_resh = new StreamWriter("output_osc_resh.txt", true);
                StreamWriter swOsc_second_chain = new StreamWriter("output_osc_second_chain.txt", true);
                swOsc.Write("{0:F1}", t_); swOsc_resh.Write("{0:F1}", t_); swOsc_second_chain.Write("{0:F1}", t_);
                //swOsc.Write(t_);//:F2
                for (int l = 0; l < rank; l += 2)
                {
                    swOsc.Write("  ");
                    swOsc.Write("{0:F7}", value[l]);
                }

                for (int l = 0; l < size_line; l++)
                    for (int l1 = 0; l1 < size_column; l1++)
                    {
                        swOsc_resh.Write(" ");
                        swOsc_resh.Write("{0:F2}", valueX[l, l1]);
                    }
                for (int l = 0; l < rank2; l += 2)
                {
                    swOsc_second_chain.Write("  ");
                    swOsc_second_chain.Write("{0:F7}", value2[l]);
                }
                swOsc.WriteLine();
                swOsc.Close();
                swOsc_resh.WriteLine();
                swOsc_resh.Close();
                swOsc_second_chain.WriteLine();
                swOsc_second_chain.Close();
            }
            while (t_ < tFin)//счет
            {
                if (t_ >= tMin && t_ <= tMax)//вывод
                {
                    if (fullLog && Convert.ToInt64(t_ / dt) % Convert.ToInt32(tInterval / dt) == 0)
                    {
                        StreamWriter swOsc = new StreamWriter(outFileNameOsc, true);
                        swOsc.Write(t_);
                        for (int l = 0; l < rank; l += 2)
                            swOsc.Write("\t{0}\t{1}", value[l], value[l + 1]);
                        swOsc.WriteLine();
                        swOsc.Close();
                        dataoutput(rank, ref t_, ref dt, tEst, tMin, tMax, tFin, boundaryConditions, value, data, parameter, outFileNameDat);
                    }
                    else if (Convert.ToInt64(t_ / dt) % Convert.ToInt32(tInterval / dt) == 0)
                    {                        
                        StreamWriter swOsc = new StreamWriter(outFileNameOsc, true);
                        StreamWriter swOsc_resh = new StreamWriter("output_osc_resh.txt", true);
                        StreamWriter swOsc_second_chain = new StreamWriter("output_osc_second_chain.txt", true);
                        swOsc.Write("{0:F1}", t_); swOsc_resh.Write("{0:F1}", t_); swOsc_second_chain.Write("{0:F1}", t_);
                        for (int l = 0; l < rank; l += 2)
                        {
                            swOsc.Write("  ");
                            swOsc.Write("{0:F7}", value[l]);
                        }
                        //resh
                        for (int l = 0; l < size_line; l ++)
                            for(int l1 = 0; l1 < size_column; l1++)
                            {
                               swOsc_resh.Write(" ");
                               swOsc_resh.Write("{0:F2}", valueX[l,l1]);
                            }
                        //2я цепочка
                        for (int l = 0; l < rank2; l += 2)
                        {
                            swOsc_second_chain.Write("  ");
                            swOsc_second_chain.Write("{0:F7}", value2[l]);
                        }
                        swOsc.WriteLine();
                        swOsc.Close();
                        swOsc_resh.WriteLine();
                        swOsc_resh.Close();
                        swOsc_second_chain.WriteLine();
                        swOsc_second_chain.Close();
                        //if (t_ > tEst && t + dt> tMax)
                        //dataoutput(rank, ref t_, ref dt, tEst, tMin, tMax, tFin, boundaryConditions, value, data, parameter, outFileNameDat);
                    }
                }
                //////////////////////////////////////
                double xx = 0;///12412 ijk=0
                if (flag_first_elem_exist == 1)
                {
                    double alpha = 0.5;
                    xx = x[0];
                    if (t_ > d && t_ < d + 2*tInterval)
                    {
                        if (ijk >= x.Count-1)
                            xx = x[x.Count - 1];
                        else
                            xx = (x[ijk] + x[ijk + 1]) / 2;
                        if (t_ + dt > d + 2*tInterval)
                        {
                            d = d + 2*tInterval;
                            ijk++;
                        }
                    }
                    if (t_ == d + 2*tInterval)//маловероятно
                    {
                        xx = x[(int) (ijk*alpha)];
                        if (t_ + dt > d + 2*tInterval)
                        {
                            d = d + 2*tInterval;
                            ijk++;
                        }
                    }
                }
                else { xx = 0; }
                /////////////////////////////////////
                calculate(rank, ref t_, ref dt, tEst, tMin, tMax, tFin, boundaryConditions, value, data, parameter, outFileNameDat);
                rk4(flag_first_elem_exist, rank, ref t_, ref dt, rightPart, value, parameter, param_a, valueX, valueY, size_column, size_line, rank2, value2, parameter2, xx);////////////////////
                t_ += dt;
                Console.SetCursorPosition(2,1);
                Console.Write("{0}",(int)(((double)t_ / tMax)*100));
            }
            //string soundfile = "sound.";
           /* byte[] tb = File.ReadAllBytes("Sound.wav");
            var sound = new System.Media.SoundPlayer("Sound.wav"));
            sound.PlaySync();*/
            //private NAudio.Wave.BlockAlignReductionStream stream = null;

             //System.Media.SoundPlayer pl = new System.Media.SoundPlayer();
             //pl.SoundLocation = "Sound.wav";
             //pl.Load();
             //pl.PlaySync();
             System.Media.SystemSounds.Exclamation.Play();           
            // NAudio
        }

        static void rk4(int flag_first_elem_exist, int rank, ref double t, ref double dt, Del rightPart, List<double> value, List<double> parameter, double[,] param_a, double[,] valueX, double[,] valueY, int size_column, int size_line, int rank2, List<double> value2, List<double> parameter2, double x_alph)
        {
            int i;
            double[,] k = new double[4, rank + 2*size_line*size_column + rank2];
            List<double> value_ = new List<double>();
            double[,] valueX_ = new double[size_line, size_column];
            double[,] valueY_ = new double[size_line, size_column];
            List<double> value2_ = new List<double>();
            double elem = x_alph;
            for (i = 0; i < rank; i++)
                k[0, i] = rightPartFree(flag_first_elem_exist, rank, i, t, parameter, value, valueX[0, 0], value2[0]) * dt;////size_column, не используется в rightPartFree убрать Del//valueX[0,0]
            for (int x = 0; x < size_line; x++)                                                         //value2[0]
                for(int y = 0; y < size_column; y++)                                                    //elem
                {
                    k[0, i] = rightPartFree_lattice(x, y, i, t, parameter, param_a, valueX, valueY, size_column, size_line, value[rank-2], value2[rank2-2]) * dt;
                    i++;                                                                                                 //лев верхний с пер // нижний правый со второй цепочкой
                    k[0, i] = rightPartFree_lattice(x, y, i, t, parameter, param_a, valueX, valueY, size_column, size_line, value[rank - 2], value2[rank2 - 2]) * dt;
                    i++;
                }
            for (int r = 0; r < rank2; r++)
            {
                k[0, i] = rightPart(flag_first_elem_exist, rank2, r, t, parameter2, value2, valueX[size_line - 1, size_column - 1], value[0]) * dt;////size_column, value[rank - 2] не используется в rightPartFree убрать Del
                i++;
            }
            ///////////////////////////////////
            for (i = 0; i < rank; i++)
                value_.Add(value[i] + k[0, i] * 0.5);
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    valueX_[x, y] = valueX[x, y] + k[0, i]*0.5;
                    i++;
                    valueY_[x, y] = valueY[x, y] + k[0, i] * 0.5;
                    i++;
                }
            for (int r = 0; r < rank2; r++)
            {
                value2_.Add(value2[r] + k[0, i] * 0.5);
                i++;
            }
            ////////////////////////
            for (i = 0; i < rank; i++)
                k[1, i] = rightPart(flag_first_elem_exist, rank, i, t + dt * 0.5, parameter, value_, valueX_[0, 0], value2_[0]) * dt;//value2_[0]
            for (int x = 0; x < size_line; x++)                                                                     //elem
                for (int y = 0; y < size_column; y++)
                {
                    k[1, i] = rightPartFree_lattice(x, y, i, t + dt * 0.5, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2], value2_[rank2 - 2]) *dt;
                    i++;
                    k[1, i] = rightPartFree_lattice(x, y, i, t + dt * 0.5, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2], value2_[rank2 - 2]) * dt;
                    i++;
                }
            for (int r = 0; r < rank2; r++)
            {
                k[1, i] = rightPart(flag_first_elem_exist, rank2, r, t + dt * 0.5, parameter2, value2_, valueX_[size_line - 1, size_column - 1], value_[0]) * dt;
                i++;
            }
            //////////////////////////
            for (i = 0; i < rank; i++)
                value_[i] = value[i] + k[1, i] * 0.5;
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    valueX_[x, y] = valueX[x, y] + k[1, i] * 0.5;
                    i++;//
                    valueY_[x, y] = valueY[x, y] + k[1, i] * 0.5;
                    i++;
                }
            for (int r = 0; r < rank2; r++)
            {
                value2_[r] = value2[r] + k[1, i] * 0.5;
                i++;
            }
            ////////////////////////////
            for (i = 0; i < rank; i++)
                k[2, i] = rightPart(flag_first_elem_exist, rank, i, t + dt * 0.5, parameter, value_, valueX_[0, 0], value2_[0]) * dt;
            for (int x = 0; x < size_line; x++)                                                                     //elem
                for (int y = 0; y < size_column; y++)
                {
                    k[2, i] = rightPartFree_lattice(x, y, i, t + dt * 0.5, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2], value2_[rank2 - 2]) * dt;
                    i++;
                    k[2, i] = rightPartFree_lattice(x, y, i, t + dt * 0.5, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2], value2_[rank2 - 2]) * dt;
                    i++;
                }
            for (int r = 0; r < rank2; r++)
            {
                k[2, i] = rightPart(flag_first_elem_exist, rank2, r, t + dt * 0.5, parameter2, value2_, valueX_[size_line - 1, size_column - 1], value_[0]) * dt;
                i++;
            }
            ////////////////////////////
            for (i = 0; i < rank; i++)
                value_[i] = value[i] + k[2, i];
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    valueX_[x, y] = valueX[x, y] + k[2, i];
                    i++;//
                    valueY_[x, y] = valueY[x, y] + k[2, i];
                    i++;
                }
            for (int r = 0; r < rank2; r++)
            {
                value2_[r] = value2[r] + k[2, i];
                i++;
            }
            ///////////////////////////////////////
            for (i = 0; i < rank; i++)
                k[3, i] = rightPart(flag_first_elem_exist, rank, i, t + dt, parameter, value_, valueX_[0, 0], value2_[0]) * dt;//size_column, value[rank - 2] не используется в rightPartFree
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    k[3, i] = rightPartFree_lattice(x, y, i, t + dt, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2], value2_[rank2 - 2]) * dt;
                    i++;
                    k[3, i] = rightPartFree_lattice(x, y, i, t + dt, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2], value2_[rank2 - 2]) * dt;
                    i++;
                }
            for (int r = 0; r < rank2; r++)
            {
                k[3, i] = rightPart(flag_first_elem_exist, rank2, r, t + dt, parameter2, value2_, valueX_[size_line - 1, size_column - 1], value_[0]) * dt;//size_column, value[rank - 2] не используется в rightPartFree
                i++;
            }
            ///////////////////////////////////
            for (i = 0; i < rank; i++)
                value[i] = value[i] + 1.0 / 6 * (k[0, i] + 2 * k[1, i] + 2 * k[2, i] + k[3, i]);
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    valueX[x, y] = valueX[x, y] + 1.0 / 6 * (k[0, i] + 2 * k[1, i] + 2 * k[2, i] + k[3, i]);
                    i++;//
                    valueY[x, y] = valueY[x, y] + 1.0 / 6 * (k[0, i] + 2 * k[1, i] + 2 * k[2, i] + k[3, i]);
                    i++;
                }
            for (int r = 0; r < rank2; r++)
            {
                value2[r] = value2[r] + 1.0 / 6 * (k[0, i] + 2 * k[1, i] + 2 * k[2, i] + k[3, i]);
                i++;
            }
            value_.Clear();
            value_.Capacity = 0;
            value2_.Clear();
            value2_.Capacity = 0;
        }

        static void calculate(int rank, ref double t, ref double dt, double tEst, double tMin, double tMax, double tFin, String boundaryConditions,
            List<double> value, List<double> data, List<double> parameter, String outFileNameDat)
        {
            if (data.Count < rank/2 )
            {
                for (int i = 0; i < rank / 2 + 7; i++)//rank/2 + 1
                {
                    data.Add(0.0);
                }
                for (int i = 0; i <  rank; i++)
                {
                    value.Add(0.0); 
                }
            }
            else if (t > tEst && t < tFin)
            {
                double T1_average, TN_average, omega1=0, omega2=0;
                int Flag = 0;
                int j = 1;//индекс для data
                for (int i = 0; i < rank; i += 2)//для каждого элемента           
                {
                    if (value[i] > 0.0 && value[rank + i] <= 0.0 ) //происходит оборот    
                    {
                        if (data[0] == 0)
                        { data[rank/2 + 1] = t;}//t1 первого элемента

                        if((data[rank / 2 + 3] == 0) && (j == rank/2))// t1 для последнего элемента
                        { data[rank / 2 + 3] = t; }//j == rank/2 - последний элемент
                     
                        if (j == 1)//если изменяется значение частоты первого элемента
                        { data[rank / 2 + 2] = t; }//tn первого элемента

                        if (j == rank / 2)//tn для последнего элемента
                        { data[rank / 2 + 4] = t; }

                        data[0] = t;
                        data[j] += 1.0;          //подсч средн част //частота jго элемента     
                        Flag = 1;

                        T1_average = 1.0 * (data[rank / 2 + 2] - data[rank / 2 + 1]) / (data[1]-1);//средний период первого элемента
                        TN_average = 1.0 * (data[rank / 2 + 4] - data[rank / 2 + 3]) / (data[rank/2]-1);//средний период последнего элемента
                        omega1 = 2 * Math.PI / T1_average;
                        omega2 = 2 * Math.PI / TN_average;
                        data[rank / 2 + 5] = omega1;
                        data[rank / 2 + 6] = omega2;
                        
                        /*if ((t + dt > tMax) && (f==0))//вывод 
                        {
                            StreamWriter swData = new StreamWriter(outFileNameDat, true);
                            swData.Write("{0:F8}", omega1);
                            swData.Write("    ");
                            swData.Write("{0:F8}", omega2);
                            swData.Write("    ");
                            swData.WriteLine();
                            swData.Close();
                            f++;
                        }*/
                    }
                    j++;
                }

                if ((t >= tMin && t <= tMax))//выввод  //(Flag == 1) &&   
                    dataoutput(rank, ref t, ref dt, tEst, tMin, tMax, tFin, boundaryConditions, value, data, parameter, outFileNameDat);             
            }
            for (int i = 0; i < rank; i++)
            {
                value[i + rank] = value[i];
            }
        }
        
        static void dataoutput(int rank, ref double t, ref double dt, double tEst, double tMin, double tMax, double tFin, String boundaryConditions,
            List<double> value, List<double> data, List<double> parameter, String outFileNameDat)
        {
            if (t + dt > tMax)//вывод только последней строки "omega1 omega2"
            {
                StreamWriter swData = new StreamWriter(outFileNameDat, true);
                //swData.Write("{0:F2}", t);
                swData.Write(parameter[0]);
                swData.Write("    ");
                for (int i = data.Count - 2; i < data.Count; i++)//i=0  //data.Count - 3
                {
                    //swData.Write("    ");
                    swData.Write("{0:F8}", data[i]);
                    swData.Write("    ");
                }
                swData.WriteLine();
                swData.Close();
            }
        }

        static double rightPart1(int flag_first_elem_exist, int rank, int eqNumber, double t, List<double> parameter, List<double> value, double resh_start_value, double elem) // 1 element
        {//этот метод не исполюзую
            double a1 = parameter[0], da = parameter[1], betta = parameter[2], eps = parameter[3];
            double result = 0;
            if (eqNumber == 0)
            {
                result = value[0] - Math.Pow(value[0], 3.0) / 3.0 - value[1] + betta*(resh_start_value - value[0]);
            }
            else
            {
                result = eps * (value[0] + a1);
            }
            return result;
        }

        static double rightPartFree(int flag_first_elem_exist, int rank, int eqNumber/*i*/, double t, List<double> parameter, List<double> value, double elem_resh_value, double elem)
        {
            double betta = parameter[parameter.Count - 2], eps = parameter[parameter.Count - 1];
            double[] a = new double[rank/2];
            for (int i = 0; i < a.Count(); i++)
            { a[i] = parameter[i]; }
            double result = 0;
            if (eqNumber / 2 * 2 != eqNumber)//четный т е y(нечетн i)
            {
                result = eps * (value[eqNumber - 1] + a[eqNumber/2] );
            }
            else if (eqNumber == 0)
            {
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (elem + value[eqNumber + 2] - 2*value[eqNumber]);//соединение со второй цепочкой
                if (flag_first_elem_exist==1)
                    result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (elem + value[eqNumber + 2] - value[eqNumber]);//для одной цепочки у которой первый не интегр-ся
                //result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber + 2] - value[eqNumber]);//для одной цепочки
            }
            else if (eqNumber == rank - 2)
            {
                //result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber - 2] - value[eqNumber]);//для одной цепочки
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (elem_resh_value - 2*value[eqNumber] + value[eqNumber - 2]);//соединение с решеткой
            }
            else
            {
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber + 2] - 2 * value[eqNumber] + value[eqNumber - 2]);
            }           
            return result;
        }

        static double rightPartFree_lattice(int i, int j, int k, double t, List<double> parameter, double [,] param_a, double [,] valueX, double[,] valueY, int size_column, int size_line, double chain1_value, double chain2_value)
        {
            double betta = parameter[parameter.Count - 2], eps = parameter[parameter.Count - 1];
            double result = 0;
            if (k / 2 * 2 != k)//четный т е y(нечетн k)
            {
                result = eps * (valueX[i, j] + param_a[i, j]);// было valueY исправили
            }
            else if (i == 0 && j == 0)//верхний левый
            {
                result = valueX[0, 0] - Math.Pow(valueX[0, 0], 3.0) / 3.0 - valueY[0, 0] + betta * (chain1_value + valueX[0, 1] + valueX[1, 0] - 3 * valueX[0, 0]);
            }
            else if (i == size_line - 1 && j == size_column - 1)//нижний правый
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (chain2_value + valueX[i, j - 1] + valueX[i - 1, j] - 3 * valueX[i, j]);
            }//3elem
            ////////////////////////2 elem
            else if (i == 0 && j == size_column - 1)//верхний правый
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j - 1] + valueX[i + 1, j] - 2 * valueX[i, j]);
            }
            else if (i == size_line - 1 && j == 0)//нижний левый
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j + 1] + valueX[i - 1, j] - 2 * valueX[i, j]);
            }            
            ////////////////////////2 elem
            ////////////////////////3 elem
            else if (i == 0 && 0 < j && j < size_column - 1)//все элементы в первой строке между первым и последним в строке эл-ми
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j - 1] + valueX[i, j + 1] + valueX[i + 1, j] - 3 * valueX[i, j]);
            }
            else if (j == 0 && 0 < i && i < size_line - 1)//все элементы в первом столбце между первым и последнем в столбце элем-ми
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i - 1, j] + valueX[i, j + 1] + valueX[i + 1, j] - 3 * valueX[i, j]);
            }
            else if (j == size_column - 1 && 0 < i && i < size_line - 1)//все элемнты в последнем столбце между первым и последним в этом столбце эл-ми
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i - 1, j] + valueX[i, j - 1] + valueX[i + 1, j] - 3 * valueX[i, j]);
            }
            else if (i == size_line - 1 && 0 < j && j < size_column - 1)//все элем в последней строке между первым и последним эл в этой строке
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j - 1] + valueX[i, j + 1] + valueX[i - 1, j] - 3 * valueX[i, j]);
            }
            ////////////////////////3 elem
            ////////////////////////4 elem
            else
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i-1, j] + valueX[i, j + 1] + valueX[i + 1, j] + valueX[i,j-1] - 4 * valueX[i, j]);
            }
            ////////////////////////4 elem
            return result;
        }
    }
}