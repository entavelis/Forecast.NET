
' ******************************************~V~*******************************************
'
' Description: An exception class that handles the call of functions with parameters that
' do not match
' Author: Evangelos Ntavelis
' Email: vaggelis@fsu.gr
' Date: 25/7/2014
'
' ******************************************~V~*******************************************

Public Class PropertiesMismatchException
    Inherits Exception

    ''' <summary>
    ''' Creates an exception that handles the call of functions with parameters that do not match 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Creates an exception that handles the call of functions with parameters that do not match 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    ''' <summary>
    ''' Creates an exception that handles the call of functions with parameters that do not match 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

