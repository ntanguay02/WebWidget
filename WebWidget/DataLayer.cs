using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

/// File: DataLayer.cs
/// Name: Nicholas Tanguay
/// Class: CITC 1317
/// Semester: Fall 2023
/// Project: WebWidget
namespace WebWidget
{
    /// <summary>
    /// The data DataLayer class is used to hide implementation details of
    /// connecting to the database doing standard CRUD operations.
    /// 
    /// IMPORTANT NOTES:
    /// On the serverside, any input-output operations should be done asynchronously. This includes
    /// file and database operations. In doing so, the thread is freed up for the entire time a request
    /// is in flight. When a request executes the await code, the request thread is returned back to the
    /// thread pool. When the request is satisfied, the thread is taken from the thread pool and continues.
    /// This is all built into the .NET Core Framework making it very easy to implement into our code.
    /// 
    /// When throwing an exception from an ASYNC function the exception is never thrown back to the calling entity. 
    /// This makes sense because the function could possibly block and cause strange and unexpected 
    /// behavior. Instead, we will LOG the exception.
    /// </summary>
    internal class DataLayer
    {

        #region "Properties"

        /// <summary>
        /// This variable holds the connection details
        /// such as name of database server, database name, username, and password.
        /// The ? makes the property nullable
        /// </summary>
        private readonly string? connectionString = null;

        #endregion

        #region "Constructors"

        /// <summary>
        /// This is the default constructor and has the default 
        /// connection string specified 
        /// </summary>
        public DataLayer()
        {
            //preprocessor directives can help by using a debug build database environment for testing
            // while using a production database environment for production build.
#if (DEBUG)
            //connectionString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=WebWidget;Integrated Security=True;Connect Timeout=30";
            connectionString = @"Server=localhost;Port=3306;Database=WebWidget;Uid=root;Pwd=''";
#else
            connectionString = @"Production Server Connection Information";
#endif
        }

        /// <summary>
        /// Parameterized Constructor passing in a connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public DataLayer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion

        #region "Database Operations"

        /// <summary>
        /// Get all widgets in the database and return in a List
        /// </summary>
        /// <param>None</param>
        /// <returns>List of Widgets </returns>
        /// <exception cref="Exception"></exception>
        public List<Widget> GetWidgets()
        {
            List<Widget> widgets= new();

            try
            {

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spGetWidgets", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                using MySqlDataReader rdr = (MySqlDataReader)cmd.ExecuteReader();

                // if the reader contains a data set, convert to widget objects
                while (rdr.Read())
                {
                    Widget widget= new Widget();

                    widget.Id = (int)rdr.GetValue(0);
                    widget.Name = (string)rdr.GetValue(1);
                    widget.Description = (string)rdr.GetValue(2);
                    widget.Cost = (double)rdr.GetValue(3);
                    widget.Location = (string)rdr.GetValue(4);

                    widgets.Add(widget);
                }
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //check for widgets length to be zero after returned from database
            return widgets;

        } // end function GetWidgets

        /// <summary>
        /// Get a user by key (GUID)
        /// returns a single User object or a null User
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Widget</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public Widget? GetUserLevelByKey(string key)
        {

            Widget? userDTO = null;

            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException("Username or Password can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new MySqlConnection(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spGetUserLevel", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("aUserKey", key));

                // execute the command which returns a data reader object
                using MySqlDataReader rdr = (MySqlDataReader)cmd.ExecuteReader();

                // if the reader contains a data set, load a local user object
                if (rdr.Read())
                {
                    userDTO = new();
                    userDTO.Name = (string)rdr.GetValue(0);
                    userDTO.Id = (int)rdr.GetValue(1);
                }
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            return userDTO;

        } // end function GetUserLevelByKey

        /// <summary>
        /// Gets a widget in the database by widget ID and returns an Widget or null
        /// </summary>
        /// <param>Id</param>
        /// <returns>Widget</returns>
        /// <exception cref="Exception"></exception>
        public Widget? GetWidgetById(int Id)
        {
            Widget? widget = null;

            try
            {

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spGetAWidget", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("aid", Id));

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                using MySqlDataReader rdr = (MySqlDataReader)cmd.ExecuteReader();

                // if the reader contains a data set, convert to widget objects
                if (rdr.Read())
                {
                    //widget is null so create a new instance
                    widget = new Widget();

                    widget.Id = (int)rdr.GetValue(2);
                    widget.Name = (string)rdr.GetValue(3);
                    widget.Description = (string)rdr.GetValue(4);
                    widget.Cost = (double)rdr.GetValue(5);
                    widget.Location = (string)rdr.GetValue(6);

                }
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //check for widgets length to be zero after returned from database
            return widget;

        } // end function GetWidgetById

        /// <summary>
        /// Insert an widget into the database and return the widgetvwith the new ID
        /// </summary>
        /// <param>Widget</param>
        /// <returns>Widget</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>"
        public Widget? InsertWidget(Widget widget)
        {

            Widget? tempWidget= null;
            try
            {
                if (widget == null)
                {
                    throw new ArgumentNullException("Widget can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spInsertWidget", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("wId", widget.Id));
                cmd.Parameters.Add(new MySqlParameter("wName", widget.Name));
                cmd.Parameters.Add(new MySqlParameter("wDescription", widget.Description));
                cmd.Parameters.Add(new MySqlParameter("wCost", widget.Cost));
                cmd.Parameters.Add(new MySqlParameter("wLocation", widget.Location));

                //create a parameter to hold the output value
                MySqlParameter IdValue = new MySqlParameter("aid", SqlDbType.Int);
                IdValue.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(IdValue);

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                int count = cmd.ExecuteNonQuery();

                // if the reader contains a data set, convert to widget objects
                if (count > 0)
                {
                    //widget is null so create a new instance
                    tempWidget = new Widget();

                    tempWidget.Id = (int)IdValue.Value;

                    tempWidget.Id = widget.Id;
                    tempWidget.Name = widget.Name;
                    tempWidget.Description= widget.Description;
                    tempWidget.Cost = widget.Cost;
                    tempWidget.Location = widget.Location;

                }
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //widget is null if there was an error
            return tempWidget;

        } // end function GetWidgetById

        /// <summary>
        /// Update an widget in the database and return row count affected
        /// </summary>
        /// <param>Id, Widget</param>
        /// <returns>int</returns>
        /// <exception cref="Exception"></exception>
        public int UpdateWidget(int Id, Widget widget)
        {
            int count;

            try
            {
                if (widget == null)
                {
                    throw new ArgumentNullException("Widget can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spUpdateWidget", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("wid", Id));
                cmd.Parameters.Add(new MySqlParameter("wname", widget.Name));
                cmd.Parameters.Add(new MySqlParameter("wdescription", widget.Description));
                cmd.Parameters.Add(new MySqlParameter("wcost", widget.Cost));
                cmd.Parameters.Add(new MySqlParameter("wlocation", widget.Location));

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                count = cmd.ExecuteNonQuery();

            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //widget is null if there was an error
            return count;

        } // end function GetWidgetById

        /// <summary>
        /// Delete an widget in the database and return row count affected
        /// </summary>
        /// <param>Id</param>
        /// <returns>int</returns>
        /// <exception cref="Exception"></exception>
        public int DeleteWidget(int Id)
        {
            int count;
            try
            {
                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spDeleteWidget", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("aid", Id));

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                count = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //widget is null if there was an error
            return count;

        } // end function GetWidgetById

        #endregion

    } // end class DataLayer

} // end namespace