' ******************************************~V~*******************************************
'
' Description: A class containing the methods needed for the procedure of a timeseries
' decomposition
' Author: Evangelos Ntavelis
' Email: vaggelis@fsu.gr
' Date: 25/7/2014
'
' ******************************************~V~*******************************************

Imports Timeseries.StatisticalMethods

Public Class Decomposition
    Public Class SimpleDecomposition
        Private T As Timeseries
        Private C As Timeseries
        Private S As Timeseries
        Private R As Timeseries
        Private SIndices As Timeseries

        ''' <returns>The trend component of the timeseries</returns>
        Public ReadOnly Property Trend As Timeseries
            Get
                Return T
            End Get
        End Property
        ''' <returns>The circle component of the timeseries</returns>
        Public ReadOnly Property Circularity As Timeseries
            Get
                Return C
            End Get
        End Property
        ''' <returns>The seasonality component of the timeseries</returns>
        Public ReadOnly Property Seasononality As Timeseries
            Get
                Return S
            End Get
        End Property
        ''' <returns>The randomness component of the timeseries</returns>
        Public ReadOnly Property Randomness As Timeseries
            Get
                Return S
            End Get
        End Property
        ''' <returns>The seasonal indices of the timeseries</returns>
        Public ReadOnly Property SeasonalIndices As Timeseries
            Get
                Return SIndices
            End Get
        End Property

        ''' <summary>
        ''' Initializes a new timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries for which the New sub is called</param>
        Public Sub New(ByVal Original As Timeseries)
            Initializer(Original, Original.Frequency)
        End Sub
        ''' <summary>
        ''' Initializes a new timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries for which the New sub is called</param>
        ''' <param name="n">The length of the Moving Average to be calculated</param>
        Public Sub New(ByVal Original As Timeseries, ByVal n As Integer)
            Initializer(Original, n)
        End Sub
        ''' <summary>
        ''' Decomposes the Original timeseries into its Trend,Circle,Seasonality and Randomness components
        ''' </summary>
        ''' <param name="Original">The Original timeseries to be decomposed</param>
        ''' <param name="n">The length of the Moving Average to be calculated</param>
        Private Sub Initializer(ByVal Original As Timeseries, ByVal n As Integer)
            Dim TxC As Timeseries
            If n Mod 2 = 1 Then
                TxC = SMA(Original, n)
            Else
                TxC = CSMA(Original, n)
            End If

            Dim SxR As Timeseries = GetSxR(Original, TxC)

            SIndices = GetSIndices(SxR)

            S = GetS(SIndices, Original.Length, Original.Offset)

            Dim TxCxR As Timeseries = GetTxCxR(Original, S)


            R = GetR(TxCxR, DMA(Original, 3, 3))

            T = GetT(TxC)

            C = GetC(TxC, T)
        End Sub
        ''' <summary>
        ''' Creates an enumeration of the different Moving Average types
        ''' </summary>
        Enum MovingAverage
            SMA
            DMA
            CSMA
            ' WMA
        End Enum

        ''' <summary>
        ''' Calculates the requested Moving Average of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which a Moving Average will be calculated</param>
        ''' <param name="MA">The type of Moving Average to be calculated</param>
        ''' <param name="n">The length of the Moving Average to be calculated</param>
        ''' <param name="m">The secondary length of the Moving Average to be calculated</param>
        ''' <returns>The timeseries whose time-points are the values of the requested Moving Average of the Original timeseries</returns>
        Private Shared Function GetMovingAverage(ByVal Original As Timeseries, ByVal MA As MovingAverage, ByVal n As Integer, Optional m As Integer = -1) As Timeseries
            Select Case MA
                Case MovingAverage.SMA
                    Return SMA(Original, n)
                Case MovingAverage.DMA
                    Return DMA(Original, n, m)
                Case MovingAverage.CSMA
                    Return CSMA(Original, n)
                    '   Case MovingAverage.WMA
                    ' return WMA(Original,n)
            End Select

            Return Nothing ' In case of wrong input
        End Function
        ''' <summary>
        ''' Calculates the timeseries whose time-points are the seasonality ratios of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The Original timeseries from which the seasonality ratios will be calculated</param>
        ''' <param name="TxC">The timeseries whose time-points are the product of the Trend and Circle components of the timeseries</param>
        ''' <returns>The timeseries whose time-points are the seasonality ratios of the Original timeseries</returns>
        Private Shared Function GetSxR(ByVal Original As Timeseries, ByVal TxC As Timeseries) As Timeseries
            Return (Original / TxC)
        End Function
        ''' <summary>
        ''' Calculates the corresponding deseasonalised timeseries of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The Original timeseries from which the deseasonalised timeseries will be calculated</param>
        ''' <param name="S">Th Seasonality component of the Original timeseries</param>
        ''' <returns>The corresponding deseasonalised timeseries of the Original timeseries</returns>
        Private Shared Function GetTxCxR(ByVal Original As Timeseries, ByVal S As Timeseries) As Timeseries
            Return (Original / S)
        End Function
        ''' <summary>
        ''' Calculates the Randomness component of a deseasonalised timeseries
        ''' </summary>
        ''' <param name="TxCxR">The deseasonalised timeseries</param>
        ''' <param name="TxC">The timeseries whose time-points are the product of the Trend and Circle components of an original timeseries</param>
        ''' <returns>The Randomness component of the deseasonalised timeseries</returns>
        Private Shared Function GetR(ByVal TxCxR As Timeseries, ByVal TxC As Timeseries) As Timeseries
            Return (TxCxR / TxC)
        End Function
        ''' <summary>
        ''' Calculates the Randomness component of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Randomness component will be calculated</param>
        ''' <returns>The Randomness component of the Original timeseries</returns>
        Public Shared Function GetRandomness(ByVal Original As Timeseries) As Timeseries
            Return GetRandomness(Original, Original.Frequency, Original.Frequency)
        End Function

        ''' <summary>
        ''' Calculates the Randomness component of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The Original timeseries of which the Randomness component will be calculated</param>
        ''' <param name="Seasons">The length of seasonality of the Original timeseries</param>
        ''' <returns>The Randomness component of the Original timeseries</returns>
        Public Shared Function GetRandomness(ByVal Original As Timeseries, ByVal Seasons As Integer) As Timeseries
            Return GetRandomness(Original, Seasons, Seasons)
        End Function
        ''' <summary>
        ''' Calculates the Randomness component of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Randomness component will be calculated </param>
        ''' <param name="Seasons">The length of seasonality of the Original timeseries</param>
        ''' <param name="MovAvrLen">The length of the Moving Average to be calculated</param>
        ''' <returns>The Randomness component of the Original timeseries</returns>
        Public Shared Function GetRandomness(ByVal Original As Timeseries, ByVal Seasons As Integer, ByVal MovAvrLen As Integer) As Timeseries
            Dim TrendXCircle As Timeseries
            If (MovAvrLen Mod 2 = 1) Then
                TrendXCircle = SMA(Original, MovAvrLen)
            Else
                TrendXCircle = CSMA(Original, MovAvrLen)
            End If

            Return GetR(GetTxCxR(Original, GetS(GetSIndices(GetSxR(Original, TrendXCircle), Seasons), Original.Length, Original.Offset)), DMA(Original, 3, 3))
        End Function
        ''' <summary>
        ''' Calculates the Trend component of the timeseries
        ''' </summary>
        ''' <param name="TxC">The timeseries whose time-points are the product of the Trend and Circle components of an original timeseries</param>
        ''' <returns>The Trend component of the timeseries</returns>
        Private Shared Function GetT(ByVal TxC As Timeseries) As Timeseries ' LRL          
            Dim DataArray(TxC.Length) As Double
            For i As Integer = 1 To TxC.Length
                DataArray(i) = i
            Next

            Dim t As New Timeseries(DataArray, TxC.Frequency, TxC.Offset)

            Dim b As Double = ((TxC * t).Average - t.Average * TxC.Average) / ((t ^ 2).Average - (t.Average) ^ 2)
            Dim a As Double = TxC.Average - b * t.Average

            Return a + b * t
        End Function
        ''' <summary>
        ''' Calculates the Trend component of the timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Trend component will be calculated</param>
        ''' <returns>The Trend component of the timeseries</returns>
        Public Shared Function GetTrend(ByVal Original As Timeseries) As Timeseries
            Return GetTrend(Original, Original.Frequency)
        End Function
        ''' <summary>
        ''' Calculates the Trend component of the timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Trend component will be calculated</param>
        ''' <param name="MovAvrLen">The length of the Moving Average to be calculated</param>
        ''' <returns>The Trend component of the timeseries</returns>
        Public Shared Function GetTrend(ByVal Original As Timeseries, ByVal MovAvrLen As Integer) As Timeseries
            Dim TrendXCircle As Timeseries
            If (MovAvrLen Mod 2 = 1) Then
                TrendXCircle = SMA(Original, MovAvrLen)
            Else
                TrendXCircle = CSMA(Original, MovAvrLen)
            End If

            Return GetT(TrendXCircle)
        End Function
        ''' <summary>
        ''' Calculates the Circle component of the timeseries
        ''' </summary>
        ''' <param name="TxC">The timeseries whose time-points are the product of the Trend and Circle components of an original timeseries</param>
        ''' <param name="T">The Trend component of an original timeseries</param>
        ''' <returns>The Circle component of the timeseries</returns>
        Private Shared Function GetC(ByVal TxC As Timeseries, ByVal T As Timeseries) As Timeseries
            Return TxC / T
        End Function
        ''' <summary>
        ''' Calculates the Circle component of the timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Circle component will be calculated</param>
        ''' <returns>The Circle component of the timeseries</returns>
        Public Shared Function GetCircle(ByVal Original As Timeseries) As Timeseries
            Return GetCircle(Original, Original.Frequency)
        End Function
        ''' <summary>
        ''' Calculates the Circle component of the timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Circle component will be calculated</param>
        ''' <param name="MovAvrLen">The length of the Moving Average to be calculated</param>
        ''' <returns>The Circle component of the timeseries</returns>
        Public Shared Function GetCircle(ByVal Original As Timeseries, ByVal MovAvrLen As Integer) As Timeseries
            Dim TrendXCircle As Timeseries
            If (MovAvrLen Mod 2 = 1) Then
                TrendXCircle = SMA(Original, MovAvrLen)
            Else
                TrendXCircle = CSMA(Original, MovAvrLen)
            End If

            Dim Trend As Timeseries = GetT(TrendXCircle)

            Return GetC(TrendXCircle, Trend)
        End Function
        ''' <summary>
        ''' Calculates the Seasonality Indices of a timeseries
        ''' </summary>
        ''' <param name="SI">The Seasonality Indices of a timeseries</param>
        ''' <param name="Length">The timeseries' length</param>
        ''' <param name="Offset">The timeseries' offset</param>
        ''' <returns>The Seasonality Indices of the timeseries</returns>
        Private Shared Function GetS(ByVal SI As Timeseries, ByVal Length As Integer, ByVal Offset As Integer) As Timeseries
            'Dim len As Integer = SI.Length
            'Dim temp As Timeseries = SI.UpdateOffset(SI.Offset + Length)
            'While temp.Offset <= (Offset + Length)
            '    SI &= temp
            '    temp = temp.UpdateOffset(temp.Offset + len)
            'End While

            Dim Result As Timeseries = DummyTimeseries.Ones(Length, SI.Frequency, Offset)
            Dim OffDiff As Integer = SI.Offset - Offset
            For i As Integer = 0 To Result.Length - 1
                Result.DataPoints(i) *= SI.DataPoints((i + OffDiff) Mod SI.Frequency)
            Next
            Return Result
        End Function
        ''' <summary>
        ''' Calculates the Seasonality Indices of the timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Seasonality Indices will be calculated</param>
        ''' <returns>The Seasonality Indices of the timeseries</returns>
        Public Shared Function GetSeasonalityIndices(ByVal Original As Timeseries) As Timeseries
            Return GetSeasonalityIndices(Original, Original.Frequency, Original.Frequency)
        End Function
        ''' <summary>
        ''' Calculates the Seasonality Indices of the timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Seasonality Indices will be calculated</param>
        ''' <param name="Seasons">The length of seasonality of the Original timeseries</param>
        ''' <returns>The Seasonality Indices of the timeseries</returns>
        Public Shared Function GetSeasonalityIndices(ByVal Original As Timeseries, ByVal Seasons As Integer) As Timeseries
            Return GetSeasonalityIndices(Original, Seasons, Seasons)
        End Function
        ''' <summary>
        ''' Calculates the Seasonality Indices of the timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Seasonality Indices will be calculated</param>
        ''' <param name="Seasons">The length of seasonality of the Original timeseries</param>
        ''' <param name="MovAvrLen">The length of the Moving Average to be calculated</param>
        ''' <returns>he Seasonality Indices of the timeseries</returns>
        Public Shared Function GetSeasonalityIndices(ByVal Original As Timeseries, ByVal Seasons As Integer, ByVal MovAvrLen As Integer) As Timeseries
            Dim TrendXCircle As Timeseries
            If (MovAvrLen Mod 2 = 1) Then
                TrendXCircle = SMA(Original, MovAvrLen)
            Else
                TrendXCircle = CSMA(Original, MovAvrLen)
            End If

            Dim SeasonalityXRandomness As Timeseries = GetSxR(Original, TrendXCircle)

            Return GetSIndices(SeasonalityXRandomness, Seasons)

        End Function
        ''' <summary>
        ''' Calculates the Seasonality component of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Seasonality component will be calculated</param>
        ''' <returns>The Seasonality component of the Original timeseries</returns>
        Public Shared Function GetSeasonality(ByVal Original As Timeseries) As Timeseries
            Return GetSeasonality(Original, Original.Frequency, Original.Frequency)
        End Function
        ''' <summary>
        ''' Calculates the Seasonality component of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Seasonality component will be calculated</param>
        ''' <param name="Seasons">The length of seasonality of the Original timeseries</param>
        ''' <returns>The Seasonality component of the Original timeseries</returns>
        Public Shared Function GetSeasonality(ByVal Original As Timeseries, ByVal Seasons As Integer) As Timeseries
            Return GetSeasonality(Original, Seasons, Seasons)
        End Function
        ''' <summary>
        ''' Calculates the Seasonality component of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Seasonality component will be calculated</param>
        ''' <param name="Seasons">The length of seasonality of the Original timeseries</param>
        ''' <param name="MovAvrLen">The length of the Moving Average to be calculated</param>
        ''' <returns>The Seasonality component of the Original timeseries</returns>
        Public Shared Function GetSeasonality(ByVal Original As Timeseries, ByVal Seasons As Integer, ByVal MovAvrLen As Integer)
            Return GetS(GetSeasonalityIndices(Original, Seasons, MovAvrLen), Original.Length, Original.Offset)
        End Function
        ''' <summary>
        ''' Calculates the Seasonality component of the Original timeseries
        ''' </summary>
        ''' <param name="Original">The timeseries of which the Seasonality component will be calculated</param>
        ''' <param name="Seasons">The length of seasonality of the Original timeseries</param>
        ''' <param name="MovAvrLen">The length of the Moving Average to be calculated</param>
        ''' <param name="Length">The length of the returned timeseries</param>
        ''' <returns>The Seasonality component of the Original timeseries</returns>
        Public Shared Function GetSeasonality(ByVal Original As Timeseries, ByVal Seasons As Integer, ByVal MovAvrLen As Integer, ByVal Length As Integer)
            Return GetS(GetSeasonalityIndices(Original, Seasons, MovAvrLen), Length, Original.Offset)
        End Function

        ''' <summary>
        ''' Calculates the Seasonality Indices of the timeseries
        ''' </summary>
        ''' <param name="SxR">The timeseries whose time-points are the Seasonality Ratios of an original timeseries</param>
        ''' <param name="seasons">The length of seasonality of the Original timeseries</param>
        ''' <returns>Calculates the Seasonality Indices of the timeseries</returns>
        Private Shared Function GetSIndices(ByVal SxR As Timeseries, Optional ByVal seasons As Integer = -1) As Timeseries
            If seasons = -1 Then seasons = SxR.Frequency


            Dim Result(seasons - 1) As Double
            Dim Counter As Integer
            Dim max As Double
            Dim min As Double
            Dim temp As Double

            For i As Integer = 0 To seasons - 1
                Dim index As Integer = i
                Result(i) = 0
                Counter = 0
                max = Double.MinValue
                min = Double.MaxValue

                While (index < SxR.Length)
                    Counter += 1
                    temp = SxR.DataPoints(index)
                    Result(i) += temp
                    If temp > max Then max = temp
                    If temp < min Then min = temp
                    index += seasons
                End While


                If Counter > 3 Then
                    Counter -= 2
                    Result(i) -= (min + max)
                End If

                Result(i) /= Counter
            Next


            Dim NormalizationRatio = Result.Sum / seasons


            Return New Timeseries(Result, SxR.Frequency, SxR.Offset) / NormalizationRatio
        End Function
    End Class



End Class