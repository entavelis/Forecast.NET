
Public Class DecomposedTimeseries

#Region "Variables"
    Private TheDecomp As Decomposition.SimpleDecomposition
#End Region

#Region "Properties"
    Public ReadOnly Property Seasonality As Timeseries
        Get
            Return TheDecomp.Seasononality
        End Get
    End Property

    Public ReadOnly Property Circularity As Timeseries
        Get
            Return TheDecomp.Circularity
        End Get
    End Property

    Public ReadOnly Property Trend As Timeseries
        Get
            Return TheDecomp.Trend
        End Get
    End Property

    Public ReadOnly Property Randomness As Timeseries
        Get
            Return TheDecomp.Randomness
        End Get
    End Property

    Public ReadOnly Property SeasonalityIndices As Timeseries
        Get
            Return TheDecomp.SeasonalIndices
        End Get
    End Property

    Public ReadOnly Property Deseasonalised As Timeseries
        Get
            Return _Original / TheDecomp.Seasononality
        End Get
    End Property

    Public Property Original As Timeseries

#End Region

#Region "Constructors"
    Public Sub New()
        _Original = New Timeseries
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        _Original = Input
    End Sub
#End Region

#Region "Public Functions"

    Public Sub Decompose()
        TheDecomp = New Decomposition.SimpleDecomposition(_Original)
    End Sub

    Public Function Deseasonalise() As Timeseries
        Return Decomposition.SimpleDecomposition.GetSeasonality(_Original) * _Original
    End Function
#End Region


#Region "Private Functions"


#End Region
End Class
