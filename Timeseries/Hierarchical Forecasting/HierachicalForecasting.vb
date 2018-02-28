Public Class HierachicalForecasting

#Region "Variables"

    Protected _HierarchicalTree As ForecastTree

    Protected _ForecastModels

    Protected _Horizon As Integer
#End Region

#Region "Properties"
    ReadOnly Property HierarchicalTree As ForecastTree
        Get
            Return _HierarchicalTree
        End Get
    End Property

    Property ForecastModels As List(Of Forecast)
        Get
            Return _ForecastModels
        End Get
        Set(value As List(Of Forecast))
            _ForecastModels = value
        End Set
    End Property

    Property Horizon As Integer
        Get
            Return _Horizon
        End Get
        Set(value As Integer)
            _Horizon = value
        End Set
    End Property
#End Region

#Region "Constructors"
    Public Sub New()
        _HierarchicalTree = New ForecastTree
        _ForecastModels = New List(Of Forecast)
    End Sub

    Public Sub New(ByVal Tree As ForecastTree, ByVal Horizon As Integer)
        _HierarchicalTree = Tree
        _Horizon = Horizon
        _ForecastModels = New List(Of Forecast)
    End Sub

    Public Sub New(ByVal Tree As ForecastTree, ByVal Horizon As Integer, ByVal Models As List(Of Forecast))
        _HierarchicalTree = Tree
        _Horizon = Horizon
        _ForecastModels = Models
    End Sub

#End Region

#Region "Public Functions"


#End Region

#Region "Private Functions"

    Protected Sub MakeForecast(ByVal ForecastNode As ForecastTree, Optional ByVal TryDeseasonalise As Boolean = True)
        Dim Original = ForecastNode.Original

        Dim TempAnalysis = New ForecastingAnalysis(Original, _Horizon)
        TempAnalysis.ForecastModels = _ForecastModels
        If TryDeseasonalise AndAlso Original.HasSeasonalityBehavior Then
            TempAnalysis.Deseasonalised = Decomposition.SimpleDecomposition.GetSeasonality(Original)
        End If
        ForecastNode.Forecast = TempAnalysis.CalculateBest.OutOfSample
    End Sub

#End Region
End Class
