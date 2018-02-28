Public Module ForecastFactory

    Public Enum ForecastModel
        Naive
        LRL
        MovingAverage
        SES
        Holt
        Damped
        ThetaClassic
        Croston
        SBA
    End Enum

    Public Function GetModel(ByVal Model As ForecastModel) As Forecast
        Dim NewModel As Forecast
        Select Case (Model)
            Case ForecastModel.Naive
                NewModel = New Naive()
            Case ForecastModel.LRL
                NewModel = New LRL()
            Case ForecastModel.MovingAverage
                NewModel = New MovingAverage()
            Case ForecastModel.SES
                NewModel = New SES()
            Case ForecastModel.Holt
                NewModel = New Holt()
            Case ForecastModel.Damped
                NewModel = New Holt()
            Case ForecastModel.ThetaClassic
                NewModel = New ThetaClassic()
            Case ForecastModel.Croston
                NewModel = New Croston()
            Case ForecastModel.SBA
                NewModel = New SBA()
            Case Else
                Throw New InvalidParameterException("Not a known Model")
        End Select

        Return NewModel
    End Function

    Public Function GetModel(ByVal Model As ForecastModel, ByVal Input As Timeseries) As Forecast
        Dim NewModel As Forecast
        Select Case (Model)
            Case ForecastModel.Naive
                NewModel = New Naive(Input)
            Case ForecastModel.LRL
                NewModel = New LRL(Input)
            Case ForecastModel.MovingAverage
                NewModel = New MovingAverage(Input)
            Case ForecastModel.SES
                NewModel = New SES(Input)
            Case ForecastModel.Holt
                NewModel = New Holt(Input)
            Case ForecastModel.Damped
                NewModel = New Damped(Input)
            Case ForecastModel.ThetaClassic
                NewModel = New ThetaClassic(Input)
            Case ForecastModel.Croston
                NewModel = New Croston(Input)
            Case ForecastModel.SBA
                NewModel = New SBA(Input)
            Case Else
                Throw New InvalidParameterException("Not a known Model")
        End Select

        Return NewModel
    End Function

End Module
