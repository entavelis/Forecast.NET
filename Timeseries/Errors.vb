' ******************************************~V~*******************************************
'
' Description: A class containing the basic error operations upon timeseries
' Author: Evangelos Ntavelis
' Email: vaggelis@fsu.gr
' Date: 25/7/2014
'
' ******************************************~V~*******************************************
Imports System.Math

Public Class Errors
    Delegate Function ErrorMethodForOptimization(ByVal Input As Timeseries, ByVal Forecast As Timeseries) As Double

#Region "Private Functions"

    ''' <summary>
    ''' Percentage Error
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>The difference between the two timeseries as a percentage</returns>
    ''' <remarks></remarks>

    Private Shared Function p(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Timeseries
        Return 100 * e(Original, Derived) / Original
    End Function

    'Private Shared Function r(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Timeseries
    '    Return e(Original, Derived) / e(Original, Original.Forecast.Naive)
    'End Function
    ''' <summary>
    ''' Absolute Scaled Error
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>The absolute scaled difference between the two timeseries</returns>
    ''' <remarks></remarks>
    Private Shared Function q(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Timeseries
        Return e(Original, Derived) / Original.Scale.Average
    End Function
#End Region

    ''' <summary>
    ''' Error
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>The difference between the two timeseries</returns>
    ''' <remarks></remarks>
    Public Shared Function e(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Timeseries
        Return Original - Derived
    End Function
    ''' <summary>
    '''  calculates the mean error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the mean error of the derived timeseries compared to the original timeseries</returns>
    Public Shared Function MeanError(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return e(Original, Derived).Average
    End Function
    ''' <summary>
    ''' calculates the mean squared error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the mean squared error of the derived timeseries compared to the original timeseries</returns>
    Public Shared Function MSE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return (e(Original, Derived) ^ 2).Average
    End Function
    ''' <summary>
    ''' calculates the root mean squared error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the root mean squared error of the derived timeseries compared to the original timeseries</returns>
    Public Shared Function RMSE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return Sqrt(MSE(Original, Derived))
    End Function
    ''' <summary>
    ''' calculates the mean absolute error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the mean absolute error of the derived timeseries compared to the original timeseries</returns>
    ''' <remarks></remarks>
    Public Shared Function MAE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return e(Original, Derived).Absolute.Average
    End Function
    ''' <summary>
    ''' calculates the median of the mean absolute error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the median of the mean absolute error of the derived timeseries compared to the original timeseries</returns>
    Public Shared Function MdAE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return e(Original, Derived).Absolute.Median
    End Function
    ''' <summary>
    ''' calculates the mean absolute percentage error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the mean absolute percentage error of the derived timeseries compared to the original timeseries</returns>
    Public Shared Function MAPE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return p(Original, Derived).Absolute.Average
    End Function
    ''' <summary>
    ''' calculates the median of the mean absolute percentage error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the median of the mean absolute percentage error of the derived timeseries compared to the original timeseries</returns>
    Public Shared Function MdAPE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return p(Original, Derived).Absolute.Median
    End Function
    ''' <summary>
    ''' calculates the symmetric mean absolute percentage error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the symmetric mean absolute percentage error of the derived timeseries compared to the original timeseries</returns>
    ''' <remarks></remarks>
    Public Shared Function sMAPE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return 100 * (2 * (Original - Derived) / (Original + Derived)).Absolute.Average
    End Function
    ''' <summary>
    '''  calculates the median of the symmetric mean absolute percentage error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the median of the symmetric mean absolute percentage error of the derived timeseries compared to the original timeseries</returns>
    Public Shared Function sMdAPE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return 100 * (2 * (Original - Derived) / (Original + Derived)).Absolute.Median
    End Function

    'Public Shared Function MRAE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
    '    Return r(Original, Derived).Absolute.Average
    'End Function

    'Public Shared Function MdAE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
    '    Return r(Original, Derived).Absolute.Median
    'End Function

    'Public Shared Function GMRAE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
    '    Return r(Original, Derived).Absolute.GAverage
    'End Function
    ''' <summary>
    ''' calculates the base 10 logarithm of the mean squared error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the base 10 logarithm of the mean squared error of the derived timeseries compared to the original timeseries</returns>
    Public Shared Function LMSE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return Log10((e(Original, Derived) ^ 2).Average)
    End Function
    ''' <summary>
    ''' calculates the mean absolute scaled error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the mean absolute scaled error of the derived timeseries compared to the original timeseries</returns>
    ''' <remarks></remarks>
    Public Shared Function MAsE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return q(Original, Derived).Absolute.Average
    End Function
    ''' <summary>
    ''' calculates the median of the mean absolute scaled error of the derived timeseries compared to the original timeseries
    ''' </summary>
    ''' <param name="Original">The original timeseries</param>
    ''' <param name="Derived">The derived from the original timeseries</param>
    ''' <returns>the value of the median of the mean absolute scaled error of the derived timeseries compared to the original timeseries</returns>
    ''' <remarks></remarks>
    Public Shared Function MdAsE(ByVal Original As Timeseries, ByVal Derived As Timeseries) As Double
        Return q(Original, Derived).Absolute.Median
    End Function
End Class
