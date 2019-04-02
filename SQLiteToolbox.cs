using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace SmiteBot
{
    public class SQLiteToolBox
    {
        public SQLiteConnection smiteDBConnection { get; private set; }

        public SQLiteToolBox()
        {
            //SQLiteConnection.CreateFile("SmiteDatabase.sqlite");
            smiteDBConnection = new SQLiteConnection("Data Source=SmiteDatabase.sqlite;Version=3;");
            smiteDBConnection.Open();
            SQLiteCommand command;

            string drop = "drop table gods; drop table players";
            command = new SQLiteCommand(drop, smiteDBConnection);
            //command.ExecuteNonQuery();

            string createGodsTable = "create table Gods (GodID int primary key, Name varchar(40), Title varchar(40), Roles varchar(40), Type varchar(40), Pantheon varchar(40), Lore text, FreeRotation int)";
            command = new SQLiteCommand(createGodsTable, smiteDBConnection);
            //command.ExecuteNonQuery();

            string createPlayersTable = "create table Players (PlayerID int primary key, Name varchar(40), Status varchar(40), AccountCreated varchar(40), LastLogin varchar(40), Level int, Wins int, Losses int, Leaves int, MasteryLevel int, Clan varchar(40))";
            command = new SQLiteCommand(createPlayersTable, smiteDBConnection);
            //command.ExecuteNonQuery();
        }

       
        /// Executes a SQL Insert through a Non-Query Command.
        /// <param name="Query">SQL Insert Query</param>
        /// <returns>Returns true if there is an error.</returns>
        public bool InsertInto(string Query)
        {
            bool error = false;

            using (SQLiteConnection sqlCon = new SQLiteConnection(smiteDBConnection))
            using (SQLiteCommand sqlCmd = new SQLiteCommand(Query, sqlCon))
            {
                //Setup Command properties
                sqlCmd.CommandTimeout = 0;

                try
                {
                    //
                    sqlCmd.ExecuteNonQuery();
                }
                catch
                { error = true; }
                finally
                { sqlCon.Close(); }
            }
            return error;
        }
        
        /// Fills a DataTable.
        /// SQL Query to Fill DataTable, but hold onto the forma
        /// <param name="Query">SQL Query to Fill DataTable.</param>
        /// <param name="Table">An initialized DataTable to send by reference.</param>
        /// <returns>Returns true if there is an error</returns>
        public bool FillDataTable(string Query, ref DataTable Table)
        {
            bool error = false;
            //clear Datatable or new data won't fill
            Table.Clear();

            using (SQLiteConnection sqlCon = new SQLiteConnection(smiteDBConnection))
            using (SQLiteDataAdapter sqlAdtpr = new SQLiteDataAdapter(Query, sqlCon))
            {
                sqlAdtpr.SelectCommand.CommandTimeout = 0;
                try
                {

                    sqlAdtpr.Fill(Table);
                }
                catch (Exception e)
                { error = true; }
                finally
                { sqlCon.Close(); }
            }
            return error;
        }

        
        /// Pulls the first answer from the SQL Database
        /// if an error occurs the answer will be null
        /// <param name="Query">SQL Query</param>
        /// <param name="Answer">An uninitialized string to place the answer into.</param>
        /// <returns>Returns true if there is an error</returns>
        public bool SingleAnswer(string Query, out string Answer)
        {
            bool error = false;

            using (SQLiteConnection sqlCon = new SQLiteConnection(smiteDBConnection))
            using (SQLiteCommand sqlCmd = new SQLiteCommand(Query, sqlCon))
            {
                sqlCmd.CommandTimeout = 0;
                try
                {

                    SQLiteDataReader sqlRdr = sqlCmd.ExecuteReader();
                    sqlRdr.Read();
                    Answer = sqlRdr[0].ToString();

                    //Dispose & Close
                    sqlRdr.Close();
                    sqlRdr.Dispose();
                }
                catch (Exception e)
                {
                    error = true;
                    Answer = null;
                }
                finally
                { sqlCon.Close(); }
            }
            return error;
        }
        
        /// Pulls the first answer from the SQL Database
        /// if an error occurs the answel will be -999        
        /// <param name="Query">SQL Query</param>
        /// <param name="Answer">An uninitialized integer to place the answer into.</param>
        /// <returns> Returns true if there is an error.</returns>
        public bool SingleAnswer(string Query, out int Answer)
        {
            bool error = false;
            using (SQLiteConnection sqlCon = new SQLiteConnection(smiteDBConnection))
            using (SQLiteCommand sqlCmd = new SQLiteCommand(Query, sqlCon))
            {
                sqlCmd.CommandTimeout = 0;
                try
                {
                    object _rawVal;

                    //-- Pull Value --

                    SQLiteDataReader sqlRdr = sqlCmd.ExecuteReader();
                    sqlRdr.Read();

                    //-- Check datatype --
                    //if no correct Convert
                    _rawVal = sqlRdr[0];
                    if (_rawVal.GetType() != typeof(int))
                    { Answer = Convert.ToInt32(_rawVal); }
                    else
                    { Answer = (int)_rawVal; }

                    //-- Dispose & Close --
                    sqlRdr.Close();
                    sqlRdr.Dispose();
                }
                catch (Exception e)
                {
                    error = true;
                    Answer = -999;
                }
                finally
                { sqlCon.Close(); }
            }
            return error;
        }
        
        /// Pulls the first answer from the SQL Database
        /// if an error occurs the answel will be -999
        /// <param name="Query">SQL Query</param>
        /// <param name="Answer">An uninitialized integer to place the answer into.</param>
        /// <returns>Returns true if there is an error.</returns>
        public bool SingleAnswer(string Query, out double Answer)
        {
            bool error = false;
            using (SQLiteConnection sqlCon = new SQLiteConnection(smiteDBConnection))
            using (SQLiteCommand sqlCmd = new SQLiteCommand(Query, sqlCon))
            {
                sqlCmd.CommandTimeout = 0;
                try
                {
                    object _rawVal;

                    //-- Pull Value --

                    SQLiteDataReader sqlRdr = sqlCmd.ExecuteReader();
                    sqlRdr.Read();

                    //-- Check datatype --
                    //if no correct Convert
                    _rawVal = sqlRdr[0];
                    if (_rawVal.GetType() != typeof(double))
                    { Answer = Convert.ToInt32(_rawVal); }
                    else
                    { Answer = (double)_rawVal; }

                    //-- Dispose & Close --
                    sqlRdr.Close();
                    sqlRdr.Dispose();
                }
                catch (Exception e)
                {
                    error = true;
                    Answer = -999;
                }
                finally
                { sqlCon.Close(); }
            }
            return error;
        }

        public bool Update(string Query)
        {
            bool error = false;

            using (SQLiteConnection sqlCon = new SQLiteConnection(smiteDBConnection))
            using (SQLiteCommand sqlCmd = new SQLiteCommand(Query, sqlCon))
            {
                //Setup Command Properties
                sqlCmd.CommandTimeout = 0;

                try
                {

                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception e)
                { error = true; }
                finally
                { sqlCon.Close(); }
            }

            return error;
        }
    }
}
