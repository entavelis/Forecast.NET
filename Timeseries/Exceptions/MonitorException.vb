
Public Class MonitorException
    Inherits Exception

    Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

End Class
