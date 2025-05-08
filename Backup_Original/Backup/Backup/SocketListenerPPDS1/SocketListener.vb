Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Data.SqlClient

Public Class SocketListener

#Region "Service Events"
    Dim EM As New SendEMail

    Public Sub New()
        MyBase.new()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    ''' <summary>
    ''' Start of the Service which starts all the process
    ''' </summary>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        PlantCode = "B1"

        Try
            LogFilePath = System.Configuration.ConfigurationManager.AppSettings("ApplicationLog").ToString()
            DataLogPath = System.Configuration.ConfigurationManager.AppSettings("DataLog").ToString()
            WriteToLog("Log File Path Loaded :  " & LogFilePath)
            WriteToLog("DataLog File Path Loaded : " & DataLogPath)

        Catch ex As Exception
            WriteToLog("Error While getting Path data " & ex.Message)
        End Try

StartQuery:
        Try
            PortNumber = SQLDM.GetValue("Select PortNumber from SocketMaster where PlantCode='" & PlantCode & "'")
            WriteToLog("Port Number For Service is : " & PortNumber)
        Catch ex As Exception
            WriteToLog("Error While getting Portnumber  For Plant #1 " & ex.Message)
            GoTo StartQuery
        End Try


        Try
            SocketThread = New Thread(AddressOf Start)
        Catch ex As Exception
            WriteToLog("Error while Addressing the THREAD : " & ex.Message)
        End Try


        Try
            SocketThread.Start()
        Catch ex As Exception
            WriteToLog("Error while Starting the Socket Thread : " & ex.Message)
        End Try


        Dim t As New Thread(AddressOf EmailStartStatus)
        t.Start()


    End Sub

    ''' <summary>
    ''' Continuation of the service which will resume the service
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub OnContinue()


    End Sub

    ''' <summary>
    ''' Stop the service and make unavailable 
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub OnStop()
        ' Add code here to perform any tear-down necessary to stop your service.
        WriteToLog(" ")

        Try
            serverSocket.Stop()
        Catch ex As Exception
            WriteToLog("Error While Stopping the port ..- " & ex.Message & " " & ex.StackTrace)
        End Try

        Try
            SocketThread.Abort()
        Catch ex As Exception
            WriteToLog("Error While aborting the thread ..- " & ex.Message & " " & ex.StackTrace)
        End Try



        Dim t As New Thread(AddressOf EmailStopStatus)
        t.Start()

    End Sub

    Protected Overrides Sub OnShutdown()
        MyBase.OnShutdown()
        WriteToLog("Server Shutdown and Service Stopped.")

        Try
            serverSocket.Stop()
        Catch ex As Exception
            WriteToLog("Error While Stopping the port ..- " & ex.Message & " " & ex.StackTrace)
        End Try

        Try
            SocketThread.Abort()
        Catch ex As Exception
            WriteToLog("Error While aborting the thread ..- " & ex.Message & " " & ex.StackTrace)
        End Try

        Dim t As New Thread(AddressOf EmailStopStatus)
        t.Start()

    End Sub

    Private Sub EmailStartStatus()
        Try
            EM.SendStatusMail("Started", "")
        Catch ex As Exception
            WriteToLog("Error while sending email on STOP - " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Private Sub EmailStopStatus()
        Try
            EM.SendStatusMail("Stopped", "")
        Catch ex As Exception
            WriteToLog("Error while sending email on STOP - " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "Socket Programming and Data Processing"

#Region "Variable Declaration"

    Dim SQLDM As New DataManager.SqlServerDataManager

    Dim SocketThread As Thread
    Dim serverSocket As TcpListener
    Dim ClientSocket As TcpClient

    Dim IsListening As Boolean
    Dim LPort As String


    'The data Received will be divided and stored in these variables.
    Dim DDestinationLogicalName As String
    Dim DTransmissionbaseLogicalName As String
    Dim DSerialNo As String
    Dim DMode As String
    Dim DLength As String
    Dim DDataProcessType As String
    Dim DProcessResult As String
    Dim DLine As String
    Dim DBCSequenceNo As String
    Dim DBodyNo As String
    Dim DTrackingPoint As String
    Dim DPlantCode As String
    Dim DIdentNo As String
    Dim DURN As String
    Dim DProdSuffix As String
    Dim DLotCode As String
    Dim DModelCode As String
    Dim DExteriorColor As String
    Dim DEnginerPrefix As String
    Dim DEnginerNo As String
    Dim DVinNo As String
    Dim DLoDate As String
    Dim DLoTime As String
    Dim DTotalDelay As String
    Dim DPlannedLineOdate As String
    Dim TappingId As String

    Dim DNorm As String
    Dim DGrade As String

    Dim msgLen As Integer
    Dim strResponse As String
    Dim strData As String
    Dim intSerial As Integer

    'Avinash Desai
    'Below used to Justify the Date Length for File names
    Dim LenValue As String

    'Avinash Desai
    'Below used for Message count and Message file name
    Dim AppLength As String
    Public PortNo As String
    Dim Data As String
    Dim I As Integer
    Dim count As Integer

    Dim Dtable As DataTable

    Dim Index As Integer
    Public SocktRun As New Hashtable
    Public SocktLastData As New Hashtable
    Private CloseForm As Boolean
    Private StartPort As String
    Private StartIndex As String

    Private Firsttime(2) As Boolean

    Private PortNumber As Integer

    Public EmailSequence As Integer
    Public EmailBodyNo As String
    Public EmailSerialNo As String

#End Region

#Region "Socket Listening"

    ''' <summary>
    ''' Listen to Port and invoke the process data for splitting data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Start()
        'Buffer for reading data
        Dim bytes(30000) As [Byte]
        Dim Data As String
        Dim Ct As Integer = 0

        ClientSocket = New TcpClient

RestartListening:

        Dim HN As String = Dns.GetHostName
        Dim IP As IPHostEntry = Dns.GetHostByName(HN)

        Dim ipAddress As IPAddress = IP.AddressList(0)


        Try
            serverSocket = New TcpListener(PortNumber)

        Catch ex As Exception
            WriteToLog("ERROR While initializing the server socket in  START procedure - " & ex.Message)
        End Try

        Try
            ' Start listening for client requests.
            serverSocket.Start()
        Catch ex As Exception
            WriteToLog("ERROR While starting the server socket in  START procedure - " & ex.Message & " " & ex.StackTrace)
            Ct = Ct + 1

            If Ct = 3 Then
                WriteToLog("ERROR While starting the server socket in  START procedure -connection attempted " & Ct & " - " & ex.Message & " " & ex.StackTrace)
                Exit Sub
            End If
            GoTo RestartListening
        End Try


        While (True)
            Try
                ClientSocket = serverSocket.AcceptTcpClient()
            Catch ex As Exception
                WriteToLog("ERROR While accepting new client in  START procedure - " & ex.Message & " " & ex.StackTrace)
                WriteToLog("THIS ERROR MAY OCCUR WHILE THE SOCKET IS READY FOR ACCEPTING NEW CLIENT AND SOCKET THREAD FORECIBLY STOPPED. THIS ERROR CAN BE IGNORED WHEN THE SOCKET IS STOPPED")
                GoTo RestartListening
            End Try

            Data = Nothing

            Try
                ' Get a stream object for reading and writing
                Dim networkStream As NetworkStream = ClientSocket.GetStream

                Dim i As Int32

                ' Loop to receive all the data sent by the client.
                i = networkStream.Read(bytes, 0, bytes.Length)

                While (i <> 0)
                    ' Translate data bytes to a ASCII string.
                    Data = System.Text.Encoding.ASCII.GetString(bytes, 0, i)

                    ' Process the data sent by the client.
                    strData = Data.ToUpper()

                    Try
                        ProcessData()
                    Catch ex As Exception
                        WriteToLog("ERROR While Processing the received data in  START procedure - " & ex.Message & " " & ex.StackTrace)
                    End Try

                    Dim Response As Byte() = Nothing

                    Try
                        Response = System.Text.Encoding.ASCII.GetBytes(strResponse)
                    Catch ex As Exception
                        WriteToLog("ERROR While encoding the received data in  START procedure - " & ex.Message & " " & ex.StackTrace)
                    End Try

                    Try
                        networkStream.Write(Response, 0, Response.Length)
                        networkStream.Flush()
                    Catch ex As Exception
                        WriteToLog("While Sending response in  START procedure - " & ex.Message & " " & ex.StackTrace)
                    End Try

                    i = networkStream.Read(bytes, 0, bytes.Length)

                End While

                ' End Client connection
                ClientSocket.Close()

            Catch ex As Exception
                WriteToLog("ERROR While Listening port  in procedure START- " & ex.Message & " " & ex.StackTrace)
            Finally
                serverSocket.Stop()
            End Try

        End While
    End Sub

#End Region

#Region "Socket Data Processing"

    ''' <summary>
    ''' Socket data will be processed with the BC format and Inserted into Database.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessData()


        Dim Query As String = Nothing

        Try
            DProcessResult = "00"
            DDestinationLogicalName = strData.Substring(0, 6)
            DTransmissionbaseLogicalName = strData.Substring(6, 6)
            DSerialNo = strData.Substring(12, 4)
            DMode = strData.Substring(16, 1)
            DLength = strData.Substring(17, 5)
            DDataProcessType = strData.Substring(22, 2)
            DProcessResult = strData.Substring(24, 2)
            DLine = strData.Substring(26, 1)
            'Modification : TrackingPoint Lenth is required 3 character According to the TrackingPoint Configuration into TrackingPointTable.
            DTrackingPoint = strData.Substring(26, 3)
            DBCSequenceNo = strData.Substring(29, 3)
            DBodyNo = strData.Substring(32, 5)
            msgLen = Len(strData)
            DPlantCode = strData.Substring(37, 2)
            DIdentNo = strData.Substring(39, 10)
            DProdSuffix = strData.Substring(49, 2)
            DLotCode = strData.Substring(51, 2)
            DModelCode = strData.Substring(53, 15)
            DExteriorColor = strData.Substring(68, 4)
            DLoDate = strData.Substring(72, 9)
            DLoTime = strData.Substring(81, 4)
            DTrackingPoint = DTrackingPoint.Trim

            TappingId = strData.Substring(85, 2)

            DNorm = strData.Substring(87, 8).Replace(".", "")
            DGrade = strData.Substring(95, 3).Replace(".", "")

        Catch ex As Exception
            WriteToLog("ERROR While Splitting data - " & strData & " - " & ex.Message & " " & ex.StackTrace)
            DProcessResult = "13"
        End Try

        Try
            WriteFormattedData("Received :", strData, PlantCode)
        Catch ex As Exception
            WriteToLog("ERROR While Writing to FormattedData file - " & ex.Message & " " & ex.StackTrace)
        End Try



        'Code modified By Avinash D and Sandesha G R
        'Check for the PLANT 2 
        'This change was made for especially for 5U3 Tracking point data for Plant 1 were in need to avoid data of plant 2 into its count and use only p#1 data.

        Dim DBBCSEQ As Integer = Nothing

        If DPlantCode = "B1" Then


            Query = "Select Length from dbo.SocketMaster where MasterId=(Select MasterId From Socketdetail where TrackingPoint='" & DTrackingPoint & "' and line=" & DLine & " and PlantCode='" & DPlantCode & "')"

            Try
                AppLength = SQLDM.GetValue(Query)

            Catch ex As Exception
                WriteToLog("ERROR While retriving AppLength " & ex.Message & " " & ex.StackTrace)

            End Try


            Try
                LPort = SQLDM.GetValue("Select Portnumber from SocketMaster where MasterId=(Select MasterId from dbo.SocketDetail  where TrackingPoint='" & DTrackingPoint & "' and line=" & DLine & ")")

            Catch ex As Exception
                WriteToLog("ERROR While retriving the port number from database..- " & ex.Message)
            End Try


            Query = "Select BcSequence from dbo.SocketDetail  where TrackingPoint='" & DTrackingPoint & "' and line=" & DLine & " and MasterId=(Select MasterId from dbo.SocketMaster  where PortNumber='" & LPort & "' and PlantCode='" & DPlantCode & "')"
            Try
                DBBCSEQ = SQLDM.GetValue(Query)
            Catch ex As Exception
                WriteToLog("ERROR While retriving Bc Sequence for Tracking point " & DTrackingPoint & ex.Message & ex.StackTrace)
            End Try


            If DBCSequenceNo = "" Or DBCSequenceNo.Trim.Length = Nothing Then
                WriteToLog("ERROR BLANK BC SEQUENCE NUMBER - DATA REJECTED FOR DATA - " & strData & "")
                Exit Sub
            End If


            Try
                If msgLen <> AppLength And (DDataProcessType = "00" Or DDataProcessType = "01" Or (DDataProcessType = "0E" And DTrackingPoint <> "1H0")) Then
                    DProcessResult = "91"

                ElseIf DLength <> (msgLen - 26) And (DDataProcessType = "00" Or DDataProcessType = "01" Or (DDataProcessType = "0E" And DTrackingPoint <> "1H0")) Then

                    DProcessResult = "76"

                ElseIf DProcessResult <> "  " Then

                    DProcessResult = "13"

                ElseIf (DBBCSEQ <> (DBCSequenceNo - 1)) And (DDataProcessType = "00" Or DDataProcessType = "01" Or (DDataProcessType = "0E" And DTrackingPoint = "1H0")) Then
                    If DBCSequenceNo = 0 Then
                        DProcessResult = "00"
                    Else
                        DProcessResult = "90"
                    End If
                Else
                    DProcessResult = "00"
                End If
            Catch ex As Exception

                WriteToLog("ERROR While Validating the BC Sequence number condition  - " & ex.Message)
            End Try

Response:

            Try
                strResponse = DTransmissionbaseLogicalName + DDestinationLogicalName + DSerialNo + "0" + "00000" + DDataProcessType + DProcessResult
            Catch ex As Exception

                WriteToLog("ERROR While Concatinating the response  - " & ex.Message)
            End Try

            'Checks for the Process Type
            'Process Types :
            '01 and 00 are NORMAL
            'OE Empty Dolly - NORMAL Process has to be done but No Need to Create Interface File
            '10,11,12 -RESEND OR RECREATE BC DATA - No Need to Check BC Number and No Need to update BC Number Only Interface File has to be created

            If DProcessResult = "00" And (DDataProcessType = "00" Or DDataProcessType = "01" Or DDataProcessType = "10" Or DDataProcessType = "11" Or DDataProcessType = "12" Or DDataProcessType = "0E") Then

                'If Process Type Matches then Creates the INTERFACE FILE.
                'Creates Interface file for the Below Process Types

                If DDataProcessType = "00" Or DDataProcessType = "01" Or DDataProcessType = "10" Or DDataProcessType = "11" Or DDataProcessType = "12" Or (DDataProcessType = "0E" And DTrackingPoint = "1H0") Then

                    ''''''''''''''''INSERT STATEMENT''''''''''''''''''''''''''''
                    '***********************************************************
                    Try
                        If DPlantCode <> "B2" Then
                            Dim MyParams() As SqlParameter
                            ReDim Preserve MyParams(12)

                            MyParams(0) = New SqlParameter("@Param_BodyNo", DBodyNo.Trim())
                            MyParams(1) = New SqlParameter("@Param_ModelCode", DModelCode.Trim())
                            MyParams(2) = New SqlParameter("@Param_SuffixCode", DProdSuffix)
                            MyParams(3) = New SqlParameter("@Param_ColorCode", DExteriorColor)
                            MyParams(4) = New SqlParameter("@Param_TPID", DTrackingPoint.Trim())
                            MyParams(5) = New SqlParameter("@Param_PlantCode", DPlantCode.Trim())
                            MyParams(6) = New SqlParameter("@Param_IdentNumber", DIdentNo.Trim())
                            MyParams(7) = New SqlParameter("@Param_LineOffDate", DLoDate)
                            MyParams(8) = New SqlParameter("@Param_LineOffTime", DLoTime)
                            MyParams(9) = New SqlParameter("@PARAM_BAYNAME", TappingId)
                            MyParams(10) = New SqlParameter("@PARAM_Norm", DNorm)
                            MyParams(11) = New SqlParameter("@PARAM_Grade", DGrade)
                            MyParams(12) = New SqlParameter("@PARAM_BCSEQUENCENO", DBCSequenceNo)


                            SQLDM.ExecuteStoredProcedure("InsertIntoVehicleInfoPlantA", MyParams)

                        End If


                    Catch ex As Exception
                        WriteToLog("ERROR While inserting the data  - " & ex.Message & "  " & vbNewLine & " STORED PROCEDURE ERROR FOR DATA - " & strData)
                    End Try
                End If

                'ProcessType matches then increment the BcSequence number
                If (DDataProcessType = "01" Or DDataProcessType = "00" Or DDataProcessType = "0E") And DProcessResult = "00" Then

                    Dim NewBc As Integer = DBCSequenceNo

                    If NewBc = "999" Then
                        NewBc = -1
                    Else
                        NewBc = DBBCSEQ + 1
                    End If

                    Try
                        SQLDM.ExecuteQuery("UPDATE [SocketDetail]  SET [BcSequence] = " & NewBc & " WHERE [TrackingPoint] ='" & DTrackingPoint & "' and MasterId=(Select MasterId from dbo.SocketMaster  where PortNumber='" & LPort & "' and PlantCode='" & DPlantCode & "')")

                    Catch ex As Exception
                        WriteToLog("ERROR While Updatig BC Sequence number  - " & ex.Message & "  " & vbNewLine & Query)
                    End Try

                End If
            End If


            'This Else Condition is specially for B2 Plant Code and which has TP 5U3
        ElseIf DPlantCode = "B2" And DTrackingPoint = "5U3" Then


            'Set the Process Type to ZERO. i.e., 
            'This below condition will not check lentgh/any kind of bc error for the data which has plant2.
            'Just it passes the data and sends the OK ACK. If B1 data comes then it will execute Above if condition.


            'No Other Condition needs to be checked because the data itself is not being used by application . Just to Skip the data for P#2 data 
            'this change has been done



            Dim NewBc As Integer = DBCSequenceNo

            If NewBc = "999" Then
                NewBc = -1
            Else
                'This doesnt requires increment by 1 Just have to be assigned to incremented received data.
                NewBc = DBCSequenceNo
            End If

            Try
                'BElow Update Query is also modified to suite the requirement
                SQLDM.ExecuteQuery("UPDATE [SocketDetail]  SET [BcSequence] = " & NewBc & " WHERE [TrackingPoint] ='" & DTrackingPoint & "'")
                DProcessResult = "00"
            Catch ex As Exception
                WriteToLog("ERROR While Updatig BC Sequence number  - " & ex.Message & "  " & vbNewLine & Query)
            End Try

            Try
                strResponse = DTransmissionbaseLogicalName + DDestinationLogicalName + DSerialNo + "0" + "00000" + DDataProcessType + DProcessResult
            Catch ex As Exception

                WriteToLog("ERROR While Concatinating the response  - " & ex.Message)
            End Try


        End If

        Try
            If (DProcessResult <> "00" And EmailBodyNo <> DBodyNo And EmailSequence <> DBCSequenceNo And EmailSerialNo <> DSerialNo) Or (DProcessResult <> "00" And EmailBodyNo = Nothing And EmailSequence = Nothing And EmailSerialNo = Nothing) Then

                EmailBodyNo = Nothing
                EmailSequence = Nothing
                EmailSerialNo = Nothing

                EM.SendMail(DProcessResult & "</br>Sequence No :" & DBBCSEQ & " </br> DATA = " & strData, DBodyNo)

                'Below Sequence is received from GALC not from the Application Database
                EmailBodyNo = DBodyNo
                EmailSequence = DBCSequenceNo
                EmailSerialNo = DSerialNo

            End If

        Catch ex As Exception
            WriteToLog("ERROR While sending BC Error mail  - " & ex.Message & "  " & vbNewLine)
        End Try



        'After Sending the Response . Resetting the Variable data for the next use
        Data = ""

      
        Try
            WriteFormattedData("Sent :", strResponse, PlantCode)
        Catch ex As Exception

            WriteToLog("ERROR While Writing to FormattedData file - " & ex.Message)
        End Try



    End Sub

#End Region

#Region "Data Log and Error Log"

    Private Sub WriteFormattedData(ByVal headerText As String, ByVal formattedData As String, ByVal Plantcode As String)
        ' Writing the formatted data in text file

        Dim Request As String

        If headerText = "Received :" Then
            Request = AppLength
        Else
            Request = 26
        End If

        Request = " Request :" & Request & "  Response :" & Len(formattedData)

        Dim Value As String = "FormattedData_" & Plantcode & "_" & Now.Day & "-" & Now.Month & "-" & Now.Year

        DataLogFileName = DataLogPath & Value & ".txt"

        ' If System.IO.File.Exists(DataLogFileName) = False Then
        'System.IO.File.CreateText(DataLogFileName)
        '  End If


        Dim oWrite As System.IO.StreamWriter
        oWrite = System.IO.File.AppendText(DataLogFileName)

        If headerText = "Received :" Then
            oWrite.Write(vbNewLine)
            oWrite.Write(vbNewLine & headerText + "( " + Format$(Now, "M/d/yy h:mm:ssss") + " )" + Request + vbNewLine + "Data:  " + formattedData)
        Else
            oWrite.Write(vbNewLine & headerText + "( " + Format$(Now, "M/d/yy h:mm:ssss") + " )" + Request + vbNewLine + "Data:  " + formattedData)
        End If

        oWrite.Close()

    End Sub

    Public Sub WriteToLog(ByVal ErrorData As String)
        Try
            Dim Path As String = LogFilePath

            If ErrorData = "" Then
                ErrorData = DateTime.Now & "  " & "Service Started."
            ElseIf ErrorData = " " Then
                ErrorData = DateTime.Now & "  " & "Service Stopped."
            ElseIf ErrorData = "*" Then
                ErrorData = "*************************************************************************************************"
            Else
                ErrorData = DateTime.Now & " -- " & ErrorData
            End If

            Dim oWrite As IO.StreamWriter
            oWrite = New IO.StreamWriter(Path, True)
            Dim t As New System.Windows.Forms.TextBox
            t.WordWrap = False
            t.Text = ErrorData
            ErrorData = t.Text

            oWrite.Write(vbNewLine & ErrorData)

            oWrite.Close()
        Catch ex As Exception

        End Try

    End Sub

#End Region

#End Region
End Class
