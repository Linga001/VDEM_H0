Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Reflection
Imports System.Windows.Forms



Namespace DataManager

    ''' <summary>
    ''' SOCKET LISTENER 
    ''' Developed by : Avinash Desai (Technoforte -TKM IS)
    ''' Email id : TF_Avinashdesai@tkm.co.in
    ''' Date:30-September-2010
    ''' </summary>
    ''' <remarks></remarks>
    ''' 

    Public Class SqlServerDataManager

#Region "Private Attributes"
        Private m_ConnectionString As String
        Public m_Connection As SqlConnection
        Private m_Command As SqlCommand
        Private m_DataAdapter As SqlDataAdapter
        Private m_CommandBuilder As SqlCommandBuilder
        Private StrSql As String
#End Region

#Region "Constructor"
        ' Constructor
        Public Sub New()
            m_ConnectionString = ConfigurationManager.ConnectionStrings("SocketDataConnectionString").ConnectionString

            Try

                m_Connection = New Data.SqlClient.SqlConnection(m_ConnectionString.ToString())
                m_Command = New Data.SqlClient.SqlCommand
                m_Command.Connection = m_Connection
                m_Command.CommandType = CommandType.Text

            Catch ex As System.InvalidOperationException
                MessageBox.Show(ex.Message.ToString)

            Catch ex As Exception
                MessageBox.Show(ex.Message.ToString)
            End Try
        End Sub

#End Region

