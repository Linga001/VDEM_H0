Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Data.SqlClient


Public Class SocketListener

#Region "==>Service Events"
    Dim EM As New SendEMail

    Public Sub New()
        MyBase.New()
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
        '=>Code Modifed by: Mahesh Avanti on Date:21-10-2019 PlantCode=B1-VINMS(New)/Earlier PlantCode=B3
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
            PortNumber = SQLDM.GetValue("Select Port from tblSocketSetting where Id=1")
            WriteToLog("Port Number For Service is : " & PortNumber)
        Catch ex As Exception
            WriteToLog("Error While getting Portnumber For Plant#1-VINMS" & ex.Message)
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
            'EM.SendStatusMail("Started", "")
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

#Region "==>Socket Programming and Data Processing"

#Region "==>Variable Declaration"

    Dim SQLDM As New DataManager.SqlServerDataManager

    Dim SocketThread As Thread
    Dim serverSocket As TcpListener
    Dim ClientSocket As TcpClient

    Dim IsListening As Boolean
    Dim LPort As String


    'The data Received will be divided and stored in these variables.
    Dim BODY_NO As String
    Dim PLANT_CODE As String
    Dim ID_NO As String
    Dim SUFFIX As String
    Dim VIN_NO As String
    Dim MODEL_CODE As String
    Dim COLOR As String
    Dim ASSY_LINE_OFF_SEQ_NO As String
    Dim EG_NO As String
    Dim KEY_NO As String
    Dim BUY_OFF_ETD_with_TIME As String
    Dim ASSY_LO_DATE As String
    Dim ASSY_LO_TIME As String
    Dim EG_PREFIX As String
    Dim ETD_DELAY As String
    Dim CNG_TANK_NUMBER As String


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

    'Newly Added Variables
    Dim EngineModelcode As String
    Dim EngineNumber As String
    Dim WMI As String
    Dim VDS As String
    Dim Carfamily As String
    Dim MMYY As String
    Dim ChkDigit As String
    Dim FrameSeqno As String

    Dim msgLen As Integer
    Dim strResponse As String
    Dim strData As String
    Dim intSerial As Integer
    Dim VINMS As String

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
    'Changed by anupam on 30-Aug-2017
    Dim PalletNo As Integer 'SortingLocationId
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
    Public VehicleInfoID As Integer
    Public PlantID As Integer




#End Region

#Region "==>Socket Listening"

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

