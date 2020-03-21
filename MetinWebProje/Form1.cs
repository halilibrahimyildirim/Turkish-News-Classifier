using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace MetinWebProje
{
    public partial class Form1 : Form
    {
        bool readTest=false, readTrain=false;
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            dataGridView1.Rows.Add("Accuracy");
            dataGridView1.Rows.Add("Time");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowDialog();
            if (fd.SelectedPath == "") readTrain = false;
            else readTrain = true;
            TrainPath.Text = fd.SelectedPath;
            if (readTest && readTrain) startButton.Enabled = true;
            else startButton.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowDialog();
            if (fd.SelectedPath == "") readTest = false;
            else readTest = true;
            TestPath.Text = fd.SelectedPath;
            if (readTest && readTrain) startButton.Enabled = true;
            else startButton.Enabled = false;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && textBox1.Visible)
            {
                MessageBox.Show("Please enter value of K", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(!(checkBox1.Checked || checkBox2.Checked || checkBox3.Checked))
            {
                MessageBox.Show("Please select a classification method", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Console.Clear();
                dataGridView2.Rows.Clear();
                dataGridView3.Rows.Clear();
                dataGridView4.Rows.Clear();
                dataGridView2.Columns.Clear();
                dataGridView3.Columns.Clear();
                dataGridView4.Columns.Clear();
                dataGridView2.Columns.Add("whitespace", " ");
                dataGridView3.Columns.Add("whitespace", " ");
                dataGridView4.Columns.Add("whitespace", " ");
                Stopwatch timer = new Stopwatch();
                Tokenizer train = new Tokenizer(stopWordsPath.Text);
                Tokenizer test = new Tokenizer(stopWordsPath.Text);
                string[] directories = Directory.GetDirectories(TrainPath.Text);
                string[] filePaths;
                int classIndex = 0;
                Console.WriteLine("Tokenizing the train files");
                foreach (string directory in directories)
                {
                    dataGridView2.Columns.Add(Path.GetFileName(directory), Path.GetFileName(directory));
                    dataGridView3.Columns.Add(Path.GetFileName(directory), Path.GetFileName(directory));
                    dataGridView4.Columns.Add(Path.GetFileName(directory), Path.GetFileName(directory));
                    filePaths = Directory.GetFiles(directory);
                    foreach (string filePath in filePaths)
                    {
                        train.tokenize(filePath, classIndex);
                    }
                    classIndex++;
                }
                Console.WriteLine("Finished");
                dataGridView1[1, 0].Value = "";
                dataGridView1[1, 1].Value = "";
                dataGridView1[2, 0].Value = "";
                dataGridView1[2, 1].Value = "";
                dataGridView1[3, 0].Value = "";
                dataGridView1[3, 1].Value = "";
                dataGridView2.Rows.Add("Precision");
                dataGridView2.Rows.Add("Recall");
                dataGridView2.Rows.Add("Fscore");
                dataGridView3.Rows.Add("Precision");
                dataGridView3.Rows.Add("Recall");
                dataGridView3.Rows.Add("Fscore");
                dataGridView4.Rows.Add("Precision");
                dataGridView4.Rows.Add("Recall");
                dataGridView4.Rows.Add("Fscore");
                classIndex = 0;
                directories = Directory.GetDirectories(TestPath.Text);
                Console.WriteLine("Tokenizing the test files");
                foreach (string directory in directories)
                {
                    filePaths = Directory.GetFiles(directory);
                    foreach (string filePath in filePaths)
                    {
                        test.tokenize(filePath, classIndex);
                    }
                    classIndex++;
                }
                Console.WriteLine("Finished");
                int[,] result=new int[test.DocumentWordFreq.Count,2];
                int classCount = train.DocumentClassIndex[train.DocumentClassIndex.Count - 1] + 1;
                
                if (checkBox1.Checked)
                {//naive
                    Console.WriteLine("Classification started, using Multinomial Naive Bayes");
                    timer.Reset();
                    timer.Start();
                    result=Classification.Bayes(train, test);
                    timer.Stop();
                    dataGridView1[1, 0].Value=Metrics.Accuracy(result);
                    dataGridView1[1, 1].Value = (double)timer.ElapsedMilliseconds / 1000;
                    for(int i=0;i< classCount; i++)
                    {
                        dataGridView2[i+1, 0].Value = Metrics.Precision(Metrics.performanceMatrix(result, i));
                        dataGridView2[i+1, 1].Value = Metrics.Recall(Metrics.performanceMatrix(result, i));
                        dataGridView2[i+1, 2].Value = Metrics.Fscore(Metrics.performanceMatrix(result, i));
                    }
                    Console.WriteLine("Finished");
                }
                if (checkBox2.Checked)
                {//knn
                    Console.WriteLine("Classification started, using kNN with "+comboBox1.SelectedItem.ToString()+" and k equals to "+ int.Parse(textBox1.Text));
                    timer.Reset();
                    timer.Start();
                    result=Classification.kNN(int.Parse(textBox1.Text), train, test, comboBox1.SelectedIndex);
                    timer.Stop();
                    dataGridView1[2, 0].Value = Metrics.Accuracy(result);
                    dataGridView1[2, 1].Value = (double)timer.ElapsedMilliseconds / 1000;
                    for (int i = 0; i < classCount; i++)
                    {
                        dataGridView3[i + 1, 0].Value = Metrics.Precision(Metrics.performanceMatrix(result, i));
                        dataGridView3[i + 1, 1].Value = Metrics.Recall(Metrics.performanceMatrix(result, i));
                        dataGridView3[i + 1, 2].Value = Metrics.Fscore(Metrics.performanceMatrix(result, i));
                    }
                    Console.WriteLine("Finished");
                }
                if (checkBox3.Checked)
                {//rocchio
                    Console.WriteLine("Classification started, using Rocchio with " + comboBox1.SelectedItem.ToString());
                    timer.Reset();
                    timer.Start();
                    result = Classification.Rocchio(train, test, comboBox1.SelectedIndex);
                    timer.Stop();
                    dataGridView1[3, 0].Value = Metrics.Accuracy(result);
                    dataGridView1[3, 1].Value = (double)timer.ElapsedMilliseconds / 1000;
                    for (int i = 0; i < classCount; i++)
                    {
                        dataGridView4[i + 1, 0].Value = Metrics.Precision(Metrics.performanceMatrix(result, i));
                        dataGridView4[i + 1, 1].Value = Metrics.Recall(Metrics.performanceMatrix(result, i));
                        dataGridView4[i + 1, 2].Value = Metrics.Fscore(Metrics.performanceMatrix(result, i));
                    }
                    Console.WriteLine("Finished");
                }
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            stopWordsPath.Text = fd.FileName;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(!int.TryParse(textBox1.Text,out int result) && textBox1.Text!="")
            {
                MessageBox.Show("Value of K only can be an integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = "";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(!checkBox2.Checked)
            {
                comboBox1.Visible = !comboBox1.Visible;
                label4.Visible = !label4.Visible;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex==2)
            {
                MessageBox.Show("If you use pearson on kNN it will take a bit time to compute.(more than minute probably)","Beware",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(!checkBox3.Checked)
            {
                comboBox1.Visible = !comboBox1.Visible;
                label4.Visible = !label4.Visible;
            }
            label6.Visible = !label6.Visible;
            textBox1.Visible = !textBox1.Visible;
        }
    }
}
