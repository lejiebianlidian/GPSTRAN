using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;

namespace GPSTran
{
    public partial class EntityListForm : Form
    {

        private List<int> entityList;
       
        private void EntityList_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            this.Text = Resource.lHelper.Key("m11");
            this.enterBtn.Text = Resource.lHelper.Key("n37");
            this.cancelBtn.Text = Resource.lHelper.Key("n61");
            Bind();
        }

        public EntityListForm(ref List<int> entityList) 
        {
            InitializeComponent();
            this.entityList = entityList;
        }

        public void Bind() 
        {
            SqlHelper helper = new SqlHelper(Resource.WebGisConn);
            string sql = "select id,name from entity";
            DataTable dt;
            try
            {
                //dt = helper.ExecuteRead(CommandType.Text, sql, "entity", null);
                dt = helper.ExecuteDataReader(CommandType.Text, sql, null);
            }
            catch (Exception ex) 
            {
                Logger.Error("error occur when init entity select,error message:"+ex.ToString());
                MessageBox.Show(Resource.lHelper.Key("m12"));
                return;
            }
            if (dt == null) 
            {
                return;
            }

            this.entityListChkBox.DataSource = dt;
            this.entityListChkBox.ValueMember = "id";
            this.entityListChkBox.DisplayMember = "name";

            foreach (int i in this.entityList) 
            {
                for (int j = 0; j < this.entityListChkBox.Items.Count; j++)
                {
                    if (i == Convert.ToInt32(((DataRowView)entityListChkBox.Items[j]).Row["id"].ToString())) 
                    {
                        entityListChkBox.SetItemChecked(j, true);
                    }
                }
            }

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void enterBtn_Click(object sender, EventArgs e)
        {
            entityList.Clear();
            for (int i = 0; i < this.entityListChkBox.CheckedItems.Count; i++) 
            {
                entityList.Add(Convert.ToInt32(((DataRowView)entityListChkBox.CheckedItems[i]).Row["id"].ToString()));
            
            }
            this.Close();
        }





    }
}
