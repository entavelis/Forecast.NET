
' ******************************************~V~*******************************************
'
' Description: The basic class of type timeseries, contain the basic structures and a 
' variety of operation upon them
' Author: Evangelos Ntavelis
' Email: vaggelis@fsu.gr
' Date: 25/7/2014
'
' ******************************************~V~*******************************************

Imports System.Math


Public Class Timeseries

#Region "Private Variables"
    Protected _DataPoints As Double()
    Protected _Offset As Integer
    Protected _Frequency As Integer

#End Region

#Region "Properties"

    Public Property DataPoints() As Double()
        Set(value As Double())
            _DataPoints = value
        End Set
        Get
            Return _DataPoints
        End Get
    End Property


    Public Property Offset As Integer
        Get
            Return _Offset
        End Get
        Set(value As Integer)
            _Offset = value
        End Set
    End Property


    Public Property Frequency As Integer
        Get
            Return _Frequency
        End Get
        Set(value As Integer)
            _Frequency = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the length of timeseries
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Length As Integer
        Get
            Return _DataPoints.Length
        End Get
    End Property


#End Region

#Region "Constructors"

    ''' <summary>
    ''' Defines a new timeseries
    ''' </summary>
    Public Sub New()
    End Sub


    ''' <summary>
    ''' Defines a new timeseries
    ''' </summary>
    Public Sub New(ByVal InputDataValues() As Double, Optional ByVal FrequencyValue As Integer = 1, Optional ByVal OffsetValue As Integer = 0)
        _DataPoints = InputDataValues
        _Offset = OffsetValue
        _Frequency = FrequencyValue
    End Sub

    ''' <summary>
    ''' Defines a new timeseries
    ''' </summary>
    Public Sub New(ByVal Length As Integer, Optional ByVal FrequencyValue As Integer = 1, Optional ByVal OffsetValue As Integer = 0)
        ReDim _DataPoints(Length - 1)
        _Offset = OffsetValue
        _Frequency = FrequencyValue
    End Sub


    Public Sub New(ByVal Input As Timeseries)
        _DataPoints = Input.DataPoints
        _Offset = Input.Offset
        _Frequency = Input.Frequency
    End Sub
#End Region

#Region "Operators Overload"
    ''' <summary>
    ''' adds a timeseries B to a timeseries A
    ''' </summary>
    ''' <param name="A">the first timeseries to be added</param>
    ''' <param name="B">the second timeseries to be added</param>
    ''' <returns>the timeseries whose time-points are the sum of the corresponding time-points of timeseries A and B</returns>
    ''' <remarks>if the timeseries have different lengths,then the new length is floored to the length of the shortest timeseries</remarks>
    Public Shared Operator +(ByVal A As Timeseries, ByVal B As Timeseries) As Timeseries

        If A._Frequency <> B._Frequency Then Throw New PropertiesMismatchException("The timeseries must have the same frequency")
        ' Needs clarification
        Dim Aoff As Integer = B._Offset - Min(A._Offset, B._Offset)
        Dim Boff As Integer = A._Offset - Min(A._Offset, B._Offset)
        Dim NewLength = Min(A.Length - Aoff, B.Length - Boff)
        Dim ResultDataValues(NewLength - 1) As Double

        For i As Integer = 0 To NewLength - 1
            ResultDataValues(i) = A._DataPoints(i + Aoff) + B._DataPoints(i + Boff)
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, Max(A._Offset, B._Offset))
    End Operator


    ''' <summary>
    ''' adds constant to a timeseries
    ''' </summary>
    ''' <param name="A">the timeseries to which the constant will be added</param>
    ''' <param name="b">the constant to be added to the timeseries</param>
    ''' <returns>the timeseries whose time-points are the sum of the corresponding time-points of timeseries A and constant b</returns>
    ''' <remarks></remarks>
    Public Shared Operator +(ByVal A As Timeseries, ByVal b As Double) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = A._DataPoints(i) + b
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator
    ''' <summary>
    ''' adds constant to a timeseries
    ''' </summary>
    ''' <param name="b">the constant to be added to the timeseries</param>
    ''' <param name="A">the timeseries to which the constant will be added</param>
    ''' <returns>the timeseries whose time-points are the sum of the corresponding time-points of timeseries A and constant b</returns>
    ''' <remarks></remarks>
    Public Shared Operator +(ByVal b As Double, ByVal A As Timeseries) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = A._DataPoints(i) + b
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator
    ''' <summary>
    ''' subtracts a timeseries B from a timeseries A
    ''' </summary>
    ''' <param name="A">the timeseries that represents the minuend of the subtraction</param>
    ''' <param name="B">the timeseries that represents the subtrahend of the subtraction</param>
    ''' <returns>the timeseries whose time-points are the difference of the corresponding time-points of timeseries A and B</returns>
    ''' <remarks>if the timeseries have different lengths,then the new length is floored to the length of the shortest timeseries</remarks>
    Public Shared Operator -(ByVal A As Timeseries, ByVal B As Timeseries) As Timeseries
        If A._Frequency <> B._Frequency Then Throw New PropertiesMismatchException("The timeseries must have the same frequency")

        Dim Aoff As Integer = B._Offset - Min(A._Offset, B._Offset)
        Dim Boff As Integer = A._Offset - Min(A._Offset, B._Offset)
        Dim NewLength = Min(A.Length - Aoff, B.Length - Boff)
        Dim ResultDataValues(NewLength - 1) As Double

        For i As Integer = 0 To NewLength - 1
            ResultDataValues(i) = A._DataPoints(i + Aoff) - B._DataPoints(i + Boff)
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, Max(A._Offset, B._Offset))
    End Operator



    ''' <summary>
    ''' subtracts a timeseries from a constant
    ''' </summary>
    ''' <param name="b">the constant that represents the minuend of the subtraction</param>
    ''' <param name="A">the timeseries that represents the subtrahend of the subtraction</param>
    ''' <returns>the timeseries whose time-points are the difference of the corresponding time-points of timeseries A and constant b</returns>
    Public Shared Operator -(ByVal b As Double, ByVal A As Timeseries) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = b - A._DataPoints(i)
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator

    ''' <summary>
    ''' subtracts a constant from a timeseries
    ''' </summary>
    ''' <param name="A">the timeseries that represents the minuend of the subtraction</param>
    ''' <param name="b">the constant that represents the subtrahend of the subtraction</param>
    ''' <returns>the timeseries whose time-points are the difference of the corresponding time-points of constant b and timeseries A</returns>
    Public Shared Operator -(ByVal A As Timeseries, ByVal b As Double) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = A._DataPoints(i) - b
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator

    ''' <summary>
    ''' calculates the product of a timeseries A and a timeseries B
    ''' </summary>
    ''' <param name="A">the timeseries that represents the first factor of the multiplication</param>
    ''' <param name="B">the timeseries that represents the second factor of the multiplication</param>
    ''' <returns>the timeseries whose time-points are the product of the corresponding time-points of timeseries A and timeseries B</returns>
    ''' <remarks>if the timeseries have different lengths,then the new length is floored to the length of the shortest timeseries</remarks>
    Public Shared Operator *(ByVal A As Timeseries, ByVal B As Timeseries) As Timeseries
        If A._Frequency <> B._Frequency Then Throw New PropertiesMismatchException("The timeseries must have the same frequency")

        Dim Aoff As Integer = B._Offset - Min(A._Offset, B._Offset)
        Dim Boff As Integer = A._Offset - Min(A._Offset, B._Offset)
        Dim NewLength = Min(A.Length - Aoff, B.Length - Boff)
        Dim ResultDataValues(NewLength - 1) As Double

        For i As Integer = 0 To NewLength - 1
            ResultDataValues(i) = A._DataPoints(i + Aoff) * B._DataPoints(i + Boff)
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, Max(A._Offset, B._Offset))
    End Operator

    ''' <summary>
    ''' calculates the product of a constant b and a timeseries A
    ''' </summary>
    ''' <param name="b">the constant that represents the first factor of the multiplication</param>
    ''' <param name="A">the timeseries that represents the second factor of the multiplication</param>
    ''' <returns>the timeseries whose time-points are the product of the corresponding time-points of constant b and timeseries A</returns>
    Public Shared Operator *(ByVal b As Double, ByVal A As Timeseries) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = b * A._DataPoints(i)
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator
    ''' <summary>
    ''' calculates the product of a timeseries A and a constant b
    ''' </summary>
    ''' <param name="A">the timeseries that represents the first factor of the multiplication</param>
    ''' <param name="b">the constant that represents the second factor of the multiplication</param>
    ''' <returns>the timeseries whose time-points are the product of the corresponding time-points of timeseries A and constant b</returns>
    Public Shared Operator *(ByVal A As Timeseries, ByVal b As Double) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = A._DataPoints(i) * b
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator
    ''' <summary>
    ''' divides timeseries A with a timeseries B
    ''' </summary>
    ''' <param name="A">the timeseries that represents the dividend of the division</param>
    ''' <param name="B">the timeseries that represents the divisor of the division</param>
    ''' <returns>the timeseries whose time-points are the quotient of the corresponding time-points of timeseries A and timeseries B</returns>
    ''' <remarks>if the timeseries have different lengths,then the new length is floored to the length of the shortest timeseries</remarks>
    Public Shared Operator /(ByVal A As Timeseries, ByVal B As Timeseries) As Timeseries
        If A._Frequency <> B._Frequency Then Throw New PropertiesMismatchException("The timeseries must have the same frequency")

        Dim Aoff As Integer = B._Offset - Min(A._Offset, B._Offset)                          'emfanizei th leksh apeiro an diairesw
        Dim Boff As Integer = A._Offset - Min(A._Offset, B._Offset)                          'me 0.kalutera na ginei me try-catch
        Dim NewLength = Min(A.Length - Aoff, B.Length - Boff)
        Dim ResultDataValues(NewLength - 1) As Double

        For i As Integer = 0 To NewLength - 1
            ResultDataValues(i) = A._DataPoints(i + Aoff) / B._DataPoints(i + Boff)
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, Max(A._Offset, B._Offset))
    End Operator

    ''' <summary>
    ''' divides a constant b with a timeseries A
    ''' </summary>
    ''' <param name="b">the constant that represents the dividend of the division</param>
    ''' <param name="A">the timeseries that represents the divisor of the division</param>
    ''' <returns>the timeseries whose time-points are the quotient of the corresponding time-points of constant b and timeseries A</returns>
    Public Shared Operator /(ByVal b As Double, ByVal A As Timeseries) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = b / A._DataPoints(i)
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator
    ''' <summary>
    ''' divides a timeseries A with a constant b
    ''' </summary>
    ''' <param name="A">the timeseries which represents the dividend of the division</param>
    ''' <param name="b">the constant that represents the divisor of the division</param>
    ''' <returns>the timeseries whose time-points are the quotient of the corresponding time-points of timeseries A and constant b</returns>
    Public Shared Operator /(ByVal A As Timeseries, ByVal b As Double) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = A._DataPoints(i) / b
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator

    ''' <summary>
    ''' raises timeseries A to power expo
    ''' </summary>
    ''' <param name="A">the timeseries that represents the base of the exponentiation</param>
    ''' <param name="expo">the constant that represents the exponent of the exponentiation</param>
    ''' <returns>the timeseries whose time-points are the exponentiation of the corresponding time-points of timeseries A and constant expo</returns>
    Public Shared Operator ^(ByVal A As Timeseries, ByVal expo As Double) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = A._DataPoints(i) ^ expo
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator
    ''' <summary>
    ''' raises timeseries A to (integer) power expo
    ''' </summary>
    ''' <param name="A">the timeseries that represents the base of the exponentiation</param>
    ''' <param name="expo">the constant that represents the exponent of the exponentiation</param>
    ''' <returns>the timeseries whose time-points are the exponentiation of the corresponding time-points of timeseries A and constant expo</returns>
    Public Shared Operator ^(ByVal A As Timeseries, ByVal expo As Integer) As Timeseries

        Dim ResultDataValues(A.Length - 1) As Double

        For i As Integer = 0 To A.Length - 1
            ResultDataValues(i) = A._DataPoints(i) ^ expo
        Next

        Return New Timeseries(ResultDataValues, A._Frequency, A._Offset)
    End Operator

    ''' <summary>
    ''' concatenates the additional time-points of a timeseries to the end of a shorter timeseries
    ''' </summary>
    ''' <param name="A">the timeseries to which the time-points will be concatenated</param>
    ''' <param name="B">the timeseries whose time-points will be concatenated</param>
    ''' <returns>a timeseries whose time-points are those of the shorter timeseries,concatenated with the additional time-points of the longer one</returns>
    ''' <remarks>if timeseries A is longer than timeseries B,then timeseries A is returned</remarks>
    Public Shared Operator &(ByVal A As Timeseries, ByVal B As Timeseries) As Timeseries ' NEEDS TESTING
        If A._Frequency <> B._Frequency Then Throw New PropertiesMismatchException("The timeseries must have the same frequency")

        Dim LenDif As Integer = (B._Offset + B.Length) - (A._Offset + A.Length)
        If LenDif < 1 Then
            Return A
        End If

        Dim Result(A.Length + LenDif - 1) As Double
        Array.Copy(A._DataPoints, Result, A.Length)
        Array.Copy(B._DataPoints, B.Length - LenDif, Result, A.Length, LenDif)
        Return New Timeseries(Result, A._Frequency, A._Offset)
    End Operator
