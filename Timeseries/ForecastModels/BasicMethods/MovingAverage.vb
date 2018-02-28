Public Class MovingAverage
    Inherits Forecast

    Const MyMethodName As String = "Moving Averages"

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
        MyBase.New(Input, ErrorMethod)
        _MethodName = MyMethodName
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double, ByVal ErrorMethod As Errors.ErrorMethodForOptimization)
        MyBase.New(Input, Horizon, ErrorMethod)
        _MethodName = MyMethodName
    End Sub


#End Region

    Public Property LengthForAverageOut As Double
        Get
            Return _Parameters("n")
        End Get
        Set(value As Double)
            If value <= 0 Then Throw New InvalidParameterException("The length value for average must be positive and less than the timeseries' length")
            _Parameters("n") = value
        End Set
    End Property

    ''' <summary>
    '''  Calculates the Forecast Model of the Simple Moving Average Method
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Calculate()
        Dim MovingTimeseries = MA(_Original, Horizon, _Parameters("n"))
        _DataPoints = MovingTimeseries.DataPoints
        _Frequency = MovingTimeseries.Frequency
        _Offset = MovingTimeseries.Offset
    End Sub

    Public Overrides Sub OptimizeParameters()
        If Not _Parameters.ContainsKey("n") Then _Parameters("n") = _Original.Frequency
    End Sub

    Public Overrides Function MyType() As ForecastFactory.ForecastModel
        Return ForecastFactory.ForecastModel.MovingAverage
    End Function

    Private Function MA(ByVal Input As Timeseries, ByVal horizon As Integer, Optional ByVal LengthForAverageOut As Double = 0) As Timeseries
        If LengthForAverageOut <= 0 Or LengthForAverageOut > Input.Length Then Throw New InvalidParameterException("The length value for average must be positive and less than the timeseries' length")
        If horizon < 0 Then Throw New InvalidParameterException("Horizon must be a non negative integer")

        MA = StatisticalMethods.MA(Input, LengthForAverageOut).UpdateOffset(LengthForAverageOut)
        Select Case horizon
            Case 0
                Return MA.SubTimeseries(MA.Length - 1)
            Case 1
                Return MA
            Case Else
                Return MA & (MA.LastValue * DummyTimeseries.Ones(horizon - 1, MA.Frequency, MA.Length + MA.Offset))
        End Select

    End Function
End Class
