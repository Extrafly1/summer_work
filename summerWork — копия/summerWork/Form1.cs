using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NCalc;
using NCalc.Domain;
using MPI;
using System.Diagnostics;

namespace summerWork
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private double f1(double x)
        {
            string expression = textBox3.Text;
            string per = textBox4.Text;
            if (per.Length != 1) per = "x";

            string userInput = x.ToString().Replace(",", ".");
            expression = expression.Replace(per, userInput);

            double doubleResult = 0;
            try
            {
                Expression expressionEvaluator = new Expression(expression);

                object result = expressionEvaluator.Evaluate();
                doubleResult = Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return doubleResult;
        }

        public double Integrate(double a, double b, double step)
        {
            int n = (int)((b - a) / step);
            if (n % 2 != 0) n++;
            double h = (b - a) / n;

            double sum = f1(a) + f1(b);

            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                if (i % 2 != 0)
                {
                    sum += 4 * f1(x);
                }
                else
                {
                    sum += 2 * f1(x);
                }
            }

            sum = sum * h / 3.0;

            double target = 1;
            textBox5.Text = textBox5.Text.Replace(".", ",");
            try
            {
                if (textBox5.Text != "") target = Convert.ToDouble(textBox5.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return sum - target;
        }

        public double ParallelIntegrate(double a, double b, double step)
        {
            double localSum = 0.0, totalSum = 0.0;

            int n = (int)((b - a) / step);
            if (n % 2 != 0) n++;
            double h = (b - a) / n;

            double local_a = a + Program.rank * (b - a) / Program.size;
            double local_b = a + (Program.rank + 1) * (b - a) / Program.size;

            localSum = f1(local_a) + f1(local_b);

            for (int i = 1; i < n / Program.size; i++)
            {
                double x = local_a + i * h;
                if (i % 2 != 0)
                {
                    localSum += 4 * f1(x);
                }
                else
                {
                    localSum += 2 * f1(x);
                }
            }

            localSum = localSum * h / 3.0;
            totalSum = Program.comm.Reduce(localSum, Operation<double>.Add, 0);

            if (Program.rank == 0)
            {
                double target = 1;
                textBox5.Text = textBox5.Text.Replace(".", ",");
                try
                {
                    if (textBox5.Text != "") target = Convert.ToDouble(textBox5.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                totalSum -= target;
            }

            return totalSum;
        }

        public double Steffensen(double initialGuess, double tolerance, double a, double step)
        {
            double x0 = initialGuess;
            double x1 = initialGuess, x2, fun, integ;
            do
            {
                x0 = x1;
                fun = f1(x0);
                integ = ParallelIntegrate(a, x0, step);
                x2 = x0 - integ / fun;
                x1 = x2 - ParallelIntegrate(a, x2, step) / f1(x2);
                textBox9.AppendText(x0 + "     \n");
            }
            while (Math.Abs(x1 - x0) > tolerance);
            return x0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch;
            stopwatch = new Stopwatch();
            stopwatch.Start();
            textBox9.Text = "";
            double a = 0;
            double initialGuess = 0;
            double tolerance = 0.0001;
            double step = 0.1;
            textBox6.Text = textBox6.Text.Replace(".", ",");
            textBox2.Text = textBox2.Text.Replace(".", ",");
            textBox1.Text = textBox1.Text.Replace(".", ",");
            try
            {
                if (textBox6.Text != "") tolerance = Convert.ToDouble(textBox6.Text);
                if (textBox2.Text != "") a = Convert.ToDouble(textBox2.Text);
                if (textBox1.Text != "") initialGuess = Convert.ToDouble(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (tolerance <= 0) tolerance = 0.0001;
            double doubleResult = Steffensen(initialGuess, tolerance, a, step);
            textBox7.Text = "" + doubleResult;

            stopwatch.Stop();
            textBox8.Text = stopwatch.ElapsedMilliseconds.ToString() + " миллисекунд";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Arithmetic operators: +, -, *, /, %, ++, -
Logical operators: &&, ||, !
Bitwise operators: &, |, ^, ~, <<, >>
Comparison operators: ==, !=, >, <, >=, <=
Trigonometric functions: Sin, Cos, Tan, Asin, Acos, Atan
Exponential and logarithmic functions: Exp, Log, Log10, Sqrt
Absolute value function: Abs
Minimum and maximum functions: Min, Max");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
