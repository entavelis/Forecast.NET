' ******************************************~V~*******************************************
'
' Description: A class of basic statistical methods, mostly moving averages
' Author: Evangelos Ntavelis
' Email: vaggelis@fsu.gr
' Date: 7/8/2014
'
' ******************************************~V~*******************************************

Public Class StatisticalMethods

#Region "Moving Averages"

    Public Shared Function MA(ByVal Input As Timeseries, ByVal n As Integer) As Timeseries
        If Input.Length < n Then Throw New InvalidParameterException("Moving Average Input's length must be larger than n")

        Dim DataArray(Input.Length - n) As Double

        For i As Integer = 0 To Input.Length - n
            DataArray(i) = Input.SubAverage(n, i)
        Next

        Return New Timeseries(DataArray, Input.Frequency, Input.Offset + n \ 2)

    End Function

    ''' <summary>
    ''' Simple Moving Average
    ''' </summary>
    ''' <param name="Input">The Timeseries we want to smooth</param>
    ''' <param name="n">The length parameter</param>
    ''' <returns>The smoothed Timeseries</returns>
    ''' <remarks></remarks>
    Public Shared Function SMA(ByVal Input As Timeseries, ByVal n As Integer) As Timeseries
        If n Mod 2 = 0 Then Throw New InvalidParameterException("N must be an odd number")
        Return MA(Input, n)
    End Function

    ''' <summary>
    ''' Double Moving Average
    ''' </summary>
    ''' <param name="Input">The Timeseries we want to smooth</param>
    ''' <param name="n">The first length parameter</param>
    ''' <param name="m">The second length parameter</param>
    ''' <returns>The smoothed Timeseries</returns>
    ''' <remarks></remarks>
    Public Shared Function DMA(ByVal Input As Timeseries, ByVal n As Integer, ByVal m As Integer) As Timeseries
        If (n Mod 2) * (m Mod 2) = 0 Then Throw New InvalidParameterException("Both m and n must be odd numbers")
        Return MA(MA(Input, m), n)
    End Function

    ''' <summary>
    ''' Centered Moving Average
    ''' </summary>
    ''' <param name="Input">The Timeseries we want to smooth</param>
    ''' <param name="n">The length parameter</param>
    ''' <returns>The smoothed Timeseries</returns>
    ''' <remarks></remarks>
    Public Shared Function CSMA(ByVal Input As Timeseries, ByVal n As Integer) As Timeseries
        If n Mod 2 = 1 Then Throw New InvalidParameterException("N must be an even number")

        Dim DataArray(Input.Length - n - 1) As Double

        For i As Integer = 0 To Input.Length - n - 1
            DataArray(i) = (Input.SubAverage(n, i) + Input.SubAverage(n, i + 1)) / 2
        Next

        Return New Timeseries(DataArray, Input.Frequency, Input.Offset + n \ 2)
    End Function

    ' ''' <summary>
    ' ''' Weighted Moving Average
    ' ''' </summary>
    ' ''' <param name="Input">The Timeseries we want to smooth</param>
    ' ''' <param name="n">The length parameter</param>
    ' ''' <returns>The smoothed Timeseries</returns>
    ' ''' <remarks></remarks>
    'Public Shared Function WMA(ByVal Input As Timeseries, ByVal n As Integer) As Timeseries

    'End Function

#End Region

    '#Region "ADIDA Related"
    '    Public Enum WeightMethod
    '        EqualWeights
    '        PreviousWeights
    '        AverageWeights
    '    End Enum

    '    ''' <summary>
    '    ''' Aggregation of the input timeseries based on the input level
    '    ''' </summary>
    '    ''' <param name="Original">The input timeseries we want to aggregate</param>
    '    ''' <param name="level">The level of aggregation</param>
    '    ''' <returns>The aggregated timeseries</returns>
    '    ''' <remarks></remarks>
    '    Public Shared Function Aggregation(ByVal Original As Timeseries, ByVal level As Integer) As Timeseries
    '        Dim Result(Original.Length \ level) As Double
    '        Dim OffSet As Integer = Original.Length Mod level
    '        For i As Integer = 1 To Original.Length \ level
    '            Result(i) = Original.SubAverage(level, (i - 1) * level + OffSet + 1)
    '        Next
    '        Return New Timeseries(Result, Original.Frequency \ level, Original.Offset + OffSet)
    '    End Function

    '    ''' <summary>
    '    ''' Disaggregation of the input timeseries based on the input level
    '    ''' </summary>
    '    ''' <param name="Aggregated">The input timeseries we want to disaggregate</param>
    '    ''' <param name="level">The level of disaggregation</param>
    '    ''' <returns>The disaggregated timeseries</returns>
    '    ''' <remarks></remarks>
    '    Public Shared Function Disaggregation(ByVal Aggregated As Timeseries, ByVal level As Integer) As Timeseries
    '        Dim Result(Aggregated.Length * level) As Double
    '        For i As Integer = 1 To Aggregated.Length * level
    '            Result(i) = Aggregated.DataPoints((i - 1) \ level + 1) / level
    '        Next
    '        Return New Timeseries(Result, Aggregated.Frequency * level, Aggregated.Offset)
    '    End Function

    '    ''' <summary>
    '    ''' Disaggregation of the input timeseries based on the input level, the original timeseries and the demarcation method we choose.
    '    ''' </summary>
    '    ''' <param name="Aggregated">The input timeseries we want to disaggregate</param>
    '    ''' <param name="level">The level of disaggregation</param>
    '    ''' <param name="Original">The original pre-aggregated timeseries</param>
    '    ''' <param name="WeightMeth">The demarcation method</param>
    '    ''' <returns>The disaggregated timeseries</returns>
    '    ''' <remarks></remarks>
    '    Public Shared Function Disaggregation(ByVal Aggregated As Timeseries, ByVal level As Integer, ByVal Original As Timeseries, ByVal WeightMeth As WeightMethod) As Timeseries
    '        Dim Weights(level) As Double

    '        Select Case WeightMeth
    '            Case WeightMethod.EqualWeights
    '                For i As Integer = 1 To level
    '                    Weights(i) = 1 / level
    '                Next
    '            Case WeightMethod.PreviousWeights
    '                Dim Sum As Double = Original.SubSum(level, Original.Length - level + 1)
    '                For i As Integer = 1 To level
    '                    Weights(i) = Original.DataPoints(Original.Length - level + i) / Sum
    '                Next
    '            Case WeightMethod.AverageWeights
    '                Dim Sum As Double = Original.Sum
    '                For i As Integer = 1 To level
    '                    Weights(i) = 0
    '                    For j As Integer = 1 To Aggregated.Length
    '                        Weights(i) += Original.DataPoints(Original.Length - (j - 1) - (level - i))
    '                    Next
    '                    Weights(i) /= Sum
    '                Next
    '        End Select

    '        Dim Result(Aggregated.Length * level) As Double
    '        For i As Integer = 1 To Aggregated.Length * level
    '            Result(i) = Aggregated.DataPoints((i - 1) \ level + 1) * Weights((i - 1) Mod level + 1)
    '        Next
    '        Return New Timeseries(Result, Aggregated.Frequency * level, Aggregated.Offset)
    '    End Function
    '#End Region
End Class
