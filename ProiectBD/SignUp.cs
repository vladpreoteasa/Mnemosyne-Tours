using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProiectBD
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }
    
        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {
            panel1.Location = new Point(
            this.ClientSize.Width / 2 - panel1.Size.Width / 2,
            this.ClientSize.Height / 2 - panel1.Size.Height / 2);
            panel1.Anchor = AnchorStyles.None;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        static readonly string constr = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=MnemosyneTours;Data Source=DESKTOP-FJITK79\\SQLEXPRESS";
        static readonly SqlConnection con = new SqlConnection(constr);

        private void creazaCont_click(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                string selectQuery = "select Nume, Prenume, NumeUtilizator, Parola from Ghizi where Email = @Email";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@Email", textBox3.Text);

                SqlDataReader sdr = selectCmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    sdr.Read();

                    if (sdr["Parola"] == DBNull.Value)
                    {
                        sdr.Close();

                        string updateUsernameQuery = "update Ghizi set NumeUtilizator = @Username where Email = @Email";
                        SqlCommand updateUsernameCmd = new SqlCommand(updateUsernameQuery, con);
                        updateUsernameCmd.Parameters.AddWithValue("@Username", textBox1.Text);
                        updateUsernameCmd.Parameters.AddWithValue("@Email", textBox3.Text);
                        updateUsernameCmd.ExecuteNonQuery();

                        string updatePasswordQuery = "update Ghizi set Parola = @Password where Email = @Email";
                        SqlCommand updatePasswordCmd = new SqlCommand(updatePasswordQuery, con);
                        updatePasswordCmd.Parameters.AddWithValue("@Password", textBox2.Text);
                        updatePasswordCmd.Parameters.AddWithValue("@Email", textBox3.Text);
                        updatePasswordCmd.ExecuteNonQuery();

                        MessageBox.Show("Cont creat cu succes", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("Cont deja existent", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("E-mail incorect", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
