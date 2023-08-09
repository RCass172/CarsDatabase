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
using System.Globalization;

namespace CarsDatabase
{
    public partial class frmAdd : Form
    {
        // Connection to the database
        public SQLiteConnection myDBConnection = new SQLiteConnection(@"data source=C:\data\hire.db");
        public DataTable dtCars = new DataTable();

        public frmAdd()
        {
            InitializeComponent();
        }

        // Functionality when the form is loaded
        private void frmAdd_Load(object sender, EventArgs e)
        {
            // Sets the text property for the car form with name and today's date
            var dateTime = DateTime.Now;
            this.Text = "Task A Add - Ruth Cassidy " + dateTime.ToShortDateString();
        }

        // Functionality for close button
        private void CloseBtn_Click(object sender, EventArgs e)
        {
            DialogResult closeWindow = MessageBox.Show("Are you sure you want to close?",
                "Close Add Form",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);
            if (closeWindow.Equals(DialogResult.OK))
            {
                Close();
            }
        }

        // Functionality for save button
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var available = "0";
           
            // If available is checked it will save as 1 to database
            if (availableCheck.Checked)
            {
                available = "1";
            }

            // Using the SQL insert query to add a record in the database 
            string sqlAdd = $"INSERT INTO tblCar (VehicleRegNo, Make, EngineSize, DateRegistered, RentalPerDay, Available)" +
                " VALUES ('" + regTxt.Text.ToUpper() + "','"
                             + makeTxt.Text + "','"
                             + engineTxt.Text.ToUpper() + "','"
                             + dateTxt.Text + "','"
                             + rentalTxt.Text + "','"
                             + available + "')";

            double number;
            DateTime date;
            // Checks if the vehicle reg number text field is empty to create error message
            if (String.IsNullOrEmpty(regTxt.Text))
            {
                MessageBox.Show("Vehicle Registration Number cannot be empty",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            // Checks vehicle make wasn't input to reg number my mistake
            else if (regTxt.Text == "Ford" || regTxt.Text == "Mercedes" || regTxt.Text == "Mazda" || regTxt.Text == "Honda" || regTxt.Text == "Nissan" || regTxt.Text == "Fiat" || regTxt.Text == "")
            {
                MessageBox.Show("Please enter Vehicle Registration Number and not Make",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            // Checks vehicle reg wasn't entered by mistake to make field
            else if (makeTxt.Text.ToCharArray().Any(char.IsDigit))
            {
                MessageBox.Show("Please enter a Vehicle Make in the Make field",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            // Checks if a value has been added to engine size
            else if (engineTxt.Text == "L")
            {
                MessageBox.Show("Please enter an engine size",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            // Checks if date text is not correct date format to throw error message
            else if (!DateTime.TryParseExact(dateTxt.Text, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
            {
                MessageBox.Show("Please enter date format DD/MM/YYYY",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            // Checks if rental text is not a number to throw an error massage
            else if (!double.TryParse(rentalTxt.Text, out number))
            {
                MessageBox.Show("Please enter a number in rental per day field",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                // Opens database connection
                myDBConnection.Open();

                // Creates a new command instance with the update and connection
                SQLiteCommand sqlCommand = new SQLiteCommand(sqlAdd, myDBConnection);

                // Needed to execute queries that don't return any data
                sqlCommand.ExecuteNonQuery();

                // Closes the database connection
                myDBConnection.Close();
                MessageBox.Show("Vehicle No. " + regTxt.Text + " has been added successfully",
                    "Added",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Close();
            }
        }
    }
}