#Region "Properties"

        ReadOnly Property ConnectionString() As String
            ' This property will give currently using ConnectionString
            Get
                Return m_ConnectionString
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Returns TRUE if the Input SQL raises any error when run against a database table.
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function HasError(ByVal sql As String) As Boolean
            Dim cmdSQL As SqlClient.SqlCommand

            Try
                If m_Connection.State = ConnectionState.Closed Then m_Connection.Open()
                cmdSQL = New SqlClient.SqlCommand(sql, m_Connection)
                cmdSQL.CommandType = CommandType.Text
                cmdSQL.ExecuteNonQuery()
            Catch ex As SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try
            Return False
        End Function

        Public Function ExecuteQuery(ByVal SQL As String) As Boolean
            ' Function will be used by client to Execute SQL Query over SQL Database
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If
            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oObject As Object

            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            Try
                m_Connection.Open()
                oObject = m_Command.ExecuteNonQuery
                m_Connection.Close()
                Return True

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function ExecuteQuery(ByVal SQL As String, ByVal Parameters() As Data.SqlClient.SqlParameter) As Boolean

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            ' Function will be used by client to Execute SQL Query over SQL Database
            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oObject As Object

            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            ' Checking for Parameter collection
            ' And adding to the Command object Collection
            m_Command.Parameters.Clear()
            If Not IsNothing(Parameters) Then
                Dim Parameter As SqlClient.SqlParameter
                For Each Parameter In Parameters
                    m_Command.Parameters.Add(Parameter)
                Next
            End If

            Try
                m_Connection.Open()
                oObject = m_Command.ExecuteNonQuery
                m_Connection.Close()
                Return True

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function GetValue(ByVal SQL As String) As Object
            ' Function will return First-Row First-Column result of executing SQL Query over Access Database
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oObject As Object
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            Try
                m_Connection.Open()
                oObject = m_Command.ExecuteScalar()
                m_Connection.Close()
                Return oObject

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function getValue(ByVal SQL As String, ByVal Parameters() As Data.SqlClient.SqlParameter) As Object
            ' Function will return First-Row First-Column result of executing SQL Query over Access Database

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oObject As Object
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            ' Checking for Parameter collection
            ' And adding to the Command object Collection
            m_Command.Parameters.Clear()
            If Not IsNothing(Parameters) Then
                Dim Parameter As SqlClient.SqlParameter
                For Each Parameter In Parameters
                    m_Command.Parameters.Add(Parameter)
                Next
            End If


            Try
                m_Connection.Open()
                oObject = m_Command.ExecuteScalar()
                m_Connection.Close()
                Return oObject

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function GetDataTable(ByVal SQL As String) As DataTable
            ' This function will execute sql and retrun flat datatable to the client
            ' an SQLException will be thrown incase of wrong sql statement, 
            ' This function can also throw a generic Database Exception

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oDataTable As New DataTable
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text
            m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)
            m_CommandBuilder = New Data.SqlClient.SqlCommandBuilder(m_DataAdapter)

            Try
                m_DataAdapter.Fill(oDataTable)
                m_DataAdapter.Dispose()
                m_CommandBuilder.Dispose()
                'Imt - Uniqe Grid R&D - S
                oDataTable.TableName = SQL
                'Imt - Uniqe Grid R&D - E
                Return oDataTable

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function GetDataTable(ByVal SQL As String, ByVal Parameters() As Data.SqlClient.SqlParameter) As DataTable
            ' This function will execute sql and retrun flat datatable to the client
            ' an SQLException will be thrown incase of wrong sql statement, 
            ' This function can also throw a generic Database Exception

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oDataTable As New DataTable
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            ' Checking for Parameter collection
            ' And adding to the Command object Collection
            m_Command.Parameters.Clear()
            If Not IsNothing(Parameters) Then
                Dim Parameter As SqlClient.SqlParameter
                For Each Parameter In Parameters
                    m_Command.Parameters.Add(Parameter)
                Next
            End If

            m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)
            m_CommandBuilder = New Data.SqlClient.SqlCommandBuilder(m_DataAdapter)

            Try
                m_DataAdapter.Fill(oDataTable)
                m_DataAdapter.Dispose()
                m_CommandBuilder.Dispose()
                Return oDataTable

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function GetDataReader(ByVal SQL As String) As System.Data.IDataReader

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                MessageBox.Show("Error in Database Connection")
            End If

            Dim oDataReader As Data.SqlClient.SqlDataReader = Nothing
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            Try
                m_Connection.Open()
                oDataReader = m_Command.ExecuteReader(CommandBehavior.CloseConnection)
                Return oDataReader
            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                CloseConnectionObject()
                'MessageBox.Show("Error in Database Connection")
            Catch ex As System.InvalidOperationException
                ' Closing connection
                CloseConnectionObject()
                ''MessageBox.Show("Error in Database Connection")
            Catch ex As Exception
                ' Closing connection
                CloseConnectionObject()
                ''MessageBox.Show("Error in Database Connection")
            Finally
                GetDataReader = oDataReader
            End Try
        End Function

        Public Function GetDataSet(ByVal SQL As String, ByVal TableName As String) As DataSet
            ' Function will return DataSet (May Contain Multiple tables) result of executing SQL Query over Access Database
            ' an SQLException will be thrown incase of wrong sql statement, 
            ' This function can also throw a generic Database Exception
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oDataSet As New DataSet
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text
            m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)
            m_CommandBuilder = New Data.SqlClient.SqlCommandBuilder(m_DataAdapter)

            Try
                m_DataAdapter.Fill(oDataSet, TableName)
                m_DataAdapter.Dispose()
                m_CommandBuilder.Dispose()
                Return oDataSet

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function GetDataSet(ByVal SQL As String, ByVal Parameters() As Data.SqlClient.SqlParameter) As DataSet
            ' Function will return DataSet (May Contain Multiple tables) result of executing SQL Query over Access Database
            ' an SQLException will be thrown incase of wrong sql statement, 
            ' This function can also throw a generic Database Exception

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oDataSet As New DataSet
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            ' Checking for Parameter collection
            ' And adding to the Command object Collection
            m_Command.Parameters.Clear()
            If Not IsNothing(Parameters) Then
                Dim Parameter As SqlClient.SqlParameter
                For Each Parameter In Parameters
                    m_Command.Parameters.Add(Parameter)
                Next
            End If

            m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)
            m_CommandBuilder = New Data.SqlClient.SqlCommandBuilder(m_DataAdapter)

            Try
                m_DataAdapter.Fill(oDataSet)
                m_DataAdapter.Dispose()
                m_CommandBuilder.Dispose()
                Return oDataSet

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function UpdateDataSet(ByVal dsSource As DataSet, Optional ByVal TableName As String = "") As Boolean
            ' Function will update DataSet (May Contain Multiple tables) result of executing SQL Query over Access Database
            ' an SQLException will be thrown incase of wrong sql statement, 
            ' This function can also throw a generic Database Exception
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If


            Dim dsChanges As DataSet

            If TableName.Length <= 0 Then
                Throw New InvalidExpressionException
            End If
            If dsSource.HasChanges Then
                dsChanges = dsSource.GetChanges
            Else
                Return True
            End If
            m_Command.CommandText = TableName
            m_Command.CommandType = CommandType.TableDirect
            m_Command.Connection = m_Connection
            m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)
            m_CommandBuilder = New Data.SqlClient.SqlCommandBuilder(m_DataAdapter)
            Try
                m_DataAdapter.Update(dsChanges, TableName)
                Return True
            Catch ex As System.Data.SqlClient.SqlException
                Throw New InvalidExpressionException(ex.Message.ToString)
            Finally
                ' Closing connection
                Call CloseConnectionObject()
                m_DataAdapter.Dispose()
                dsChanges.Dispose()
                dsSource.Dispose()
            End Try

        End Function

        Public Function UpdataDataTable(ByVal [DataTable] As DataTable, ByVal SQL As String) As Boolean
            ' Function will Updata Table in specific database on the basis of Changes in the DataTable.
            ' SQL will be used to re-Map the DataTable to the same Table and Same Columns in the Access database.

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text
            m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)
            m_CommandBuilder = New Data.SqlClient.SqlCommandBuilder(m_DataAdapter)

            Try
                m_DataAdapter.Update([DataTable])
                m_DataAdapter.Dispose()
                m_CommandBuilder.Dispose()
                Return True

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function UpdataDataTable(ByVal [DataTable] As DataTable, ByVal SQL As String, ByVal Parameters() As Data.SqlClient.SqlParameter) As Boolean
            ' Function will Updata Table in specific database on the basis of Changes in the DataTable.
            ' SQL will be used to re-Map the DataTable to the same Table and Same Columns in the Access database.

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            ' Checking for Parameter collection
            ' And adding to the Command object Collection
            m_Command.Parameters.Clear()
            If Not IsNothing(Parameters) Then
                Dim Parameter As SqlClient.SqlParameter
                For Each Parameter In Parameters
                    m_Command.Parameters.Add(Parameter)
                Next
            End If

            m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)
            m_CommandBuilder = New Data.SqlClient.SqlCommandBuilder(m_DataAdapter)

            Try
                m_DataAdapter.Update([DataTable])
                m_DataAdapter.Dispose()
                m_CommandBuilder.Dispose()
                Return True

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try

        End Function

        Public Function ExecuteStoredProcedure(ByVal StoredProcedureName As String, Optional ByVal Parameters() As SqlClient.SqlParameter = Nothing) As Boolean
            ' This function will execute storedProcedure
            ' and if parameter array is not null then it will assign 
            ' parameter to the parameters of the command object
            ' Function will return First-Row First-Column result of executing SQL Query over Access Database
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            Try
                m_Command.Parameters.Clear()
                m_Command.CommandText = StoredProcedureName
                m_Command.CommandType = CommandType.StoredProcedure

                If Not IsNothing(Parameters) Then
                    Dim oParameter As SqlParameter
                    For Each oParameter In Parameters
                        m_Command.Parameters.Add(oParameter)
                    Next
                End If

                m_Connection.Open()
                m_Command.ExecuteNonQuery()
                m_Connection.Close()
                Return True

            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
                m_Command.Parameters.Clear()
            End Try

        End Function

        Public Function GetDataTableFromSP(ByVal StoredProcedureName As String, Optional ByVal Parameters() As SqlClient.SqlParameter = Nothing) As DataTable
            ' This function will execute storedProcedure
            ' and if parameter array is not null then it will assign 
            ' parameter to the parameters of the command object
            ' Function will return Datatable

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            Try
                m_Command.Parameters.Clear()
                m_Command.CommandText = StoredProcedureName
                m_Command.CommandType = CommandType.StoredProcedure

                If Not IsNothing(Parameters) Then
                    Dim oParameter As SqlParameter
                    For Each oParameter In Parameters
                        m_Command.Parameters.Add(oParameter)
                    Next
                End If

                m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)

                Dim oDataTable As New DataTable

                m_Connection.Open()
                m_DataAdapter.Fill(oDataTable)
                m_Connection.Close()

                Return oDataTable

            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
                m_Command.Parameters.Clear()
                m_DataAdapter.Dispose()
            End Try

        End Function

        Public Function GetDataSetFromSP(ByVal StoredProcedureName As String, Optional ByVal Parameters() As SqlClient.SqlParameter = Nothing) As DataSet
            ' This function will execute storedProcedure
            ' and if parameter array is not null then it will assign 
            ' parameter to the parameters of the command object
            ' Function will return Datatable

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            Try
                m_Command.Parameters.Clear()
                m_Command.CommandText = StoredProcedureName
                m_Command.CommandType = CommandType.StoredProcedure

                If Not IsNothing(Parameters) Then
                    Dim oParameter As SqlParameter
                    For Each oParameter In Parameters
                        m_Command.Parameters.Add(oParameter)
                    Next
                End If

                m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)

                Dim oDataSet As New DataSet

                m_Connection.Open()
                m_DataAdapter.Fill(oDataSet)
                m_Connection.Close()
                Return oDataSet

            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
                m_Command.Parameters.Clear()
            End Try

        End Function

        Public Function GetDataReaderFromSP(ByVal ProcedureName As String, Optional ByVal Parameters() As SqlClient.SqlParameter = Nothing) As System.Data.IDataReader

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If ProcedureName.Length <= 0 Then
                MessageBox.Show("Error in Database Connection")
            End If

            Dim oDataReader As Data.SqlClient.SqlDataReader = Nothing
            m_Command.CommandText = ProcedureName
            m_Command.CommandType = CommandType.StoredProcedure

            ' Checking for Parameter collection
            ' And adding to the Command object Collection
            m_Command.Parameters.Clear()
            If Not IsNothing(Parameters) Then
                Dim Parameter As SqlClient.SqlParameter
                For Each Parameter In Parameters
                    m_Command.Parameters.Add(Parameter)
                Next
            End If

            Try
                m_Connection.Open()
                oDataReader = m_Command.ExecuteReader(CommandBehavior.CloseConnection)
                Return oDataReader
            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                CloseConnectionObject()
                'MessageBox.Show("Error in Database Connection")
            Catch ex As System.InvalidOperationException
                ' Closing connection
                CloseConnectionObject()
                ''MessageBox.Show("Error in Database Connection")
            Catch ex As Exception
                ' Closing connection
                CloseConnectionObject()
                ''MessageBox.Show("Error in Database Connection")
            Finally
                GetDataReaderFromSP = oDataReader
            End Try
        End Function

        Public Function ExecuteScalar(ByVal SQL As String) As Integer
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim iReturn As Integer
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            Try
                m_Connection.Open()
                iReturn = m_Command.ExecuteScalar
                m_Connection.Close()
                Return True

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ExecuteScalar = iReturn
                ' Closing connection                
                Call CloseConnectionObject()
            End Try
        End Function

        Public Function GetTablesList() As DataTable

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            Dim sSql As String = ""
            Dim oDataTable As New DataTable
            'If ConfigurationManager.AppSettings("DatumDBType") = "SQL" Then
            '    sSql = "SELECT * FROM sysobjects WHERE (xtype = 'U')"
            'Else
            '    sSql = "Select Name from MsysObjects where Type =1"
            'End If

            m_Command.CommandText = sSql
            m_Command.CommandType = CommandType.Text
            m_DataAdapter = New Data.SqlClient.SqlDataAdapter(m_Command)
            m_DataAdapter.Fill(oDataTable)
            Return oDataTable
        End Function

        Public Function ValueDouble(ByVal SQL As String) As Double
            ' Function will return First-Row First-Column result of executing SQL Query over Access Database
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim oObject As Double
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            Try
                m_Connection.Open()
                If m_Command.ExecuteScalar() Is DBNull.Value Then
                    oObject = 0.0
                    Return oObject
                Else
                    oObject = m_Command.ExecuteScalar()
                    Return oObject
                End If
                m_Connection.Close()


            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ' Closing connection
                Call CloseConnectionObject()
            End Try
        End Function

        Public Function ExecuteStringScalar(ByVal SQL As String) As String
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            If SQL.Length <= 0 Then
                Throw New InvalidExpressionException
            End If

            Dim iReturn As String = Nothing
            m_Command.CommandText = SQL
            m_Command.CommandType = CommandType.Text

            Try
                m_Connection.Open()
                iReturn = m_Command.ExecuteScalar
                m_Connection.Close()
                Return iReturn

            Catch ex As System.Data.SqlClient.SqlException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidExpressionException(ex.Message.ToString)
            Catch ex As System.InvalidOperationException
                ' Closing connection
                Call CloseConnectionObject()
                Throw New InvalidOperationException(ex.Message.ToString)
            Catch ex As Exception
                ' Closing connection
                Call CloseConnectionObject()
                Throw New Data.DataException
            Finally
                ExecuteStringScalar = iReturn
                ' Closing connection                
                Call CloseConnectionObject()
            End Try
        End Function

        'To do-Executes stored procedure and returns result variable
        Function ExecuteProcedAndReturnValue(ByVal StoredProcedureName As String, Optional ByVal Parameters() As SqlClient.SqlParameter = Nothing)

            Dim RESULT

            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If

            m_Command.Parameters.Clear()
            m_Command.CommandText = StoredProcedureName
            m_Command.CommandType = CommandType.StoredProcedure

            m_Connection.Open()

            If Not IsNothing(Parameters) Then
                Dim oParameter As SqlParameter
                For Each oParameter In Parameters
                    m_Command.Parameters.Add(oParameter)
                Next
            End If

            m_Command.ExecuteNonQuery()
            RESULT = m_Command.Parameters("@SEQNO").Value
            m_Connection.Close()
            Return RESULT
        End Function
#End Region

#Region "Private Methods"
        Public Sub CloseConnectionObject()

            ' Closing connection
            If m_Connection.State = ConnectionState.Open Then
                m_Connection.Close()
            End If
            If Not IsNothing(m_DataAdapter) Then
                m_DataAdapter.Dispose()
            End If
            If Not IsNothing(m_CommandBuilder) Then
                m_CommandBuilder.Dispose()
            End If
        End Sub
#End Region


    End Class

End Namespace