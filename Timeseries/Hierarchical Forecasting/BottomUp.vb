Public Class BottomUp
    Inherits HierachicalForecasting

#Region "Constructors"

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal Tree As ForecastTree, ByVal Horizon As Integer)
        MyBase.New(Tree, Horizon)
    End Sub

    Public Sub New(ByVal Tree As ForecastTree, ByVal Horizon As Integer, ByVal Models As List(Of Forecast))
        MyBase.New(Tree, Horizon, Models)
    End Sub


#End Region
#Region "Public Sub"
    Public Sub Calculate()
        InitializeForecastTree(_HierarchicalTree)
        CalculateBottomUp(_HierarchicalTree)
    End Sub
#End Region

#Region "Private Sub"
    'Private Function InitializeForecastTree(ByVal Node As ForecastTree) As Forecast
    '    If Node.Count > 0 Then

    '        Dim BestFc As Forecast
    '        Dim minHorizon As Integer = Integer.MaxValue
    '        For Each Child In Node
    '            Dim TempFC = InitializeForecastTree(Child.Value)
    '            If TempFC.Horizon < minHorizon Then
    '                BestFc = TempFC
    '                minHorizon = TempFC.Horizon
    '            End If
    '        Next

    '        Node.Forecast = ForecastFactory.GetModel(BestFc.MyType)
    '        Dim DummyTS = DummyTimeseries.Zeroes(BestFc.Length, BestFc.Frequency, BestFc.Offset)
    '        Node.Forecast.DataPoints = DummyTS.DataPoints
    '        Node.Forecast.Frequency = DummyTS.Frequency
    '        Node.Forecast.Offset = DummyTS.Offset
    '    Else
    '        If Node.Forecast Is Nothing Then
    '            MakeForecast(Node)
    '        End If
    '    End If
    '    Return Node.Forecast

    'End Function
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

    Private Function CalculateBottomUp(ByVal Node As ForecastTree) As Timeseries
        If Node.Count > 0 Then

            'Dim SumTS As New Timeseries(Node.Forecast)
            For Each Child In Node
                If Node.Forecast Is Nothing Then
                    Node.Forecast = CalculateBottomUp(Child.Value)
                Else
                    Node.Forecast += CalculateBottomUp(Child.Value)
                End If
            Next

            'Node.Forecast.DataPoints = SumTS.DataPoints
            'Node.Forecast.Frequency = SumTS.Frequency
            'Node.Forecast.Offset = SumTS.Offset


        Else
            If Node.Forecast Is Nothing Then
                MakeForecast(Node)
            Else
                If Node.Forecast.Length < _Horizon Then
                    MakeForecast(Node)
                Else
                    Node.Forecast = Node.Forecast.SubTimeseries(Horizon)
                End If
            End If

        End If

        Return Node.Forecast
    End Function

#End Region

End Class
