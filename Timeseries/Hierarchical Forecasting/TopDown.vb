Public Class TopDown
    Inherits HierachicalForecasting

    Private HistoricalWeights As Boolean

#Region "Constructors"
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal Tree As ForecastTree, ByVal Horizon As Integer, Optional ByVal WeightsAreHistorical As Boolean = True)
        MyBase.New(Tree, Horizon)
        HistoricalWeights = WeightsAreHistorical
    End Sub

    Public Sub New(ByVal Tree As ForecastTree, ByVal Horizon As Integer, ByVal Models As List(Of Forecast), Optional ByVal WeightsAreHistorical As Boolean = True)
        MyBase.New(Tree, Horizon, Models)
        HistoricalWeights = WeightsAreHistorical
    End Sub


#End Region

#Region "Public Functions"
    Public Sub EnableHistoricalWeights()
        HistoricalWeights = True
    End Sub

    Public Sub EnableHorizontalWeights()
        HistoricalWeights = False
    End Sub

    Public Sub Calculate()
        InitializeForecastTree(_HierarchicalTree)

        'Dim TopAnalysis As New ForecastingAnalysis(_HierarchicalTree.Original)
        'For Each fm In _ForecastModels
        '    fm.Horizon = _Horizon
        'Next
        'TopAnalysis.ForecastModels = CloneForecastModels(_ForecastModels)
        '_HierarchicalTree.Forecast = TopAnalysis.CalculateBest

        MakeForecast(_HierarchicalTree)

        If HistoricalWeights Then
            CalculateHistorical(_HierarchicalTree)
        Else
            CalculateHorizontal(_HierarchicalTree)
        End If

    End Sub

#End Region

#Region "Private Methods"

    Private Function InitializeForecastTree(ByVal Node As ForecastTree) As Timeseries
        If Node.Count > 0 Then

            Dim Flag = True
            For Each Child In Node
                If Flag Then
                    Node.Original = InitializeForecastTree(Child.Value)
                    Flag = False
                Else
                    Node.Original += InitializeForecastTree(Child.Value)
                End If
            Next

        End If

        Return Node.Original
    End Function


    Private Sub CalculateHistorical(ByVal Node As ForecastTree)
        If Node.Count > 0 Then
            Dim MyAverage As Double = Node.Original.Average

            For Each Child In Node.Values
                Child.Forecast = Node.Forecast * (Child.Original.Average / MyAverage)
                'Dim NewForecastTS = Node.Forecast * (Child.Original.Average / MyAverage)

                'Child.Forecast.DataPoints = NewForecastTS.DataPoints
                'Child.Forecast.Frequency = NewForecastTS.Frequency
                'Child.Forecast.Offset = NewForecastTS.Offset

                CalculateHistorical(Child)
            Next
        End If
    End Sub

    Private Sub CalculateHorizontal(ByVal Node As ForecastTree)
        If Node.Count > 0 Then
            Dim SumTimeseries As Timeseries = DummyTimeseries.Zeroes(Node.Forecast.Length, Node.Forecast.Frequency, Node.Forecast.Offset)


            For Each Child In Node.Values
                'Dim ChildAnalysis As New ForecastingAnalysis(Child.Original) With {.ForecastModels = CloneForecastModels(_ForecastModels)}
                'Child.Forecast = ChildAnalysis.CalculateBest
                If Child.Forecast Is Nothing OrElse Child.Forecast.Length < _Horizon Then
                    MakeForecast(Child)
                End If
                SumTimeseries += Child.Forecast
            Next

            For Each Child In Node.Values
                Dim NewForecastTS = Child.Forecast * Node.Forecast / SumTimeseries
                Child.Forecast.DataPoints = NewForecastTS.DataPoints
                Child.Forecast.Frequency = NewForecastTS.Frequency
                Child.Forecast.Offset = NewForecastTS.Offset

                CalculateHorizontal(Child)
            Next

        End If
    End Sub

    'Private Function CloneForecastModels(ByVal FMs As List(Of Forecast)) As List(Of Forecast)
    '    CloneForecastModels = New List(Of Forecast)
    '    For Each fm In FMs
    '        CloneForecastModels.Add(fm.Clone)
    '    Next
    'End Function

#End Region
End Class
