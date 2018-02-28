Imports Timeseries.Errors

Public Class ForecastingAnalysis

#Region "Variables"
    Protected _ErrorMethod As Errors.ErrorMethodForOptimization

    Protected _RatioOfInsample As Double

    Protected Property _Original As Timeseries

    Protected Property _DeSeasonalised As Timeseries

    Protected Property _SeasonalityIndices As Timeseries
#End Region

#Region "Properties"
    Public Property Original As Timeseries
        Get
            Return _Original
        End Get
        Set(value As Timeseries)
            _Original = value
        End Set
    End Property

    Public Property Horizon As Integer

    Public Property RatioOfInsample As Double
        Get
            Return _RatioOfInsample
        End Get
        Set(value As Double)
            If value < 0 Or value > 1 Then Throw New InvalidParameterException("RatioOfInsample must be a number between 0 and 1")
            _RatioOfInsample = value
        End Set
    End Property

    Public WriteOnly Property NewForecastModel As Forecast
        Set(value As Forecast)
            _ForecastModels.Add(value)
        End Set
    End Property

    Public Property ForecastModels As List(Of Forecast)


    Public WriteOnly Property ErrorMethod As Errors.ErrorMethodForOptimization
        Set(value As Errors.ErrorMethodForOptimization)
            _ErrorMethod = value
        End Set
    End Property

    Public Property BudgetForecast As Timeseries

    Public Property JudgmentalForecast As Timeseries

    Public Property StatisticalForecast As Timeseries

    Public Property Deseasonalised As Timeseries
        Get
            Return _DeSeasonalised
        End Get
        Set(value As Timeseries)
            If (_Original.Length <> value.Length) Or (_Original.Frequency <> value.Frequency) Or (_Original.Offset <> value.Offset) Then Throw New InvalidParameterException("The Deseasonalised Timeseries must share the same characteristics as the Original Timeseries")
            _isDeseasonalised = True
            _DeSeasonalised = value
            _SeasonalityIndices = New Timeseries(_Original.Length + _Horizon, _Original.Frequency, _Original.Offset)
            For i As Integer = 0 To _Original.Length - 1
                _SeasonalityIndices.DataPoints(i) = (_Original / value).DataPoints(i)
            Next
            For i As Integer = _Original.Length To _Original.Length + _Horizon - 1
                _SeasonalityIndices.DataPoints(i) = _SeasonalityIndices.DataPoints(i Mod _Original.Frequency)
            Next
        End Set
    End Property

    Public ReadOnly Property SeasonalityIndices As Timeseries
        Get
            Return _SeasonalityIndices
        End Get
    End Property
    Public Property isDeseasonalised As Boolean

#End Region

#Region "Constructors"
    Public Sub New()
        _RatioOfInsample = 0.8
        _ErrorMethod = AddressOf Errors.sMAPE
        _ForecastModels = New List(Of Forecast)
        _isDeseasonalised = False
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        _Original = Input
        _RatioOfInsample = 0.8
        _ErrorMethod = AddressOf Errors.sMAPE
        _ForecastModels = New List(Of Forecast)
        _isDeseasonalised = False
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Integer)
        _Original = Input
        _Horizon = Horizon
        _RatioOfInsample = 0.8
        _ErrorMethod = AddressOf Errors.sMAPE
        _ForecastModels = New List(Of Forecast)
        _isDeseasonalised = False
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Integer, ByVal RatioOfInsample As Double)
        _Original = Input
        If RatioOfInsample < 0 Or RatioOfInsample > 1 Then Throw New InvalidParameterException("RatioOfInsample must be a number between 0 and 1")
        _Horizon = Horizon
        _RatioOfInsample = RatioOfInsample
        _ErrorMethod = AddressOf Errors.sMAPE
        _ForecastModels = New List(Of Forecast)
        _isDeseasonalised = False
    End Sub


    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Integer, ByVal RatioOfInsample As Double, ByVal ErrorMethod As Errors.ErrorMethodForOptimization)
        _Original = Input
        _Horizon = Horizon
        _RatioOfInsample = RatioOfInsample
        _ErrorMethod = ErrorMethod
        _ForecastModels = New List(Of Forecast)
        _isDeseasonalised = False
    End Sub

#End Region

#Region "Private Methods"

#End Region

