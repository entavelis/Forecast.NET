Public Class RollingForecastingAnalysis
    Inherits ForecastingAnalysis

#Region "Variables"
#End Region

#Region "Properties"
    Public Property RollingStep As Integer

    Public Property RollingHorizon As Integer
#End Region

#Region "Constructors"
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        MyBase.New(Input)
    End Sub



    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer)
        MyBase.New(Input)
        _RollingStep = RollingStep
        _RollingHorizon = 1
    End Sub


    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer, ByVal RollingHorizon As Integer)
        MyBase.New(Input)
        _RollingStep = RollingStep
        _RollingHorizon = RollingHorizon
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer, ByVal RollingHorizon As Integer, ByVal RatioOfInsample As Double)
        MyBase.New(Input, 0, RatioOfInsample)
        _RollingStep = RollingStep
        _RollingHorizon = RollingHorizon
    End Sub

    'Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer, ByVal RollingHorizon As Integer, ByVal ErrorMethod As Errors.ErrorMethodForOptimization)
    '    MyBase.New(Input, 0, ErrorMethod)
    '    _RollingStep = RollingStep
    '    _RollingHorizon = RollingHorizon
    'End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal RollingStep As Integer, ByVal RollingHorizon As Integer, ByVal RatioOfInsample As Double, ByVal ErrorMethod As Errors.ErrorMethodForOptimization)
        MyBase.New(Input, 0, RatioOfInsample, ErrorMethod)
        _RollingStep = RollingStep
        _RollingHorizon = RollingHorizon
    End Sub



#End Region


#Region "Public Functions"
    Public Overrides Function CalculateBest() As Forecast


        If ForecastModels.Count = 0 Then Throw New InvalidOperationException("There are no forecast models to make this operation upon")

        Dim TempForecastModels As New List(Of Forecast)
        For i = 0 To ForecastModels.Count - 1
            TempForecastModels.Add(ForecastModels.Item(i))
        Next

        Dim bestError As Double = Double.MaxValue
        Dim bestForecast As Integer = 0

        For i = 0 To TempForecastModels.Count - 1
            Dim fm = TempForecastModels.Item(i)
            Dim tempRoll As New RollingForecast(_Original, _RollingStep, _RollingHorizon, _ErrorMethod, _RatioOfInsample)
            tempRoll.ForecastModel = ForecastModels(i)
            tempRoll.Calculate()
            tempRoll.CalculateErrors()

            Dim tempError As Double = tempRoll.ErrorsAverage
            If tempError < bestError Then
                bestError = tempError
                bestForecast = i
            End If
        Next

        CalculateBest = ForecastModels.Item(bestForecast)
        CalculateBest.Original = _Original
        CalculateBest.OptimizeParameters()
        CalculateBest.Calculate()
    End Function


#End Region

#Region "Private Functions"

#End Region


End Class