#Region "==>Socket Data Processing"

    ''' <summary>
    ''' Socket data will be processed with the BC format and Inserted into Database.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessData()


        Dim Query As String = Nothing
        'Debugger.Break()
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

            DTrackingPoint = strData.Substring(27, 2)
            DBCSequenceNo = strData.Substring(29, 3)
            BODY_NO = strData.Substring(32, 5)
            PLANT_CODE = strData.Substring(42, 2)
            ID_NO = strData.Substring(44, 10)
            SUFFIX = strData.Substring(54, 2)
            VIN_NO = strData.Substring(56, 17)
            VINMS = strData.Substring(73, 4)
            MODEL_CODE = strData.Substring(77, 15)
            COLOR = strData.Substring(92, 3)
            ASSY_LINE_OFF_SEQ_NO = strData.Substring(95, 3)
            EG_NO = strData.Substring(98, 7)
            KEY_NO = strData.Substring(105, 5)
            BUY_OFF_ETD_with_TIME = strData.Substring(110, 12)
            ASSY_LO_DATE = strData.Substring(136, 8)
            ASSY_LO_TIME = strData.Substring(144, 4)
            EG_PREFIX = strData.Substring(148, 5)
            ETD_DELAY = strData.Substring(155, 7)
            CNG_TANK_NUMBER = strData.Substring(162, 20)

            DPlantCode = PLANT_CODE
            msgLen = Len(strData)

            'DDestinationLogicalName = strData.Substring(0, 6)
            'DTransmissionbaseLogicalName = strData.Substring(6, 6)
            'DSerialNo = strData.Substring(12, 4)
            'DMode = strData.Substring(16, 1)
            'DLength = strData.Substring(17, 5)
            'DDataProcessType = strData.Substring(22, 2)
            'DProcessResult = strData.Substring(24, 2)
            'DLine = strData.Substring(26, 1)

            'DTrackingPoint = strData.Substring(26, 3) 'Modification : TrackingPoint Lenth is required 3 character According to the TrackingPoint Configuration into TrackingPointTable.
            'DBCSequenceNo = strData.Substring(29, 3)
            'DBodyNo = strData.Substring(32, 5)
            'DModelCode = strData.Substring(37, 15)
            'DLotCode = strData.Substring(52, 2)
            'EngineModelcode = strData.Substring(54, 3)
            'EngineNumber = strData.Substring(57, 7)
            'WMI = strData.Substring(64, 3)
            'VDS = strData.Substring(67, 6)
            'DExteriorColor = strData.Substring(73, 4)
            'DLoDate = strData.Substring(77, 8)
            'FrameSeqno = strData.Substring(85, 7)
            'DProdSuffix = strData.Substring(92, 2)
            'Carfamily = strData.Substring(94, 4)
            'MMYY = strData.Substring(98, 4)
            'ChkDigit = strData.Substring(102, 1)
            'msgLen = Len(strData)
            'DTrackingPoint = DTrackingPoint.Trim
            'DPlantCode = "B1"
            'Dim lDate As DateTime = DateTime.Parse(DLoDate, CultureInfo.InvariantCulture)
            'DLoTime = strData.Substring(81, 4)
            'DIdentNo = strData.Substring(39, 10)          
            'TappingId = strData.Substring(85, 2)
            'DNorm = strData.Substring(87, 8).Replace(".", "")
            'DGrade = strData.Substring(95, 3).Replace(".", "")


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

        'Code Modified by: Mahesh Avanti on Date: 21-10-2019
        If DPlantCode = "B1" Then


            'Query = "Select Length from dbo.SocketMaster where MasterId=(Select MasterId From Socketdetail where TrackingPoint='" & DTrackingPoint & "' and line=" & DLine & " and PlantCode='" & DPlantCode & "')"

            Try
                If DTrackingPoint = "N0" Then
                    Query = "Select Length from dbo.tblSocketSetting where Id=1"
                ElseIf DTrackingPoint = "R0" Then
                    Query = "Select Length from dbo.tblSocketSetting where Id=2"
                End If

            Catch ex As Exception
                WriteToLog("ERROR While retriving AppLength " & ex.Message & " " & ex.StackTrace)
            End Try

            Try
                AppLength = SQLDM.GetValue(Query)

            Catch ex As Exception
                WriteToLog("ERROR While retriving AppLength " & ex.Message & " " & ex.StackTrace)

            End Try


            Try
                'LPort = SQLDM.GetValue("Select Portnumber from SocketMaster where MasterId=(Select MasterId from dbo.SocketDetail  where TrackingPoint='" & DTrackingPoint & "' and line=" & DLine & ")")
                If DTrackingPoint = "N0" Then
                    LPort = SQLDM.GetValue("Select Port from tblSocketSetting where Id=1")
                ElseIf DTrackingPoint = "R0" Then
                    LPort = SQLDM.GetValue("Select Port from tblSocketSetting where Id=2")
                End If

            Catch ex As Exception
                WriteToLog("ERROR While retriving the port number from database..- " & ex.Message)
            End Try


            'Query = "Select BcSequence from dbo.SocketDetail  where TrackingPoint='" & DTrackingPoint & "' and line=" & DLine & " and MasterId=(Select MasterId from dbo.SocketMaster  where PortNumber='" & LPort & "' and PlantCode='" & DPlantCode & "')"
            If DTrackingPoint = "N0" Then
                Query = "Select BcSequence from dbo.tblSocketSetting  where Id=1"
            ElseIf DTrackingPoint = "R0" Then
                Query = "Select BcSequence from dbo.tblSocketSetting  where Id=2"
            End If

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
                If msgLen <> AppLength And (DDataProcessType = "00" Or DDataProcessType = "01" Or DDataProcessType = "10" Or (DDataProcessType = "0E" And DTrackingPoint <> "1H0")) Then
                    DProcessResult = "91"

                ElseIf DLength <> (msgLen - 30) And (DDataProcessType = "00" Or DDataProcessType = "01" Or DDataProcessType = "10" Or (DDataProcessType = "0E" And DTrackingPoint <> "1H0")) Then

                    DProcessResult = "76"

                ElseIf DProcessResult <> "  " Then

                    DProcessResult = "13"

                ElseIf (DBBCSEQ <> (DBCSequenceNo - 1)) And (DDataProcessType = "00" Or DDataProcessType = "01" Or (DTrackingPoint = "R0" And DDataProcessType = "10") Or (DDataProcessType = "0E" And DTrackingPoint = "1H0")) Then
                    If DBCSequenceNo = 0 Then
                        DProcessResult = "00"
                    Else
                        DProcessResult = "90"
                        WriteFormattedData("BcSequenceError : Expected Seq: " & DBBCSEQ & ". Updating it to " & DBCSequenceNo, "", PlantCode)
                        If DTrackingPoint = "N0" Then
                            SQLDM.ExecuteQuery("UPDATE [tblSocketSetting]  SET [BcSequence] = " & DBCSequenceNo - 1 & " WHERE Id=1")
                        ElseIf DTrackingPoint = "R0" Then
                            SQLDM.ExecuteQuery("UPDATE [tblSocketSetting]  SET [BcSequence] = " & DBCSequenceNo - 1 & " WHERE Id=2")
                        End If
                        DBBCSEQ = DBCSequenceNo - 1
                        DProcessResult = "00"
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
                        If DPlantCode = "B1" Then
                            'If DTrackingPoint = "1H6" Then
                            '    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            '    '''''''''''''''''''''''''''       1H6     '''''''''''''''''''''''''''''''''''
                            '    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            '    Dim BCount As Integer

                            '    Query = "SELECT count(JCBDNO) FROM   F91JCF where JCBDNO='" + DBodyNo + "'"
                            '    BCount = SQLDM.GetValue(Query)

                            '    If BCount >= 1 Then
                            '        'Move data from F91TAF to BAKTAF if the data exists:'Written by Mahesh Avanti on 23-Oct-2019
                            '        'SQLQ = "Insert Into Baktaf select TABDNO,TAFRSQ, TAEGNO,TAKEYN,TATMCD,TAPRDM,TAEGMD,TAPLOD from f91taf where TABDNO='" + BodyNo + "'"
                            '        Query = "Insert Into Baktaf(TABDNO,TAFRSQ, TAEGNO,TAKEYN,TATMCD,TAPRDM,TAEGMD,TAPLOD,TAWMI,TAVDS) select TABDNO,TAFRSQ, TAEGNO,TAKEYN,TATMCD,TAPRDM,TAEGMD,TAPLOD,TAWMI,TAVDS from f91taf where TABDNO='" + DBodyNo + "'"
                            '        SQLDM.ExecuteQuery(Query)
                            '        WriteToLog("Moved Data from F91TAF to BAKTAF - " & DBodyNo)

                            '        'Delete the data from F91TAF
                            '        Query = "Delete from F91TAF WHERE (UPPER(TABDNO)='" + DBodyNo + "')"
                            '        SQLDM.ExecuteQuery(Query)
                            '        WriteToLog("Deleted Data from F91TAF - " & DBodyNo)

                            '        'Delete data From BcDetails
                            '        Query = "Delete from BcDetails WHERE (UPPER(BodyNumber)='" + DBodyNo + "')"
                            '        SQLDM.ExecuteQuery(Query)
                            '        WriteToLog("Deleted Data from BcDetails - " & DBodyNo)

                            '        'Delete the Data in the F91JCF table for the Bodynumber
                            '        Query = "Delete from F91JCF where JCBDNO='" + DBodyNo + "'"
                            '        SQLDM.ExecuteQuery(Query)
                            '        WriteToLog("Deleted Data from F91JCF - " & DBodyNo)

                            '    End If

                            '    'Insert the New data into the F91JCF Table
                            '    'GoTo Err
                            '    Dim val() As String
                            '    val = Split(DModelCode, "-")
                            '    Dim _currentDateTime As DateTime = Now()
                            '    'Dim _currentDateTime As DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            '    'Dim _date As Date = Date.ParseExact(DLoDate, "yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                            '    ' "CONVERT(DATETIME, '" & DLoDate & "')"
                            '    'Testing
                            '    Query = "INSERT INTO F91JCF ( JCBDNO,JCKATA,JCLTCD,JCEGMD,JCEGNO,JCGWMI,JCGVDS,JCEXTC,JCPLOD,JCCTRP,JCSTKT,JCFSEQ,FRSQIN,IsNew,CreatedDate,SuffixCode, TAPRDM,ChkDigitFrame) VALUES " & "('" & DBodyNo & "','" & DModelCode & "','" & DLotCode & "','" & EngineModelcode & "','" & EngineNumber & "','" & WMI & "','" & VDS & "','" & DExteriorColor & "','" & DLoDate & "','" & DTrackingPoint.Substring(1, 2) & "','" & val(0) & "'," & DBCSequenceNo & ", '" & FrameSeqno & "'," & "'True'" & "," & "convert(datetime, getdate(), 21)" & ",'" & DProdSuffix & "','" & MMYY & "','" & ChkDigit & "')"
                            '    'Production

                            '    'Query = "INSERT INTO F91JCF ( JCBDNO,JCKATA,JCLTCD,JCEGMD,JCEGNO,JCGWMI,JCGVDS,JCEXTC,JCPLOD,JCCTRP,JCSTKT,JCFSEQ,FRSQIN,IsNew,CreatedDate,SuffixCode, TAPRDM,ChkDigitFrame)" VALUES " & "('" & DBodyNo & "','" & DModelCode & "','" & DLotCode & "','" & EngineModelcode & "','" & EngineNumber & "','" & WMI & "','" & VDS & "','" & DExteriorColor & "','" & DLoDate & "','" & DTrackingPoint & "','" & val(0) & "'," & DBCSequenceNo & ", '" & FrameSeqno & "','True'," & _currentDateTime & ",'" & DProdSuffix & "','" & MMYY & "','" & ChkDigit & "')"
                            '    SQLDM.ExecuteQuery(Query)
                            '    WriteToLog("Data has been Saved successfully in F91JCF Table and 1H6 TP data is: - " & DBodyNo & "','" & DModelCode & "','" & DLotCode & "','" & EngineModelcode & "','" & EngineNumber & "','" & WMI & "','" & VDS & "','" & DExteriorColor & "','" & DLoDate & "','" & DTrackingPoint & "','" & val(0) & "'," & DBCSequenceNo & ", '" & FrameSeqno & "','True','" & DLoDate & "','" & DProdSuffix & "','" & MMYY & "','" & ChkDigit)
                            '    'Err:
                            '    If Err.Description <> "" Then
                            '        WriteToLog(Err.Description & "- " & Query)
                            '    End If

                            'ElseIf DTrackingPoint = "1N5" Then
                            '    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            '    '''''''''''''''''''''''''''       1N5     '''''''''''''''''''''''''''''''''''
                            '    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                            '    'Insert the data into the table VinPrint for the bodynumber if it is not Else Update the same
                            '    'NOTE:
                            '    'Only Bodynumber is being inserted
                            '    'Rest ISNEW and Created Date Has been Added to the Table as the Default value
                            '    'If new Data Get Inserted Always Created Date Will be GetDate() functions date and
                            '    'IsNew Will be TRUE
                            '    'If you want to check this Please go to Table and Click Modify
                            '    'Select Column and Check 'Default Value or Binding' property of the Column

                            '    If UCase(Carfamily) <> "514W" Then

                            '        Query = "IF EXISTS (Select Bodynumber from Vinprint where Bodynumber='" & DBodyNo & "') " _
                            '        & " Update Vinprint Set CreatedDate=GetDate(), PrintedDate=Null,ReprintedDate=null,IsNew=1,ReprintCount=0 where Bodynumber='" & DBodyNo & "' " _
                            '        & " Else " _
                            '        & " Insert into vinprint( bodynumber,Suffix,ModelCode,Color ) values('" & DBodyNo & "','" & DProdSuffix & "','" & DModelCode & "','" & DExteriorColor & "')"
                            '        SQLDM.ExecuteQuery(Query)
                            '        WriteToLog("Data has been Saved/Modfied successfully in 1N5 TP and Table is vinprint data - " & DBodyNo & "','" & DProdSuffix & "','" & DModelCode & "','" & DExteriorColor)
                            '    End If
                            '    'End If

                            '    'Added to update the tacking point details in the F91JCF table for respective vehicle - By Sathish BL
                            'ElseIf DTrackingPoint = "1H5" Then
                            '    Query = "update F91JCF set JCCTRP = '" & DTrackingPoint.Substring(1, 2) & "', JCFSEQ='" & DBCSequenceNo & "'  WHERE (UPPER(JCBDNO)='" + DBodyNo + "')"
                            '    SQLDM.ExecuteQuery(Query)
                            '    WriteToLog("Update the 1H5 tracking point Data in F91JCF - " & DBodyNo)
                            'End If




                            'Insert Data to DB

                            'Dim MyParams() As SqlParameter
                            ''=>Code Modifed by: Mahesh Avanti on Date:31-08-2019
                            'ReDim Preserve MyParams(10)

                            'MyParams(0) = New SqlParameter("@Param_BodyNo", DBodyNo.Trim())
                            'MyParams(1) = New SqlParameter("@Param_ModelCode", DModelCode.Trim())
                            'MyParams(2) = New SqlParameter("@Param_Suffix", DProdSuffix)
                            'MyParams(3) = New SqlParameter("@Param_ColorCode", DExteriorColor)
                            'MyParams(4) = New SqlParameter("@Param_TPID", DTrackingPoint.Trim())
                            'MyParams(5) = New SqlParameter("@Param_PlantCode", DPlantCode.Trim())
                            'MyParams(6) = New SqlParameter("@Param_IdentNumber", DIdentNo.Trim())
                            'MyParams(7) = New SqlParameter("@Param_LineOffDate", DLoDate)
                            'MyParams(8) = New SqlParameter("@Param_LineOffTime", DLoTime)
                            'MyParams(9) = New SqlParameter("@Param_LotCode", DLotCode)
                            'MyParams(10) = New SqlParameter("@PARAM_BCSEQUENCENO", DBCSequenceNo)

                            'SQLDM.ExecuteStoredProcedure("VINInsertIntoVehicleInfo", MyParams) '=>Code Modifed by: Mahesh Avanti on Date:31-08-2019

                            If DTrackingPoint = "N0" Then

                                Query = "IF NOT EXISTS(SELECT 1 FROM TB_R_VEHICLE WHERE VIN = '" & VIN_NO & "')
                                        BEGIN
                                        INSERT INTO TB_R_VEHICLE(
                                        BODY_NO,PLANT_CD,VEHICLE_ID,SUFFIX_CD,VIN,MODEL_CD,COLOR_CD,ASSEMBLY_SEQ_NO,KEY_NO,BUY_OFF_ETD_with_TIME,URN,KAKU_STATUS,DOK_STATUS,
										ENG_NO,ENG_PREFIX,EXP_OR_DOM,FUEL_TYPE,DESTINATION,CREATED_BY,CREATED_DATE,ASSY_LO_DATE,ASSY_LO_TIME,ETD_DELAY,CNG_TANK_NUMBER,VINMS
                                        ) 
                                        VALUES('" & BODY_NO & "','" & PLANT_CODE & "','" & ID_NO & "','" & SUFFIX & "','" & VIN_NO & "','" & MODEL_CODE & "','" & COLOR & "'," & ASSY_LINE_OFF_SEQ_NO & ",'" & KEY_NO & "','" & BUY_OFF_ETD_with_TIME & "','',0,0,
										'" & EG_NO & "','" & EG_PREFIX & "','','','','N0_GALC',GETDATE(),'" & ASSY_LO_DATE & "','" & ASSY_LO_TIME & "','" & ETD_DELAY & "','" & CNG_TANK_NUMBER & "','" & VIN_NO + "-" + VINMS & "')
                                        END
                                        ELSE
                                        BEGIN
                                        UPDATE TB_R_VEHICLE SET BODY_NO = '" & BODY_NO & "',VEHICLE_ID = '" & ID_NO & "',SUFFIX_CD = '" & SUFFIX & "',MODEL_CD = '" & MODEL_CODE & "', 
										COLOR_CD = '" & COLOR & "',ASSY_LO_DATE = '" & ASSY_LO_DATE & "',ASSY_LO_TIME = '" & ASSY_LO_TIME & "',ETD_DELAY = '" & ETD_DELAY & "',CNG_TANK_NUMBER = '" & CNG_TANK_NUMBER & "',
										KEY_NO = '" & KEY_NO & "', ENG_NO = '" & EG_NO & "', ENG_PREFIX = '" & EG_PREFIX & "', VINMS ='" & VIN_NO + "-" + VINMS & "' WHERE VIN = '" & VIN_NO & "'
                                        END"
                                SQLDM.ExecuteQuery(Query)
                            ElseIf DTrackingPoint = "R0" Then
                                Dim SQLList As List(Of String)
                                SQLList = New List(Of String)

                                Query = "INSERT INTO TB_R0_VEHICLE 
                                        SELECT PK_ID,MODEL_CD,SUFFIX_CD,COLOR_CD,BODY_NO,VEHICLE_ID,VIN,EXP_OR_DOM,B_O_ETD,FUEL_TYPE,DESTINATION,URN,PLANT_CD,ASSEMBLY_SEQ_NO,KEY_NO,
                                        CREATED_BY,KAKU_STATUS,DOK_STATUS,CREATED_DATE,PAINT,ADM,TOE,S_T,DRUM,BRAKE,OPT1,OPT2,ENG_NO,ENG_PREFIX,ASSY_LO_DATE,ASSY_LO_TIME,
                                        ETD_DELAY,CNG_TANK_NUMBER,BUY_OFF_ETD_with_TIME,UPDATE_BY,UPDATE_ON,GETDATE(),'R0_GALC',MANUAL_CD,VEHICLE_INFO_CODE,EXPORT_STATUS,EXPORT_UPDATE,
                                        PK_ID_LOCATION,LOCATION_SCANNED_TIME,BAY_NO,VINMS FROM TB_R_VEHICLE WHERE VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "INSERT INTO TB_R0_VEH_PR_SCAN_HIST 
                                        SELECT SH.* FROM TB_R_VEH_PR_SCAN_HIST SH JOIN TB_R_VEHICLE RV ON RV.PK_ID = SH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "INSERT INTO TB_R0_VEH_ENTRY_COUNT 
                                        SELECT VEC.* FROM TB_R_VEH_ENTRY_COUNT VEC JOIN TB_R_VEHICLE RV ON RV.PK_ID = VEC.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "INSERT INTO TB_R0_VEH_DEFECT_H 
                                        SELECT VDH.* FROM TB_R_VEH_DEFECT_H VDH JOIN TB_R_VEHICLE RV ON RV.PK_ID = VDH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "INSERT INTO TB_R0_VEH_DEFECT_D  
                                        SELECT VDD.* FROM TB_R_VEH_DEFECT_D VDD JOIN TB_R_VEH_DEFECT_H VDH ON VDH.PK_ID = VDD.PK_ID_VEH_DEFECT 
                                        JOIN TB_R_VEHICLE RV ON RV.PK_ID = VDH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "DELETE VDD FROM TB_R_VEH_DEFECT_D VDD JOIN TB_R_VEH_DEFECT_H VDH ON VDH.PK_ID = VDD.PK_ID_VEH_DEFECT JOIN TB_R_VEHICLE RV ON RV.PK_ID = VDH.PK_ID_VEHICLE 
                                        WHERE RV.VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "DELETE VDH FROM TB_R_VEH_DEFECT_H VDH JOIN TB_R_VEHICLE RV ON RV.PK_ID = VDH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "DELETE VEC FROM TB_R_VEH_ENTRY_COUNT VEC JOIN TB_R_VEHICLE RV ON RV.PK_ID = VEC.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "DELETE SH FROM TB_R_VEH_PR_SCAN_HIST SH JOIN TB_R_VEHICLE RV ON RV.PK_ID = SH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                Query = "DELETE FROM TB_R_VEHICLE WHERE VIN = '" & VIN_NO & "';"
                                SQLList.Add(Query)

                                'Query = "
                                '        INSERT INTO TB_R0_VEHICLE 
                                '        SELECT PK_ID,MODEL_CD,SUFFIX_CD,COLOR_CD,BODY_NO,VEHICLE_ID,VIN,EXP_OR_DOM,B_O_ETD,FUEL_TYPE,DESTINATION,URN,PLANT_CD,ASSEMBLY_SEQ_NO,KEY_NO,
                                '        CREATED_BY,KAKU_STATUS,DOK_STATUS,CREATED_DATE,PAINT,ADM,TOE,S_T,DRUM,BRAKE,OPT1,OPT2,ENG_NO,ENG_PREFIX,ASSY_LO_DATE,ASSY_LO_TIME,
                                '        ETD_DELAY,CNG_TANK_NUMBER,BUY_OFF_ETD_with_TIME,UPDATE_BY,UPDATE_ON,GETDATE(),'R0_GALC' FROM TB_R_VEHICLE WHERE VIN = '" & VIN_NO & "';

                                '        INSERT INTO TB_R0_VEH_PR_SCAN_HIST 
                                '        SELECT SH.* FROM TB_R_VEH_PR_SCAN_HIST SH JOIN TB_R_VEHICLE RV ON RV.PK_ID = SH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';

                                '        INSERT INTO TB_R0_VEH_ENTRY_COUNT 
                                '        SELECT VEC.* FROM TB_R_VEH_ENTRY_COUNT VEC JOIN TB_R_VEHICLE RV ON RV.PK_ID = VEC.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';

                                '        INSERT INTO TB_R0_VEH_DEFECT_H 
                                '        SELECT VDH.* FROM TB_R_VEH_DEFECT_H VDH JOIN TB_R_VEHICLE RV ON RV.PK_ID = VDH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';

                                '        INSERT INTO TB_R0_VEH_DEFECT_D  
                                '        SELECT VDD.* FROM TB_R_VEH_DEFECT_D VDD JOIN TB_R_VEH_DEFECT_H VDH ON VDH.PK_ID = VDD.PK_ID_VEH_DEFECT 
                                '        JOIN TB_R_VEHICLE RV ON RV.PK_ID = VDH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';

                                '        DELETE VDD FROM TB_R_VEH_DEFECT_D VDD JOIN TB_R_VEH_DEFECT_H VDH ON VDH.PK_ID = VDD.PK_ID_VEH_DEFECT JOIN TB_R_VEHICLE RV ON RV.PK_ID = VDH.PK_ID_VEHICLE 
                                '        WHERE RV.VIN = '" & VIN_NO & "';

                                '        DELETE VDH FROM TB_R_VEH_DEFECT_H VDH JOIN TB_R_VEHICLE RV ON RV.PK_ID = VDH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';

                                '        DELETE VEC FROM TB_R_VEH_ENTRY_COUNT VEC JOIN TB_R_VEHICLE RV ON RV.PK_ID = VEC.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';

                                '        DELETE SH FROM TB_R_VEH_PR_SCAN_HIST SH JOIN TB_R_VEHICLE RV ON RV.PK_ID = SH.PK_ID_VEHICLE WHERE RV.VIN = '" & VIN_NO & "';

                                '        DELETE FROM TB_R_VEHICLE WHERE VIN = '" & VIN_NO & "';"
                                SQLDM.ExecuteQuery(SQLList)
                            End If
                        End If
                    Catch ex As Exception
                        WriteToLog("ERROR While inserting the data  - " & ex.Message & "  " & vbNewLine & " STORED PROCEDURE ERROR FOR DATA - " & strData & " " & vbNewLine)
                        WriteToLog("Input Data for Insert/Update Body Details For VINMS-Plant#1 are PlantCode =" & DPlantCode.Trim() & " , PlantID = " & PlantID & " , BodyNo = " & DBodyNo.Trim() & " BCSequenceNo = " & DBCSequenceNo & " , VehicleInfoID = " & VehicleInfoID)
                    End Try
                End If

                'ProcessType matches then increment the BcSequence number
                If (DDataProcessType = "01" Or DDataProcessType = "00" Or (DTrackingPoint = "R0" And DDataProcessType = "10") Or DDataProcessType = "0E") And DProcessResult = "00" Then

                    Dim NewBc As Integer = DBCSequenceNo

                    If NewBc = "999" Then
                        NewBc = -1
                    Else
                        NewBc = DBBCSEQ + 1
                    End If

                    Try
                        'SQLDM.ExecuteQuery("UPDATE [SocketDetail]  SET [BcSequence] = " & NewBc & " WHERE [TrackingPoint] ='" & DTrackingPoint & "' and MasterId=(Select MasterId from dbo.SocketMaster  where PortNumber='" & LPort & "' and PlantCode='" & DPlantCode & "')")
                        If DTrackingPoint = "N0" Then
                            SQLDM.ExecuteQuery("UPDATE [tblSocketSetting]  SET [BcSequence] = " & NewBc & " WHERE Id=1")
                        ElseIf DTrackingPoint = "R0" Then
                            SQLDM.ExecuteQuery("UPDATE [tblSocketSetting]  SET [BcSequence] = " & NewBc & " WHERE Id=2")
                        End If

                    Catch ex As Exception
                        WriteToLog("ERROR While Updatig BC Sequence number  - " & ex.Message & "  " & vbNewLine & Query)
                    End Try

                End If
            End If


            'This Else Condition is specially for B2 Plant Code and which has TP 5U3
        ElseIf DPlantCode = "B1" And DTrackingPoint = "5U3" Then


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

        'Try
        '    If (DProcessResult <> "00" And EmailBodyNo <> DBodyNo And EmailSequence <> DBCSequenceNo And EmailSerialNo <> DSerialNo) Or (DProcessResult <> "00" And EmailBodyNo = Nothing And EmailSequence = Nothing And EmailSerialNo = Nothing) Then

        '        EmailBodyNo = Nothing
        '        EmailSequence = Nothing
        '        EmailSerialNo = Nothing

        '        EM.SendMail(DProcessResult & "</br></br>Last BC Sequence No :" & DBBCSEQ & "<br/> Current BC Sequence No:" & DBCSequenceNo & "<br/></br> DATA = " & strData, DBodyNo)

        '        'Below Sequence is received from GALC not from the Application Database
        '        EmailBodyNo = DBodyNo
        '        EmailSequence = DBCSequenceNo
        '        EmailSerialNo = DSerialNo

        '    End If

        'Catch ex As Exception
        '    WriteToLog("ERROR While sending BC Error mail  - " & ex.Message & "  " & vbNewLine)
        'End Try

        'After Sending the Response . Resetting the Variable data for the next use
        Data = ""

        Try
            WriteFormattedData("Sent :", strResponse, PlantCode)
        Catch ex As Exception

            WriteToLog("ERROR While Writing to FormattedData file - " & ex.Message)
        End Try

    End Sub

#End Region

#Region "==>Data Log and Error Log"

    Private Sub WriteFormattedData(ByVal headerText As String, ByVal formattedData As String, ByVal DPlantcode As String)
        ' Writing the formatted data in text file

        Dim Request As String

        If headerText = "Received :" Then
            Request = AppLength
        Else
            Request = 26
        End If

        Request = " Request :" & Request & "  Response :" & Len(formattedData)

        Dim Value As String = "VINMS_" & DPlantcode & "_" & Now.Day & "-" & Now.Month & "-" & Now.Year

        DataLogFileName = DataLogPath & "DataLog_" & Value & ".txt"

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
