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
    public partial class EditItem : Form
    {
        public bool recievedOperation = false;
        public int recievedID = -1;
        public int recievedGhidID = -1;
        private Login loginForm;

        public EditItem(Login login)
        {
            InitializeComponent();
            this.loginForm = login;
        }

        private void EditItem_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        static readonly string constr = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=MnemosyneTours;Data Source=DESKTOP-FJITK79\\SQLEXPRESS";
        static readonly SqlConnection con = new SqlConnection(constr);

        private void button1_click(object sender, EventArgs e)
        {
            if (!recievedOperation)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    string query = "UPDATE Excursii SET Nume = @nume, Pret = @pret, Descriere = @descriere, DataSosire = @dataSosire, DataPlecare = @dataPlecare, LocuriDisponibile = @locuriDisponibile WHERE ExcursieID = @id";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@nume", textBoxNume.Text);
                    cmd.Parameters.AddWithValue("@pret", textBoxPret.Text);
                    cmd.Parameters.AddWithValue("@descriere", textBoxDescriere.Text);
                    cmd.Parameters.AddWithValue("@dataSosire", textBoxDataSosire.Text);
                    cmd.Parameters.AddWithValue("@dataPlecare", textBoxDataPlecare.Text);
                    cmd.Parameters.AddWithValue("@locuriDisponibile", textBoxLocuriDisponibile.Text);
                    cmd.Parameters.AddWithValue("@id", recievedID);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Update realizat cu succes");
                    }
                    else
                    {
                        MessageBox.Show("Update eșuat");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }

                loginForm.afisare_detalii_excursie(textBoxNume.Text);
            }

            else
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    string insertQuery = "INSERT INTO Excursii (Nume, Pret, Descriere, DataSosire, DataPlecare, LocuriDisponibile) VALUES (@nume, @pret, @descriere, @dataSosire, @dataPlecare, @locuriDisponibile); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmd = new SqlCommand(insertQuery, con);

                    cmd.Parameters.AddWithValue("@nume", textBoxNume.Text);
                    cmd.Parameters.AddWithValue("@pret", textBoxPret.Text);
                    cmd.Parameters.AddWithValue("@descriere", textBoxDescriere.Text);
                    cmd.Parameters.AddWithValue("@dataSosire", textBoxDataSosire.Text);
                    cmd.Parameters.AddWithValue("@dataPlecare", textBoxDataPlecare.Text);
                    cmd.Parameters.AddWithValue("@locuriDisponibile", textBoxLocuriDisponibile.Text);
                    cmd.Parameters.AddWithValue("@ghidID", recievedGhidID);

                    int newExcursieID = Convert.ToInt32(cmd.ExecuteScalar());

                    string ghiziExcursiiQuery = "INSERT INTO GhiziExcursii (ExcursieID, GhidID) VALUES (@newExcursieID, @currentGuideID)";
                    SqlCommand ghiziExcursiiCmd = new SqlCommand(ghiziExcursiiQuery, con);

                    ghiziExcursiiCmd.Parameters.AddWithValue("@newExcursieID", newExcursieID);
                    ghiziExcursiiCmd.Parameters.AddWithValue("@currentGuideID", recievedGhidID);
                    int result = ghiziExcursiiCmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Excursie adăugată cu succes.");
                    }
                    else
                    {
                        MessageBox.Show("Eroare la adăugarea excursiei.");
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }

                loginForm.afisare_excursii();
            }
        }
    }
}
