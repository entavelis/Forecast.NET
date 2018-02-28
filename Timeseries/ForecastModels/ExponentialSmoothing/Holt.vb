Public Class Holt
    Inherits ExponentialSmoothing

    Const MyMethodName As String = "Holt Exponential Smoothing"

#Region "Properties"
    Public Property a As Double
        Get
            Return _Parameters("a")
        End Get
        Set(value As Double)
            _Parameters("a") = value
        End Set
    End Property

    Public Property b As Double
        Get
            Return _Parameters("b")
        End Get
        Set(value As Double)
            _Parameters("b") = value
        End Set
    End Property

    Public Property S0 As Double
        Get
            Return _Parameters("S0")
        End Get
        Set(value As Double)
            _Parameters("S0") = value
        End Set
    End Property

    Public Property T0 As Double
        Get
            Return _Parameters("T0")
        End Get
        Set(value As Double)
            _Parameters("T0") = value
        End Set
    End Property

#End Region

    ' ''' <summary>
    ' ''' Calculates the Forecast Model of the Holt Exponential Smoothing Method
    ' ''' </summary>
    ' ''' <param name="Input">The Input Timeseries</param>
    ' ''' <param name="Horizon">The horizon of the forecast</param>
    ' ''' <param name="S0">The value for the initial state value for the smoothing process, if NaN or no value is assigned to S0, the level of LRL will be used instead </param>
    ' ''' <param name="T0">The value for the initial trend value for the smoothing process, if NaN or no value is assigned to T0, the trend of LRL will be used instead </param>
    ' ''' <param name="a">The value of smoothing parameter for the level, if the value '-1' is assigned to a, the method will optimize the parameter</param>
    ' ''' <param name="b">The value of smoothing parameter for the trend, if the value '-1' is assigned to b, the method will optimize the parameter</param>
    ' ''' <param name="ErrorMethod">The address of the error function that will be used to optimize the parameters (Default Method MSE)</param>
    ' ''' <remarks></remarks>
    'Public Sub Holt(ByVal Input As Timeseries, ByVal Horizon As Integer, Optional ByVal S0 As Double = Double.NaN, _
    '                Optional ByVal T0 As Double = Double.NaN, Optional a As Double = -1, Optional b As Double = -1, _
    '                Optional ByVal ErrorMethod As ErrorMethodForOptimization = Nothing)
    '    Dim Paras = Optimization(ErrorMethod, Input, S0, T0, a, b, 0)
    '    CalculateForecast(Input, Horizon, Paras)
    '    _Parameters.Add("S0", Paras.Item1)
    '    _Parameters.Add("T0", Paras.Item2)
    '    _Parameters.Add("a", Paras.Item3)
    '    _Parameters.Add("b", Paras.Item4)

    '    _MethodName = "Holt Exponential Smoothing"
    'End Sub



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

    Public Overrides Sub Calculate()
        Try
            Dim SesTs = CalculateForecastTimeseries(_Horizon, _Parameters("S0"), _Parameters("T0"), _Parameters("a"), _Parameters("b"), 1)
            _DataPoints = SesTs.DataPoints
            _Frequency = SesTs.Frequency
            _Offset = SesTs.Offset
        Catch ex As Exception
            Throw New InvalidParameterException("The Method Parameters have not initialized properly")
        End Try
    End Sub


    Public Overrides Sub OptimizeParameters()
        Dim Value As Double
        Dim tempS0 = IIf(_Parameters.TryGetValue("S0", Value), Value, Double.NaN)
        Dim tempa = IIf(_Parameters.TryGetValue("a", Value), Value, -1)
        Dim tempT0 = IIf(_Parameters.TryGetValue("T0", Value), Value, Double.NaN)
        Dim tempb = IIf(_Parameters.TryGetValue("b", Value), Value, -1)
        Dim Paras = Optimization(_ErrorMethod, tempS0, tempT0, tempa, tempb, 1)
        _Parameters("S0") = Paras.Item1
        _Parameters("T0") = Paras.Item2
        _Parameters("a") = Paras.Item3
        _Parameters("b") = Paras.Item4
    End Sub

    Public Overrides Function MyType() As ForecastFactory.ForecastModel
        Return ForecastFactory.ForecastModel.Holt
    End Function
#End Region
End Class
