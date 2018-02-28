Public Class Monitoring

#Region "PrivateVariables"

    Private _originalTS As Timeseries
    Private _forecastTS As Timeseries
    Private _fcPointsValid As Long 'datapoints for which original data exist
    Private _errorTS As Timeseries
    Dim _monitorStartOffset As Long
    Private _a As Double
    Private _b As Double

#End Region

#Region "Properties"

    Public Property OriginalTS As Timeseries
        Get
            Return _originalTS
        End Get
        Set(value As Timeseries)
            _originalTS = value
        End Set
    End Property

    Public Property ForecastTS As Timeseries
        Get
            Return _forecastTS
        End Get
        Set(value As Timeseries)
            _forecastTS = value
        End Set
    End Property

    Public Property ErrorTS As Timeseries
        Get
            Return _errorTS
        End Get
        Set(value As Timeseries)
            _errorTS = value
        End Set
    End Property

    Public Property a As Double
        Get
            Return _a
        End Get
        Set(value As Double)
            _a = value
        End Set
    End Property

    Public Property b As Double
        Get
            Return _b
        End Get
        Set(value As Double)
            _b = value
        End Set
    End Property

    Public Property fcPointsValid As Long
        Get
            Return _fcPointsValid
        End Get
        Set(value As Long)
            _fcPointsValid = value
        End Set
    End Property

    Public Property monitorStartOffset As Long
        Get
            Return _monitorStartOffset
        End Get
        Set(value As Long)
            _monitorStartOffset = value
        End Set
    End Property

#End Region

#Region "Constructors"

    Public Sub New()
    End Sub

    Public Sub New(ByVal originalTS As Timeseries, ByVal forecastTS As Timeseries, ByVal monitorStartOffset As Long, ByVal forecastPoints As Long, ByVal a As Double, ByVal b As Double)
        _originalTS = originalTS
        _forecastTS = forecastTS
        _monitorStartOffset = monitorStartOffset
        _fcPointsValid = forecastPoints
        _errorTS = Errors.e(_originalTS, _forecastTS)
        _a = a
        _b = b
    End Sub

#End Region

#Region "PrivateMethods"

    Private Function CUSUM(ByVal k As Integer, ByVal n As Integer) As Double
        Dim sum As Double = 0
        For t As Integer = n To (n - k + 1)
            sum += _errorTS.DataPoints(t - 1)
        Next t

        Return (sum)
    End Function

    Private Function E(ByVal n As Integer) As Double
        Dim sum As Double = 0
        For t As Integer = _monitorStartOffset To n
            sum += _a * ((1 - _a) ^ (n - t)) * _errorTS.DataPoints(t)
        Next t

        Return (sum)
    End Function

    Private Function e0() As Double
        Dim sum As Double = 0
        For t As Integer = _monitorStartOffset To _monitorStartOffset + 5
            sum += Math.Abs(_errorTS.DataPoints(t))
        Next t

        Return (sum / 6)
    End Function

    Private Function MAD(ByVal n As Integer) As Double
        Dim sum As Double = 0
        For t As Integer = _monitorStartOffset To n
            sum += _b * ((1 - _b) ^ (n - t)) * Math.Abs(_errorTS.DataPoints(t))
        Next t

        Return (((1 - b) ^ n) * e0() + sum)
    End Function

#End Region

#Region "PublicMethods"

    Public Function Brown(ByVal k As Integer, ByVal n As Integer)
        Return (Math.Abs(CUSUM(k, n) / MAD(n)))
    End Function

    Public Function Trigg() As Timeseries
        If _fcPointsValid < 6 Then
            ' Throw New MonitorException("Monitoring: There must be at least 6 forecast points with their corresponding original data filled in")
        End If

        Dim monitorLength As Integer = _fcPointsValid
        'Dim monitorOffset = _originalTS.Length - _fcPointsValid + 1
        Dim monitorTS As Timeseries = New Timeseries(monitorLength, _originalTS.Frequency, _monitorStartOffset)
        For n As Integer = _monitorStartOffset To (_monitorStartOffset + _fcPointsValid - 1)
            monitorTS.DataPoints(n - _monitorStartOffset) = Math.Abs(E(n) / MAD(n))
        Next n
        Return monitorTS
    End Function

#End Region

End Class
