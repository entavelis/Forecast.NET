Public MustInherit Class AnalysisTimeseriesBaseClass
    Inherits Timeseries

    Protected _Original As Timeseries

    Public Overridable Property Original As Timeseries
        Get
            Return _Original
        End Get
        Set(value As Timeseries)
            _Original = value
        End Set
    End Property


    Public Sub New()
        _Original = New Timeseries
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        _Original = Input
    End Sub

    Public MustOverride Sub Calculate()

End Class
