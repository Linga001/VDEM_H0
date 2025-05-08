<System.ComponentModel.RunInstaller(True)> Partial Class ProjectInstaller
    Inherits System.Configuration.Install.Installer

    'Installer overrides dispose to clean up the component list.
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

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ServiceProcessInstaller1 = New System.ServiceProcess.ServiceProcessInstaller()
        Me.SocketInstaller = New System.ServiceProcess.ServiceInstaller()
        '
        'ServiceProcessInstaller1
        '
        Me.ServiceProcessInstaller1.Password = Nothing
        Me.ServiceProcessInstaller1.Username = Nothing
        '
        'SocketInstaller
        '
        Me.SocketInstaller.Description = "Socket listener : Listens to the port number provided and  process the data to th" &
    "e database of the application connected. "
        Me.SocketInstaller.ServiceName = "Socket Listener for VINMS #1"
        Me.SocketInstaller.ServicesDependedOn = New String() {""}
        '
        'ProjectInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.ServiceProcessInstaller1, Me.SocketInstaller})

    End Sub
    Friend WithEvents ServiceProcessInstaller1 As System.ServiceProcess.ServiceProcessInstaller
    Friend WithEvents SocketInstaller As System.ServiceProcess.ServiceInstaller

End Class
