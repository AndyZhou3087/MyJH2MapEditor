using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyJHMapEditor
{
    public partial class NewMapForm : Form
    {
        public NewMapForm()
        {
            InitializeComponent();
        }

        private void NewMapForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void okbtn_Click(object sender, EventArgs e)
        {
            try
            {
                MapFrom.blockColCount = int.Parse(this.textBox1.Text);
                MapFrom.blockRowCount = int.Parse(this.textBox2.Text);
                this.Hide();
                MapFrom mapfrom = this.Owner as MapFrom;
                mapfrom.setTabelGrid();
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //阻止从键盘输入键
            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
