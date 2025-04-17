using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mslab5
{
    internal class SLAY
    {
        double[][] matrix;

        public SLAY(DataGridView dataGridView)
        {
            if (dataGridView != null)
            {
                this.matrix = new double[dataGridView.RowCount][];


                for (int i = 0; i < dataGridView.RowCount; i++)
                {
                    matrix[i] = new double[dataGridView.ColumnCount];
                    for (int j = 0; j < dataGridView.ColumnCount; j++)
                    {
                        matrix[i][j] = Convert.ToInt32(dataGridView.Rows[i].Cells[j].Value);
                    }
                }

            }
        }
        public SLAY(double[][] _matrix)
        {
            matrix = _matrix;
        }
            public double[][][] Find()
        {
            //if(matrix.Length < matrix[0].Length-1)
            //{
            //    MessageBox.Show("Неизвестный больше чем уравнений", "Warning");
            //    return null;
            //}
            if (matrix.Length > matrix[0].Length - 1)
            {
                MessageBox.Show("Решений нет", "Warning");
                return null;

            }
            double[][][] rezult = new double[matrix[0].Length][][];
            double[][] tmpp = new double[matrix.Length][];
            for (int k = 0; k < matrix.Length; k++)
                tmpp[k] = (double[])matrix[k].Clone();
            rezult[0] = tmpp;
            int indexRez = 1;
            for (int i = 0; i < matrix[0].Length - 2; i++)
            {
                double[] tmpMat = matrix[i];
                int index = i;
                for (int j = i + 1; j < matrix.Length; j++)
                {
                    if (matrix[j][i] > tmpMat[i])
                    {
                        tmpMat = matrix[j];
                        index = j;
                    }
                }
                if (tmpMat[i] == 0)
                {
                    MessageBox.Show("Элемент на главной диагонали равен 0", "Warning");
                    return null;
                }
                matrix[index] = matrix[i];
                matrix[i] = tmpMat;
                double num = matrix[i][i];
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    matrix[i][j] = matrix[i][j] / num;
                }
                for (int j = i + 1; j < matrix.Length; j++)
                {
                    double tmp = matrix[j][i];
                    for (int k = i; k < matrix[j].Length; k++)
                    {
                        matrix[j][k] = matrix[j][k] - matrix[i][k] * tmp;
                    }
                }
                double[][] tmpRez = new double[matrix.Length][];
                for (int k = 0; k < matrix.Length; k++)
                    tmpRez[k] = (double[])matrix[k].Clone();
                rezult[i + 1] = tmpRez;
                indexRez++;
            }
            double[] Xarr = new double[matrix[0].Length - 1];
            Xarr[Xarr.Length - 1] = matrix[matrix[0].Length - 2][matrix[0].Length - 1] / matrix[matrix[0].Length - 2][matrix[0].Length - 2];
            for (int i = 2; i < matrix[0].Length; i++)
            {
                double sum = 0;
                for (int j = Xarr.Length - i + 1; j < Xarr.Length; j++)
                {
                    sum = sum + matrix[Xarr.Length - i][j] * Xarr[j];
                }
                Xarr[Xarr.Length - i] = matrix[Xarr.Length - i][Xarr.Length] - sum;
            }
            double[][] tm = new double[1][];
            tm[0] = Xarr;
            rezult[rezult.Length - 1] = tm;
            return rezult;
        }
    }
}
