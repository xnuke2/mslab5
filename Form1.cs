using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace mslab5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<string> equations = new List<string>();

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            int countStage =Convert.ToInt32(numericUpDown1.Value);
            dataGridView1.Columns.Add("zero","");
            
            for(int i = 0; i < countStage; i++)
                dataGridView1.Columns.Add("S" + (i + 1), "S" + (i + 1));
            for (int i = 0; i < countStage; i++)
                dataGridView1.Rows.Add("S" + (i + 1));
            dataGridView1.Columns[0].ReadOnly = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1_ValueChanged(sender, e);
        }

        private void buttonSetIntensiv_Click(object sender, EventArgs e)
        {
            for(int i = 0;i<charts.Count;i++)
                charts[i].Dispose();
            charts.Clear();
            listBox1.Items.Clear();
            dataGridViewRezult.Rows.Clear();
            dataGridViewRezult.Columns.Clear();
            double[][] matrix = new double[dataGridView1.RowCount][];
            string yr = "";
            Func<double[], double>[] funcs = new Func<double[], double>[dataGridView1.RowCount];
            for (int yrIndex = 0; yrIndex < dataGridView1.RowCount; yrIndex++)
            {
                List<Point>[] vals = new List<Point>[dataGridView1.RowCount];
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = new List<Point>();
                
                for (int rowIndex = 0; rowIndex < dataGridView1.RowCount; rowIndex++)
                        if (Convert.ToString(dataGridView1[ yrIndex+1, rowIndex].Value) != "")
                        {
                            vals[rowIndex].Add(new Point(yrIndex + 1, rowIndex));
                        }


                    
                
                for (int columnsIndex = 0; columnsIndex < dataGridView1.Rows.Count; columnsIndex++)
                    if (yrIndex != columnsIndex)
                        if (Convert.ToString(dataGridView1[columnsIndex+1, yrIndex].Value) != "")
                        {
                            vals[yrIndex].Add(new Point(columnsIndex + 1, yrIndex));

                        }

                double[] ratio = new double[vals.Length];
                ratio[0] = 0;

                for (int i = 0; i < vals.Length; i++)
                {
                    if(i==yrIndex) 
                        for (int j = 0; j < vals[i].Count; j++)
                        {
                            ratio[i] -= Convert.ToDouble(dataGridView1[vals[i][j].X, vals[i][j].Y].Value);
                        }
                    else
                        if (vals[i] != null && vals[i].Count > 0)
                        {
                            ratio[i] = Convert.ToDouble(dataGridView1[vals[i][0].X, vals[i][0].Y].Value);
                        }
                    
                }
                
                yr = "";
                for(int i =0; i < ratio.Length;i++)
                    if(ratio[i]>0)
                        yr+= "+"+ratio[i]+"*S"+(i+1);
                    else if (ratio[i]<0)
                        yr += ratio[i] + "*S" + (i + 1);
                if (yr[0]=='+')
                    yr =yr.Remove(0,1);
                if (yr == "")
                {
                    MessageBox.Show("В строке " + (yrIndex + 1) + " отсутсвует значение");
                    return;
                }

                funcs[yrIndex] = Yvals =>
                {
                    double sum = 0;
                    for (int j = 0; j < ratio.Length&&j<Yvals.Length; j++)
                        sum += ratio[j] * Yvals[j];
                    return sum;
                };
                matrix[yrIndex]=new double[ratio.Length+1];
                for (int i = 0;i < ratio.Length;i++)
                    matrix[yrIndex][i]= ratio[i];
                listBox1.Items.Add(yr);
                equations.Add(yr);
            }
            double[] initial = new double[listBox1.Items.Count];
            initial[0] = 1;
            RungeKutta rungeKutta = new RungeKutta(initial, funcs);
            var tmp = rungeKutta.Solve(10,0.1);
            for (int i = 0; i < matrix[0].Length; i++)
                matrix[0][i] = 1;
            SLAY sLAY = new SLAY(matrix);
            var res = sLAY.Find();
            for (int i=0;i< res[res.Length - 1][0].Length;i++)
                dataGridViewRezult.Columns.Add("X" +(i+1), "X" + (i+1));
            dataGridViewRezult.Rows.Add("Гаусс");
            for (int i = 1; i < res[res.Length - 1][0].Length+1; i++)
                dataGridViewRezult[i-1,0].Value = res[res.Length-1][0][i-1];
            int x = 0;
            int y = 0;
            for (int i = 0;i<listBox1.Items.Count;i++)
            {
                Chart chart2 = new Chart();
                chart2.Series.Add("P"+(i+1));
                chart2.Size = new Size(300, 250);
                chart2.Location = new Point(x, y);
                chart2.ChartAreas.Add(new ChartArea(""));
                chart2.Legends.Add("");
                chart2.Series[0].IsXValueIndexed = true;
                chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart2.Titles.Add("Уравнение"+(i+1));
                for(int j = 0; j < tmp.Count; j++)
                    chart2.Series[0].Points.AddXY(Math.Round( tmp[j][0],3), Math.Round(tmp[j][i+1], 3));
                panel1.Controls.Add(chart2);
                chart2.Visible = true;
                x += 305;
                charts.Add(chart2);
            }

        }
        List<Chart>charts = new List<Chart>();
    }
}