#Region "Public Methods"
    'MUST BE CHECKED!!!!
    ''' <summary>
    ''' Calculates the best forecast based on the given error (default is sMAPE), doesn't change the ForecastModels list values.
    ''' </summary>
    ''' <returns>The best forecast</returns>
    ''' <remarks></remarks>
    Public Overridable Function CalculateBest() As Forecast
        If ForecastModels.Count = 0 Then Throw New InvalidOperationException("There are no forecast models to make this operation upon")

        Dim bestError As Double = Double.MaxValue
        Dim bestForecast As Integer = 0

        Dim InSampleHistorical As Integer = _Original.Length * _RatioOfInsample
        Dim InSampleHorizon = _Original.Length - InSampleHistorical

        Dim applyDeseasonalisation As Boolean
        'Code got ugly, must be optimized/cleaned up in revision
        For i = 0 To ForecastModels.Count - 1
            Dim input As Timeseries
            applyDeseasonalisation = _isDeseasonalised And ForecastModels(i).MyType <> ForecastModel.Croston And ForecastModels(i).MyType <> ForecastModel.SBA
            If applyDeseasonalisation Then
                input = _DeSeasonalised
            Else
                input = _Original
            End If
            Dim fm = ForecastFactory.GetModel(ForecastModels(i).MyType, input.SubTimeseries(InSampleHistorical))
            fm.Horizon = input.Length * (1 - _RatioOfInsample)

            For Each keyvalue In ForecastModels(i).Parameters
                fm.Parameters.Add(keyvalue.Key, keyvalue.Value)
            Next

            fm.OptimizeParameters()
            fm.Calculate()


            If applyDeseasonalisation Then
                Dim temp = fm * SeasonalityIndices
                fm.DataPoints = temp.DataPoints
                fm.Frequency = temp.Frequency
                fm.Offset = temp.Offset
            End If

            Dim tempError As Double = _ErrorMethod.Invoke(_Original.SubTimeseries(InSampleHorizon, InSampleHistorical), fm)
            If tempError < bestError Then
                bestError = tempError
                bestForecast = i
            End If
        Next

        Dim theInput As Timeseries
        applyDeseasonalisation = _isDeseasonalised And ForecastModels(bestForecast).MyType <> ForecastModel.Croston And ForecastModels(bestForecast).MyType <> ForecastModel.SBA
        If applyDeseasonalisation Then
            theInput = _DeSeasonalised
        Else
            theInput = _Original
        End If

        CalculateBest = ForecastFactory.GetModel(ForecastModels.Item(bestForecast).MyType, theInput)

        CalculateBest.Original.DataPoints = theInput.DataPoints
        CalculateBest.Original.Offset = theInput.Offset
        CalculateBest.Original.Frequency = theInput.Frequency

        For Each keyvalue In ForecastModels(bestForecast).Parameters
            CalculateBest.Parameters.Add(keyvalue.Key, keyvalue.Value)
        Next

        CalculateBest.Horizon = _Horizon

        CalculateBest.OptimizeParameters()
        CalculateBest.Calculate()

        If applyDeseasonalisation Then
            Dim temp = CalculateBest * SeasonalityIndices
            CalculateBest.DataPoints = temp.DataPoints
            CalculateBest.Frequency = temp.Frequency
            CalculateBest.Offset = temp.Offset
        End If
    End Function

    ''' <summary>
    ''' Calculates all the forecasts in the ForecastModels list, so every forecast can be accessed by the list (ForecastModels(index)
    '''  </summary>
    ''' <remarks></remarks>
    Public Sub CalculateAll()
        For Each fm In _ForecastModels
            Dim input As Timeseries

            If _isDeseasonalised And fm.MyType <> ForecastModel.Croston And fm.MyType <> ForecastModel.SBA Then
                input = _DeSeasonalised
            Else
                input = _Original
            End If


            fm.Original = New Timeseries(input)
            fm.Original.DataPoints = input.DataPoints
            fm.Original.Offset = input.Offset
            fm.Original.Frequency = input.Frequency
            fm.Horizon = _Horizon
            fm.OptimizeParameters()
            fm.Calculate()

            If _isDeseasonalised And fm.mytype <> ForecastModel.Croston And fm.mytype <> ForecastModel.SBA Then
                Dim temp = fm * SeasonalityIndices
                fm.DataPoints = temp.DataPoints
                fm.Frequency = temp.Frequency
                fm.Offset = temp.Offset
            End If

        Next
    End Sub

    ''' <summary>
    ''' Calculates the single forecast in the ForecastModels list's specific Index
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <remarks></remarks>
    Public Sub CalculateAtIndex(ByVal Index As Integer)
        CalculateModel(_ForecastModels(Index))
    End Sub

    ''' <summary>
    ''' Calculates the forecast model given by input (it alters the input itself)
    ''' </summary>
    ''' <param name="InputForecastModel">The forecast model to be calculated</param>
    ''' <remarks></remarks>
    Public Sub CalculateModel(ByRef InputForecastModel As Forecast)

        Dim input As Timeseries

        If _isDeseasonalised And InputForecastModel.MyType <> ForecastModel.Croston And InputForecastModel.MyType <> ForecastModel.SBA Then
            input = _DeSeasonalised
        Else
            input = _Original
        End If

        InputForecastModel.Original = New Timeseries(input)
        InputForecastModel.Original.DataPoints = input.DataPoints
        InputForecastModel.Original.Offset = input.Offset
        InputForecastModel.Original.Frequency = input.Frequency
        InputForecastModel.Horizon = _Horizon
        InputForecastModel.OptimizeParameters()
        InputForecastModel.Calculate()

        If _isDeseasonalised And InputForecastModel.MyType <> ForecastModel.Croston And InputForecastModel.MyType <> ForecastModel.SBA Then
            Dim temp = InputForecastModel * SeasonalityIndices
            InputForecastModel.DataPoints = temp.DataPoints
            InputForecastModel.Frequency = temp.Frequency
            InputForecastModel.Offset = temp.Offset
        End If
    End Sub

    ''' <summary>
    ''' Returns the cross validation error of the forecast model and the original timeseries, the default type of error is sMAPE and the default InsampleRatio Value = 0.8
    ''' </summary>
    ''' <param name="index">The index of the forecast in ForecastModels list that its error we want to calculate</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CrossValidationError(ByVal index As Integer) As Double
        Return CrossValidationError(_ForecastModels(index))
    End Function

    ''' <summary>
    ''' Returns the cross validation error of the forecast model and the original timeseries, the default type of error is sMAPE and the default InsampleRatio Value = 0.8
    ''' </summary>
    ''' <param name="InputForecastModel">The Forecast that its error we want to calculate based on this instance's original timeseries</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CrossValidationError(ByVal InputForecastModel As Forecast) As Double
        Dim InSampleHistorical = _Original.Length * _RatioOfInsample
        Dim InSampleHorizon = _Original.Length - InSampleHistorical
        Return _ErrorMethod.Invoke(_Original.SubTimeseries(InSampleHorizon, InSampleHistorical), InputForecastModel)
    End Function

    ''' <summary>
    ''' Returns the Budget Forecasting Timeseries based on the provided GrowthRate Array
    ''' </summary>
    ''' <param name="GrowthRate">The array of horizon's GrowthRate values</param>
    ''' <returns>The Budget Forecasting (Its insample values equals to the original's</returns>
    ''' <remarks></remarks>
    Public Function CalculateBudgetForecast(ByVal GrowthRate As Double()) As Timeseries
        Dim temp As Double() = GrowthRate.Clone
        Dim OutOfSample = New Timeseries(temp, _Original.Frequency, _Original.Offset + _Original.Length) + 1

        OutOfSample.DataPoints(0) *= _Original.LastValue
        For i As Integer = 1 To OutOfSample.Length - 1
            OutOfSample.DataPoints(i) *= OutOfSample.DataPoints(i - 1)
        Next

        Return _Original & OutOfSample
    End Function

    ''' <summary>
    ''' Returns the Final Forecast Timeseries based on the values of the statistical, judgmental and budget forecast timeseries, and their respective weights which are given as input and must sum up to one.
    ''' </summary>
    ''' <param name="StatisticalWeight">The Final Forecast percentage that comes from the Statistical Forecast</param>
    ''' <param name="JudgmentalWeight">The Final Forecast percentage that comes from the Judgmental Forecast</param>
    ''' <param name="BudgetWeight">The Final Forecast percentage that comes from the Budget Forecast</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalculateFinalForecast(ByVal StatisticalWeight As Double, ByVal JudgmentalWeight As Double, ByVal BudgetWeight As Double) As Timeseries
        'We need to add check so we can be sure that the weights sum adds up to one, but they are doubles so...
        Try
            CalculateFinalForecast = StatisticalForecast * StatisticalWeight + JudgmentalWeight * JudgmentalForecast + BudgetForecast * BudgetWeight
        Catch ex As Exception
            Throw New Exception("One of the Forecasts has not been initialized")
        End Try
    End Function

#End Region
End Class
