using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                yr = "";
                for (int rowIndex = 0; rowIndex < dataGridView1.RowCount; rowIndex++)
                        if (Convert.ToString(dataGridView1[ yrIndex+1, rowIndex].Value) != "")
                        {
                            yr += Convert.ToString(dataGridView1[yrIndex + 1, rowIndex].Value) + "*S" + (rowIndex + 1) + "+";
                            vals[rowIndex].Add(new Point(yrIndex + 1, rowIndex));
                        }
                            
                if(yr!="")
                    yr =yr.Remove(yr.Length-1);
                
                for (int columnsIndex = 0; columnsIndex < dataGridView1.Rows.Count; columnsIndex++)
                    if (yrIndex != columnsIndex)
                        if (Convert.ToString(dataGridView1[columnsIndex+1, yrIndex].Value) != "")
                        {
                            vals[yrIndex].Add(new Point(columnsIndex + 1, yrIndex));
                            yr += "-" + Convert.ToString(dataGridView1[columnsIndex + 1, yrIndex].Value) + "*S" + (yrIndex + 1);

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
            var last = tmp[tmp.Count-1];
            double check = 0;
            for(int j = 1; j < last.Length; j++)
            {
                //last[j] = Math.Round(last[j], 5);
                check += last[j];
            }
            for (int i = 0; i < matrix[0].Length; i++)
                matrix[0][i] = 1;
            SLAY sLAY = new SLAY(matrix);
            var res = sLAY.Find();
            dataGridViewRezult.Columns.Add("name", "Название");
            for (int i=1;i<last.Length;i++)
                dataGridViewRezult.Columns.Add("X" +(i+1), "X" + (i+1));
            dataGridViewRezult.Rows.Add("Гаусс");
            for (int i = 1; i < last.Length; i++)
                dataGridViewRezult[i,0].Value = res[res.Length-1][0][i-1];
            dataGridViewRezult.Rows.Add("Ранге Кутты");
            for (int i = 1; i < last.Length; i++)
                dataGridViewRezult[i, 1].Value = last[i];
        }
    }
}
