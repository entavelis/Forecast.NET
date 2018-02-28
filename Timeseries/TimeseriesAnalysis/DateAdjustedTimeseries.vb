Public Class DateAdjustedTimeseries
    Inherits AnalysisTimeseriesBaseClass

    Public Property WorkingDays As Timeseries

#Region "Constructors"

    Public Sub New()
        MyBase.New()
        _Original = New Timeseries
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        MyBase.New(Input)
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal WorkingDays As Timeseries)
        MyBase.New(Input)
        _WorkingDays = WorkingDays
    End Sub
#End Region

    Public Overrides Sub Calculate()
        CalculateDateAdjustments()
    End Sub

    Private Sub CalculateDateAdjustments()
        _DataPoints = ((_WorkingDays / _WorkingDays.Average) * _Original).DataPoints
        _Offset = _Original.Offset
        _Frequency = _Original.Frequency
    End Sub
End Class
