Public Module DummyTimeseries
    ''' <summary>
    ''' Creates a timeseries with all its DataPoints assigned to one.
    ''' </summary>
    ''' <param name="Length">The length of the newly created timeseries</param>
    ''' <param name="FrequencyValue">The frequency of the newly created timeseries</param>
    ''' <param name="OffsetValue">The offset of the newly created timeseries</param>
    ''' <returns>Returns the created Timeseries</returns>
    ''' <remarks></remarks>
    Public Function Ones(ByVal Length As Integer, Optional ByVal FrequencyValue As Integer = 1, Optional ByVal OffsetValue As Integer = 0) As Timeseries
        Dim InputDataValues(Length - 1) As Double
        For i As Integer = 0 To Length - 1
            InputDataValues(i) = 1
        Next

        Return New Timeseries(InputDataValues, FrequencyValue, OffsetValue)
    End Function

    ''' <summary>
    ''' Creates a timeseries with all its DataPoints assigned to zero.
    ''' </summary>
    ''' <param name="Length">The length of the newly created timeseries</param>
    ''' <param name="FrequencyValue">The frequency of the newly created timeseries</param>
    ''' <param name="OffsetValue">The offset of the newly created timeseries</param>
    ''' <returns>Returns the created Timeseries</returns>
    ''' <remarks></remarks>
    Public Function Zeroes(ByVal Length As Integer, Optional ByVal FrequencyValue As Integer = 1, Optional ByVal OffsetValue As Integer = 0) As Timeseries
        Dim InputDataValues(Length - 1) As Double
        For i As Integer = 0 To Length - 1
            InputDataValues(i) = 0
        Next

        Return New Timeseries(InputDataValues, FrequencyValue, OffsetValue)
    End Function

    ''' <summary>
    ''' Creates a timeseries with all its DataPoints assigned to a sequence starting at StartValue, ending before or at EndValue, with StepValue as step .
    ''' </summary>
    ''' <param name="StartValue">The starting value of the newly created timeseries</param>
    ''' <param name="StepValue">The value of the step of the newly created timeseries</param>
    ''' <param name="EndValue">The ending value of the newly created timeseries</param>
    ''' <param name="FrequencyValue">The frequency of the newly created timeseries</param>
    ''' <param name="OffsetValue">The offset of the newly created timeseries</param>
    ''' <returns>Returns the created Timeseries</returns>
    ''' <remarks></remarks>
    Public Function Stepped(ByVal StartValue As Double, ByVal StepValue As Double, ByVal EndValue As Double, Optional ByVal FrequencyValue As Integer = 1, Optional ByVal OffsetValue As Integer = 0) As Timeseries
        If EndValue < StartValue Then Throw New InvalidParameterException("EndValue must be greater than or equal to the StartValue")
        Dim Length As Integer = ((EndValue - StartValue) / StepValue) + 1
        Dim InputDataValues(Length - 1) As Double

        InputDataValues(0) = StartValue
        For i As Integer = 1 To Length - 1
            InputDataValues(i) = InputDataValues(i - 1) + StepValue
        Next

        Return New Timeseries(InputDataValues, FrequencyValue, OffsetValue)
    End Function

    ''' <summary>
    ''' Creates a timeseries with all its DataPoints assigned to a sequence starting at StartValue, ending before or at EndValue, with a step value of one .
    ''' </summary>
    ''' <param name="StartValue">The starting value of the newly created timeseries</param>
    ''' <param name="EndValue">The ending value of the newly created timeseries</param>
    ''' <param name="FrequencyValue">The frequency of the newly created timeseries</param>
    ''' <param name="OffsetValue">The offset of the newly created timeseries</param>
    ''' <returns>Returns the created Timeseries</returns>
    ''' <remarks></remarks>
    Public Function SteppedByOne(ByVal StartValue As Double, ByVal EndValue As Double, Optional FrequencyValue As Integer = 1, Optional OffsetValue As Integer = 0) As Timeseries
        Return Stepped(StartValue, 1, EndValue, FrequencyValue, OffsetValue)
    End Function
End Module
