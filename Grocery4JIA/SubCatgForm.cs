﻿using BLL.Com;
using Model.Const;
using Model.Entity;
using Model.Util;
using Model.VO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UI
{
    public partial class SubCatgForm : Form
    {
        SubCatgManager manager;
        CatgManager catgManager;
        bool chngFlag = false;

        public SubCatgForm()
        {
            InitializeComponent();
            dtgvSubCatg.AutoGenerateColumns = false;
            if (null == manager)
                manager = new SubCatgManager();
            if(null == catgManager)
                catgManager = new CatgManager();
        }

        private void SubCatgForm_Load(object sender, EventArgs e)
        {
            BindData();
            List<Category> mdfyList = catgManager.LoadData(true);
            List<Category> addList = catgManager.LoadData(false);
            cboxMdfySuperName.DataSource = mdfyList;
            cboxMdfySuperName.DisplayMember = "CName";
            cboxMdfySuperName.ValueMember = "CId__PK";
            cboxAddSuperName.DataSource = addList;
            cboxAddSuperName.DisplayMember = "CName";
            cboxAddSuperName.ValueMember = "CId__PK";
        }

        private void BindData()
        {
            try
            {
                List<SubCatgVO> data = manager.LoadData(true);
                if (null != data && data.Count > 0)
                {
                    dtgvSubCatg.DataSource = null;
                    dtgvSubCatg.DataSource = data;
                }
            }
            catch
            {
                MessageBox.Show("系统异常！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtgvSubCatg_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dtgvSubCatg.Columns[e.ColumnIndex].Name == colSt.Name)
            {
                if (null == e.Value || e.Value.ToString() == string.Empty)
                    return;
                e.Value = ST.CATG_MAP[e.Value.ToString()];
            }
        }

        private void dtgvSubCatg_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow curRow = dtgvSubCatg.CurrentRow;
            txtMdfyCName.Text = StringUtil.Obj2Str(curRow.Cells[colCName.Name].Value);
            txtMdfyPinyin.Text = StringUtil.Obj2Str(curRow.Cells[colPinYin.Name].Value);
            txtMdfyAbbr.Text = StringUtil.Obj2Str(curRow.Cells[colAbbr.Name].Value);
            short superId = StringUtil.Obj2Short(curRow.Cells[colSuperID.Name].Value);
            string stEnum = StringUtil.Obj2Str(curRow.Cells[colStEnum.Name].Value);
            cboxMdfySuperName.SelectedValue = superId;

            if (stEnum == ST.VALID)
                rbtnMdfyValid.Checked = true;
            else if (stEnum == ST.INVALID)
                rbtnMdfyInvalid.Checked = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(chngFlag)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
            this.Close();
            chngFlag = false;
        }

        private void btnMdfy_Click(object sender, EventArgs e)
        {
            DataGridViewRow curRow = dtgvSubCatg.CurrentRow;
            string cid = StringUtil.Obj2Str(curRow.Cells[colCID.Name].Value);
            string name = StringUtil.Obj2Str(curRow.Cells[colCName.Name].Value);

            string newName = txtMdfyCName.Text.Trim();
            if (newName == string.Empty)
            {
                MessageBox.Show("分类名称不能为空！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMdfyCName.Focus();
                return;
            }
            if (newName != name)
            {
                int cnt = manager.GenCatgCount(newName);
                if (cnt > 0)
                {
                    MessageBox.Show("分类名称已存在！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMdfyCName.Focus();
                    return;
                }
            }
            short superId = StringUtil.Obj2Short(cboxMdfySuperName.SelectedValue);
            if (!catgManager.isValid(superId))
            {
                MessageBox.Show("综合分类已废弃！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboxMdfySuperName.Focus();
                return;
            }
            string stEnum = ST.VALID;
            if (rbtnMdfyInvalid.Checked)
                stEnum = ST.INVALID;
            
            SubCategory subCatg = new SubCategory();
            subCatg.CId__PK = StringUtil.Obj2Short(cid);
            subCatg.CName = newName;
            subCatg.PinYin = txtMdfyPinyin.Text.Trim();
            subCatg.Abbr = txtMdfyAbbr.Text.Trim();
            subCatg.SuperId = superId;
            subCatg.St = stEnum;

            int exCnt = manager.Update(subCatg);
            string msg = "修改失败！";
            if (exCnt > 0)
            {
                msg = "修改成功！";
                BindData();
                chngFlag = true;
            }
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtAddCName.Text.Trim();
            short superId = StringUtil.Obj2Short(cboxAddSuperName.SelectedValue);
            if (name == string.Empty)
            {
                MessageBox.Show("分类名称不能为空！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddCName.Focus();
                return;
            }
            int cnt = manager.GenCatgCount(name);
            if (cnt > 0)
            {
                MessageBox.Show("分类名称已存在！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddCName.Focus();
                return;
            }
            if (!catgManager.isValid(superId))
            {
                MessageBox.Show("综合分类已废弃！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboxAddSuperName.Focus();
                return;
            }
            SubCategory subCatg = new SubCategory();
            subCatg.St = ST.VALID;
            subCatg.CName = name;
            subCatg.PinYin = txtAddAbbr.Text.Trim();
            subCatg.Abbr = txtAddAbbr.Text.Trim();
            subCatg.SuperId = superId;

            int exCnt = manager.Save(subCatg);
            string msg = "添加失败！";
            if (exCnt > 0)
            {
                msg = "添加成功！";
                BindData();
                chngFlag = true;
            }
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
