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

            string yr = "";
            Func<double[], double>[] funcs = new Func<double[], double>[dataGridView1.RowCount];
            for (int yrIndex = 0; yrIndex < dataGridView1.RowCount; yrIndex++)
            {
                List<Point>[] vals = new List<Point>[dataGridView1.RowCount];
                yr = "";
                vals[yrIndex] = new List<Point>();
                for (int rowIndex = 0; rowIndex < dataGridView1.RowCount; rowIndex++)
                        if (Convert.ToString(dataGridView1[ yrIndex+1, rowIndex].Value) != "")
                        {
                            yr += Convert.ToString(dataGridView1[yrIndex + 1, rowIndex].Value) + "*S" + (yrIndex + 1) + "+";
                            vals[yrIndex].Add(new Point(yrIndex + 1, rowIndex));
                        }
                            
                if(yr!="")
                    yr =yr.Remove(yr.Length-1);
                
                for (int columnsIndex = 0; columnsIndex < dataGridView1.Rows.Count; columnsIndex++)
                    if (yrIndex != columnsIndex)
                        if (Convert.ToString(dataGridView1[columnsIndex+1, yrIndex].Value) != "")
                        {
                            vals[columnsIndex]=new List<Point>();
                            vals[columnsIndex].Add(new Point(columnsIndex + 1, yrIndex));
                            yr += "-" + Convert.ToString(dataGridView1[columnsIndex + 1, yrIndex].Value) + "*S" + (columnsIndex + 1);

                        }
                double[] ratio = new double[vals.Length];
                for (int i = 0; i < vals.Length; i++)
                {
                    ratio[0] = 0;
                    if(vals[i]!=null)
                    for(int j=0; j < vals[i].Count; j++)
                    {
                        ratio[0] += Convert.ToDouble(dataGridView1[vals[i][j].X, vals[i][j].Y].Value);
                    }
                    for(int j=1;j<vals.Length; j++)
                        if (vals[j] !=null&& vals[j].Count>0)
                        {
                            ratio[j]= -Convert.ToDouble(dataGridView1[vals[j][0].X, vals[j][0].Y].Value);
                        }


                }
                funcs[yrIndex] = Yvals =>
                {
                    double sum = 0;
                    for (int j = 0; j < ratio.Length&&j<Yvals.Length; j++)
                        sum += ratio[j] * Yvals[j];
                    return sum;
                };

                listBox1.Items.Add(yr);
            }
            

            


        }
    }
}
