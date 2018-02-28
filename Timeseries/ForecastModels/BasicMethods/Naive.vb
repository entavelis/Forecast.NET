Public Class Naive
    Inherits Forecast

    Const MyMethodName As String = "Naive"

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

    Public Sub New(ByVal Input As Timeseries, ByVal ErrorMethod As Errors.ErrorMethodForOptimization)
        MyBase.New(Input)
        _MethodName = MyMethodName
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double,ByVal ErrorMethod as Errors.ErrorMEthodForOptimization)
        MyBase.New(Input, Horizon,ErrorMethod)
        _MethodName = MyMethodName
    End Sub


#End Region

    ''' <summary>
    ''' Calculates the Forecast Model of the Naive Method
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Calculate()
        ReDim _DataPoints(_Original.Length + _Horizon - 2)
        Array.Copy(_Original.DataPoints, _DataPoints, _Original.Length - 1)

        Dim LastV As Double = _Original.LastValue
        For i As Integer = _Original.Length - 1 To _Original.Length + _Horizon - 2
            _DataPoints(i) = LastV
        Next
        _Frequency = _Original.Frequency
        _Offset = _Original.Offset + 1


    End Sub

    Public Overrides Sub OptimizeParameters()
    End Sub


    Public Overrides Function MyType() As ForecastFactory.ForecastModel
        Return ForecastFactory.ForecastModel.Naive
    End Function
End Class
