﻿Imports System.Net.Mail
Imports System.Configuration
Imports System.Windows.Forms

Public Class SendEMail
    Dim Toaddress As New List(Of String)()

    'Code change for Lotus Note or OutLook mail sending
    'Writen By : Nilamani Bhanja
    'Date : 09 - Dec -2015
    Public Sub SendMail(ByVal StrMssage As String, ByVal Bnumber As String)
        Dim Mailserverengine As String = System.Configuration.ConfigurationManager.AppSettings("MailServerEngine").ToString()
        ' SendeMail(StrMsg, Bodynumber)
        If Mailserverengine.Trim().ToUpper() = "LOTUSNOTES" Then
            SendeMail(StrMssage, Bnumber)
        Else
            SendMailOutlook(StrMssage, Bnumber)
        End If

    End Sub


#Region "Sending Mail"
    Public Sub SendeMail(ByVal StrMsg As String, ByVal Bodynumber As String)
        Toaddress.Clear()
        Toaddress.Add(System.Configuration.ConfigurationManager.AppSettings("ToEmails").ToString())


        Dim strFromMailID As String = [String].Empty
        Dim strSmtp As String = [String].Empty
        Dim strbody As String = String.Empty
        Dim port As Integer = 0
        Dim MyMessage As New MailMessage()
        strFromMailID = System.Configuration.ConfigurationManager.AppSettings("FromEmail").ToString()
        strSmtp = System.Configuration.ConfigurationManager.AppSettings("SMTP").ToString()
        port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings("PortNO").ToString())

        Dim SendFrom As New MailAddress(strFromMailID)
        MyMessage.From = SendFrom
        MyMessage.[To].Clear()
        MyMessage.CC.Clear()
        MyMessage.Bcc.Clear()
        MyMessage.Body = ""
        MyMessage.Subject = System.Configuration.ConfigurationManager.AppSettings("Subject").ToString() & Bodynumber & " SERVER :" & My.Computer.Name

        strbody = "<table border=0 cellspacing=0 cellpadding=0> " & _
         "<tr><td>Dear All," & _
         "</td></tr>" & _
       "<tr><td><br/> BC Data Error Occurred.!! Error Code :" & StrMsg & "<br/><br/>" & _
       "********* DO NOT REPLY TO THIS MAIL. THIS IS AUTOGENERATED E-MAIL. *********</td></tr>"

        For Each ToAddr As String In Toaddress
            MyMessage.[To].Add(ToAddr)
        Next

        MyMessage.Body = strbody
        MyMessage.IsBodyHtml = True

        Dim smtp As New SmtpClient()
        smtp.UseDefaultCredentials = True
        smtp.Host = strSmtp
        smtp.Port = port
        smtp.Send(MyMessage)
    End Sub
#End Region