#End Region

#Region "Statistic Functions"
    ''' <summary>
    ''' calculates the maximum time-point of the timeseries
    ''' </summary>
    ''' <returns>the value of the maximum time-point of the timeseries</returns>
    Public Function Maximum() As Double

        Dim i As Integer
        Dim Result As Double = _DataPoints(0)
        For i = 1 To Me.Length - 1
            If _DataPoints(i) > Result Then
                Result = _DataPoints(i)
            End If
        Next

        Return Result
    End Function
    ''' <summary>
    ''' calculates the minimum time-point of the timeseries
    ''' </summary>
    ''' <returns>the value of the minimum time-point of the timeseries</returns>
    Public Function Minimum() As Double

        Dim i As Integer
        Dim Result As Double = _DataPoints(0)
        For i = 1 To Me.Length - 1
            If _DataPoints(i) < Result Then
                Result = _DataPoints(i)
            End If
        Next

        Return Result
    End Function
    ''' <summary>
    ''' calculates the median of the timeseries
    ''' </summary>
    ''' <returns>the value of the median of the timeseries</returns>

    Public Function Median() As Double
        Dim TempArray() As Double = _DataPoints.Clone

        System.Array.Sort(TempArray)

        Dim LenMean As Integer = Floor((Me.Length) / 2)

        If (Me.Length Mod 2) = 0 Then
            Return (TempArray(Length() / 2 - 1) + TempArray(Length() / 2)) / 2
        Else
            Return TempArray(LenMean)
        End If
    End Function

    ''' <summary>
    ''' calculates the average of the time-points of the timeseries
    ''' </summary>
    ''' <returns>the value of the average of the timeseries</returns>
    Public Function Average() As Double
        Return Me.Sum / Me.Length
    End Function

    ''' <summary>
    ''' calculates the average of a subset of time-points of the timeseries
    ''' </summary>
    ''' <param name="len">the length of the subset of time-points of the timeseries</param>
    ''' <param name="start">the starting index of the subset of time-points of the timeseries</param>
    ''' <returns>the value of the average of the subset of time-points of the timeseries</returns>
    Public Function SubAverage(ByVal len As Integer, Optional ByVal start As Integer = 0) As Double 'len is the length of the interval whose the average we want to compute and start is the start of the interval
        Return Me.SubSum(len, start) / len
    End Function

    ''' <summary>
    ''' calculates the sum of the time-points of the timeseries
    ''' </summary>
    ''' <returns>the value of the sum of the time-series</returns>
    Public Function Sum() As Double
        Dim i As Integer
        Sum = 0
        For i = 0 To Me.Length - 1
            Sum = _DataPoints(i) + Sum
        Next
    End Function

    ''' <summary>
    ''' calculates the sum of a subset of time-points of a timeseries
    ''' </summary>
    ''' <param name="len">the length of the subset of time-points of the timeseries</param>
    ''' <param name="start">the starting index of the subset of time-points of the timeseries</param>
    ''' <returns>the value of the sum of the subset of time-points of the timeseries</returns>
    Public Function SubSum(ByVal len As Integer, Optional ByVal start As Integer = 0) As Double 'len is the length of the interval whose the average we want to compute and start is the start of the interval
        If (start + len > Me.Length) Then Throw New InvalidParameterException("The sum of input length and start index value must not exceed the timeseries' length")

        Dim Result As Double = 0

        For i As Integer = start To len + start - 1
            Result += _DataPoints(i)
        Next
        Return Result
    End Function

    ''' <summary>
    ''' calculates the standard deviation of the timeseries
    ''' </summary>
    ''' <returns>the value of the standard deviation of the timeseries</returns>
    Public Function StDev() As Double
        Return Sqrt(Variance)
    End Function

    ''' <summary>
    ''' calculates the variance of the timeseries
    ''' </summary>
    ''' <returns>the value of the variance of the timeseries</returns>
    Public Function Variance() As Double
        Return (Me ^ 2).Average - Average() ^ 2
    End Function

    ''' <summary>
    ''' calculates the covariance of a given timeseries with timeseries B
    ''' </summary>
    ''' <param name="B">the timeseries of which the covariance with the given timeseries is calculated</param>
    ''' <returns>the value of the covariance of the given timeseries with timeseries B</returns>
    Public Function Covariance(ByVal B As Timeseries) As Double
        If _Frequency <> B._Frequency Then Throw New PropertiesMismatchException("The timeseries must have the same frequency")
        Return (Me * B).Average - Average() * B.Average
    End Function

    ''' <summary>
    ''' calculates the correlation of the given timeseries with timeseries B
    ''' </summary>
    ''' <param name="B">the timeseries of which the correlation with the given timeseries is calculated</param>
    ''' <returns>the value of the correlation of the given timeseries with timeseries B</returns>
    Public Function rXY(ByVal B As Timeseries) As Double 'Prepei oi Xronoseires na einai idiou mikous kai offset
        Return Covariance(B) / (StDev() * B.StDev)     'mhpws na uparxei periorismos an dw8ei xronoseira diaforetikou mhkous?
    End Function

    ''' <summary>
    ''' calculates the auto-correlation of the timeseries
    ''' </summary>
    ''' <returns>the value of the auto-correlation of the timeseries</returns>
    Public Function ACF() As Double
        Dim avg As Double = Average()
        Dim sum As Double = 0

        For i As Integer = _Frequency To Me.Length - 1
            sum += (_DataPoints(i) - avg) * (_DataPoints(i - _Frequency) - avg)
        Next

        Return (sum / (Me.Length * Variance()))
    End Function

    ''' <summary>
    ''' calculates the auto-correlation of the timeseries
    ''' <paramref name="pos">The number of periods in a seasonality circle</paramref>
    ''' </summary>
    ''' <returns>the value of the auto-correlation of the timeseries</returns>
    Public Function ACF(ByVal pos As Integer) As Double
        Dim avg As Double = Average()
        Dim sum As Double = 0

        For i As Integer = pos To Me.Length - 1
            sum += (_DataPoints(i) - avg) * (_DataPoints(i - pos) - avg)
        Next

        Return (sum / (Me.Length * Variance()))
    End Function
    ''' <summary>
    ''' calculates the coefficient of variation of the timeseries
    ''' </summary>
    ''' <returns>the value of the coefficient of variation of the timeseries</returns>
    Public Function CoVar() As Double 'Coefficient of Variation
        Return 100 * (StDev() / Average())
    End Function

    ''' <summary>
    ''' calculates the Intermittent Demand Interval (the average interval between non-zero time-points) of the timeseries
    ''' </summary>
    ''' <returns>the value of the average interval between non-zero time-points of the timeseries</returns>
    Public Function IDIn() As Double
        Dim count As Integer = 0
        Dim sum As Integer = 0
        Dim interval As Integer = 1
        For i As Integer = 0 To Me.Length - 1
            If _DataPoints(i) = 0 Then
                interval += 1
            Else
                count += 1
                sum += interval
                interval = 1
            End If
        Next
        Return IIf(count <> 0, sum / count, 0)
    End Function

    ''' <summary>
    ''' calculates the growth rate of the timeseries
    ''' </summary>
    ''' <returns>the value of the growth rate of the timeseries</returns>
    Public Function GrowthRate() As Double
        If _Frequency = 0 Then Throw New PropertiesMismatchException("Timeseries' frequency must be a positive integer in order to calculate the growth rate")
        Return (SubAverage(_Frequency, Length() - _Frequency) / SubAverage(Length() - _Frequency)) - 1
    End Function

#End Region

#Region "Timeseries Functions"

    ''' <summary>
    ''' derives a new timeseries with a subset of the time-points of the original timeseries
    ''' </summary>
    ''' <param name="len">the length of the new timeseries</param>
    ''' <param name="start">the starting index from which the time-points of the new timeseries will be derived</param>
    ''' <returns>a sub-timeseries of the original timeseries</returns>
    ''' <remarks></remarks>
    Public Function SubTimeseries(ByVal len As Integer, Optional start As Integer = 0) As Timeseries
        If (start > Me.Length) Then Throw New InvalidParameterException("The start index value must not exceed the timeseries' length")

        Dim newlen = (Min(len, Me.Length - start)) ' Here, we are making sure that the derived subseries don't go beyond the original's last time-point
        Dim Result(newlen - 1) As Double

        Array.Copy(_DataPoints, start, Result, 0, newlen)
        'For i As Integer = 0 To newlen - 1
        '    Result(i) = _DataPoints(i + start)
        'Next
        Return New Timeseries(Result, _Frequency, _Offset + start)
    End Function
    ''' <summary>
    ''' calculates the absolute value of every time-point in the timeseries
    ''' </summary>
    ''' <returns>the timeseries whose time-points are the absolute values of the corresponding time-points of the original timeseries</returns>
    Public Function Absolute() As Timeseries
        Dim Result(Length() - 1) As Double
        For i As Integer = 0 To Length() - 1
            Result(i) = Abs(_DataPoints(i))
        Next
        Return New Timeseries(Result, _Frequency, _Offset)
    End Function

    ''' <summary>
    ''' calculates the difference between adjacent time-points of the timeseries
    ''' </summary>
    ''' <returns>a timeseries whose time-points are the differences of adjacent time-points of the timeseries</returns>
    Public Function Scale() As Timeseries
        Dim Result(Length() - 2) As Double
        For i As Integer = 0 To Length() - 2
            Result(i) = _DataPoints(i + 1) - _DataPoints(i)
        Next
        Return New Timeseries(Result, _Frequency, _Offset + 1)
    End Function

    ''' <summary>
    ''' Returns the reversed Timeseries
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Reverse() As Timeseries
        ' Return New Timeseries(DataValues.Reverse, _Frequency, Offset)
        Dim ReversedArray(Length() - 1) As Double
        For i As Integer = 0 To Length() - 1
            ReversedArray(i) = _DataPoints(Length() - 1 - i)
        Next

        Return New Timeseries(ReversedArray, _Frequency, _Offset)
    End Function

    ''' <summary>
    ''' Returns a String containing information about the Timeseries
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function toString() As String
        toString = "Length: " & Length & vbTab & "Frequency: " & _Frequency & vbTab & "Offset: " & _Offset & vbNewLine & "DataPoints:"
        For i As Integer = 0 To Length() - 1
            toString &= " " & _DataPoints(i)
        Next
    End Function

    ''' <summary>
    ''' Updates the value of Timeseries' Frequency
    ''' </summary>
    ''' <param name="NewFrequency"></param>
    ''' <returns>A new updated timeseries</returns>
    ''' <remarks></remarks>
    Public Function UpdateFrequency(ByVal NewFrequency As Integer) As Timeseries
        Return New Timeseries(_DataPoints, NewFrequency, _Offset)
    End Function

    ''' <summary>
    ''' Updates the value of Timeseries' Offset
    ''' </summary>
    ''' <param name="NewOffset"></param>
    ''' <returns>A new updated timeseries</returns>
    ''' <remarks></remarks>
    Public Function UpdateOffset(ByVal NewOffset As Integer) As Timeseries
        Return New Timeseries(_DataPoints, _Frequency, NewOffset)
    End Function

    ''' <summary>
    ''' Updates the value of Timeseries' Offset
    ''' </summary>
    ''' <param name="NewValue">The new value</param>
    ''' <param name="Position">The position that the new value should be updated</param>
    ''' <remarks></remarks>
    Public Sub UpdateValue(ByVal NewValue As Double, ByVal Position As Integer)
        _DataPoints(Position) = NewValue
    End Sub



    ''' <summary>
    ''' Returns the last DataPoint Value of the Timeseries
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LastValue() As Double
        Return _DataPoints.Last
    End Function

    ''' <summary>
    ''' Returns the first DataPoint Value of the Timeseries
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FirstValue() As Double
        Return _DataPoints.First
    End Function

    ''' <summary>
    ''' Rounds every element of the timeseries to the desired number of fractional digits
    ''' </summary>
    ''' <param name="Decimals"></param>
    ''' <returns>The rounded timeseries</returns>
    ''' <remarks></remarks>
    Public Function Round(Optional ByVal Decimals As Integer = 0) As Timeseries
        Dim Result(Length() - 1) As Double
        For i As Integer = 0 To Length() - 1
            Result(i) = Math.Round(_DataPoints(i), Decimals)
        Next
        Return New Timeseries(Result, _Frequency, _Offset)
    End Function

    ''' <summary>
    ''' Returns the number of datapoints that are equal to zero
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ZeroesCount() As Integer
        ZeroesCount = 0
        For i As Integer = 0 To Length() - 1
            If _DataPoints(i) = 0 Then
                ZeroesCount += 1
            End If
        Next
    End Function

    ''' <summary>
    ''' Returns the percentage of zero valued datapoints in the timeseries
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ZeroesPercentage() As Double
        Return ZeroesCount() / Length()
    End Function

    ' ''' <summary>
    ' ''' Creates a shallow copy of the Timeseries
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Function Clone() As Object
    '    Return New Timeseries(_DataPoints.Clone, _Frequency, _Offset)
    'End Function

    ''' <summary>
    ''' Returns if the timeseries has a significant seasonality behavior
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HasSeasonalityBehavior() As Boolean
        If _Frequency < 2 Then Return False

        Dim tcrit = 1.645
        Dim acfvalue = ACF()
        Dim limsum = 0
        For i As Integer = 2 To _Frequency - 1
            limsum += ACF(i) ^ 2
        Next
        limsum += ACF(1)
        Dim limit = tcrit * Math.Sqrt((1 + 2 * limsum) / Length)
        If Math.Abs(acfvalue) > limit Then
            Return True
        Else
            Return False
        End If

    End Function
#End Region




End Class