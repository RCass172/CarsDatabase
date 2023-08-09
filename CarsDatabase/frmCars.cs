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
using System.IO;

namespace CarsDatabase
{
    public partial class frmCars : Form
    {
        // Connection to the database
        public SQLiteConnection myDBConnection = new SQLiteConnection(@"data source=C:\data\hire.db");
        // Makes table to store the data
        public DataTable dtCars = new DataTable();

        // The first car record in database
        int currentHireRecord = 0;

        public frmCars()
        {
            InitializeComponent();

        }

        // Functionality for once the car form loads
        private void frmCars_Load(object sender, EventArgs e)
        {
            // Checks file exists
            if (File.Exists("c:\\data\\hire.db"))
            {
                Console.WriteLine("The file exists.");
            }

            // Sets the text property for the car form with name and today's date
            var dateTime = DateTime.Now;
            this.Text = "Task A - Ruth Cassidy " + dateTime.ToShortDateString();

            DisplayRecord();
        }

        // Functionality that refreshes form back to first record
        private void RefreshForm()
        {
            currentHireRecord = 0;
            DisplayRecord();
        }

        //------ Action buttons ------

        // Functionality for update button
        private void updateBtn_Click(object sender, EventArgs e)
        {
            int available = 0;
            double number;
            DateTime date;

            // If available is checked it will save as 1 to database
            if (availableCheck.Checked)
            {
                available = 1;
            }

            // Using the SQL update query to modify a record in the database 
            string sqlUpdate = $"UPDATE tblCar " +
                $"SET VehicleRegNo = '{regTxt.Text.ToUpper()}', " +
                $"Make = '{makeTxt.Text}'," +
                $"EngineSize = '{engineTxt.Text}'," +
                $"DateRegistered = '{dateTxt.Text}'," +
                $"RentalPerDay = '{rentalTxt.Text.TrimStart('€')}'," +
                $"Available = '{available}' " +
                $"WHERE VehicleRegNo = '{regTxt.Text}'";

            // Checks if update is wanted to execute
            DialogResult closeWindow = MessageBox.Show("Are you sure you want to update?",
                "Update",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            if (closeWindow.Equals(DialogResult.OK))
            {
                // Checks if date text is not correct date format to throw error message
                if (!DateTime.TryParseExact(dateTxt.Text, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                {
                    MessageBox.Show("Please enter date format DD/MM/YYYY",
                        "Error",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Error);
                }
                // Checks if rental text is not a number to throw an error massage
                else if (!double.TryParse(rentalTxt.Text.TrimStart('€'), out number))
                {
                    MessageBox.Show("Please enter a number in rental per day field",
                        "Error",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Error);
                }
                else
                {
                    myDBConnection.Open();

                    // Creates a new command instance with the update and connection
                    SQLiteCommand sqlCommand = new SQLiteCommand(sqlUpdate, myDBConnection);

                    // Needed to execute queries that don't return any data
                    sqlCommand.ExecuteNonQuery();

                    dtCars.Clear();
                    myDBConnection.Close();
                    MessageBox.Show("Vehicle No. " + regTxt.Text + " has been updated successfully",
                        "Updated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    DisplayRecord();
                }
            }
        }

        // Functionality for add button
        private void addBtn_Click(object sender, EventArgs e)
        {
            frmAdd addForm = new frmAdd();
            addForm.Show();

            // Refreshes form
            RefreshForm();
        }

        // Functionality for delete button
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            DialogResult deleteRecord = MessageBox.Show("Are you sure you want to delete Vehicle No. " + regTxt.Text + "?",
                    "Delete",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);
            if (deleteRecord.Equals(DialogResult.OK))
            {
                var vehicleText = regTxt.Text;
                string sqlDelete = $"DELETE FROM tblCar WHERE VehicleRegNo ='{vehicleText}'";

                myDBConnection.Open();
                SQLiteCommand sqlDeleteQuery = new SQLiteCommand(sqlDelete, myDBConnection);
                sqlDeleteQuery.ExecuteNonQuery();
                MessageBox.Show("Record has been deleted",
                    "Delete Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                myDBConnection.Close();

                RefreshForm();
            }
        }

        // Functionality for search button
        private void searchBtn_Click(object sender, EventArgs e)
        {
            frmSearch searchForm = new frmSearch();
            searchForm.Show();
        }

        // Functionality for cancel button
        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DisplayRecord();
        }

        // Functionality for exit button
        private void exitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        //------ Navigation buttons ------

        // Functionality for nrxt button
        private void nextBtn_Click(object sender, EventArgs e)
        {
            // moves forward 1 record and displays
            currentHireRecord++;
            DisplayRecord();
        }

        // Functionality for previous button
        private void prevBtn_Click(object sender, EventArgs e)
        {
            // moves back 1 record and displays
            currentHireRecord--;
            DisplayRecord();
        }

        // Functionality for first button
        private void firstBtn_Click(object sender, EventArgs e)
        {
            // moves back to first record and displays
            currentHireRecord = 0;
            DisplayRecord();
        }

        // Functionality for last button
        private void lastBtn_Click(object sender, EventArgs e)
        {
            // moves to last record and displays
            currentHireRecord = dtCars.Rows.Count - 1;
            DisplayRecord();
        }

        // Functionality to obtain data from the database
        private void FetchDataFromDB()
        {
            // try and catch in order to show any runtime errors
            try
            {
                // Check if database is open
                if (myDBConnection.State != System.Data.ConnectionState.Open)
                {
                    // opens the database
                    myDBConnection.Open();
                }

                string mySQLStatement = "SELECT * FROM tblCar";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(mySQLStatement, myDBConnection);
                dtCars.Clear();
                adapter.Fill(dtCars);

                // Closes the database
                myDBConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex);
            }
        }

        // Functionality to display a record from database
        private void DisplayRecord()
        {
            FetchDataFromDB();

            regTxt.Text = dtCars.Rows[currentHireRecord].ItemArray[1].ToString();
            makeTxt.Text = dtCars.Rows[currentHireRecord].ItemArray[2].ToString();
            engineTxt.Text = dtCars.Rows[currentHireRecord].ItemArray[3].ToString();
            dateTxt.Text = dtCars.Rows[currentHireRecord].ItemArray[4].ToString();
            rentalTxt.Text = "€" + String.Format("{0:0.00}", dtCars.Rows[currentHireRecord].ItemArray[5]);
            availableCheck.Checked = Convert.ToBoolean(dtCars.Rows[currentHireRecord].ItemArray[6]);

            RecordCount();
        }

        /* Functionality to display current record number and total
        record count along with disabling navigation buttons*/
        private void RecordCount()
        {
            if (currentHireRecord == 0)
            {
                /* Makes sure previous and first buttons are 
                disabled once at the start of the records */
                prevBtn.Enabled = false;
                nextBtn.Enabled = true;
                firstBtn.Enabled = false;
                lastBtn.Enabled = true;
            } 
            else if (currentHireRecord >= (dtCars.Rows.Count - 1))
            {
                /* Makes sure next and last buttons are 
                disabled once at the end of the records */
                prevBtn.Enabled = true;
                nextBtn.Enabled = false;
                firstBtn.Enabled = true;
                lastBtn.Enabled = false;
            }
            else
            {
                // Ensures all buttons enabled otherwise
                prevBtn.Enabled = true;
                nextBtn.Enabled = true;
                firstBtn.Enabled = true;
                lastBtn.Enabled = true;
            }

            // Displays current record number and total record count
            displayTxt.Text = (currentHireRecord + 1) + " of " + dtCars.Rows.Count;
        }

        /* Creates shortcuts to the update add delete search cancel 
         and exit buttons by pressing ALT + relevant letter*/
        private void frmCars_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt == true && e.KeyCode == Keys.U)
            {
                updateBtn.PerformClick();
            }

            if (e.Alt == true && e.KeyCode == Keys.A)
            {
                addBtn.PerformClick();
            }

            if (e.Alt == true && e.KeyCode == Keys.D)
            {
                deleteBtn.PerformClick();
            }

            if (e.Alt == true && e.KeyCode == Keys.S)
            {
                searchBtn.PerformClick();
            }

            if (e.Alt == true && e.KeyCode == Keys.C)
            {
                cancelBtn.PerformClick();
            }

            if (e.Alt == true && e.KeyCode == Keys.X)
            {
                exitBtn.PerformClick();
            }
        }
    }
}
