Imports Timeseries.StatisticalMethods

Public Class SpecialEventsTimeseries
    Inherits AnalysisTimeseriesBaseClass

#Region "Variables"
    Private Deseasonalized As Timeseries
    Private TrendXCircle As Timeseries
    Private Forecast As Timeseries
#End Region

#Region "Properties"
    Public Overrides Property Original As Timeseries
        Get
            Return MyBase.Original
        End Get
        Set(value As Timeseries)
            MyBase.Original = value
            Initialize()
        End Set
    End Property

    Public Property IndicesOfSEA As List(Of Integer)

#End Region

#Region "Constructors"
    Public Sub New()
        MyBase.New()
        _Original = New Timeseries
        _IndicesOfSEA = New List(Of Integer)
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        MyBase.New(Input)
        _IndicesOfSea = New List(Of Integer)
        Initialize()
    End Sub
#End Region

#Region "Public Functions"
    Public Overrides Sub Calculate()
        If _IndicesOfSEA.Contains(0) Or _IndicesOfSEA.Contains(_Original.Length - 1) Then Throw New InvalidParameterException("The first or the last observation of a timeseries can not be missing/zero value")
        SemiSum()
    End Sub

    Public Function Method(ByVal NumberOfMethod As Integer, Optional ByVal ThresholdA As Double = 0, Optional ThresholdB As Double = 0) As List(Of Integer)

        Select Case NumberOfMethod
            Case 1
                Return Method1(ThresholdA, ThresholdB)
            Case 2
                Return Method2(ThresholdA)
            Case 3
                Return Method3(ThresholdA)
            Case 4
                Return Method4(ThresholdA)
        End Select

        Throw New InvalidParameterException("The SEA methods are numbered from 1 to 4")

    End Function

    Public Sub SetSEA(ByVal NumberOfMethod As Integer, Optional ByVal ThresholdA As Double = 0, Optional ThresholdB As Double = 0)
        _IndicesOfSEA = Method(NumberOfMethod, ThresholdA, ThresholdB)
    End Sub

    Public Function Impact() As Timeseries
        Return ((_Original / Me) - 1) * 100
    End Function

#End Region

#Region "Private Functions"

    Private Sub Initialize()
        If _Original.Frequency > 1 AndAlso _Original.Length > _Original.Frequency Then
            Deseasonalized = Decomposition.SimpleDecomposition.GetSeasonality(_Original) * _Original
            TrendXCircle = Decomposition.SimpleDecomposition.GetTrend(_Original) * Decomposition.SimpleDecomposition.GetCircle(_Original)
        Else
            Deseasonalized = _Original
            TrendXCircle = Decomposition.SimpleDecomposition.GetTrend(_Original, 3) * Decomposition.SimpleDecomposition.GetCircle(_Original, 3)
        End If
        Dim ForecastSes = New SES(Deseasonalized, 0)
        ForecastSes.OptimizeParameters()
        ForecastSes.Calculate()
        Forecast = ForecastSes
    End Sub



    Private Function Method1(Optional ByVal ThresholdA As Double = 0, Optional ByVal ThresholdB As Double = 0) As List(Of Integer)
        Dim Ratio1 As Timeseries = Deseasonalized / TrendXCircle
        Dim Ratio2 As Timeseries = Deseasonalized / Forecast

        Method1 = New List(Of Integer)

        For i As Integer = 0 To Ratio1.Length - 1
            If (Ratio1.DataPoints(i) >= 1.1 - ThresholdA / 100 Or Ratio1.DataPoints(i) <= 0.9 + ThresholdA / 100) And (Ratio2.DataPoints(i) >= 1.25 - ThresholdB / 100 Or Ratio2.DataPoints(i) <= 0.75 + ThresholdB / 100) Then
                Method1.Add(i + Ratio1.Offset)
            End If
        Next
    End Function

    Private Function Method2(Optional ByVal ThresholdA As Double = 0) As List(Of Integer)
        If ThresholdA > 3 Then Throw New InvalidParameterException("SEA's second method demands its threshold to be not greater than 3")

        Dim DesMean = Deseasonalized.Average
        Dim StDf = Forecast.StDev

        Method2 = New List(Of Integer)

        For i As Integer = 0 To Deseasonalized.Length - 1
            If (Deseasonalized.DataPoints(i) >= DesMean + (3 - ThresholdA) * StDf Or Deseasonalized.DataPoints(i) <= DesMean - (3 - ThresholdA) * StDf) Then
                Method2.Add(i)
            End If

        Next
    End Function

    Private Function Method3(Optional ByVal ThresholdA As Double = 0) As List(Of Integer)
        If ThresholdA > 5 Then Throw New InvalidParameterException("SEA's third method demands its threshold to be not greater than 5")

        Dim Ratio As Timeseries = SMA(Deseasonalized, 7) / SMA(Deseasonalized, 5)

        Method3 = New List(Of Integer)

        For i As Integer = 0 To Ratio.Length - 1
            If (Ratio.DataPoints(i) >= 1.05 - ThresholdA / 100 Or Ratio.DataPoints(i) <= 0.95 + ThresholdA / 100) Then
                Method3.Add(i + Ratio.Offset)
            End If
        Next
    End Function

    Private Function Method4(Optional ByVal ThresholdA As Double = 0) As List(Of Integer)
        If ThresholdA > 10 Then Throw New InvalidParameterException("SEA's forth method demands its threshold to be not greater than 5")
        If Deseasonalized.Frequency < 2 Then Throw New InvalidParameterException("SEA's forth method requires a seasonal timeseries as input")

        Dim Ratio As Timeseries = Deseasonalized / CSMA(Deseasonalized, Deseasonalized.Frequency)

        Method4 = New List(Of Integer)

        For i As Integer = 0 To Ratio.Length - 1
            If (Ratio.DataPoints(i) >= 1.1 - ThresholdA / 100 Or Ratio.DataPoints(i) <= 0.9 + ThresholdA / 100) Then
                Method4.Add(i + Ratio.Offset)
            End If
        Next
    End Function


    Private Sub SemiSum()
        _IndicesOfSEA.Sort()

        Dim Result() As Double = _Original.DataPoints.Clone
        Dim _IndicesOfSeaIntervals As New List(Of Tuple(Of Integer, Integer))
        Dim i As Integer = 0
        While i < _IndicesOfSEA.Count
            Dim count As Integer = 1
            While (i + count < _IndicesOfSEA.Count) AndAlso (_IndicesOfSEA(i) = _IndicesOfSEA(i + count) - count)
                count += 1
            End While
            _IndicesOfSeaIntervals.Add(Tuple.Create(_IndicesOfSEA(i), count))
            i += count
        End While

        For Each it In _IndicesOfSeaIntervals
            Dim b = Result(it.Item1 - 1)
            Dim a = (Result(it.Item1 + it.Item2) - b) / (it.Item2 + 1)

            For k As Integer = 0 To it.Item2 - 1
                Result(it.Item1 + k) = (k + 1) * a + b
            Next
        Next

        _DataPoints = Result
        _Offset = _Original.Offset
        _Frequency = _Original.Frequency
    End Sub


#End Region
End Class
