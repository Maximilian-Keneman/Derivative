using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Derivative
{
    public partial class Form1 : Form
    {
        private Function Example;
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void ExampleBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void AcceptExampleButton_Click(object sender, EventArgs e)
        {
            Example = new Function(ExampleBox.Text.ToLower(), comboBox1.Text.ToLower()[0], 10);
            FunctionLabel.Text = Example.ToString();
        }

        private void DerivativeButton_Click(object sender, EventArgs e)
        {
            DerivativeLabel.Text = Example.GetDerivative.ToString();
        }
    }
}
