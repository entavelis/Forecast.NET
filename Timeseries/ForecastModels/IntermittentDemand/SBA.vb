Public Class SBA
    Inherits Croston
    Const MyMethodName As String = "SBA"

#Region "Constructors"
    Public Sub New()
        MyBase.New()
        _MethodName = MyMethodName
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        MyBase.New(Input)
        _MethodName = MyMethodName
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double)
        MyBase.New(Input, Horizon)
        _MethodName = MyMethodName
    End Sub

#End Region

#Region "Functions"

    ''' <summary>
    ''' Calculates the Forecast Model of the Croston Method for intermittent demand manipulation
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Calculate()
        MyBase.Calculate()
        _DataPoints = (Me * (1 - _Parameters("DEM_a") / 2)).DataPoints
    End Sub

    Public Overrides Function MyType() As ForecastFactory.ForecastModel
        Return ForecastFactory.ForecastModel.SBA
    End Function

#End Region

End Class
