' ******************************************~V~*******************************************
'
' Description: An exception class that handles the call of functions with invalid 
' parameters
' Author: Evangelos Ntavelis
' Email: vaggelis@fsu.gr
' Date: 25/7/2014
'
' ******************************************~V~*******************************************

Public Class InvalidParameterException
    Inherits Exception

    ''' <summary>
    ''' Creates an exception that handles the call of functions with invalid 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Creates an exception that handles the call of functions with invalid 
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    ''' <summary>
    ''' Creates an exception that handles the call of functions with invalid 
    ''' </summary>
    ''' <param name="message"></param>
    ''' <param name="inner"></param>
    ''' <remarks></remarks>
    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

