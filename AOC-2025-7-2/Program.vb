Imports System
Imports System.IO
Imports System.Collections.Generic

Module Program
    Sub Main(args As String())

        Dim Strings() As String = File.ReadAllLines(args(0))

        Dim PlinkoWidth As Integer = Strings(0).Length
        Dim PlinkoHeight As Integer = Strings.Count

        Dim PlinkoBoard(PlinkoWidth - 1, PlinkoHeight - 1) As Long

        For x As Integer = 0 To PlinkoWidth - 1
            For y As Integer = 0 To PlinkoHeight - 1
                PlinkoBoard(x, y) = 0
            Next
        Next

        PlinkoBoard(Strings(0).IndexOf("S"), 0) = 1

        For y As Integer = 0 To PlinkoHeight - 2

            For x As Integer = 1 To PlinkoWidth - 2

                If PlinkoBoard(x, y) > 0 Then

                    If Strings(y + 1).Substring(x, 1) = "." Then
                        PlinkoBoard(x, y + 1) += PlinkoBoard(x, y)
                    ElseIf Strings(y + 1).Substring(x, 1) = "^" Then
                        PlinkoBoard(x - 1, y + 1) += PlinkoBoard(x, y)
                        PlinkoBoard(x + 1, y + 1) += PlinkoBoard(x, y)
                    End If

                End If

            Next

        Next

        Dim Timelines As Long = 0
        For x As Integer = 0 To PlinkoWidth - 1
            Timelines += PlinkoBoard(x, PlinkoHeight - 2)
        Next

        Console.WriteLine($"Total timeslines: {CStr(Timelines)}")

    End Sub

End Module