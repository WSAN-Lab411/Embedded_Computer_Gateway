﻿using System.Drawing;
using System.Windows.Forms;

namespace Emboard
{
    public partial class Emboard : Form
    {
        public Emboard()
        {
            InitializeComponent();
            btSend.Enabled = false;
            cbMalenh.Enabled = false;
            cbnode.Enabled = false;
            btDisconnect.Enabled = false;
            btexit.Enabled = false;
            pnGeneral.Visible = true;
            pnNode.Visible = false;
            pnGeneral.Location = new Point(0, 0);
            panel2.Visible = true;
            panel2.Controls.Add(pnGeneral);
        }
    }
}