Public Class ThetaClassic
    Inherits Forecast

    Const MyMethodName As String = "Theta Classic"

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



#Region "properties"

    Public Property LRL_a As Double
        Get
            Return _Parameters("LRL_a")
        End Get
        Set(value As Double)
            _Parameters("LRL_a") = value
        End Set
    End Property

    Public Property LRL_b As Double
        Get
            Return _Parameters("LRL_b")
        End Get
        Set(value As Double)
            _Parameters("LRL_b") = value
        End Set
    End Property


    Public Property SES_S0 As Double
        Get
            Return _Parameters("SES_S0")
        End Get
        Set(value As Double)
            _Parameters("SES_S0") = value
        End Set
    End Property

    Public Property SES_a As Double
        Get
            Return _Parameters("SES_a")
        End Get
        Set(value As Double)
            _Parameters("SES_a") = value
        End Set
    End Property
#End Region

    ''' <summary>
    ''' Calculates Forecast Model of the ThetaClassic Method
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Calculate()
        Dim Theta0 As New LRL(_Original, _Horizon)
        Theta0.Calculate()

        Dim Theta2 As New SES(2 * _Original - Theta0, _Horizon)
        Theta2.S0 = _Parameters("SES_S0")
        Theta2.a = _Parameters("SES_a")
        Theta2.Calculate()


        Dim ThetaTimeseries = (Theta0 + Theta2) / 2

        _DataPoints = ThetaTimeseries.DataPoints
        _Frequency = ThetaTimeseries.Frequency
        _Offset = ThetaTimeseries.Offset
    End Sub

    Public Overrides Sub OptimizeParameters()
        Dim Theta0 As New LRL(_Original, _Horizon)
        Theta0.Calculate()

        Dim Theta2 As New SES(2 * _Original - Theta0, _ErrorMethod)
        If _Parameters.ContainsKey("SES_a") Then Theta2.a = SES_a
        If _Parameters.ContainsKey("SES_S0") Then Theta2.S0 = SES_S0
        Theta2.OptimizeParameters()

        _Parameters("LRL_a") = Theta0.Parameters("a")
        _Parameters("LRL_b") = Theta0.Parameters("b")
        _Parameters("SES_S0") = Theta2.Parameters("S0")
        _Parameters("SES_a") = Theta2.Parameters("a")
    End Sub

    Public Overrides Function MyType() As ForecastFactory.ForecastModel
        Return ForecastFactory.ForecastModel.ThetaClassic
    End Function
End Class