#Region "Outlook Configuration mail"
    'Code change for Lotus Note or OutLook mail sending
    'Writen By : Nilamani Bhanja
    'Date : 09 - Dec -2015

    Public Function SendMailOutlook(ByVal StrMsg As String, ByVal Bodynumber As String)
        'SendMail(string strMailServer, string strUserName, string strPassword, string strMailFrom, string strMailTo, string strMailCC, string strSubject, string strBody, System.Net.Mail.MailPriority MailPriority)
        Dim IsErrorRaised As Boolean = False

        Dim mailAttach As System.Net.Mail.Attachment = Nothing
        Dim strMailServer As String = System.Configuration.ConfigurationManager.AppSettings("HostOutlook").ToString()
        Dim strUserName As String = System.Configuration.ConfigurationManager.AppSettings("EmailOutlook").ToString()
        Dim strPassword As String = System.Configuration.ConfigurationManager.AppSettings("PasswordOutlook").ToString()
        Dim strMailFrom As String = System.Configuration.ConfigurationManager.AppSettings("EmailOutlook").ToString()
        Dim strMailTo As String = System.Configuration.ConfigurationManager.AppSettings("ToEmails").ToString()
        'Dim strMailCC As String = System.Configuration.ConfigurationManager.AppSettings("ToEmails").ToString()
        Dim strSubject As String = System.Configuration.ConfigurationManager.AppSettings("Subject").ToString() & Bodynumber & " SERVER :" & My.Computer.Name
        Dim strBody As String = ""
        'strBody = sb.ToString()
        Dim MailPriority As System.Net.Mail.MailPriority = MailPriority.High
        Try
            Dim mailMessage As New System.Net.Mail.MailMessage()

            Dim smtpClient As New System.Net.Mail.SmtpClient(strMailServer)
            Dim networkCredential As New System.Net.NetworkCredential(strUserName, strPassword)

            mailMessage.From = New System.Net.Mail.MailAddress(strMailFrom)

            Dim delimiters As Char() = New Char() {";"c, ","c}

            Dim arrMailTo As String() = strMailTo.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
            For Each MailTo As String In arrMailTo
                If MailTo.Trim() <> "" Then
                    mailMessage.[To].Add(MailTo)
                End If
            Next

            'Dim arrMailCC As String() = strMailCC.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
            'For Each MailCC As String In arrMailCC
            '    If MailCC.Trim() <> "" Then
            '        mailMessage.CC.Add(MailCC)
            '    End If
            'Next

            mailMessage.Subject = strSubject
            mailMessage.Body = "<table border=0 cellspacing=0 cellpadding=0> " & _
         "<tr><td>Dear All," & _
         "</td></tr>" & _
       "<tr><td><br/> BC Data Error Occurred.!! Error Code :" & StrMsg & "<br/><br/>" & _
       "********* DO NOT REPLY TO THIS MAIL. THIS IS AUTOGENERATED E-MAIL. *********</td></tr>"

            'strBody;
            mailMessage.Priority = MailPriority
            '  mailMessage.Attachments.Add(New Attachment(smbo.EmailAttachment))
            mailMessage.BodyEncoding = System.Text.Encoding.[Default]
            mailMessage.IsBodyHtml = True
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = networkCredential
            smtpClient.Send(mailMessage)
        Catch ex As Exception
            'errMsg = "SmtpMail Send Failure : " + ex.Message.ToString();
            IsErrorRaised = True
        Finally
            If mailAttach IsNot Nothing Then
                mailAttach.Dispose()
            End If
        End Try
        Return (If(IsErrorRaised, 1, 0))
    End Function
#End Region


    Public Sub SendStatusMail(ByVal StrMssage As String, ByVal Bnumber As String)
        Dim Mailserverengine As String = System.Configuration.ConfigurationManager.AppSettings("MailServerEngine").ToString()
        ' SendeMail(StrMsg, Bodynumber)
        If Mailserverengine.Trim().ToUpper() = "LOTUSNOTES" Then
            SendStatusMailLotusNote(StrMssage, Bnumber)
        Else
            SendStatusMailOutLook(StrMssage, Bnumber)
        End If

    End Sub


