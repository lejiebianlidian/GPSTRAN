using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Common;
using System.Text.RegularExpressions;
using Microsoft.Win32;

/**
 *配置界面 
**/


namespace GPSTran
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
            this.splitContainer1.IsSplitterFixed = true;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            this.Text = Resource.lHelper.Key("n1");
            treeView1.Height = splitContainer1.Panel1.Height;
            treeView1.Width = splitContainer1.Panel1.Width;

            //初始化左侧树形
            TreeNode root = new TreeNode();
            root.Tag = "root";
            root.Text = Resource.lHelper.Key("n1");
            root.Name = "root";
            TreeNode general = new TreeNode();
            general.Tag = "general";
            general.Name = "general";
            general.Text = Resource.lHelper.Key("n58");

            TreeNode portNode = new TreeNode();
            portNode.Tag = "listeningPort";
            portNode.Text = Resource.lHelper.Key("n12");
            portNode.Name = "listeningPort";
            TreeNode webGis = new TreeNode();
            webGis.Tag = "webGis";
            webGis.Text = Resource.lHelper.Key("n33");
            webGis.Name = "webGis";
            TreeNode gpsTran = new TreeNode();
            gpsTran.Tag = "gpsTran";
            gpsTran.Text = Resource.lHelper.Key("n34");
            gpsTran.Name = "gpsTran";
            TreeNode userInfo = new TreeNode();
            userInfo.Tag = "userInfo";
            userInfo.Text = Resource.lHelper.Key("n35");
            userInfo.Name = "userInfo";
            TreeNode ipNodes = new TreeNode();
            ipNodes.Tag = "ipTran";
            ipNodes.Text = Resource.lHelper.Key("n28");
            ipNodes.Name = "ipTran";
            TreeNode dbNodes = new TreeNode();
            dbNodes.Tag = "dbTran";
            dbNodes.Text = Resource.lHelper.Key("n29");
            dbNodes.Name = "dbTran";
            TreeNode pluginInstanceNodes = new TreeNode();
            pluginInstanceNodes.Tag = "pluginTran";
            pluginInstanceNodes.Text = Resource.lHelper.Key("n36");
            pluginInstanceNodes.Name = "pluginTran";
            foreach (IPManager ipManager in Resource.IPList.Values)
            {
                TreeNode node = new TreeNode();
                node.Tag = ipManager.GetModel().Name;
                node.Text = ipManager.GetModel().Name;
                node.Name = ipManager.GetModel().Name;
                ipNodes.Nodes.Add(node);


            }
            foreach (DBManager dbManager in Resource.DBList.Values)
            {
                TreeNode node = new TreeNode();
                node.Tag = dbManager.GetModel().Name;
                node.Text = dbManager.GetModel().Name;
                node.Name = dbManager.GetModel().Name;
                dbNodes.Nodes.Add(node);

            }



            foreach (PluginManager pManager in Resource.PluginList.Values)
            {
                TreeNode node = new TreeNode();


                node.Tag = pManager.GetPluginModel().Name;
                node.Text = pManager.GetPluginModel().Name;
                //node.Name = pManager.GetPluginModel().Name; 
                node.Name = pManager.GetPluginModel().Name + "plugin";  //han modify 
                PluginType ptype = pManager.GetPluginType();
                if (ptype == PluginType.RemoteDB)
                {
                    dbNodes.Nodes.Add(node);
                }
                else if (ptype == PluginType.TranDB)
                {
                    dbNodes.Nodes.Add(node);
                }
                else if (ptype == PluginType.RemoteIP)
                {
                    ipNodes.Nodes.Add(node);
                }

            }



            root.Nodes.Add(general);
            root.Nodes.Add(portNode);
            root.Nodes.Add(webGis);
            root.Nodes.Add(gpsTran);
            root.Nodes.Add(userInfo);
            //   if (Resource.DBCheck)
            //  {
            root.Nodes.Add(ipNodes);
            root.Nodes.Add(dbNodes);
            //     root.Nodes.Add(pluginInstanceNodes);  han modify 去掉插件节点
            // }

            treeView1.Nodes.Add(root);

            splitContainer1.Panel2.Controls.Clear();


        }


        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            treeView1.Width = splitContainer1.Panel1.Width;
            treeView1.Height = splitContainer1.Panel1.Height;
        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Resource.configFormExisted = false;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ControlBind(e);
        }

        //选择不同的树形结点，显示不同的配置界面
        private void ControlBind(TreeViewEventArgs e)
        {
            if (e.Node.Tag.Equals("root"))
            {
                return;
            }

            if (e.Node.Tag.ToString().Equals("general"))
            {
                BindGeneral();
            }

            if (e.Node.Tag.ToString().Equals("listeningPort"))
            {

                BindPort();

            }
            else if (e.Node.Tag.ToString().Equals("webGis"))
            {
                BindWebGis();

            }
            else if (e.Node.Tag.Equals("gpsTran"))
            {
                BindTran();

            }
            else if (e.Node.Tag.Equals("userInfo"))
            {
                BindUserInfo();

            }
            else if (e.Node.Tag.Equals("ipTran"))
            {
                BindPlugin(e);  //BindIPItem(); 选择协议版本
            }
            else if (e.Node.Parent.Tag.Equals("ipTran"))
            {
                //BindIPInstance();
                chooseBindIPInstance();
            }
            else if (e.Node.Tag.Equals("dbTran"))
            {
                BindPlugin(e); // BindDBItem();
            }
            else if (e.Node.Parent.Tag.Equals("dbTran"))
            {
                // BindDBInstance();
                chooseBindDBInstance();
            }
            /*
            else if (e.Node.Tag.Equals("pluginTran")) 
            {
                BindPlugin();
            }
            else if (e.Node.Parent.Tag.Equals("pluginTran")) 
            {
                BindPluginInstance();
            }
             * */



        }


        //一般配置界面
        private void BindGeneral()
        {
            splitContainer1.Panel2.Controls.Clear();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.ComboBox comboBox1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label panelType;
            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.CheckBox autoRunBox;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.TextBox logKeepBox;


            autoRunBox = new CheckBox();
            enterBtn = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new Label();
            panelType = new System.Windows.Forms.Label();
            comboBox1 = new System.Windows.Forms.ComboBox();
            label4 = new Label();
            logKeepBox = new TextBox();

            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n37");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "general";
            panelType.Visible = false;

            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label1.Location = new System.Drawing.Point(23, 14);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(67, 14);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("n59");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(43, 133);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(53, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n57");
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new System.Drawing.Point(180, 129);
            comboBox1.Name = "languageBox";
            comboBox1.Size = new System.Drawing.Size(173, 20);
            comboBox1.TabIndex = 2;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.AddRange(Resource.LanguageList);
            if (Resource.lHelper.CurLanguage == Language.en_us)
            {
                comboBox1.SelectedItem = Resource.LanguageList[0];
            }
            else if (Resource.lHelper.CurLanguage == Language.zh_cn)
            {
                comboBox1.SelectedItem = Resource.LanguageList[1];
            }


            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(43, 340);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(251, 12);
            label3.TabIndex = 3;
            label3.Text = Resource.lHelper.Key("m67");
            label3.ForeColor = System.Drawing.Color.Red;
            label3.Width = 300;
            label3.AutoSize = false;
            label3.Height = 40;

            // 
            // autoRunBox
            // 
            autoRunBox.AutoSize = true;
            autoRunBox.Location = new System.Drawing.Point(180, 240);
            autoRunBox.Name = "autoRunBox";
            autoRunBox.Size = new System.Drawing.Size(72, 16);
            autoRunBox.TabIndex = 4;
            autoRunBox.Text = Resource.lHelper.Key("n62");
            autoRunBox.UseVisualStyleBackColor = true;
            autoRunBox.Checked = Resource.IsAutoRun;


            ///
            ///todo: show  control about logkeepdays
            ///

            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(43, 188);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(90, 12);
            label4.TabIndex = 1;
            label4.Text = Resource.lHelper.Key("n73");

            logKeepBox.Location = new System.Drawing.Point(180, 183);
            logKeepBox.Name = "logKeepBox";
            logKeepBox.Size = new System.Drawing.Size(150, 21);
            logKeepBox.TabIndex = 1;
            logKeepBox.Text = Resource.logKeepDays.ToString();




            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(comboBox1);
            splitContainer1.Panel2.Controls.Add(label2);
            // splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(panelType);
            splitContainer1.Panel2.Controls.Add(autoRunBox);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(logKeepBox);


        }

        //监听端口配置界面
        private void BindPort()
        {
            splitContainer1.Panel2.Controls.Clear();

            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.TextBox listeningPort;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label panelType;


            enterBtn = new System.Windows.Forms.Button();
            listeningPort = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            panelType = new System.Windows.Forms.Label();

            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n37");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);
            // 
            // listeningPort
            // 
            listeningPort.Location = new System.Drawing.Point(180, 121);
            listeningPort.Name = "listeningPort";
            listeningPort.Size = new System.Drawing.Size(150, 21);
            listeningPort.TabIndex = 1;
            listeningPort.Text = Resource.ListeningPort.ToString();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(43, 127);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(59, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("n12");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = System.Drawing.Color.Red;
            label2.Location = new System.Drawing.Point(34, 220);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(269, 12);
            label2.TabIndex = 3;
            label2.Text = Resource.lHelper.Key("m14");
            label2.Width = 300;
            label2.AutoSize = false;
            label2.Height = 40;

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "listeningPort";
            panelType.Visible = false;

            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(listeningPort);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(panelType);

        }
        //WEBGIS数据库配置界面
        private void BindWebGis()
        {
            //clear controls from panel2
            splitContainer1.Panel2.Controls.Clear();

            System.Windows.Forms.TextBox webGisInstance;
            System.Windows.Forms.TextBox webGisUser;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.TextBox webGisPort;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.TextBox webGisDB;
            System.Windows.Forms.TextBox webGisPwd;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.TextBox webGisIP;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Button testConn;
            System.Windows.Forms.Label panelType;
            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.Label label8;


            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            webGisIP = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            webGisDB = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            webGisUser = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            webGisPwd = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            webGisPort = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            webGisInstance = new System.Windows.Forms.TextBox();
            testConn = new System.Windows.Forms.Button();
            panelType = new System.Windows.Forms.Label();
            enterBtn = new System.Windows.Forms.Button();
            label8 = new System.Windows.Forms.Label();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(21, 23);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(101, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("m17");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(43, 96);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(23, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n46");
            // 
            // webGisIP
            // 
            webGisIP.Location = new System.Drawing.Point(180, 92);
            webGisIP.Name = "webGisIP";
            webGisIP.Size = new System.Drawing.Size(154, 21);
            webGisIP.TabIndex = 2;
            webGisIP.Text = Resource.WebGisIp;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(43, 134);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(47, 12);
            label3.TabIndex = 1;
            label3.Text = Resource.lHelper.Key("n45");
            // 
            // webGisDB
            // 
            webGisDB.Location = new System.Drawing.Point(180, 130);
            webGisDB.Name = "webGisDB";
            webGisDB.Size = new System.Drawing.Size(154, 21);
            webGisDB.TabIndex = 2;
            webGisDB.Text = Resource.WebGisDb;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(43, 171);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(53, 12);
            label4.TabIndex = 1;
            label4.Text = Resource.lHelper.Key("n43");
            // 
            // webGisUser
            // 
            webGisUser.Location = new System.Drawing.Point(180, 167);
            webGisUser.Name = "webGisUser";
            webGisUser.Size = new System.Drawing.Size(154, 21);
            webGisUser.TabIndex = 2;
            webGisUser.Text = Resource.WebGisUser;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(43, 206);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(53, 12);
            label5.TabIndex = 1;
            label5.Text = Resource.lHelper.Key("n44");
            // 
            // webGisPwd
            // 
            webGisPwd.Location = new System.Drawing.Point(180, 202);
            webGisPwd.Name = "webGisPwd";
            webGisPwd.Size = new System.Drawing.Size(154, 21);
            webGisPwd.TabIndex = 2;
            webGisPwd.Text = Resource.WebGisPwd;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(43, 244);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(65, 12);
            label6.TabIndex = 1;
            label6.Text = Resource.lHelper.Key("n42");
            // 
            // webGisPort
            // 
            webGisPort.Location = new System.Drawing.Point(180, 240);
            webGisPort.Name = "webGisPort";
            webGisPort.Size = new System.Drawing.Size(154, 21);
            webGisPort.TabIndex = 2;
            webGisPort.Text = Resource.WebGisPort.ToString();
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(43, 281);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(53, 12);
            label7.TabIndex = 1;
            label7.Text = Resource.lHelper.Key("n41");
            // 
            // webGisInstance
            // 
            webGisInstance.Location = new System.Drawing.Point(180, 277);
            webGisInstance.Name = "webGisInstance";
            webGisInstance.Size = new System.Drawing.Size(154, 21);
            webGisInstance.TabIndex = 2;
            webGisInstance.Text = Resource.WebGisInstance;
            // 
            // testConn
            // 
            testConn.Location = new System.Drawing.Point(180, 335);
            testConn.Name = "testConn";
            testConn.Size = new System.Drawing.Size(94, 23);
            testConn.TabIndex = 3;
            testConn.Text = Resource.lHelper.Key("n40");
            testConn.UseVisualStyleBackColor = true;
            testConn.Click += new EventHandler(TestWebGisConnection);

            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n37");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "webGis";
            panelType.Visible = false;

            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = System.Drawing.Color.Red;
            label8.Location = new System.Drawing.Point(45, 394);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(245, 24);
            label8.TabIndex = 4;
            label8.Text = Resource.lHelper.Key("m15");
            label8.Width = 300;
            label8.AutoSize = false;
            label8.Height = 40;

            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(panelType);
            splitContainer1.Panel2.Controls.Add(testConn);
            splitContainer1.Panel2.Controls.Add(webGisInstance);
            splitContainer1.Panel2.Controls.Add(webGisUser);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(webGisPort);
            splitContainer1.Panel2.Controls.Add(label6);
            splitContainer1.Panel2.Controls.Add(webGisDB);
            splitContainer1.Panel2.Controls.Add(webGisPwd);
            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(webGisIP);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(label8);

        }
        //转发数据库配置界面
        private void BindTran()
        {
            //clear controls from panel2
            splitContainer1.Panel2.Controls.Clear();

            System.Windows.Forms.TextBox tranInstance;
            System.Windows.Forms.TextBox tranUser;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.TextBox tranPort;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.TextBox tranDB;
            System.Windows.Forms.TextBox tranPwd;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.TextBox tranIP;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Button testConn;
            System.Windows.Forms.Label panelType;
            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.Label label8;


            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            tranIP = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            tranDB = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            tranUser = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            tranPwd = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            tranPort = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            tranInstance = new System.Windows.Forms.TextBox();
            testConn = new System.Windows.Forms.Button();
            panelType = new System.Windows.Forms.Label();
            enterBtn = new System.Windows.Forms.Button();
            label8 = new System.Windows.Forms.Label();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(21, 23);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(101, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("m18");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(43, 96);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(23, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n46");
            // 
            // webGisIP
            // 
            tranIP.Location = new System.Drawing.Point(180, 92);
            tranIP.Name = "tranIP";
            tranIP.Size = new System.Drawing.Size(154, 21);
            tranIP.TabIndex = 2;
            tranIP.Text = Resource.TranIp;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(43, 134);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(47, 12);
            label3.TabIndex = 1;
            label3.Text = Resource.lHelper.Key("n45");
            // 
            // webGisDB
            // 
            tranDB.Location = new System.Drawing.Point(180, 130);
            tranDB.Name = "tranDB";
            tranDB.Size = new System.Drawing.Size(154, 21);
            tranDB.TabIndex = 2;
            tranDB.Text = Resource.TranDb;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(43, 171);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(53, 12);
            label4.TabIndex = 1;
            label4.Text = Resource.lHelper.Key("n43");
            // 
            // webGisUser
            // 
            tranUser.Location = new System.Drawing.Point(180, 167);
            tranUser.Name = "tranUser";
            tranUser.Size = new System.Drawing.Size(154, 21);
            tranUser.TabIndex = 2;
            tranUser.Text = Resource.TranUser;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(43, 206);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(53, 12);
            label5.TabIndex = 1;
            label5.Text = Resource.lHelper.Key("n44");
            // 
            // webGisPwd
            // 
            tranPwd.Location = new System.Drawing.Point(180, 202);
            tranPwd.Name = "tranPwd";
            tranPwd.Size = new System.Drawing.Size(154, 21);
            tranPwd.TabIndex = 2;
            tranPwd.Text = Resource.TranPwd;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(43, 244);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(65, 12);
            label6.TabIndex = 1;
            label6.Text = Resource.lHelper.Key("n42");
            // 
            // webGisPort
            // 
            tranPort.Location = new System.Drawing.Point(180, 240);
            tranPort.Name = "tranPort";
            tranPort.Size = new System.Drawing.Size(154, 21);
            tranPort.TabIndex = 2;
            tranPort.Text = Resource.TranPort.ToString();
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(43, 281);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(53, 12);
            label7.TabIndex = 1;
            label7.Text = Resource.lHelper.Key("n41");
            // 
            // webGisInstance
            // 
            tranInstance.Location = new System.Drawing.Point(180, 277);
            tranInstance.Name = "tranInstance";
            tranInstance.Size = new System.Drawing.Size(154, 21);
            tranInstance.TabIndex = 2;
            tranInstance.Text = Resource.TranInstance;
            // 
            // testConn
            // 
            testConn.Location = new System.Drawing.Point(180, 335);
            testConn.Name = "testConn";
            testConn.Size = new System.Drawing.Size(94, 23);
            testConn.TabIndex = 3;
            testConn.Text = Resource.lHelper.Key("n40");
            testConn.UseVisualStyleBackColor = true;
            testConn.Click += new EventHandler(TestTranConnection);

            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n37");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "gpsTran";
            panelType.Visible = false;

            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = System.Drawing.Color.Red;
            label8.Location = new System.Drawing.Point(45, 394);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(245, 24);
            label8.TabIndex = 4;
            label8.Text = Resource.lHelper.Key("m16");
            label8.Width = 300;
            label8.AutoSize = false;
            label8.Height = 40;

            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(panelType);
            splitContainer1.Panel2.Controls.Add(testConn);
            splitContainer1.Panel2.Controls.Add(tranInstance);
            splitContainer1.Panel2.Controls.Add(tranUser);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(tranPort);
            splitContainer1.Panel2.Controls.Add(label6);
            splitContainer1.Panel2.Controls.Add(tranDB);
            splitContainer1.Panel2.Controls.Add(tranPwd);
            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(tranIP);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(label8);

        }
        //用户表配置界面
        private void BindUserInfo()
        {
            //clear controls from panel2
            splitContainer1.Panel2.Controls.Clear();


            System.Windows.Forms.Label label4;
            System.Windows.Forms.TextBox flushInterval;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.CheckBox InstanceEnabled;
            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.Label panelType;

            panelType = new System.Windows.Forms.Label();
            enterBtn = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            flushInterval = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            InstanceEnabled = new System.Windows.Forms.CheckBox();


            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n37");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "userInfo";
            panelType.Visible = false;


            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(25, 27);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(113, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("m19");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(43, 111);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(101, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n47");
            // 
            // textBox1
            // 
            flushInterval.Location = new System.Drawing.Point(180, 107);
            flushInterval.Name = "flushInterval";
            flushInterval.Size = new System.Drawing.Size(143, 21);
            flushInterval.TabIndex = 2;
            flushInterval.Text = Resource.FlushInterval.ToString();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = System.Drawing.Color.Red;
            label4.Location = new System.Drawing.Point(45, 308);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(251, 24);
            label4.TabIndex = 4;
            label4.Text = Resource.lHelper.Key("m20");
            label4.Width = 300;
            label4.AutoSize = false;
            label4.Height = 40;
            // 
            // InstanceEnabled
            // 
            InstanceEnabled.AutoSize = true;
            InstanceEnabled.Location = new System.Drawing.Point(180, 180);
            InstanceEnabled.Name = "InstanceEnabled";
            InstanceEnabled.Size = new System.Drawing.Size(48, 16);
            InstanceEnabled.TabIndex = 5;
            InstanceEnabled.Text = Resource.lHelper.Key("n48");
            InstanceEnabled.UseVisualStyleBackColor = true;
            InstanceEnabled.Checked = Resource.UserinfoEnabled;


            splitContainer1.Panel2.Controls.Add(InstanceEnabled);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(flushInterval);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(panelType);


        }
        //IP转发配置界面
        public void BindIPItem()
        {
            //clear controls from panel2
            splitContainer1.Panel2.Controls.Clear();

            System.Windows.Forms.Label label1;
            //System.Windows.Forms.ComboBox protocolBox;
            //System.Windows.Forms.Label label3;
            System.Windows.Forms.TextBox ipBox;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.TextBox InstanceNameBox;
            System.Windows.Forms.Label label2;
            //System.Windows.Forms.ComboBox netProtocolBox;
            //System.Windows.Forms.Label label6;
            System.Windows.Forms.TextBox portBox;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.TextBox entityFilterBox;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.TextBox lonOffsetBox;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.TextBox latOffsetBox;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Button entityFilterBtn;
            System.Windows.Forms.CheckBox instanceEnabled;
            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.Label panelType;
            System.Windows.Forms.Label protolChoose;
            System.Windows.Forms.ComboBox pluginNameBox;
            protolChoose = new Label();
            pluginNameBox = new System.Windows.Forms.ComboBox();
            panelType = new System.Windows.Forms.Label();
            enterBtn = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();

            label2 = new System.Windows.Forms.Label();
            InstanceNameBox = new System.Windows.Forms.TextBox();
            //label3 = new System.Windows.Forms.Label();
            //protocolBox = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            ipBox = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            portBox = new System.Windows.Forms.TextBox();
            //label6 = new System.Windows.Forms.Label();
            //netProtocolBox = new System.Windows.Forms.ComboBox();
            label7 = new System.Windows.Forms.Label();
            latOffsetBox = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            lonOffsetBox = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            entityFilterBox = new System.Windows.Forms.TextBox();
            instanceEnabled = new System.Windows.Forms.CheckBox();
            entityFilterBtn = new System.Windows.Forms.Button();

            protolChoose.AutoSize = true;
            protolChoose.Location = new System.Drawing.Point(39, 55);
            protolChoose.Name = "protolChoose";
            protolChoose.Size = new System.Drawing.Size(77, 12);
            protolChoose.TabIndex = 1;
            protolChoose.Text = Resource.lHelper.Key("n55");
            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n39");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "ipTran";
            panelType.Visible = false;


            // 
            // label1 添加IP转发实例
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(26, 21);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(89, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("m21");
            // 
            // InstanceNameBox
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(42, 83);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(53, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n49");
            // 
            // InstanceNameBox
            // 
            InstanceNameBox.Location = new System.Drawing.Point(180, 80);
            InstanceNameBox.Name = "InstanceNameBox";
            InstanceNameBox.Size = new System.Drawing.Size(153, 21);
            InstanceNameBox.TabIndex = 2;
            // 
            // label3
            // 
            //label3.AutoSize = true;
            //label3.Location = new System.Drawing.Point(44, 122);
            //label3.Name = "label3";
            //label3.Size = new System.Drawing.Size(53, 12);
            //label3.TabIndex = 3;
            //label3.Text = Resource.lHelper.Key("n18");
            // 
            // protocolBox
            // 
            //protocolBox.FormattingEnabled = true;
            //protocolBox.Items.AddRange(XMLIni.ProtocolList);
            //protocolBox.Location = new System.Drawing.Point(180, 120);
            //protocolBox.Name = "protocolBox";
            //protocolBox.Size = new System.Drawing.Size(153, 20);
            //protocolBox.TabIndex = 4;
            //protocolBox.SelectedIndex = 0;
            //protocolBox.DropDownStyle = ComboBoxStyle.DropDownList;
            // 
            // ipBox
            // 42, 160
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(44, 122);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(17, 12);
            label4.TabIndex = 1;
            label4.Text = Resource.lHelper.Key("n46");
            // 
            // ipBox
            // 180, 157
            ipBox.Location = new System.Drawing.Point(180, 120);
            ipBox.Name = "ipBox";
            ipBox.Size = new System.Drawing.Size(153, 21);
            ipBox.TabIndex = 2;
            // 
            // portBox
            // 42, 196
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(42, 160);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(29, 12);
            label5.TabIndex = 1;
            label5.Text = Resource.lHelper.Key("n12");
            // 
            // portBox
            // 180, 193
            portBox.Location = new System.Drawing.Point(180, 157);
            portBox.Name = "portBox";
            portBox.Size = new System.Drawing.Size(153, 21);
            portBox.TabIndex = 2;
            // 
            // netProtocolBox
            // 
            //label6.AutoSize = true;
            //label6.Location = new System.Drawing.Point(44, 232);
            //label6.Name = "label6";
            //label6.Size = new System.Drawing.Size(53, 12);
            //label6.TabIndex = 3;
            //label6.Text = Resource.lHelper.Key("n50");
            // 
            // netProtocolBox
            // 
            //netProtocolBox.FormattingEnabled = true;
            //netProtocolBox.Items.AddRange(XMLIni.NetProtocolList);
            //netProtocolBox.Location = new System.Drawing.Point(180, 230);
            //netProtocolBox.Name = "netProtocolBox";
            //netProtocolBox.Size = new System.Drawing.Size(153, 20);
            //netProtocolBox.TabIndex = 4;
            //netProtocolBox.SelectedIndex = 0;
            //netProtocolBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // 
            // latOffsetBox
            // 42, 307
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(42, 196);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(53, 12);
            label7.TabIndex = 1;
            label7.Text = Resource.lHelper.Key("n52");
            // 
            // latOffsetBox
            // 180, 304
            latOffsetBox.Location = new System.Drawing.Point(180, 193);
            latOffsetBox.Name = "latOffsetBox";
            latOffsetBox.Size = new System.Drawing.Size(153, 21);
            latOffsetBox.TabIndex = 2;
            // 
            // lonOffsetBox 经度偏移
            // 42, 271
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(44, 232);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(53, 12);
            label8.TabIndex = 1;
            label8.Text = Resource.lHelper.Key("n51");
            // 
            // lonOffsetBox
            // 180, 268
            lonOffsetBox.Location = new System.Drawing.Point(180, 230);
            lonOffsetBox.Name = "lonOffsetBox";
            lonOffsetBox.Size = new System.Drawing.Size(153, 21);
            lonOffsetBox.TabIndex = 2;
            // 
            // entityFilterBox
            // 42, 344
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(42, 268);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(53, 12);
            label10.TabIndex = 1;
            label10.Text = Resource.lHelper.Key("n24");
            // 
            // entityFilterBox
            // 180, 341
            entityFilterBox.Location = new System.Drawing.Point(180, 268);
            entityFilterBox.Name = "entityFilterBox";
            entityFilterBox.Size = new System.Drawing.Size(153, 21);
            entityFilterBox.TabIndex = 2;
            entityFilterBox.Enabled = false;
            // 
            // instanceEnabled
            // 
            instanceEnabled.AutoSize = true;
            instanceEnabled.Location = new System.Drawing.Point(180, 300);
            instanceEnabled.Name = "instanceEnabled";
            instanceEnabled.Size = new System.Drawing.Size(48, 16);
            instanceEnabled.TabIndex = 5;
            instanceEnabled.Text = Resource.lHelper.Key("n48");
            instanceEnabled.UseVisualStyleBackColor = true;

            pluginNameBox.FormattingEnabled = true;
            pluginNameBox.Location = new System.Drawing.Point(180, 51);
            pluginNameBox.Name = "pluginNameBox";
            pluginNameBox.Size = new System.Drawing.Size(151, 20);
            pluginNameBox.TabIndex = 2;
            pluginNameBox.Items.Clear();
            pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;
            pluginNameBox.Items.Add("UDP_Version_0");
            foreach (string s in Resource.DllList.Keys)
            {
                if (s.Contains("UDP") || s.Contains("TCP"))
                {
                    pluginNameBox.Items.Add(s);
                }
            }
            pluginNameBox.SelectedItem = "UDP_Version_0";
            pluginNameBox.SelectedValueChanged += new EventHandler(pluginName_SelectedIndexChanged);



            // 
            // entityFilterBtn
            // 340, 339
            entityFilterBtn.Location = new System.Drawing.Point(340, 268);
            entityFilterBtn.Name = "entityFilterBtn";
            entityFilterBtn.Size = new System.Drawing.Size(36, 23);
            entityFilterBtn.TabIndex = 6;
            entityFilterBtn.Text = "...";
            entityFilterBtn.UseVisualStyleBackColor = true;
            entityFilterBtn.Click += new EventHandler(SelectEntity);

            splitContainer1.Panel2.Controls.Add(entityFilterBtn);
            splitContainer1.Panel2.Controls.Add(instanceEnabled);
            //splitContainer1.Panel2.Controls.Add(netProtocolBox);
            //splitContainer1.Panel2.Controls.Add(protocolBox);
            //splitContainer1.Panel2.Controls.Add(label6);
            //splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(entityFilterBox);
            splitContainer1.Panel2.Controls.Add(label10);
            splitContainer1.Panel2.Controls.Add(lonOffsetBox);
            splitContainer1.Panel2.Controls.Add(label8);
            splitContainer1.Panel2.Controls.Add(ipBox);
            splitContainer1.Panel2.Controls.Add(latOffsetBox);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(portBox);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(InstanceNameBox);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(panelType);
            splitContainer1.Panel2.Controls.Add(pluginNameBox);
            splitContainer1.Panel2.Controls.Add(protolChoose);


        }

        public void chooseBindDBInstance()
        {
            if (!treeView1.SelectedNode.Name.Contains("plugin"))
            {
                BindDBInstance();
            }
            else
            {
                BindPluginInstance();
            }
        }


        public void chooseBindIPInstance()
        {
            if (!treeView1.SelectedNode.Name.Contains("plugin"))
            {
                BindIPInstance();
            }
            else
            {
                BindPluginInstance();
            }
        }

        //IP转发实例配置界面
        public void BindIPInstance()
        {
            splitContainer1.Panel2.Controls.Clear();

            string ipInstanceName = treeView1.SelectedNode.Tag.ToString();

            System.Windows.Forms.Label label1;
            //System.Windows.Forms.ComboBox protocolBox;
            //System.Windows.Forms.Label label3;
            System.Windows.Forms.TextBox ipBox;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.TextBox InstanceNameBox;
            System.Windows.Forms.Label label2;
            //System.Windows.Forms.ComboBox netProtocolBox;
            //System.Windows.Forms.Label label6;
            System.Windows.Forms.TextBox portBox;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.TextBox entityFilterBox;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.TextBox lonOffsetBox;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.TextBox latOffsetBox;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Button entityFilterBtn;
            System.Windows.Forms.CheckBox instanceEnabled;
            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.Label panelType;
            System.Windows.Forms.Label oldInstanceName;
            System.Windows.Forms.Button deleteBtn;
            System.Windows.Forms.ComboBox pluginNameBox;
            System.Windows.Forms.Label protolChoose;

            protolChoose = new Label();
            deleteBtn = new Button();
            pluginNameBox = new ComboBox();
            panelType = new System.Windows.Forms.Label();
            enterBtn = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            InstanceNameBox = new System.Windows.Forms.TextBox();
            //label3 = new System.Windows.Forms.Label();
            //protocolBox = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            ipBox = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            portBox = new System.Windows.Forms.TextBox();
            //label6 = new System.Windows.Forms.Label();
            //netProtocolBox = new System.Windows.Forms.ComboBox();
            label7 = new System.Windows.Forms.Label();
            latOffsetBox = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            lonOffsetBox = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            entityFilterBox = new System.Windows.Forms.TextBox();
            instanceEnabled = new System.Windows.Forms.CheckBox();
            entityFilterBtn = new System.Windows.Forms.Button();
            oldInstanceName = new Label();


            protolChoose.AutoSize = true;
            protolChoose.Location = new System.Drawing.Point(39, 55);
            protolChoose.Name = "protolChoose";
            protolChoose.Size = new System.Drawing.Size(77, 12);
            protolChoose.TabIndex = 1;
            protolChoose.Text = Resource.lHelper.Key("n55");


            pluginNameBox.FormattingEnabled = true;
            pluginNameBox.Location = new System.Drawing.Point(180, 51);
            pluginNameBox.Name = "pluginNameBox";
            pluginNameBox.Size = new System.Drawing.Size(151, 20);
            pluginNameBox.TabIndex = 2;
            pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;
            pluginNameBox.Items.Add("UDP_Version_0");
            pluginNameBox.SelectedIndex = 0;
            pluginNameBox.Enabled = false;

            //
            //deleteBtn
            //
            deleteBtn.Location = new System.Drawing.Point(170, 496);
            deleteBtn.Name = "deleteBtn";
            deleteBtn.Size = new System.Drawing.Size(75, 23);
            deleteBtn.TabIndex = 2;
            deleteBtn.Text = Resource.lHelper.Key("n38");
            deleteBtn.UseVisualStyleBackColor = true;
            deleteBtn.Click += new EventHandler(DeleteBtn_Click);

            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n37");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "ipTranInstance";
            panelType.Visible = false;

            oldInstanceName.AutoSize = true;
            oldInstanceName.Location = new System.Drawing.Point(96, 122);
            oldInstanceName.Name = "oldInstanceName";
            oldInstanceName.Size = new System.Drawing.Size(41, 12);
            oldInstanceName.TabIndex = 0;
            oldInstanceName.Text = ipInstanceName;
            oldInstanceName.Visible = false;


            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(26, 21);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(89, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("m22");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(42, 83);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(53, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n49");
            // 
            // InstanceNameBox
            // 
            InstanceNameBox.Location = new System.Drawing.Point(180, 80);
            InstanceNameBox.Name = "InstanceNameBox";
            InstanceNameBox.Size = new System.Drawing.Size(153, 21);
            InstanceNameBox.TabIndex = 2;
            InstanceNameBox.Text = ipInstanceName;
            // 
            // protocolBox
            // 
            //label3.AutoSize = true;
            //label3.Location = new System.Drawing.Point(44, 122);
            //label3.Name = "label3";
            //label3.Size = new System.Drawing.Size(53, 12);
            //label3.TabIndex = 3;
            //label3.Text = Resource.lHelper.Key("n18");
            // 
            // protocolBox
            // 
            //protocolBox.FormattingEnabled = true;
            //protocolBox.Items.AddRange(new object[] {
            //"PGIS"});
            //protocolBox.Location = new System.Drawing.Point(180, 120);
            //protocolBox.Name = "protocolBox";
            //protocolBox.Size = new System.Drawing.Size(153, 20);
            //protocolBox.TabIndex = 4;
            //protocolBox.SelectedIndex = 0;
            //protocolBox.DropDownStyle = ComboBoxStyle.DropDownList;
            //for (int i = 0; i < protocolBox.Items.Count; i++) 
            //{
            //    if (((string)protocolBox.Items[i]).Equals(Resource.IPList[ipInstanceName].GetModel().Protocol.ToUpper())) 
            //    {
            //        protocolBox.SelectedIndex = i;
            //        break;
            //    }

            //}

            // 
            // label4
            // (42, 160);
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(44, 122);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(17, 12);
            label4.TabIndex = 1;
            label4.Text = Resource.lHelper.Key("n46");
            // 
            // ipBox
            // (180, 157);
            ipBox.Location = new System.Drawing.Point(180, 120);
            ipBox.Name = "ipBox";
            ipBox.Size = new System.Drawing.Size(153, 21);
            ipBox.TabIndex = 2;
            ipBox.Text = Resource.IPList[ipInstanceName].GetModel().Ip;
            // 
            // label5
            // (42, 196);
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(42, 160);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(29, 12);
            label5.TabIndex = 1;
            label5.Text = Resource.lHelper.Key("n12");
            // 
            // portBox
            // (180, 193);
            portBox.Location = new System.Drawing.Point(180, 157);
            portBox.Name = "portBox";
            portBox.Size = new System.Drawing.Size(153, 21);
            portBox.TabIndex = 2;
            portBox.Text = Resource.IPList[ipInstanceName].GetModel().Port.ToString();
            // 
            // label6
            // 
            //label6.AutoSize = true;
            //label6.Location = new System.Drawing.Point(44, 232);
            //label6.Name = "label6";
            //label6.Size = new System.Drawing.Size(53, 12);
            //label6.TabIndex = 3;
            //label6.Text = Resource.lHelper.Key("n50");
            // 
            // netProtocolBox
            // 
            //netProtocolBox.FormattingEnabled = true;
            //netProtocolBox.Items.AddRange(new object[] {
            //"UDP"});
            //netProtocolBox.Location = new System.Drawing.Point(180, 230);
            //netProtocolBox.Name = "netProtocolBox";
            //netProtocolBox.Size = new System.Drawing.Size(153, 20);
            //netProtocolBox.TabIndex = 4;
            //netProtocolBox.SelectedIndex = 0;
            //netProtocolBox.DropDownStyle = ComboBoxStyle.DropDownList;
            //for (int i = 0; i < netProtocolBox.Items.Count; i++)
            //{
            //    if (((string)netProtocolBox.Items[i]).Equals(Resource.IPList[ipInstanceName].GetModel().NetProtocol.ToUpper()))
            //    {
            //        netProtocolBox.SelectedIndex = i;
            //        break;
            //    }

            //}


            // 
            // label7
            // (42, 307);
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(42, 196);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(53, 12);
            label7.TabIndex = 1;
            label7.Text = Resource.lHelper.Key("n52");
            // 
            // latOffsetBox
            // (180, 304);
            latOffsetBox.Location = new System.Drawing.Point(180, 193);
            latOffsetBox.Name = "latOffsetBox";
            latOffsetBox.Size = new System.Drawing.Size(153, 21);
            latOffsetBox.TabIndex = 2;
            latOffsetBox.Text = Resource.IPList[ipInstanceName].GetModel().LatOffset.ToString();
            // 
            // label8 经度偏移
            // (42, 271);
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(44, 232);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(53, 12);
            label8.TabIndex = 1;
            label8.Text = Resource.lHelper.Key("n51");
            // 
            // lonOffsetBox
            // (180, 268); 
            lonOffsetBox.Location = new System.Drawing.Point(180, 230);
            lonOffsetBox.Name = "lonOffsetBox";
            lonOffsetBox.Size = new System.Drawing.Size(153, 21);
            lonOffsetBox.TabIndex = 2;
            lonOffsetBox.Text = Resource.IPList[ipInstanceName].GetModel().LonOffset.ToString();
            // 
            // label10
            // (42, 344);
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(42, 260);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(53, 12);
            label10.TabIndex = 1;
            label10.Text = Resource.lHelper.Key("n24");
            // 
            // entityFilterBox
            // (180, 341);
            entityFilterBox.Location = new System.Drawing.Point(180, 260);
            entityFilterBox.Name = "entityFilterBox";
            entityFilterBox.Size = new System.Drawing.Size(153, 21);
            entityFilterBox.TabIndex = 2;

            string str = "";
            foreach (int s in Resource.IPList[ipInstanceName].GetModel().EntityID)
            {
                str = str + s.ToString() + ",";

            }
            if (str.IndexOf(",") >= 0)
            {
                entityFilterBox.Text = str.Remove(str.LastIndexOf(","));
            }
            else
            {
                entityFilterBox.Text = "";
            }
            entityFilterBox.Enabled = false;

            // 
            // instanceEnabled
            // (180, 377);
            instanceEnabled.AutoSize = true;
            instanceEnabled.Location = new System.Drawing.Point(180, 337);
            instanceEnabled.Name = "instanceEnabled";
            instanceEnabled.Size = new System.Drawing.Size(48, 16);
            instanceEnabled.TabIndex = 5;
            instanceEnabled.Text = Resource.lHelper.Key("n48");
            instanceEnabled.UseVisualStyleBackColor = true;
            if (Resource.IPList[ipInstanceName].GetModel().Enabled)
            {
                instanceEnabled.Checked = true;
            }
            else
            {
                instanceEnabled.Checked = false;
            }

            // 
            // entityFilterBtn
            // 
            entityFilterBtn.Location = new System.Drawing.Point(340, 260);
            entityFilterBtn.Name = "entityFilterBtn";
            entityFilterBtn.Size = new System.Drawing.Size(36, 23);
            entityFilterBtn.TabIndex = 6;
            entityFilterBtn.Text = "...";
            entityFilterBtn.UseVisualStyleBackColor = true;
            entityFilterBtn.Click += new EventHandler(SelectEntity);


            splitContainer1.Panel2.Controls.Add(entityFilterBtn);
            splitContainer1.Panel2.Controls.Add(instanceEnabled);
            //splitContainer1.Panel2.Controls.Add(netProtocolBox);
            //splitContainer1.Panel2.Controls.Add(protocolBox);
            //splitContainer1.Panel2.Controls.Add(label6);
            //splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(entityFilterBox);
            splitContainer1.Panel2.Controls.Add(label10);
            splitContainer1.Panel2.Controls.Add(lonOffsetBox);
            splitContainer1.Panel2.Controls.Add(label8);
            splitContainer1.Panel2.Controls.Add(ipBox);
            splitContainer1.Panel2.Controls.Add(latOffsetBox);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(portBox);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(InstanceNameBox);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(panelType);
            splitContainer1.Panel2.Controls.Add(oldInstanceName);
            splitContainer1.Panel2.Controls.Add(deleteBtn);
            splitContainer1.Panel2.Controls.Add(pluginNameBox);
            splitContainer1.Panel2.Controls.Add(protolChoose);


        }
        //数据库转发配置界面
        private void BindDBItem()
        {
            splitContainer1.Panel2.Controls.Clear();

            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.Label panelType;
            System.Windows.Forms.Button entityFilterBtn;
            System.Windows.Forms.CheckBox instanceEnabled;
            System.Windows.Forms.TextBox entityFilterBox;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.TextBox lonOffsetBox;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.TextBox tableNameBox;
            System.Windows.Forms.TextBox latOffsetBox;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.TextBox maxCountBox;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.TextBox InstanceNameBox;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.ComboBox pluginNameBox;
            System.Windows.Forms.Label protolChoose;
            protolChoose = new Label();
            pluginNameBox = new ComboBox();
            panelType = new System.Windows.Forms.Label();
            enterBtn = new System.Windows.Forms.Button();
            entityFilterBtn = new System.Windows.Forms.Button();
            instanceEnabled = new System.Windows.Forms.CheckBox();
            entityFilterBox = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            lonOffsetBox = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            tableNameBox = new System.Windows.Forms.TextBox();
            latOffsetBox = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            maxCountBox = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            InstanceNameBox = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();

            protolChoose.AutoSize = true;
            protolChoose.Location = new System.Drawing.Point(39, 55);
            protolChoose.Name = "protolChoose";
            protolChoose.Size = new System.Drawing.Size(77, 12);
            protolChoose.TabIndex = 1;
            protolChoose.Text = Resource.lHelper.Key("n55");
            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n39");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "dbTran";
            panelType.Visible = false;


            pluginNameBox.FormattingEnabled = true;
            pluginNameBox.Location = new System.Drawing.Point(180, 51);
            pluginNameBox.Name = "pluginNameBox";
            pluginNameBox.Size = new System.Drawing.Size(151, 20);
            pluginNameBox.TabIndex = 2;
            pluginNameBox.Items.Clear();
            pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;
            pluginNameBox.Items.Add("DB_Version_0");
            foreach (string s in Resource.DllList.Keys)
            {
                if (s.Contains("DB"))
                {
                    pluginNameBox.Items.Add(s);
                }
            }
            pluginNameBox.SelectedItem = "DB_Version_0";
            pluginNameBox.SelectedValueChanged += new EventHandler(pluginName_SelectedIndexChanged);

            // 
            // entityFilterBtn
            // 
            entityFilterBtn.Location = new System.Drawing.Point(340, 246);
            entityFilterBtn.Name = "entityFilterBtn";
            entityFilterBtn.Size = new System.Drawing.Size(36, 23);
            entityFilterBtn.TabIndex = 6;
            entityFilterBtn.Text = Resource.lHelper.Key("n60");
            entityFilterBtn.UseVisualStyleBackColor = true;
            entityFilterBtn.Click += new EventHandler(SelectEntity);

            // 
            // instanceEnabled
            // 

            instanceEnabled.AutoSize = true;
            instanceEnabled.Location = new System.Drawing.Point(180, 284);
            instanceEnabled.Name = "instanceEnabled";
            instanceEnabled.Size = new System.Drawing.Size(48, 16);
            instanceEnabled.TabIndex = 5;
            instanceEnabled.Text = Resource.lHelper.Key("n48");
            instanceEnabled.UseVisualStyleBackColor = true;
            // 
            // entityFilterBox
            // 
            entityFilterBox.Location = new System.Drawing.Point(180, 248);
            entityFilterBox.Name = "entityFilterBox";
            entityFilterBox.Size = new System.Drawing.Size(153, 21);
            entityFilterBox.TabIndex = 2;
            entityFilterBox.Enabled = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(42, 251);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(53, 12);
            label10.TabIndex = 1;
            label10.Text = Resource.lHelper.Key("n24");
            // 
            // lonOffsetBox
            // 
            lonOffsetBox.Location = new System.Drawing.Point(180, 150);
            lonOffsetBox.Name = "lonOffsetBox";
            lonOffsetBox.Size = new System.Drawing.Size(153, 21);
            lonOffsetBox.TabIndex = 2;
            // 
            // label8 经度偏移
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(42, 153);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(53, 12);
            label8.TabIndex = 1;
            label8.Text = Resource.lHelper.Key("n51");
            // 
            // tableNameBox
            // 
            tableNameBox.Location = new System.Drawing.Point(180, 115);
            tableNameBox.Name = "tableNameBox";
            tableNameBox.Size = new System.Drawing.Size(153, 21);
            tableNameBox.TabIndex = 2;
            // 
            // latOffsetBox
            // 
            latOffsetBox.Location = new System.Drawing.Point(180, 183);
            latOffsetBox.Name = "latOffsetBox";
            latOffsetBox.Size = new System.Drawing.Size(153, 21);
            latOffsetBox.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(42, 118);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(41, 12);
            label4.TabIndex = 1;
            label4.Text = Resource.lHelper.Key("n53");
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(42, 186);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(53, 12);
            label7.TabIndex = 1;
            label7.Text = Resource.lHelper.Key("n52");
            // 
            // maxCountBox
            // 
            maxCountBox.Location = new System.Drawing.Point(180, 215);
            maxCountBox.Name = "maxCountBox";
            maxCountBox.Size = new System.Drawing.Size(153, 21);
            maxCountBox.TabIndex = 2;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(42, 218);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(53, 12);
            label5.TabIndex = 1;
            label5.Text = Resource.lHelper.Key("n54");
            // 
            // InstanceNameBox
            // 
            InstanceNameBox.Location = new System.Drawing.Point(180, 80);
            InstanceNameBox.Name = "InstanceNameBox";
            InstanceNameBox.Size = new System.Drawing.Size(153, 21);
            InstanceNameBox.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(42, 83);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(53, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n49");
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(26, 21);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(113, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("m23");

            splitContainer1.Panel2.Controls.Add(entityFilterBtn);
            splitContainer1.Panel2.Controls.Add(instanceEnabled);
            splitContainer1.Panel2.Controls.Add(entityFilterBox);
            splitContainer1.Panel2.Controls.Add(label10);
            splitContainer1.Panel2.Controls.Add(lonOffsetBox);
            splitContainer1.Panel2.Controls.Add(label8);
            splitContainer1.Panel2.Controls.Add(tableNameBox);
            splitContainer1.Panel2.Controls.Add(latOffsetBox);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(maxCountBox);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(InstanceNameBox);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(panelType);
            splitContainer1.Panel2.Controls.Add(protolChoose);
            splitContainer1.Panel2.Controls.Add(pluginNameBox);



        }
        //数据库转发实例配置界面
        private void BindDBInstance()
        {
            splitContainer1.Panel2.Controls.Clear();

            string dbInstanceName = treeView1.SelectedNode.Tag.ToString();

            System.Windows.Forms.Button enterBtn;
            System.Windows.Forms.Label panelType;
            System.Windows.Forms.Button entityFilterBtn;
            System.Windows.Forms.CheckBox instanceEnabled;
            System.Windows.Forms.TextBox entityFilterBox;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.TextBox lonOffsetBox;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.TextBox tableNameBox;
            System.Windows.Forms.TextBox latOffsetBox;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.TextBox maxCountBox;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.TextBox InstanceNameBox;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label oldInstanceName;
            System.Windows.Forms.Button deleteBtn;
            System.Windows.Forms.ComboBox pluginNameBox;
            System.Windows.Forms.Label protolChoose;

            protolChoose = new Label();
            pluginNameBox = new ComboBox();
            deleteBtn = new Button();
            panelType = new System.Windows.Forms.Label();
            enterBtn = new System.Windows.Forms.Button();
            entityFilterBtn = new System.Windows.Forms.Button();
            instanceEnabled = new System.Windows.Forms.CheckBox();
            entityFilterBox = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            lonOffsetBox = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            tableNameBox = new System.Windows.Forms.TextBox();
            latOffsetBox = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            maxCountBox = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            InstanceNameBox = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            oldInstanceName = new Label();


            protolChoose.AutoSize = true;
            protolChoose.Location = new System.Drawing.Point(39, 55);
            protolChoose.Name = "protolChoose";
            protolChoose.Size = new System.Drawing.Size(77, 12);
            protolChoose.TabIndex = 1;
            protolChoose.Text = Resource.lHelper.Key("n55");

            pluginNameBox.FormattingEnabled = true;
            pluginNameBox.Location = new System.Drawing.Point(180, 51);
            pluginNameBox.Name = "pluginNameBox";
            pluginNameBox.Size = new System.Drawing.Size(151, 20);
            pluginNameBox.TabIndex = 2;
            pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;
            pluginNameBox.Items.Add("DB_Version_0");
            pluginNameBox.SelectedIndex = 0;
            pluginNameBox.Enabled = false;
            //
            //deleteBtn
            //
            deleteBtn.Location = new System.Drawing.Point(170, 496);
            deleteBtn.Name = "deleteBtn";
            deleteBtn.Size = new System.Drawing.Size(75, 23);
            deleteBtn.TabIndex = 2;
            deleteBtn.Text = Resource.lHelper.Key("n38");
            deleteBtn.UseVisualStyleBackColor = true;
            deleteBtn.Click += new EventHandler(DeleteBtn_Click);


            // 
            // enterBtn
            // 
            enterBtn.Location = new System.Drawing.Point(250, 496);
            enterBtn.Name = "enterBtn";
            enterBtn.Size = new System.Drawing.Size(75, 23);
            enterBtn.TabIndex = 2;
            enterBtn.Text = Resource.lHelper.Key("n37");
            enterBtn.UseVisualStyleBackColor = true;
            enterBtn.Click += new EventHandler(enterBtn_Click);

            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "dbTranInstance";
            panelType.Visible = false;

            //
            // oldInstanceName
            //
            oldInstanceName.AutoSize = true;
            oldInstanceName.Name = "oldInstanceName";
            oldInstanceName.Location = new System.Drawing.Point(96, 122);
            oldInstanceName.Size = new System.Drawing.Size(41, 12);
            oldInstanceName.Text = dbInstanceName;
            oldInstanceName.Visible = false;
            // 
            // entityFilterBtn
            // 
            entityFilterBtn.Location = new System.Drawing.Point(340, 246);
            entityFilterBtn.Name = "entityFilterBtn";
            entityFilterBtn.Size = new System.Drawing.Size(36, 23);
            entityFilterBtn.TabIndex = 6;
            entityFilterBtn.Text = "...";
            entityFilterBtn.UseVisualStyleBackColor = true;
            entityFilterBtn.Click += new EventHandler(SelectEntity);

            // 
            // InstanceNameBox
            // 
            InstanceNameBox.Location = new System.Drawing.Point(180, 80);
            InstanceNameBox.Name = "InstanceNameBox";
            InstanceNameBox.Size = new System.Drawing.Size(153, 21);
            InstanceNameBox.TabIndex = 2;
            InstanceNameBox.Text = dbInstanceName;


            // 
            // instanceEnabled
            // 
            instanceEnabled.AutoSize = true;
            instanceEnabled.Location = new System.Drawing.Point(180, 284);
            instanceEnabled.Name = "instanceEnabled";
            instanceEnabled.Size = new System.Drawing.Size(48, 16);
            instanceEnabled.TabIndex = 5;
            instanceEnabled.Text = Resource.lHelper.Key("n48");
            instanceEnabled.UseVisualStyleBackColor = true;
            if (Resource.DBList[dbInstanceName].GetModel().Enabled)
            {
                instanceEnabled.Checked = true;
            }
            else
            {
                instanceEnabled.Checked = false;
            }


            // 
            // entityFilterBox
            // 
            entityFilterBox.Location = new System.Drawing.Point(180, 248);
            entityFilterBox.Name = "entityFilterBox";
            entityFilterBox.Size = new System.Drawing.Size(153, 21);
            entityFilterBox.TabIndex = 2;

            string str = "";
            foreach (int s in Resource.DBList[dbInstanceName].GetModel().EntityID)
            {
                str = str + s.ToString() + ",";

            }
            if (str.IndexOf(",") >= 0)
            {
                entityFilterBox.Text = str.Remove(str.LastIndexOf(","));
            }
            else
            {
                entityFilterBox.Text = "";
            }
            entityFilterBox.Enabled = false;

            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(42, 251);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(53, 12);
            label10.TabIndex = 1;
            label10.Text = Resource.lHelper.Key("n24");
            // 
            // lonOffsetBox
            // 
            lonOffsetBox.Location = new System.Drawing.Point(180, 150);
            lonOffsetBox.Name = "lonOffsetBox";
            lonOffsetBox.Size = new System.Drawing.Size(153, 21);
            lonOffsetBox.TabIndex = 2;
            lonOffsetBox.Text = Resource.DBList[dbInstanceName].GetModel().LonOffset.ToString();
            // 
            // label8 经度偏移
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(42, 153);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(53, 12);
            label8.TabIndex = 1;
            label8.Text = Resource.lHelper.Key("n51");
            // 
            // tableNameBox
            // 
            tableNameBox.Location = new System.Drawing.Point(180, 115);
            tableNameBox.Name = "tableNameBox";
            tableNameBox.Size = new System.Drawing.Size(153, 21);
            tableNameBox.TabIndex = 2;
            tableNameBox.Text = Resource.DBList[dbInstanceName].GetModel().TableName;
            // 
            // latOffsetBox
            // 
            latOffsetBox.Location = new System.Drawing.Point(180, 183);
            latOffsetBox.Name = "latOffsetBox";
            latOffsetBox.Size = new System.Drawing.Size(153, 21);
            latOffsetBox.TabIndex = 2;
            latOffsetBox.Text = Resource.DBList[dbInstanceName].GetModel().LatOffset.ToString();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(42, 118);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(41, 12);
            label4.TabIndex = 1;
            label4.Text = Resource.lHelper.Key("n53");
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(42, 186);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(53, 12);
            label7.TabIndex = 1;
            label7.Text = Resource.lHelper.Key("n52");
            // 
            // maxCountBox
            // 
            maxCountBox.Location = new System.Drawing.Point(180, 215);
            maxCountBox.Name = "maxCountBox";
            maxCountBox.Size = new System.Drawing.Size(153, 21);
            maxCountBox.TabIndex = 2;
            maxCountBox.Text = Resource.DBList[dbInstanceName].GetModel().MaxCount.ToString();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(42, 218);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(53, 12);
            label5.TabIndex = 1;
            label5.Text = Resource.lHelper.Key("n54");

            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(42, 83);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(53, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n49");
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(26, 21);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(113, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("m24");

            splitContainer1.Panel2.Controls.Add(entityFilterBtn);
            splitContainer1.Panel2.Controls.Add(instanceEnabled);
            splitContainer1.Panel2.Controls.Add(entityFilterBox);
            splitContainer1.Panel2.Controls.Add(label10);
            splitContainer1.Panel2.Controls.Add(lonOffsetBox);
            splitContainer1.Panel2.Controls.Add(label8);
            splitContainer1.Panel2.Controls.Add(tableNameBox);
            splitContainer1.Panel2.Controls.Add(latOffsetBox);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(maxCountBox);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(InstanceNameBox);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(enterBtn);
            splitContainer1.Panel2.Controls.Add(panelType);
            splitContainer1.Panel2.Controls.Add(oldInstanceName);
            splitContainer1.Panel2.Controls.Add(deleteBtn);
            splitContainer1.Panel2.Controls.Add(pluginNameBox);
            splitContainer1.Panel2.Controls.Add(protolChoose);


        }

        //插件配置界面 
        private void BindPlugin(TreeViewEventArgs e)
        {

            /*  if (Resource.DllList.Count <= 0) 
              {
                  MessageBox.Show( Resource.lHelper.Key("m25"));
                
                  treeView1.SelectedNode = treeView1.Nodes[0];
                  return;
              }
             * */
            //注释掉该部分代码，因为协议至少有一种。

            splitContainer1.Panel2.Controls.Clear();
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.ComboBox pluginNameBox;
            System.Windows.Forms.Label panelType;

            panelType = new Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            pluginNameBox = new System.Windows.Forms.ComboBox();


            panelType.AutoSize = true;
            panelType.Location = new System.Drawing.Point(96, 122);
            panelType.Name = "panelType";
            panelType.Size = new System.Drawing.Size(41, 12);
            panelType.TabIndex = 0;
            panelType.Text = "pluginTran";
            panelType.Visible = false;


            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(23, 14);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(101, 12);
            label1.TabIndex = 0;
            label1.Text = Resource.lHelper.Key("m26");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(39, 55);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(77, 12);
            label2.TabIndex = 1;
            label2.Text = Resource.lHelper.Key("n55");
            // 
            // pluginNameBox
            // 
            pluginNameBox.FormattingEnabled = true;
            pluginNameBox.Location = new System.Drawing.Point(180, 51);
            pluginNameBox.Name = "pluginNameBox";
            pluginNameBox.Size = new System.Drawing.Size(151, 20);
            pluginNameBox.TabIndex = 2;
            pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;

            pluginNameBox.Items.Clear();
            if (e.Node.Tag.Equals("ipTran"))
            {
                pluginNameBox.Items.Add("UDP_Version_0");
                foreach (string s in Resource.DllList.Keys)
                {
                    if (s.Contains("UDP") || s.Contains("TCP"))
                    {
                        pluginNameBox.Items.Add(s);
                    }
                }
            }
            else
            {
                pluginNameBox.Items.Add("DB_Version_0");
                foreach (string s in Resource.DllList.Keys)
                {
                    if (s.Contains("DB"))
                    {
                        pluginNameBox.Items.Add(s);
                    }
                }

            }
            pluginNameBox.SelectedIndex = 0;
            pluginNameBox.SelectedValueChanged += new EventHandler(pluginName_SelectedIndexChanged);
            splitContainer1.Panel2.Controls.Add(pluginNameBox);
            splitContainer1.Panel2.Controls.Add(label2);
            // splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(panelType);
        }
        //插件实例配置界面
        private void BindPluginInstance()
        {
            string pluginInstanceName = treeView1.SelectedNode.Tag.ToString();
            if (Resource.PluginList[pluginInstanceName].GetPluginModel().DllModel.PType == PluginType.RemoteIP)
            {

                splitContainer1.Panel2.Controls.Clear();
                System.Windows.Forms.ComboBox pluginNameBox;
                System.Windows.Forms.Label label2;
                System.Windows.Forms.Label label1;
                System.Windows.Forms.TextBox instanceNameBox;
                System.Windows.Forms.Label label3;
                //System.Windows.Forms.ComboBox protocolBox;
                //System.Windows.Forms.Label label4;
                System.Windows.Forms.TextBox portBox;
                System.Windows.Forms.Label label7;
                System.Windows.Forms.TextBox ipBox;
                System.Windows.Forms.Label label5;
                //System.Windows.Forms.ComboBox netProtocolBox;
                //System.Windows.Forms.Label label6;
                System.Windows.Forms.TextBox latOffsetBox;
                System.Windows.Forms.Label label9;
                System.Windows.Forms.TextBox lonOffsetBox;
                System.Windows.Forms.Label lonOffset;
                System.Windows.Forms.CheckBox instanceEnabled;
                System.Windows.Forms.Button entityFilterBtn;
                System.Windows.Forms.TextBox entityFilterBox;
                System.Windows.Forms.Label label8;
                System.Windows.Forms.Button enterBtn;
                System.Windows.Forms.Label panelType;
                System.Windows.Forms.Label oldInstanceName;
                System.Windows.Forms.Button deleteBtn;

                deleteBtn = new Button();
                oldInstanceName = new Label();
                panelType = new System.Windows.Forms.Label();
                enterBtn = new System.Windows.Forms.Button();
                label1 = new System.Windows.Forms.Label();
                label2 = new System.Windows.Forms.Label();
                pluginNameBox = new System.Windows.Forms.ComboBox();
                label3 = new System.Windows.Forms.Label();
                instanceNameBox = new System.Windows.Forms.TextBox();
                //label4 = new System.Windows.Forms.Label();
                //protocolBox = new System.Windows.Forms.ComboBox();
                label5 = new System.Windows.Forms.Label();
                ipBox = new System.Windows.Forms.TextBox();
                label7 = new System.Windows.Forms.Label();
                portBox = new System.Windows.Forms.TextBox();
                //label6 = new System.Windows.Forms.Label();
                //netProtocolBox = new System.Windows.Forms.ComboBox();
                lonOffset = new System.Windows.Forms.Label();
                lonOffsetBox = new System.Windows.Forms.TextBox();
                label9 = new System.Windows.Forms.Label();
                latOffsetBox = new System.Windows.Forms.TextBox();
                label8 = new System.Windows.Forms.Label();
                entityFilterBox = new System.Windows.Forms.TextBox();
                entityFilterBtn = new System.Windows.Forms.Button();
                instanceEnabled = new System.Windows.Forms.CheckBox();

                //
                //deleteBtn
                //
                deleteBtn.Location = new System.Drawing.Point(170, 496);
                deleteBtn.Name = "deleteBtn";
                deleteBtn.Size = new System.Drawing.Size(75, 23);
                deleteBtn.TabIndex = 2;
                deleteBtn.Text = Resource.lHelper.Key("n38");
                deleteBtn.UseVisualStyleBackColor = true;
                deleteBtn.Click += new EventHandler(DeleteBtn_Click);
                // 
                // enterBtn
                // 
                enterBtn.Location = new System.Drawing.Point(250, 496);
                enterBtn.Name = "enterBtn";
                enterBtn.Size = new System.Drawing.Size(75, 23);
                enterBtn.TabIndex = 2;
                enterBtn.Text = Resource.lHelper.Key("n37");
                enterBtn.UseVisualStyleBackColor = true;
                enterBtn.Click += new EventHandler(enterBtn_Click);

                panelType.AutoSize = true;
                panelType.Location = new System.Drawing.Point(96, 122);
                panelType.Name = "panelType";
                panelType.Size = new System.Drawing.Size(41, 12);
                panelType.TabIndex = 0;
                panelType.Text = "pluginInstance";
                panelType.Visible = false;

                oldInstanceName.AutoSize = true;
                oldInstanceName.Location = new System.Drawing.Point(96, 122);
                oldInstanceName.Name = "oldInstanceName";
                oldInstanceName.Size = new System.Drawing.Size(41, 12);
                oldInstanceName.TabIndex = 0;
                oldInstanceName.Text = pluginInstanceName;
                oldInstanceName.Visible = false;

                // 
                // label1
                // 
                label1.AutoSize = true;
                label1.Location = new System.Drawing.Point(23, 14);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(101, 12);
                label1.TabIndex = 0;
                label1.Text = Resource.lHelper.Key("m22");  //han modify
                // 
                // label2
                // 
                label2.AutoSize = true;
                label2.Location = new System.Drawing.Point(39, 55);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(77, 12);
                label2.TabIndex = 1;
                label2.Text = Resource.lHelper.Key("n55");
                // 
                // pluginNameBox
                // 
                pluginNameBox.FormattingEnabled = true;
                pluginNameBox.Location = new System.Drawing.Point(180, 51);
                pluginNameBox.Name = "pluginNameBox";
                pluginNameBox.Size = new System.Drawing.Size(151, 20);
                pluginNameBox.TabIndex = 2;
                pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;
                pluginNameBox.Items.Add(Resource.PluginList[pluginInstanceName].GetPluginModel().DllModel.Name);
                pluginNameBox.SelectedIndex = 0;
                pluginNameBox.Enabled = false;

                // 
                // label3
                // 
                label3.AutoSize = true;
                label3.Location = new System.Drawing.Point(41, 90);
                label3.Name = "label3";
                label3.Size = new System.Drawing.Size(53, 12);
                label3.TabIndex = 3;
                label3.Text = Resource.lHelper.Key("n49");
                // 
                // instanceNameBox
                // 
                instanceNameBox.Location = new System.Drawing.Point(180, 84);
                instanceNameBox.Name = "instanceNameBox";
                instanceNameBox.Size = new System.Drawing.Size(151, 21);
                instanceNameBox.TabIndex = 4;
                instanceNameBox.Text = pluginInstanceName;
                // 
                // protocolBox
                // 
                //label4.AutoSize = true;
                //label4.Location = new System.Drawing.Point(39, 120);
                //label4.Name = "label4";
                //label4.Size = new System.Drawing.Size(53, 12);
                //label4.TabIndex = 1;
                //label4.Text = Resource.lHelper.Key("n18");
                // 
                // protocolBox
                // 
                //protocolBox.FormattingEnabled = true;
                //protocolBox.Location = new System.Drawing.Point(180, 116);
                //protocolBox.Name = "protocolBox";
                //protocolBox.Size = new System.Drawing.Size(151, 20);
                //protocolBox.TabIndex = 2;
                //protocolBox.Items.AddRange(XMLIni.ProtocolList);
                //protocolBox.DropDownStyle = ComboBoxStyle.DropDownList;
                //protocolBox.SelectedItem = Resource.PluginList[pluginInstanceName].GetPluginModel().Protocol;
                // 
                // label5
                // (41, 151);
                label5.AutoSize = true;
                label5.Location = new System.Drawing.Point(39, 120);
                label5.Name = "label5";
                label5.Size = new System.Drawing.Size(17, 12);
                label5.TabIndex = 3;
                label5.Text = Resource.lHelper.Key("n46");
                // 
                // ipBox
                // (180, 145);
                ipBox.Location = new System.Drawing.Point(180, 116);
                ipBox.Name = "ipBox";
                ipBox.Size = new System.Drawing.Size(151, 21);
                ipBox.TabIndex = 4;
                ipBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().Ip;
                // 
                // label7
                // (41, 182);
                label7.AutoSize = true;
                label7.Location = new System.Drawing.Point(41, 151);
                label7.Name = "label7";
                label7.Size = new System.Drawing.Size(29, 12);
                label7.TabIndex = 3;
                label7.Text = Resource.lHelper.Key("n12");
                // 
                // portBox
                // (180, 176);
                portBox.Location = new System.Drawing.Point(180, 145);
                portBox.Name = "portBox";
                portBox.Size = new System.Drawing.Size(151, 21);
                portBox.TabIndex = 4;
                portBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().Port.ToString();
                // 
                // label6
                // 
                //label6.AutoSize = true;
                //label6.Location = new System.Drawing.Point(39, 211);
                //label6.Name = "label6";
                //label6.Size = new System.Drawing.Size(53, 12);
                //label6.TabIndex = 1;
                //label6.Text = Resource.lHelper.Key("n50");
                // 
                // netProtocolBox
                // 
                //netProtocolBox.FormattingEnabled = true;
                //netProtocolBox.Location = new System.Drawing.Point(180, 207);
                //netProtocolBox.Name = "netProtocolBox";
                //netProtocolBox.Size = new System.Drawing.Size(151, 20);
                //netProtocolBox.TabIndex = 2;
                //netProtocolBox.Items.AddRange(XMLIni.NetProtocolList);
                //netProtocolBox.DropDownStyle = ComboBoxStyle.DropDownList;
                //netProtocolBox.SelectedItem = Resource.PluginList[pluginInstanceName].GetPluginModel().NetProtocol;
                // 
                // lonOffset 经度偏移
                // (41, 246);
                lonOffset.AutoSize = true;
                lonOffset.Location = new System.Drawing.Point(41, 182);
                lonOffset.Name = "lonOffset";
                lonOffset.Size = new System.Drawing.Size(53, 12);
                lonOffset.TabIndex = 3;
                lonOffset.Text = Resource.lHelper.Key("n51");

                // 
                // lonOffsetBox
                // (180, 240);
                lonOffsetBox.Location = new System.Drawing.Point(180, 176);
                lonOffsetBox.Name = "lonOffsetBox";
                lonOffsetBox.Size = new System.Drawing.Size(151, 21);
                lonOffsetBox.TabIndex = 4;
                lonOffsetBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().LonOffset.ToString();
                // 
                // label9
                // (41, 278);
                label9.AutoSize = true;
                label9.Location = new System.Drawing.Point(39, 211);
                label9.Name = "label9";
                label9.Size = new System.Drawing.Size(53, 12);
                label9.TabIndex = 3;
                label9.Text = Resource.lHelper.Key("n52");
                // 
                // latOffsetBox
                // (180, 272);
                latOffsetBox.Location = new System.Drawing.Point(180, 207);
                latOffsetBox.Name = "latOffsetBox";
                latOffsetBox.Size = new System.Drawing.Size(151, 21);
                latOffsetBox.TabIndex = 4;
                latOffsetBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().LatOffset.ToString();
                // 
                // label8
                // (41, 313);
                label8.AutoSize = true;
                label8.Location = new System.Drawing.Point(41, 246);
                label8.Name = "label8";
                label8.Size = new System.Drawing.Size(53, 12);
                label8.TabIndex = 3;
                label8.Text = Resource.lHelper.Key("n24");
                // 
                // entityFilterBox
                // (180, 307);
                entityFilterBox.Location = new System.Drawing.Point(180, 240);
                entityFilterBox.Name = "entityFilterBox";
                entityFilterBox.Size = new System.Drawing.Size(151, 21);
                entityFilterBox.TabIndex = 4;
                entityFilterBox.Enabled = true;
                string str = "";
                foreach (int s in Resource.PluginList[pluginInstanceName].GetPluginModel().EntityID)
                {
                    str = str + s + ",";

                }
                if (str.IndexOf(",") >= 0)
                {
                    entityFilterBox.Text = str.Remove(str.LastIndexOf(","));
                }
                else
                {
                    entityFilterBox.Text = "";
                }
                entityFilterBox.Enabled = false;
                // 
                // entityFilterBtn
                // (340, 305);
                entityFilterBtn.Location = new System.Drawing.Point(340, 240);
                entityFilterBtn.Name = "entityFilterBtn";
                entityFilterBtn.Size = new System.Drawing.Size(37, 23);
                entityFilterBtn.TabIndex = 5;
                entityFilterBtn.Text = "...";
                entityFilterBtn.UseVisualStyleBackColor = true;
                entityFilterBtn.Click += new EventHandler(SelectEntity);
                // 
                // instanceEnabled
                // (180, 345);
                instanceEnabled.AutoSize = true;
                instanceEnabled.Location = new System.Drawing.Point(180, 270);
                instanceEnabled.Name = "instanceEnabled";
                instanceEnabled.Size = new System.Drawing.Size(48, 16);
                instanceEnabled.TabIndex = 6;
                instanceEnabled.Text = Resource.lHelper.Key("n48");
                instanceEnabled.UseVisualStyleBackColor = true;

                if (Resource.PluginList[pluginInstanceName].GetPluginModel().Enabled)
                {
                    instanceEnabled.Checked = true;
                }
                else
                {
                    instanceEnabled.Checked = false;

                }

                splitContainer1.Panel2.Controls.Add(oldInstanceName);
                splitContainer1.Panel2.Controls.Add(instanceEnabled);
                splitContainer1.Panel2.Controls.Add(entityFilterBtn);
                splitContainer1.Panel2.Controls.Add(entityFilterBox);
                splitContainer1.Panel2.Controls.Add(latOffsetBox);
                splitContainer1.Panel2.Controls.Add(label8);
                splitContainer1.Panel2.Controls.Add(portBox);
                splitContainer1.Panel2.Controls.Add(label9);
                splitContainer1.Panel2.Controls.Add(label7);
                splitContainer1.Panel2.Controls.Add(lonOffsetBox);
                splitContainer1.Panel2.Controls.Add(lonOffset);
                splitContainer1.Panel2.Controls.Add(ipBox);
                splitContainer1.Panel2.Controls.Add(label5);
                splitContainer1.Panel2.Controls.Add(instanceNameBox);
                splitContainer1.Panel2.Controls.Add(label3);
                //splitContainer1.Panel2.Controls.Add(netProtocolBox);
                //splitContainer1.Panel2.Controls.Add(label6);
                //splitContainer1.Panel2.Controls.Add(protocolBox);
                //splitContainer1.Panel2.Controls.Add(label4);
                splitContainer1.Panel2.Controls.Add(pluginNameBox);
                splitContainer1.Panel2.Controls.Add(label2);
                splitContainer1.Panel2.Controls.Add(label1);

                splitContainer1.Panel2.Controls.Add(enterBtn);
                splitContainer1.Panel2.Controls.Add(panelType);
                splitContainer1.Panel2.Controls.Add(deleteBtn);


            }
            else if (Resource.PluginList[pluginInstanceName].GetPluginModel().DllModel.PType == PluginType.RemoteDB)
            {
                splitContainer1.Panel2.Controls.Clear();

                System.Windows.Forms.ComboBox pluginNameBox;
                System.Windows.Forms.Label label2;
                System.Windows.Forms.Label label1;
                System.Windows.Forms.TextBox instanceNameBox;
                System.Windows.Forms.Label label3;
                System.Windows.Forms.TextBox latOffsetBox;
                System.Windows.Forms.Label label9;
                System.Windows.Forms.TextBox lonOffsetBox;
                System.Windows.Forms.Label lonOffset;
                System.Windows.Forms.CheckBox instanceEnabled;
                System.Windows.Forms.Button entityFilterBtn;
                System.Windows.Forms.TextBox entityFilterBox;
                System.Windows.Forms.Label label8;
                System.Windows.Forms.Label label5;
                System.Windows.Forms.Label label4;
                System.Windows.Forms.TextBox maxCountBox;
                System.Windows.Forms.TextBox tableNameBox;
                System.Windows.Forms.TextBox remoteInstanceBox;
                System.Windows.Forms.TextBox remotePwdBox;
                System.Windows.Forms.TextBox remoteCatalogBox;
                System.Windows.Forms.Label label13;
                System.Windows.Forms.Label label11;
                System.Windows.Forms.Label label7;
                System.Windows.Forms.TextBox remotePortBox;
                System.Windows.Forms.Label label12;
                System.Windows.Forms.TextBox remoteUserBox;
                System.Windows.Forms.Label label10;
                System.Windows.Forms.TextBox remoteIPBox;
                System.Windows.Forms.Label label6;
                System.Windows.Forms.Button testConnBtn;
                System.Windows.Forms.Button enterBtn;
                System.Windows.Forms.Label panelType;
                System.Windows.Forms.Label oldInstanceName;
                System.Windows.Forms.Button deleteBtn;

                deleteBtn = new Button();
                oldInstanceName = new Label();
                panelType = new System.Windows.Forms.Label();
                enterBtn = new System.Windows.Forms.Button();
                instanceEnabled = new System.Windows.Forms.CheckBox();
                entityFilterBtn = new System.Windows.Forms.Button();
                entityFilterBox = new System.Windows.Forms.TextBox();
                latOffsetBox = new System.Windows.Forms.TextBox();
                label8 = new System.Windows.Forms.Label();
                label9 = new System.Windows.Forms.Label();
                lonOffsetBox = new System.Windows.Forms.TextBox();
                lonOffset = new System.Windows.Forms.Label();
                instanceNameBox = new System.Windows.Forms.TextBox();
                label3 = new System.Windows.Forms.Label();
                pluginNameBox = new System.Windows.Forms.ComboBox();
                label2 = new System.Windows.Forms.Label();
                label1 = new System.Windows.Forms.Label();
                label4 = new System.Windows.Forms.Label();
                tableNameBox = new System.Windows.Forms.TextBox();
                maxCountBox = new System.Windows.Forms.TextBox();
                label5 = new System.Windows.Forms.Label();
                label6 = new System.Windows.Forms.Label();
                remoteIPBox = new System.Windows.Forms.TextBox();
                label7 = new System.Windows.Forms.Label();
                remoteCatalogBox = new System.Windows.Forms.TextBox();
                label10 = new System.Windows.Forms.Label();
                remoteUserBox = new System.Windows.Forms.TextBox();
                label11 = new System.Windows.Forms.Label();
                remotePwdBox = new System.Windows.Forms.TextBox();
                label12 = new System.Windows.Forms.Label();
                remotePortBox = new System.Windows.Forms.TextBox();
                label13 = new System.Windows.Forms.Label();
                remoteInstanceBox = new System.Windows.Forms.TextBox();
                testConnBtn = new System.Windows.Forms.Button();

                //
                //deleteBtn
                //
                deleteBtn.Location = new System.Drawing.Point(170, 496);
                deleteBtn.Name = "deleteBtn";
                deleteBtn.Size = new System.Drawing.Size(75, 23);
                deleteBtn.TabIndex = 2;
                deleteBtn.Text = Resource.lHelper.Key("n38");
                deleteBtn.UseVisualStyleBackColor = true;
                deleteBtn.Click += new EventHandler(DeleteBtn_Click);
                // 
                // enterBtn
                // 
                enterBtn.Location = new System.Drawing.Point(250, 496);
                enterBtn.Name = "enterBtn";
                enterBtn.Size = new System.Drawing.Size(75, 23);
                enterBtn.TabIndex = 2;
                enterBtn.Text = Resource.lHelper.Key("n37");
                enterBtn.UseVisualStyleBackColor = true;
                enterBtn.Click += new EventHandler(enterBtn_Click);

                panelType.AutoSize = true;
                panelType.Location = new System.Drawing.Point(96, 122);
                panelType.Name = "panelType";
                panelType.Size = new System.Drawing.Size(41, 12);
                panelType.TabIndex = 0;
                panelType.Text = "pluginInstance";
                panelType.Visible = false;

                oldInstanceName.AutoSize = true;
                oldInstanceName.Location = new System.Drawing.Point(96, 122);
                oldInstanceName.Name = "oldInstanceName";
                oldInstanceName.Size = new System.Drawing.Size(41, 12);
                oldInstanceName.TabIndex = 0;
                oldInstanceName.Text = pluginInstanceName;
                oldInstanceName.Visible = false;


                // 
                // instanceEnabled
                // 
                instanceEnabled.AutoSize = true;
                instanceEnabled.Location = new System.Drawing.Point(180, 422);
                instanceEnabled.Name = "instanceEnabled";
                instanceEnabled.Size = new System.Drawing.Size(48, 16);
                instanceEnabled.TabIndex = 6;
                instanceEnabled.Text = Resource.lHelper.Key("n48");
                instanceEnabled.UseVisualStyleBackColor = true;

                if (Resource.PluginList[pluginInstanceName].GetPluginModel().Enabled)
                {
                    instanceEnabled.Checked = true;
                }
                else
                {
                    instanceEnabled.Checked = false;

                }

                // 
                // entityFilterBtn
                // 
                entityFilterBtn.Location = new System.Drawing.Point(340, 221);
                entityFilterBtn.Name = "entityFilterBtn";
                entityFilterBtn.Size = new System.Drawing.Size(37, 23);
                entityFilterBtn.TabIndex = 5;
                entityFilterBtn.Text = "...";
                entityFilterBtn.UseVisualStyleBackColor = true;
                entityFilterBtn.Click += new EventHandler(SelectEntity);
                // 
                // entityFilterBox
                // 
                entityFilterBox.Location = new System.Drawing.Point(180, 223);
                entityFilterBox.Name = "entityFilterBox";
                entityFilterBox.Size = new System.Drawing.Size(151, 21);
                entityFilterBox.TabIndex = 4;
                entityFilterBox.Enabled = true;
                string str = "";
                foreach (int s in Resource.PluginList[pluginInstanceName].GetPluginModel().EntityID)
                {
                    str = str + s.ToString() + ",";

                }
                if (str.IndexOf(",") >= 0)
                {
                    entityFilterBox.Text = str.Remove(str.LastIndexOf(","));
                }
                else
                {
                    entityFilterBox.Text = "";
                }
                entityFilterBox.Enabled = false;
                // 
                // latOffsetBox
                // 
                latOffsetBox.Location = new System.Drawing.Point(180, 193);
                latOffsetBox.Name = "latOffsetBox";
                latOffsetBox.Size = new System.Drawing.Size(151, 21);
                latOffsetBox.TabIndex = 4;
                latOffsetBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().LatOffset.ToString();
                // 
                // label8
                // 
                label8.AutoSize = true;
                label8.Location = new System.Drawing.Point(41, 228);
                label8.Name = "label8";
                label8.Size = new System.Drawing.Size(53, 12);
                label8.TabIndex = 3;
                label8.Text = Resource.lHelper.Key("n24");
                // 
                // label9
                // 
                label9.AutoSize = true;
                label9.Location = new System.Drawing.Point(41, 200);
                label9.Name = "label9";
                label9.Size = new System.Drawing.Size(53, 12);
                label9.TabIndex = 3;
                label9.Text = Resource.lHelper.Key("n52");
                // 
                // lonOffsetBox
                // 
                lonOffsetBox.Location = new System.Drawing.Point(180, 164);
                lonOffsetBox.Name = "lonOffsetBox";
                lonOffsetBox.Size = new System.Drawing.Size(151, 21);
                lonOffsetBox.TabIndex = 4;
                lonOffsetBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().LonOffset.ToString();
                // 
                // lonOffset 经度偏移
                // 
                lonOffset.AutoSize = true;
                lonOffset.Location = new System.Drawing.Point(41, 170);
                lonOffset.Name = "lonOffset";
                lonOffset.Size = new System.Drawing.Size(53, 12);
                lonOffset.TabIndex = 3;
                lonOffset.Text = Resource.lHelper.Key("n51");
                // 
                // instanceNameBox
                // 
                instanceNameBox.Location = new System.Drawing.Point(180, 79);
                instanceNameBox.Name = "instanceNameBox";
                instanceNameBox.Size = new System.Drawing.Size(151, 21);
                instanceNameBox.TabIndex = 4;
                instanceNameBox.Text = pluginInstanceName;
                // 
                // label3
                // 
                label3.AutoSize = true;
                label3.Location = new System.Drawing.Point(41, 85);
                label3.Name = "label3";
                label3.Size = new System.Drawing.Size(53, 12);
                label3.TabIndex = 3;
                label3.Text = Resource.lHelper.Key("n49");
                // 
                // pluginNameBox
                // 
                pluginNameBox.FormattingEnabled = true;
                pluginNameBox.Location = new System.Drawing.Point(180, 51);
                pluginNameBox.Name = "pluginNameBox";
                pluginNameBox.Size = new System.Drawing.Size(151, 20);
                pluginNameBox.TabIndex = 2;
                pluginNameBox.Items.Clear();
                pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;
                pluginNameBox.Items.Add(Resource.PluginList[pluginInstanceName].GetPluginModel().DllModel.Name);
                pluginNameBox.SelectedIndex = 0;
                pluginNameBox.Enabled = false;



                // 
                // label2
                // 
                label2.AutoSize = true;
                label2.Location = new System.Drawing.Point(39, 55);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(77, 12);
                label2.TabIndex = 1;
                label2.Text = Resource.lHelper.Key("n55");
                // 
                // label1
                // 
                label1.AutoSize = true;
                label1.Location = new System.Drawing.Point(23, 14);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(101, 12);
                label1.TabIndex = 0;
                label1.Text = Resource.lHelper.Key("m24");   //han modify
                // 
                // label4
                // 
                label4.AutoSize = true;
                label4.Location = new System.Drawing.Point(43, 112);
                label4.Name = "label4";
                label4.Size = new System.Drawing.Size(41, 12);
                label4.TabIndex = 7;
                label4.Text = Resource.lHelper.Key("n53");
                // 
                // tableNameBox
                // 
                tableNameBox.Location = new System.Drawing.Point(180, 107);
                tableNameBox.Name = "tableNameBox";
                tableNameBox.Size = new System.Drawing.Size(151, 21);
                tableNameBox.TabIndex = 4;
                tableNameBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().TableName;
                // 
                // maxCountBox
                // 
                maxCountBox.Location = new System.Drawing.Point(180, 135);
                maxCountBox.Name = "maxCountBox";
                maxCountBox.Size = new System.Drawing.Size(151, 21);
                maxCountBox.TabIndex = 4;
                maxCountBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().MaxCount.ToString();
                // 
                // label5
                // 
                label5.AutoSize = true;
                label5.Location = new System.Drawing.Point(43, 140);
                label5.Name = "label5";
                label5.Size = new System.Drawing.Size(65, 12);
                label5.TabIndex = 7;
                label5.Text = Resource.lHelper.Key("n54");
                // 
                // label6
                // 
                label6.AutoSize = true;
                label6.Location = new System.Drawing.Point(42, 252);
                label6.Name = "label6";
                label6.Size = new System.Drawing.Size(53, 12);
                label6.TabIndex = 8;
                label6.Text = Resource.lHelper.Key("n56");
                // 
                // remoteIPBox
                // 
                remoteIPBox.Location = new System.Drawing.Point(180, 252);
                remoteIPBox.Name = "remoteIPBox";
                remoteIPBox.Size = new System.Drawing.Size(151, 21);
                remoteIPBox.TabIndex = 9;
                remoteIPBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().DesIP;
                // 
                // label7
                // 
                label7.AutoSize = true;
                label7.Location = new System.Drawing.Point(42, 279);
                label7.Name = "label7";
                label7.Size = new System.Drawing.Size(41, 12);
                label7.TabIndex = 8;
                label7.Text = Resource.lHelper.Key("n45");
                // 
                // remoteCatalogBox
                // 
                remoteCatalogBox.Location = new System.Drawing.Point(180, 279);
                remoteCatalogBox.Name = "remoteCatalogBox";
                remoteCatalogBox.Size = new System.Drawing.Size(151, 21);
                remoteCatalogBox.TabIndex = 9;
                remoteCatalogBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().DesCatalog;
                // 
                // label10
                // 
                label10.AutoSize = true;
                label10.Location = new System.Drawing.Point(42, 306);
                label10.Name = "label10";
                label10.Size = new System.Drawing.Size(41, 12);
                label10.TabIndex = 8;
                label10.Text = Resource.lHelper.Key("n43");
                // 
                // remoteUserBox
                // 
                remoteUserBox.Location = new System.Drawing.Point(180, 306);
                remoteUserBox.Name = "remoteUserBox";
                remoteUserBox.Size = new System.Drawing.Size(151, 21);
                remoteUserBox.TabIndex = 9;
                remoteUserBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().DesUser;
                // 
                // label11
                // 
                label11.AutoSize = true;
                label11.Location = new System.Drawing.Point(42, 333);
                label11.Name = "label11";
                label11.Size = new System.Drawing.Size(29, 12);
                label11.TabIndex = 8;
                label11.Text = Resource.lHelper.Key("n44");
                // 
                // remotePwdBox
                // 
                remotePwdBox.Location = new System.Drawing.Point(180, 333);
                remotePwdBox.Name = "remotePwdBox";
                remotePwdBox.Size = new System.Drawing.Size(151, 21);
                remotePwdBox.TabIndex = 9;
                remotePwdBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().DesPwd;
                // 
                // label12
                // 
                label12.AutoSize = true;
                label12.Location = new System.Drawing.Point(42, 360);
                label12.Name = "label12";
                label12.Size = new System.Drawing.Size(29, 12);
                label12.TabIndex = 8;
                label12.Text = Resource.lHelper.Key("n42");
                // 
                // remotePortBox
                // 
                remotePortBox.Location = new System.Drawing.Point(180, 360);
                remotePortBox.Name = "remotePortBox";
                remotePortBox.Size = new System.Drawing.Size(151, 21);
                remotePortBox.TabIndex = 9;
                remotePortBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().DesPort.ToString();
                // 
                // label13
                // 
                label13.AutoSize = true;
                label13.Location = new System.Drawing.Point(42, 387);
                label13.Name = "label13";
                label13.Size = new System.Drawing.Size(41, 12);
                label13.TabIndex = 8;
                label13.Text = Resource.lHelper.Key("n41");
                // 
                // remoteInstanceBox
                // 
                remoteInstanceBox.Location = new System.Drawing.Point(180, 387);
                remoteInstanceBox.Name = "remoteInstanceBox";
                remoteInstanceBox.Size = new System.Drawing.Size(151, 21);
                remoteInstanceBox.TabIndex = 9;
                remoteInstanceBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().DesInstance;
                // 
                // testConnBtn
                // 
                testConnBtn.Location = new System.Drawing.Point(180, 445);
                testConnBtn.Name = "testConnBtn";
                testConnBtn.Size = new System.Drawing.Size(88, 23);
                testConnBtn.TabIndex = 10;
                testConnBtn.Text = Resource.lHelper.Key("n40");
                testConnBtn.UseVisualStyleBackColor = true;
                testConnBtn.Click += new EventHandler(TestRemoteDBConn);


                splitContainer1.Panel2.Controls.Add(testConnBtn);
                splitContainer1.Panel2.Controls.Add(remoteInstanceBox);
                splitContainer1.Panel2.Controls.Add(remotePwdBox);
                splitContainer1.Panel2.Controls.Add(remoteCatalogBox);
                splitContainer1.Panel2.Controls.Add(label13);
                splitContainer1.Panel2.Controls.Add(label11);
                splitContainer1.Panel2.Controls.Add(label7);
                splitContainer1.Panel2.Controls.Add(remotePortBox);
                splitContainer1.Panel2.Controls.Add(label12);
                splitContainer1.Panel2.Controls.Add(remoteUserBox);
                splitContainer1.Panel2.Controls.Add(label10);
                splitContainer1.Panel2.Controls.Add(remoteIPBox);
                splitContainer1.Panel2.Controls.Add(label6);
                splitContainer1.Panel2.Controls.Add(label5);
                splitContainer1.Panel2.Controls.Add(label4);
                splitContainer1.Panel2.Controls.Add(instanceEnabled);
                splitContainer1.Panel2.Controls.Add(entityFilterBtn);
                splitContainer1.Panel2.Controls.Add(entityFilterBox);
                splitContainer1.Panel2.Controls.Add(latOffsetBox);
                splitContainer1.Panel2.Controls.Add(label8);
                splitContainer1.Panel2.Controls.Add(label9);
                splitContainer1.Panel2.Controls.Add(lonOffsetBox);
                splitContainer1.Panel2.Controls.Add(lonOffset);
                splitContainer1.Panel2.Controls.Add(maxCountBox);
                splitContainer1.Panel2.Controls.Add(tableNameBox);
                splitContainer1.Panel2.Controls.Add(instanceNameBox);
                splitContainer1.Panel2.Controls.Add(label3);
                splitContainer1.Panel2.Controls.Add(pluginNameBox);
                splitContainer1.Panel2.Controls.Add(label2);
                splitContainer1.Panel2.Controls.Add(label1);
                splitContainer1.Panel2.Controls.Add(enterBtn);
                splitContainer1.Panel2.Controls.Add(panelType);
                splitContainer1.Panel2.Controls.Add(oldInstanceName);

                splitContainer1.Panel2.Controls.Add(deleteBtn);




            }
            else if (Resource.PluginList[pluginInstanceName].GetPluginModel().DllModel.PType == PluginType.TranDB)
            {
                splitContainer1.Panel2.Controls.Clear();


                System.Windows.Forms.ComboBox pluginNameBox;
                System.Windows.Forms.Label label2;
                System.Windows.Forms.Label label1;
                System.Windows.Forms.TextBox instanceNameBox;
                System.Windows.Forms.Label label3;
                System.Windows.Forms.TextBox latOffsetBox;
                System.Windows.Forms.Label label9;
                System.Windows.Forms.TextBox lonOffsetBox;
                System.Windows.Forms.Label lonOffset;
                System.Windows.Forms.CheckBox instanceEnabled;
                System.Windows.Forms.Button entityFilterBtn;
                System.Windows.Forms.TextBox entityFilterBox;
                System.Windows.Forms.Label label8;
                System.Windows.Forms.Label label5;
                System.Windows.Forms.Label label6;
                System.Windows.Forms.Label label4;
                System.Windows.Forms.TextBox maxCountBox;
                System.Windows.Forms.TextBox tableNameBox;
                System.Windows.Forms.TextBox citycode;
                System.Windows.Forms.Button enterBtn;
                System.Windows.Forms.Label panelType;
                System.Windows.Forms.Label oldInstanceName;
                System.Windows.Forms.Button deleteBtn;
                

                deleteBtn = new Button();
                oldInstanceName = new Label();
                panelType = new System.Windows.Forms.Label();
                enterBtn = new System.Windows.Forms.Button();
                instanceEnabled = new System.Windows.Forms.CheckBox();
                entityFilterBtn = new System.Windows.Forms.Button();
                entityFilterBox = new System.Windows.Forms.TextBox();
                latOffsetBox = new System.Windows.Forms.TextBox();
                label8 = new System.Windows.Forms.Label();
                label9 = new System.Windows.Forms.Label();
                lonOffsetBox = new System.Windows.Forms.TextBox();
                lonOffset = new System.Windows.Forms.Label();
                instanceNameBox = new System.Windows.Forms.TextBox();
                label3 = new System.Windows.Forms.Label();
                pluginNameBox = new System.Windows.Forms.ComboBox();
                label2 = new System.Windows.Forms.Label();
                label1 = new System.Windows.Forms.Label();
                label4 = new System.Windows.Forms.Label();
                tableNameBox = new System.Windows.Forms.TextBox();
                maxCountBox = new System.Windows.Forms.TextBox();
                label5 = new System.Windows.Forms.Label();
                label6 = new System.Windows.Forms.Label();
                citycode = new System.Windows.Forms.TextBox();
                //
                //deleteBtn
                //
                deleteBtn.Location = new System.Drawing.Point(170, 496);
                deleteBtn.Name = "deleteBtn";
                deleteBtn.Size = new System.Drawing.Size(75, 23);
                deleteBtn.TabIndex = 2;
                deleteBtn.Text = Resource.lHelper.Key("n38");
                deleteBtn.UseVisualStyleBackColor = true;
                deleteBtn.Click += new EventHandler(DeleteBtn_Click);
                // 
                // enterBtn
                // 
                enterBtn.Location = new System.Drawing.Point(250, 496);
                enterBtn.Name = "enterBtn";
                enterBtn.Size = new System.Drawing.Size(75, 23);
                enterBtn.TabIndex = 2;
                enterBtn.Text = Resource.lHelper.Key("n37");
                enterBtn.UseVisualStyleBackColor = true;
                enterBtn.Click += new EventHandler(enterBtn_Click);

                panelType.AutoSize = true;
                panelType.Location = new System.Drawing.Point(96, 122);
                panelType.Name = "panelType";
                panelType.Size = new System.Drawing.Size(41, 12);
                panelType.TabIndex = 0;
                panelType.Text = "pluginInstance";
                panelType.Visible = false;


                oldInstanceName.AutoSize = true;
                oldInstanceName.Location = new System.Drawing.Point(96, 122);
                oldInstanceName.Name = "oldInstanceName";
                oldInstanceName.Size = new System.Drawing.Size(41, 12);
                oldInstanceName.TabIndex = 0;
                oldInstanceName.Text = pluginInstanceName;
                oldInstanceName.Visible = false;


                // 
                // instanceEnabled
                // 
                instanceEnabled.AutoSize = true;
                instanceEnabled.Location = new System.Drawing.Point(180, 320);
                instanceEnabled.Name = "instanceEnabled";
                instanceEnabled.Size = new System.Drawing.Size(48, 16);
                instanceEnabled.TabIndex = 6;
                instanceEnabled.Text = Resource.lHelper.Key("n48");
                instanceEnabled.UseVisualStyleBackColor = true;
                if (Resource.PluginList[pluginInstanceName].GetPluginModel().Enabled)
                {
                    instanceEnabled.Checked = true;
                }
                else
                {
                    instanceEnabled.Checked = false;

                }

                // 
                // entityFilterBtn
                // 
                entityFilterBtn.Location = new System.Drawing.Point(340, 244);
                entityFilterBtn.Name = "entityFilterBtn";
                entityFilterBtn.Size = new System.Drawing.Size(37, 23);
                entityFilterBtn.TabIndex = 5;
                entityFilterBtn.Text = "...";
                entityFilterBtn.UseVisualStyleBackColor = true;
                entityFilterBtn.Click += new EventHandler(SelectEntity);
                // 
                // entityFilterBox
                // 
                entityFilterBox.Location = new System.Drawing.Point(180, 246);
                entityFilterBox.Name = "entityFilterBox";
                entityFilterBox.Size = new System.Drawing.Size(151, 21);
                entityFilterBox.TabIndex = 4;
                entityFilterBox.Enabled = true;
                string str = "";
                foreach (int s in Resource.PluginList[pluginInstanceName].GetPluginModel().EntityID)
                {
                    str = str + s + ",";

                }
                if (str.IndexOf(",") >= 0)
                {
                    entityFilterBox.Text = str.Remove(str.LastIndexOf(","));
                }
                else
                {
                    entityFilterBox.Text = "";
                }
                entityFilterBox.Enabled = false;

                // 
                // latOffsetBox
                // 
                latOffsetBox.Location = new System.Drawing.Point(180, 211);
                latOffsetBox.Name = "latOffsetBox";
                latOffsetBox.Size = new System.Drawing.Size(151, 21);
                latOffsetBox.TabIndex = 4;
                latOffsetBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().LatOffset.ToString();

                // 
                // label8
                // 
                label8.AutoSize = true;
                label8.Location = new System.Drawing.Point(41, 252);
                label8.Name = "label8";
                label8.Size = new System.Drawing.Size(53, 12);
                label8.TabIndex = 3;
                label8.Text = Resource.lHelper.Key("n24");
                // 
                // label9
                // 
                label9.AutoSize = true;
                label9.Location = new System.Drawing.Point(41, 217);
                label9.Name = "label9";
                label9.Size = new System.Drawing.Size(53, 12);
                label9.TabIndex = 3;
                label9.Text = Resource.lHelper.Key("n52");
                // 
                // lonOffsetBox
                // 
                lonOffsetBox.Location = new System.Drawing.Point(180, 179);
                lonOffsetBox.Name = "lonOffsetBox";
                lonOffsetBox.Size = new System.Drawing.Size(151, 21);
                lonOffsetBox.TabIndex = 4;
                lonOffsetBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().LonOffset.ToString();
                // 
                // lonOffset 经度偏移
                // 
                lonOffset.AutoSize = true;
                lonOffset.Location = new System.Drawing.Point(41, 185);
                lonOffset.Name = "lonOffset";
                lonOffset.Size = new System.Drawing.Size(53, 12);
                lonOffset.TabIndex = 3;
                lonOffset.Text = Resource.lHelper.Key("n51");
                // 
                // instanceNameBox
                // 
                instanceNameBox.Location = new System.Drawing.Point(180, 84);
                instanceNameBox.Name = "instanceNameBox";
                instanceNameBox.Size = new System.Drawing.Size(151, 21);
                instanceNameBox.TabIndex = 4;
                instanceNameBox.Text = pluginInstanceName;
                // 
                // label3
                // 
                label3.AutoSize = true;
                label3.Location = new System.Drawing.Point(41, 90);
                label3.Name = "label3";
                label3.Size = new System.Drawing.Size(53, 12);
                label3.TabIndex = 3;
                label3.Text = Resource.lHelper.Key("n49");
                // 
                // pluginNameBox
                // 
                pluginNameBox.FormattingEnabled = true;
                pluginNameBox.Location = new System.Drawing.Point(180, 51);
                pluginNameBox.Name = "pluginNameBox";
                pluginNameBox.Size = new System.Drawing.Size(151, 20);
                pluginNameBox.TabIndex = 2;
                pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;

                pluginNameBox.Items.Clear();
                pluginNameBox.Items.Add(Resource.PluginList[pluginInstanceName].GetPluginModel().DllModel.Name);
                pluginNameBox.SelectedIndex = 0;
                pluginNameBox.Enabled = false;

                // 
                // label2
                // 
                label2.AutoSize = true;
                label2.Location = new System.Drawing.Point(39, 55);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(77, 12);
                label2.TabIndex = 1;
                label2.Text = Resource.lHelper.Key("n55");
                // 
                // label1
                // 
                label1.AutoSize = true;
                label1.Location = new System.Drawing.Point(23, 14);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(101, 12);
                label1.TabIndex = 0;
                label1.Text = Resource.lHelper.Key("m24");  //han modify
                // 
                // label4
                // 
                label4.AutoSize = true;
                label4.Location = new System.Drawing.Point(43, 120);
                label4.Name = "label4";
                label4.Size = new System.Drawing.Size(41, 12);
                label4.TabIndex = 7;
                label4.Text = Resource.lHelper.Key("n53");
                // 
                // tableNameBox
                // 
                tableNameBox.Location = new System.Drawing.Point(180, 115);
                tableNameBox.Name = "tableNameBox";
                tableNameBox.Size = new System.Drawing.Size(151, 21);
                tableNameBox.TabIndex = 4;
                tableNameBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().TableName;
                // 
                // maxCountBox
                // 
                maxCountBox.Location = new System.Drawing.Point(180, 147);
                maxCountBox.Name = "maxCountBox";
                maxCountBox.Size = new System.Drawing.Size(151, 21);
                maxCountBox.TabIndex = 4;
                maxCountBox.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().MaxCount.ToString();
                // 
                // label5
                // 
                label5.AutoSize = true;
                label5.Location = new System.Drawing.Point(43, 152);
                label5.Name = "label5";
                label5.Size = new System.Drawing.Size(65, 12);
                label5.TabIndex = 7;
                label5.Text = Resource.lHelper.Key("n54");

                // 
                // citycode
                // 
                label6.AutoSize = true;
                label6.Location = new System.Drawing.Point(43, 284);
                label6.Name = "label6";
                label6.Size = new System.Drawing.Size(65, 52);
                label6.TabIndex = 7;
                label6.Text = Resource.lHelper.Key("n76");

                // 
                // citycode
                // 
                citycode.Location = new System.Drawing.Point(180, 284);
                citycode.Name = "citycode";
                citycode.Size = new System.Drawing.Size(151, 51);
                citycode.TabIndex = 10;
                citycode.Text = Resource.PluginList[pluginInstanceName].GetPluginModel().Citycode.ToString();

                splitContainer1.Panel2.Controls.Add(label5);
                splitContainer1.Panel2.Controls.Add(label4);
                splitContainer1.Panel2.Controls.Add(instanceEnabled);
                splitContainer1.Panel2.Controls.Add(entityFilterBtn);
                splitContainer1.Panel2.Controls.Add(entityFilterBox);
                splitContainer1.Panel2.Controls.Add(latOffsetBox);
                splitContainer1.Panel2.Controls.Add(label8);
                splitContainer1.Panel2.Controls.Add(label9);
                splitContainer1.Panel2.Controls.Add(lonOffsetBox);
                splitContainer1.Panel2.Controls.Add(lonOffset);
                splitContainer1.Panel2.Controls.Add(maxCountBox);
                splitContainer1.Panel2.Controls.Add(tableNameBox);
                splitContainer1.Panel2.Controls.Add(instanceNameBox);
                splitContainer1.Panel2.Controls.Add(label3);
                splitContainer1.Panel2.Controls.Add(pluginNameBox);
                splitContainer1.Panel2.Controls.Add(label2);
                splitContainer1.Panel2.Controls.Add(label1);
                splitContainer1.Panel2.Controls.Add(label6);
                splitContainer1.Panel2.Controls.Add(citycode);
                splitContainer1.Panel2.Controls.Add(enterBtn);
                splitContainer1.Panel2.Controls.Add(panelType);
                splitContainer1.Panel2.Controls.Add(oldInstanceName);
                splitContainer1.Panel2.Controls.Add(deleteBtn);



            }
            else
            {
                MessageBox.Show(Resource.lHelper.Key("m59"));
                Logger.Error("error occur when modify plugin instance,error message:unknown plugin type,instance name:" + pluginInstanceName);
                return;
            }





        }
        //show controls by pluginName
        private void selectPanelByVersion(string pluginName)
        {
            if (pluginName.Equals("UDP_Version_0"))
            {
                BindIPItem();
            }
            else if (pluginName.Equals("DB_Version_0"))
            {
                BindDBItem();
            }
            else
            {
                choosepluginPanel(pluginName);
            }
        }

        private void pluginName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pluginName = (string)((ComboBox)sender).SelectedItem;
            selectPanelByVersion(pluginName);
        }


        private void choosepluginPanel(string pluginName)
        {

            if (Resource.DllList[pluginName].PType == PluginType.TranDB)
            {
                splitContainer1.Panel2.Controls.Clear();


                System.Windows.Forms.ComboBox pluginNameBox;
                System.Windows.Forms.Label label2;
                System.Windows.Forms.Label label1;
                System.Windows.Forms.TextBox instanceNameBox;
                System.Windows.Forms.Label label3;
                System.Windows.Forms.TextBox latOffsetBox;
                System.Windows.Forms.Label label9;
                System.Windows.Forms.TextBox lonOffsetBox;
                System.Windows.Forms.Label lonOffset;
                System.Windows.Forms.CheckBox instanceEnabled;
                System.Windows.Forms.Button entityFilterBtn;
                System.Windows.Forms.TextBox entityFilterBox;
                System.Windows.Forms.Label label8;
                System.Windows.Forms.Label label5;
                System.Windows.Forms.Label label6;
                System.Windows.Forms.Label label4;
                System.Windows.Forms.TextBox maxCountBox;
                System.Windows.Forms.TextBox citycode;
                System.Windows.Forms.TextBox tableNameBox;
                System.Windows.Forms.Button enterBtn;
                System.Windows.Forms.Label panelType;


                panelType = new System.Windows.Forms.Label();
                enterBtn = new System.Windows.Forms.Button();
                instanceEnabled = new System.Windows.Forms.CheckBox();
                entityFilterBtn = new System.Windows.Forms.Button();
                entityFilterBox = new System.Windows.Forms.TextBox();
                latOffsetBox = new System.Windows.Forms.TextBox();
                label8 = new System.Windows.Forms.Label();
                label9 = new System.Windows.Forms.Label();
                lonOffsetBox = new System.Windows.Forms.TextBox();
                lonOffset = new System.Windows.Forms.Label();
                instanceNameBox = new System.Windows.Forms.TextBox();
                label3 = new System.Windows.Forms.Label();
                pluginNameBox = new System.Windows.Forms.ComboBox();
                label2 = new System.Windows.Forms.Label();
                label1 = new System.Windows.Forms.Label();
                label4 = new System.Windows.Forms.Label();
                tableNameBox = new System.Windows.Forms.TextBox();
                maxCountBox = new System.Windows.Forms.TextBox();
                citycode = new System.Windows.Forms.TextBox();
                label5 = new System.Windows.Forms.Label();
                label6 = new System.Windows.Forms.Label();

                // 
                // enterBtn
                // 
                enterBtn.Location = new System.Drawing.Point(250, 496);
                enterBtn.Name = "enterBtn";
                enterBtn.Size = new System.Drawing.Size(75, 23);
                enterBtn.TabIndex = 2;
                enterBtn.Text = Resource.lHelper.Key("n39");
                enterBtn.UseVisualStyleBackColor = true;
                enterBtn.Click += new EventHandler(enterBtn_Click);

                panelType.AutoSize = true;
                panelType.Location = new System.Drawing.Point(96, 122);
                panelType.Name = "panelType";
                panelType.Size = new System.Drawing.Size(41, 12);
                panelType.TabIndex = 0;
                panelType.Text = "pluginTran";
                panelType.Visible = false;

                // 
                // instanceEnabled
                // 
                instanceEnabled.AutoSize = true;
                instanceEnabled.Location = new System.Drawing.Point(180, 320);
                instanceEnabled.Name = "instanceEnabled";
                instanceEnabled.Size = new System.Drawing.Size(48, 16);
                instanceEnabled.TabIndex = 6;
                instanceEnabled.Text = Resource.lHelper.Key("n48");
                instanceEnabled.UseVisualStyleBackColor = true;


                // 
                // entityFilterBtn
                // 
                entityFilterBtn.Location = new System.Drawing.Point(340, 244);
                entityFilterBtn.Name = "entityFilterBtn";
                entityFilterBtn.Size = new System.Drawing.Size(37, 23);
                entityFilterBtn.TabIndex = 5;
                entityFilterBtn.Text = "...";
                entityFilterBtn.UseVisualStyleBackColor = true;
                entityFilterBtn.Click += new EventHandler(SelectEntity);
                // 
                // entityFilterBox
                // 
                entityFilterBox.Location = new System.Drawing.Point(180, 246);
                entityFilterBox.Name = "entityFilterBox";
                entityFilterBox.Size = new System.Drawing.Size(151, 21);
                entityFilterBox.TabIndex = 4;
                entityFilterBox.Enabled = false;
                // 
                // latOffsetBox
                // 
                latOffsetBox.Location = new System.Drawing.Point(180, 211);
                latOffsetBox.Name = "latOffsetBox";
                latOffsetBox.Size = new System.Drawing.Size(151, 21);
                latOffsetBox.TabIndex = 4;

                // 
                // label8
                // 
                label8.AutoSize = true;
                label8.Location = new System.Drawing.Point(41, 252);
                label8.Name = "label8";
                label8.Size = new System.Drawing.Size(53, 12);
                label8.TabIndex = 3;
                label8.Text = Resource.lHelper.Key("n24");
                // 
                // label9
                // 
                label9.AutoSize = true;
                label9.Location = new System.Drawing.Point(41, 217);
                label9.Name = "label9";
                label9.Size = new System.Drawing.Size(53, 12);
                label9.TabIndex = 3;
                label9.Text = Resource.lHelper.Key("n52");
                // 
                // lonOffsetBox
                // 
                lonOffsetBox.Location = new System.Drawing.Point(180, 179);
                lonOffsetBox.Name = "lonOffsetBox";
                lonOffsetBox.Size = new System.Drawing.Size(151, 21);
                lonOffsetBox.TabIndex = 4;
                // 
                // lonOffset 经度偏移
                // 
                lonOffset.AutoSize = true;
                lonOffset.Location = new System.Drawing.Point(41, 185);
                lonOffset.Name = "lonOffset";
                lonOffset.Size = new System.Drawing.Size(53, 12);
                lonOffset.TabIndex = 3;
                lonOffset.Text = Resource.lHelper.Key("n51");
                // 
                // instanceNameBox
                // 
                instanceNameBox.Location = new System.Drawing.Point(180, 84);
                instanceNameBox.Name = "instanceNameBox";
                instanceNameBox.Size = new System.Drawing.Size(151, 21);
                instanceNameBox.TabIndex = 4;
                // 
                // label3
                // 
                label3.AutoSize = true;
                label3.Location = new System.Drawing.Point(41, 90);
                label3.Name = "label3";
                label3.Size = new System.Drawing.Size(53, 12);
                label3.TabIndex = 3;
                label3.Text = Resource.lHelper.Key("n49");
                // 
                // pluginNameBox
                // 
                pluginNameBox.FormattingEnabled = true;
                pluginNameBox.Location = new System.Drawing.Point(180, 51);
                pluginNameBox.Name = "pluginNameBox";
                pluginNameBox.Size = new System.Drawing.Size(151, 20);
                pluginNameBox.TabIndex = 2;
                pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;

                pluginNameBox.Items.Clear();
                pluginNameBox.Items.Add("DB_Version_0");
                foreach (string s in Resource.DllList.Keys)
                {
                    if (s.Contains("DB"))
                    {
                        pluginNameBox.Items.Add(s);
                    }
                }
                pluginNameBox.SelectedItem = pluginName;
                pluginNameBox.SelectedValueChanged += new EventHandler(pluginName_SelectedIndexChanged);

                // 
                // label2
                // 
                label2.AutoSize = true;
                label2.Location = new System.Drawing.Point(39, 55);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(77, 12);
                label2.TabIndex = 1;
                label2.Text = Resource.lHelper.Key("n55");
                // 
                // label1
                // 
                label1.AutoSize = true;
                label1.Location = new System.Drawing.Point(23, 14);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(101, 12);
                label1.TabIndex = 0;
                label1.Text = Resource.lHelper.Key("m23");   //han modify
                // 
                // label4
                // 
                label4.AutoSize = true;
                label4.Location = new System.Drawing.Point(43, 120);
                label4.Name = "label4";
                label4.Size = new System.Drawing.Size(41, 12);
                label4.TabIndex = 7;
                label4.Text = Resource.lHelper.Key("n53");
                // 
                // tableNameBox
                // 
                tableNameBox.Location = new System.Drawing.Point(180, 115);
                tableNameBox.Name = "tableNameBox";
                tableNameBox.Size = new System.Drawing.Size(151, 21);
                tableNameBox.TabIndex = 4;
                // 
                // maxCountBox
                // 
                maxCountBox.Location = new System.Drawing.Point(180, 147);
                maxCountBox.Name = "maxCountBox";
                maxCountBox.Size = new System.Drawing.Size(151, 21);
                maxCountBox.TabIndex = 4;
                // 
                // label5
                // 
                label5.AutoSize = true;
                label5.Location = new System.Drawing.Point(43, 152);
                label5.Name = "label5";
                label5.Size = new System.Drawing.Size(65, 12);
                label5.TabIndex = 7;
                label5.Text = Resource.lHelper.Key("n54");

                // 
                // citycode
                // 
                label6.AutoSize = true;
                label6.Location = new System.Drawing.Point(43, 284);
                label6.Name = "label6";
                label6.Size = new System.Drawing.Size(65, 52);
                label6.TabIndex = 7;
                label6.Text = Resource.lHelper.Key("n76");

                // 
                // citycode
                // 
                citycode.Location = new System.Drawing.Point(180, 284);
                citycode.Name = "citycode";
                citycode.Size = new System.Drawing.Size(151, 51);
                citycode.TabIndex = 10;

                splitContainer1.Panel2.Controls.Add(label5);
                splitContainer1.Panel2.Controls.Add(label4);
                splitContainer1.Panel2.Controls.Add(instanceEnabled);
                splitContainer1.Panel2.Controls.Add(entityFilterBtn);
                splitContainer1.Panel2.Controls.Add(entityFilterBox);
                splitContainer1.Panel2.Controls.Add(latOffsetBox);
                splitContainer1.Panel2.Controls.Add(label8);
                splitContainer1.Panel2.Controls.Add(label9);
                splitContainer1.Panel2.Controls.Add(lonOffsetBox);
                splitContainer1.Panel2.Controls.Add(lonOffset);
                splitContainer1.Panel2.Controls.Add(maxCountBox);
                splitContainer1.Panel2.Controls.Add(tableNameBox);
                splitContainer1.Panel2.Controls.Add(instanceNameBox);
                splitContainer1.Panel2.Controls.Add(label3);
                splitContainer1.Panel2.Controls.Add(pluginNameBox);
                splitContainer1.Panel2.Controls.Add(label6);
                splitContainer1.Panel2.Controls.Add(citycode);
                splitContainer1.Panel2.Controls.Add(label2);
                splitContainer1.Panel2.Controls.Add(label1);
                splitContainer1.Panel2.Controls.Add(maxCountBox);
                splitContainer1.Panel2.Controls.Add(enterBtn);
                splitContainer1.Panel2.Controls.Add(panelType);

            }
            else if (Resource.DllList[pluginName].PType == PluginType.RemoteDB)
            {
                splitContainer1.Panel2.Controls.Clear();

                System.Windows.Forms.ComboBox pluginNameBox;
                System.Windows.Forms.Label label2;
                System.Windows.Forms.Label label1;
                System.Windows.Forms.TextBox instanceNameBox;
                System.Windows.Forms.Label label3;
                System.Windows.Forms.TextBox latOffsetBox;
                System.Windows.Forms.Label label9;
                System.Windows.Forms.TextBox lonOffsetBox;
                System.Windows.Forms.Label lonOffset;
                System.Windows.Forms.CheckBox instanceEnabled;
                System.Windows.Forms.Button entityFilterBtn;
                System.Windows.Forms.TextBox entityFilterBox;
                System.Windows.Forms.Label label8;
                System.Windows.Forms.Label label5;
                System.Windows.Forms.Label label4;
                System.Windows.Forms.TextBox maxCountBox;
                System.Windows.Forms.TextBox tableNameBox;
                System.Windows.Forms.TextBox remoteInstanceBox;
                System.Windows.Forms.TextBox remotePwdBox;
                System.Windows.Forms.TextBox remoteCatalogBox;
                System.Windows.Forms.Label label13;
                System.Windows.Forms.Label label11;
                System.Windows.Forms.Label label7;
                System.Windows.Forms.TextBox remotePortBox;
                System.Windows.Forms.Label label12;
                System.Windows.Forms.TextBox remoteUserBox;
                System.Windows.Forms.Label label10;
                System.Windows.Forms.TextBox remoteIPBox;
                System.Windows.Forms.Label label6;
                System.Windows.Forms.Button testConnBtn;
                System.Windows.Forms.Button enterBtn;
                System.Windows.Forms.Label panelType;


                panelType = new System.Windows.Forms.Label();
                enterBtn = new System.Windows.Forms.Button();
                instanceEnabled = new System.Windows.Forms.CheckBox();
                entityFilterBtn = new System.Windows.Forms.Button();
                entityFilterBox = new System.Windows.Forms.TextBox();
                latOffsetBox = new System.Windows.Forms.TextBox();
                label8 = new System.Windows.Forms.Label();
                label9 = new System.Windows.Forms.Label();
                lonOffsetBox = new System.Windows.Forms.TextBox();
                lonOffset = new System.Windows.Forms.Label();
                instanceNameBox = new System.Windows.Forms.TextBox();
                label3 = new System.Windows.Forms.Label();
                pluginNameBox = new System.Windows.Forms.ComboBox();
                label2 = new System.Windows.Forms.Label();
                label1 = new System.Windows.Forms.Label();
                label4 = new System.Windows.Forms.Label();
                tableNameBox = new System.Windows.Forms.TextBox();
                maxCountBox = new System.Windows.Forms.TextBox();
                label5 = new System.Windows.Forms.Label();
                label6 = new System.Windows.Forms.Label();
                remoteIPBox = new System.Windows.Forms.TextBox();
                label7 = new System.Windows.Forms.Label();
                remoteCatalogBox = new System.Windows.Forms.TextBox();
                label10 = new System.Windows.Forms.Label();
                remoteUserBox = new System.Windows.Forms.TextBox();
                label11 = new System.Windows.Forms.Label();
                remotePwdBox = new System.Windows.Forms.TextBox();
                label12 = new System.Windows.Forms.Label();
                remotePortBox = new System.Windows.Forms.TextBox();
                label13 = new System.Windows.Forms.Label();
                remoteInstanceBox = new System.Windows.Forms.TextBox();
                testConnBtn = new System.Windows.Forms.Button();


                // 
                // enterBtn
                // 
                enterBtn.Location = new System.Drawing.Point(250, 496);
                enterBtn.Name = "enterBtn";
                enterBtn.Size = new System.Drawing.Size(75, 23);
                enterBtn.TabIndex = 2;
                enterBtn.Text = Resource.lHelper.Key("n39");
                enterBtn.UseVisualStyleBackColor = true;
                enterBtn.Click += new EventHandler(enterBtn_Click);

                panelType.AutoSize = true;
                panelType.Location = new System.Drawing.Point(96, 122);
                panelType.Name = "panelType";
                panelType.Size = new System.Drawing.Size(41, 12);
                panelType.TabIndex = 0;
                panelType.Text = "pluginTran";
                panelType.Visible = false;



                // 
                // instanceEnabled
                // 
                instanceEnabled.AutoSize = true;
                instanceEnabled.Location = new System.Drawing.Point(180, 422);
                instanceEnabled.Name = "instanceEnabled";
                instanceEnabled.Size = new System.Drawing.Size(48, 16);
                instanceEnabled.TabIndex = 6;
                instanceEnabled.Text = Resource.lHelper.Key("n48");
                instanceEnabled.UseVisualStyleBackColor = true;
                // 
                // entityFilterBtn
                // 
                entityFilterBtn.Location = new System.Drawing.Point(340, 221);
                entityFilterBtn.Name = "entityFilterBtn";
                entityFilterBtn.Size = new System.Drawing.Size(37, 23);
                entityFilterBtn.TabIndex = 5;
                entityFilterBtn.Text = "...";
                entityFilterBtn.UseVisualStyleBackColor = true;
                entityFilterBtn.Click += new EventHandler(SelectEntity);
                // 
                // entityFilterBox
                // 
                entityFilterBox.Location = new System.Drawing.Point(180, 223);
                entityFilterBox.Name = "entityFilterBox";
                entityFilterBox.Size = new System.Drawing.Size(151, 21);
                entityFilterBox.TabIndex = 4;
                entityFilterBox.Enabled = false;
                // 
                // latOffsetBox
                // 
                latOffsetBox.Location = new System.Drawing.Point(180, 193);
                latOffsetBox.Name = "latOffsetBox";
                latOffsetBox.Size = new System.Drawing.Size(151, 21);
                latOffsetBox.TabIndex = 4;
                // 
                // entityFilterBox
                // 
                label8.AutoSize = true;
                label8.Location = new System.Drawing.Point(41, 228);
                label8.Name = "label8";
                label8.Size = new System.Drawing.Size(53, 12);
                label8.TabIndex = 3;
                label8.Text = Resource.lHelper.Key("n24");
                // 
                // latOffsetBox 纬度偏移
                // 
                label9.AutoSize = true;
                label9.Location = new System.Drawing.Point(41, 200);
                label9.Name = "label9";
                label9.Size = new System.Drawing.Size(53, 12);
                label9.TabIndex = 3;
                label9.Text = Resource.lHelper.Key("n52");
                // 
                // lonOffsetBox
                // 
                lonOffsetBox.Location = new System.Drawing.Point(180, 164);
                lonOffsetBox.Name = "lonOffsetBox";
                lonOffsetBox.Size = new System.Drawing.Size(151, 21);
                lonOffsetBox.TabIndex = 4;
                // 
                // lonOffset 经度偏移
                // 
                lonOffset.AutoSize = true;
                lonOffset.Location = new System.Drawing.Point(41, 170);
                lonOffset.Name = "lonOffset";
                lonOffset.Size = new System.Drawing.Size(53, 12);
                lonOffset.TabIndex = 3;
                lonOffset.Text = Resource.lHelper.Key("n51");
                // 
                // instanceNameBox
                // 
                instanceNameBox.Location = new System.Drawing.Point(180, 79);
                instanceNameBox.Name = "instanceNameBox";
                instanceNameBox.Size = new System.Drawing.Size(151, 21);
                instanceNameBox.TabIndex = 4;
                // 
                // label3 实例名称
                // 
                label3.AutoSize = true;
                label3.Location = new System.Drawing.Point(41, 85);
                label3.Name = "label3";
                label3.Size = new System.Drawing.Size(53, 12);
                label3.TabIndex = 3;
                label3.Text = Resource.lHelper.Key("n49");
                // 
                // pluginNameBox
                // 
                pluginNameBox.FormattingEnabled = true;
                pluginNameBox.Location = new System.Drawing.Point(180, 51);
                pluginNameBox.Name = "pluginNameBox";
                pluginNameBox.Size = new System.Drawing.Size(151, 20);
                pluginNameBox.TabIndex = 2;
                pluginNameBox.Items.Clear();
                pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;
                pluginNameBox.Items.Add("DB_Version_0");
                foreach (string s in Resource.DllList.Keys)
                {
                    if (s.Contains("DB"))
                    {
                        pluginNameBox.Items.Add(s);
                    }
                }
                pluginNameBox.SelectedItem = pluginName;
                pluginNameBox.SelectedValueChanged += new EventHandler(pluginName_SelectedIndexChanged);



                // 
                // label2 选择协议版本
                // 
                label2.AutoSize = true;
                label2.Location = new System.Drawing.Point(39, 55);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(77, 12);
                label2.TabIndex = 1;
                label2.Text = Resource.lHelper.Key("n55");
                // 
                // label1 添加数据库转储实例
                // 
                label1.AutoSize = true;
                label1.Location = new System.Drawing.Point(23, 14);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(101, 12);
                label1.TabIndex = 0;
                label1.Text = Resource.lHelper.Key("m23");  //han modify
                // 
                // label4 表名称
                // 
                label4.AutoSize = true;
                label4.Location = new System.Drawing.Point(43, 112);
                label4.Name = "label4";
                label4.Size = new System.Drawing.Size(41, 12);
                label4.TabIndex = 7;
                label4.Text = Resource.lHelper.Key("n53");
                // 
                // tableNameBox
                // 
                tableNameBox.Location = new System.Drawing.Point(180, 107);
                tableNameBox.Name = "tableNameBox";
                tableNameBox.Size = new System.Drawing.Size(151, 21);
                tableNameBox.TabIndex = 4;
                // 
                // maxCountBox
                // 
                maxCountBox.Location = new System.Drawing.Point(180, 135);
                maxCountBox.Name = "maxCountBox";
                maxCountBox.Size = new System.Drawing.Size(151, 21);
                maxCountBox.TabIndex = 4;
                // 
                // label5
                // 
                label5.AutoSize = true;
                label5.Location = new System.Drawing.Point(43, 140);
                label5.Name = "label5";
                label5.Size = new System.Drawing.Size(65, 12);
                label5.TabIndex = 7;
                label5.Text = Resource.lHelper.Key("n54");
                // 
                // label6
                // 
                label6.AutoSize = true;
                label6.Location = new System.Drawing.Point(42, 252);
                label6.Name = "label6";
                label6.Size = new System.Drawing.Size(53, 12);
                label6.TabIndex = 8;
                label6.Text = Resource.lHelper.Key("n56");
                // 
                // remoteIPBox
                // 
                remoteIPBox.Location = new System.Drawing.Point(180, 252);
                remoteIPBox.Name = "remoteIPBox";
                remoteIPBox.Size = new System.Drawing.Size(151, 21);
                remoteIPBox.TabIndex = 9;
                // 
                // label7
                // 
                label7.AutoSize = true;
                label7.Location = new System.Drawing.Point(42, 279);
                label7.Name = "label7";
                label7.Size = new System.Drawing.Size(41, 12);
                label7.TabIndex = 8;
                label7.Text = Resource.lHelper.Key("n45");
                // 
                // remoteCatalogBox
                // 
                remoteCatalogBox.Location = new System.Drawing.Point(180, 279);
                remoteCatalogBox.Name = "remoteCatalogBox";
                remoteCatalogBox.Size = new System.Drawing.Size(151, 21);
                remoteCatalogBox.TabIndex = 9;
                // 
                // label10
                // 
                label10.AutoSize = true;
                label10.Location = new System.Drawing.Point(42, 306);
                label10.Name = "label10";
                label10.Size = new System.Drawing.Size(41, 12);
                label10.TabIndex = 8;
                label10.Text = Resource.lHelper.Key("n43");
                // 
                // remoteUserBox
                // 
                remoteUserBox.Location = new System.Drawing.Point(180, 306);
                remoteUserBox.Name = "remoteUserBox";
                remoteUserBox.Size = new System.Drawing.Size(151, 21);
                remoteUserBox.TabIndex = 9;
                // 
                // label11
                // 
                label11.AutoSize = true;
                label11.Location = new System.Drawing.Point(42, 333);
                label11.Name = "label11";
                label11.Size = new System.Drawing.Size(29, 12);
                label11.TabIndex = 8;
                label11.Text = Resource.lHelper.Key("n44");
                // 
                // remotePwdBox
                // 
                remotePwdBox.Location = new System.Drawing.Point(180, 333);
                remotePwdBox.Name = "remotePwdBox";
                remotePwdBox.Size = new System.Drawing.Size(151, 21);
                remotePwdBox.TabIndex = 9;
                // 
                // label12
                // 
                label12.AutoSize = true;
                label12.Location = new System.Drawing.Point(42, 360);
                label12.Name = "label12";
                label12.Size = new System.Drawing.Size(29, 12);
                label12.TabIndex = 8;
                label12.Text = Resource.lHelper.Key("n42");
                // 
                // remotePortBox
                // 
                remotePortBox.Location = new System.Drawing.Point(180, 360);
                remotePortBox.Name = "remotePortBox";
                remotePortBox.Size = new System.Drawing.Size(151, 21);
                remotePortBox.TabIndex = 9;
                // 
                // label13
                // 
                label13.AutoSize = true;
                label13.Location = new System.Drawing.Point(42, 387);
                label13.Name = "label13";
                label13.Size = new System.Drawing.Size(41, 12);
                label13.TabIndex = 8;
                label13.Text = Resource.lHelper.Key("n41");
                // 
                // remoteInstanceBox
                // 
                remoteInstanceBox.Location = new System.Drawing.Point(180, 387);
                remoteInstanceBox.Name = "remoteInstanceBox";
                remoteInstanceBox.Size = new System.Drawing.Size(151, 21);
                remoteInstanceBox.TabIndex = 9;
                // 
                // testConnBtn
                // 
                testConnBtn.Location = new System.Drawing.Point(180, 445);
                testConnBtn.Name = "testConnBtn";
                testConnBtn.Size = new System.Drawing.Size(88, 23);
                testConnBtn.TabIndex = 10;
                testConnBtn.Text = Resource.lHelper.Key("n40");
                testConnBtn.UseVisualStyleBackColor = true;
                testConnBtn.Click += new EventHandler(TestRemoteDBConn);


                splitContainer1.Panel2.Controls.Add(testConnBtn);
                splitContainer1.Panel2.Controls.Add(remoteInstanceBox);
                splitContainer1.Panel2.Controls.Add(remotePwdBox);
                splitContainer1.Panel2.Controls.Add(remoteCatalogBox);
                splitContainer1.Panel2.Controls.Add(label13);
                splitContainer1.Panel2.Controls.Add(label11);
                splitContainer1.Panel2.Controls.Add(label7);
                splitContainer1.Panel2.Controls.Add(remotePortBox);
                splitContainer1.Panel2.Controls.Add(label12);
                splitContainer1.Panel2.Controls.Add(remoteUserBox);
                splitContainer1.Panel2.Controls.Add(label10);
                splitContainer1.Panel2.Controls.Add(remoteIPBox);
                splitContainer1.Panel2.Controls.Add(label6);
                splitContainer1.Panel2.Controls.Add(label5);
                splitContainer1.Panel2.Controls.Add(label4);
                splitContainer1.Panel2.Controls.Add(instanceEnabled);
                splitContainer1.Panel2.Controls.Add(entityFilterBtn);
                splitContainer1.Panel2.Controls.Add(entityFilterBox);
                splitContainer1.Panel2.Controls.Add(latOffsetBox);
                splitContainer1.Panel2.Controls.Add(label8);
                splitContainer1.Panel2.Controls.Add(label9);
                splitContainer1.Panel2.Controls.Add(lonOffsetBox);
                splitContainer1.Panel2.Controls.Add(lonOffset);
                splitContainer1.Panel2.Controls.Add(maxCountBox);
                splitContainer1.Panel2.Controls.Add(tableNameBox);
                splitContainer1.Panel2.Controls.Add(instanceNameBox);
                splitContainer1.Panel2.Controls.Add(label3);
                splitContainer1.Panel2.Controls.Add(pluginNameBox);
                splitContainer1.Panel2.Controls.Add(label2);
                splitContainer1.Panel2.Controls.Add(label1);
                splitContainer1.Panel2.Controls.Add(enterBtn);
                splitContainer1.Panel2.Controls.Add(panelType);



            }
            else if (Resource.DllList[pluginName].PType == PluginType.RemoteIP)
            {
                splitContainer1.Panel2.Controls.Clear();
                System.Windows.Forms.ComboBox pluginNameBox;
                System.Windows.Forms.Label label2;
                System.Windows.Forms.Label label1;
                System.Windows.Forms.TextBox instanceNameBox;
                System.Windows.Forms.Label label3;
                //System.Windows.Forms.ComboBox protocolBox;
                //System.Windows.Forms.Label label4;
                System.Windows.Forms.TextBox portBox;
                System.Windows.Forms.Label label7;
                System.Windows.Forms.TextBox ipBox;
                System.Windows.Forms.Label label5;
                //System.Windows.Forms.ComboBox netProtocolBox;
                //System.Windows.Forms.Label label6;
                System.Windows.Forms.TextBox latOffsetBox;
                System.Windows.Forms.Label label9;
                System.Windows.Forms.TextBox lonOffsetBox;
                System.Windows.Forms.Label lonOffset;
                System.Windows.Forms.CheckBox instanceEnabled;
                System.Windows.Forms.Button entityFilterBtn;
                System.Windows.Forms.TextBox entityFilterBox;
                System.Windows.Forms.Label label8;
                System.Windows.Forms.Button enterBtn;
                System.Windows.Forms.Label panelType;


                panelType = new System.Windows.Forms.Label();
                enterBtn = new System.Windows.Forms.Button();
                label1 = new System.Windows.Forms.Label();
                label2 = new System.Windows.Forms.Label();
                pluginNameBox = new System.Windows.Forms.ComboBox();
                label3 = new System.Windows.Forms.Label();
                instanceNameBox = new System.Windows.Forms.TextBox();
                //label4 = new System.Windows.Forms.Label();
                //protocolBox = new System.Windows.Forms.ComboBox();
                label5 = new System.Windows.Forms.Label();
                ipBox = new System.Windows.Forms.TextBox();
                label7 = new System.Windows.Forms.Label();
                portBox = new System.Windows.Forms.TextBox();
                //label6 = new System.Windows.Forms.Label();
                //netProtocolBox = new System.Windows.Forms.ComboBox();
                lonOffset = new System.Windows.Forms.Label();
                lonOffsetBox = new System.Windows.Forms.TextBox();
                label9 = new System.Windows.Forms.Label();
                latOffsetBox = new System.Windows.Forms.TextBox();
                label8 = new System.Windows.Forms.Label();
                entityFilterBox = new System.Windows.Forms.TextBox();
                entityFilterBtn = new System.Windows.Forms.Button();
                instanceEnabled = new System.Windows.Forms.CheckBox();

                // 
                // enterBtn
                // 
                enterBtn.Location = new System.Drawing.Point(250, 496);
                enterBtn.Name = "enterBtn";
                enterBtn.Size = new System.Drawing.Size(75, 23);
                enterBtn.TabIndex = 2;
                enterBtn.Text = Resource.lHelper.Key("n39");
                enterBtn.UseVisualStyleBackColor = true;
                enterBtn.Click += new EventHandler(enterBtn_Click);

                panelType.AutoSize = true;
                panelType.Location = new System.Drawing.Point(96, 122);
                panelType.Name = "panelType";
                panelType.Size = new System.Drawing.Size(41, 12);
                panelType.TabIndex = 0;
                panelType.Text = "pluginTran";
                panelType.Visible = false;


                // 
                // label1
                // 
                label1.AutoSize = true;
                label1.Location = new System.Drawing.Point(23, 14);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(101, 12);
                label1.TabIndex = 0;
                label1.Text = Resource.lHelper.Key("m21");    //han modify
                // 
                // label2
                // 
                label2.AutoSize = true;
                label2.Location = new System.Drawing.Point(39, 55);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(77, 12);
                label2.TabIndex = 1;
                label2.Text = Resource.lHelper.Key("n55");
                // 
                // pluginNameBox
                // 
                pluginNameBox.FormattingEnabled = true;
                pluginNameBox.Location = new System.Drawing.Point(180, 51);
                pluginNameBox.Name = "pluginNameBox";
                pluginNameBox.Size = new System.Drawing.Size(151, 20);
                pluginNameBox.TabIndex = 2;
                pluginNameBox.DropDownStyle = ComboBoxStyle.DropDownList;

                pluginNameBox.Items.Clear();

                pluginNameBox.Items.Add("UDP_Version_0");
                foreach (string s in Resource.DllList.Keys)
                {
                    if (s.Contains("UDP") || s.Contains("TCP"))
                    {
                        pluginNameBox.Items.Add(s);
                    }
                }
                pluginNameBox.SelectedItem = pluginName;
                pluginNameBox.SelectedValueChanged += new EventHandler(pluginName_SelectedIndexChanged);




                // 
                // label3
                // 
                label3.AutoSize = true;
                label3.Location = new System.Drawing.Point(41, 90);
                label3.Name = "label3";
                label3.Size = new System.Drawing.Size(53, 12);
                label3.TabIndex = 3;
                label3.Text = Resource.lHelper.Key("n49");
                // 
                // instanceNameBox
                // 
                instanceNameBox.Location = new System.Drawing.Point(180, 84);
                instanceNameBox.Name = "instanceNameBox";
                instanceNameBox.Size = new System.Drawing.Size(151, 21);
                instanceNameBox.TabIndex = 4;
                // 
                // protocolBox
                // 

                //label4.AutoSize = true;
                //label4.Location = new System.Drawing.Point(39, 120);
                //label4.Name = "label4";
                //label4.Size = new System.Drawing.Size(53, 12);
                //label4.TabIndex = 1;
                //label4.Text = Resource.lHelper.Key("n18");
                // 
                // protocolBox
                // 
                //protocolBox.FormattingEnabled = true;
                //protocolBox.Location = new System.Drawing.Point(180, 116);
                //protocolBox.Name = "protocolBox";
                //protocolBox.Size = new System.Drawing.Size(151, 20);
                //protocolBox.TabIndex = 2;
                //protocolBox.Items.AddRange(new object[] { "PGIS" });
                //protocolBox.DropDownStyle = ComboBoxStyle.DropDownList;
                //protocolBox.SelectedIndex = 0;
                // 
                // label5
                // (41, 151);
                label5.AutoSize = true;
                label5.Location = new System.Drawing.Point(41, 121);
                label5.Name = "label5";
                label5.Size = new System.Drawing.Size(17, 12);
                label5.TabIndex = 3;
                label5.Text = Resource.lHelper.Key("n46");
                // 
                // ipBox
                // (180, 145);
                ipBox.Location = new System.Drawing.Point(180, 115);
                ipBox.Name = "ipBox";
                ipBox.Size = new System.Drawing.Size(151, 21);
                ipBox.TabIndex = 4;
                // 
                // label7
                // (41, 182);
                label7.AutoSize = true;
                label7.Location = new System.Drawing.Point(41, 152);
                label7.Name = "label7";
                label7.Size = new System.Drawing.Size(29, 12);
                label7.TabIndex = 3;
                label7.Text = Resource.lHelper.Key("n12");
                // 
                // portBox
                // (180, 176);
                portBox.Location = new System.Drawing.Point(180, 146);
                portBox.Name = "portBox";
                portBox.Size = new System.Drawing.Size(151, 21);
                portBox.TabIndex = 4;
                // 
                // netProtocolBox
                // 
                //label6.AutoSize = true;
                //label6.Location = new System.Drawing.Point(39, 211);
                //label6.Name = "label6";
                //label6.Size = new System.Drawing.Size(53, 12);
                //label6.TabIndex = 1;
                //label6.Text = Resource.lHelper.Key("n50");
                // 
                // netProtocolBox
                // 
                //netProtocolBox.FormattingEnabled = true;
                //netProtocolBox.Location = new System.Drawing.Point(180, 207);
                //netProtocolBox.Name = "netProtocolBox";
                //netProtocolBox.Size = new System.Drawing.Size(151, 20);
                //netProtocolBox.TabIndex = 2;
                //netProtocolBox.Items.AddRange(new object[] { "UDP" });
                //netProtocolBox.DropDownStyle = ComboBoxStyle.DropDownList;
                //netProtocolBox.SelectedIndex = 0;
                // 
                // lonOffset 经度偏移
                // (41, 246);
                lonOffset.AutoSize = true;
                lonOffset.Location = new System.Drawing.Point(41, 186);
                lonOffset.Name = "lonOffset";
                lonOffset.Size = new System.Drawing.Size(53, 12);
                lonOffset.TabIndex = 3;
                lonOffset.Text = Resource.lHelper.Key("n51");
                // 
                // lonOffsetBox
                // (180, 240);
                lonOffsetBox.Location = new System.Drawing.Point(180, 180);
                lonOffsetBox.Name = "lonOffsetBox";
                lonOffsetBox.Size = new System.Drawing.Size(151, 21);
                lonOffsetBox.TabIndex = 4;
                // 
                // label9
                // (41, 278);
                label9.AutoSize = true;
                label9.Location = new System.Drawing.Point(41, 218);
                label9.Name = "label9";
                label9.Size = new System.Drawing.Size(53, 12);
                label9.TabIndex = 3;
                label9.Text = Resource.lHelper.Key("n52");
                // 
                // latOffsetBox
                // (180, 272);
                latOffsetBox.Location = new System.Drawing.Point(180, 212);
                latOffsetBox.Name = "latOffsetBox";
                latOffsetBox.Size = new System.Drawing.Size(151, 21);
                latOffsetBox.TabIndex = 4;
                // 
                // label8
                // (41, 313);
                label8.AutoSize = true;
                label8.Location = new System.Drawing.Point(41, 253);
                label8.Name = "label8";
                label8.Size = new System.Drawing.Size(53, 12);
                label8.TabIndex = 3;
                label8.Text = Resource.lHelper.Key("n24");
                // 
                // entityFilterBox
                // (180, 307);
                entityFilterBox.Location = new System.Drawing.Point(180, 247);
                entityFilterBox.Name = "entityFilterBox";
                entityFilterBox.Size = new System.Drawing.Size(151, 21);
                entityFilterBox.TabIndex = 4;
                entityFilterBox.Enabled = false;
                // 
                // entityFilterBtn
                // (340, 305);
                entityFilterBtn.Location = new System.Drawing.Point(340, 245);
                entityFilterBtn.Name = "entityFilterBtn";
                entityFilterBtn.Size = new System.Drawing.Size(37, 23);
                entityFilterBtn.TabIndex = 5;
                entityFilterBtn.Text = "...";
                entityFilterBtn.UseVisualStyleBackColor = true;
                entityFilterBtn.Click += new EventHandler(SelectEntity);
                // 
                // instanceEnabled
                // (180, 345);
                instanceEnabled.AutoSize = true;
                instanceEnabled.Location = new System.Drawing.Point(180, 285);
                instanceEnabled.Name = "instanceEnabled";
                instanceEnabled.Size = new System.Drawing.Size(48, 16);
                instanceEnabled.TabIndex = 6;
                instanceEnabled.Text = Resource.lHelper.Key("n48");
                instanceEnabled.UseVisualStyleBackColor = true;


                splitContainer1.Panel2.Controls.Add(instanceEnabled);
                splitContainer1.Panel2.Controls.Add(entityFilterBtn);
                splitContainer1.Panel2.Controls.Add(entityFilterBox);
                splitContainer1.Panel2.Controls.Add(latOffsetBox);
                splitContainer1.Panel2.Controls.Add(label8);
                splitContainer1.Panel2.Controls.Add(portBox);
                splitContainer1.Panel2.Controls.Add(label9);
                splitContainer1.Panel2.Controls.Add(label7);
                splitContainer1.Panel2.Controls.Add(lonOffsetBox);
                splitContainer1.Panel2.Controls.Add(lonOffset);
                splitContainer1.Panel2.Controls.Add(ipBox);
                splitContainer1.Panel2.Controls.Add(label5);
                splitContainer1.Panel2.Controls.Add(instanceNameBox);
                splitContainer1.Panel2.Controls.Add(label3);
                //splitContainer1.Panel2.Controls.Add(netProtocolBox);
                //splitContainer1.Panel2.Controls.Add(label6);
                //splitContainer1.Panel2.Controls.Add(protocolBox);
                //splitContainer1.Panel2.Controls.Add(label4);
                splitContainer1.Panel2.Controls.Add(pluginNameBox);
                splitContainer1.Panel2.Controls.Add(label2);
                splitContainer1.Panel2.Controls.Add(label1);

                splitContainer1.Panel2.Controls.Add(enterBtn);
                splitContainer1.Panel2.Controls.Add(panelType);

            }
            else
            {

                MessageBox.Show(Resource.lHelper.Key("m59"));
                Logger.Error("error occur when add plugin instance,error message:unknown plugin type,pluginName:" + pluginName);
                return;

            }



        }





        //相应确定按钮或者添加按钮的事件函数
        private void enterBtn_Click(object sender, EventArgs e)
        {

            if (((Label)(splitContainer1.Panel2.Controls["panelType"])).Text.Equals("general"))
            {
                string s = (string)((ComboBox)splitContainer1.Panel2.Controls["languageBox"]).SelectedItem;
                int logKeepDays = 0;
                Regex reg = new Regex("^\\d+$");
                if (!reg.IsMatch(splitContainer1.Panel2.Controls["logKeepBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m68"));
                    return;
                }
                //整数长度限制不能超过9位
                if (splitContainer1.Panel2.Controls["logKeepBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n73") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;
                }

                logKeepDays = Convert.ToInt32(splitContainer1.Panel2.Controls["logKeepBox"].Text.Trim());
                if (logKeepDays < 1)
                {
                    MessageBox.Show(Resource.lHelper.Key("m68"));
                    return;

                }


                try
                {
                    RegistryKey Local = Registry.LocalMachine;
                    RegistryKey runKey = Local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\");


                    if (((CheckBox)splitContainer1.Panel2.Controls["autoRunBox"]).Checked)
                    {
                        runKey.SetValue("EastcomGPSTran", AppDomain.CurrentDomain.BaseDirectory + "GPSTran.exe");
                    }
                    else
                    {
                        runKey.DeleteValue("EastcomGPSTran", false);

                    }
                    new XMLIni().ModifyLanguage(s);
                    new XMLIni().ModifyLogKeepDays(logKeepDays);
                    MessageBox.Show(Resource.lHelper.Key("m31"));
                }
                catch (Exception ex)
                {
                    Logger.Error("error occur when modify general language,error message:" + ex.ToString());
                    MessageBox.Show(Resource.lHelper.Key("m32"));
                    return;
                }



            }
            else if (((Label)(splitContainer1.Panel2.Controls["panelType"])).Text.Equals("listeningPort"))
            {
                //listeningPort can not be null or empty
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["listeningPort"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m28"));
                    return;
                }
                //参数长度不能过长
                if (splitContainer1.Panel2.Controls["listeningPort"].Text.Trim().Length > 6)
                {
                    MessageBox.Show(Resource.lHelper.Key("n12") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }

                Regex reg = new Regex("^\\d+$");
                if (!reg.IsMatch(splitContainer1.Panel2.Controls["listeningPort"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m29"));
                    return;
                }


                if (Convert.ToInt32(splitContainer1.Panel2.Controls["listeningPort"].Text.Trim()) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["listeningPort"].Text.Trim()) > 65534)
                {
                    MessageBox.Show(Resource.lHelper.Key("m30"));
                    return;
                }


                //todo: modify listeningPort 
                int port = Convert.ToInt32(splitContainer1.Panel2.Controls["listeningPort"].Text);
                try
                {
                    XMLIni ini = new XMLIni();
                    ini.ModifyPort(port);
                    MessageBox.Show(Resource.lHelper.Key("m31"));
                }
                catch (Exception ex)
                {
                    Logger.Error("error occur when modify ListeningPort,error message:" + ex.ToString());
                    MessageBox.Show(Resource.lHelper.Key("m32"));
                    return;
                }


            }
            else if (((Label)(splitContainer1.Panel2.Controls["panelType"])).Text.Equals("webGis"))
            {
                //判断IP格式
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisIP"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m33"));
                    return;

                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["webGisIP"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m34"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisDB"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m35"));
                    return;

                }
                if (splitContainer1.Panel2.Controls["webGisDB"].Text.Trim().ToUpper().Equals("DAGDB_TRAN"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m36"));
                    return;

                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisUser"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m37"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisPwd"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m38"));
                    return;

                }

                if (splitContainer1.Panel2.Controls["webGisPort"].Text.Trim().Length > 6)
                {
                    MessageBox.Show(Resource.lHelper.Key("n42") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisPort"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m28"));
                    return;

                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["webGisPort"].Text.Trim(), "^\\d+$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m29"));
                    return;

                }
                if (Convert.ToInt32(splitContainer1.Panel2.Controls["webGisPort"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["webGisPort"].Text) > 65534)
                {
                    MessageBox.Show(Resource.lHelper.Key("m30"));
                    return;
                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisInstance"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m51"));
                    return;

                }
                //todo: modify webGis config
                string ip = splitContainer1.Panel2.Controls["webGisIP"].Text.Trim();
                string catalog = splitContainer1.Panel2.Controls["webGisDB"].Text.Trim();
                string user = splitContainer1.Panel2.Controls["webGisUser"].Text.Trim();
                string pwd = splitContainer1.Panel2.Controls["webGisPwd"].Text.Trim();
                string port = splitContainer1.Panel2.Controls["webGisPort"].Text.Trim();
                string instance = splitContainer1.Panel2.Controls["webGisInstance"].Text.Trim();

                try
                {
                    XMLIni ini = new XMLIni();
                    ini.ModifyWebGis(ip, catalog, user, pwd, port, instance);
                    MessageBox.Show(Resource.lHelper.Key("m31"));

                    Resource.WebGisIp = ip;
                    Resource.WebGisDb = catalog;
                    Resource.WebGisUser = user;
                    Resource.WebGisPwd = pwd;
                    Resource.WebGisInstance = instance;
                    Resource.WebGisPort = Convert.ToInt32(port);
                    Resource.WebGisConn = string.Format("Data Source={0},{1}\\{2};Initial Catalog={3};uid={4};pwd={5};pooling=true;min pool size =1;max pool size=50", Resource.WebGisIp, Resource.WebGisPort, Resource.WebGisInstance, Resource.WebGisDb, Resource.WebGisUser, Resource.WebGisPwd);
                    ///加入提示重启程序
                    MessageBox.Show(Resource.lHelper.Key("m75"), Resource.lHelper.Key("m77"));

                }
                catch (Exception ex)
                {

                    Logger.Error("error occur when modify WEBGIS,error message:" + ex.ToString());
                    MessageBox.Show(Resource.lHelper.Key("m32"));
                    return;

                }



            }
            else if (((Label)(splitContainer1.Panel2.Controls["panelType"])).Text.Equals("gpsTran"))
            {
                //判断IP格式
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranIP"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m33"));
                    return;

                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["tranIP"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m34"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranDB"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m35"));
                    return;

                }
                if (splitContainer1.Panel2.Controls["tranDB"].Text.Trim().ToUpper().Equals("DAGDB"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m40"));
                    return;

                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranUser"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m37"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranPwd"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m38"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranPort"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m28"));
                    return;

                }

                if (splitContainer1.Panel2.Controls["tranPort"].Text.Trim().Length > 6)
                {
                    MessageBox.Show(Resource.lHelper.Key("n42") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }


                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["tranPort"].Text.Trim(), "^\\d+$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m29"));
                    return;

                }
                if (Convert.ToInt32(splitContainer1.Panel2.Controls["tranPort"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["tranPort"].Text) > 65534)
                {
                    MessageBox.Show(Resource.lHelper.Key("m30"));
                    return;
                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranInstance"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m51"));
                    return;

                }
                //todo: modify tran config

                string ip = splitContainer1.Panel2.Controls["tranIP"].Text.Trim();
                string catalog = splitContainer1.Panel2.Controls["tranDB"].Text.Trim();
                string user = splitContainer1.Panel2.Controls["tranUser"].Text.Trim();
                string pwd = splitContainer1.Panel2.Controls["tranPwd"].Text.Trim();
                string port = splitContainer1.Panel2.Controls["tranPort"].Text.Trim();
                string instance = splitContainer1.Panel2.Controls["tranInstance"].Text.Trim();

                try
                {
                    XMLIni ini = new XMLIni();
                    ini.ModifyTran(ip, catalog, user, pwd, port, instance);
                    MessageBox.Show(Resource.lHelper.Key("m31"));

                    Resource.TranIp = ip;
                    Resource.TranDb = catalog;
                    Resource.TranUser = user;
                    Resource.TranPwd = pwd;
                    Resource.TranInstance = instance;
                    Resource.TranPort = Convert.ToInt32(port);

                    Resource.TranConn = string.Format("Data Source={0},{1}\\{2};Initial Catalog={3};uid={4};pwd={5};pooling=true;min pool size =1;max pool size=50", Resource.TranIp, Resource.TranPort, Resource.TranInstance, Resource.TranDb, Resource.TranUser, Resource.TranPwd);
                    ///加入提示重启程序
                    MessageBox.Show(Resource.lHelper.Key("m75"), Resource.lHelper.Key("m77"));
                }
                catch (Exception ex)
                {

                    Logger.Error("error occur when modify Tran,error message:" + ex.ToString());
                    MessageBox.Show(Resource.lHelper.Key("m32"));
                    return;

                }




            }
            else if (((Label)(splitContainer1.Panel2.Controls["panelType"])).Text.Equals("userInfo"))
            {
                //todo: modify userinfo config
                if (string.IsNullOrEmpty(((TextBox)(splitContainer1.Panel2.Controls["flushInterval"])).Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m41"));
                    return;
                }
                if (splitContainer1.Panel2.Controls["flushInterval"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n47") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;
                }


                if (!Regex.IsMatch(((TextBox)(splitContainer1.Panel2.Controls["flushInterval"])).Text.Trim(), @"^\d+$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m42"));
                    return;
                }
                //todo: modify userInfo config

                bool enabled = ((CheckBox)(splitContainer1.Panel2.Controls["InstanceEnabled"])).Checked;
                int flushInterval = Convert.ToInt32(splitContainer1.Panel2.Controls["flushInterval"].Text.Trim());

                if (flushInterval <= 0)
                {
                    MessageBox.Show(Resource.lHelper.Key("m42"));
                    return;

                }

                try
                {
                    XMLIni ini = new XMLIni();
                    ini.ModifyUserInfo(enabled, flushInterval);
                    Resource.FlushInterval = flushInterval;
                    Resource.UserinfoEnabled = enabled;
                    Form1.userInfoThread.Safe_Stop();
                    Form1.userInfoThread.Start();

                    MessageBox.Show(Resource.lHelper.Key("m31"));

                }
                catch (Exception ex)
                {

                    Logger.Error("error occur when modify UserInfo,error message:" + ex.ToString());
                    MessageBox.Show(Resource.lHelper.Key("m32"));
                    return;

                }

            }
            else if (((Label)(splitContainer1.Panel2.Controls["panelType"])).Text.Equals("ipTran"))
            {
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m51"));
                    return;

                }
                //判断IP格式
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["ipBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m33"));
                    return;

                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["ipBox"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m34"));
                    return;

                }

                if (splitContainer1.Panel2.Controls["portBox"].Text.Trim().Length > 6)
                {
                    MessageBox.Show(Resource.lHelper.Key("n12") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }


                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["portBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m28"));
                    return;

                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["portBox"].Text.Trim(), "^\\d+$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m29"));
                    return;

                }
                if (Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text.Trim()) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text) > 65534)
                {
                    MessageBox.Show(Resource.lHelper.Key("m30"));
                    return;
                }

                if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m43"));
                    return;

                }
                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m44"));
                    return;

                }
                if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                {
                    MessageBox.Show(Resource.lHelper.Key("m45"));
                    return;

                }

                if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;
                }


                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m46"));
                    return;

                }
                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m47"));
                    return;

                }
                if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                {
                    MessageBox.Show(Resource.lHelper.Key("m48"));
                    return;

                }

                IPModel ipModel = new IPModel();
                ipModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();
                foreach (string s in Resource.IPList.Keys)
                {
                    if (s.ToUpper().Equals(ipModel.Name.ToUpper()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m49"));
                        return;
                    }
                }
                ipModel.Ip = splitContainer1.Panel2.Controls["ipBox"].Text.Trim();
                ipModel.Port = Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text.Trim());
                //ip port check
                foreach (IPManager im in Resource.IPList.Values)
                {
                    if (im.GetModel().Port == ipModel.Port && im.GetModel().Ip.Equals(ipModel.Ip))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m50"));
                        return;
                    }

                }
                foreach (PluginManager pManager in Resource.PluginList.Values)
                {
                    if (pManager.GetPluginType() == PluginType.RemoteIP)
                    {
                        if (pManager.GetPluginModel().Ip.Equals(ipModel.Ip) && pManager.GetPluginModel().Port == ipModel.Port)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m50"));
                            return;
                        }

                    }

                }

                ipModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                ipModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());
                //ipModel.Protocol = (string)((ComboBox)splitContainer1.Panel2.Controls["protocolBox"]).SelectedItem;
                //ipModel.NetProtocol = (string)((ComboBox)splitContainer1.Panel2.Controls["netProtocolBox"]).SelectedItem;
                ipModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;
                string entityStr = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim();

                List<int> entityList = new List<int>();
                if (!string.IsNullOrEmpty(entityStr))
                {
                    string[] temp = entityStr.Split(new char[] { ',' });
                    if (temp != null)
                    {
                        foreach (string s in temp)
                        {
                            entityList.Add(Convert.ToInt32(s));
                        }
                    }
                }
                ipModel.EntityID = entityList;
                ipModel.IpAvailable = new StatusDetect().IpDetect(ipModel.EndPoint);

                if (!ipModel.Enabled)
                {
                    try
                    {
                        new XMLIni().AddIPInstance(ipModel);
                        Resource.IPList.Add(ipModel.Name, new IPManager(ipModel, false));

                        TreeNode no = new TreeNode();
                        no.Name = ipModel.Name;
                        no.Tag = ipModel.Name;
                        no.Text = ipModel.Name;
                        treeView1.Nodes["root"].Nodes["ipTran"].Nodes.Add(no);
                        MessageBox.Show(Resource.lHelper.Key("m65"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m66"));
                        Logger.Error("error occur when add Iptransfer instance,error message:" + ex.ToString());
                        return;
                    }
                }
                else
                {
                    try
                    {
                        new XMLIni().AddIPInstance(ipModel);
                        Resource.IPList.Add(ipModel.Name, new IPManager(ipModel, false));
                        lock (Resource.lckIPInstanceList)
                        {
                            Resource.IPInstanceList.Add(ipModel.Name, new IPManager(ipModel, true));
                        }


                        TreeNode no = new TreeNode();
                        no.Name = ipModel.Name;
                        no.Tag = ipModel.Name;
                        no.Text = ipModel.Name;
                        treeView1.Nodes["root"].Nodes["ipTran"].Nodes.Add(no);
                        MessageBox.Show(Resource.lHelper.Key("m65"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m66"));
                        Logger.Error("error occur when add Iptransfer instance,error message:" + ex.ToString());
                        return;
                    }


                }




            }
            else if (((Label)(splitContainer1.Panel2.Controls["panelType"])).Text.Equals("ipTranInstance"))
            {
                string instanceName = ((Label)splitContainer1.Panel2.Controls["oldInstanceName"]).Text.Trim();

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m51"));
                    return;

                }
                //判断IP格式
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["ipBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m33"));
                    return;

                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["ipBox"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m34"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["portBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m28"));
                    return;

                }

                if (splitContainer1.Panel2.Controls["portBox"].Text.Trim().Length > 6)
                {
                    MessageBox.Show(Resource.lHelper.Key("n12") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }


                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["portBox"].Text.Trim(), "^\\d+$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m29"));
                    return;

                }
                if (Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text) > 65534)
                {
                    MessageBox.Show(Resource.lHelper.Key("m30"));
                    return;
                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m43"));
                    return;

                }

                if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }


                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.?\d*[1-9]\d*|0?\.?0+|0)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m44"));
                    return;

                }
                if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                {
                    MessageBox.Show(Resource.lHelper.Key("m45"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m46"));
                    return;

                }
                if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }


                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.?\d*[1-9]\d*|0?\.?0+|0)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m47"));
                    return;

                }
                if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                {
                    MessageBox.Show(Resource.lHelper.Key("m48"));
                    return;

                }
                string oldInstanceName = splitContainer1.Panel2.Controls["oldInstanceName"].Text.Trim();


                IPModel ipModel = new IPModel();
                ipModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();
                //instance name check 
                foreach (string s in Resource.IPList.Keys)
                {
                    if (!oldInstanceName.ToUpper().Equals(s.ToUpper()))
                    {
                        if (s.ToUpper().Equals(ipModel.Name.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m49"));
                            return;
                        }
                    }
                }
                ipModel.Ip = splitContainer1.Panel2.Controls["ipBox"].Text.Trim();
                ipModel.Port = Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text.Trim());
                //ip port check
                foreach (IPManager im in Resource.IPList.Values)
                {
                    if (!oldInstanceName.ToUpper().Equals(im.GetModel().Name.ToUpper()))
                    {
                        if (im.GetModel().Port == ipModel.Port && im.GetModel().Ip.Equals(ipModel.Ip))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m50"));
                            return;
                        }
                    }
                }
                foreach (PluginManager pManager in Resource.PluginList.Values)
                {
                    if (pManager.GetPluginType() == PluginType.RemoteIP)
                    {
                        if (pManager.GetPluginModel().Ip.Equals(ipModel.Ip) && pManager.GetPluginModel().Port == ipModel.Port)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m50"));
                            return;
                        }

                    }

                }

                ipModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                ipModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());
                //ipModel.Protocol = (string)((ComboBox)splitContainer1.Panel2.Controls["protocolBox"]).SelectedItem;
                //ipModel.NetProtocol = (string)((ComboBox)splitContainer1.Panel2.Controls["netProtocolBox"]).SelectedItem;
                ipModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;
                string entityStr = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim();

                List<int> entityList = new List<int>();
                if (!string.IsNullOrEmpty(entityStr))
                {
                    string[] temp = entityStr.Split(new char[] { ',' });
                    if (temp != null)
                    {
                        foreach (string s in temp)
                        {
                            entityList.Add(Convert.ToInt32(s));
                        }
                    }
                }
                ipModel.EntityID = entityList;
                ipModel.IpAvailable = new StatusDetect().IpDetect(ipModel.EndPoint);

                if (!ipModel.Enabled)
                {
                    try
                    {
                        new XMLIni().ModifyIPInstance(oldInstanceName, ipModel);
                        Resource.IPList.Remove(oldInstanceName);
                        Resource.IPList.Add(ipModel.Name, new IPManager(ipModel, false));
                        lock (Resource.lckIPInstanceList)
                        {
                            if (Resource.IPInstanceList.ContainsKey(oldInstanceName))
                            {
                                Resource.IPInstanceList[oldInstanceName].SetEnabled(false);
                                Resource.IPInstanceList.Remove(oldInstanceName);
                            }
                        }


                        int index = treeView1.Nodes["root"].Nodes["ipTran"].Nodes.IndexOf(treeView1.SelectedNode);
                        TreeNode no = new TreeNode();
                        no.Name = ipModel.Name;
                        no.Text = ipModel.Name;
                        no.Tag = ipModel.Name;
                        treeView1.Nodes["root"].Nodes["ipTran"].Nodes.RemoveAt(index);
                        treeView1.Nodes["root"].Nodes["ipTran"].Nodes.Insert(index, no);
                        treeView1.SelectedNode = no;

                        splitContainer1.Panel2.Controls["oldInstanceName"].Text = ipModel.Name;
                        MessageBox.Show(Resource.lHelper.Key("m31"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m32"));
                        Logger.Error("error occur when modify Iptransfer instance,error message:" + ex.ToString());
                        return;
                    }
                }
                else
                {
                    try
                    {
                        new XMLIni().ModifyIPInstance(oldInstanceName, ipModel);
                        Resource.IPList.Remove(oldInstanceName);
                        Resource.IPList.Add(ipModel.Name, new IPManager(ipModel, false));
                        lock (Resource.lckIPInstanceList)
                        {
                            if (Resource.IPInstanceList.ContainsKey(oldInstanceName))
                            {
                                Resource.IPInstanceList[oldInstanceName].SetEnabled(false);
                                Resource.IPInstanceList.Remove(oldInstanceName);
                            }
                            Resource.IPInstanceList.Add(ipModel.Name, new IPManager(ipModel, true));
                        }


                        int index = treeView1.Nodes["root"].Nodes["ipTran"].Nodes.IndexOf(treeView1.SelectedNode);
                        TreeNode no = new TreeNode();
                        no.Name = ipModel.Name;
                        no.Text = ipModel.Name;
                        no.Tag = ipModel.Name;
                        treeView1.Nodes["root"].Nodes["ipTran"].Nodes.RemoveAt(index);
                        treeView1.Nodes["root"].Nodes["ipTran"].Nodes.Insert(index, no);
                        treeView1.SelectedNode = no;

                        splitContainer1.Panel2.Controls["oldInstanceName"].Text = ipModel.Name;
                        MessageBox.Show(Resource.lHelper.Key("m31"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m32"));
                        Logger.Error("error occur when modify Iptransfer instance,error message:" + ex.ToString());
                        return;
                    }


                }


            }
            else if (splitContainer1.Panel2.Controls["panelType"].Text.Equals("dbTran"))
            {
                SqlHelper helper = new SqlHelper(Resource.TranConn);
                string sqlStr = "";

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m51"));
                    return;

                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m52"));
                    return;
                }
                if (splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim().Length > 50)
                {
                    MessageBox.Show(Resource.lHelper.Key("n53") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;
                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim(), @"[A-Za-z]+(_*)(\d*)"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m78"));
                    return;
                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m53"));
                    return;

                }
                if (splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n54") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;
                }
                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim(), "^[1-9]\\d*$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m54"));
                    return;
                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m43"));
                    return;

                }
                if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }



                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m44"));
                    return;

                }
                if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                {
                    MessageBox.Show(Resource.lHelper.Key("m45"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m46"));
                    return;

                }
                if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m47"));
                    return;

                }
                if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                {
                    MessageBox.Show(Resource.lHelper.Key("m48"));
                    return;

                }

                DBModel dModel = new DBModel();

                dModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();
                foreach (DBManager dm in Resource.DBList.Values)
                {
                    if (dm.GetModel().Name.ToUpper().Equals(dModel.Name.ToUpper()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m49"));
                        return;
                    }

                }


                dModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;
                dModel.TableName = splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim();
                foreach (DBManager dm in Resource.DBList.Values)
                {
                    if (dm.GetModel().TableName.ToUpper().Equals(dModel.TableName.ToUpper()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m55"));
                        return;
                    }
                }

                foreach (PluginManager pm in Resource.PluginList.Values)
                {
                    if (pm.GetPluginType() == PluginType.TranDB)
                    {
                        if (pm.GetPluginModel().TableName.ToUpper().Equals(dModel.TableName.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m55"));
                            return;
                        }

                    }
                }

                sqlStr = "select count(0) from sysobjects where xtype='U' and name='" + dModel.TableName + "'";
                //check table when change table name

                try
                {
                    DataTable dt = helper.ExecuteRead(CommandType.Text, sqlStr, "test", null);
                    if (dt == null)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m73"));
                        return;
                    }
                    if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n53") + " " + dModel.TableName + " " + Resource.lHelper.Key("m72"));
                        return;
                    }

                }
                catch (Exception ex)
                {

                    Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                    MessageBox.Show(Resource.lHelper.Key("m73"));
                    return;
                }




                dModel.MaxCount = Convert.ToInt32(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim());
                dModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                dModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());

                string entityStr = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim();

                List<int> entityList = new List<int>();
                if (!string.IsNullOrEmpty(entityStr))
                {
                    string[] temp = entityStr.Split(new char[] { ',' });
                    if (temp != null)
                    {
                        foreach (string s in temp)
                        {
                            entityList.Add(Convert.ToInt32(s));
                        }
                    }
                }
                dModel.EntityID = entityList;

                if (!dModel.Enabled)
                {
                    try
                    {
                        new XMLIni().AddDBInstance(dModel);
                        Resource.DBList.Add(dModel.Name, new DBManager(dModel, false));
                        TreeNode no = new TreeNode();
                        no.Name = dModel.Name;
                        no.Tag = dModel.Name;
                        no.Text = dModel.Name;
                        treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Add(no);
                        MessageBox.Show(Resource.lHelper.Key("m65"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m66"));
                        Logger.Error("error occur when add DBTransfer instance,error message:" + ex.ToString());
                        return;
                    }
                }
                else
                {
                    try
                    {
                        new XMLIni().AddDBInstance(dModel);
                        Resource.DBList.Add(dModel.Name, new DBManager(dModel, false));
                        lock (Resource.lckDBInstanceList)
                        {
                            Resource.DBInstanceList.Add(dModel.Name, new DBManager(dModel, true));
                        }

                        TreeNode no = new TreeNode();
                        no.Name = dModel.Name;
                        no.Tag = dModel.Name;
                        no.Text = dModel.Name;
                        treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Add(no);
                        MessageBox.Show(Resource.lHelper.Key("m65"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m66"));
                        Logger.Error("error occur when add DBTransfer instance,error message:" + ex.ToString());
                        return;
                    }


                }





            }
            else if (splitContainer1.Panel2.Controls["panelType"].Text.Equals("dbTranInstance"))
            {
                SqlHelper helper = new SqlHelper(Resource.TranConn);
                string sqlStr = "";

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m51"));
                    return;

                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m52"));
                    return;

                }
                if (splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim().Length > 50)
                {
                    MessageBox.Show(Resource.lHelper.Key("n53") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;
                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m53"));
                    return;
                }

                if (splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n54") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;
                }


                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim(), "^[1-9]\\d*$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m54"));
                    return;

                }

                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m43"));
                    return;

                }

                if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }

                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m44"));
                    return;

                }
                if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                {
                    MessageBox.Show(Resource.lHelper.Key("m45"));
                    return;

                }
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                {
                    MessageBox.Show(Resource.lHelper.Key("m46"));
                    return;

                }
                if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                {
                    MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                    return;

                }


                if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                {
                    MessageBox.Show(Resource.lHelper.Key("m47"));
                    return;

                }
                if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                {
                    MessageBox.Show(Resource.lHelper.Key("m48"));
                    return;

                }
                string oldInstanceName = splitContainer1.Panel2.Controls["oldInstanceName"].Text.Trim();

                DBModel dModel = new DBModel();

                dModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();
                foreach (DBManager dm in Resource.DBList.Values)
                {
                    if (!oldInstanceName.ToUpper().Equals(dm.GetModel().Name.ToUpper()))
                    {
                        if (dm.GetModel().Name.ToUpper().Equals(dModel.Name.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m49"));
                            return;
                        }
                    }
                }





                dModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;
                dModel.TableName = splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim();
                foreach (DBManager dm in Resource.DBList.Values)
                {
                    if (!oldInstanceName.ToUpper().Equals(dm.GetModel().Name.ToUpper()))
                    {
                        if (dm.GetModel().TableName.ToUpper().Equals(dModel.TableName.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m55"));
                            return;
                        }
                    }
                }

                foreach (PluginManager pm in Resource.PluginList.Values)
                {
                    if (pm.GetPluginType() == PluginType.TranDB)
                    {
                        if (pm.GetPluginModel().TableName.ToUpper().Equals(dModel.TableName.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m55"));
                            return;
                        }

                    }
                }

                sqlStr = "select count(0) from sysobjects where xtype='U' and name='" + dModel.TableName + "'";
                //check table when change table name
                if (!Resource.DBList[oldInstanceName].GetModel().TableName.ToUpper().Equals(dModel.TableName.ToUpper()))
                {
                    try
                    {
                        DataTable dt = helper.ExecuteRead(CommandType.Text, sqlStr, "test", null);
                        if (dt == null)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m73"));
                            return;
                        }
                        if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                        {
                            MessageBox.Show(Resource.lHelper.Key("n53") + " " + dModel.TableName + " " + Resource.lHelper.Key("m72"));
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                        MessageBox.Show(Resource.lHelper.Key("m73"));
                        return;

                    }
                }

                dModel.MaxCount = Convert.ToInt32(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim());
                dModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                dModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());

                string entityStr = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim();

                List<int> entityList = new List<int>();
                if (!string.IsNullOrEmpty(entityStr))
                {
                    string[] temp = entityStr.Split(new char[] { ',' });
                    if (temp != null)
                    {
                        foreach (string s in temp)
                        {
                            entityList.Add(Convert.ToInt32(s));
                        }
                    }
                }
                dModel.EntityID = entityList;

                if (!dModel.Enabled)
                {
                    try
                    {

                        sqlStr = "";
                        if (!Resource.DBList[oldInstanceName].GetModel().TableName.ToUpper().Equals(dModel.TableName.ToUpper()))
                        {
                            sqlStr = "if exists(select 0 from sysobjects where xtype='U' and name='" + Resource.DBList[oldInstanceName].GetModel().TableName + "') begin exec sp_rename '" + Resource.DBList[oldInstanceName].GetModel().TableName + "','" + dModel.TableName + "' end";

                        }

                        //rename old table
                        try
                        {
                            if (!string.IsNullOrEmpty(sqlStr))
                            {
                                helper.ExecuteNonQueryNonTran(CommandType.Text, sqlStr);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                            MessageBox.Show(Resource.lHelper.Key("m73"));
                            return;

                        }
                        new XMLIni().ModifyDBInstance(oldInstanceName, dModel);
                        Resource.DBList.Remove(oldInstanceName);
                        Resource.DBList.Add(dModel.Name, new DBManager(dModel, false));
                        lock (Resource.lckDBInstanceList)
                        {
                            if (Resource.DBInstanceList.ContainsKey(oldInstanceName))
                            {
                                Resource.DBInstanceList[oldInstanceName].SetEnabled(false);
                                Resource.DBInstanceList.Remove(oldInstanceName);
                            }
                        }

                        int index = treeView1.Nodes["root"].Nodes["dbTran"].Nodes.IndexOf(treeView1.SelectedNode);
                        TreeNode no = new TreeNode();
                        no.Name = dModel.Name;
                        no.Text = dModel.Name;
                        no.Tag = dModel.Name;
                        treeView1.Nodes["root"].Nodes["dbTran"].Nodes.RemoveAt(index);
                        treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Insert(index, no);
                        treeView1.SelectedNode = no;
                        splitContainer1.Panel2.Controls["oldInstanceName"].Text = dModel.Name;
                        MessageBox.Show(Resource.lHelper.Key("m31"));


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m32"));
                        Logger.Error("error occur when modify DBTransfer instance,error message:" + ex.ToString());
                        return;
                    }
                }
                else
                {
                    try
                    {


                        sqlStr = "";
                        if (!Resource.DBList[oldInstanceName].GetModel().TableName.ToUpper().Equals(dModel.TableName.ToUpper()))
                        {
                            sqlStr = "if exists(select 0 from sysobjects where xtype='U' and name='" + Resource.DBList[oldInstanceName].GetModel().TableName + "') begin exec sp_rename '" + Resource.DBList[oldInstanceName].GetModel().TableName + "','" + dModel.TableName + "' end";

                        }
                        //rename old table
                        try
                        {
                            if (!string.IsNullOrEmpty(sqlStr))
                            {
                                helper.ExecuteNonQueryNonTran(CommandType.Text, sqlStr);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                            MessageBox.Show(Resource.lHelper.Key("m73"));
                            return;

                        }
                        new XMLIni().ModifyDBInstance(oldInstanceName, dModel);
                        Resource.DBList.Remove(oldInstanceName);
                        Resource.DBList.Add(dModel.Name, new DBManager(dModel, false));
                        lock (Resource.lckDBInstanceList)
                        {
                            if (Resource.DBInstanceList.ContainsKey(oldInstanceName))
                            {
                                Resource.DBInstanceList[oldInstanceName].SetEnabled(false);
                                Resource.DBInstanceList.Remove(oldInstanceName);
                            }
                            Resource.DBInstanceList.Add(dModel.Name, new DBManager(dModel, true));
                        }



                        //init statistics and database status again

                        int index = treeView1.Nodes["root"].Nodes["dbTran"].Nodes.IndexOf(treeView1.SelectedNode);
                        TreeNode no = new TreeNode();
                        no.Name = dModel.Name;
                        no.Text = dModel.Name;
                        no.Tag = dModel.Name;
                        treeView1.Nodes["root"].Nodes["dbTran"].Nodes.RemoveAt(index);
                        treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Insert(index, no);
                        treeView1.SelectedNode = no;
                        splitContainer1.Panel2.Controls["oldInstanceName"].Text = dModel.Name;
                        MessageBox.Show(Resource.lHelper.Key("m31"));

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m32"));
                        Logger.Error("error occur when modify DBTransfer instance,error message:" + ex.ToString());
                        return;
                    }


                }





            }
            else if (splitContainer1.Panel2.Controls["panelType"].Text.Equals("pluginTran"))
            {
                string pluginName = splitContainer1.Panel2.Controls["pluginNameBox"].Text.Trim();

                if (!Resource.DllList.ContainsKey(pluginName))
                {
                    MessageBox.Show(Resource.lHelper.Key("m56"));
                    Logger.Error("error occur when add plugin instance,error message: unknown plugin name,plugin name:" + pluginName);
                    return;
                }
                //deal by the value of pluginType
                if (Resource.DllList[pluginName].PType == PluginType.RemoteIP)
                {
                    PluginModel pModel = new PluginModel();

                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m51"));
                        return;
                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["ipBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m33"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["ipBox"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m34"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["portBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m28"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["portBox"].Text.Trim().Length > 6)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n12") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["portBox"].Text.Trim(), "^\\d+$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m29"));
                        return;

                    }
                    if (Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text) > 65534)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m30"));
                        return;
                    }

                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m43"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m44"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m45"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m46"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m47"));
                        return;
                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m48"));
                        return;
                    }
                    pModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();


                    foreach (string str in Resource.PluginList.Keys)
                    {
                        if (str.ToUpper().Equals(pModel.Name.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m49"));
                            return;

                        }
                    }





                    pModel.DllModel = Resource.DllList[pluginName];
                    pModel.Ip = splitContainer1.Panel2.Controls["ipBox"].Text.Trim();
                    pModel.Port = Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text.Trim());

                    foreach (IPManager ipManager in Resource.IPList.Values)
                    {
                        if (pModel.Ip.Equals(ipManager.GetModel().Ip) && pModel.Port == ipManager.GetModel().Port)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m50"));
                            return;
                        }

                    }
                    foreach (PluginManager pManager in Resource.PluginList.Values)
                    {
                        if (pManager.GetPluginType() == PluginType.RemoteIP)
                        {
                            if (pManager.GetPluginModel().Ip.Equals(pModel.Ip) && pManager.GetPluginModel().Port == pModel.Port)
                            {
                                MessageBox.Show(Resource.lHelper.Key("m50"));
                                return;

                            }
                        }

                    }

                    //pModel.Protocol=(string)((ComboBox)splitContainer1.Panel2.Controls["protocolBox"]).SelectedItem;
                    //pModel.NetProtocol = (string)((ComboBox)splitContainer1.Panel2.Controls["netProtocolBox"]).SelectedItem;
                    pModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                    pModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());
                    List<int> entityList = new List<int>();
                    if (!string.IsNullOrEmpty(splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim()))
                    {
                        string[] sList = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim().Split(new char[] { ',' });

                        foreach (string s in sList)
                        {
                            try
                            {
                                entityList.Add(Convert.ToInt32(s));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("error occur when parse entityFilterBox string,error message:" + ex.ToString());
                                MessageBox.Show(Resource.lHelper.Key("m57"));
                                return;

                            }
                        }

                    }
                    pModel.EntityID = entityList;
                    pModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;


                    if (!pModel.Enabled)
                    {
                        try
                        {
                            new XMLIni().AddPluginInstance(pModel);
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Tag = pModel.Name;
                            no.Text = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Add(no);
                            treeView1.Nodes["root"].Nodes["ipTran"].Nodes.Add(no);
                            MessageBox.Show(Resource.lHelper.Key("m65"));     //han modify
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m66"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            new XMLIni().AddPluginInstance(pModel);

                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckDBInstanceList)
                            {
                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));
                            }

                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Tag = pModel.Name;
                            no.Text = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Add(no); //han modify
                            treeView1.Nodes["root"].Nodes["ipTran"].Nodes.Add(no);
                            MessageBox.Show(Resource.lHelper.Key("m65"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m66"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }


                    }




                }
                else if (Resource.DllList[pluginName].PType == PluginType.RemoteDB)
                {
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m51"));
                        return;

                    }
                    //判断IP格式
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m52"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim().Length > 50)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n53") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }


                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m53"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n54") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }


                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim(), "^[1-9]\\d*$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m54"));
                        return;

                    }


                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m43"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m44"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m45"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m46"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m47"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m48"));
                        return;

                    }
                    //判断IP格式
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteIPBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m33"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["remoteIPBox"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m34"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteCatalogBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m35"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteUserBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m37"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remotePwdBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m38"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m28"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim().Length > 6)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n42") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }


                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim(), "^\\d+$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m29"));
                        return;

                    }
                    if (Convert.ToInt32(splitContainer1.Panel2.Controls["remotePortBox"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["remotePortBox"].Text) > 65534)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m30"));
                        return;
                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteInstanceBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m51"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["citycode"].Text.Trim()) || !Regex.IsMatch(splitContainer1.Panel2.Controls["citycode"].Text.Trim(), @"^-?[0-9]\d*$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m79"));
                        return;
                    }

                    if (splitContainer1.Panel2.Controls["citycode"].Text.Trim().Length > 10)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m80"));
                        return;
                    }

                    PluginModel pModel = new PluginModel();

                    pModel.Citycode = Convert.ToInt32(splitContainer1.Panel2.Controls["citycode"].Text.Trim());

                    pModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();

                    foreach (string str in Resource.PluginList.Keys)
                    {
                        if (str.ToUpper().Equals(pModel.Name.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m49"));
                            return;

                        }
                    }


                    pModel.DllModel = Resource.DllList[pluginName];
                    pModel.TableName = splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim();

                    pModel.MaxCount = Convert.ToInt32(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim());
                    pModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                    pModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());
                    pModel.DesIP = splitContainer1.Panel2.Controls["remoteIPBox"].Text.Trim();

                    //remote database ip can not be the same with tranDB 
                    if (pModel.DesIP.Equals(Resource.TranIp))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m58"));
                        return;
                    }
                    pModel.DesCatalog = splitContainer1.Panel2.Controls["remoteCatalogBox"].Text.Trim();
                    pModel.DesUser = splitContainer1.Panel2.Controls["remoteUserBox"].Text.Trim();
                    pModel.DesPwd = splitContainer1.Panel2.Controls["remotePwdBox"].Text.Trim();
                    pModel.DesPort = Convert.ToInt32(splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim());
                    pModel.DesInstance = splitContainer1.Panel2.Controls["remoteInstanceBox"].Text.Trim();
                    List<int> entityList = new List<int>();
                    if (!string.IsNullOrEmpty(splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim()))
                    {
                        string[] sList = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim().Split(new char[] { ',' });

                        foreach (string s in sList)
                        {
                            try
                            {
                                entityList.Add(Convert.ToInt32(s));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("error occur when parse entityFilterBox string,error message:" + ex.ToString());
                                MessageBox.Show(Resource.lHelper.Key("m57"));
                                return;

                            }
                        }

                    }
                    pModel.EntityID = entityList;
                    pModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;


                    if (!pModel.Enabled)
                    {
                        try
                        {
                            new XMLIni().AddPluginInstance(pModel);
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Tag = pModel.Name;
                            no.Text = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Add(no);   //han modify
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Add(no);
                            MessageBox.Show(Resource.lHelper.Key("m65"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m66"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            new XMLIni().AddPluginInstance(pModel);

                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckDBInstanceList)
                            {
                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));
                            }

                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Tag = pModel.Name;
                            no.Text = pModel.Name;
                            //treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Add(no); //han modify
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Add(no);
                            MessageBox.Show(Resource.lHelper.Key("m65"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m66"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }


                    }





                }
                else if (Resource.DllList[pluginName].PType == PluginType.TranDB)
                {
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m51"));
                        return;

                    }
                    //判断IP格式
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m52"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim().Length > 50)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n53") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim(), @"[A-Za-z]+(_*)(\d*)"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m78"));
                        return;
                    }

                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m53"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n54") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim(), "^[1-9]\\d*$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m54"));
                        return;

                    }

                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m43"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m44"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m45"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m46"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m47"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m48"));
                        return;

                    }

                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["citycode"].Text.Trim()) || !Regex.IsMatch(splitContainer1.Panel2.Controls["citycode"].Text.Trim(), @"^-?[0-9]\d*$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m79"));
                        return;
                    }

                    if (splitContainer1.Panel2.Controls["citycode"].Text.Trim().Length > 10)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m80"));
                        return;
                    }

                    PluginModel pModel = new PluginModel();

                    pModel.Citycode = Convert.ToInt32(splitContainer1.Panel2.Controls["citycode"].Text.Trim());

                    pModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();


                    foreach (string str in Resource.PluginList.Keys)
                    {
                        if (str.ToUpper().Equals(pModel.Name.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m49"));
                            return;

                        }
                    }

                    pModel.DllModel = Resource.DllList[pluginName];
                    pModel.TableName = splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim();

                    foreach (DBManager dbManager in Resource.DBList.Values)
                    {
                        if (dbManager.GetModel().TableName.ToUpper().Equals(pModel.TableName.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m55"));
                            return;
                        }

                    }
                    foreach (PluginManager pManager in Resource.PluginList.Values)
                    {
                        if (pManager.GetPluginType() == PluginType.TranDB)
                        {
                            if (pManager.GetPluginModel().TableName.ToUpper().Equals(pModel.TableName.ToUpper()))
                            {
                                MessageBox.Show(Resource.lHelper.Key("m55"));
                                return;

                            }
                        }
                    }
                    ///排除对oracle判断
                    if (Resource.TranInstance.Trim().Equals("mssqlserver", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //table already existed
                        var helper = new SqlHelper(Resource.TranConn);
                        var sqlStr = "select count(0) from sysobjects where xtype='U' and name='" + pModel.TableName + "'";
                        try
                        {
                            //DataTable dt = helper.ExecuteRead(CommandType.Text, sqlStr, "test", null);
                            //if (dt == null)
                            //{
                            //    MessageBox.Show(Resource.lHelper.Key("m73"));
                            //    return;
                            //}
                            //if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                            //{
                            //    MessageBox.Show(Resource.lHelper.Key("n53") + " " + pModel.TableName + " " +
                            //                    Resource.lHelper.Key("m72"));
                            //    return;
                            //}
                            if (helper.ExecuteScalar(CommandType.Text, sqlStr) > 0)
                            {
                                MessageBox.Show(Resource.lHelper.Key("n53") + " " + pModel.TableName + " " +
                                                Resource.lHelper.Key("m72"));
                                return;
                            }
                            //else
                            //{
                            //    MessageBox.Show(Resource.lHelper.Key("m73"));
                            //    return;
                            //}
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                            MessageBox.Show(Resource.lHelper.Key("m73"));
                            return;

                        }
                    }

                    pModel.MaxCount = Convert.ToInt32(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim());
                    pModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                    pModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());
                    pModel.DesIP = Resource.TranIp;
                    pModel.DesCatalog = Resource.TranDb;
                    pModel.DesUser = Resource.TranUser;
                    pModel.DesPwd = Resource.TranPwd;
                    pModel.DesPort = Resource.TranPort;
                    pModel.DesInstance = Resource.TranInstance;

                    List<int> entityList = new List<int>();
                    if (!string.IsNullOrEmpty(splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim()))
                    {
                        string[] sList = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim().Split(new char[] { ',' });

                        foreach (string s in sList)
                        {
                            try
                            {
                                entityList.Add(Convert.ToInt32(s));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("error occur when parse entityFilterBox string,error message:" + ex.ToString());
                                MessageBox.Show(Resource.lHelper.Key("m57"));
                                return;

                            }
                        }

                    }
                    pModel.EntityID = entityList;
                    pModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;


                    if (!pModel.Enabled)
                    {
                        try
                        {
                            new XMLIni().AddPluginInstance(pModel);

                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));

                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Tag = pModel.Name;
                            no.Text = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Add(no);   //han modify
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Add(no);
                            MessageBox.Show(Resource.lHelper.Key("m65"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m66"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            new XMLIni().AddPluginInstance(pModel);

                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckDBInstanceList)
                            {
                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));
                            }

                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Tag = pModel.Name;
                            no.Text = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Add(no);  //han modify trandb
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Add(no);
                            MessageBox.Show(Resource.lHelper.Key("m65"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m66"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }


                    }


                }
                else
                {
                    MessageBox.Show(Resource.lHelper.Key("m59"));
                    Logger.Error("error occur when add plugin instance,error message:unknown plugin type,pluginName:" + pluginName);
                    return;
                }

            }
            else if (splitContainer1.Panel2.Controls["panelType"].Text.Equals("pluginInstance"))
            {
                string pluginName = splitContainer1.Panel2.Controls["pluginNameBox"].Text.Trim();
                string oldInstanceName = splitContainer1.Panel2.Controls["oldInstanceName"].Text.Trim();
                if (!Resource.DllList.ContainsKey(pluginName))
                {
                    MessageBox.Show(Resource.lHelper.Key("m56"));
                    Logger.Error("error occur when add plugin instance,error message: unknown plugin name,plugin name:" + pluginName);
                    return;
                }
                //deal by the value of pluginType
                if (Resource.DllList[pluginName].PType == PluginType.RemoteIP)
                {


                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m51"));
                        return;
                    }
                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["ipBox"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m34"));
                        return;

                    }

                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["portBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("28"));
                        return;
                    }

                    if (splitContainer1.Panel2.Controls["portBox"].Text.Trim().Length > 6)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n12") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["portBox"].Text.Trim(), "^\\d+$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m29"));
                        return;

                    }
                    if (Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text) > 65534)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m30"));
                        return;
                    }

                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m43"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }



                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m44"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m45"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m46"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m47"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m48"));
                        return;

                    }


                    PluginModel pModel = new PluginModel();


                    pModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();
                    foreach (string str in Resource.PluginList.Keys)
                    {
                        if (!str.ToUpper().Equals(oldInstanceName.ToUpper()))
                        {
                            if (str.ToUpper().Equals(pModel.Name.ToUpper()))
                            {
                                MessageBox.Show(Resource.lHelper.Key("m49"));
                                return;
                            }
                        }

                    }

                    pModel.DllModel = Resource.DllList[pluginName];
                    pModel.Ip = splitContainer1.Panel2.Controls["ipBox"].Text.Trim();
                    pModel.Port = Convert.ToInt32(splitContainer1.Panel2.Controls["portBox"].Text.Trim());

                    foreach (IPManager ipManager in Resource.IPList.Values)
                    {
                        if (pModel.Ip.Equals(ipManager.GetModel().Ip) && pModel.Port == ipManager.GetModel().Port)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m50"));
                            return;
                        }

                    }
                    foreach (PluginManager pManager in Resource.PluginList.Values)
                    {
                        if (pManager.GetPluginType() == PluginType.RemoteIP)
                        {
                            if (!pManager.GetPluginModel().Name.ToUpper().Equals(oldInstanceName.ToUpper()))
                            {
                                if (pManager.GetPluginModel().Ip.Equals(pModel.Ip) && pManager.GetPluginModel().Port == pModel.Port)
                                {
                                    MessageBox.Show(Resource.lHelper.Key("m50"));
                                    return;

                                }
                            }
                        }

                    }


                    //pModel.Protocol = (string)((ComboBox)splitContainer1.Panel2.Controls["protocolBox"]).SelectedItem;
                    //pModel.NetProtocol = (string)((ComboBox)splitContainer1.Panel2.Controls["netProtocolBox"]).SelectedItem;
                    pModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                    pModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());
                    List<int> entityList = new List<int>();
                    if (!string.IsNullOrEmpty(splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim()))
                    {
                        string[] sList = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim().Split(new char[] { ',' });

                        foreach (string s in sList)
                        {
                            try
                            {
                                entityList.Add(Convert.ToInt32(s));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("error occur when parse entityFilterBox string,error message:" + ex.ToString());
                                MessageBox.Show(Resource.lHelper.Key("m57"));
                                return;

                            }
                        }

                    }
                    pModel.EntityID = entityList;
                    pModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;

                    if (!pModel.Enabled)
                    {
                        try
                        {
                            new XMLIni().ModifyPluginInstance(oldInstanceName, pModel);
                            Resource.PluginList.Remove(oldInstanceName);
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckPluginInstanceList)
                            {
                                if (Resource.PluginInstanceList.ContainsKey(oldInstanceName))
                                {
                                    //stop plugin instance thread
                                    Resource.PluginInstanceList[oldInstanceName].SetEnabled(false);
                                    Resource.PluginInstanceList.Remove(oldInstanceName);
                                }

                            }
                            //int index = treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.IndexOf(treeView1.SelectedNode);
                            int index = treeView1.Nodes["root"].Nodes["ipTran"].Nodes.IndexOf(treeView1.SelectedNode);

                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Text = pModel.Name;
                            no.Tag = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.RemoveAt(index);    //han modify
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Insert(index, no); //han modify
                            treeView1.Nodes["root"].Nodes["ipTran"].Nodes.RemoveAt(index);
                            treeView1.Nodes["root"].Nodes["ipTran"].Nodes.Insert(index, no);
                            treeView1.SelectedNode = no;
                            splitContainer1.Panel2.Controls["oldInstanceName"].Text = pModel.Name;
                            MessageBox.Show(Resource.lHelper.Key("m31"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m32"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            new XMLIni().ModifyPluginInstance(oldInstanceName, pModel);

                            Resource.PluginList.Remove(oldInstanceName);
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckPluginInstanceList)
                            {
                                if (Resource.PluginInstanceList.ContainsKey(oldInstanceName))
                                {
                                    //stop plugin instance thread
                                    Resource.PluginInstanceList[oldInstanceName].SetEnabled(false);
                                    Resource.PluginInstanceList.Remove(oldInstanceName);
                                }
                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));
                            }
                            //  int index = treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.IndexOf(treeView1.SelectedNode);
                            int index = treeView1.Nodes["root"].Nodes["ipTran"].Nodes.IndexOf(treeView1.SelectedNode);
                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Text = pModel.Name;
                            no.Tag = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.RemoveAt(index);  
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Insert(index, no); 
                            treeView1.Nodes["root"].Nodes["ipTran"].Nodes.RemoveAt(index);  //han modify
                            treeView1.Nodes["root"].Nodes["ipTran"].Nodes.Insert(index, no); //han modify
                            treeView1.SelectedNode = no;
                            splitContainer1.Panel2.Controls["oldInstanceName"].Text = pModel.Name;
                            MessageBox.Show(Resource.lHelper.Key("m31"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m32"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }


                    }


                }
                else if (Resource.DllList[pluginName].PType == PluginType.RemoteDB)
                {
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m51"));
                        return;

                    }
                    //判断IP格式
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m52"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim().Length > 50)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n53") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }


                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m53"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n54") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }


                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim(), "^[1-9]\\d*$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m54"));
                        return;
                    }



                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m43"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m44"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m45"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m46"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m47"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m48"));
                        return;

                    }
                    //判断IP格式
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteIPBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m33"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["remoteIPBox"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m34"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteCatalogBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m35"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteUserBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m37"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remotePwdBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m38"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m28"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim().Length > 6)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n42") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("n71"));
                        return;

                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim(), "^\\d+$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m29"));
                        return;

                    }
                    if (Convert.ToInt32(splitContainer1.Panel2.Controls["remotePortBox"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["remotePortBox"].Text) > 65534)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m30"));
                        return;
                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteInstanceBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m51"));
                        return;

                    }
                    PluginModel pModel = new PluginModel();

                    pModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();
                    foreach (string str in Resource.PluginList.Keys)
                    {
                        if (!str.ToUpper().Equals(oldInstanceName.ToUpper()))
                        {
                            if (str.ToUpper().Equals(pModel.Name.ToUpper()))
                            {
                                MessageBox.Show(Resource.lHelper.Key("m49"));
                                return;
                            }
                        }

                    }

                    pModel.DllModel = Resource.DllList[pluginName];
                    pModel.TableName = splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim();

                    pModel.MaxCount = Convert.ToInt32(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim());
                    pModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                    pModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());
                    pModel.DesIP = splitContainer1.Panel2.Controls["remoteIPBox"].Text.Trim();

                    //remote database ip can not be the same with tranDB 
                    if (pModel.DesIP.Equals(Resource.TranIp))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m58"));
                        return;
                    }

                    pModel.DesCatalog = splitContainer1.Panel2.Controls["remoteCatalogBox"].Text.Trim();
                    pModel.DesUser = splitContainer1.Panel2.Controls["remoteUserBox"].Text.Trim();
                    pModel.DesPwd = splitContainer1.Panel2.Controls["remotePwdBox"].Text.Trim();
                    pModel.DesPort = Convert.ToInt32(splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim());
                    pModel.DesInstance = splitContainer1.Panel2.Controls["remoteInstanceBox"].Text.Trim();
                    List<int> entityList = new List<int>();
                    if (!string.IsNullOrEmpty(splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim()))
                    {
                        string[] sList = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim().Split(new char[] { ',' });

                        foreach (string s in sList)
                        {
                            try
                            {
                                entityList.Add(Convert.ToInt32(s));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("error occur when parse entityFilterBox string,error message:" + ex.ToString());
                                MessageBox.Show(Resource.lHelper.Key("m57"));
                                return;

                            }
                        }

                    }
                    pModel.EntityID = entityList;
                    pModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;

                    if (!pModel.Enabled)
                    {
                        try
                        {
                            new XMLIni().ModifyPluginInstance(oldInstanceName, pModel);
                            Resource.PluginList.Remove(oldInstanceName);
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckPluginInstanceList)
                            {
                                if (Resource.PluginInstanceList.ContainsKey(oldInstanceName))
                                {
                                    //stop plugin instance thread
                                    Resource.PluginInstanceList[oldInstanceName].SetEnabled(false);
                                    Resource.PluginInstanceList.Remove(oldInstanceName);
                                }

                            }

                            //  int index = treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.IndexOf(treeView1.SelectedNode);
                            int index = treeView1.Nodes["root"].Nodes["dbTran"].Nodes.IndexOf(treeView1.SelectedNode);
                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Text = pModel.Name;
                            no.Tag = pModel.Name;
                            //  treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.RemoveAt(index);
                            //  treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Insert(index, no);
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.RemoveAt(index);  //han modify
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Insert(index, no); //han modify
                            treeView1.SelectedNode = no;
                            splitContainer1.Panel2.Controls["oldInstanceName"].Text = pModel.Name;
                            MessageBox.Show(Resource.lHelper.Key("m31"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m32"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            new XMLIni().ModifyPluginInstance(oldInstanceName, pModel);

                            Resource.PluginList.Remove(oldInstanceName);
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckPluginInstanceList)
                            {
                                if (Resource.PluginInstanceList.ContainsKey(oldInstanceName))
                                {
                                    //stop plugin instance thread
                                    Resource.PluginInstanceList[oldInstanceName].SetEnabled(false);
                                    Resource.PluginInstanceList.Remove(oldInstanceName);
                                }
                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));
                            }
                            //int index = treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.IndexOf(treeView1.SelectedNode);
                            int index = treeView1.Nodes["root"].Nodes["dbTran"].Nodes.IndexOf(treeView1.SelectedNode); //han modify
                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Text = pModel.Name;
                            no.Tag = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.RemoveAt(index);
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Insert(index, no);
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.RemoveAt(index);  //han modify
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Insert(index, no); // han modify
                            treeView1.SelectedNode = no;
                            splitContainer1.Panel2.Controls["oldInstanceName"].Text = pModel.Name;
                            MessageBox.Show(Resource.lHelper.Key("m31"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m32"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }


                    }



                }
                else if (Resource.DllList[pluginName].PType == PluginType.TranDB)
                {

                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m51"));
                        return;

                    }
                    //判断IP格式
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m52"));
                        return;

                    }
                    if (splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim().Length > 50)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n53") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }


                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m53"));
                        return;
                    }
                    if (splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n54") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }


                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim(), "^[1-9]\\d*$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m54"));
                        return;

                    }


                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m43"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n51") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }


                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m44"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) < (-180) || Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim()) > 180)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m45"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m46"));
                        return;

                    }

                    if (splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim().Length > 9)
                    {
                        MessageBox.Show(Resource.lHelper.Key("n52") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                        return;
                    }

                    if (!Regex.IsMatch(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim(), @"^-?([1-9]\d*\.?\d*|0\.\d*[1-9]\d*|0?\.0+|0)$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m47"));
                        return;

                    }
                    if (Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) < (-90) || Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim()) > 90)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m48"));
                        return;

                    }
                    if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["citycode"].Text.Trim()) || !Regex.IsMatch(splitContainer1.Panel2.Controls["citycode"].Text.Trim(), @"^-?[0-9]\d*$"))
                    {
                        MessageBox.Show(Resource.lHelper.Key("m79"));
                        return;
                    }
                    if (splitContainer1.Panel2.Controls["citycode"].Text.Trim().Length>10)
                    {
                        MessageBox.Show(Resource.lHelper.Key("m80"));
                        return;
                    }

                    PluginModel pModel = new PluginModel();

                    pModel.Citycode = Convert.ToInt32(splitContainer1.Panel2.Controls["citycode"].Text.Trim());
                    pModel.Name = splitContainer1.Panel2.Controls["instanceNameBox"].Text.Trim();
                    foreach (string str in Resource.PluginList.Keys)
                    {
                        if (!str.ToUpper().Equals(oldInstanceName.ToUpper()))
                        {
                            if (str.ToUpper().Equals(pModel.Name.ToUpper()))
                            {
                                MessageBox.Show(Resource.lHelper.Key("m49"));
                                return;
                            }
                        }

                    }


                    pModel.DllModel = Resource.DllList[pluginName];
                    pModel.TableName = splitContainer1.Panel2.Controls["tableNameBox"].Text.Trim();

                    foreach (DBManager dbManager in Resource.DBList.Values)
                    {
                        if (dbManager.GetModel().TableName.ToUpper().Equals(pModel.TableName.ToUpper()))
                        {
                            MessageBox.Show(Resource.lHelper.Key("m55"));
                            return;
                        }

                    }
                    foreach (PluginManager pManager in Resource.PluginList.Values)
                    {
                        if (pManager.GetPluginType() == PluginType.TranDB)
                        {
                            if (!pManager.GetPluginModel().Name.ToUpper().Equals(oldInstanceName.ToUpper()))
                            {
                                if (pManager.GetPluginModel().TableName.ToUpper().Equals(pModel.TableName.ToUpper()))
                                {
                                    MessageBox.Show(Resource.lHelper.Key("m55"));
                                    return;
                                }
                            }

                        }
                    }
                    var helper = new SqlHelper(Resource.TranConn);
                    string sqlStr = "select count(0) from sysobjects where xtype='U' and name='" + pModel.TableName + "'";
                    //check table when change table name
                    if (!Resource.PluginList[oldInstanceName].GetPluginModel().TableName.ToUpper().Equals(pModel.TableName.ToUpper()))
                    {
                        try
                        {
                            DataTable dt = helper.ExecuteRead(CommandType.Text, sqlStr, "test", null);
                            if (dt == null)
                            {
                                MessageBox.Show(Resource.lHelper.Key("m73"));
                                return;
                            }
                            if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                            {
                                MessageBox.Show(Resource.lHelper.Key("n53") + " " + pModel.TableName + " " + Resource.lHelper.Key("m72"));
                                return;
                            }

                        }
                        catch (Exception ex)
                        {
                            Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                            MessageBox.Show(Resource.lHelper.Key("m73"));
                            return;
                        }

                    }


                    pModel.MaxCount = Convert.ToInt32(splitContainer1.Panel2.Controls["maxCountBox"].Text.Trim());
                    pModel.LonOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["lonOffsetBox"].Text.Trim());
                    pModel.LatOffset = Convert.ToDouble(splitContainer1.Panel2.Controls["latOffsetBox"].Text.Trim());

                    pModel.DesIP = Resource.TranIp;
                    pModel.DesCatalog = Resource.TranDb;
                    pModel.DesUser = Resource.TranUser;
                    pModel.DesPwd = Resource.TranPwd;
                    pModel.DesPort = Resource.TranPort;
                    pModel.DesInstance = Resource.TranInstance;


                    List<int> entityList = new List<int>();
                    if (!string.IsNullOrEmpty(splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim()))
                    {
                        string[] sList = splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim().Split(new char[] { ',' });

                        foreach (string s in sList)
                        {
                            try
                            {
                                entityList.Add(Convert.ToInt32(s));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("error occur when parse entityFilterBox string,error message:" + ex.ToString());
                                MessageBox.Show(Resource.lHelper.Key("m57"));
                                return;

                            }
                        }

                    }
                    pModel.EntityID = entityList;
                    pModel.Enabled = ((CheckBox)splitContainer1.Panel2.Controls["instanceEnabled"]).Checked;

                    if (!pModel.Enabled)
                    {
                        try
                        {

                            sqlStr = "";
                            if (!Resource.PluginList[oldInstanceName].GetPluginModel().TableName.ToUpper().Equals(pModel.TableName.ToUpper()))
                            {
                                sqlStr = "if exists(select 0 from sysobjects where xtype='U' and name ='" + Resource.PluginList[oldInstanceName].GetPluginModel().TableName + "') begin exec sp_rename '" + Resource.PluginList[oldInstanceName].GetPluginModel().TableName + "','" + pModel.TableName + "' end";
                            }
                            try
                            {
                                if (!string.IsNullOrEmpty(sqlStr))
                                {
                                    helper.ExecuteNonQueryNonTran(CommandType.Text, sqlStr);
                                }

                            }
                            catch (Exception ex)
                            {
                                Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                                MessageBox.Show(Resource.lHelper.Key("m73"));
                                return;
                            }

                            new XMLIni().ModifyPluginInstance(oldInstanceName, pModel);
                            Resource.PluginList.Remove(oldInstanceName);
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckPluginInstanceList)
                            {
                                if (Resource.PluginInstanceList.ContainsKey(oldInstanceName))
                                {
                                    Resource.PluginInstanceList[oldInstanceName].SetEnabled(false);
                                    Resource.PluginInstanceList.Remove(oldInstanceName);
                                }

                            }




                            //int index = treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.IndexOf(treeView1.SelectedNode);
                            int index = treeView1.Nodes["root"].Nodes["dbTran"].Nodes.IndexOf(treeView1.SelectedNode); //han modify
                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Text = pModel.Name;
                            no.Tag = pModel.Name;
                            // treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.RemoveAt(index);
                            //   treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Insert(index, no);
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.RemoveAt(index);  //han modify
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Insert(index, no); //han modify
                            treeView1.SelectedNode = no;
                            splitContainer1.Panel2.Controls["oldInstanceName"].Text = pModel.Name;
                            MessageBox.Show(Resource.lHelper.Key("m31"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m32"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }
                    }
                    else
                    {
                        try
                        {

                            sqlStr = "";
                            if (!Resource.PluginList[oldInstanceName].GetPluginModel().TableName.ToUpper().Equals(pModel.TableName.ToUpper()))
                            {
                                sqlStr = "if exists(select 0 from sysobjects where xtype='U' and name ='" + Resource.PluginList[oldInstanceName].GetPluginModel().TableName + "') begin exec sp_rename '" + Resource.PluginList[oldInstanceName].GetPluginModel().TableName + "','" + pModel.TableName + "' end";
                            }
                            try
                            {
                                if (!string.IsNullOrEmpty(sqlStr))
                                {
                                    helper.ExecuteNonQueryNonTran(CommandType.Text, sqlStr);
                                }

                            }
                            catch (Exception ex)
                            {
                                Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                                MessageBox.Show(Resource.lHelper.Key("m73"));
                                return;
                            }
                            new XMLIni().ModifyPluginInstance(oldInstanceName, pModel);

                            Resource.PluginList.Remove(oldInstanceName);
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                            lock (Resource.lckPluginInstanceList)
                            {
                                if (Resource.PluginInstanceList.ContainsKey(oldInstanceName))
                                {
                                    Resource.PluginInstanceList[oldInstanceName].SetEnabled(false);
                                    Resource.PluginInstanceList.Remove(oldInstanceName);
                                }
                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));
                            }

                            // int index = treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.IndexOf(treeView1.SelectedNode);
                            int index = treeView1.Nodes["root"].Nodes["dbTran"].Nodes.IndexOf(treeView1.SelectedNode);//han modify

                            TreeNode no = new TreeNode();
                            no.Name = pModel.Name + "plugin";
                            no.Text = pModel.Name;
                            no.Tag = pModel.Name;
                            //treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.RemoveAt(index);
                            //treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.Insert(index, no);

                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.RemoveAt(index);  // han modify
                            treeView1.Nodes["root"].Nodes["dbTran"].Nodes.Insert(index, no);  // han modify
                            treeView1.SelectedNode = no;
                            splitContainer1.Panel2.Controls["oldInstanceName"].Text = pModel.Name;
                            MessageBox.Show(Resource.lHelper.Key("m31"));

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Resource.lHelper.Key("m32"));
                            Logger.Error("error occur when add Plugin instance,error message:" + ex.ToString());
                            return;
                        }


                    }

                }
                else
                {
                    MessageBox.Show(Resource.lHelper.Key("m59"));
                    Logger.Error("error occur when add plugin instance,error message:unknown plugin type,pluginName:" + pluginName);
                    return;
                }


            }
            else
            {
                MessageBox.Show(Resource.lHelper.Key("m60"));
                Logger.Error("error occur when modify config,error message: unknown instance,instance name" + splitContainer1.Panel2.Controls["panelType"].Text);
                return;

            }


        }

        //delete instance
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            SqlHelper helper = new SqlHelper(Resource.TranConn);
            string sqlStr = "";

            if (splitContainer1.Panel2.Controls["panelType"].Text.Equals("ipTranInstance"))
            {
                string oldInstanceName = splitContainer1.Panel2.Controls["oldInstanceName"].Text;

                try
                {
                    new XMLIni().RemoveIPInstance(oldInstanceName);
                    Resource.IPList.Remove(oldInstanceName);
                    lock (Resource.lckIPInstanceList)
                    {
                        if (Resource.IPInstanceList.ContainsKey(oldInstanceName))
                        {
                            Resource.IPInstanceList[oldInstanceName].SetEnabled(false);
                            Resource.IPInstanceList.Remove(oldInstanceName);

                        }

                    }

                    int index = treeView1.Nodes["root"].Nodes["ipTran"].Nodes.IndexOf(treeView1.SelectedNode);
                    treeView1.Nodes["root"].Nodes["ipTran"].Nodes.RemoveAt(index);
                    treeView1.SelectedNode = treeView1.Nodes["root"].Nodes["ipTran"];
                    MessageBox.Show(Resource.lHelper.Key("m63"));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resource.lHelper.Key("m64"));
                    Logger.Error("error occur when delete IP instance,error message:" + ex.ToString());
                    return;

                }


            }
            else if (splitContainer1.Panel2.Controls["panelType"].Text.Equals("dbTranInstance"))
            {
                string oldInstanceName = splitContainer1.Panel2.Controls["oldInstanceName"].Text;

                try
                {
                    sqlStr = "if exists(select 0 from sysobjects where xtype='U' and name ='" + Resource.DBList[oldInstanceName].GetModel().TableName + "') begin drop table " + Resource.DBList[oldInstanceName].GetModel().TableName + " end";
                    try
                    {
                        helper.ExecuteNonQueryNonTran(CommandType.Text, sqlStr);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                        MessageBox.Show(Resource.lHelper.Key("m73"));
                        return;
                    }
                    new XMLIni().RemoveDBInstance(oldInstanceName);
                    Resource.DBList.Remove(oldInstanceName);
                    lock (Resource.lckDBInstanceList)
                    {
                        if (Resource.DBInstanceList.ContainsKey(oldInstanceName))
                        {
                            Resource.DBInstanceList[oldInstanceName].SetEnabled(false);
                            Resource.DBInstanceList.Remove(oldInstanceName);

                        }

                    }

                    int index = treeView1.Nodes["root"].Nodes["dbTran"].Nodes.IndexOf(treeView1.SelectedNode);
                    treeView1.Nodes["root"].Nodes["dbTran"].Nodes.RemoveAt(index);
                    treeView1.SelectedNode = treeView1.Nodes["root"].Nodes["dbTran"];
                    MessageBox.Show(Resource.lHelper.Key("m63"));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resource.lHelper.Key("m64"));
                    Logger.Error("error occur when delete DB instance,error message:" + ex.ToString());
                    return;

                }



            }
            else if (splitContainer1.Panel2.Controls["panelType"].Text.Equals("pluginInstance"))
            {
                string oldInstanceName = splitContainer1.Panel2.Controls["oldInstanceName"].Text;
                sqlStr = "";
                int deltag = 0;
                try
                {
                    if (Resource.PluginList[oldInstanceName].GetPluginType() == PluginType.RemoteIP)
                    {
                        deltag = 1;
                    }
                    //drop table when pluginType=TranDB
                    if (Resource.PluginList[oldInstanceName].GetPluginType() == PluginType.TranDB)
                    {
                        sqlStr = "if exists(select 0 from sysobjects where xtype='U' and name ='" + Resource.PluginList[oldInstanceName].GetPluginModel().TableName + "' ) begin drop table " + Resource.PluginList[oldInstanceName].GetPluginModel().TableName + " end";
                        try
                        {
                            if (!string.IsNullOrEmpty(sqlStr))
                            {
                                helper.ExecuteNonQueryNonTran(CommandType.Text, sqlStr);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(sqlStr + " failed to execute,error message:" + ex.ToString());
                            MessageBox.Show(Resource.lHelper.Key("m73"));
                            return;
                        }

                    }

                    new XMLIni().RemovePluginInstance(oldInstanceName);
                    Resource.PluginList.Remove(oldInstanceName);
                    lock (Resource.lckPluginInstanceList)
                    {
                        if (Resource.PluginInstanceList.ContainsKey(oldInstanceName))
                        {
                            Resource.PluginInstanceList[oldInstanceName].SetEnabled(false);
                            Resource.PluginInstanceList.Remove(oldInstanceName);
                        }

                    }
                    // int index = treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.IndexOf(treeView1.SelectedNode);
                    //  treeView1.Nodes["root"].Nodes["pluginTran"].Nodes.RemoveAt(index);
                    //  treeView1.SelectedNode = treeView1.Nodes["root"].Nodes["pluginTran"];
                    if (deltag == 1)
                    {
                        int index = treeView1.Nodes["root"].Nodes["ipTran"].Nodes.IndexOf(treeView1.SelectedNode);
                        treeView1.Nodes["root"].Nodes["ipTran"].Nodes.RemoveAt(index);
                        treeView1.SelectedNode = treeView1.Nodes["root"].Nodes["ipTran"];  //han modify
                    }
                    else
                    {
                        int index = treeView1.Nodes["root"].Nodes["DBTran"].Nodes.IndexOf(treeView1.SelectedNode);
                        treeView1.Nodes["root"].Nodes["DBTran"].Nodes.RemoveAt(index);
                        treeView1.SelectedNode = treeView1.Nodes["root"].Nodes["DBTran"];  //han modify
                    }

                    MessageBox.Show(Resource.lHelper.Key("m63"));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resource.lHelper.Key("m64"));
                    Logger.Error("error occur when delete Plugin instance,error message:" + ex.ToString());
                    return;

                }

            }
            else
            {
                MessageBox.Show(Resource.lHelper.Key("m60"));
                Logger.Error("error occur when delete config,error message: unknown instance,instance name" + splitContainer1.Panel2.Controls["panelType"].Text);
                return;

            }

        }


        //test webgis database connection
        private void TestWebGisConnection(object sender, EventArgs e)
        {
            //判断IP格式
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisIP"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m33"));
                return;

            }

            if (!Regex.IsMatch(splitContainer1.Panel2.Controls["webGisIP"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
            {
                MessageBox.Show(Resource.lHelper.Key("m34"));
                return;

            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisDB"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m35"));
                return;

            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisUser"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m37"));
                return;

            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisPwd"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m38"));
                return;

            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisPort"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m28"));
                return;

            }

            if (splitContainer1.Panel2.Controls["webGisPort"].Text.Trim().Length > 6)
            {
                MessageBox.Show(Resource.lHelper.Key("n42") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                return;
            }


            if (!Regex.IsMatch(splitContainer1.Panel2.Controls["webGisPort"].Text.Trim(), "^\\d+$"))
            {
                MessageBox.Show(Resource.lHelper.Key("m29"));
                return;

            }
            if (Convert.ToInt32(splitContainer1.Panel2.Controls["webGisPort"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["webGisPort"].Text) > 65534)
            {
                MessageBox.Show(Resource.lHelper.Key("m30"));
                return;
            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["webGisInstance"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m51"));
                return;
            }
            //todo: modify webGis config

            string conn = "Data Source=" + splitContainer1.Panel2.Controls["webGisIP"].Text.Trim() + "," + splitContainer1.Panel2.Controls["webGisPort"].Text.Trim() + "\\" + splitContainer1.Panel2.Controls["webGisInstance"].Text.Trim() + ";Initial Catalog=" + splitContainer1.Panel2.Controls["webGisDB"].Text.Trim() + ";uid=" + splitContainer1.Panel2.Controls["webGisUser"].Text.Trim() + ";pwd=" + splitContainer1.Panel2.Controls["webGisPwd"].Text.Trim();
            SqlHelper helper = new SqlHelper(conn);

            if (helper.TestCon())
            {
                MessageBox.Show(Resource.lHelper.Key("m61"));
            }
            else
            {
                MessageBox.Show(Resource.lHelper.Key("m62"));

            }


        }




        //get selected entity id from child form EntityListForm
        private void SelectEntity(object sender, EventArgs e)
        {

            string inType = ((Button)sender).Parent.Controls["panelType"].Text;

            List<int> entityList = new List<int>();
            if (inType.Equals("ipTranInstance"))
            {
                string instanceName = ((Button)sender).Parent.Controls["oldInstanceName"].Text;
                entityList.Clear();
                foreach (int i in Resource.IPList[instanceName].GetModel().EntityID)
                {
                    entityList.Add(i);
                }

            }
            else if (inType.Equals("dbTranInstance"))
            {
                string instanceName = ((Button)sender).Parent.Controls["oldInstanceName"].Text;
                entityList.Clear();
                foreach (int i in Resource.DBList[instanceName].GetModel().EntityID)
                {
                    entityList.Add(i);
                }

            }
            else if (inType.Equals("pluginInstance"))
            {
                string instanceName = ((Button)sender).Parent.Controls["oldInstanceName"].Text;
                entityList.Clear();
                foreach (int i in Resource.PluginList[instanceName].GetPluginModel().EntityID)
                {
                    entityList.Add(i);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["entityFilterBox"].Text.Trim()))
                {
                    entityList = new List<int>();
                }
                else
                {
                    string[] temp = ((TextBox)splitContainer1.Panel2.Controls["entityFilterBox"]).Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    entityList = new List<int>();
                    foreach (string str in temp)
                    {
                        try
                        {
                            entityList.Add(Convert.ToInt32(str));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("error occur when split entityFilterBox in add instance operation,error message:" + ex.ToString());
                            MessageBox.Show(Resource.lHelper.Key("m57"));
                            return;
                        }
                    }


                }
            }




            EntityListForm entity = new EntityListForm(ref entityList);
            entity.ShowDialog();
            ((TextBox)splitContainer1.Panel2.Controls["entityFilterBox"]).Enabled = true;
            ((TextBox)splitContainer1.Panel2.Controls["entityFilterBox"]).Text = "";
            string s = "";
            foreach (int i in entityList)
            {
                s = s + i.ToString() + ",";

            }
            if (s.IndexOf(",") >= 0)
            {
                s = s.Remove(s.LastIndexOf(","));
            }

            ((TextBox)splitContainer1.Panel2.Controls["entityFilterBox"]).Text = s;
            ((TextBox)splitContainer1.Panel2.Controls["entityFilterBox"]).Enabled = false;
        }

        //test tran database connection
        private void TestTranConnection(object sender, EventArgs e)
        {
            //判断IP格式
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranIP"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m33"));
                return;
            }
            if (!Regex.IsMatch(splitContainer1.Panel2.Controls["tranIP"].Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
            {
                MessageBox.Show(Resource.lHelper.Key("m34"));
                return;
            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranDB"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m35"));
                return;
            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranUser"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m37"));
                return;
            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranPwd"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m38"));
                return;
            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranPort"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m28"));
                return;
            }

            if (splitContainer1.Panel2.Controls["tranPort"].Text.Trim().Length > 6)
            {
                MessageBox.Show(Resource.lHelper.Key("n42") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                return;
            }

            if (!Regex.IsMatch(splitContainer1.Panel2.Controls["tranPort"].Text.Trim(), "^\\d+$"))
            {
                MessageBox.Show(Resource.lHelper.Key("m29"));
                return;
            }
            if (Convert.ToInt32(splitContainer1.Panel2.Controls["tranPort"].Text) < 1025 || Convert.ToInt32(splitContainer1.Panel2.Controls["tranPort"].Text) > 65534)
            {
                MessageBox.Show(Resource.lHelper.Key("m30"));
                return;
            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["tranInstance"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m51"));
                return;
            }
            string conn = string.Empty;
            //bool result = false;

            //xzj--20190131--oracle--将对oracle测试连接的代码注释删除
            if (!splitContainer1.Panel2.Controls["tranInstance"].Text.Trim()
                .Equals("mssqlserver", StringComparison.CurrentCultureIgnoreCase))
            {
                var helper = new OracleHelper(splitContainer1.Panel2.Controls["tranIP"].Text.Trim(),
                    splitContainer1.Panel2.Controls["tranPort"].Text.Trim(),
                    splitContainer1.Panel2.Controls["tranDB"].Text.Trim(),
                    splitContainer1.Panel2.Controls["tranUser"].Text.Trim(),
                    splitContainer1.Panel2.Controls["tranPwd"].Text.Trim());

                MessageBox.Show(helper.TestCon() ? Resource.lHelper.Key("m61") : Resource.lHelper.Key("m62"));
            }
            else
            {
                conn = "Data Source=" + splitContainer1.Panel2.Controls["tranIP"].Text.Trim() + "," +
                       splitContainer1.Panel2.Controls["tranPort"].Text.Trim() + "\\" +
                       splitContainer1.Panel2.Controls["tranInstance"].Text.Trim() + ";Initial Catalog=" +
                       splitContainer1.Panel2.Controls["tranDB"].Text.Trim() + ";uid=" +
                       splitContainer1.Panel2.Controls["tranUser"].Text.Trim() + ";pwd=" +
                       splitContainer1.Panel2.Controls["tranPwd"].Text.Trim();
                var helper = new SqlHelper(conn);

                MessageBox.Show(helper.TestCon() ? Resource.lHelper.Key("m61") : Resource.lHelper.Key("m62"));
           }




        }

        private void TestRemoteDBConn(object sender, EventArgs e)
        {
            //判断IP格式
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteIPBox"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m33"));
                return;

            }

            if (
                !Regex.IsMatch(splitContainer1.Panel2.Controls["remoteIPBox"].Text.Trim(),
                    @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
            {
                MessageBox.Show(Resource.lHelper.Key("m34"));
                return;

            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteCatalogBox"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m35"));
                return;

            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteUserBox"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m37"));
                return;

            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remotePwdBox"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m38"));
                return;

            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m28"));
                return;

            }

            if (!Regex.IsMatch(splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim(), "^\\d+$"))
            {
                MessageBox.Show(Resource.lHelper.Key("m29"));
                return;

            }
            if (splitContainer1.Panel2.Controls["remotePortBox"].Text.Trim().Length > 6)
            {
                MessageBox.Show(Resource.lHelper.Key("n42") + Resource.lHelper.Key("m70") + Resource.lHelper.Key("m71"));
                return;
            }


            if (Convert.ToInt32(splitContainer1.Panel2.Controls["remotePortBox"].Text) < 1025 ||
                Convert.ToInt32(splitContainer1.Panel2.Controls["remotePortBox"].Text) > 65534)
            {
                MessageBox.Show(Resource.lHelper.Key("m30"));
                return;
            }
            if (string.IsNullOrEmpty(splitContainer1.Panel2.Controls["remoteInstanceBox"].Text.Trim()))
            {
                MessageBox.Show(Resource.lHelper.Key("m51"));
                return;

            }

            string conn = string.Empty;
            //bool result = false;

             //xzj--20190131--oracle--将对oracle测试连接的代码注释删除
            if (!splitContainer1.Panel2.Controls["tranInstance"].Text.Trim()
                .Equals("mssqlserver", StringComparison.CurrentCultureIgnoreCase))
            {
                var helper = new OracleHelper(splitContainer1.Panel2.Controls["tranIP"].Text.Trim(),
                    splitContainer1.Panel2.Controls["tranPort"].Text.Trim(),
                    splitContainer1.Panel2.Controls["tranDB"].Text.Trim(),
                    splitContainer1.Panel2.Controls["tranUser"].Text.Trim(),
                    splitContainer1.Panel2.Controls["tranPwd"].Text.Trim());

                if (helper.TestCon())
                {
                    MessageBox.Show(Resource.lHelper.Key("m61"));
                }
                else
                {
                    MessageBox.Show(Resource.lHelper.Key("m62"));
                }
            }
            else
            {
                conn = "Data Source=" + splitContainer1.Panel2.Controls["tranIP"].Text.Trim() + "," +
                       splitContainer1.Panel2.Controls["tranPort"].Text.Trim() + "\\" +
                       splitContainer1.Panel2.Controls["tranInstance"].Text.Trim() + ";Initial Catalog=" +
                       splitContainer1.Panel2.Controls["tranDB"].Text.Trim() + ";uid=" +
                       splitContainer1.Panel2.Controls["tranUser"].Text.Trim() + ";pwd=" +
                       splitContainer1.Panel2.Controls["tranPwd"].Text.Trim();
                var helper = new SqlHelper(conn);

                if (helper.TestCon())
                {
                    MessageBox.Show(Resource.lHelper.Key("m61"));
                }
                else
                {
                    MessageBox.Show(Resource.lHelper.Key("m62"));
                }
              }
        }
    }
}
