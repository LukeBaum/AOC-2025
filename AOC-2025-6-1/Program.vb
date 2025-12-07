Imports System
Imports System.IO
Imports System.Text.RegularExpressions

Module Program
    Sub Main(args As String())

        Dim MathHomework As List(Of String) = File.ReadAllLines(args(0)).ToList()
        MathHomework.Reverse()
        MathHomework.Item(0) = Regex.Replace(MathHomework.First, "\s{2,}", " ")

        Dim Answers As New List(Of MathProblem)

        For Each Operation As String In MathHomework.Item(0).Split(" ")
            Dim Problem As New MathProblem
            Select Case Operation
                Case Is = "*"
                    Problem.Operation = Program.Operation.Multiply
                Case Is <> "+"
                    Continue For
            End Select
            Answers.Add(Problem)
        Next

        MathHomework.RemoveAt(0)

        Console.WriteLine(Answers.Count)


        For Each Row As String In MathHomework
            Row = Regex.Replace(Row, "\s{2,}", " ")
            Dim NumStrings() As String = Row.Split(" ")
            For i As Integer = 0 To NumStrings.Length - 1
                Answers.Item(i).PerformOperation(CLng(NumStrings(i)))
            Next
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

    End Class

End Module
