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
        bool isstart = false;
        int editwalkable = -1;

        public static int monstormincount = 0;
        public static int monstormaxcount = 0;

        private MonstorRndProper[] monstorRndProper = new MonstorRndProper[6];

        private MonstorAwardRes monstorAwardRes = null;
        public MapFrom()
        {
            InitializeComponent();

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(60, 60);

            int boardcount = 0;
            int buildcount = 0;
            try
            {
                string cfgstr = readCfg();
                string[] cfg = cfgstr.Split(',');
                boardcount = int.Parse(cfg[0]);
                buildcount = int.Parse(cfg[1]);
            }catch(Exception ex){

            }

            DirectoryInfo folder = new DirectoryInfo(resPath);

            foreach (FileInfo file in folder.GetFiles(imgextension))
            {
                string extension = file.Extension;
                string newname = resPath + "/" + file.Name;
                string key = file.Name;
                if (file.Name.Contains("buildblock"))
                {
                    if (!file.Name.Contains("_"))
                    {
                        buildcount++;
                        newname = resPath + "/" + "buildblock_" + buildcount + extension;
                        file.MoveTo(newname);
                        key = "buildblock_" + buildcount + extension;
                    }
                }
                else
                {
                    if (!file.Name.Contains("_"))
                    {
                        boardcount++;
                        newname = resPath + "/" + "boardblock_" + boardcount + extension;
                        file.MoveTo(newname);
                        key = "boardblock_" + boardcount + extension;
                    }
                }
                imageList.Images.Add(key, Image.FromFile(newname));
            }
            saveCfg(boardcount +","+ buildcount);

            listView1.View = View.LargeIcon;
            listView1.LargeImageList = imageList;
            int c = imageList.Images.Count;
           
            for (int i = 0; i < c; i++)
            {
                ListViewItem li = new ListViewItem();
                li.ImageIndex = i;
                li.Text = imageList.Images.Keys[i];
                listView1.Items.Add(li);
            }

            initMonstorData();
        }

        private string readCfg()
        {
            StreamReader sr = new StreamReader("./pic/cfg.txt", Encoding.Default);
            string line = sr.ReadLine();
            sr.Close();
            return line;
        }
        private void saveCfg(string content)
        {
            FileStream fs = new FileStream("./pic/cfg.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(content);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        private void initMonstorData()
        {
            for (int i = 0; i < 6; i++)
            {
                monstorRndProper[i] = new MonstorRndProper();
            }
            monstorAwardRes = new MonstorAwardRes();
            monstormincount = 0;
            monstormaxcount = 0;
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

            this.maptable.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.maptable, true, null);
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
            //maptable.SuspendLayout();
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
                    pictureBox.MouseEnter += new System.EventHandler(pic_MouseEnter);
                    pictureBox.Click += new System.EventHandler(pic_Click);

                    pictureBox.TabIndex = 1;
                    pictureBox.TabStop = false;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    maptable.Controls.Add(pictureBox, i, j);

                    Label lbl = new System.Windows.Forms.Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft YaHei",7.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                    lbl.BackColor = Color.Transparent;
                    lbl.Name = "poslbl";
                    lbl.TabIndex = 1;
                    lbl.Text = "";
                    lbl.Click += new System.EventHandler(pic_Click);
                    lbl.Tag = pictureBox;
                    pictureBox.Controls.Add(lbl);
                    lbl.Parent = pictureBox;

                    Label lbl2 = new System.Windows.Forms.Label();
                    lbl2.AutoSize = true;
                    lbl2.Font = new System.Drawing.Font("Microsoft YaHei", 15.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                    lbl2.BackColor = Color.Transparent;
                    lbl2.Name = "poslbl2";
                    lbl2.TabIndex = 2;
                    lbl2.ForeColor = Color.Red;
                    lbl2.Text = "";
                    lbl2.Location = new System.Drawing.Point(0, pictureBox.Size.Height - lbl2.Size.Height);
                    lbl2.Click += new System.EventHandler(pic_Click);
                    lbl2.Tag = pictureBox;
                    pictureBox.Controls.Add(lbl2);
                    lbl2.Parent = pictureBox;
                }
            }
            //maptable.ResumeLayout();
        }

        private void setSelectPic(PictureBox pictureBox)
        {

            if (cur_picbox != null)
            {
                cur_picbox.BorderStyle = BorderStyle.None;
                //cur_picbox.Refresh();
            }

            //pictureBox.BackColor = System.Drawing.Color.Red;
            pictureBox.BorderStyle = BorderStyle.Fixed3D;
            //pictureBox.Refresh();
            cur_picbox = null;
            clearInputContent();
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
            pictureBox.Location = new Point(this.maptable.Location.X + c * (gridsize) + (c+1)*1, this.maptable.Location.Y + r * (gridsize) + (r+1)*1);
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
                boardPath = null;
            }
        }

        private void pic_Click(object sender, EventArgs e)//鼠标触发的事件
        {
            System.Windows.Forms.PictureBox pictureBox;
            try
            {
                pictureBox= (System.Windows.Forms.PictureBox)sender;
            }
            catch(Exception ex)
            {
                Label lbl = (System.Windows.Forms.Label)sender;
                pictureBox = (System.Windows.Forms.PictureBox)lbl.Tag;

            }
            setSelectPic(pictureBox);

            if (editwalkable == -1)
            {
                if (boardPath != null && boardPath.Contains("boardblock"))
                {
                    if (isstart)
                    {
                        isstart = false;
                        boardPath = null;
                    }
                    else
                        isstart = true;
                }
            }
            else
            {
                if (editwalkable == 0)
                {
                    editwalkable = 1;
                    CellData cdata = pictureBox.Tag as CellData;
                    if (cdata != null)
                        cdata.walkable = true;
                    picChageGray(pictureBox);
                }
                else if (editwalkable == 1)
                {
                    editwalkable = -1;
                }
            }
        }

        private void pic_MouseEnter(object sender, System.EventArgs e)
        {
            PictureBox picbox = (PictureBox)sender;
            if (editwalkable == -1)
            {
                if (boardPath != null && boardPath.Contains("boardblock") && isstart)
                {
                    setSelectPic(picbox);
                    //picbox.Load(boardPath);
                }
            }
            else if (editwalkable == 1)
            {
                setSelectPic(picbox);
                CellData cdata = picbox.Tag as CellData;
                if (cdata != null)
                    cdata.walkable = true;
                picChageGray(picbox);
            }
         
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)//鼠标触发的事件
        {
            System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)sender;
            /*if (e.Button == MouseButtons.Left)
            {
                setSelectPic(pictureBox);
                
            }
            else */if (e.Button == MouseButtons.Right)
            {
                //pictureBox.BackColor = System.Drawing.SystemColors.Control;
                if (cur_picbox != null)
                {
                    //cur_picbox.BackColor = System.Drawing.Color.Green;
                    cur_picbox.BorderStyle = BorderStyle.None;
                    pictureBox.Controls.Find("poslbl", false)[0].Text = "";
                    // pictureBox.Refresh();
                }
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
            if (cur_picbox != null)
            {
                RadioButton radiobtn = (RadioButton)sender;

                int radiotag = int.Parse(radiobtn.Tag.ToString());

                if (radiotag == 0)
                {
                    this.groupBox3.Visible = false;
                    this.groupBox6.Visible = false;
                    this.panel1.Visible = false;
                }
                else if (radiotag == 5)
                {
                    this.panel1.Visible = false;
                    this.groupBox3.Visible = false;
                    this.groupBox6.Visible = true;
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
                CellData cdata = cur_picbox.Tag as CellData;
                if (cdata != null)
                    cdata.postype = radiotag;

                cur_picbox.Controls.Find("poslbl", false)[0].Text = radiobtn.Text;
            }
        }

        private void eventTextBox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                TextBox txtbox = (TextBox)sender;
                int tag = int.Parse(txtbox.Tag.ToString());
                CellData cdata = (CellData)cur_picbox.Tag;
                if (cdata != null)
                {
                    try
                    {
                        int val = int.Parse(txtbox.Text);
                        cdata.setEventRnd(tag, int.Parse(txtbox.Text));
                    }
                    catch (Exception ex)
                    {

                        cdata.setEventRnd(tag, 0);
                        if (txtbox.Text.Length > 0)
                            MessageBox.Show(this, "请输入整数！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                try
                {
                    bool isfind = false;
                    for (int i = 1; i <= 7; i++)
                    {
                        string tname = "textBox" + i;
                        int r = int.Parse(groupBox1.Controls.Find(tname, false)[0].Text);
                        if (r > 0)
                        {
                            isfind = true;
                            break;
                        }

                    }
                    if (isfind)
                    {
                        cur_picbox.Controls.Find("poslbl2", false)[0].Text = "*";
                    }
                    else
                    {
                        cur_picbox.Controls.Find("poslbl2", false)[0].Text = "";
                    }
                }catch (Exception ex)
                {

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
                if (boardPath!=null && boardPath.Contains("boardblock"))
                    celldata.boardblock = boardPath.Substring(boardPath.LastIndexOf("/") + 1);
                picBox.Tag = celldata;
                this.panel1.Visible = true;
                this.groupBox3.Visible = true;
                this.groupBox6.Visible = true;
                this.checkBox1.Checked = false;
            }
            else
            {
                if (boardPath != null && boardPath.Contains("boardblock"))
                {
                    cdata.boardblock = boardPath.Substring(boardPath.LastIndexOf("/") + 1);
                    if (cdata.walkable)
                        picChageGray(picBox);
                }
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
                    for (int i = 1; i <= 6; i++)
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

                    if (cdata.choiceDataA == null)
                        cdata.choiceDataA = new ChoicesData();
                    if (cdata.choiceDataB == null)
                        cdata.choiceDataB = new ChoicesData();
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

                            name = "AgetResQUTxtbox" + i;
                            txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                            txtbox.Text = cdata.choiceDataA.getGetResQU(i) + "";

                            name = "AgetResRndTxtbox" + i;
                            txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                            txtbox.Text = cdata.choiceDataA.getGetResRnd(i) + "";

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


                            name = "BgetResQUTxtbox" + i;
                            txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                            txtbox.Text = cdata.choiceDataB.getGetResQU(i) + "";

                            name = "BgetResRndTxtbox" + i;
                            txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                            txtbox.Text = cdata.choiceDataB.getGetResRnd(i) + "";

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

                        name = "monsterGetResQU" + i;
                        txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                        txtbox.Text = cdata.getMonstersGetResQU(i) + "";

                        name = "monsterGetResRnd" + i;
                        txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                        txtbox.Text = cdata.getMonstersGetResRnd(i) + "";

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
                    else
                    {
                        chooseContent1.Text = "";
                        AlossResTxtbox.Text = "";
                        AlossResCountTxtbox.Text = "0";
                        AeffectBossTxtbox.Text = "";
                    }
                    if (cdata.choiceDataB != null)
                    {
                        chooseContent2.Text = cdata.choiceDataB.Content; 
                        BlossResTxtbox.Text = cdata.choiceDataB.LossRes;
                        BlossResCountTxtbox.Text = cdata.choiceDataB.LossResCount + "";
                        BeffectBossTxtbox.Text = cdata.choiceDataB.EffectBoss;
                    }
                    else
                    {
                        chooseContent2.Text = "";
                        BlossResTxtbox.Text = "";
                        BlossResCountTxtbox.Text = "0";
                        BeffectBossTxtbox.Text = "";
                    }
                }
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData cdata = (CellData)cur_picbox.Tag;
                if (cdata != null)
                {
                    TextBox txtBox = (sender as TextBox);
                    cdata.posnpcid = txtBox.Text;
                }
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData cdata = (CellData)cur_picbox.Tag;
                TextBox txtBox = (sender as TextBox);
                if (cdata != null)
                {
                    try
                    {
                        cdata.posnpcrnd = int.Parse(txtBox.Text);
                    }
                    catch (Exception ex)
                    {
                        cdata.posnpcrnd = 0;
                        if (txtBox.Text.Length > 0)
                            MessageBox.Show(this, "请输入整数！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
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
            for (int i = 1; i <= 6; i++)
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

                name = "AgetResQUTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "AgetResRndTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "100";
                
                name = "BgetResTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "";

                name = "BgetResCountTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "BgetResQUTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "BgetResRndTxtbox" + i;
                txtbox = (TextBox)groupBox3.Controls.Find(name, false)[0];
                txtbox.Text = "100";

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

                name = "monsterGetResQU" + i;
                txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monsterGetResRnd" + i;
                txtbox = (TextBox)groupBox6.Controls.Find(name, false)[0];
                txtbox.Text = "100";

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

        }

        private void clearMonstorProperData()
        {

            for (int i = 0; i < 6; i++)
            {
                string name = "monstoridtxt" + i;
                TextBox txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "";

                name = "monstorlvtxt" + i + "_0";
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monstorlvtxt" + i + "_1";
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monstorqutxt" + i + "_0";
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monstorqutxt" + i + "_1";
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monstorrndtxt" + i;
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monstorawardid" + i;
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "";

                name = "monstorawardcount" + i;
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monstorawardqu" + i;
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "0";

                name = "monstorawardrnd" + i;
                txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                txtbox.Text = "100";
            }

            monstorcount0.Text = "0";
            monstorcount1.Text = "0";
            initMonstorData();
        }

        private void textBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //阻止从键盘输入键
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }

        private void reset()
        {
            clearInputContent();
            clearMonstorProperData();
            for (int i = 0; i < this.maptable.ColumnCount; i++)
            {
                for (int j = 0; j < this.maptable.RowCount; j++)
                {
                    PictureBox pictureBox = (PictureBox)this.maptable.GetControlFromPosition(i, j);
                    if (pictureBox != null)
                    {
                        //pictureBox.BackColor = System.Drawing.SystemColors.Control;
                        pictureBox.BorderStyle = BorderStyle.None;
                        pictureBox.Controls.Find("poslbl", false)[0].Text = "";
                        pictureBox.Controls.Find("poslbl2", false)[0].Text = "";
                        // pictureBox.Refresh();
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
            boardPath = null;
            isstart = false;
            editwalkable = -1;
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
                //try
                {
                    this.Text = "mapeditor" +"---"+openFileDialog.FileName;
                    if (cur_picbox != null)
                        reset();

                    loadxml(openFileDialog.FileName);
                    bindControlsData();
                }
                //catch (Exception ex)
                //{
                //    MessageBox.Show("打开文件出错：" + ex.Message);
                //}
            }
        }

        private void chooseContent1_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataA != null)
                {
                    celldata.choiceDataA.Content = chooseContent1.Text;
                }
            }
        }

        private void chooseContent2_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataB != null)
                    celldata.choiceDataB.Content = chooseContent2.Text;
            }
        }

        private void AlossResTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataA != null)
                    celldata.choiceDataA.LossRes = AlossResTxtbox.Text;
            }
        }

        private void AlossResCountTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
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
                            MessageBox.Show(this, "请输入整数！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    celldata.choiceDataA.LossResCount = count;
                }
            }
        }

        private void AgetResTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                TextBox txtBox = sender as TextBox;
                string name = txtBox.Name;
                int index = int.Parse(name.Substring(name.Length - 1));
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataA != null)
                    celldata.choiceDataA.setGetRes(index, txtBox.Text);
            }
        }

        private void AgetResCountTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
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
                            MessageBox.Show(this, "请输入整数！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    celldata.choiceDataA.setGetResCount(index, count);
                }
            }
        }

        private void AeffectBossTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataA != null)
                    celldata.choiceDataA.EffectBoss = AeffectBossTxtbox.Text;
            }
        }

        private void AretRd_Clicked(object sender, EventArgs e)
        {
            if (cur_picbox != null)
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
        }

        private void BgetResTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                TextBox txtBox = sender as TextBox;
                string name = txtBox.Name;
                int index = int.Parse(name.Substring(name.Length - 1));
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataB != null)
                    celldata.choiceDataB.setGetRes(index, txtBox.Text);
            }
        }

        private void BgetResCountTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
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
                            MessageBox.Show(this, "请输入整数！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    celldata.choiceDataB.setGetResCount(index, count);
                }
            }
        }

        private void BlossResTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataB != null)
                    celldata.choiceDataB.LossRes = BlossResTxtbox.Text;
            }
        }

        private void BlossResCountTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
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
                            MessageBox.Show(this, "请输入整数！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    celldata.choiceDataB.LossResCount = count;
                }
            }
        }

        private void BeffectBossTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataB != null)
                    celldata.choiceDataB.EffectBoss = BeffectBossTxtbox.Text;
            }
        }

        private void BretRd_Clicked(object sender, EventArgs e)
        {
            if (cur_picbox != null)
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
        }

        private void monsterTxt_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
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
                    else if (name.Contains("monsterGetResQU"))
                        celldata.setMonstersGetResQU(index, int.Parse(txtBox.Text));
                    else if (name.Contains("monsterGetResRnd"))
                        celldata.setMonstersGetResRnd(index, float.Parse(txtBox.Text));
                }
            }
        }

        private void loadxml(string xmlfile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlfile);
            XmlNode rootele = xmlDoc.SelectSingleNode("cfg");
            //查找<Monstors>  
            XmlElement msele = (XmlElement)rootele.SelectSingleNode("Monstors");

            if (msele != null)
            {
                if (msele.HasAttribute("minc") && msele.HasAttribute("maxc"))
                {
                    monstormincount = int.Parse(msele.GetAttribute("minc"));
                    monstormaxcount = int.Parse(msele.GetAttribute("maxc"));
                }

                int c1 = 0;
                int c2 = 0;
                XmlNodeList nodeList0 = msele.ChildNodes;
                //遍历所有子节点  
                foreach (XmlNode xn in nodeList0)
                {
                    XmlElement xe = (XmlElement)xn;
                    if (xe.Name.Equals("m"))
                    {
                        monstorRndProper[c1].mid = xe.GetAttribute("id");
                        monstorRndProper[c1].minlv = int.Parse(xe.GetAttribute("minlv"));
                        monstorRndProper[c1].maxlv = int.Parse(xe.GetAttribute("maxlv"));
                        monstorRndProper[c1].minqu = int.Parse(xe.GetAttribute("minqu"));
                        monstorRndProper[c1].maxqu = int.Parse(xe.GetAttribute("maxqu"));
                        monstorRndProper[c1].rnd = int.Parse(xe.InnerText);
                        c1++;
                    }
                    else if (xe.Name.Equals("ma"))
                    {
                        monstorAwardRes.setRes(c2, xe.GetAttribute("id"));
                        monstorAwardRes.setResCount(c2, int.Parse(xe.GetAttribute("c")));
                        monstorAwardRes.setResQU(c2, int.Parse(xe.GetAttribute("qu")));
                        monstorAwardRes.setResRnd(c2, float.Parse(xe.InnerText));
                        c2++;
                    }
                }
            }

                //查找<Cells> 
            XmlElement cellele = (XmlElement)rootele.SelectSingleNode("Cells");
            blockColCount = int.Parse(cellele.GetAttribute("cs"));
            blockRowCount = int.Parse(cellele.GetAttribute("rs"));
            //获取到所有<Cells>的子节点  
            XmlNodeList nodeList = cellele.ChildNodes;
            //遍历所有子节点  
            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;

                if (xe.Name.Equals("Cell"))
                {
                    CellData celldata = new CellData();
                    cellsDatalist.Add(celldata);
                    celldata.col = int.Parse(xe.GetAttribute("c"));
                    celldata.row = int.Parse(xe.GetAttribute("r"));
                    celldata.postype = int.Parse(xe.GetAttribute("p"));
                    celldata.walkable = int.Parse(xe.GetAttribute("w")) == 1 ? true : false;
                    if (xe.HasAttribute("m"))
                    {
                        celldata.boardblock = "boardblock_" + xe.GetAttribute("m");
                    }
                    if (xe.HasAttribute("d"))
                    {
                        celldata.buildblock = "buildblock_" + xe.GetAttribute("d");
                    }

                    XmlNodeList subList = xe.ChildNodes;
                    foreach (XmlNode xmlNode in subList)
                    {
                        XmlElement sxe = (XmlElement)xmlNode;
                        string ename = sxe.Name;

                        if (ename.Contains("event"))
                        {
                            string[] events = sxe.InnerText.Split(';');
                            for (int i=0;i< events.Length;i++)
                                celldata.setEventRnd(i, int.Parse(events[i]));
                        }
                        else if (ename.Equals("npcid"))
                        {
                            celldata.posnpcid = sxe.InnerText;
                        }
                        else if (ename.Equals("npcrnd"))
                        {
                            celldata.posnpcrnd = int.Parse(sxe.InnerText);
                        }
                        else if (ename.Contains("msatt"))
                        {
                            int mindex = int.Parse(ename.Substring(ename.Length - 1)) - 1;
                            celldata.setMonstersId(mindex, sxe.GetAttribute("id"));
                            celldata.setMonstersQU(mindex, int.Parse(sxe.GetAttribute("qu")));
                            celldata.setMonstersLv(mindex, int.Parse(sxe.InnerText));
                        }
                        else if (ename.Contains("msawd"))
                        {
                            int mindex = int.Parse(ename.Substring(ename.Length - 1)) - 1;
                            celldata.setMonstersGetRes(mindex, sxe.GetAttribute("id"));
                            celldata.setMonstersGetResQU(mindex, int.Parse(sxe.GetAttribute("qu")));
                            celldata.setMonstersGetResRnd(mindex, float.Parse(sxe.GetAttribute("rnd")));
                            celldata.setMonstersGetResCount(mindex, int.Parse(sxe.InnerText));
                        }
                        else if (ename.Equals("A") || ename.Equals("B"))
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
                                else if (axe.Name.Equals("eboss"))
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
                                    cdata.setGetResQU(k, int.Parse(axe.GetAttribute("qu")));
                                    cdata.setGetResRnd(k, float.Parse(axe.GetAttribute("rnd")));
                                    k++;
                                }
                            }
                            if (ename.Equals("A"))
                            {
                                celldata.choiceDataA = cdata;
                            }
                            else if (ename.Equals("B"))
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
                //c.BackColor = System.Drawing.Color.Green;
                c.BorderStyle = BorderStyle.None;
                if (celldata.boardblock != null)
                {
                    c.Load(resPath + "/" + celldata.boardblock);
                    if (celldata.walkable)
                        picChageGray(c);
                }

                if (celldata.buildblock != null)
                {
                    string imgpath = resPath + "/" + celldata.buildblock;
                    AddBuldingPic(celldata.col, celldata.row, imgpath);
                }
                int posRadiotype = celldata.postype;
                if (posRadiotype >= 0)
                {
                    string radiontypestr = "radioButton" + (posRadiotype + 1);
                    RadioButton radiobtn = (RadioButton)groupBox2.Controls.Find(radiontypestr, false)[0];
                    c.Controls.Find("poslbl", false)[0].Text = radiobtn.Text;
                }
                for (int i = 0; i < 7; i++)
                {
                    if (celldata.getEventRnd(i) > 0)
                    {
                        c.Controls.Find("poslbl2", false)[0].Text = "*";
                        break;
                    }
                }
            }
            if (cellsDatalist.Count > 0)
            {
                CellData c0 = (CellData)cellsDatalist[0];
                PictureBox c = this.maptable.GetControlFromPosition(c0.col, c0.row) as PictureBox;
                setSelectPic(c);
            }

            for (int i = 0; i < 6; i++)
            {
                if (monstorRndProper[i].mid != null && monstorRndProper[i].mid.Length > 0 && monstorRndProper[i].rnd > 0)
                {
                    string name = "monstoridtxt" + i;
                    TextBox txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorRndProper[i].mid;

                    name = "monstorlvtxt" + i + "_0";
                    txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorRndProper[i].minlv + "";

                    name = "monstorlvtxt" + i + "_1";
                    txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorRndProper[i].maxlv + "";

                    name = "monstorqutxt" + i + "_0";
                    txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorRndProper[i].minqu + "";

                    name = "monstorqutxt" + i + "_1";
                    txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorRndProper[i].maxqu + "";

                    name = "monstorrndtxt" + i;
                    txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorRndProper[i].rnd +"";
                }

                if (monstorAwardRes.getRes(i) != null && monstorAwardRes.getRes(i).Length > 0 && monstorAwardRes.getResCount(i) > 0 && monstorAwardRes.getResRnd(i) > 0)
                {
                    string name = "monstorawardid" + i;
                    TextBox txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorAwardRes.getRes(i);

                    name = "monstorawardcount" + i;
                    txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorAwardRes.getResCount(i) + "";

                    name = "monstorawardqu" + i;
                    txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorAwardRes.getResQU(i) + "";

                    name = "monstorawardrnd" + i;
                    txtbox = (TextBox)groupBox7.Controls.Find(name, false)[0];
                    txtbox.Text = monstorAwardRes.getResRnd(i) + "";
                }
            }

            monstorcount0.Text = monstormincount + "";
            monstorcount1.Text = monstormaxcount + "";
        }

        private void saveXml(string xmlfile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlSM = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlSM);
            XmlElement rootele = xmlDoc.CreateElement("cfg");
            xmlDoc.AppendChild(rootele);
            if (monstormincount > 0 && monstormaxcount > 0 && monstormaxcount >= monstormincount)
            {
                XmlElement xmlms = xmlDoc.CreateElement("Monstors");
                xmlms.SetAttribute("minc", monstormincount + "");
                xmlms.SetAttribute("maxc", monstormaxcount + "");

                for (int i = 0; i < 6; i++)
                {
                    if (monstorRndProper[i].mid != null && monstorRndProper[i].mid.Length > 0 && monstorRndProper[i].rnd > 0)
                    {
                        XmlElement xmlele = xmlDoc.CreateElement("m");
                        xmlele.SetAttribute("id", monstorRndProper[i].mid);
                        xmlele.SetAttribute("minlv", monstorRndProper[i].minlv + "");
                        xmlele.SetAttribute("maxlv", monstorRndProper[i].maxlv + "");
                        xmlele.SetAttribute("minqu", monstorRndProper[i].minqu + "");
                        xmlele.SetAttribute("maxqu", monstorRndProper[i].maxqu + "");
                        xmlele.InnerText = monstorRndProper[i].rnd + "";
                        xmlms.AppendChild(xmlele);
                    }
                }
                for (int i = 0; i < 6; i++)
                {
      
                    if (monstorAwardRes.getRes(i) != null && monstorAwardRes.getRes(i).Length > 0 && monstorAwardRes.getResCount(i) > 0 && monstorAwardRes.getResRnd(i) > 0)
                    {
                        XmlElement xmlele = xmlDoc.CreateElement("ma");
                        xmlele.SetAttribute("id", monstorAwardRes.getRes(i));
                        xmlele.SetAttribute("c", monstorAwardRes.getResCount(i) + "");
                        xmlele.SetAttribute("qu", monstorAwardRes.getResQU(i) + "");
                        xmlele.InnerText = monstorAwardRes.getResRnd(i).ToString("f2");
                        xmlms.AppendChild(xmlele);
                    }
                }
                rootele.AppendChild(xmlms);
            }

            XmlElement xmlcells = xmlDoc.CreateElement("Cells");
            xmlcells.SetAttribute("rs", this.maptable.RowCount+ "");
            xmlcells.SetAttribute("cs", this.maptable.ColumnCount + "");
            rootele.AppendChild(xmlcells);

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
                        cell.SetAttribute("p", celldata.postype + "");
                        int canwalk = celldata.walkable ? 1 : 0;
                        cell.SetAttribute("w", canwalk + "");

                        if (celldata.boardblock != null)
                        {
                            cell.SetAttribute("m", celldata.boardblock.Substring(celldata.boardblock.LastIndexOf("_")+1));
                        }
                        else
                        {
                            MessageBox.Show(this, (j+1) + "行，" +(i+1)+"列未配置地块！！！！！！！" , "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        foreach (PictureBox picbox in buildPicBoxList)
                        {
                            int r = picbox.TabIndex % 100;
                            int c = picbox.TabIndex / 100;
                            if (r == j && c == i)
                            {
                                string imgpth = picbox.Tag as string;
                                string bname = imgpth.Substring(imgpth.LastIndexOf("/") + 1);
                                cell.SetAttribute("d", bname.Substring(bname.LastIndexOf("_") + 1));
                            }
                        }
                        xmlcells.AppendChild(cell);

                        XmlElement xmlele;

                        xmlele = xmlDoc.CreateElement("event");
                        string eventstr = "";
                        for (int m = 1; m <= 7; m++)
                        {
                            string oneevent = celldata.getEventRnd(m - 1) + ";";
                            eventstr += oneevent;
                        }
                        if (!eventstr.Equals("0;0;0;0;0;0;0;"))
                        {
                            xmlele.InnerText = eventstr.Substring(0, eventstr.Length - 1);
                            cell.AppendChild(xmlele);
                        }
                        if (celldata.postype > 0)
                        {
                            if (celldata.posnpcid.Length > 0)
                            {
                                xmlele = xmlDoc.CreateElement("npcid");
                                xmlele.InnerText = celldata.posnpcid;
                                cell.AppendChild(xmlele);
                            }
                            if (celldata.posnpcrnd > 0)
                            {
                                xmlele = xmlDoc.CreateElement("npcrnd");
                                xmlele.InnerText = celldata.posnpcrnd + "";
                                cell.AppendChild(xmlele);
                            }

                            for (int n = 1;n <= 6;n++)
                            {
                                string msid = celldata.getMonstersId(n - 1);
                                if (msid != null && msid.Length > 0)
                                {
                                    string mname = "msatt" + n;
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
                                int qu = celldata.getMonstersGetResQU(k - 1);
                                float rnd = celldata.getMonstersGetResRnd(k - 1);
                                if (msid != null && msid.Length > 0 && count > 0)
                                {
                                    string mname = "msawd" + k;
                                    xmlele = xmlDoc.CreateElement(mname);
                                    xmlele.InnerText = count + "";
                                    xmlele.SetAttribute("id", msid);
                                    xmlele.SetAttribute("qu", qu+"");
                                    xmlele.SetAttribute("rnd", rnd.ToString("f2"));
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
                                    cstring = "A";
                                }
                                else
                                {
                                    cdata = celldata.choiceDataB;
                                    cstring = "B";
                                }
                                if (cdata != null && cdata.Content != null)
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
                                            xe.SetAttribute("qu", cdata.getGetResQU(n) + "");
                                            xe.SetAttribute("rnd", cdata.getGetResRnd(n).ToString("f2"));
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
                                        XmlElement xet = xmlDoc.CreateElement("eboss");
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
            StreamWriter sw = new StreamWriter(xmlfile, false, new UTF8Encoding(false));
            xmlDoc.Save(sw);
            sw.Close();
            //xmlDoc.Save(xmlfile);
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
                editwalkable = -1;
                //if (boardPath.Contains("boardblock"))
                //    this.maptable.MouseMove += new System.Windows.Forms.MouseEventHandler(maptable_MouseMove);
            }
            //else
            //{
            //    boardPath = null;
            //   // this.maptable.MouseMove -= new System.Windows.Forms.MouseEventHandler(maptable_MouseMove);
            //}
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                CellData cellData = cur_picbox.Tag as CellData;
                cellData.walkable = this.checkBox1.Checked;

                if (cur_picbox.Image != null)
                {
                    if (this.checkBox1.Checked)
                        picChageGray(cur_picbox);
                    else
                    {
                        if (cellData.boardblock != null)
                            cur_picbox.Load(resPath + "/" + cellData.boardblock);
                    }
                }
            }
        }

        private void picChageGray(PictureBox picbox)
        {
            if (picbox.Image == null)
                return;
            int Height = picbox.Image.Height;
            int Width = picbox.Image.Width;
            Bitmap bitmap = new Bitmap(Width, Height);
            Bitmap MyBitmap = (Bitmap)picbox.Image;
            Color pixel;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    pixel = MyBitmap.GetPixel(x, y);
                    int r, g, b, Result = 0;
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    //实例程序以加权平均值法产生黑白图像  
                    int iType = 2;
                    switch (iType)
                    {
                        case 0://平均值法  
                            Result = ((r + g + b) / 3);
                            break;
                        case 1://最大值法  
                            Result = r > g ? r : g;
                            Result = Result > b ? Result : b;
                            break;
                        case 2://加权平均值法  
                            Result = ((int)(0.7 * r) + (int)(0.2 * g) + (int)(0.1 * b));
                            break;
                    }
                    bitmap.SetPixel(x, y, Color.FromArgb(160,r, g, b));
                }
            picbox.Image = bitmap;
        }

        private void AgetResQUTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                TextBox txtBox = sender as TextBox;
                string name = txtBox.Name;
                int index = int.Parse(name.Substring(name.Length - 1));
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataA != null)
                {
                    int qu = 0;
                    try
                    {
                        qu = int.Parse(txtBox.Text);
                    }
                    catch (Exception ex)
                    {
                        if (txtBox.Text.Length > 0)
                            MessageBox.Show(this, "请输入整数！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    celldata.choiceDataA.setGetResQU(index, qu);
                }
            }
        }

        private void BgetResQUTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                TextBox txtBox = sender as TextBox;
                string name = txtBox.Name;
                int index = int.Parse(name.Substring(name.Length - 1));
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataB != null)
                {
                    int qu = 0;
                    try
                    {
                        qu = int.Parse(txtBox.Text);
                    }
                    catch (Exception ex)
                    {
                        if (txtBox.Text.Length > 0)
                            MessageBox.Show(this, "请输入整数！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    celldata.choiceDataB.setGetResQU(index, qu);
                }
            }
        }

        private void listView1_Validated(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
                boardPath = null;
        }

        private void walkablebtn_Click(object sender, EventArgs e)
        {
            editwalkable = 0;
        }

        private void AgetResRndTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                TextBox txtBox = sender as TextBox;
                string name = txtBox.Name;
                int index = int.Parse(name.Substring(name.Length - 1));
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataA != null)
                {
                    float rnd = 0;
                    try
                    {
                        rnd = float.Parse(txtBox.Text);
                    }
                    catch (Exception ex)
                    {
                        if (txtBox.Text.Length > 0)
                            MessageBox.Show(this, "请输入数字！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    celldata.choiceDataA.setGetResRnd(index, rnd);
                }
            }
        }

        private void BgetResRndTxtbox_TextChanged(object sender, EventArgs e)
        {
            if (cur_picbox != null)
            {
                TextBox txtBox = sender as TextBox;
                string name = txtBox.Name;
                int index = int.Parse(name.Substring(name.Length - 1));
                CellData celldata = cur_picbox.Tag as CellData;
                if (celldata != null && celldata.choiceDataB != null)
                {
                    float rnd = 0;
                    try
                    {
                        rnd = float.Parse(txtBox.Text);
                    }
                    catch (Exception ex)
                    {
                        if (txtBox.Text.Length > 0)
                            MessageBox.Show(this, "请输入数字！！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    celldata.choiceDataB.setGetResRnd(index, rnd);
                }
            }
        }

        private void monstorpropertxt_TextChanged(object sender, EventArgs e)
        {
            TextBox txtbox = sender as TextBox;
            try
            {
                if (txtbox.Name.Contains("monstoridtxt"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));

                    monstorRndProper[index].mid = txtbox.Text;
                }
                else if (txtbox.Name.Contains("monstorlvtxt"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 3, 1));
                    int sub = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));

                    if (sub == 0)
                        monstorRndProper[index].minlv = int.Parse(txtbox.Text);
                    else if (sub == 1)
                        monstorRndProper[index].maxlv = int.Parse(txtbox.Text);
                }
                else if (txtbox.Name.Contains("monstorqutxt"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 3, 1));
                    int sub = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));

                    if (sub == 0)
                        monstorRndProper[index].minqu = int.Parse(txtbox.Text);
                    else if (sub == 1)
                        monstorRndProper[index].maxqu = int.Parse(txtbox.Text);
                }
                else if (txtbox.Name.Contains("monstorrndtxt"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));

                    monstorRndProper[index].rnd = int.Parse(txtbox.Text);
                }
                else if (txtbox.Name.Contains("monstorcount"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));
                    if (index == 0)
                        monstormincount = int.Parse(txtbox.Text);
                    else if (index == 1)
                        monstormaxcount = int.Parse(txtbox.Text);
                }
            }catch(Exception ex)
            {

            }
        }

        private void monstoraward_TextChanged(object sender, EventArgs e)
        {
            TextBox txtbox = sender as TextBox;
            try
            {
                if (txtbox.Name.Contains("monstorawardid"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));

                    monstorAwardRes.setRes(index, txtbox.Text);
                }
                else if (txtbox.Name.Contains("monstorawardcount"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));

                    monstorAwardRes.setResCount(index, int.Parse(txtbox.Text));
                }
                else if (txtbox.Name.Contains("monstorawardqu"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));

                    monstorAwardRes.setResQU(index, int.Parse(txtbox.Text));
                }
                else if (txtbox.Name.Contains("monstorawardrnd"))
                {
                    int index = int.Parse(txtbox.Name.Substring(txtbox.Name.Length - 1));

                    monstorAwardRes.setResRnd(index, float.Parse(txtbox.Text));
                }
            }catch (Exception ex)
            {

            }
        }
    }
}
