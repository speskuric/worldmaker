using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldMaker.ui.enums;

namespace WorldMaker.ui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripSelectModeButton_Click(object sender, EventArgs e)
        {
            setMode(sender, MouseMode.Select);
        }

        private void toolStripDrawModeButton_Click(object sender, EventArgs e)
        {
            setMode(sender, MouseMode.FreeDraw);
        }

        private void toolStripScrollButton_Click(object sender, EventArgs e)
        {
            setMode(sender, MouseMode.View);
        }

        private void toolStripZoomButton_Click(object sender, EventArgs e)
        {
            setMode(sender, MouseMode.Zoom);
        }

        private void toolStripPointModeButton1_Click(object sender, EventArgs e)
        {
            setMode(sender, MouseMode.PointDraw);
        }

        private void setMode(object sender, MouseMode mouseMode)
        {
            ToolStripButton[] modeButtons = {
                toolStripDrawModeButton,
                toolStripSelectModeButton ,
                toolStripScrollButton,
                toolStripZoomButton ,
                toolStripPointModeButton1
            };
            worldView.Mode = mouseMode;
            foreach(ToolStripButton modeButton in modeButtons) modeButton.Checked = modeButton == sender;
        }
    }
}
