Public Class Croston
    Inherits Forecast

    Const MyMethodName As String = "Croston"

#Region "Properties"

    Public Property Aofdemand As Double
        Get
            Return _Parameters("DEM_a")
        End Get
        Set(value As Double)
            _Parameters("DEM_a") = value
        End Set
    End Property

    Public Property S0ofdemand As Double
        Get
            Return _Parameters("DEM_S0")
        End Get
        Set(value As Double)
            _Parameters("DEM_S0") = value
        End Set
    End Property


    Public Property Aofintervals As Double
        Get
            Return _Parameters("INT_a")
        End Get
        Set(value As Double)
            _Parameters("INT_a") = value
        End Set
    End Property

    Public Property S0ofintervals As Double
        Get
            Return _Parameters("INT_S0")
        End Get
        Set(value As Double)
            _Parameters("INT_S0") = value
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

    ''' <summary>
    ''' Returns the Forecast Model of the Croston Method for intermittent demand manipulation
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Calculate()
        Dim demands As New List(Of Double)
        Dim intervals As New List(Of Double)

        Dim count As Integer = 0
        For i As Integer = 0 To _Original.Length - 1
            count += 1
            If _Original.DataPoints(i) <> 0 Then
                demands.Add(_Original.DataPoints(i))
                intervals.Add(count)
                count = 0
            End If
        Next

        Dim demandsTS As Timeseries = New Timeseries(demands.ToArray, _Original.Frequency, _Original.Offset)
        Dim intervalsTS As Timeseries = New Timeseries(intervals.ToArray, _Original.Frequency, _Original.Offset)

        Dim demandsSes As New SES(demandsTS, _Horizon)
        demandsSes.S0 = _Parameters("DEM_S0")
        demandsSes.a = _Parameters("DEM_a")
        demandsSes.Calculate()

        Dim intervalsSes As New SES(intervalsTS, _Horizon)
        intervalsSes.S0 = _Parameters("INT_S0")
        intervalsSes.a = _Parameters("INT_a")
        intervalsSes.Calculate()


        Dim tempTS As Timeseries = demandsSes / intervalsSes

        Dim CrostonArray(_Original.Length + Horizon - 1) As Double
        Dim index As Integer = 0
        count = 0
        For index = 0 To _Original.Length - 1
            CrostonArray(index) = tempTS.DataPoints(count)
            If _Original.DataPoints(index) <> 0 Then
                count += 1
            End If
        Next
        'While index < _Original.Length
        '    For j As Integer = 0 To intervals(count) - 1
        '        CrostonArray(index) = tempTS.DataPoints(count)
        '        index += 1
        '    Next
        '    count += 1
        'End While
        For j As Integer = _Original.Length To _Original.Length + Horizon - 1
            CrostonArray(index) = tempTS.DataPoints(count)
            index += 1
        Next

        _DataPoints = CrostonArray
        _Frequency = _Original.Frequency
        _Offset = _Original.Offset

    End Sub

    Public Overrides Sub OptimizeParameters()
        Dim demandssum As Double = 0
        Dim intervalsum As Double = 0
        Dim count As Integer = 0
        Dim countall As Integer = 0
        For i As Integer = 0 To _Original.Length - 1
            count += 1
            If _Original.DataPoints(i) <> 0 Then
                demandssum += _Original.DataPoints(i)
                intervalsum += count
                count = 0
                countall += 1
            End If
        Next

        If Not _Parameters.ContainsKey("DEM_S0") Then _Parameters("DEM_S0") = demandssum / countall
        If Not _Parameters.ContainsKey("INT_S0") Then _Parameters("INT_S0") = intervalsum / countall
        If Not _Parameters.ContainsKey("DEM_a") Then _Parameters("DEM_a") = 0.1
        If Not _Parameters.ContainsKey("INT_a") Then _Parameters("INT_a") = 0.1
    End Sub

    Public Overrides Function MyType() As ForecastFactory.ForecastModel
        Return ForecastFactory.ForecastModel.Croston
    End Function
#End Region
End Class
