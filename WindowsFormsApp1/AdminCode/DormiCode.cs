﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DormiCode : Form
    {       
        //Connection String  
        string cs = "Data Source=222.237.134.74:1522/Ora7;User Id=edu;Password=edu1234;";
        OracleConnection con;
        OracleDataAdapter adapt;
        DataTable dt;       
        DataSet dtSet = new DataSet();
        public DormiCode()
        {
            InitializeComponent();

        }

        private void DormiCode_Load(object sender, EventArgs e)
        {

        }
        private void btn_load()
        {
            con = new OracleConnection(cs);
            con.Open();
            adapt = new OracleDataAdapter("select * from P22_LMG_TATM_DON order by DON_CODE", con);
            dt = new DataTable();
            adapt.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();

            //datagridview1 Columns
            dataGridView1.Columns[0].HeaderText = "동";
            dataGridView1.Columns[1].HeaderText = "기숙사명";
            dataGridView1.Columns[0].Name = "Code";
            dataGridView1.Columns[1].Name = "Name";

        }

        public void btn_enrollment()
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("코드를 입력해주세요.");
                return;
            }
            else
            {
                if (textBox2.Text == "")
                {
                    MessageBox.Show("명칭을 입력해주세요.");
                    return;
                }
            }

            // 오라클 연결
            string queryString = "select * from P22_LMG_TATM_DON where DON_CODE = '" + textBox1.Text + "'";
            using (OracleConnection connection = new OracleConnection(cs))
            {
                OracleCommand command = new OracleCommand(queryString, connection);
                connection.Open();
                OracleDataReader reader;
                reader = command.ExecuteReader();
                Boolean check1 = false;

                // Always call Read before accessing data.
                while (reader.Read())
                {
                    check1 = true;
                }
                if (check1)
                {
                    MessageBox.Show("이미 존재하는 동입니다.");
                    return;
                }
                else
                {
                    command.Connection = connection;

                    // SQL문 지정 및 INSERT 실행
                    this.dataGridView1.AllowUserToAddRows = true;
                    command.CommandText = "INSERT INTO P22_LMG_TATM_DON(DON_CODE, DON_NAME) VALUES('" + textBox1.Text + "','" + textBox2.Text + "')";
                    MessageBox.Show("등록이 완료되었습니다.");
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                // Always call Close when done reading.
                reader.Close();
                connection.Close();
            }
            btn_load();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            string DOR1 = this.dataGridView1.CurrentRow.Cells["Code"].Value.ToString();
            string DOR2 = this.dataGridView1.CurrentRow.Cells["Name"].Value.ToString();

            textBox1.Text = DOR1;
            textBox2.Text = DOR2;

        }
        public void btn_update()
        {
            string DOR1 = this.dataGridView1.CurrentRow.Cells["Code"].Value.ToString();
            // 명령 객체 생성
            con = new OracleConnection(cs);
            con.Open();
            OracleCommand cmdd = new OracleCommand();
            cmdd.Connection = con;
            //학번이 텍박1과 동일한것을 찾아 이름을 변경해준다.
            cmdd.CommandText = "UPDATE P22_LMG_TATM_DON SET DON_NAME = '" + textBox2.Text + "' WHERE DON_CODE = '" + textBox1.Text + "'";
            MessageBox.Show("수정이 완료 되었습니다.");
            cmdd.ExecuteNonQuery();
            btn_load();
            con.Close();
        }

        public void btn_delete()
        {
            int rowindex = dataGridView1.CurrentCell.RowIndex;
            String indexvalue = dataGridView1.Rows[rowindex].Cells[0].Value.ToString();

            con = new OracleConnection(cs);
            con.Open();
            OracleCommand cmdd = new OracleCommand();
            cmdd.Connection = con;

            OracleCommand cmd1 = con.CreateCommand();
            String sql = "select * from p22_lmg_tatm_stu where stu_dor_don = '" + indexvalue + "'";
            cmd1.CommandText = sql;
            Boolean check = false;
            OracleDataReader dr = cmd1.ExecuteReader();
            while (dr.Read())
            {
                check = true;
            }
            if (check)
            {
                MessageBox.Show("사용중인 코드는 삭제할 수 없습니다.");
                return;
            }
            else
            {
                cmdd.CommandText = "DELETE P22_LMG_TATM_DON WHERE DON_CODE = '" + indexvalue + "'";
                MessageBox.Show("삭제가 완료되었습니다.");
                cmdd.ExecuteNonQuery();
                btn_load();
                textBox1.Enabled = true;
                textBox1.Text = "";
                textBox2.Text = "";
                con.Close();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search1_Click();
            }
        }

        public void Search1_Click()
        {
            con = new OracleConnection(cs);
            con.Open();
            adapt = new OracleDataAdapter("select * from P22_LMG_TATM_DON where DON_CODE like '" + textBox1.Text + "%' and DON_NAME like '" + textBox2.Text + "%' order by BNK_CODE", con);
            dt = new DataTable();
            adapt.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }
    }
    
}

