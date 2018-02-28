Public Class LRL
    Inherits Forecast

    Const MyMethodName As String = "LRL"
#Region "Properties"


    Public ReadOnly Property a As Double
        Get
            Return _Parameters("a")
        End Get
    End Property



    Public ReadOnly Property b As Double
        Get
            Return _Parameters("b")
        End Get
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

    Public Sub New(ByVal Input As Timeseries, ByVal ErrorMethod As Errors.ErrorMethodForOptimization)
        MyBase.New(Input, ErrorMethod)
        _MethodName = MyMethodName
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double, ByVal ErrorMethod As Errors.ErrorMethodForOptimization)
        MyBase.New(Input, Horizon,ErrorMethod)
        _MethodName = MyMethodName
    End Sub




#End Region


#Region "Functions"
    ''' <summary>
    ''' Calculates the Forecast Model of the LRL Method
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Calculate()
        Dim a, b As Double
        b = GetB(_Original)
        a = GetA(_Original, b)
        _DataPoints = (b * DummyTimeseries.SteppedByOne(1, _Original.Length + Horizon, _Original.Frequency, _Original.Offset) + a).DataPoints
        _Offset = _Original.Offset
        _Frequency = _Original.Frequency
        _Parameters.Add("a", a)
        _Parameters.Add("b", b)
    End Sub

    Public Overrides Sub OptimizeParameters()
    End Sub

    ''' <summary>
    ''' Returns the Forecast Model of the LRL Method
    ''' </summary>
    ''' <param name="Input">The Input Timeseries</param>
    ''' <param name="Horizon">The horizon of the forecast</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function LRLTimeseries(ByVal Input As Timeseries, ByVal horizon As Integer) As Timeseries
        Dim a, b As Double
        b = GetB(Input)
        a = GetA(Input, b)

        Return b * DummyTimeseries.SteppedByOne(1, Input.Length + horizon, Input.Frequency, Input.Offset) + a
    End Function

    ''' <summary>
    ''' Returns the Slope Value of the LRL Method
    ''' </summary>
    ''' <param name="Input">The Input Timeseries</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetB(ByVal Input As Timeseries) As Double 'We 
        Dim len As Integer = Input.Length
        Return ((Input * DummyTimeseries.SteppedByOne(1, len, Input.Frequency, Input.Offset)).Average - (len + 1) / 2 * Input.Average) / (((len ^ 2) / 3 + len / 2 + 1 / 6) - ((len + 1) / 2) ^ 2) 'I use sum of series to calculate the result
    End Function

    ''' <summary>
    ''' Returns the Constant Value of the LRL Method
    ''' </summary>
    ''' <param name="Input">The Input Timeseries</param>
    ''' <param name="b">The slope value of LRL</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetA(ByVal Input As Timeseries, ByVal b As Double) As Double
        Return Input.Average - b * (Input.Length + 1) / 2
    End Function

    ''' <summary>
    ''' Returns the Constant Value of the LRL Method
    ''' </summary>
    ''' <param name="Input">The Input Timeseries</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetA(ByVal Input As Timeseries) As Double
        Dim b = GetB(Input)
        Return GetA(Input, b)
    End Function

    ''' <summary>
    ''' Returns a tuple of (Constant,Slope) of the LRL of the input timeseries
    ''' </summary>
    ''' <param name="Input">The input timeseries</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetParameters(ByVal Input As Timeseries) As Tuple(Of Double, Double)
        Dim b = GetB(Input)
        Dim a = GetA(Input, b)
        Return Tuple.Create(a, b)
    End Function

    Public Overrides Function MyType() As ForecastFactory.ForecastModel
        Return ForecastFactory.ForecastModel.LRL
    End Function
#End Region
End Class
