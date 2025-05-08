Imports System.ServiceProcess

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SocketListener
    Inherits System.ServiceProcess.ServiceBase

    'UserService overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ' The main entry point for the process
    <MTAThread()> _
    <System.Diagnostics.DebuggerNonUserCode()> _
    Shared Sub Main()
        'While installing this service in Production Please uncomment below code: Mahesh Avanti
        'Dim ServicesToRun() As System.ServiceProcess.ServiceBase
        'ServicesToRun = New System.ServiceProcess.ServiceBase() {New SocketListener}
        'System.ServiceProcess.ServiceBase.Run(ServicesToRun)

        'Debug locally use belo code: Mahesh Avanti
        Dim obj As SocketListener = New SocketListener
        obj.OnStart(New String() {""})


    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
        Me.ServiceName = "Service1"
    End Sub

End Class
