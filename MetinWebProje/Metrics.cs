using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetinWebProje
{
    class Metrics
    {
        //result i,0 = prediction 
        //result i,1 = real
        public static double[,] performanceMatrix(int[,] result,int classId)
        {
            double[,] matrix=new double[2, 2];
            for (int i=0;i<result.GetLength(0);i++)
            {
                if (result[i, 1] == classId && result[i, 0] == classId) matrix[0, 0]++;         //TP
                else if (result[i, 1] != classId && result[i, 0] == classId) matrix[1, 0]++;    //FN
                else if (result[i, 1] == classId && result[i, 0] != classId) matrix[0, 1]++;    //FP
                else if (result[i, 1] != classId && result[i, 0] != classId) matrix[1, 1]++;    //TN
            }
            return matrix;
        }
        public static double Fscore(double[,] matrix)
        {
            return 2 * (Precision(matrix) * Recall(matrix) / (Precision(matrix) + Recall(matrix)));
        }
        public static double Precision(double[,] matrix)
        {
            return matrix[0, 0] / (matrix[0, 0] + matrix[0, 1]);
        }
        public static double Recall(double[,] matrix)
        {
            return matrix[0, 0] / (matrix[0, 0] + matrix[1, 0]);
        }
        public static double Accuracy(int[,] result)
        {
            int correct=0;
            for (int i=0;i<result.GetLength(0);i++)
            {
                if (result[i, 0] == result[i, 1]) correct++;
            }
            return ((double)correct / result.GetLength(0)) *100;
        }
    }
}
