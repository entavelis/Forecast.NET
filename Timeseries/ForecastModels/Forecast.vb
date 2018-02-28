' ******************************************~V~*******************************************
'
' Description: The class ForecastMethod contains the basic information of the forecast model 
' Author: Evangelos Ntavelis
' Email: vaggelis@fsu.gr
' Date: 25/7/2014
'
' ******************************************~V~******************************************
Imports Timeseries.Errors

Public MustInherit Class Forecast
    Inherits Timeseries


#Region "Private Variables"
    Const MyMethodName As String = ""
    Protected _ErrorMethod As Errors.ErrorMethodForOptimization

    Protected _Horizon As Integer
    Protected _Original As Timeseries
    Protected _Parameters As Dictionary(Of String, Double)
    Protected _MethodName As String
#End Region

#Region "Properties"

    Public Property Horizon As Integer
        Get
            Return _Horizon
        End Get
        Set(value As Integer)
            _Horizon = value
        End Set
    End Property

    Public Property Original As Timeseries
        Get
            Return _Original
        End Get
        Set(value As Timeseries)
            _Original = Original
        End Set
    End Property

    Public Property Parameters As Dictionary(Of String, Double)
        Get
            Return _Parameters
        End Get
        Set(value As Dictionary(Of String, Double))
            _Parameters = value
        End Set
    End Property

    Public Property MethodName As String
        Get
            Return _MethodName
        End Get
        Set(value As String)
            _MethodName = value
        End Set
    End Property

    Public WriteOnly Property ErrorMethod As Errors.ErrorMethodForOptimization
        Set(value As Errors.ErrorMethodForOptimization)
            _ErrorMethod = value
        End Set
    End Property

    Public ReadOnly Property InSample As Timeseries
        Get
            Return Me.SubTimeseries(_Original.Length + _Original.Offset - _Offset)
        End Get
    End Property

    Public ReadOnly Property OutOfSample As Timeseries
        Get
            Return Me.SubTimeseries(_Horizon, _Original.Length + _Original.Offset - _Offset)
        End Get
    End Property

    Public ReadOnly Property UpperConfidenceLevel As Timeseries
        Get
            Return ConfidenceLevels(0.95).Item1
        End Get
    End Property
    Public ReadOnly Property LowerConfidenceLevel As Timeseries
        Get
            Return ConfidenceLevels(0.95).Item2
        End Get
    End Property
    Public ReadOnly Property MeanError As Double
        Get
            Return Errors.MeanError(_Original, Me)
        End Get
    End Property

    Public ReadOnly Property MSE As Double
        Get
            Return Errors.MSE(_Original, Me)
        End Get
    End Property


    Public ReadOnly Property RMSE As Double
        Get
            Return Errors.RMSE(_Original, Me)
        End Get
    End Property

    Public ReadOnly Property MAE As Double
        Get
            Return Errors.MAE(_Original, Me)
        End Get
    End Property

    Public ReadOnly Property MdAE As Double
        Get
            Return Errors.MdAE(_Original, Me)
        End Get
    End Property


    Public ReadOnly Property MAPE As Double
        Get
            Return Errors.MAPE(_Original, Me)
        End Get
    End Property



    Public ReadOnly Property MdAPE As Double
        Get
            Return Errors.MdAPE(_Original, Me)
        End Get
    End Property


    Public ReadOnly Property sMAPE As Double
        Get
            Return Errors.sMAPE(_Original, Me)
        End Get
    End Property


    Public ReadOnly Property sMdAPE As Double
        Get
            Return Errors.sMdAPE(_Original, Me)
        End Get
    End Property


    Public ReadOnly Property LMSE As Double
        Get
            Return Errors.LMSE(_Original, Me)
        End Get
    End Property


    Public ReadOnly Property MAsE As Double
        Get
            Return Errors.MAsE(_Original, Me)
        End Get
    End Property


    Public ReadOnly Property MdAsE As Double
        Get
            Return Errors.MdAsE(_Original, Me)
        End Get
    End Property


#End Region

