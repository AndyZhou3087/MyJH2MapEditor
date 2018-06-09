using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MyJHMapEditor
{
    public partial class MapFrom : Form
    {
        PictureBox cur_picbox = null;
        ArrayList cellsDatalist = new ArrayList();
        static string FILEFILTER = "xml files(*.xml)|*.xml";
        static string resPath = "./pic";
        static string imgextension = "*.png";
        int gridsize = 0;
        private System.Windows.Forms.TableLayoutPanel maptable = null;
        public static int blockRowCount;
        public static int blockColCount;

        private string boardPath = null;
        ArrayList buildPicBoxList = new ArrayList();
        ArrayList buildDatalist = new ArrayList();
        public MapFrom()
        {
            InitializeComponent();

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(36, 36);

            DirectoryInfo folder = new DirectoryInfo(resPath);

            int m = 0;
            foreach (FileInfo file in folder.GetFiles(imgextension))
            {
                string extension = file.Extension;
                string newname;
                string key;
                if (file.Name.Substring(0, 10).Equals("buildblock"))
                {
                    newname = resPath + "/" + "buildblock_" + m + extension;
                    key = "buildblock_" + m + extension;
                }
                else
                {
                    newname = resPath + "/" + "boardblock_" + m + extension;
                    key = "boardblock_" + m + extension;
                }
                if (!File.Exists(newname))
                {
                    file.MoveTo(newname);
                }
                imageList.Images.Add(key, Image.FromFile(newname));
                m++;
            }

            listView1.View = View.SmallIcon;
            listView1.SmallImageList = imageList;
            int c = imageList.Images.Count;
            for (int i = 0; i < c; i++)
            {
                ListViewItem li = new ListViewItem();
                li.ImageIndex = i;
                li.Text = imageList.Images.Keys[i];
                listView1.Items.Add(li);
            }
        }

        public void setTabelGrid()
        {
            if (maptable != null)
                this.Controls.Remove(this.maptable);

            this.maptable = new System.Windows.Forms.TableLayoutPanel();
            this.maptable.ColumnCount = blockColCount;
            this.maptable.RowCount = blockRowCount;
            this.maptable.AutoSize = true;
            this.maptable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.maptable.Margin = new System.Windows.Forms.Padding(0);
            gridsize = this.mapgridPanel.Size.Width / this.maptable.ColumnCount;
            if (gridsize > 54)
                gridsize = 54;
            int posx = this.mapgridPanel.Location.X + this.mapgridPanel.Size.Width/2 - this.maptable.ColumnCount*(gridsize)/ 2;
            int posy = this.mapgridPanel.Location.Y + this.mapgridPanel.Size.Height / 2 - this.maptable.RowCount * (gridsize) / 2;
            this.maptable.Location = new System.Drawing.Point(posx, posy);
            this.maptable.Margin = new System.Windows.Forms.Padding(0);
            this.maptable.Name = "maptable";
            this.maptable.TabIndex = 0;
            mapgridPanel.Visible = false;
            maptable.SuspendLayout();
            for (int i = 0; i < this.maptable.ColumnCount; i++)
            {
                this.maptable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, gridsize));
            }
            for (int j = 0; j < this.maptable.RowCount; j++)
            {
                this.maptable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, gridsize));
            }

            this.Controls.Add(this.maptable);

            for (int i = 0; i < this.maptable.ColumnCount; i++)
            {
                for (int j = 0; j < this.maptable.RowCount; j++)
                {
                    System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();
                    //pictureBox.BackColor = System.Drawing.Color.;
                    pictureBox.Location = new System.Drawing.Point(0, 0);
                    pictureBox.Margin = new System.Windows.Forms.Padding(0);
                    pictureBox.Name = "picbox" + (i * maptable.ColumnCount + j);
                    pictureBox.Size = new System.Drawing.Size(gridsize, gridsize);
                    pictureBox.MouseClick += new MouseEventHandler(pic_MouseClick);//PictureBox鼠标点击事件
                    //pictureBox.MouseDoubleClick += new MouseEventHandler(pic_MouseDoubleClick);
                    pictureBox.TabIndex = 1;
                    pictureBox.TabStop = false;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    maptable.Controls.Add(pictureBox, i, j);
                }
            }
            maptable.ResumeLayout();
        }

        private void setSelectPic(PictureBox pictureBox)
        {
            if (cur_picbox != null)
            {
                cur_picbox.BackColor = System.Drawing.Color.Green;
            }
            pictureBox.BackColor = System.Drawing.Color.Red;
            cur_picbox = pictureBox;

            int c = this.maptable.GetPositionFromControl(pictureBox).Column;
            int r = this.maptable.GetPositionFromControl(pictureBox).Row;
            xpostexbox.Text = (c+ 1) + "";
            ypostexbox.Text = (r + 1) + "";

            if (boardPath != null)
            {

                if (boardPath.Contains("buildblock"))
                {
                    AddBuldingPic(c, r, boardPath);
                }
                else
                {
                    pictureBox.Load(boardPath);
                }
            }
            setPicBoxCellData(pictureBox, (CellData)pictureBox.Tag);
        }

        private void AddBuldingPic(int c,int r, string bPath)
        {
            System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();
            //pictureBox.BackColor = System.Drawing.Color.;
            pictureBox.Location = new System.Drawing.Point(0, 0);
            pictureBox.Margin = new System.Windows.Forms.Padding(0);
            pictureBox.MouseClick += new MouseEventHandler(BuildingPic_MouseClick);//PictureBox鼠标点击事件
            pictureBox.TabIndex = c*100+r;
            pictureBox.TabStop = false;
            pictureBox.Location = new Point(this.maptable.Location.X + c * (gridsize), this.maptable.Location.Y+ r*(gridsize));
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.BringToFront();
            pictureBox.Load(bPath);
            pictureBox.Tag = bPath;
            float w = gridsize / 72.0f * pictureBox.Image.Width;
            float h = gridsize / 72.0f * pictureBox.Image.Height;
            pictureBox.Size = new System.Drawing.Size((int)w, (int)h);
            buildPicBoxList.Add(pictureBox);
            this.Controls.Add(pictureBox);

            maptable.SendToBack();
            //this.maptable.GetControlFromPosition()
        }
        private void BuildingPic_MouseClick(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)sender;
            if (e.Button == MouseButtons.Right)
            {
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }
                this.Controls.Remove(pictureBox);
                buildPicBoxList.Remove(pictureBox);
                if (pictureBox != null)
                {
                    pictureBox = null;
                }
            }
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)//鼠标触发的事件
        {
            System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)sender;
            if (e.Button == MouseButtons.Left)
            {
                setSelectPic(pictureBox);
                
            }
            else if (e.Button == MouseButtons.Right)
            {
                pictureBox.BackColor = System.Drawing.SystemColors.Control;
                if (cur_picbox != null && cur_picbox != pictureBox)
                    cur_picbox.BackColor = System.Drawing.Color.Green;
                clearInputContent();
                pictureBox.Tag = null;
                cur_picbox = null;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                if (pictureBox.Image != null)
                {
                    CellData celldata = (CellData)pictureBox.Tag;
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                    if (celldata != null)
                     celldata.boardblock = null;
                }
            }
        }

        //private void pic_MouseDoubleClick(object sender, MouseEventArgs e)//鼠标触发的事件
        //{
        //    System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)sender;

        //}

        private void posTypeRadioBtn_Clicked(object sender, EventArgs e)
        {
            if (checkSelectCell())
            {
                RadioButton radiobtn = (RadioButton)sender;

                int radiotag = int.Parse(radiobtn.Tag.ToString());

                if (radiotag == 0)
                {
                    this.groupBox3.Visible = false;
                    this.groupBox6.Visible = false;
                    this.panel1.Visible = false;
                }
                else
                {
                    this.groupBox3.Visible = true;
                    this.groupBox6.Visible = true;
                    this.panel1.Visible = true;
                    if (radiotag == 1)
                    {
                        this.textBox8.Enabled = false;
                    }
                    else
                    {
                        this.textBox8.Enabled = true;
                    }
                }
                (cur_picbox.Tag as CellData).postype = radiotag;
            }
        }

        private void eventTextBox_TextChanged(object sender, EventArgs e)
        {
            if (checkSelectCell())
            {
                TextBox txtbox = (TextBox)sender;
                int tag = int.Parse(txtbox.Tag.ToString());
                try
                {
                    (cur_picbox.Tag as CellData).setEventRnd(tag, int.Parse(txtbox.Text));
                }
                catch (Exception ex)
                {
                    (cur_picbox.Tag as CellData).setEventRnd(tag, 0);
                    if (txtbox.Text.Length > 0)
                        MessageBox.Show(this, "请输入整数！！"+ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        private void setPicBoxCellData(PictureBox picBox, CellData cdata)
        {
            if (cdata == null)
            {
                CellData celldata = new CellData();
                celldata.col = this.maptable.GetPositionFromControl(picBox).Column;
                celldata.row = this.maptable.GetPositionFromControl(picBox).Row;
                celldata.choiceDataA = new ChoicesData();
                celldata.choiceDataB = new ChoicesData();
                if (boardPath!=null)
                    celldata.boardblock = boardPath.Substring(boardPath.LastIndexOf("/") + 1);
                picBox.Tag = celldata;
                clearInputContent();
                this.panel1.Visible = true;
                this.groupBox3.Visible = true;
                this.groupBox6.Visible = true;
                celldata.walkable = this.checkBox1.Checked;
            }
            else
            {
                this.checkBox1.Checked = cdata.walkable;
                for (int i = 0; i < 7; i++)
                {
                    string textboxstr = "textBox" + (i+1);
                    TextBox txtbox = (TextBox)groupBox1.Controls.Find(textboxstr, false)[0];
                    txtbox.Text = cdata.getEventRnd(i) + "";
                }

                int posRadiotype = cdata.postype;
                if (posRadiotype >= 0)
                {
                    string radiontypestr = "radioButton" + (posRadiotype + 1);
                    RadioButton radiobtn = (RadioButton)groupBox2.Controls.Find(radiontypestr, false)[0];
                    radiobtn.Checked = true;
                }
                else
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        string radiontypestr = "radioButton" + i;
                        RadioButton radiobtn = (RadioButton)groupBox2.Controls.Find(radiontypestr, false)[0];
                        radiobtn.Checked = false;
                    }
                }
                if (posRadiotype == 0)
                {
                    this.panel1.Visible = false;
                    groupBox3.Visible = false;
                    groupBox6.Visible = false;
                }
                else if (posRadiotype > 0)
                {
                    this.panel1.Visible = true;
                    groupBox3.Visible = true;
                    groupBox6.Visible = true;
                    this.textBox8.Text = cdata.posnpcid;
                    this.textBox9.Text = cdata.posnpcrnd + "";
                    if (posRadiotype == 1)
                        textBox8.Enabled = false;
                    else
                        textBox8.Enabled = true;

                    for (int i = 0; i < 3; i++)
                    {
                        if (cdata.choiceDataA != null)
                        {
                            string name = "AgetResTxtbox" + i;
                            TextBox txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                            txtbox.Text = cdata.choiceDataA.getGetRes(i);

                            name = "AgetResCountTxtbox" + i;
                            txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                            txtbox.Text = cdata.choiceDataA.getGetResCount(i) + "";

                            name = "AretRd" + i;
                            RadioButton rdbtn = (RadioButton)groupBox4.Controls.Find(name, false)[0];
                            if (cdata.choiceDataA.RetType == i)
                            {
                                rdbtn.Checked = true;
                            }
                            else
                            {
                                rdbtn.Checked = false;
                            }
                        }

                        if (cdata.choiceDataB != null)
                        {
                            string name = "BgetResTxtbox" + i;
                            TextBox txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                            txtbox.Text = cdata.choiceDataB.getGetRes(i);

                            name = "BgetResCountTxtbox" + i;
                            txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                            txtbox.Text = cdata.choiceDataB.getGetResCount(i) + "";


                            name = "BretRd" + i;
                            RadioButton rdbtn = (RadioButton)groupBox5.Controls.Find(name, false)[0];
                            if (cdata.choiceDataB.RetType == i)
                            {
                                rdbtn.Checked = true;
                            }
                            else
                            {
                                rdbtn.Checked = false;
                            }
                        }
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        string name = "monsterLvTxt" + i;
                        TextBox txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                        txtbox.Text = cdata.getMonstersLv(i) + "";

                        name = "monsterQUTxt" + i;
                        txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                        txtbox.Text = cdata.getMonstersQU(i) + "";

                        name = "monsterGetResCount" + i;
                        txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                        txtbox.Text = cdata.getMonstersGetResCount(i) + "";

                        name = "monsterIdTxt" + i;
                        txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                        txtbox.Text = cdata.getMonstersId(i);

                        name = "monsterGetResId" + i;
                        txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                        txtbox.Text = cdata.getMonstersGetRes(i);
                    }


                    if (cdata.choiceDataA != null)
                    {
                        chooseContent1.Text = cdata.choiceDataA.Content;
                        AlossResTxtbox.Text = cdata.choiceDataA.LossRes;
                        AlossResCountTxtbox.Text = cdata.choiceDataA.LossResCount + "";
                        AeffectBossTxtbox.Text = cdata.choiceDataA.EffectBoss;
                    }
                    else if (cdata.choiceDataB != null)
                    {
                        chooseContent2.Text = cdata.choiceDataB.Content;


                        BlossResTxtbox.Text = cdata.choiceDataB.LossRes;
                        BlossResCountTxtbox.Text = cdata.choiceDataB.LossResCount + "";

                        BeffectBossTxtbox.Text = cdata.choiceDataB.EffectBoss;
                    }
                }

            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (checkSelectCell())
            {
                CellData cdata = (CellData)cur_picbox.Tag;
                TextBox txtBox = (sender as TextBox);
                cdata.posnpcid = txtBox.Text;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (checkSelectCell())
            {
                CellData cdata = (CellData)cur_picbox.Tag;
                TextBox txtBox = (sender as TextBox);
                try
                {
                    cdata.posnpcrnd = int.Parse(txtBox.Text);
                }
                catch (Exception ex)
                {
                    cdata.posnpcrnd = 0;
                    if (txtBox.Text.Length > 0)
                        MessageBox.Show(this, "请输入整数！！" +ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                
            }
        }

        private bool checkSelectCell()
        {
            if (cur_picbox == null)
            {
                MessageBox.Show(this, "请先选择地图块！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void clearInputContent()
        {
            this.panel1.Visible = true;
            this.groupBox3.Visible = true;
            this.groupBox6.Visible = true;
            for (int i = 1; i <= 7; i++)
            {
                string name = "textBox" + i;
                TextBox txtbox = (TextBox)groupBox1.Controls.Find(name, false)[0];
                txtbox.Text = "0";
            }
            for (int i = 1; i <= 5; i++)
            {
                string name = "radioButton" + i;
                RadioButton radiobtn = (RadioButton)groupBox2.Controls.Find(name, false)[0];
                radiobtn.Checked = false;
            }
            this.textBox8.Text = "";
            this.textBox9.Text = "0";

            for (int i = 0;i<3;i++)
            {
                string name = "AgetResTxtbox" + i;
                TextBox txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "";

                name = "AgetResCountTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "BgetResTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "";

                name = "BgetResCountTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "AretRd" + i;
                RadioButton rdbtn = (RadioButton)groupBox4.Controls.Find(name, false)[0];
                rdbtn.Checked = false;
                name = "BretRd" + i;
                rdbtn = (RadioButton)groupBox5.Controls.Find(name, false)[0];
                rdbtn.Checked = false;
            }

            for (int i=0;i<6;i++)
            {
                string name = "monsterLvTxt" + i;
                TextBox txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                txtbox.Text = "0";
                name = "monsterQUTxt" + i;
                txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                txtbox.Text = "0";


                name = "monsterGetResCount" + i;
                txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monsterIdTxt" + i;
                txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                txtbox.Text = "";

                name = "monsterGetResId" + i;
                txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                txtbox.Text = "";
            }


            chooseContent1.Text = "";
            chooseContent2.Text = "";
            AlossResTxtbox.Text = "";
            AlossResCountTxtbox.Text = "0";
            BlossResTxtbox.Text = "";
            BlossResCountTxtbox.Text = "0";
            AeffectBossTxtbox.Text = "";
            BeffectBossTxtbox.Text = "";
            this.checkBox1.Checked = false;
        }

        private void textBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //阻止从键盘输入键
            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void reset()
        {
            clearInputContent();
            for (int i = 0; i < this.maptable.ColumnCount; i++)
            {
                for (int j = 0; j < this.maptable.RowCount; j++)
                {
                    PictureBox pictureBox = (PictureBox)this.maptable.GetControlFromPosition(i, j);
                    if (pictureBox != null)
                    {
                        pictureBox.BackColor = System.Drawing.SystemColors.Control;
                        pictureBox.Tag = null;
                        cur_picbox = null;
                        if (pictureBox.Image!=null)
                        {
                            pictureBox.Image.Dispose();
                            pictureBox.Image = null;
                        }
                    }

                }
            }

            foreach(PictureBox picbox in buildPicBoxList)
            {
                if (picbox.Image != null)
                {
                    picbox.Image.Dispose();
                    picbox.Image = null;
                }
                this.Controls.Remove(picbox);
            }
            cellsDatalist.Clear();
            buildPicBoxList.Clear();
        }
        private void resetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否确认清空所有配置数据？", "提示", MessageBoxButtons.YesNo,
                     MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                this.Text = "mapeditor";
                if (cur_picbox != null)
                    reset();
            }
        }

        private void openBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = FILEFILTER;
            //openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.Text = "mapeditor" +"---"+openFileDialog.FileName;
                    if (cur_picbox != null)
                        reset();

                    loadxml(openFileDialog.FileName);
                    bindControlsData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("打开文件出错：" + ex.Message);
                }
            }
        }

        private void chooseContent1_TextChanged(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataA != null)
            {
                celldata.choiceDataA.Content = chooseContent1.Text;
            }
        }

        private void chooseContent2_TextChanged(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataB != null)
                celldata.choiceDataB.Content = chooseContent2.Text;
        }

        private void AlossResTxtbox_TextChanged(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataA != null)
                celldata.choiceDataA.LossRes = AlossResTxtbox.Text;
        }

        private void AlossResCountTxtbox_TextChanged(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataA != null)
            {
                int count = 0;
                try
                {
                    count = int.Parse(AlossResCountTxtbox.Text);
                }
                catch (Exception ex)
                {
                    if (AlossResCountTxtbox.Text.Length > 0)
                        MessageBox.Show(this, "请输入整数！！"+ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                celldata.choiceDataA.LossResCount = count;
            }
        }

        private void AgetResTxtbox_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string name = txtBox.Name;
            int index = int.Parse(name.Substring(name.Length - 1));
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataA != null)
                celldata.choiceDataA.setGetRes(index, txtBox.Text);
        }

        private void AgetResCountTxtbox_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string name = txtBox.Name;
            int index = int.Parse(name.Substring(name.Length - 1));
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataA != null)
            {
                int count = 0;
                try
                {
                    count = int.Parse(txtBox.Text);
                }
                catch (Exception ex)
                {
                    if (txtBox.Text.Length > 0)
                        MessageBox.Show(this, "请输入整数！！" +ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                celldata.choiceDataA.setGetResCount(index, count);
            }
        }

        private void AeffectBossTxtbox_TextChanged(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataA != null)
                celldata.choiceDataA.EffectBoss = AeffectBossTxtbox.Text;
        }

        private void AretRd_Clicked(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataA != null)
            {
                RadioButton rdbtn = sender as RadioButton;
                string name = rdbtn.Name;
                int rettype = int.Parse(name.Substring(name.Length - 1));
                celldata.choiceDataA.RetType = rettype;
            }
        }

        private void BgetResTxtbox_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string name = txtBox.Name;
            int index = int.Parse(name.Substring(name.Length - 1));
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataB != null)
                celldata.choiceDataB.setGetRes(index, txtBox.Text);
        }

        private void BgetResCountTxtbox_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string name = txtBox.Name;
            int index = int.Parse(name.Substring(name.Length - 1));
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataB != null)
            {
                int count = 0;
                try
                {
                    count = int.Parse(txtBox.Text);
                }
                catch (Exception ex)
                {
                    if (txtBox.Text.Length > 0)
                        MessageBox.Show(this, "请输入整数！！"+ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                celldata.choiceDataB.setGetResCount(index, count);
            }
        }

        private void BlossResTxtbox_TextChanged(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataB != null)
                celldata.choiceDataB.LossRes = BlossResTxtbox.Text;
        }

        private void BlossResCountTxtbox_TextChanged(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataB != null)
            {
                int count = 0;
                try
                {
                    count = int.Parse(BlossResCountTxtbox.Text);
                }
                catch (Exception ex)
                {
                    if (BlossResCountTxtbox.Text.Length > 0)
                        MessageBox.Show(this, "请输入整数！！"+ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                celldata.choiceDataB.LossResCount = count;
            }
        }

        private void BeffectBossTxtbox_TextChanged(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataB != null)
                celldata.choiceDataB.EffectBoss = BeffectBossTxtbox.Text;
        }

        private void BretRd_Clicked(object sender, EventArgs e)
        {
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null && celldata.choiceDataB != null)
            {
                RadioButton rdbtn = sender as RadioButton;
                string name = rdbtn.Name;
                int rettype = int.Parse(name.Substring(name.Length - 1));
                celldata.choiceDataB.RetType = rettype;
            }
        }

        private void monsterTxt_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string name = txtBox.Name;
            int index = int.Parse(name.Substring(name.Length - 1));
            CellData celldata = cur_picbox.Tag as CellData;
            if (celldata != null)
            {
                if (name.Contains("monsterIdTxt"))
                    celldata.setMonstersId(index, txtBox.Text);
                else if (name.Contains("monsterLvTxt"))
                    celldata.setMonstersLv(index, int.Parse(txtBox.Text));
                else if (name.Contains("monsterQUTxt"))
                    celldata.setMonstersQU(index, int.Parse(txtBox.Text));
                else if (name.Contains("monsterGetResId"))
                    celldata.setMonstersGetRes(index, txtBox.Text);
                else if (name.Contains("monsterGetResCount"))
                    celldata.setMonstersGetResCount(index, int.Parse(txtBox.Text));
            }
        }

        private void loadxml(string xmlfile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlfile);
            //查找<Cells>  
            XmlNode root = xmlDoc.SelectSingleNode("Cells");
            XmlElement rootele = (XmlElement)root;
            blockColCount = int.Parse(rootele.GetAttribute("cs"));
            blockRowCount = int.Parse(rootele.GetAttribute("rs"));
            //获取到所有<Cells>的子节点  
            XmlNodeList nodeList = root.ChildNodes;
            //遍历所有子节点  
            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;

                if (xe.Name.Equals("buildblocks"))
                {
                    XmlNodeList subList = xe.ChildNodes;
                    foreach (XmlNode xmlNode in subList)
                    {
                        XmlElement sxe = (XmlElement)xmlNode;
                        int sr = int.Parse(sxe.GetAttribute("r"));
                        int sc = int.Parse(sxe.GetAttribute("c"));
                        string str = (sc*100+sr) + "-" + sxe.InnerText;
                        buildDatalist.Add(str);
                    }
                }
                else if (xe.Name.Equals("Cell"))
                {
                    CellData celldata = new CellData();
                    cellsDatalist.Add(celldata);
                    celldata.col = int.Parse(xe.GetAttribute("c"));
                    celldata.row = int.Parse(xe.GetAttribute("r"));
                    celldata.postype = int.Parse(xe.GetAttribute("postype"));
                    celldata.walkable = int.Parse(xe.GetAttribute("walkable")) == 1 ? true : false;

                    XmlNodeList subList = xe.ChildNodes;
                    foreach (XmlNode xmlNode in subList)
                    {
                        XmlElement sxe = (XmlElement)xmlNode;
                        string ename = sxe.Name;


                        if (ename.Contains("event"))
                        {
                            int index = int.Parse(ename.Substring(ename.Length - 1)) - 1;
                            celldata.setEventRnd(index, int.Parse(sxe.InnerText));
                        }
                        else if (ename.Equals("boardblock"))
                        {
                            celldata.boardblock = sxe.InnerText;
                        }
                        else if (ename.Equals("posnpcid"))
                        {
                            celldata.posnpcid = sxe.InnerText;
                        }
                        else if (ename.Equals("posnpcrnd"))
                        {
                            celldata.posnpcrnd = int.Parse(sxe.InnerText);
                        }
                        else if (ename.Contains("monsteratt"))
                        {
                            int mindex = int.Parse(ename.Substring(ename.Length - 1)) - 1;
                            celldata.setMonstersId(mindex, sxe.GetAttribute("id"));
                            celldata.setMonstersQU(mindex, int.Parse(sxe.GetAttribute("qu")));
                            celldata.setMonstersLv(mindex, int.Parse(sxe.InnerText));
                        }
                        else if (ename.Contains("monsterawd"))
                        {
                            int mindex = int.Parse(ename.Substring(ename.Length - 1)) - 1;
                            celldata.setMonstersGetRes(mindex, sxe.GetAttribute("id"));
                            celldata.setMonstersGetResCount(mindex, int.Parse(sxe.InnerText));
                        }
                        else if (ename.Contains("choice"))
                        {
                            ChoicesData cdata = new ChoicesData();
                            cdata.Content = sxe.GetAttribute("cname");
                            int k = 0;
                            foreach (XmlNode cA in sxe)
                            {
                                XmlElement axe = (XmlElement)cA;
                                if (axe.Name.Equals("lossres"))
                                {
                                    cdata.LossRes = axe.GetAttribute("id");
                                    cdata.LossResCount = int.Parse(axe.InnerText);
                                }
                                else if (axe.Name.Equals("effectboss"))
                                {
                                    cdata.EffectBoss = axe.InnerText;
                                }
                                else if (axe.Name.Equals("rettype"))
                                {
                                    cdata.RetType = int.Parse(axe.InnerText);
                                }
                                else
                                {
                                    cdata.setGetRes(k, axe.Name);
                                    cdata.setGetResCount(k, int.Parse(axe.InnerText));
                                    k++;
                                }
                            }
                            if (ename.Equals("choiceA"))
                            {
                                celldata.choiceDataA = cdata;
                            }
                            else if (ename.Equals("choiceB"))
                            {
                                celldata.choiceDataB = cdata;
                            }
                        }
                    }
                }
            }
        }
        private void bindControlsData()
        {
            setTabelGrid();

            foreach (CellData celldata in cellsDatalist)
            {
                PictureBox c = (PictureBox)this.maptable.GetControlFromPosition(celldata.col, celldata.row);
                c.Tag = celldata;
                c.BackColor = System.Drawing.Color.Green;
                c.Load(resPath + "/" + celldata.boardblock);
            }
            if (cellsDatalist.Count > 0)
            {
                CellData c0 = (CellData)cellsDatalist[0];
                PictureBox c = this.maptable.GetControlFromPosition(c0.col, c0.row) as PictureBox;
                setSelectPic(c);
            }

            foreach(string buildstr in buildDatalist)
            {
                string[] cfg = buildstr.Split('-');

                int c = int.Parse(cfg[0]) / 100;
                int r = int.Parse(cfg[0]) % 100;

                string imgpath = resPath + "/" + cfg[1];
                AddBuldingPic(c, r, imgpath);
            }
        }

        private void saveXml(string xmlfile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlSM = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlSM);
            XmlElement xml = xmlDoc.CreateElement("", "Cells", "");
            xml.SetAttribute("rs", this.maptable.RowCount+ "");
            xml.SetAttribute("cs", this.maptable.ColumnCount + "");
            //追加Gen的根节点位置
            xmlDoc.AppendChild(xml);

            XmlElement bcell = xmlDoc.CreateElement("buildblocks");
            xml.AppendChild(bcell);

            foreach (PictureBox picbox in buildPicBoxList)
            {
                XmlElement bone = xmlDoc.CreateElement("b");

                int r = picbox.TabIndex % 100;
                int c = picbox.TabIndex / 100;
                bone.SetAttribute("r", r+"");
                bone.SetAttribute("c", c + "");
                string imgpth = picbox.Tag as string;
                bone.InnerText = imgpth.Substring(imgpth.LastIndexOf("/") + 1);
                bcell.AppendChild(bone);
            }

            for (int i = 0; i < this.maptable.ColumnCount; i++)
            {
                for (int j = 0; j < this.maptable.RowCount; j++)
                {
                    CellData celldata = maptable.GetControlFromPosition(i, j).Tag as CellData;
                    if (celldata != null /*&& celldata.postype >= 0*/)
                    {
                        XmlElement cell = xmlDoc.CreateElement("Cell");
                        //为<Cell>节点的属性
                        cell.SetAttribute("c", i + "");
                        cell.SetAttribute("r", j + "");
                        cell.SetAttribute("postype", celldata.postype + "");
                        int canwalk = celldata.walkable ? 1 : 0;
                        cell.SetAttribute("walkable", canwalk + "");
                        xml.AppendChild(cell);
                        XmlElement xmlele;

                        if (celldata.boardblock != null)
                        {
                            xmlele = xmlDoc.CreateElement("boardblock");
                            xmlele.InnerText = celldata.boardblock;
                            cell.AppendChild(xmlele);
                        }
                        if (celldata.postype > 0)
                        {
                            for (int m = 1; m <= 7; m++)
                            {
                                string eventname = "event" + m;
                                xmlele = xmlDoc.CreateElement(eventname);
                                xmlele.InnerText = celldata.getEventRnd(m - 1) + "";
                                cell.AppendChild(xmlele);
                            }
                            xmlele = xmlDoc.CreateElement("posnpcid");
                            xmlele.InnerText = celldata.posnpcid;
                            cell.AppendChild(xmlele);
                            xmlele = xmlDoc.CreateElement("posnpcrnd");
                            xmlele.InnerText = celldata.posnpcrnd + "";
                            cell.AppendChild(xmlele);

                            for (int n = 1;n <= 6;n++)
                            {
                                string msid = celldata.getMonstersId(n - 1);
                                if (msid != null && msid.Length > 0)
                                {
                                    string mname = "monsteratt" + n;
                                    xmlele = xmlDoc.CreateElement(mname);
                                    xmlele.InnerText = celldata.getMonstersLv(n - 1) + "";
                                    xmlele.SetAttribute("id", msid);
                                    xmlele.SetAttribute("qu", celldata.getMonstersQU(n - 1) + "");
                                    cell.AppendChild(xmlele);
                                }
                            }

                            for (int k = 1; k <= 6; k++)
                            {
                                string msid = celldata.getMonstersGetRes(k - 1);
                                int count = celldata.getMonstersGetResCount(k - 1);
                                if (msid != null && msid.Length > 0 && count > 0)
                                {
                                    string mname = "monsterawd" + k;
                                    xmlele = xmlDoc.CreateElement(mname);
                                    xmlele.InnerText = count + "";
                                    xmlele.SetAttribute("id", msid);
                                    cell.AppendChild(xmlele);
                                }
                            }

                            for (int k = 0; k < 2; k++)
                            {
                                ChoicesData cdata;
                                string cstring;
                                if (k == 0)
                                {
                                    cdata = celldata.choiceDataA;
                                    cstring = "choiceA";
                                }
                                else
                                {
                                    cdata = celldata.choiceDataB;
                                    cstring = "choiceB";
                                }
                                if (cdata.Content != null)
                                {
                                    xmlele = xmlDoc.CreateElement(cstring);
                                    xmlele.SetAttribute("cname", cdata.Content);


                                    for (int n = 0; n < 3; n++)
                                    {
                                        string resid = cdata.getGetRes(n);
                                        int rescount = cdata.getGetResCount(n);
                                        if (resid != null && resid.Length > 0 && rescount > 0)
                                        {
                                            XmlElement xe = xmlDoc.CreateElement(resid);
                                            xe.InnerText = rescount + "";
                                            xmlele.AppendChild(xe);
                                        }
                                    }
                                    if (cdata.LossRes != null && cdata.LossRes.Length > 0 && cdata.LossResCount > 0)
                                    {
                                        XmlElement xet = xmlDoc.CreateElement("lossres");
                                        xet.SetAttribute("id", cdata.LossRes);
                                        xet.InnerText = cdata.LossResCount + "";
                                        xmlele.AppendChild(xet);
                                    }
                                    if (cdata.EffectBoss!= null && cdata.EffectBoss.Length > 0)
                                    {
                                        XmlElement xet = xmlDoc.CreateElement("effectboss");
                                        xet.InnerText = cdata.EffectBoss;
                                        xmlele.AppendChild(xet);
                                    }

                                    XmlElement xetp = xmlDoc.CreateElement("rettype");
                                    xetp.InnerText = cdata.RetType + "";
                                    xmlele.AppendChild(xetp);

                                    cell.AppendChild(xmlele);
                                }
                            }

                        }
                    }
                }
            }
            xmlDoc.Save(xmlfile);
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FILEFILTER;
            saveFileDialog.InitialDirectory = "C:\\";
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                saveXml(saveFileDialog.FileName.ToString());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cur_picbox != null)
            {
                if (MessageBox.Show("请确保配置已保存！！确定退出吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void mapFrom_Shown(object sender, EventArgs e)
        {
            Form newMapForm = new NewMapForm();
            newMapForm.ShowDialog(this);
        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices != null && listView1.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = listView1.SelectedIndices;
                string filename = resPath + "/" + listView1.Items[c[0]].Text;
                //cur_picbox.Load(filename);
                boardPath = filename;
            }
            else
            {
                boardPath = null;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData cellData = cur_picbox.Tag as CellData;
                cellData.walkable = this.checkBox1.Checked;
            }
        }
    }
}
