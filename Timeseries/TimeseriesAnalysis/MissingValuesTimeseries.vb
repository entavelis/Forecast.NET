Public Class MissingValuesTimeseries
    Inherits AnalysisTimeseriesBaseClass

#Region "Variables"
    Private IndicesList As List(Of Integer)
#End Region

#Region "Properties"
    Public Property IndicesOfMissingValues As List(Of Integer)
        Get
            Return IndicesList
        End Get
        Set(value As List(Of Integer))
            If value.Contains(0) Or value.Contains(_Original.Length) Then Throw New InvalidParameterException("The first observation of a timeseries can not be missing/zero value")
            IndicesList = value
        End Set
    End Property

    Public Property SemisumTS As Timeseries

    Public Property SeasonalTS As Timeseries
#End Region

#Region "Constructors"

    Public Sub New()
        MyBase.New()
        _Original = New Timeseries()
        IndicesOfMissingValues = New List(Of Integer)
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        MyBase.New(Input)
        IndicesOfMissingValues = New List(Of Integer)
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal MissingIndices As List(Of Integer))
        MyBase.New(Input)
        If MissingIndices.Contains(0) Or MissingIndices.Contains(_Original.Length - 1) Then Throw New InvalidParameterException("The first observation of a timeseries can not be missing/zero value")
        IndicesList = MissingIndices
    End Sub
#End Region

#Region "Functions"

    Public Overrides Sub Calculate()
        SemiSum()
        If _Original.Frequency > 1 Then
            Seasonal()
            SetSeasonal()
        Else
            SetSemisum()
        End If
    End Sub

    Public Sub SetSemisum()
        _DataPoints = SemisumTS.DataPoints
        _Offset = SemisumTS.Offset
        _Frequency = SemisumTS.Frequency
    End Sub


    Public Sub SetSeasonal()
        _DataPoints = SeasonalTS.DataPoints
        _Offset = SeasonalTS.Offset
        _Frequency = SeasonalTS.Frequency
    End Sub

    Private Sub SemiSum()
        IndicesList.Sort()

        Dim Result() As Double = _Original.DataPoints.Clone
        Dim IndicesListIntervals As New List(Of Tuple(Of Integer, Integer))
        Dim i As Integer = 0
        While i < IndicesList.Count
            Dim count As Integer = 1
            While (i + count < IndicesList.Count) AndAlso (IndicesList(i) = IndicesList(i + count) - count)
                count += 1
            End While
            IndicesListIntervals.Add(Tuple.Create(IndicesList(i), count))
            i += count
        End While

        For Each it In IndicesListIntervals
            Dim b = Result(it.Item1 - 1)
            Dim a = (Result(it.Item1 + it.Item2) - b) / (it.Item2 + 1)

            For k As Integer = 0 To it.Item2 - 1
                Result(it.Item1 + k) = (k + 1) * a + b
            Next
        Next

        '_DataPoints = Result
        '_Frequency = _Original.Frequency
        '_Offset = _Original.Offset

        SemisumTS = New Timeseries(Result, _Original.Frequency, _Original.Offset)
    End Sub


    Private Sub Seasonal()
        If _Original.Frequency <= 1 Then Throw New InvalidParameterException("The number of seasons must be greater than zero")

        Dim Result() As Double = _Original.DataPoints.Clone
        Dim seasonaverages(_Original.Frequency - 1) As Double
        For i As Integer = 0 To _Original.Frequency - 1
            Dim j As Integer = i
            seasonaverages(i) = 0
            Dim count As Integer = 0
            While j < _Original.Length()
                If Not IndicesList.Contains(j) Then
                    seasonaverages(i) += _Original.DataPoints(j)
                    j += _Original.Frequency
                    count += 1
                Else
                    j += _Original.Frequency
                End If
            End While
            seasonaverages(i) /= count
        Next

        For i As Integer = 0 To IndicesList.Count - 1
            Result(IndicesList.Item(i)) = seasonaverages((IndicesList.Item(i)) Mod _Original.Frequency)
        Next

        '_DataPoints = Result
        SeasonalTS = New Timeseries(Result, _Original.Frequency, _Original.Offset)
    End Sub


#End Region
End Class