#Region "Constructors"
    Public Sub New()
        _Original = New Timeseries()
        _MethodName = MyMethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = AddressOf Errors.MSE
    End Sub

    Public Sub New(ByVal Input As Timeseries)
        _Original = Input
        _MethodName = MyMethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = AddressOf Errors.MSE
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal ErrorMethod As ErrorMethodForOptimization)
        _Original = Input
        _MethodName = MyMethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = ErrorMethod
    End Sub


    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double)
        _Original = Input
        _Horizon = Horizon
        _MethodName = MyMethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = AddressOf Errors.MSE
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double, ByVal ErrorMethod As ErrorMethodForOptimization)
        _Original = Input
        _Horizon = Horizon
        _MethodName = MyMethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = ErrorMethod
    End Sub


    Public Sub New(ByVal MethodName As String)
        _MethodName = MethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = AddressOf Errors.MSE
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal MethodName As String)
        _Original = Input
        _MethodName = MethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = AddressOf Errors.MSE
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double, ByVal MethodName As String)
        _Original = Input
        _Horizon = Horizon
        _MethodName = MethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = AddressOf Errors.MSE
    End Sub

    Public Sub New(ByVal Input As Timeseries, ByVal Horizon As Double, ByVal ErrorMethod As ErrorMethodForOptimization, ByVal MethodName As String)
        _Original = Input
        _Horizon = Horizon
        _MethodName = MethodName
        _Parameters = New Dictionary(Of String, Double)
        _ErrorMethod = ErrorMethod
    End Sub


    Public Sub New(ByVal Input As Forecast)
        _Original = New Timeseries(Input.Original)
        _Horizon = Input.Horizon
        _MethodName = Input.MethodName
        _Parameters = Input.Parameters
        _ErrorMethod = AddressOf Errors.MSE
    End Sub

#End Region

#Region "Public Functions"
    Public MustOverride Sub Calculate()

    Public MustOverride Sub OptimizeParameters()

    Public MustOverride Function MyType() As ForecastFactory.ForecastModel


    Public Sub OptimizeClear()
        _Parameters.Clear()
        OptimizeParameters()
    End Sub



    ''' <summary>
    ''' Returns a String containing information about the Forecast Model
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function toString() As String
        '' MUST BE CHECKED!!!!!!!!

        toString = "Method: " & _MethodName & vbNewLine & "Timeseries:" & vbNewLine & MyBase.toString() & vbNewLine & vbNewLine & "Parameters:"
        For Each par In _Parameters
            toString &= vbNewLine & par.Key & ":" & vbTab & par.Value
        Next

    End Function

    Public Function Clone() As Forecast
        Clone = ForecastFactory.GetModel(MyType, _Original)
        Clone.Horizon = _Horizon
        Clone.DataPoints = _DataPoints ' .Clone
        Clone.Offset = _Offset
        Clone.Parameters = _Parameters
        Clone.MethodName = _MethodName
    End Function

    Public Function ConfidenceLevels(Optional ByVal Level As Double = 0.95) As Tuple(Of Timeseries, Timeseries)
        Dim rm = Errors.RMSE(_Original, Me)

        Dim tcrit As Double

        Select Case Level
            Case 0.99
                tcrit = 2.58
            Case 0.98
                tcrit = 2.33
            Case 0.95
                tcrit = 1.96
            Case 0.9
                tcrit = 1.645
            Case 0.8
                tcrit = 1.28
            Case Else
                Throw New InvalidParameterException("Level is not in the list of supported values (0.8-0.9-0.95-0.98-0.99)")
        End Select
        'Below this sentence we calculate a timeseries corresponding to the part of sqrt(i-n) of the Confidence levels formula
        Dim rootiminusn = DummyTimeseries.SteppedByOne(1, _Horizon, _Frequency, _Original.Length + _Original.Offset) ^ (1 / 2)

        Dim Up = Me + tcrit * rm * rootiminusn
        Dim Down = Me - tcrit * rm * rootiminusn

        Return Tuple.Create(Of Timeseries, Timeseries)(Up, Down)

    End Function

#End Region

#Region "Private Functions"
#End Region

End Class
