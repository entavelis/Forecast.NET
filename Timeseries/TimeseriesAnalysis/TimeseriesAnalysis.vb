Public Class TimeseriesAnalysis

#Region "Private Variables"
    Private _Original As Timeseries
    Private _NonMissing As MissingValuesTimeseries
    Private _DateAdjusted As DateAdjustedTimeseries
    Private _SeaAdjusted As SpecialEventsTimeseries
    Private _Decomposed As DecomposedTimeseries
#End Region

#Region "Properties"
    Public Property Original As Timeseries
        Set(value As Timeseries)
            _Original = value
        End Set
        Get
            Return _Original
        End Get
    End Property

    Public Property NonMissing As MissingValuesTimeseries
        Set(value As MissingValuesTimeseries)
            _NonMissing = value
        End Set
        Get
            Return _NonMissing
        End Get
    End Property

    Public Property DateAdjusted As DateAdjustedTimeseries
        Set(value As DateAdjustedTimeseries)
            _DateAdjusted = value
        End Set
        Get
            Return _DateAdjusted
        End Get
    End Property

    Public Property SeaAdjusted As SpecialEventsTimeseries
        Set(value As SpecialEventsTimeseries)
            _SeaAdjusted = value
        End Set
        Get
            Return _SeaAdjusted
        End Get
    End Property

    Public Property IsSeasonal As Boolean

    Public Property Decomposed As DecomposedTimeseries
        Set(value As DecomposedTimeseries)
            _Decomposed = value
        End Set
        Get
            Return _Decomposed
        End Get
    End Property

    Public Property Timestamps As Date()

#End Region

#Region "Constructors"
    Public Sub New(ByVal OriginalData() As Double, Optional ByVal Frequency As Integer = 1, Optional ByVal Offset As Integer = 0)
        _Original = New Timeseries(OriginalData, Frequency, Offset)
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        _Original = Input
    End Sub

    Public Sub New()
        _Original = New Timeseries
    End Sub

#End Region

#Region "Missing Values"
    Public Sub CreateMissingValues()
        _NonMissing = New MissingValuesTimeseries(_Original)
    End Sub


    Public Sub CreateMissingValues(ByVal IndicesOfMissingValues As List(Of Integer))
        _NonMissing = New MissingValuesTimeseries(_Original, IndicesOfMissingValues)
    End Sub

    Public Sub CalculateMissingValues()
        _NonMissing.Calculate()
    End Sub

    Public Sub UpdateMissingValues(ByVal IndicesOfMissingValues As List(Of Integer))
        _NonMissing.IndicesOfMissingValues = IndicesOfMissingValues
    End Sub

#End Region

#Region "Date Adjustments"
    Public Sub CreateDateAdjusted()
        _DateAdjusted = New DateAdjustedTimeseries(_NonMissing)
    End Sub
    Public Sub CreateDateAdjusted(ByVal WorkingDays As Timeseries)
        _DateAdjusted = New DateAdjustedTimeseries(_NonMissing, WorkingDays)
    End Sub

    Public Sub CalculateDateAdjusted()
        _DateAdjusted.Calculate()
    End Sub

    Public Sub UpdateDateAdjusted(ByVal WorkingDays As Timeseries)
        _DateAdjusted.WorkingDays = WorkingDays
    End Sub

#End Region

#Region "Special Events"
    Public Sub CreateSpecialEventsTimeseries()
        _SeaAdjusted = New SpecialEventsTimeseries(_DateAdjusted)
    End Sub

    Public Sub CreateSpecial()
        _SeaAdjusted = New SpecialEventsTimeseries(_NonMissing)
    End Sub

    Public Function GetSEAMethod(ByVal MethodNumber As Integer, Optional ByVal ThresholdA As Integer = 0, Optional ByVal ThresholdB As Integer = 0) As List(Of Integer)
        Return _SeaAdjusted.Method(MethodNumber, ThresholdA, ThresholdB)
    End Function

    Public Sub SetSea(ByVal IndicesOfSEA As List(Of Integer))
        _SeaAdjusted.IndicesOfSEA = IndicesOfSEA
    End Sub

    Public Sub CalculateSEAAdjustments()
        _SeaAdjusted.Calculate()
    End Sub

#End Region

#Region "Decomposition"

    Public Sub CreateDecomposedTimeseries()
        _Decomposed = New DecomposedTimeseries(_DateAdjusted)
    End Sub

    Public Sub MakeDecomposition()
        _Decomposed.Decompose()
    End Sub

    Public Function GetDeseasonalized()
        Return _Decomposed.Deseasonalised
    End Function
#End Region

#Region "Auto"

#End Region

End Class
