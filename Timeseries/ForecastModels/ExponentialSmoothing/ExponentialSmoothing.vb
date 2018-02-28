Public MustInherit Class ExponentialSmoothing
    Inherits Forecast

#Region "Constructors"
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        MyBase.New(Input)
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double)
        MyBase.New(Input, Horizon)
    End Sub

    Public Sub New(ByVal Input As Timeseries,ByVal ErrorMethod as Errors.ErrorMEthodForOptimization)
        MyBase.New(Input,ErrorMethod)
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double,ByVal ErrorMethod as Errors.ErrorMEthodForOptimization)
        MyBase.New(Input, Horizon,ErrorMethod)
    End Sub


#End Region



    Protected Function CalculateForecastTimeseries(ByVal horizon As Integer, ByVal S0 As Double, ByVal T0 As Double, ByVal a As Double, ByVal b As Double, ByVal phi As Double) As Timeseries
        If (horizon < 0) OrElse (a < 0) OrElse (a > 1) OrElse (b > 1) OrElse (b < 0) OrElse (phi < 0) Then Throw New InvalidParameterException("Horizon must be a non negative integer, phi a non negative decimal number and both a and b must be between zero and one")

        Dim Forecast(_Original.Length + horizon - 1) As Double
        Dim LastS As Double = S0
        Dim LastT As Double = T0 * phi

        For i As Integer = 0 To _Original.Length - 1
            Forecast(i) = LastS + LastT
            LastS = LastS + LastT + a * (_Original.DataPoints(i) - Forecast(i))
            LastT = phi * (LastT + a * b * (_Original.DataPoints(i) - Forecast(i)))
        Next

        If horizon <> 0 Then
            Forecast(_Original.Length) = LastS + LastT
            For i As Integer = _Original.Length + 1 To _Original.Length + horizon - 1
                LastT *= phi
                Forecast(i) = Forecast(i - 1) + LastT
            Next

        End If

        Return New Timeseries(Forecast, _Original.Frequency, _Original.Offset)
    End Function

    Protected Sub CalculateForecast(ByVal Parameters As Tuple(Of Double, Double, Double, Double, Double))

        Dim Result = CalculateForecastTimeseries(_Horizon, Parameters.Item1, Parameters.Item2, Parameters.Item3, Parameters.Item4, Parameters.Item5)
        _DataPoints = Result.DataPoints
        _Frequency = Result.Frequency
        _Offset = Result.Offset

    End Sub

    Protected Function Optimization(ByVal ErrorMethod As Errors.ErrorMethodForOptimization, ByVal S0 As Double, ByVal T0 As Double, Optional ByVal a As Double = -1, Optional ByVal b As Double = -1, Optional ByVal phi As Double = -1) As Tuple(Of Double, Double, Double, Double, Double)
        If Double.IsNaN(S0) Then S0 = LRL.GetA(_Original)
        If Double.IsNaN(T0) Then T0 = LRL.GetB(_Original)
        If ErrorMethod = Nothing Then ErrorMethod = AddressOf Errors.MSE

        Dim flaga, flagb, flagphi As Boolean
        Dim BestAlpha, BestBeta, BestPhi, BestError, tempError As Double
        BestError = Double.MaxValue
        Dim _a, _b, _phi As Integer

        Dim NoOfLoops As Integer = 100

        If a = -1 Then
            flaga = True
            _a = 0
        Else
            flaga = False
            _a = Math.Round(NoOfLoops * a)
        End If

        If b = -1 Then
            flagb = True
            _b = 0
        Else
            flagb = False
            _b = Math.Round(NoOfLoops * b)
        End If

        If phi = -1 Then
            flagphi = True
            _phi = 0
        Else
            flagphi = False
            _phi = Math.Round(NoOfLoops * phi)
        End If


        Do
            If flagb Then _b = 0
            Do
                If flagphi Then _phi = 1
                Do
                    tempError = ErrorMethod.Invoke(_Original, CalculateForecastTimeseries(0, S0, T0, _a / NoOfLoops, _b / NoOfLoops, _phi / NoOfLoops))
                    If tempError < BestError Then
                        BestError = tempError
                        BestAlpha = _a / NoOfLoops
                        BestBeta = _b / NoOfLoops
                        BestPhi = _phi / NoOfLoops
                    End If
                    If flagphi Then
                        _phi += 1
                    Else : Exit Do
                    End If
                Loop While _phi <= NoOfLoops - 1
                If flagb Then
                    _b += 1
                Else : Exit Do
                End If
            Loop While _b <= NoOfLoops

            If flaga Then
                _a += 1
            Else : Exit Do
            End If
        Loop While _a <= NoOfLoops

        Return Tuple.Create(S0, T0, BestAlpha, BestBeta, BestPhi)

    End Function


End Class
