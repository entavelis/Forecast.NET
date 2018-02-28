Public Class ForecastTree
    Inherits Dictionary(Of Integer, ForecastTree)

    Public Property Original As Timeseries

    Public Property Forecast As Timeseries

End Class

'Public Class HistoricTopDownTree
'    Inherits ForecastTree

'    Public Property Weight As Double
'End Class

'Public Class HorizonTopDownTree
'    Inherits ForecastTree

'    Public Property Weight() As Timeseries
'End Class