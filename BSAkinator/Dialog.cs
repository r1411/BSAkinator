﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSAkinator
{
    public partial class Dialog : Form
    {
        public Dialog()
        {
            InitializeComponent();
        }
        public string Nickname
        {
            get
            {
                return textBox1.Text;
            }
        }
        public string Link
        {
            get
            {
                return textBox2.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