#Region "Outlook Configuration mail for Status "
    'Code change for Lotus Note or OutLook mail sending
    'Writen By : Nilamani Bhanja
    'Date : 09 - Dec -2015

    Public Function SendStatusMailOutLook(ByVal StrMsg As String, ByVal Bodynumber As String)
        'SendMail(string strMailServer, string strUserName, string strPassword, string strMailFrom, string strMailTo, string strMailCC, string strSubject, string strBody, System.Net.Mail.MailPriority MailPriority)
        Dim IsErrorRaised As Boolean = False

        Dim mailAttach As System.Net.Mail.Attachment = Nothing
        Dim strMailServer As String = System.Configuration.ConfigurationManager.AppSettings("HostOutlook").ToString()
        Dim strUserName As String = System.Configuration.ConfigurationManager.AppSettings("EmailOutlook").ToString()
        Dim strPassword As String = System.Configuration.ConfigurationManager.AppSettings("PasswordOutlook").ToString()
        Dim strMailFrom As String = System.Configuration.ConfigurationManager.AppSettings("EmailOutlook").ToString()
        Dim strMailTo As String = System.Configuration.ConfigurationManager.AppSettings("StatusEmails").ToString()
        'Dim strMailCC As String = System.Configuration.ConfigurationManager.AppSettings("ToEmails").ToString()
        Dim strSubject As String = System.Configuration.ConfigurationManager.AppSettings("Subject").ToString() & Bodynumber & " SERVER :" & My.Computer.Name
        Dim strBody As String = ""
        'strBody = sb.ToString()
        Dim MailPriority As System.Net.Mail.MailPriority = MailPriority.High
        Try
            Dim mailMessage As New System.Net.Mail.MailMessage()

            Dim smtpClient As New System.Net.Mail.SmtpClient(strMailServer)
            Dim networkCredential As New System.Net.NetworkCredential(strUserName, strPassword)

            mailMessage.From = New System.Net.Mail.MailAddress(strMailFrom)

            Dim delimiters As Char() = New Char() {";"c, ","c}

            Dim arrMailTo As String() = strMailTo.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
            For Each MailTo As String In arrMailTo
                If MailTo.Trim() <> "" Then
                    mailMessage.[To].Add(MailTo)
                End If
            Next

            'Dim arrMailCC As String() = strMailCC.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
            'For Each MailCC As String In arrMailCC
            '    If MailCC.Trim() <> "" Then
            '        mailMessage.CC.Add(MailCC)
            '    End If
            'Next
            Dim St As String = System.Configuration.ConfigurationManager.AppSettings("StatusMail").ToString() & StrMsg

            mailMessage.Subject = strSubject
            mailMessage.Body = "<table border=0 cellspacing=0 cellpadding=0> " & _
         "<tr><td>Dear All," & _
         "</td></tr>" & _
       "<tr><td><br/>" & St & " " & Date.Now & "<br/><br/>" & _
        "******** DO NOT REPLY TO THIS MAIL. THIS IS AUTOGENERATED E-MAIL. ********</td></tr>"

            'strBody;
            mailMessage.Priority = MailPriority
            '  mailMessage.Attachments.Add(New Attachment(smbo.EmailAttachment))
            mailMessage.BodyEncoding = System.Text.Encoding.[Default]
            mailMessage.IsBodyHtml = True
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = networkCredential
            smtpClient.Send(mailMessage)
        Catch ex As Exception
            'errMsg = "SmtpMail Send Failure : " + ex.Message.ToString();
            IsErrorRaised = True
        Finally
            If mailAttach IsNot Nothing Then
                mailAttach.Dispose()
            End If
        End Try
        Return (If(IsErrorRaised, 1, 0))
    End Function
#End Region


#Region "Orginal Send Status Mail" 'Date : 09 - Dec - 2015 

    Public Sub SendStatusMailLotusNote(ByVal StrMsg As String, ByVal Bodynumber As String)

        Toaddress.Clear()
        Toaddress.Add(System.Configuration.ConfigurationManager.AppSettings("StatusEmails").ToString())


        Dim strFromMailID As String = [String].Empty
        Dim strSmtp As String = [String].Empty
        Dim strbody As String = String.Empty
        Dim port As Integer = 0
        Dim MyMessage As New MailMessage()
        strFromMailID = System.Configuration.ConfigurationManager.AppSettings("FromEmail").ToString()
        strSmtp = System.Configuration.ConfigurationManager.AppSettings("SMTP").ToString()
        port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings("PortNO").ToString())

        Dim SendFrom As New MailAddress(strFromMailID)
        MyMessage.From = SendFrom
        MyMessage.[To].Clear()
        MyMessage.CC.Clear()
        MyMessage.Bcc.Clear()
        MyMessage.Body = ""
        MyMessage.Subject = System.Configuration.ConfigurationManager.AppSettings("Subject").ToString() & Bodynumber & " SERVER :" & My.Computer.Name

        Dim St As String = System.Configuration.ConfigurationManager.AppSettings("StatusMail").ToString() & StrMsg

        For Each ToAddr As String In Toaddress
            MyMessage.[To].Add(ToAddr)
        Next

        strbody = "<table border=0 cellspacing=0 cellpadding=0> " & _
         "<tr><td>Dear All," & _
         "</td></tr>" & _
       "<tr><td><br/>" & St & " " & Date.Now & "<br/><br/>" & _
        "********* DO NOT REPLY TO THIS MAIL. THIS IS AUTOGENERATED E-MAIL. *********</td></tr>"


        MyMessage.Body = strbody
        MyMessage.IsBodyHtml = True

        Dim smtp As New SmtpClient()
        smtp.UseDefaultCredentials = True
        smtp.Host = strSmtp
        smtp.Port = port
        smtp.Send(MyMessage)


    End Sub
#End Region


End Class



























