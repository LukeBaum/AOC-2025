Imports System
Imports System.IO
Imports System.Net.Security
Imports System.Text.RegularExpressions

Module Program
    Sub Main(args As String())

        REM Overly Complicated, but it works... no need for the Enum or Class, but why not make it as Visual Basic as possible?

        Dim MathHomework As List(Of String) = File.ReadAllLines(args(0)).ToList()

        Dim Answers As New List(Of MathProblem)

        Dim NumericRowCount As Integer = MathHomework.Count - 1

        Dim Numbers As New List(Of Long)

        For Position As Integer = MathHomework.Item(0).Length - 1 To 0 Step -1

            Dim NumberString As String = ""
            For i As Integer = 0 To NumericRowCount - 1
                NumberString &= MathHomework.Item(i).Substring(Position, 1)
            Next

            Console.WriteLine(NumberString)

            NumberString = NumberString.Replace(" ", "")
            If NumberString.Length = 0 Then Continue For

            Numbers.Add(CLng(NumberString))

            Select Case MathHomework.Last.Substring(Position, 1)
                Case Is = " "
                    Continue For
                Case Is = "+"
                    Answers.Add(New MathProblem With {.Answer = Numbers.Sum})
                Case Is = "*"
                    Answers.Add(New MathProblem(True))
                    For Each Number As Long In Numbers
                        Answers.Last.PerformOperation(Number)
                    Next
            End Select

            Numbers.Clear()

        Next

        Dim FinalAnswer As Long = 0
        For Each Problem As MathProblem In Answers
            FinalAnswer += Problem.Answer
        Next

        Console.WriteLine($"Total homework sum: {CStr(FinalAnswer)}")

    End Sub

    Public Enum Operation
        Add = 0
        Multiply = 1
    End Enum

    Public Class MathProblem
        Public Property Answer As Long = 0
        Public Property Operation As Operation = Operation.Add
        Public Sub PerformOperation(ByVal Number As Long)
            If Me.Operation = Operation.Add Then
                Me.Answer += Number
                Exit Sub
            End If
            If Me.Answer = 0 Then
                Me.Answer = Number
                Exit Sub
            End If
            Me.Answer *= Number
        End Sub

        Sub New(Optional ByVal Multiply As Boolean = False)
            If Multiply Then Me.Operation = Operation.Multiply
        End Sub

    End Class

End Module
