using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace CarsDatabase
{
    public partial class frmSearch : Form
    {
        // Connection to the database
        public SQLiteConnection myDBConnection = new SQLiteConnection(@"data source=C:\data\hire.db");
        public DataTable dtCars = new DataTable();

        public frmSearch()
        {
            InitializeComponent();
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            // Sets the text property for the car form with name and today's date
            var dateTime = DateTime.Now;
            this.Text = "Task A Search - Ruth Cassidy " + dateTime.ToShortDateString();

            // Creates connection to the database
            myDBConnection.Open();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT * FROM tblCar", myDBConnection);
            DataTable dataTable = new DataTable();
            // Fills the data grid view with database table data
            adapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;
            dataGridView1.Columns["uid"].Visible = false;
            myDBConnection.Close();

            // Populates the cboField
            cboField.Items.Add("Make");
            cboField.Items.Add("EngineSize");
            cboField.Items.Add("RentalPerDay");
            cboField.Items.Add("Available");
        }

        // Functionality when a field item is selected to show relevant operator
        private void cboField_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboOperator.Items.Clear();
            // Populates the cboOperator with relevant items for each field item
            if (cboField.SelectedItem == "Make" || cboField.SelectedItem == "Available")
            {
                cboOperator.Items.Add("=");
            }
            else
            {
                cboOperator.Items.Add(" = ");
                cboOperator.Items.Add(" < ");
                cboOperator.Items.Add(" > ");
                cboOperator.Items.Add(" <= ");
                cboOperator.Items.Add(" >= ");
            }
        }

        // Functionality to run the search query
        private void btnRun_Click(object sender, EventArgs e)
        {
            string whereQuery = "";
            var valueInput = valueBox.Text.ToLower();

            // Makes sure all input boxes have values entered
            if (cboField.Text == "")
            {
                MessageBox.Show("Please pick a field value from list",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            else if (cboOperator.Text == "")
            {
                MessageBox.Show("Please pick an operator value from list",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            else if (valueBox.Text == "")
            {
                MessageBox.Show("Please enter a value",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int cost;
            // Makes sure RentalPerDay has a numeric value entered to valueBox
            if (!int.TryParse(valueBox.Text, out cost) && cboField.Text == "RentalPerDay")
 
            {
                MessageBox.Show("Please enter a number in Value field",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Creates the where queries for each field
            if (cboField.Text == "Make")
            {
                whereQuery = " WHERE Make " + cboOperator.Text + "'" + valueBox.Text + "'";
            }

            if (cboField.Text == "EngineSize")
            {
                whereQuery = "WHERE EngineSize " + cboOperator.Text + "'" + valueBox.Text + "'";
            }

            if (cboField.Text == "RentalPerDay")
            {
                whereQuery = "WHERE RentalPerDay " + " " + cboOperator.Text + " " + valueBox.Text;
            }

            if (cboField.Text == "Available")
            {
                int available;
                if (valueInput == "yes")
                {
                    available = 1;
                }
                else
                {
                    available = 0;
                }
                whereQuery = "WHERE Available " + " " + cboOperator.Text + " " + available;
            }


            // Will write what is asked for from database
            string mySQLStatement = "SELECT * FROM tblCar " + whereQuery;

            // Opens the database connection
            myDBConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(mySQLStatement, myDBConnection);

            // Clears the prvious table contents
            dtCars.Clear();
            // Fills the information into the table
            dataAdapter.Fill(dtCars);
            dataGridView1.DataSource = dtCars;
            dataGridView1.Columns["uid"].Visible = false;

            // Closes the database connection
            myDBConnection.Close();
        }

        // Functionality to close the search form
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
