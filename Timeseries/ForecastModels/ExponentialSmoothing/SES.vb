Public Class SES
    Inherits ExponentialSmoothing


#Region "Properties"
    Const MyMethodName = "Simple Exponential Smoothing"

    Public Property a As Double
        Get
            Return _Parameters("a")
        End Get
        Set(value As Double)
            _Parameters("a") = value
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
#End Region

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

    Public Sub New(ByVal Input As Timeseries,ByVal ErrorMethod as Errors.ErrorMEthodForOptimization)
        MyBase.New(Input,ErrorMethod)
        _MethodName = MyMethodName
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double,ByVal ErrorMethod as Errors.ErrorMEthodForOptimization)
        MyBase.New(Input, Horizon,ErrorMethod)
        _MethodName = MyMethodName
    End Sub


#End Region


#Region "Functions"

    Public Overrides Sub Calculate()
        Try

            Dim SesTs = CalculateForecastTimeseries(_Horizon, _Parameters("S0"), 0, _Parameters("a"), 0, 0)
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
        Dim Paras = Optimization(_ErrorMethod, tempS0, 0, tempa, 0, 0)
        _Parameters("S0") = Paras.Item1
        _Parameters("a") = Paras.Item3
    End Sub

    Public Overrides Function MyType() As ForecastFactory.ForecastModel
        Return ForecastFactory.ForecastModel.SES
    End Function
#End Region

End Class
