Imports Timeseries.Errors

Public Class RollingForecast

    Delegate Function ErrorMethodForOptimization(ByVal Input As Timeseries, ByVal Forecast As Timeseries) As Double

#Region "Variables"
    Private _ErrorMethod As Errors.ErrorMethodForOptimization

    Private _RatioOfInsample As Double

#End Region

#Region "Properties"
    Public Property RollingStep As Integer


    Public Property RatioOfInsample As Integer
        Get
            Return _RatioOfInsample
        End Get
        Set(value As Integer)
            If value < 0 Or value > 1 Then Throw New InvalidParameterException("RatioOfInsample must be a number between 0 and 1")
            _RatioOfInsample = value
        End Set
    End Property

    Public Property RollingHorizon As Integer

    Public Property Original As Timeseries

    Public Property ForecastModel As Forecast

    Public Property RollingForecasts As Forecast()

    Public Property RollingInsample As Integer
        Get
            Return _Original.Length * _RatioOfInsample
        End Get
        Set(value As Integer)
            _RatioOfInsample = value / _Original.Length
        End Set
    End Property

    Public Property RollingErrors As Double()

    Public ReadOnly Property NumberOfSteps As Integer
        Get
            Dim tempMargin = IIf(_RollingHorizon > _RollingStep, _RollingHorizon - _RollingStep, 0)
            Return (Original.Length * (1 - _RatioOfInsample) - tempMargin - 1) \ _RollingStep + 1
        End Get
    End Property

#End Region

#Region "Constructors"

    Public Sub New()
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        _Original = Input
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer)
        _Original = Input
        _RollingStep = RollingStep
        _RollingHorizon = 1
        _ErrorMethod = AddressOf Errors.sMAPE
        _RatioOfInsample = 0.8
    End Sub


    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer, ByVal RollingHorizon As Integer)
        _Original = Input
        _RollingStep = RollingStep
        _RollingHorizon = RollingHorizon
        _ErrorMethod = AddressOf Errors.sMAPE
        _RatioOfInsample = 0.8
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer, ByVal RollingHorizon As Integer, ByVal RatioOfInsample As Double)
        _Original = Input
        _RollingStep = RollingStep
        _RollingHorizon = RollingHorizon
        _ErrorMethod = AddressOf Errors.sMAPE
        _RatioOfInsample = 0.8
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer, ByVal RollingHorizon As Integer, _
                   ByVal ErrorMethod As Errors.ErrorMethodForOptimization)
        _Original = Input
        _RollingStep = RollingStep
        _RollingHorizon = RollingHorizon
        _ErrorMethod = ErrorMethod
        _RatioOfInsample = 0.8
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer, ByVal RollingHorizon As Integer, _
                    ByVal ErrorMethod As Errors.ErrorMethodForOptimization, ByVal RatioOfInsample As Double)
        _Original = Input
        _RollingStep = RollingStep
        _RollingHorizon = RollingHorizon
        _ErrorMethod = ErrorMethod
        _RatioOfInsample = 0.8
    End Sub



#End Region


#Region "Public Functions"
    Public Sub Calculate()
        Dim NumOfSteps = NumberOfSteps
        Dim RollingLength = RollingInsample

        ReDim _RollingForecasts(NumOfSteps - 1)

        For i As Integer = 0 To NumOfSteps - 1
            Dim tempModel = ForecastFactory.GetModel(_ForecastModel.MyType, _Original.SubTimeseries(RollingLength))
            tempModel.Horizon = _RollingHorizon
            tempModel.OptimizeParameters()
            tempModel.Calculate()

            RollingLength += _RollingStep
            RollingForecasts(i) = tempModel

        Next

    End Sub

    Public Sub CalculateErrors()
        Dim NumOfSteps = NumberOfSteps
        ReDim _RollingErrors(NumOfSteps - 1)

        Dim RollingLength = RollingInsample

        For i As Integer = 0 To NumOfSteps - 1
            _RollingErrors(i) = _ErrorMethod.Invoke(Original, _RollingForecasts(i).SubTimeseries(_RollingHorizon, RollingLength - 1))
            RollingLength += _RollingStep
        Next
    End Sub

    Public Function ErrorsAverage() As Double
        Return _RollingErrors.Average
    End Function
#End Region

#Region "Private Functions"

#End Region



End Class
