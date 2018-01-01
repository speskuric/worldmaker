using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldMaker.loadsave;
using WorldMaker.ui.enums;

namespace WorldMaker.ui
{
    public partial class Form1 : Form
    {
        const string WORLD_EXTENSION = "wrld";
        FileInfo currentFile = null;

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
            foreach (ToolStripButton modeButton in modeButtons) modeButton.Checked = modeButton == sender;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "World files (*." + WORLD_EXTENSION + ")|*." + WORLD_EXTENSION + "|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileInfo file = new FileInfo(openFileDialog.FileName);
                    worldView.World = LoadUtil.Load(file);
                    currentFile = file;
                    Text = file.Name;
                    worldView.Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during load", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                save(currentFile);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "World files (*." + WORLD_EXTENSION + ")|*." + WORLD_EXTENSION + "|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.DefaultExt = WORLD_EXTENSION;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo file = new FileInfo(saveFileDialog.FileName);
                save(file);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void save(FileInfo file)
        {
            try
            {
                if (SaveUtil.Save(worldView.World, file))
                {
                    currentFile = file;
                    MessageBox.Show(file.Name + " saved successfully!");
                    Text = file.Name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during save", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