#Region "OLD Class" 'Mail Functionality
'Imports System.Net.Mail
'Imports System.Configuration
'Imports System.Windows.Forms

'Public Class SendEMail
'    Dim Toaddress As New List(Of String)()


'    Public Sub SendMail(ByVal StrMsg As String, ByVal Bodynumber As String)

'        Toaddress.Clear()
'        Toaddress.Add(System.Configuration.ConfigurationManager.AppSettings("ToEmails").ToString())


'        Dim strFromMailID As String = [String].Empty
'        Dim strSmtp As String = [String].Empty
'        Dim strbody As String = String.Empty
'        Dim port As Integer = 0
'        Dim MyMessage As New MailMessage()
'        strFromMailID = System.Configuration.ConfigurationManager.AppSettings("FromEmail").ToString()
'        strSmtp = System.Configuration.ConfigurationManager.AppSettings("SMTP").ToString()
'        port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings("PortNO").ToString())

'        Dim SendFrom As New MailAddress(strFromMailID)
'        MyMessage.From = SendFrom
'        MyMessage.[To].Clear()
'        MyMessage.CC.Clear()
'        MyMessage.Bcc.Clear()
'        MyMessage.Body = ""
'        MyMessage.Subject = System.Configuration.ConfigurationManager.AppSettings("Subject").ToString() & Bodynumber & " SERVER :" & My.Computer.Name

'        strbody = "<table border=0 cellspacing=0 cellpadding=0> " & _
'         "<tr><td>Dear All," & _
'         "</td></tr>" & _
'       "<tr><td><br/> BC Data Error Occurred . Error Code :" & StrMsg & "<br/><br/>" & _
' "________ DO NOT REPLY TO THIS MAIL . THIS IS AUTOGENERATED E-MAIL ________ </td></tr>"

'        For Each ToAddr As String In Toaddress
'            MyMessage.[To].Add(ToAddr)
'        Next

'        MyMessage.Body = strbody
'        MyMessage.IsBodyHtml = True

'        Dim smtp As New SmtpClient()
'        smtp.UseDefaultCredentials = True
'        smtp.Host = strSmtp
'        smtp.Port = port
'        smtp.Send(MyMessage)


'    End Sub

'    Public Sub SendStatusMail(ByVal StrMsg As String, ByVal Bodynumber As String)

'        Toaddress.Clear()
'        Toaddress.Add(System.Configuration.ConfigurationManager.AppSettings("StatusEmails").ToString())


'        Dim strFromMailID As String = [String].Empty
'        Dim strSmtp As String = [String].Empty
'        Dim strbody As String = String.Empty
'        Dim port As Integer = 0
'        Dim MyMessage As New MailMessage()
'        strFromMailID = System.Configuration.ConfigurationManager.AppSettings("FromEmail").ToString()
'        strSmtp = System.Configuration.ConfigurationManager.AppSettings("SMTP").ToString()
'        port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings("PortNO").ToString())

'        Dim SendFrom As New MailAddress(strFromMailID)
'        MyMessage.From = SendFrom
'        MyMessage.[To].Clear()
'        MyMessage.CC.Clear()
'        MyMessage.Bcc.Clear()
'        MyMessage.Body = ""
'        MyMessage.Subject = System.Configuration.ConfigurationManager.AppSettings("Subject").ToString() & Bodynumber & " SERVER :" & My.Computer.Name

'        Dim St As String = System.Configuration.ConfigurationManager.AppSettings("StatusMail").ToString() & StrMsg

'        For Each ToAddr As String In Toaddress
'            MyMessage.[To].Add(ToAddr)
'        Next

'        strbody = "<table border=0 cellspacing=0 cellpadding=0> " & _
'         "<tr><td>Dear All," & _
'         "</td></tr>" & _
'       "<tr><td><br/>" & St & " " & Date.Now & "<br/><br/>" & _
'        " ________ DO NOT REPLY TO THIS MAIL . THIS IS AUTOGENERATED E-MAIL ________ </td></tr>"


'        MyMessage.Body = strbody
'        MyMessage.IsBodyHtml = True

'        Dim smtp As New SmtpClient()
'        smtp.UseDefaultCredentials = True
'        smtp.Host = strSmtp
'        smtp.Port = port
'        smtp.Send(MyMessage)


'    End Sub



'End Class
#End Region