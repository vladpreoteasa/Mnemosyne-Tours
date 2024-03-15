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
    public partial class Login : Form
    {
        private int ghidID = -1;
        int selectedTripID = -1;

        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
             panel1.Location = new Point(
             this.ClientSize.Width / 2 - panel1.Size.Width / 2,
             this.ClientSize.Height / 2 - panel1.Size.Height / 2);
             panel1.Anchor = AnchorStyles.None;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            SignUp signUp = new SignUp();
            signUp.Show();
        }

        static readonly string constr = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=MnemosyneTours;Data Source=DESKTOP-FJITK79\\SQLEXPRESS";
        static readonly SqlConnection con = new SqlConnection(constr);

        private void autentificare_click(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                string query = "select GhidID, Nume, Prenume, Parola from Ghizi where NumeUtilizator = @NumeUtilizator";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@NumeUtilizator", textBox1.Text);

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    sdr.Read();
                    if (sdr["Parola"].Equals(textBox2.Text))
                    {
                        ghidID = Convert.ToInt32(sdr["GhidID"]);
                        MessageBox.Show("Autentificat cu succes", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        panel1.Visible = false;
                        panel2.Visible = true;
                    }
                    else
                        MessageBox.Show("Parolă incorectă", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("Nume de utilizator incorect", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();

                afisare_excursii();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public void afisare_excursii()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                string query = @"
                    SELECT e.Nume 
                    FROM Excursii e
                    INNER JOIN GhiziExcursii ge ON e.ExcursieID = ge.ExcursieID
                    INNER JOIN Ghizi g ON ge.GhidID = g.GhidID
                    WHERE g.GhidID = @GhidID";

                if (!checkBox1.Checked)
                {
                    query += " AND e.DataSosire >= GETDATE()";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GhidID", ghidID);

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    listBox1.Items.Clear();
                    while (sdr.Read())
                        listBox1.Items.Add(sdr["Nume"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {
            panel2.Location = new Point(
            this.ClientSize.Width / 2 - panel2.Size.Width / 2,
            this.ClientSize.Height / 2 - panel2.Size.Height / 2);
            panel2.Anchor = AnchorStyles.None;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedTourName = listBox1.SelectedItem.ToString();
                afisare_detalii_excursie(selectedTourName);
                panel2.Visible = false;
                panel3.Visible = true;
            }
        }

        public void afisare_detalii_excursie(string tourName)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                string query = "SELECT * FROM Excursii WHERE Nume = @NumeExcursie";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NumeExcursie", tourName);

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    listBox2.Items.Clear();
                    while (sdr.Read())
                    {
                        listBox2.Items.Add("Nume: " + sdr["Nume"].ToString() + "\n\n");
                        listBox2.Items.Add("Descriere: " + sdr["Descriere"].ToString() + "\n\n");
                        listBox2.Items.Add("Preț: " + sdr["Pret"].ToString() + "\n\n");
                        listBox2.Items.Add("Perioada: " + sdr["DataSosire"] + " -- " + sdr["DataPlecare"].ToString() + "\n\n");
                    }
                }

                string reservedQuery = "SELECT COUNT(*) AS ReservedCount FROM Rezervari WHERE ExcursieID = (SELECT ExcursieID FROM Excursii WHERE Nume = @NumeExcursie)";
                string totalQuery = "SELECT LocuriDisponibile FROM Excursii WHERE Nume = @NumeExcursie";

                SqlCommand reservedCmd = new SqlCommand(reservedQuery, con);
                reservedCmd.Parameters.AddWithValue("@NumeExcursie", tourName);
                int reservedCount = (int)reservedCmd.ExecuteScalar();

                SqlCommand totalCmd = new SqlCommand(totalQuery, con);
                totalCmd.Parameters.AddWithValue("@NumeExcursie", tourName);
                int totalCount = (int)totalCmd.ExecuteScalar();

                listBox2.Items.Add("Locuri rezervate: " + reservedCount + "/" + totalCount);

                string locationsQuery = @"
                    SELECT L.Nume
                    FROM Locatii L
                    INNER JOIN LocatiiExcursii LE ON L.LocatieID = LE.LocatieID
                    INNER JOIN Excursii E ON LE.ExcursieID = E.ExcursieID
                    WHERE E.Nume = @NumeExcursie
                    ORDER BY L.Nume";

                SqlCommand locationsCmd = new SqlCommand(locationsQuery, con);
                locationsCmd.Parameters.AddWithValue("@NumeExcursie", tourName);

                using (SqlDataReader locationsReader = locationsCmd.ExecuteReader())
                {
                    List<string> locations = new List<string>();
                    while (locationsReader.Read())
                    {
                        locations.Add(locationsReader["Nume"].ToString());
                    }

                    string locationsString = string.Join(", ", locations);
                    listBox2.Items.Add("Locații: " + locationsString);
                }

                string feedbackQuery = @"
                    SELECT C.Nume, C.Prenume, F.Text, F.Rating 
                    FROM Feedback F
                    INNER JOIN Clienti C ON F.ClientID = C.ClientID
                    INNER JOIN Rezervari R ON F.ExcursieID = R.ExcursieID AND F.ClientID = R.ClientID
                    WHERE R.ExcursieID = (SELECT ExcursieID FROM Excursii WHERE Nume = @NumeExcursie)
                    ORDER BY C.Nume";

                SqlCommand feedbackCmd = new SqlCommand(feedbackQuery, con);
                feedbackCmd.Parameters.AddWithValue("@NumeExcursie", tourName);

                using (SqlDataReader feedbackReader = feedbackCmd.ExecuteReader())
                {
                    if (feedbackReader.HasRows)
                    {
                        listBox2.Items.Add("\nRecenzii:\n");
                        while (feedbackReader.Read())
                        {
                            string clientName = feedbackReader["Nume"].ToString() + " " + feedbackReader["Prenume"].ToString();
                            string feedbackText = feedbackReader["Text"].ToString();
                            string rating = feedbackReader["Rating"].ToString();

                            listBox2.Items.Add(clientName + ": " + feedbackText + " - Rating: " + rating + "\n");
                        }
                    }
      
                }

                string clientsQuery = @"
                    SELECT C.Nume, C.Prenume 
                    FROM Clienti C
                    INNER JOIN Rezervari R ON C.ClientID = R.ClientID
                    WHERE R.ExcursieID = (SELECT ExcursieID FROM Excursii WHERE Nume = @NumeExcursie)";

                SqlCommand clientsCmd = new SqlCommand(clientsQuery, con);
                clientsCmd.Parameters.AddWithValue("@NumeExcursie", tourName);

                using (SqlDataReader clientsReader = clientsCmd.ExecuteReader())
                {
                    listBox2.Items.Add("\nLista tuturor clienților:\n");
                    while (clientsReader.Read())
                    {
                        string clientName = clientsReader["Nume"].ToString() + " " + clientsReader["Prenume"].ToString();
                        listBox2.Items.Add(clientName);
                    }
                }

                string getIDQuery = "SELECT ExcursieID FROM Excursii WHERE Nume = @NumeExcursie";

                SqlCommand getIDCmd = new SqlCommand(getIDQuery, con);
                getIDCmd.Parameters.AddWithValue("@NumeExcursie", tourName);

                using (SqlDataReader reader = getIDCmd.ExecuteReader())
                    if (reader.Read())
                        selectedTripID = (int)reader["ExcursieID"];

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            afisare_excursii();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            panel3.Visible = false;
            panel2.Visible = true;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            OpenEditItemForm(false, selectedTripID);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenEditItemForm(true, selectedTripID);
        }

        private void OpenEditItemForm(bool operation, int ID) // operation 0 for update and 1 for delete
        {
            if (!operation)
            {
                if (ID < 0) return;

                EditItem editForm = new EditItem(this);
                editForm.recievedID = ID;
                editForm.recievedOperation = operation;

                TourDetails details = GetTourDetails(ID);
                if (details != null)
                {
                    editForm.textBoxNume.Text = details.Name;
                    editForm.textBoxPret.Text = details.Price.ToString();
                    editForm.textBoxDescriere.Text = details.Description;
                    editForm.textBoxDataSosire.Text = details.ArrivalDate.ToString("yyyy-MM-dd");
                    editForm.textBoxDataPlecare.Text = details.DepartureDate.ToString("yyyy-MM-dd");
                    editForm.textBoxLocuriDisponibile.Text = details.AvailableSeats.ToString();
                }

                editForm.ShowDialog();
            }

            else
            {
                if (ghidID < 0) return;
                EditItem addForm = new EditItem(this);
                addForm.recievedGhidID = ghidID;
                addForm.recievedOperation = operation;

                addForm.ShowDialog();
            }
        }

        private TourDetails GetTourDetails(int tripID)
        {
            TourDetails details = new TourDetails();

            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                string query = "SELECT Nume, Descriere, Pret, DataSosire, DataPlecare, LocuriDisponibile FROM Excursii WHERE ExcursieID = @ExcursieID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ExcursieID", tripID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        details.Name = reader["Nume"].ToString();
                        details.Description = reader["Descriere"].ToString();
                        details.Price = Convert.ToDecimal(reader["Pret"]);
                        details.ArrivalDate = Convert.ToDateTime(reader["DataSosire"]);
                        details.DepartureDate = Convert.ToDateTime(reader["DataPlecare"]);
                        details.AvailableSeats = Convert.ToInt32(reader["LocuriDisponibile"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return details;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selectedTripID == -1)
            {
                MessageBox.Show("Nu este selectată o călătorie.");
                return;
            }

            try
            {
                con.Open();

                string query = @"
            SELECT 
                (SELECT COUNT(*) FROM Rezervari WHERE ExcursieID = @selectedTripID) AS ClientsCount,
                (SELECT COUNT(*) FROM GhiziExcursii WHERE ExcursieID = @selectedTripID) AS GuidesCount";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@selectedTripID", selectedTripID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int clientsCount = reader.GetInt32(reader.GetOrdinal("ClientsCount"));
                        int guidesCount = reader.GetInt32(reader.GetOrdinal("GuidesCount"));

                        if (clientsCount > 0)
                        {
                            MessageBox.Show("Nu s-a putut șterge: Există clienți cu rezervări la această excursie.");
                            return;
                        }

                        if (guidesCount > 1)
                        {
                            MessageBox.Show("Nu s-a putut șterge: Nu sunteți singurul ghid pentru această excursie.");
                            return;
                        }
                    }
                }

                SqlCommand deleteGuideTripCmd = new SqlCommand("DELETE FROM GhiziExcursii WHERE ExcursieID = @selectedTripID AND GhidID = @selectedGhidID", con);
                deleteGuideTripCmd.Parameters.AddWithValue("@selectedTripID", selectedTripID);
                deleteGuideTripCmd.Parameters.AddWithValue("@selectedGhidID", ghidID);
                deleteGuideTripCmd.ExecuteNonQuery();

                SqlCommand deleteLocTripCmd = new SqlCommand("DELETE FROM LocatiiExcursii WHERE ExcursieID = @selectedTripID", con);
                deleteLocTripCmd.Parameters.AddWithValue("@selectedTripID", selectedTripID);
                deleteLocTripCmd.ExecuteNonQuery();

                SqlCommand deleteTripCmd = new SqlCommand("DELETE FROM Excursii WHERE ExcursieID = @selectedTripID", con);
                deleteTripCmd.Parameters.AddWithValue("@selectedTripID", selectedTripID);
                int rowsAffected = deleteTripCmd.ExecuteNonQuery();

                afisare_excursii();
                panel3.Visible = false;
                panel2.Visible = true;

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Excursie ștearsă.");
                }
                else
                {
                    MessageBox.Show("Excursia nu a fost ștearsă.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }

    public class TourDetails
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int AvailableSeats { get; set; }
    }
}
